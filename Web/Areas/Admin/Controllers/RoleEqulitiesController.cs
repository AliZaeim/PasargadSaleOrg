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
    public class RoleEqulitiesController : Controller
    {
        

        private readonly IUserService _userService;
        public RoleEqulitiesController(IUserService userService)
        {
            _userService = userService;
            
        }

        // GET: Admin/RoleEqulities
        [PermissionChecker(69)]
        public async Task<IActionResult> Index()
        {
            var list =await _userService.GetRoleEqulitysAsync();
            return View(list);
        }

        // GET: Admin/RoleEqulities/Details/5
        [PermissionChecker(72)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleEqulity = await _userService.GetRoleEqulityByIdAsync((int)id);
            if (roleEqulity == null)
            {
                return NotFound();
            }

            return View(roleEqulity);
        }

        // GET: Admin/RoleEqulities/Create
        [PermissionChecker(70)]
        public async Task<IActionResult> Create()
        {
            ViewData["RoleId"] = new SelectList(await _userService.GetAllRolesAsync(), "RoleId", "RoleTitle");
            return View();
        }

        // POST: Admin/RoleEqulities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(70)]
        public async Task<IActionResult> Create(RoleEqulity roleEqulity)
        {
            if (ModelState.IsValid)
            {
                _userService.CreateRoleEqulity(roleEqulity);
                await _userService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(await _userService.GetAllRolesAsync(), "RoleId", "RoleTitle", roleEqulity.RoleId);
            return View(roleEqulity);
        }

        // GET: Admin/RoleEqulities/Edit/5
        [PermissionChecker(71)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleEqulity = await _userService.GetRoleEqulityByIdAsync((int)id);
            if (roleEqulity == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(await _userService.GetAllRolesAsync(), "RoleId", "RoleTitle", roleEqulity.RoleId);
            return View(roleEqulity);
        }

        // POST: Admin/RoleEqulities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(71)]
        public async Task<IActionResult> Edit(int id,RoleEqulity roleEqulity)
        {
            if (id != roleEqulity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _userService.GetRoleByIdAsync(roleEqulity.RoleId);
                    roleEqulity.Role = role;
                    RoleEqulity roleEqulityOld = await _userService.GetRoleEqulityByIdAsync(id);
                    string res = _userService.CompareTwoEntity(roleEqulity, roleEqulityOld);
                    _userService.DoDetached(roleEqulityOld);
                    _userService.UpdateRoleEqulity(roleEqulity);
                    if (!string.IsNullOrEmpty(res))
                    {
                        ChangeLog changeLog = new ChangeLog()
                        {
                            ChangedBy = User.Identity.Name,
                            DateChanged = DateTime.Now,
                            EntityName = "RoleEqulity",
                            PrimaryKeyValue = id.ToString(),
                            Description = res
                        };
                        _userService.CreateChangeLog(changeLog);
                    }
                    await _userService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleEqulityExists(roleEqulity.Id))
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
            ViewData["RoleId"] = new SelectList(await _userService.GetAllRolesAsync(), "RoleId", "RoleTitle", roleEqulity.RoleId);
            return View(roleEqulity);
        }

        // GET: Admin/RoleEqulities/Delete/5
        [PermissionChecker(73)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleEqulity = await _userService.GetRoleEqulityByIdAsync((int)id);
            if (roleEqulity == null)
            {
                return NotFound();
            }

            return View(roleEqulity);
        }

        // POST: Admin/RoleEqulities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionChecker(73)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roleEqulity = await _userService.GetRoleEqulityByIdAsync(id);
            roleEqulity.IsDeleted = true;
            roleEqulity.OP_FakeRemove = User.Identity.Name;
            roleEqulity.RemoveDate = DateTime.Now;            
            _userService.UpdateRoleEqulity(roleEqulity);
            await _userService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleEqulityExists(int id)
        {
            return _userService.RoleEqulityExist(id);
        }
    }
}
