using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.ComplementaryInfo
{
    public class Slider
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(200, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Title { get; set; }
        /// <summary>
        /// حداکثر 1000 کاراکتر
        /// </summary>
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "متن")]
        [StringLength(1000, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Text { get; set; }

        [Display(Name ="متن لینک")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string LinkText { get; set; }
        [Display(Name ="آدرس لینک")]
        [StringLength(150, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Linkaddress { get; set; }

        public bool IsDeleted { get; set; }
        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        [Display(Name = "تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// کاربر ثبت کننده
        /// </summary>
        [Display(Name = "کاربر ثبت کننده")]
        public string OP_Create { get; set; }
        /// <summary>
        /// تاریخ حذف
        /// </summary>
        [Display(Name = "تاریخ حذف")]
        public DateTime? RemoveDate { get; set; }
        /// <summary>
        /// کاربر حذف کننده
        /// </summary>
        [Display(Name = "کاربر حذف کننده")]
        public string OP_FakeRemove { get; set; }
        public IEnumerable<string> TextList
        {
            get { return (Text ?? string.Empty).Split(Environment.NewLine); }
        }
       
        
    }
}
