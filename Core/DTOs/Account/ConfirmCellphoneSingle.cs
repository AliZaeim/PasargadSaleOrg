using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Account
{
    public class ConfirmCellphoneSingle
    {
       
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "تلفن همراه")]
        public string UserCellphone { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "کد ملی")]
        public string UserNC { get; set; }

    }
    public class GetConfirmCode
    {
        [Display(Name ="کد اعتبارسنجی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ConfirmCode { get; set; }
    }
}
