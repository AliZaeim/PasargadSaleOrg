using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.Blogs
{
    public class NewsGroup
    {
        [Key]
        public int NewsGroup_Id { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Display(Name = "عنوان")]
        public string NewsGroup_Title { get; set; }
        [StringLength(200, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Display(Name = "توضیحات")]
        public string NewsgGroup_Comment { get; set; }
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
        public virtual ICollection<News> News { get; set; }
        #endregion
    }
}
