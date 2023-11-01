using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.Blogs
{
    public class News
    {
        [Key]
        public int News_Id { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(6, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Display(Name = "کد")]
        public string News_Code { get; set; }
        [Display(Name = "تاریخ")]
        [Required]
        public DateTime News_Date { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "عنوان")]
        [StringLength(100, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string News_Title { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "خلاصه")]
        [StringLength(2000, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string News_Abstract { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "متن")]
        public string News_Text { get; set; }
        
        [Display(Name = "تگ ها")]
        [StringLength(300, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string News_Tags { get; set; }
        [Display(Name = "تصویر")]
        [StringLength(100, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string News_Image { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "گروه")]
        public int NewsGroup_Id { get; set; }
         [Display(Name = "ناشر")]
        public int Publisher_Id { get; set; }

        
        [Display(Name = "تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "کاربر ثبت کننده")]
        public string OP_Create { get; set; }
        [Display(Name = "تاریخ حذف")]
        public DateTime? RemoveDate { get; set; }
        [Display(Name = "کاربر حذف کننده")]
        public string OP_FakeRemove { get; set; }
        public string OP_Remove { get; set; }
        public IEnumerable<string> TagsList
        {
            get { return (News_Tags ?? string.Empty).Split("-"); }
        }
        #region Relations
        [ForeignKey("NewsGroup_Id")]
        [Display(Name = "گروه")]
        public NewsGroup NewsGroup { get; set; }
        [ForeignKey("Publisher_Id")]
        [Display(Name = "ناشر")]
        public Publisher Publisher { get; set; }
        
        #endregion





    }
}
