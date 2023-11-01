using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class ConversationVM
    {
        public int Id { get; set; }

        [Display(Name = "موضوع پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Subject { get; set; }
        [Display(Name = "متن پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Message { get; set; }
        [Display(Name = "کد فرستنده")]
        [StringLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string SenderCode { get; set; }
        [Display(Name = "مشخصات کامل فرستنده")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string SenderFullPro { get; set; }
        [Display(Name = "فعال/غیرفعال")]
        public bool IsActive { get; set; }
        public bool Read { get; set; }
        public int? ParentId { get; set; }
        [Display(Name = "دریافت کنندگان")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public List<string> UserInfos { get; set; } = new List<string>();

        public List<User> Users { get; set; } = new List<User>();
        public List<string> SelectedRecepsCode { get; set; } = new List<string>();

        public string Title { get; set; }
    }
}
