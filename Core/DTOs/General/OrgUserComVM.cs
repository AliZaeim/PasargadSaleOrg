using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    /// <summary>
    /// پکیج فروش شخصی و سازمانی هر کاربر
    /// </summary>
    public class OrgUserComVM
    {
        public UserRole UserRole { get; set; }
        public List<Commission> PersoanlCommissions { get; set; }
        public List<Commission> OrgCommissions { get; set; }
    }
}
