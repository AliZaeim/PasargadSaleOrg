using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class PasargadInsuredInfoModel
    {
        /// <summary>
        /// شماره بیمه نامه
        /// </summary>
        public string InsNO { get; set; }
        /// <summary>
        /// وضعیت بیمه نامه
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// نوع الحاقیه
        /// </summary>
        public string AdditionType { get; set; }
        /// <summary>
        /// تاریخ تولد بیمه شده
        /// </summary>
        public string InsuredBirthDate { get; set; }
        /// <summary>
        /// بیمه شده
        /// </summary>
        public string InsuredFullName { get; set; }
        /// <summary>
        /// تاریخ صدور
        /// </summary>
        public string IssueDate { get; set; }
        /// <summary>
        /// مدت بیمه
        /// </summary>
        public string Duration { get; set; }
        /// <summary>
        /// روش پرداخت
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// استان
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// شهر
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// تلفن همراه
        /// </summary>
        public string Cellphone { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        public string Phone { get; set; }


    }
}
