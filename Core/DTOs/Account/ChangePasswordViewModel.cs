using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "رمز عبور فعلی")]
        public string Password { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "رمز عبور جدید")]
        [MinLength(8, ErrorMessage = "{0} باید {1} رقم باشد!")]
        public string NewPassowrd { get; set; }
        public bool IsSuccess { get; set; }
    }
}
