using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.Admin
{
    public class OrgLifeInsuranceModel
    {
        [Display(Name = "کاربر سازمان فروش")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public List<int> OrgUrIds { get; set; }
        public List<UserRole> Organizations { get; set; }

    }
}
