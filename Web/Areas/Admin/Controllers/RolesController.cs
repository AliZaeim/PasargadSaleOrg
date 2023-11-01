using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.DTOs;
using Core.DTOs.Admin;
using Core.Security;
using Core.Secutiry;
using Core.Services.Interfaces;
using DataLayer.Entities.Permissions;
using DataLayer.Entities.User;

namespace VigiMarket.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RolesController : Controller
    {
        private readonly IUserService _userService;
        
        public RolesController(IUserService userService)
        {
            _userService = userService;
           
        }

        [PermissionChecker(13)]
        public async Task<IActionResult> Index()
        {
            List<Role> roles = await _userService.GetAllRolesAsync();
            return View(roles);
        }
        [PermissionChecker(61)]
        public async Task<IActionResult> PermissionsIndex()
        {
            return View(await _userService.GetAllPermissions());
        }
        [PermissionChecker(58)]
        public IActionResult Create()
        {
            RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel()
            {
                AllPermissions = _userService.GetAllPermissions().Result
            };

            return View(rolePermissionViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(58)]
        public async Task<IActionResult> Create(RolePermissionViewModel rolePermissionViewModel)
        {
            if (!ModelState.IsValid)
            {
                rolePermissionViewModel.AllPermissions = _userService.GetAllPermissions().Result;
                return View(rolePermissionViewModel);

            }
            Role role = new Role()
            {
                RoleName = rolePermissionViewModel.RoleName,
                RoleTitle = rolePermissionViewModel.RoleTitle,
                RoleRate = rolePermissionViewModel.RoleRate
            };
            await _userService.CreateRoleAsync(role, rolePermissionViewModel.SelectedPermissions);
            await _userService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //Edit Role
        [PermissionChecker(59)]
        public async Task<IActionResult> Edit(int id)
        {
            Role role = await _userService.GetRoleByIdAsync(id);
            ViewBag.AllPermissions = new SelectList(_userService.GetAllPermissions().Result, "PermissoinId", "PermissionTitle");
            ViewBag.RolePermissions = new SelectList(_userService.GetPermissions_of_RoleByRoleId(id).Result, "PermissoinId", "PermissionTitle");
            RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel()
            {
                RoleId = id,
                RoleName = role.RoleName,
                RoleTitle = role.RoleTitle,
                RoleRate = role.RoleRate,
                AllPermissions = await _userService.GetAllPermissions(),
                Permissions_of_Role = await _userService.GetPermissions_of_RoleByRoleId(id),
                SelectedPermissions = _userService.GetPermissions_of_RoleByRoleId(id).Result.Select(s => s.PermissionId).ToList(),
                SelectedPermissoinsList = await _userService.GetPermissions_of_RoleByRoleId(id)
            };
            return View(rolePermissionViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(59)]
        public async Task<IActionResult> Edit(RolePermissionViewModel rolePermissionViewModel)
        {
            if (!ModelState.IsValid)
            {
                rolePermissionViewModel.AllPermissions = _userService.GetAllPermissions().Result;
                rolePermissionViewModel.Permissions_of_Role = _userService.GetPermissions_of_RoleByRoleId(rolePermissionViewModel.RoleId).Result;
                rolePermissionViewModel.SelectedPermissions = _userService.GetPermissions_of_RoleByRoleId(rolePermissionViewModel.RoleId).Result.Select(s => s.PermissionId).ToList();
                rolePermissionViewModel.SelectedPermissoinsList = await _userService.GetPermissions_of_RoleByRoleId(rolePermissionViewModel.RoleId);
                return View(rolePermissionViewModel);
            }
            Role role = await _userService.GetRoleByIdAsync(rolePermissionViewModel.RoleId);
            role.RoleName = rolePermissionViewModel.RoleName;
            role.RoleTitle = rolePermissionViewModel.RoleTitle;
            role.RoleRate = rolePermissionViewModel.RoleRate;
            _userService.EditRole(role);
            await _userService.UpdatePermissions_of_RoleAsync(role.RoleId, rolePermissionViewModel.SelectedPermissions);
            await _userService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       
        public async Task<IActionResult> UsersofRole(int roleId)
        {
            List<User> users = await _userService.GetUsers_of_RoleAsync(roleId);
            Role role = await _userService.GetRoleByIdAsync(roleId);
            ViewData["Role"] = role;
            return View(users);
        }
        [PermissionChecker(60)]
        public async Task DeleteRole(int id)
        {
            await _userService.DeleteRoleAsync(id);
            await _userService.SaveChangesAsync();
        }

    }
}