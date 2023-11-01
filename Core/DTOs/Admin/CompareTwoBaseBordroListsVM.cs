using DataLayer.Entities.LifeBordro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class CompareTwoBaseBordroListsVM
    {
        /// <summary>
        /// داده های اضافه ی موجود در فایل
        /// </summary>
        public List<LifeBordroBase> AdditionalDatainUploadedFile { get; set; }
        /// <summary>
        /// داده های اضافه ی موجود در دیتابیس
        /// </summary>
        public List<LifeBordroBase> AdditionalDatainDb { get; set; }
        /// <summary>
        /// داده های موجود در دیتابیس جهت ویرایش اطلاعات آنها
        /// </summary>
        public List<LifeBordroBase> AnyExistinDb { get; set; }
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
