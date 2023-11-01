using Core.DTOs.Chart;
using DataLayer.Entities.LifeBordro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class AdminIndexModel
    {
        /// <summary>
        /// تعداد سازمان فروش
        /// </summary>
        public int SalesOrgCount { get; set; }
        /// <summary>
        /// تعداد فروش مستقیم
        /// </summary>
        public int DirectSalesCount { get; set; }
        /// <summary>
        ///پورتفوی فروش مستقیم 
        /// </summary>
        public long DirectSalesValue { get; set; }
        /// <summary>
        /// تعداد فروش سازمانی
        /// </summary>
        public int IndirectSalesCount { get; set; }
        /// <summary>
        /// پورتفوی فروش سازمانی
        /// </summary>
        public long IndirectSalesValue { get; set; }
        /// <summary>
        /// مبلغ آخرین کارمزد
        /// </summary>
        public long LastCommissionValue { get; set; }
        /// <summary>
        /// همکاران ثبت نامی ماه جاری
        /// </summary>
        public int CurrentMounthRegUserCount { get; set; }
        /// <summary>
        /// تعداد فروش مستقیم هفته
        /// </summary>
        public int CurrentWeekDirectSalesCount { get; set; }
        /// <summary>
        /// مبلغ فروش مستقیم هفته
        /// </summary>
        public int CurrentWeekDirectSalesValue{ get; set; }
        /// <summary>
        /// تعداد فروش سازمانی هفته
        /// </summary>
        public int CurrentWeekInDirectSalesCount { get; set; }
        /// <summary>
        /// مبلغ فروش سازمانی هفته
        /// </summary>
        public int CurrentWeekInDirectSalesValue { get; set; }
        /// <summary>
        /// ده بیمه نامه اخیر- مستقیم و سازمانی
        /// </summary>
        public List<LifeBordroBase>  Last10Bordroes { get; set; }
        /// <summary>
        ///نمودار دایره ای پراکندگی شعب فروش بر اساس استانها
        /// </summary>        
        public SimplePieChartModel SimplePieChartModel { get; set; }
        public SimplePieChartModel BranchesDistChart { get; set; }
        public ColumnChartModel ColumnChartModel { get; set; }

        /// <summary>
        /// تعداد بیمه نامه های عدم وصول
        /// </summary>
        public int NonePaymentBordroesCount { get; set; }
        /// <summary>
        /// تعداد کل بیمه نامه ها
        /// </summary>

        public int TotalBorderoesCount { get; set; }

    }
}
