using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataLayer.Entities.ComplementaryInfo
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "موضوع")]
        [StringLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "پیام")]
        public string Message { get; set; }
        [Display(Name = "کد فرستنده")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SenderCode { get; set; }
        [Display(Name = "نام کامل فرستنده")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SenderFullPro { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public DateTime CreateDate { get; set; }
        
        [Display(Name = "اطلاعات دریافت کنندگان")]
        public string RecepiesInfo { get; set; }
        /// <summary>
        /// کدهای کاربری خوانندگان پیام
        /// </summary>
        [Display(Name = "خوانندگان پیام")]        
        public string Readers { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        /// <summary>
        /// لیست اطلاعات دریافت کنندگان پیام به صورت کد کاربری-نام کامل
        /// </summary>
        [NotMapped]
        [Display(Name = "اطلاعات دریافت کنندگان")]
        public IEnumerable<string> RecepiesList
        {
            get { return (RecepiesInfo ?? string.Empty).Split(Environment.NewLine); }
        }
        [NotMapped]        
        public IEnumerable<string> MessagesList
        {
            get { return (Message ?? string.Empty).Split(Environment.NewLine); }
        }
        [NotMapped]
        public IEnumerable<string> ReadersList
        {
            get { return (Readers ?? string.Empty).Split(Environment.NewLine); }
        }
        public int? ParentId { get; set; }
        #region Relations
        [ForeignKey(nameof(ParentId))]
        public Conversation Parent { get; set; }
        #endregion
    }
}
