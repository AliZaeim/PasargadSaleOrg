﻿using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class NonePaymentBordroesDet
    {
        [Display(Name = "شماره بیمه نامه")]
        public string InsNO { get; set; }
        [Display(Name = "بیمه گذار")]
        public string Insurer { get; set; }
        [Display(Name = "بیمه شده")]
        public string Insured { get; set; }
        [Display(Name = "تاریخ صدور")]
        public DateTime IssueDate { get; set; }
        [Display(Name = "روش پرداخت")]
        public string PaymentMethod { get; set; }
        [Display(Name = "حق بیمه بر حسب روش پرداخت")]
        public int PaymentMethodValue { get; set; }
        [Display(Name = "سپرده")]
        public int Deposit { get; set; }
        [Display(Name = "عامل فروش")]
        public string Seller { get; set; }

        [Display(Name = "نوع")]
        public string Type { get; set; }
        [Display(Name = "وضعیت")]
        public string Status { get; set; }

        [Display(Name = "مجموع حق بیمه وصول شده")]
        public long TotalPremiumReceived { get; set; }
        [Display(Name = "تاریخ آخرین وصول")]
        public DateTime LastReceiveDate { get; set; }
        [Display(Name = "تعداد اقساط معوق")]
        public int NonReceivedCount { get; set; }
        [Display(Name = "تلفن بیمه گذار")]
        public string InsuredPhone { get; set; }

    }
}
