using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.LifeBordro
{
    /// <summary>
    /// جدول پایه بیمه عمر
    /// </summary>
    public class LifeBordroBase
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "سال دوره")]
        public int Year { get; set; }
        [Display(Name = "ماه دوره")]
        public int Mounth { get; set; }
        [Display(Name = "شماره بیمه نامه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string InsNO { get; set; }
        /// <summary>
        /// تاریخ صدور
        /// </summary>
        [Display(Name = "تاریخ صدور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime IssueDate { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "فعال / غیرفعال")]
        public bool IsActive { get; set; }
        [StringLength(100)]
        [Display(Name = "ثبت کننده")]
        public string OPCreate { get; set; }
        #region Relations
        public virtual ICollection<LifeBordroAddition> LifeBordroAdditions { get; set; }
        public ICollection<Commission> Commissions { get; set; }
        #endregion
    }
}
