using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class CreateAdminUserVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام")]
        public string UserFirstName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام خانوادگی")]
        public string UserFamily { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "کد ملی")]
        public string UserNC { get; set; }
        [Display(Name = "تاریخ تولد ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression(@"^$|^([1۱][۰-۹ 0-9]{3}[/\/]([0 ۰][۱-۶ 1-6])[/\/]([0 ۰][۱-۹ 1-9]|[۱۲12][۰-۹ 0-9]|[3۳][01۰۱])|[1۱][۰-۹ 0-9]{3}[/\/]([۰0][۷-۹ 7-9]|[1۱][۰۱۲012])[/\/]([۰0][1-9 ۱-۹]|[12۱۲][0-9 ۰-۹]|(30|۳۰)))$", ErrorMessage = "تاریخ وارد شده نامعتبر است.")]
        public string UserBirthDate { get; set; }
        
        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression("^[0][1-9]\\d{9}$|^[1-9]\\d{9}$", ErrorMessage = " شماره تلفن همراه نا معتبر است !")]
        public string UserCellphone { get; set; }
        [Display(Name = "نقش")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? RoleId { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? StateId { get; set; }
        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? CountyId { get; set; }

        public List<State>  States { get; set; }
        public List<County> Counties { get; set; }



    }
}
