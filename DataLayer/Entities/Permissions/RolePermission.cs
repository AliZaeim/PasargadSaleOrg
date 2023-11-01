using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using DataLayer.Entities.User;

namespace DataLayer.Entities.Permissions
{
    public class RolePermission
    {
        [Key]
        public int RP_Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public Role Role { get; set; }
        public Permission Permission { get; set; }
        
        [Display(Name = "کاربر ایجاد کننده")]
        public string OP_Create { get; set; }
        
        [Display(Name = "کاربر ویرایش کننده")]
        public string OP_Update { get; set; }
        
        [Display(Name = "کاربر حذف کننده")]
        public string OP_Remove { get; set; }
    }
}
