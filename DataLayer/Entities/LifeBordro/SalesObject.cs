using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities.LifeBordro
{
    /// <summary>
    /// مدل فروش شامل کاربر، رتبه فروش و درصد کارمزد
    /// </summary>
    public class SalesObject
    {
        public int UrId { get; set; }
        public int SRate { get; set; }
        public float SPercent { get; set; }
        public float SEqPercent { get; set; }
        public UserRole UserRole { get; set; }
    }
}
