using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.LifeBordro
{
    /// <summary>
    /// الحاقیه بیمه عمر
    /// </summary>
    public class LifeBordroAddition
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// شماره الحاقیه
        /// </summary>
        [Display(Name ="شماره الحاقیه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Number { get; set; }
       
        /// <summary>
        /// کد ملی بیمه گذار
        /// </summary>
        [Display(Name ="کد ملی بیمه گذار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(20, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        
        public string InsurerNC { get; set; }
        /// <summary>
        /// نام کامل بیمه گذار
        /// </summary>
        [Display(Name ="بیمه گذار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string InsurerFullName { get; set; }
        /// <summary>
        /// تاریخ شروع اولیه
        /// </summary>
        [Display(Name ="تاریخ شروع اولیه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime InitialStartDate { get; set; }
        /// <summary>
        /// تاریخ شروع الحاقیه
        /// </summary>
        [Display(Name ="تاریح شروع الحاقیه")]        
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// کد ملی بیمه شده
        /// </summary>
        [Display(Name ="کد ملی بیمه شده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(20, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string InsuredNC { get; set; }
        /// <summary>
        /// نام کامل بیمه شده
        /// </summary>
         [Display(Name ="بیمه شده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string InsuredFullName { get; set; }
        /// <summary>
        /// مدت بیمه نامه
        /// </summary>
        [Display(Name ="مدت بیمه نامه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Duration { get; set; }
        /// <summary>
        /// روش پرداخت
        /// </summary>
        [Display(Name ="روش پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string PaymentMethod { get; set; }
        /// <summary>
        /// حق بیمه عمر و تامین آتیه
        /// </summary>
        [Display(Name ="حق بیمه عمر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int LFPremium { get; set; }
        /// <summary>
        /// حق بیمه تکمیلی
        /// </summary>
        [Display(Name ="حق بیمه تکمیلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int SupPremium { get; set; }
        /// <summary>
        /// حق بیمه برحسب روش پرداخت
        /// </summary>
        [Display(Name ="حق بیمه بر حسب روش پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int PremiumbyPaymentMethod { get; set; }
        /// <summary>
        /// سپرده
        /// </summary>
        [Display(Name ="سپرده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long Deposit { get; set; }
        /// <summary>
        /// سرمایه خطر فوت به هر علت
        /// </summary>
        [Display(Name ="سرمایه فوت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CapitalDied { get; set; }
        /// <summary>
        /// فروشنده
        /// </summary>
        [Display(Name ="فروشنده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Seller{ get; set; }
        /// <summary>
        /// کد ملی فروشنده
        /// </summary>
        [Display(Name = "کد ملی فروشنده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(20, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string SellerNC { get; set; }
        /// <summary>
        /// کد فروشنده
        /// </summary>
        [Display(Name = "کد فروشنده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(20, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string SellerCode { get; set; }
        /// <summary>
        /// سلسله فروش
        /// از پایین ترین فروشنده تا بالاترین ناظر
        /// urId-Percent Enter ...
        /// </summary>
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Display(Name = "سلسله فروش")]

        public string SalesSeries { get; set; }
        
        
        public IEnumerable<string> SalesUsers
        {
            get { return (SalesSeries ?? string.Empty).Split(Environment.NewLine); }
        }
        
        /// <summary>
        /// وضعیت
        /// بازخرید - بازخرید سیستمی - فعال - فوت
        /// </summary>
        [Display(Name ="وضعیت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Status { get; set; }
        /// <summary>
        /// نوع الحاقیه
        /// </summary>
        [Display(Name ="نوع الحاقیه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Type { get; set; }
        [Display(Name = "فعال/غیرفعال")]
        public bool IsActive { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public IEnumerable<string> InsuredFullNameList
        {
            get { return (InsuredFullName ?? string.Empty).Split(" "); }
        }
        [NotMapped]
        public IEnumerable<string> InsurerFullNameList
        {
            get { return (InsurerFullName ?? string.Empty).Split(" "); }
        }
        [NotMapped]
        public IEnumerable<string> SellerFullNameList
        {
            get { return (Seller ?? string.Empty).Split(" "); }
        }


        [Display(Name ="بردرو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public Guid LBBId { get; set; }
        #region Relations
        [ForeignKey("LBBId")]
        public LifeBordroBase LifeBordroBase { get; set; }
        #endregion
    }
}
