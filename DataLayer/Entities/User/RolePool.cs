using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.User
{
    /// <summary>
    /// پاداش استخر
    /// </summary>
    public class RolePool
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "نقش")]
        public int? RoleId { get; set; }
        /// <summary>
        /// ارزش تجاری عملکرد
        /// </summary>
        [Display(Name = "ارزش تجاری عملکرد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long Value { get; set; }
        /// <summary>
        /// نوع استخر
        /// </summary>
        [Display(Name = "نوع استخر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Type { get; set; }
        /// <summary>
        /// شناسه استخر - متن انگلیسی
        /// </summary>
        [Display(Name = "شناسه")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Symbol { get; set; }
        /// <summary>
        /// بر اساس فروش شخصی
        /// </summary>
        [Display(Name = "بر اساس فروش شخصی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public bool? ByDirectSale { get; set; }
        /// <summary>
        /// بر اساس فروش سازمانی
        /// </summary>
        /// 
        [Display(Name = "بر اساس فروش سازمانی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public bool? ByIndirectSale { get; set; }
        [Display(Name ="درصد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public float Percent { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "کاربر ثبت کننده")]
        public string OP_Create { get; set; }
        [Display(Name = "تاریخ حذف")]
        public DateTime? RemoveDate { get; set; }
        [Display(Name = "کاربر حذف کننده")]
        public string OP_FakeRemove { get; set; }
        #region Relations
        [ForeignKey("RoleId")]
        [Display(Name ="نقش")]
        public Role Role { get; set; }
        #endregion
    }
}
