using DataLayer.Entities.Permissions;
using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles
        List<Role> GetRoles();
        int AddRole(Role role);
        Role GetRoleById(int roleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);
        void AddRolesToUser(List<int> roleIds, int userId);
        void EditRolesUser(int userId, List<int> rolesId);

        #endregion
        #region Permission
        List<Permission> GetPermissoins();
        void CreatePermision(Permission permission);
        void AddPermissionsToRole(int roleId, List<int> permission);
        List<int> PermissionsofRole(int roleId);
        void UpdatePermissionsRole(int roleId, List<int> permissions);

        bool CheckPermission(int permissionId, string userName);
        #endregion
    }
}
