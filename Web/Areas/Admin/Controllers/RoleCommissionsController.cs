using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLayer.Context;
using DataLayer.Entities.User;
using Core.Services.Interfaces;
using DataLayer.Entities.ComplementaryInfo;
using Microsoft.AspNetCore.Authorization;
using Core.Security;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RoleCommissionsController : Controller
    {

        private readonly IUserService _userService;


        public RoleCommissionsController(IUserService userService)
        {

            _userService = userService;
        }

        // GET: Admin/RoleCommissions
        [PermissionChecker(64)]
        public async Task<IActionResult> Index()
        {

            var list = await _userService.GetRoleCommissionsAsync();
            return View(list);
        }

        // GET: Admin/RoleCommissions/Details/5
        [PermissionChecker(67)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleCommission = await _userService.GetRoleCommissionByIdAsync((int)id);
            if (roleCommission == null)
            {
                return NotFound();
            }

            return View(roleCommission);
        }

        // GET: Admin/RoleCommissions/Create
        [PermissionChecker(65)]
        public async Task<IActionResult> Create()
        {
            ViewData["RoleId"] = new SelectList(await _userService.GetAllRolesAsync(), "RoleId", "RoleTitle");
            return View();
        }

        // POST: Admin/RoleCommissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(65)]
        public async Task<IActionResult> Create(RoleCommission roleCommission)
        {
            if (!ModelState.IsValid)
            {
                ViewData["RoleId"] = new SelectList(await _userService.GetRoleCommissionsAsync(), "RoleId", "RoleTitle", roleCommission.RoleId);
                return View(roleCommission);
            }
            roleCommission.OP_Create = User.Identity.Name;
            roleCommission.CreateDate = DateTime.Now;
            _userService.CreateRoleCommission(roleCommission);
            await _userService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/RoleCommissions/Edit/5
        [PermissionChecker(66)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleCommission = await _userService.GetRoleCommissionByIdAsync((int)id);
            if (roleCommission == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(await _userService.GetAllRolesAsync(), "RoleId", "RoleTitle", roleCommission.RoleId);
            return View(roleCommission);
        }

        // POST: Admin/RoleCommissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(66)]
        public async Task<IActionResult> Edit(int id, RoleCommission roleCommission)
        {
            if (id != roleCommission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _userService.GetRoleByIdAsync(roleCommission.RoleId);
                    roleCommission.Role = role;
                    RoleCommission roleCommissionOld = await _userService.GetRoleCommissionByIdAsync(id);
                    string res = _userService.CompareTwoEntity(roleCommission, roleCommissionOld);
                    _userService.DoDetached(roleCommissionOld);
                    _userService.UpdateRoleCommission(roleCommission);
                    if(!string.IsNullOrEmpty(res))
                    {
                        ChangeLog changeLog = new ChangeLog()
                        {
                            ChangedBy = User.Identity.Name,
                            DateChanged = DateTime.Now,
                            EntityName = "RoleCommission",
                            PrimaryKeyValue = id.ToString(),
                            Description = res
                        };
                        _userService.CreateChangeLog(changeLog);
                    }
                    


                    await _userService.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleCommissionExists(roleCommission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(await _userService.GetAllRolesAsync(), "RoleId", "RoleTitle", roleCommission.RoleId);
            return View(roleCommission);
        }

        // GET: Admin/RoleCommissions/Delete/5
        [PermissionChecker(68)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleCommission = await _userService.GetRoleCommissionByIdAsync((int)id);

            if (roleCommission == null)
            {
                return NotFound();
            }

            return View(roleCommission);
        }

        // POST: Admin/RoleCommissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionChecker(68)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roleCommission = await _userService.GetRoleCommissionByIdAsync(id);
            roleCommission.IsDeleted = true;
            roleCommission.OP_FakeRemove = User.Identity.Name;
            roleCommission.RemoveDate = DateTime.Now;
            _userService.UpdateRoleCommission(roleCommission);
            await _userService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleCommissionExists(int id)
        {
            return _userService.RoleCommissionExist(id);
        }
    }
}
