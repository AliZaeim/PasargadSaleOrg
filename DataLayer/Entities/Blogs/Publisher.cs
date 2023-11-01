using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.Blogs
{
    public class Publisher
    {
        [Key]
        public int Publisher_Id { get; set; }
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Publisher_Title { get; set; }
        public bool IsDeleted { get; set; }
        
        [Display(Name = "تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "کاربر ثبت کننده")]
        public string OP_Create { get; set; }
        [Display(Name = "تاریخ حذف")]
        public DateTime? RemoveDate { get; set; }
        [Display(Name = "کاربر حذف کننده")]
        public string OP_FakeRemove { get; set; }
        public string OP_Remove { get; set; }
        #region Relations
        public virtual ICollection<News> News { get; set; }
        #endregion

    }
}
