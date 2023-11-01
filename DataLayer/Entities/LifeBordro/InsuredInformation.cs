using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.LifeBordro
{
    public class InsuredInformation
    {
        public int Id { get; set; }
        [Display(Name = "شماره بیمه نامه")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string InsNO { get; set; }
        [Display(Name = "تاریخ تولد")]
        public DateTime InsuredBirthDate { get; set; }
        [Display(Name = "استان")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string State { get; set; }
        [Display(Name = "شهر")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string City { get; set; }
        [Display(Name = "آدرس")]
        [StringLength(150, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Address { get; set; }
        [Display(Name = "تلفن همراه")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Cellphone { get; set; }
        [Display(Name = "تلفن")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string Phone { get; set; }
    }
}
