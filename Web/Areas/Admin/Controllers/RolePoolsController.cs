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
    public class RolePoolsController : Controller
    {

        private readonly IUserService _userServcie;
        public RolePoolsController(IUserService userService)
        {
            _userServcie = userService;

        }

        // GET: Admin/RolePools
        [PermissionChecker(74)]
        public async Task<IActionResult> Index()
        {

            return View(await _userServcie.GetRolePoolsAsync());
        }

        // GET: Admin/RolePools/Details/5
        [PermissionChecker(77)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePool = await _userServcie.GetRolePoolByIdAsync((int)id);
            if (rolePool == null)
            {
                return NotFound();
            }

            return View(rolePool);
        }

        // GET: Admin/RolePools/Create
        [PermissionChecker(75)]
        public async Task<IActionResult> Create()        {
            
            List<Role> roles = await _userServcie.GetAllRolesAsync();            
            ViewData["Roles"] = roles.ToList();
            return View();
        }

        // POST: Admin/RolePools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(75)]
        public async Task<IActionResult> Create(RolePool rolePool)
        {

            if (!ModelState.IsValid)
            {
                List<Role> roles = await _userServcie.GetAllRolesAsync();
                ViewData["Roles"] = roles.ToList();
                return View(rolePool);

            }
            _userServcie.CreateRolePool(rolePool);
            await _userServcie.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }

        // GET: Admin/RolePools/Edit/5
        [PermissionChecker(76)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePool = await _userServcie.GetRolePoolByIdAsync((int)id);
            if (rolePool == null)
            {
                return NotFound();
            }
            List<Role> roles = await _userServcie.GetAllRolesAsync();
            ViewData["Roles"] = roles.ToList();
            return View(rolePool);
        }

        // POST: Admin/RolePools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(76)]
        public async Task<IActionResult> Edit(int id, RolePool rolePool)
        {
            if (id != rolePool.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (rolePool.RoleId != null)
                    {
                        var role = await _userServcie.GetRoleByIdAsync((int)rolePool.RoleId);
                        rolePool.Role = role;
                    }
                    else
                    {
                        rolePool.RoleId = null;
                    }
                    
                    RolePool rolepoolOld = await _userServcie.GetRolePoolByIdAsync(id);
                    string res = _userServcie.CompareTwoEntity(rolePool, rolepoolOld);
                    _userServcie.DoDetached(rolepoolOld);
                    _userServcie.UpdateRolePool(rolePool);
                    if (!string.IsNullOrEmpty(res))
                    {
                        ChangeLog changeLog = new ChangeLog()
                        {
                            ChangedBy = User.Identity.Name,
                            DateChanged = DateTime.Now,
                            EntityName = "RolePool",
                            PrimaryKeyValue = id.ToString(),
                            Description = res
                        };
                        _userServcie.CreateChangeLog(changeLog);
                    }
                    await _userServcie.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolePoolExists(rolePool.Id))
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
            List<Role> roles = await _userServcie.GetAllRolesAsync();
            ViewData["Roles"] = roles.ToList();
            return View(rolePool);
        }

        // GET: Admin/RolePools/Delete/5
        [PermissionChecker(78)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePool = await _userServcie.GetRolePoolByIdAsync((int)id);
            if (rolePool == null)
            {
                return NotFound();
            }

            return View(rolePool);
        }

        // POST: Admin/RolePools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionChecker(78)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rolePool = await _userServcie.GetRolePoolByIdAsync(id);
            rolePool.IsDeleted = true;
            rolePool.RemoveDate = DateTime.Now;
            rolePool.OP_FakeRemove = User.Identity.Name;
            _userServcie.UpdateRolePool(rolePool);
            await _userServcie.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolePoolExists(int id)
        {
            return _userServcie.RolePoolExist(id);
        }
    }
}
