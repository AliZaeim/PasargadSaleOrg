using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class AddRoleToUserViewModel
    {
        public Role Role { get; set; }
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        [Display(Name ="نقش")]
        public int RoleId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="ناظر")]
        public int ParentURId { get; set; }

        public bool IsSuccess { get; set; }

        public List<UserRole> Parents { get; set; }

        public List<Role> Roles { get; set; }
    }
}
