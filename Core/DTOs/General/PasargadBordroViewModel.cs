using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class PasargadBordroViewModel
    {
       /// <summary>
       /// ردیف
       /// </summary>
        public string Radif { get; set; }
        /// <summary>
        /// شماره پیش نویس
        /// </summary>      
        public string DraftNO { get; set; }
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>        
        public string InsNO { get; set; }
        /// <summary>
        /// تاریخ صدور
        /// </summary>       
        public string IssueDate { get; set; }
        /// <summary>
        /// کد ملی بیمه گذار
        /// </summary>
       
        public string InsurerNC { get; set; }
        /// <summary>
        /// بیمه گذار
        /// </summary>
        
        public string Insurer { get; set; }
        /// <summary>
        /// تاریخ شروع الحاقیه
        /// </summary>
        
        public string StartDate { get; set; }
        /// <summary>
        /// تاریخ شروع اولیه
        /// </summary>
       
        public string InitialStartDate { get; set; }
        /// <summary>
        /// کد ملی بیمه شده
        /// </summary>
        
        public string InsuredNC { get; set; }
        /// <summary>
        /// بیمه شده
        /// </summary>
        
        public string Insured { get; set; }
        /// <summary>
        /// مدت
        /// </summary>
       
        public string Duration { get; set; }
        /// <summary>
        /// روش پرداخت حق بیمه
        /// </summary>
        
        public string PayMethod { get; set; }
        /// <summary>
        /// حق بیمه عمر و تامین آتیه
        /// </summary>

        public string LifePremium { get; set; }
        /// <summary>
        /// حق بیمه تکمیلی
        /// </summary>
        public string SupPremium { get; set; }
        /// <summary>
        /// حق بیمه بر حسب روش پرداخت
        /// </summary>
        public string PremiumByPay { get; set; }
        /// <summary>
        /// سپرده
        /// </summary>

        public string Deposit { get; set; }
        /// <summary>
        /// سرمایه خطر فوت به هر علت
        /// </summary>
        
        public string LifeCapital { get; set; }
        /// <summary>
        /// کد و نام نمایندگی
        /// </summary>
       
        public string Director { get; set; }
        /// <summary>
        /// کد پایه نماینده فروش
        /// </summary>
        
        public string AgentCode { get; set; }
        /// <summary>
        /// کد ملی نماینده فروش
        /// </summary>
        
        public string AgentNC { get; set; }
        /// <summary>
        /// کد و نام نماینده فروش
        /// </summary>
        
        public string AgentName { get; set; }
        /// <summary>
        /// کد و نام مدیر فروش
        /// </summary>
        
        public string SalesManager { get; set; }
        /// <summary>
        /// کد و نام مدیر آموزش
        /// </summary>
        
        public string LearningManager { get; set; }
        /// <summary>
        /// کد و نام مدیر توسعه
        /// </summary>
       
        public string DeveloperManager { get; set; }
        /// <summary>
        /// وضعیت بیمه نامه
        /// </summary>
        
        public string Status { get; set; }
        /// <summary>
        /// نوع الحاقیه
        /// </summary>
        
        public string Type { get; set; }
    }
}
