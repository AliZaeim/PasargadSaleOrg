using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class PoolRewardReport_TotalVM
    {
        public User User { get; set; }
        public long UserDirSales { get; set; }
        public long UserIndirSales { get; set; }
        public bool SelectedByDirPool { get; set; }
        public bool SelectedByIndirPool { get; set; }
        public RolePool RolePool { get; set; }

        public int SharePercent { get; set; }
        public long ShareValue { get; set; }


    }
}
