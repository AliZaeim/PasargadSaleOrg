using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class StepListViewModel
    {
        public List<UserRole> Directs { get; set; }
        public List<Role> LoginUser_Roles { get; set; }

    }
}
