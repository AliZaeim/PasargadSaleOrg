using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    //منظور از کارمزد ها، مجموع کارمزدهای مربوط به تمام نقشهای کاربر می باشد
    public class CommissionListModel
    {
        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        [Display(Name = "نام")]
        public string FullName { get; set; }
        /// <summary>
        /// حساب بانکی کاربر
        /// </summary>
        [Display(Name = "حساب بانکی")]
        public string BankAccount { get; set; }
        /// <summary>
        /// سمت یا نقش 
        /// </summary>
        [Display(Name = "سمت")]
        public string Title { get; set; }
        /// <summary>
        /// کد کاربر
        /// </summary>
        [Display(Name = "کد")]
        public string Code { get; set; }
        /// <summary>
        /// کارمزدهای شخصی کاربر
        /// </summary>
        [Display(Name = "کارمزدهای شخصی")]
        public long PersonalCommAll { get; set; }
        /// <summary>
        /// کارمزدهای سازمانی کاربر
        /// </summary>
        [Display(Name = "کارمزدهای سازمانی")]
        public long OrgCommAll { get; set; }
        /// <summary>
        /// کارمزدهای برابری کاربر
        /// </summary>
        [Display(Name = "کارمزدهای برابری")]
        public long EqRewAll { get; set; }
        /// <summary>
        /// کارمزدهای استخر کاربر
        /// </summary>
        [Display(Name = "کامزدهای استخر")]
        public long PoolRewAll { get; set; }
        /// <summary>
        /// جمع کارمزدهای کاربر
        /// </summary>
        [Display(Name = "جمع")]
        public long RowCommSum { get; set; }
        [Display(Name = "توضیحات")]
        public string Comment { get; set; }
    }
}
