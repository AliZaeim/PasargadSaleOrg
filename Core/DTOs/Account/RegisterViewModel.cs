using DataLayer.Entities.ComplementaryInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Account
{
    public class RegisterViewModel
    {
        [Display(Name ="نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string UserFirstName { get; set; }
        [Display(Name ="نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string UserFamily { get; set; }
        
        [Display(Name ="سال تولد")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? BDateYear { get; set; }
        [Display(Name ="ماه تولد")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? BDateMounth { get; set; }
        [Display(Name ="روز تولد")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? BDateDay { get; set; }
        [Display(Name ="کد ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [MinLength(10, ErrorMessage = "{0} نمی تواند کمتر از {1} کاراکتر باشد!")]
        public string UserNC { get; set; }
        [Display(Name ="تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression("^[0][1-9]\\d{9}$|^[1-9]\\d{9}$", ErrorMessage = " شماره تلفن همراه نا معتبر است !")]
        public string UserCellphone { get; set; }
        [Display(Name = "تحصیلات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Education { get; set; }
        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? Sex { get; set; }
        [Display(Name ="شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? CountyId { get; set; }
        [Display(Name ="استان")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? StateId { get; set; }
        [Display(Name = "شماره حساب")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string BankAccountNumber { get; set; }       
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "شماره کارت بانکی پاسارگاد")]
        [RegularExpression(@"\d{4}-?\d{4}-?\d{4}-?\d{4}$", ErrorMessage = "شماره کارت نامعتبر است")]
        public string BankCardNumber { get; set; }
        [Display(Name ="کد سازمانی ناظر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]        
        public string SponserCode { get; set; }
        public List<County> Counties { get; set; }
        public List<State> States { get; set; }
    }
}
