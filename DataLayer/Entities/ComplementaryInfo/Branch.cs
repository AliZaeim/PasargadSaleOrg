using System.ComponentModel.DataAnnotations;


namespace DataLayer.Entities.ComplementaryInfo
{
    public class Branch
    {
        public int Id { get; set; }
        [Display(Name = "نام شعبه")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string BrName { get; set; }
        [Display(Name = "کد شعبه")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string BrCode { get; set; }
        [Display(Name = "رشته کاری")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string BrField { get; set; }
        [Display(Name = "اختیارات")]
        public string BrAttribution { get; set; }
        [Display(Name = "نام مدیر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string ManagerName { get; set; }
        [Display(Name = "تاریخ تاسیس")]
        [StringLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string DateofStablishment { get; set; }
        [Display(Name = "تلفن")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Phone { get; set; }
        [Display(Name = "فاکس")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Fax { get; set; }
        [Display(Name = "آدرس")]
        [StringLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Address { get; set; }
    }
}
