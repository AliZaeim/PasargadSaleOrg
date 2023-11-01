using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Entities.Permissions;
using DataLayer.Entities.User;

namespace Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly MyContext _Context;
        public PermissionService(MyContext Context)
        {
            _Context = Context;
        }

        public void AddPermissionsToRole(int roleId, List<int> permission)
        {
            foreach (var p in permission)
            {
                _Context.RolePermissions.Add(new RolePermission()
                {
                    PermissionId = p,
                    RoleId = roleId
                });
            }

            _Context.SaveChanges();
        }

        public int AddRole(Role role)
        {
            _Context.Roles.Add(role);
            _Context.SaveChanges();
            return role.RoleId;
        }

        public void AddRolesToUser(List<int> roleIds, int userId)
        {
            foreach (int roleId in roleIds)
            {
                _Context.UserRoles.Add(new UserRole()
                {
                    RoleId = roleId,
                    User_ID = userId
                });
            }

            _Context.SaveChanges();
        }

        public bool CheckPermission(int permissionId, string userName)
        {
           int userId = _Context.Users.Single(u => u.Code == userName).Id;

            List<int> UserRoles = _Context.UserRoles
                .Where(r => r.User_ID == userId).Select(r => r.RoleId).ToList();

            if (!UserRoles.Any())
                return false;

            List<int> RolesPermission = _Context.RolePermissions
                .Where(p => p.PermissionId == permissionId)
                .Select(p=>p.RoleId).ToList();

            return RolesPermission.Any(p => UserRoles.Contains(p));
        }

        public void CreatePermision(Permission permission)
        {
            _Context.Permissions.Add(permission);
            _Context.SaveChanges();
        }

        public void DeleteRole(Role role)
        {
           
            role.IsDeleted=true;
            UpdateRole(role);
        }

        public void EditRolesUser(int userId, List<int> rolesId)
        {
            //Delete All Roles User
            _Context.UserRoles.Where(r => r.User_ID == userId).ToList().ForEach(r => _Context.UserRoles.Remove(r));

            //Add New Roles
            AddRolesToUser(rolesId, userId);
        }

        public List<Permission> GetPermissoins()
        {
            return _Context.Permissions.ToList();
        }

        public Role GetRoleById(int roleId)
        {
            return _Context.Roles.Find(roleId);
        }

        public List<Role> GetRoles()
        {
            return _Context.Roles.ToList();
        }

        public List<int> PermissionsofRole(int roleId)
        {
            return _Context.RolePermissions
                .Where(r => r.RoleId == roleId)
                .Select(r => r.PermissionId).ToList();
        }

        public void UpdatePermissionsRole(int roleId, List<int> permissions)
        {
            _Context.RolePermissions.Where(p=>p.RoleId==roleId)
                .ToList().ForEach(p=> _Context.RolePermissions.Remove(p));

            AddPermissionsToRole(roleId,permissions);
        }

        public void UpdateRole(Role role)
        {
            _Context.Roles.Update(role);
            _Context.SaveChanges();
        }
    }
}
