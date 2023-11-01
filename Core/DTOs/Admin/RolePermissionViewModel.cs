using DataLayer.Entities.Permissions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class RolePermissionViewModel
    {
        [Key]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام نقش")]
        [MaxLength(30, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        public string RoleName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "عنوان نقش")]
        [MaxLength(30, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        public string RoleTitle { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "ضریب نقش")]
        public float RoleRate { get; set; }

        public List<Permission> AllPermissions { get; set; }
        public List<Permission> Permissions_of_Role { get; set; }
        public List<int> SelectedPermissions { get; set; }
        public List<Permission> SelectedPermissoinsList { get; set; }
    }
}
