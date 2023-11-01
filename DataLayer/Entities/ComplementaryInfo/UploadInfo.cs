using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.ComplementaryInfo
{
    public class UploadInfo
    {
        public int Id { get; set; }
        [Display(Name = "تاریخ آپلود")]
        public DateTime? UpDate { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "فایل")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string File { get; set; }
        [Display(Name = "توضیحات")]
        [StringLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Description { get; set; }
        [Display(Name = "نوع آپلود")]
        public string Type { get; set; }
    }
}
