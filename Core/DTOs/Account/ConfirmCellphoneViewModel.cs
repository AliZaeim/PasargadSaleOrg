using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Account
{
    public class ConfirmCellphoneViewModel
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string Cellphone { get; set; }
        public string NC { get; set; }
        [Display(Name ="کد فعالسازی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ConfirmCode { get; set; }

    }
}
