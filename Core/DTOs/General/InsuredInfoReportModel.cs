
using DataLayer.Entities.LifeBordro;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Core.DTOs.General
{
    /// <summary>
    /// مدل گزارش اطلاعات بیمه شدگان
    /// </summary>
    public class InsuredInfoReportModel
    {
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>
        [Display(Name = "شماره بیمه نامه")]
        public string InsNO { get; set; }
        
        /// <summary>
        /// وضعیت بیمه نامه
        /// </summary>
        [Display(Name = "وضعیت")]
        public string Status { get; set; }
        /// <summary>
        /// نوع الحاقیه
        /// </summary>
        [Display(Name = "نوع الحاقیه")]
        public string AdditionType { get; set; }
        /// <summary>
        /// کد ملی بیمه گذار
        /// </summary>
        [Display(Name = "کد ملی بیمه گذار")]
        public string InsurerNC { get; set; }
        /// <summary>
        /// نام کامل بیمه گذار
        /// </summary>
        [Display(Name = "بیمه گذار")]
        public string InsurerFullName { get; set; }
        
        /// <summary>
        /// بیمه شده
        /// </summary>
        [Display(Name = "بیمه شده")]
        public string InsuredFullName { get; set; }
        [Display(Name = "کد ملی بیمه شده")]
        public string InsuredNC { get; set; }
        /// <summary>
        /// تاریخ تولد بیمه شده
        /// </summary>
        [Display(Name = "تاریخ تولد بیمه شده")]
        public string InsuredBirthDate { get; set; }
        /// <summary>
        /// تاریخ صدور
        /// </summary>
        [Display(Name = "تاریخ صدور")]
        public string IssueDate { get; set; }
        /// <summary>
        /// مدت بیمه
        /// </summary>
        [Display(Name = "مدت بیمه")]
        public string Duration { get; set; }
        /// <summary>
        /// روش پرداخت
        /// </summary>
        [Display(Name = "روش پرداخت")]
        public string PaymentMethod { get; set; }
        /// <summary>
        /// مبلغ بر حسب روش پرداخت
        /// </summary>
        [Display(Name = "مبلغ بر حسب روش پرداخت")]
        public int PaymentMethodValue { get; set; }
        /// <summary>
        /// نماینده فروش
        /// </summary>
        [Display(Name = "عامل فروش")]
        public string Seller { get; set; }
        /// <summary>
        /// استان
        /// </summary>
        [Display(Name = "استان")]
        public string State { get; set; }
        /// <summary>
        /// شهر
        /// </summary>
        [Display(Name = "شهر")]
        public string City { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        [Display(Name = "آدرس")]
        public string Address { get; set; }
        /// <summary>
        /// تلفن همراه
        /// </summary>
        [Display(Name = "تلفن همراه")]
        public string Cellphone { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        [Display(Name = "تلفن")]
        public string Phone { get; set; }

        public LifeBordroBase   LifeBordroBase { get; set; }

        public IEnumerable<string> InsuredFullNameList
        {
            get { return (InsuredFullName ?? string.Empty).Split(" "); }
        }
        public IEnumerable<string> InsurerFullNameList
        {
            get { return (InsurerFullName ?? string.Empty).Split(" "); }
        }
        public IEnumerable<string> SellerFullNameList
        {
            get { return (Seller ?? string.Empty).Split(" "); }
        }
    }
}
