using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class ReportUserRolesViewModel
    {
        public List<UserRole> PageUserRoles { get; set; }
        public List<UserRole> AllUserRoles { get; set; }
        public int? RecCount { get; set; }
        public int TotalPages { get; set; }
        public int? CurPage { get; set; }
        public int TotalRecCount { get; set; }
        public string SearchText { get; set; }
        public string SearchField { get; set; }
    }
}
