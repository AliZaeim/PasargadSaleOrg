using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class ChangeParentViewModel
    {
        public int User_URId { get; set; }
        public UserRole userRole { get; set; }
        [Display(Name ="ناظر جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int User_NewParent_URId { get; set; }
        [Display(Name ="ناظر فعلی")]
        public UserRole CuParent { get; set; }
        public List<UserRole> Parents { get; set; }
    }
}
