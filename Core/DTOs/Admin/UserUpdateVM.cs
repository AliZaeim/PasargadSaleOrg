using DataLayer.Entities.ComplementaryInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime;
using System.Text;

namespace Core.DTOs.Admin
{
    public class UserUpdateVM
    {
        public int Id { get; set; }
        
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FName { get; set; }
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LName { get; set; }
        
        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [MinLength(10, ErrorMessage = "{0} نمی تواند کمتر از {1} کاراکتر باشد!")]
        public string NC { get; set; }
        [StringLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }
        [StringLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [RegularExpression("^[0][1-9]\\d{9}$|^[1-9]\\d{9}$", ErrorMessage = " شماره تلفن همراه نا معتبر است !")]
        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Cellphone { get; set; }
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "تلفن")]
        public string Phone { get; set; }
        [Display(Name = "تحصیلات")]
        
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Education { get; set; }
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "ایمیل")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "ایمیل نامعتبر است !")]
        public string Email { get; set; }
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "آدرس منزل")]
        public string HomeAddress { get; set; }
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "آدرس محل کار")]
        public string WorkAddress { get; set; }
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "کد پستی منزل")]
        public string HomePostalCode { get; set; }
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "کد پستی محل کار")]
        public string WorkPostalCode { get; set; }
        /// <summary>
        /// آمار اولیه
        /// </summary>
        [Display(Name = "آمار اولیه")]
        public int InitialStatic { get; set; }
        /// <summary>
        /// پورتفوی اولیه
        /// </summary>
        [Display(Name = "پورتفوی اولیه")]
        public long InitialPortfo { get; set; }
        [Display(Name = "شماره حساب")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string BankAccountNumber { get; set; }
        [Display(Name = "شماره کارت")]
        [StringLength(19, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string BankCardNumber { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int? StateId { get; set; }
        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int? CountyId { get; set; }
        /// <summary>
        /// کد سازمانی کاربر در بردروی پاسارگاد
        /// </summary>
        [Display(Name = "کد سابق سازمان فروش")]
        [StringLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]        
        public string PasargadOrgCode { get; set; }
        [Display(Name = "کد سازمانی ناظر")]
        
        public string SponserCode { get; set; }
        /// <summary>
        /// شعبه ناظر
        /// </summary>
        [Display(Name = "شعبه ناظر")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string SponserBranch { get; set; }
        public List<State>  States { get; set; }
        public List<County> Counties { get; set; }

    }
}
