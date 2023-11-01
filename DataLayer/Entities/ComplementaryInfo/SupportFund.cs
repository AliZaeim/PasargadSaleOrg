using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.ComplementaryInfo
{
    /// <summary>
    /// صندوق پشتیبان
    /// </summary>
    public class SupportFund
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="درصد")]
        public float Percent { get; set; }
        [Display(Name ="تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        [Display(Name ="ثبت کننده")]
        public string OPCreate { get; set; }
    }
}
