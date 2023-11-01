using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class UploadStep2ViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "ماه")]
        public int Mounth { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "سال")]
        public int Year { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نوع فایل")]
        public string Type { get; set; }
        [Display(Name = "توضیحات")]
        [StringLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Discription { get; set; }
        [Required(ErrorMessage = "لطفا {0} راانتخاب کنید")]
        [Display(Name = "فایل")]
        public IFormFile File { get; set; }
    }
}
