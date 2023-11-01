using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text;

namespace DataLayer.Entities.LifeBordro
{
    public class CommissionBase
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// جمع مجموع کارمزد
        /// </summary>
        [Display(Name = "جمع مجموع کارمزد")]
        public long CommissionSum { get; set; }
        /// <summary>
        /// مالیات
        /// </summary>
        [Display(Name = "مالیات")]
        public long Tax { get; set; }       
        /// <summary>
        /// کسورات
        /// </summary>
        [Display(Name = "کسورات")]
        public long Deductions { get; set; }
        /// <summary>
        /// مالیات بر ارزش افزوده
        /// </summary>
        [Display(Name = "مالیات بر ارزش افزوده")]
        public long Vat { get; set; }
        /// <summary>
        /// عوارض شهرداری
        /// </summary>
        [Display(Name = "عوارض شهرداری")]
        public long ManicipalTax { get; set; }
        /// <summary>
        /// مجموع ارزش افزوده
        /// </summary>
        [Display(Name = "مجموع ارزش افوزده")]
        public long TotalVat { get; set; }
        /// <summary>
        /// کارمزد خالص
        /// </summary>
        [Display(Name = "کارمزد خالص")]
        public long NetCommission { get; set; }
        /// <summary>
        /// شرح کسورات
        /// </summary>
        [Display(Name = "شرح کسورات")]
        public string DeductionDesc { get; set; }
        [Display(Name = "سال")]
        public int Year { get; set; }
        [Display(Name = "ماه")]
        public int Mounth { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public DateTime CreateDate { get; set; }

        #region Relations
        public ICollection<Commission> Commissions { get; set; }
        #endregion
    }
}
