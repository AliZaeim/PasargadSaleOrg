using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.Gallery
{
    public class GalleryGroup
    {
        [Key]
        public int GG_Id { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Display(Name = "عنوان")]
        public string GG_Title { get; set; }
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "تصویر")]
        public string GG_IntroImage { get; set; }
        [StringLength(300, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        [Display(Name = "توضیحات")]
        public string GG_Comment { get; set; }
        #region Relations
        public virtual ICollection<Gallery> Galleries { get; set; }
        #endregion
    }
}
