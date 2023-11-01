using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class UploadDocumentViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="فایل")]
        [StringLength(150, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string NCFileName { get; set; }
    }
}
