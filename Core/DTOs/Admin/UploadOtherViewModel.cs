using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class UploadOtherViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        [Display(Name = "نوع فایل")]
        public string Type { get; set; }
        [Display(Name = "فایل")]
        public IFormFile File { get; set; }
        [Display(Name = "عملیات")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public string Action { get; set; }

        public string Message { get; set; }
    }
}
