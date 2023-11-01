using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.LifeBordro
{
    public class Commission
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>
        [Display(Name = "شماره بیمه نامه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public LifeBordroBase LifeBordroBase { get; set; }
       // public string InsNO { get; set; }
        /// <summary>
        /// تاریخ سررسید
        /// </summary>        
        [Display(Name = "تاریخ سررسید")]
        public DateTime DueDate { get; set; }
        /// <summary>
        /// تاریخ واریز
        /// </summary>
        [Display(Name = "تاریخ واریز")]
        public DateTime PaidDate { get; set; }
        /// <summary>
        /// درصد
        /// </summary>
        [Display(Name = "درصد")]
        public float Percent { get; set; }
        /// <summary>
        /// حق بیمه عمر
        /// </summary>
        [Display(Name = "حق بیمه عمر")]
        public long LifePremium { get; set; }
        /// <summary>
        /// حق بیمه تکمیلی
        /// </summary>
        [Display(Name = "حق بیمه تکمیلی")]
        public long SupPermium { get; set; }
        /// <summary>
        /// کارمزد عمر
        /// </summary>
        [Display(Name = "کارمزد عمر")]
        public long LifeCommission { get; set; }
        /// <summary>
        /// کارمزد تکمیلی
        /// </summary>
        [Display(Name = "کارمزد تکمیلی")]
        public long SupCommission { get; set; }
        

        public int CBId { get; set; }
        #region Relation
        [ForeignKey(nameof(CBId))]
        public CommissionBase CommissionBase { get; set; }
        #endregion

    }
}
