using DataLayer.Entities.ComplementaryInfo;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.User
{
    /// <summary>
    /// کارمزدهای نقش
    /// </summary>
    public class RoleCommission
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// نقش
        /// </summary>
        [Display(Name ="نقش",Order =1, Prompt = "Enter Last Name", Description = "Emp Last Name")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        
        public int RoleId { get; set; }
        /// <summary>
        /// کارمزد فروش مستقیم
        /// </summary>
        [Display(Name ="کارمزد فروش مستقیم")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        
        public float PersonalSalesPercent { get; set; }
        /// <summary>
        /// کارمزد فروش سازمانی
        /// </summary>
        [Display(Name = "کارمزد فروش سازمانی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public float OrganizationSalesPercent { get; set; }
        /// <summary>
        /// حداقل مبلغ پورتفوی
        /// </summary>
        [Display(Name = "حداقل مبلغ پورتفوی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long MinSaleValue { get; set; }
        /// <summary>
        /// حداقل تعداد بیمه نامه
        /// </summary>
        [Display(Name ="حداقل تعداد بیمه نامه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long MinSaleCount { get; set; }
        /// <summary>
        /// رتبه کاربر دارای نقش
        /// </summary>
        [Display(Name = "رتبه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Rate { get; set; }
        /// <summary>
        /// حذف شده
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        [Display(Name ="تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// کاربر ثبت کننده
        /// </summary>
        [Display(Name ="کاربر ثبت کننده")]
        public string OP_Create { get; set; }
        /// <summary>
        /// تاریخ حذف
        /// </summary>
        [Display(Name ="تاریخ حذف")]
        public DateTime? RemoveDate { get; set; }
        /// <summary>
        /// کاربر حذف کننده
        /// </summary>
        [Display(Name ="کاربر حذف کننده")]
        public string OP_FakeRemove{ get; set; }
        #region Relations
        /// <summary>
        /// نقش
        /// </summary>
        [ForeignKey("RoleId")]
        [Display(Name ="نقش")]
        public Role Role { get; set; }
        
        #endregion
    }
}
