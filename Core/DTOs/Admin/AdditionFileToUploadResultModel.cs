using DataLayer.Entities.LifeBordro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class AdditionFileToUploadResultModel
    {
        public bool ActiveSubmit { get; set; }
        /// <summary>
        /// رکوردهای مناسب افزودن به دیتابیس
        /// </summary>
        public List<LifeBordroAddition> QualifiedAdditions { get; set; }
        /// <summary>
        /// رکوردهای اضافه ی الحاقیه در دیتابیس
        /// </summary>
        public List<LifeBordroAddition> AdditionalRecordinDb { get; set; }
        public string Message { get; set; }
    }
}
