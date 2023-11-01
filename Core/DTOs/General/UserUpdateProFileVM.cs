using DataLayer.Entities.ComplementaryInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class UserUpdateProFileVM
    {
        public int Id { get; set; }
        [Display(Name = "شماره حساب")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string BankAccountNumber { get; set; }
        [Display(Name = "شماره کارت")]
        [StringLength(19, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [RegularExpression(@"\d{4}-?\d{4}-?\d{4}-?\d{4}$", ErrorMessage ="شماره کارت نامعتبر است")]
        public string BankCardNumber { get; set; }
        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? CountyId { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? StateId { get; set; }
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "آدرس منزل")]
        public string HomeAddress { get; set; }
        [Display(Name = "تحصیلات")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Education { get; set; }
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "ایمیل")]
        [DataType(DataType.EmailAddress,ErrorMessage ="ایمیل نامعتبر است !")]
        public string Email { get; set; }
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "عکس")]
        public string Avatar { get; set; }

        public string Message { get; set; }

        public List<State>  States { get; set; }
        public List<County> Counties { get; set; }
    }
}
