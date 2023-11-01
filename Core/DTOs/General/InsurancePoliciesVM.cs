using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class InsurancePoliciesVM
    {
        public User User { get; set; }
        public List<InsuredInfoReportModel> InsuredInfoReportModels { get; set; } = new List<InsuredInfoReportModel>();
        public List<InsuredInfoReportModel> PageinsuredInfoReportModels { get; set; } = new List<InsuredInfoReportModel>();
        public List<LifeBordroBase> PageBordroBases { get; set; } = new List<LifeBordroBase>();
        public int? RecCount { get; set; }
        public int TotalPages { get; set; }
        public int? CurPage { get; set; }
        public int TotalRecCount { get; set; }
        public string SearchText { get; set; }
        public string SearchField { get; set; }
    }
}
