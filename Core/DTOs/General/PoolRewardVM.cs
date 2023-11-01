using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Core.DTOs.General
{
    public class PoolRewardVM
    {
        [Display(Name = "ماه")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? Mounth { get; set; }
        [Display(Name = "سال")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? Year { get; set; }


        public string FullName { get; set; }
        public int StartYear { get; set; }
        public int CurrentYear { get; set; }

        public string Message { get; set; }
        public CommissionBase   CommissionBase { get; set; }
        public PoolRewardReportResultVM PoolRewardReportResultVM { get; set; }
        public List<PoolRewardReport_TotalVM> poolRewardReport_TotalVMs { get; set; } = new List<PoolRewardReport_TotalVM>();

        public List<RolePool> rolePools { get; set; }

        public bool IsShow { get; set; }
    }
}
