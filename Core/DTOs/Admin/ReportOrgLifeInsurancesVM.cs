using DataLayer.Entities.LifeBordro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class ReportOrgLifeInsurancesVM
    {
        public List<LifeBordroBase> PageBordroes { get; set; }
        public List<LifeBordroBase> AllBordroes { get; set; }
        public int? RecCount { get; set; }
        public int TotalPages { get; set; }
        public int? CurPage { get; set; }
        public int TotalRecCount { get; set; }
        public string SearchText { get; set; }

        public int IsDateRange { get; set; }
    }
}
