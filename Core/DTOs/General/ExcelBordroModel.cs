using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Core.DTOs.General
{
    public class ExcelBordroModel
    {
       
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>  
        [Display(Name = "شماره بیمه نامه")]
        public string InsNO { get; set; }
        /// <summary>
        /// تاریخ صدور
        /// </summary>  
        [Display(Name = "تاریخ صدور")]
        public string IssueDate { get; set; }
        /// <summary>
        /// کد ملی بیمه گذار
        /// </summary>
        [Display(Name = "کد ملی بیمه گذار")]
        public string InsurerNC { get; set; }
        /// <summary>
        /// بیمه گذار
        /// </summary>
        [Display(Name = "بیمه گذار")]
        public string Insurer { get; set; }
        /// <summary>
        /// شماره الحاقیه
        /// </summary>
        [Display(Name = "شماره الحاقیه")]
        public int AdditionNumber { get; set; }
        /// <summary>
        /// تاریخ شروع الحاقیه
        /// </summary>
        [Display(Name = "تاریخ شروع الحاقیه")]
        public string StartDate { get; set; }
        /// <summary>
        /// تاریخ شروع اولیه
        /// </summary>
        [Display(Name = "تاریخ شروع اولیه")]
        public string InitialStartDate { get; set; }
        /// <summary>
        /// کد ملی بیمه شده
        /// </summary>
        [Display(Name = "کد ملی بیمه")]
        public string InsuredNC { get; set; }
        /// <summary>
        /// بیمه شده
        /// </summary>
        [Display(Name = "بیمه شده")]
        public string Insured { get; set; }
        /// <summary>
        /// مدت
        /// </summary>
        [Display(Name = "مدت")]
        public string Duration { get; set; }
        /// <summary>
        /// روش پرداخت حق بیمه
        /// </summary>
        [Display(Name = "روش پرداخت")]
        public string PayMethod { get; set; }
        /// <summary>
        /// حق بیمه عمر و تامین آتیه
        /// </summary>
        [Display(Name = "حق بیمه عمر")]
        public int LifePremium { get; set; }
        /// <summary>
        /// حق بیمه تکمیلی
        /// </summary>
        [Display(Name = "حق بیمه تکمیلی")]
        public int SupPremium { get; set; }
        /// <summary>
        /// حق بیمه بر حسب روش پرداخت
        /// </summary>
        [Display(Name = "حق بیمه بر حسب روش پرداخت")]
        public int PremiumByPay { get; set; }
        /// <summary>
        /// سپرده
        /// </summary>
        [Display(Name = "سپرده")]
        public int Deposit { get; set; }
        /// <summary>
        /// سرمایه خطر فوت به هر علت
        /// </summary>
        [Display(Name = "سرمایه خطر فوت")]
        public int LifeCapital { get; set; }
        
        
        /// <summary>
        /// کد ملی نماینده فروش
        /// </summary>
        [Display(Name = "کد ملی نماینده")]
        public string AgentNC { get; set; }
        /// <summary>
        /// کد و نام نماینده فروش
        /// </summary>
        [Display(Name = "نام نماینده")]
        public string AgentName { get; set; }
        /// <summary>
        /// سازمان فروش
        /// </summary>
        [Display(Name = "سازمان فروش")]
        public string SalesOrg { get; set; }

       
        /// <summary>
        /// وضعیت بیمه نامه
        /// </summary>
        [Display(Name = "وضعیت")]
        public string Status { get; set; }
        /// <summary>
        /// نوع الحاقیه
        /// </summary>
        [Display(Name = "نوع الحاقیه")]
        public string Type { get; set; }
    }
}
