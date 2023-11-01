using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="کد کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="رمز عبور")]
        public string UserPassword { get; set; }
        [Display(Name ="مرا بخاطر بسپار")]
        public bool Remember { get; set; }
        

        public string ReturnUrl { get; set; }



    }
}
