using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    /// <summary>
    /// شناسه فروشنده - رتبه در سلسه فروش - درصد فروش - درصد برابری
    /// </summary>
    public class Seller_Rate_PercentViewModel
    {
        /// <summary>
        /// فروشنده
        /// </summary>
        public UserRole Seller { get; set; }
        /// <summary>
        /// شناسه کاربری فروشنده
        /// </summary>
        public int UrId { get; set; }
        /// <summary>
        /// رتبه فروش
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// درصد کارمزد
        /// </summary>
        public float Percent { get; set; }
        /// <summary>
        /// درصد برابری
        /// </summary>
        public float EqPercent { get; set; }
    }
}
