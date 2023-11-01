using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    /// <summary>
    /// مدل اطلاعات کاربران برای خروجی اکسل
    /// </summary>
    public class UsersInfoModelForExcel
    {
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string Family { get; set; }
        [Display(Name = "کد کاربری")]
        public string Code { get; set; }
        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }
        [Display(Name = "تاریخ ثبت نام")]
        public string RegisterDate { get; set; }
        [Display(Name = "آخرین مسئولیت")]
        public string ActiveRole { get; set; }
        [Display(Name = "تلفن همراه")]
        public string Cellphone { get; set; }
        [Display(Name = "تلفن")]
        public string Phone { get; set; }
        [Display(Name = "تاریخ تولد")]
        public string BirthDate { get; set; }
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
        [Display(Name = "تحصیلات")]
        public string Ejucation { get; set; }
        [Display(Name = "ناظر")]
        public string Supervisor { get; set; }
        [Display(Name = "پورتفوی اولیه")]
        public string InitialPortfo { get; set; }
        [Display(Name = "آمار اولیه")]
        public string InitialStatic { get; set; }
        [Display(Name = "آدرس منزل")]
        public string HomeAddress { get; set; }
        [Display(Name = "شماره حساب بانکی")]
        public string BonkAccountNumber { get; set; }
        [Display(Name = "شماره کارت بانکی")]
        public string BankCardNumber { get; set; }

    }
}
