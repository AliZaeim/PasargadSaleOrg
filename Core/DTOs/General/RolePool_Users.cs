using DataLayer.Entities.User;
using System.Collections.Generic;
namespace Core.DTOs.General
{
    /// <summary>
    /// استخر و کاربران آن
    /// </summary>
    public class RolePool_Users
    {
        public RolePool RolePool { get; set; }       
        public List<RolePool_UserInfo> rolePool_UserInfos { get; set; }
    }
}
