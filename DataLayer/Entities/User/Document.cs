using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.User
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        [Display(Name ="تصویر مدرک")]
        [StringLength(150, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FileName { get; set; }
        [Display(Name ="نام مدرک")]        
        [StringLength(100, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Name { get; set; }
        /// <summary>
        /// نوع مدرک
        /// nc => کارت ملی
        /// agdeal => قرارداد نمایندگی
        /// ejudeg => مدرک تحصیلی
        /// </summary>
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="نوع مدرک")]
        [StringLength(100, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Type { get; set; }
        [Display(Name ="کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int UserId { get; set; }
        #region Relations
        [ForeignKey("UserId")]
        [Display(Name ="کاربر")]
        public User User { get; set; }
        #endregion
    }
}
