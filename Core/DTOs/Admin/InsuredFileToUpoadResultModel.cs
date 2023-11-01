using DataLayer.Entities.LifeBordro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class InsuredFileToUpoadResultModel
    {
        public bool ActiveSubmit { get; set; }
        /// <summary>
        /// رکوردهای مناسب افزودن به دیتابیس
        /// </summary>
        public List<InsuredInformation> QualifiedRecords { get; set; }
        /// <summary>
        /// رکوردهای اضافه ی الحاقیه در دیتابیس
        /// </summary>
        public List<InsuredInformation> AdditionalRecordinDb { get; set; }
        public string Message { get; set; }
    }
}
