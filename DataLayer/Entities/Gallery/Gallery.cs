using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.Gallery
{
    public class Gallery
    {
        [Key]
        public int Gallery_Id { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Display(Name = "عنوان")]
        public string Gallery_Title { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Display(Name = "فایل")]

        public string Gallery_File { get; set; }
        [Display(Name = "توضیحات")]
        [StringLength(300, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string Gallery_Comment { get; set; }
        [Display(Name = "تاریخ")]
        public DateTime Gallery_Date { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "گروه")]
        public int GG_Id { get; set; }
        #region Relations
        [ForeignKey("GG_Id")]
        public GalleryGroup GalleryGroup { get; set; }
        #endregion

    }
}
