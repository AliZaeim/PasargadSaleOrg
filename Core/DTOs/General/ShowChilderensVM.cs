using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
   public class ShowChilderensVM
    {
        public UserRole ActiveLoginUserRole { get; set; }
        public List<UserRole> LoginUserChilderens { get; set; }
        public List<UserRole>   SelectedUserRoleChilderens { get; set; }
        public UserRole ActiveSelectedUser_UserRole { get; set; }
    }
}
