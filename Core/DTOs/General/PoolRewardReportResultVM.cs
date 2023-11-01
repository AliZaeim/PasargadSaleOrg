using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;
using System.Text;

namespace Core.DTOs.General
{
    public class PoolRewardReportResultVM
    {
        public User Loguser { get; set; }
        public long LoguserDirSales { get; set; }
        public long LogUserIndirSales { get; set; }
        public RolePool LogUserDirPool { get; set; }
        public RolePool LogUserInDirPool { get; set; }
        public List<PoolRewardUserInfoVM> PoolRewardUserInfoVMs { get; set; } = new List<PoolRewardUserInfoVM>();

       

        public CommissionBase CommissionBase { get; set; }



    }
}
