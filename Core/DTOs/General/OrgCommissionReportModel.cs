using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class OrgCommissionReportModel
    {
        [Display(Name = "شماره بیمه نامه")]
        public string InsNO { get; set; }
        [Display(Name = "بیمه گذار")]
        public string Insurer { get; set; }
        [Display(Name = "بیمه شده")]
        public string Insured { get; set; }
        [Display(Name = "تاریخ سررسید")]
        public string DueDate { get; set; }
        [Display(Name = "تاریخ واریز")]
        public string PaidDate { get; set; }
        [Display(Name = "حق بیمه عمر")]
        public int LifePermium { get; set; }
        [Display(Name = "حق بیمه تکمیلی")]
        public int SupPremium { get; set; }
        [Display(Name = "کارمزد عمر")]
        public int LifeCommission { get; set; }
        [Display(Name = "کارمزد تکمیلی")]
        public int SupCommission { get; set; }
        [Display(Name = "مجموع کارمزد")]
        public int SumCommision { get; set; }
        [Display(Name = "درصد")]
        public float Percent { get; set; }
        [Display(Name = "برابری")]
        public float EqPercent { get; set; }
        [Display(Name = "فروشنده")]
        public string Seller { get; set; }
        [Display(Name = "کد ملی فروشنده")]
        public string SellerNC { get; set; }
        [Display(Name = "گروه فروش")]
        public string OwnerUser { get; set; }
        [Display(Name = "نوع کارمزد")]
        public string CommissionType { get; set; }
        [Display(Name = "سال")]
        public string Year { get; set; }
        [Display(Name = "ماه")]
        public string Mounth { get; set; }
    }
}
