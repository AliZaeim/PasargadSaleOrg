using DataLayer.Entities.LifeBordro;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class AdditionsReportModel
    {
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>
        [Display(Name = "شماره بیمه نامه")]
        public string InsNO { get; set; }
        /// <summary>
        /// شماره الحاقیه
        /// </summary>
        [Display(Name = "شماره")]
        public string Number { get; set; }
        /// <summary>
        /// تاریخ صدور
        /// </summary>
        [Display(Name = "تاریخ صدور")]
        public string IssueDate { get; set; }
        /// <summary>
        /// تاریخ شروع اولیه
        /// </summary>
        [Display(Name = "تاریخ شروع اولیه")]        
        public  string InitialStartDate { get; set; }
        /// <summary>
        /// تاریخ شروع الحاقیه
        /// </summary>
        [Display(Name = "تاریح شروع الحاقیه")]
        public string StartDate { get; set; }
        /// <summary>
        /// وضعیت الحاقیه
        /// </summary>
        [Display(Name = "وضعیت")]
        public string Status { get; set; }
        /// <summary>
        /// نوع الحاقیه
        /// </summary>
        [Display(Name = "نوع الحاقیه")]
        public string AdditionType { get; set; }
        /// <summary>
        /// نام کامل بیمه گذار
        /// </summary>
        [Display(Name = "بیمه گذار")]
        public string InsurerFullName { get; set; }
        /// <summary>
        /// کد ملی بیمه گذار
        /// </summary>
        [Display(Name = "کد ملی بیمه گذار")]
        public string InsurerNC { get; set; } 
        /// <summary>
        /// بیمه شده
        /// </summary>
        [Display(Name = "بیمه شده")]
        public string InsuredFullName { get; set; }
        [Display(Name = "کد ملی بیمه شده")]
        public string InsuredNC { get; set; }      
        
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
        /// سپرده
        /// </summary>
        [Display(Name = "سپرده")]
        public string Deposit { get; set; }
        /// <summary>
        /// سرمایه بیمه عمر
        /// </summary>
        [Display(Name = "سرمایه بیمه عمر")]
        public string LifeInsCapital { get; set; }
        /// <summary>
        /// نماینده فروش
        /// </summary>
        [Display(Name = "عامل فروش")]
        public string Seller { get; set; }
        
        

        public LifeBordroBase LifeBordroBase { get; set; }

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
