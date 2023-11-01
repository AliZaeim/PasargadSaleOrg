using DataLayer.Entities.LifeBordro;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class PCommissionsReport
    {
        [Display(Name = "ماه")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? Mounth { get; set; }
        [Display(Name = "سال")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? Year { get; set; }
        public CommissionBase CommissionBase { get; set; }
        public List<LifeBordroBase> LifeBordroBases { get; set; }
        public List<Commission> Commissions { get; set; }
        public string FullName { get; set; }
        public int StartYear { get; set; }
        public int CurrentYear { get; set; }

        public double CommissionSum { get; set; }

        public string Message { get; set; }
    }
}
