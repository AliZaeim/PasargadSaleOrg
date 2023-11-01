using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class NonPaymentBordroesExcelModel
    {
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>
        [Display(Name = "شماره بیمه نامه")]
        public string InsNO { get; set; }
        [Display(Name = "بیمه گذار")]
        public string Insurer { get; set; }
        [Display(Name = "بیمه شده")]
        public string Insured { get; set; }
        [Display(Name = "تلفن بیمه شده")]
        public string InsuredPhone { get; set; }
        [Display(Name = "تاریخ صدور")]
        public string IssueDate { get; set; }
        [Display(Name = "روش پرداخت")]
        public string PayMethod { get; set; }
        /// <summary>
        /// حق بیمه بر حسب روش پرداخت
        /// </summary>
        [Display(Name = "حق بیمه")]
        public int PayMethodValue { get; set; }
        [Display(Name = "سپرده")]
        public int Deposite { get; set; }
        [Display(Name = "عامل فروش")]
        public string Seller { get; set; }
        [Display(Name = "نوع")]
        public string Type { get; set; }
        [Display(Name = "وضعیت")]
        public string Status { get; set; }
        [Display(Name = "مجموع حق بیمه وصول شده")]
        public long TotalPremiumReceived { get; set; }
        [Display(Name = "تاریخ آخرین وصول")]
        public string LastReceiveDate { get; set; }
        [Display(Name = "تعداد اقساط معوق")]
        public int NonReceivedCount { get; set; }
        



    }
}
