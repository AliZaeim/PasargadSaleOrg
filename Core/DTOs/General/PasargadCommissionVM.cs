using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class PasargadCommissionVM
    {
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>
        public string InsNO { get; set; }
        /// <summary>
        /// تاریخ سررسید
        /// </summary>
        public string DueDate { get; set; }
        /// <summary>
        /// تاریخ پرداخت
        /// </summary>
        public string PaidDate { get; set; }
        /// <summary>
        /// درصد
        /// </summary>
        public string Percent { get; set; }
        /// <summary>
        /// حق بیمه عمر
        /// </summary>
        public string LifePremium { get; set; }
        /// <summary>
        /// حق بیمه تکمیلی
        /// </summary>
        public string SupPremium { get; set; }
        /// <summary>
        /// کارمزد عمر
        /// </summary>
        public string LifeCommission { get; set; }
        /// <summary>
        /// کارمزد تکمیلی
        /// </summary>
        public string SupCommission { get; set; }
        /// <summary>
        /// جمع مجموع کارمزد
        /// </summary>
        public string SumCommission { get; set; }
        /// <summary>
        /// مالیات
        /// </summary>
        public string Tax { get; set; }
        /// <summary>
        /// کسورات
        /// </summary>
        public string Deductions { get; set; }
        /// <summary>
        /// مالیات بر ارزش افزوده
        /// </summary>
        public string Vat { get; set; }
        /// <summary>
        /// عوارض شهرداری
        /// </summary>
        public string ManicipalTax { get; set; }
        /// <summary>
        /// مجموع ارزش افزوده
        /// </summary>
        public string TotalVat { get; set; }
        /// <summary>
        /// کارمزد خالص
        /// </summary>
        public string NetCommission { get; set; }
        /// <summary>
        /// شرح کسورات
        /// </summary>
        public string DeductionDesc { get; set; }


    }
}
