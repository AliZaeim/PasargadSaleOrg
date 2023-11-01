using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class SystemCommissionVM
    {
        public User User { get; set; }
        public string Title { get; set; }
        public long  PersonalCommissionsTotal { get; set; }
        public long OrgCommissionsTotal { get; set; }
        public long  EqulityRewardTotal { get; set; }
        public long PoolRewardTotal { get; set; }
        public string Comment { get; set; }

    }
}
