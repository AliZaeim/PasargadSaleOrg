using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class SOrgCommissionsReport
    {
        [Display(Name = "ماه")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? Mounth { get; set; }
        [Display(Name = "سال")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int? Year { get; set; }
        [Display(Name = "کاربر سازمان فروش")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public List<int> OrgUrIds { get; set; }

        public UserRole OrgUserRole { get; set; }
        public CommissionBase CommissionBase { get; set; }
        public List<LifeBordroBase> LifeBordroBases { get; set; }
        public List<Commission> Commissions { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public string FullName { get; set; }
        public int StartYear { get; set; }
        public int CurrentYear { get; set; }
        public List<OrgUserComVM> OrgUserComVMs { get; set; }

        public List<SystemCommissionVM> SystemCommissionVMs { get; set; }

        //For change systemCommissionVMs to Appropriate Model for Commission List Page 
        public List<CommissionListModel> CommissionListModels { get; set; }

        public List<PoolRewardReport_TotalVM> PoolRewardReport_TotalVMs { get; set; }

        public int? RecCount { get; set; }
        public int? CurPage { get; set; }
        public int TotalRec { get; set; }
        public long AllTotalPersonalCom { get; set; }
        public long AllTotalOrgCom { get; set; }
        public long AllTotalEqRew { get; set; }
        public long AllTotalPoolRew { get; set; }

        public string Message { get; set; }

        public string TextFileString { get; set; }
        /// <summary>
        /// عبارت جیسون رشته ای کارمزد
        /// </summary>
        public string ComListJsonString { get; set; }
        public string OrgUrIdsJsonString { get; set; }
        public string OrgUserRolesJsonString { get; set; }

        public string search { get; set; }

    }
}
