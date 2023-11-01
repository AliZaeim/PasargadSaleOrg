using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class ChangeRoleofUserVM
    {
        public User User { get; set; }
        public int UrId { get; set; }
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        [Display(Name = "نقش")]
        public int NewRoleId { get; set; }
        public List<Role> Roles { get; set; }
        public bool IsSuccess { get; set; }
    }
}
