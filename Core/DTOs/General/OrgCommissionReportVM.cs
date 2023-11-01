using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;


using System.Collections.Generic;


namespace Core.DTOs.General
{
    /// <summary>
    /// مدل کارمزد فروش مستقیم و غیرمستقیم کاربر
    /// </summary>
    public class OrgCommissionReportVM
    {
        public int UrId { get; set; }
        public UserRole UserRole { get; set; }
        /// <summary>
        /// بیمه نامه شخصی
        /// </summary>
        public List<LifeBordroBase> DirectLifeBordroBases{ get; set; }
        /// <summary>
        /// بیمه نامه های سازمانی
        /// </summary>
        public List<LifeBordroBase> IndirectLifeBordroBases { get; set; }
        /// <summary>
        /// کارمزد مستقیم
        /// </summary>
        public List<Commission> DirectCommissions { get; set; }
        /// <summary>
        /// کارمزد سازمانی
        /// </summary>
        public List<Commission> IndeirectCommissions { get; set; }
    }
}
