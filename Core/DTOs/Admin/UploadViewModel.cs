using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class UploadViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "ماه")]
        public int Mounth { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "سال")]
        public int Year { get; set; }
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        [Display(Name = "نوع فایل")]
        public string Type { get; set; }
        [Display(Name = "فایل")]
        public IFormFile File { get; set; }
        [Display(Name = "عملیات")]
        public string Action { get; set; }
        public bool ValidationStep1 { get; set; } = false;
        public bool ExistDuration { get; set; }
        public string Message { get; set; }

        public string FileName { get; set; }

    }
}
