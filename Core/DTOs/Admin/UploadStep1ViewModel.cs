using System.ComponentModel.DataAnnotations;
namespace Core.DTOs.Admin
{
    public class UploadStep1ViewModel
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
    }
}
