using Core.DTOs.General;
using DataLayer.Entities.LifeBordro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class CompareCommissionFileWithDbVM
    {
        /// <summary>
        /// اختلاف رکوردهای فایل با دیتابیس
        /// </summary>
        public List<PasargadCommissionVM> ExludeFileandDb { get; set; }
        /// <summary>
        /// دادهای مشترک فایل و دیتابیس
        /// </summary>
        public List<PasargadCommissionVM> CommonData { get; set; }
        /// <summary>
        /// معتبر بودن فایل جهت افزودن رکورد از فایل
        /// </summary>
        public bool ConfAdd { get; set; }
        /// <summary>
        /// معتبر بودن فایل جهت بازنشانی رکورد از فایل
        /// </summary>
        public bool ConfUpdate { get; set; }
        /// <summary>
        /// نوع عملیات
        /// </summary>

        public string Action { get; set; }
        public string Message { get; set; }
        public bool ActiveSubmit { get; set; }
    }
}
