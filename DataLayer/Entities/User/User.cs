
using DataLayer.Entities.ComplementaryInfo;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataLayer.Entities.User
{
    public class User
    {
       
        #region User
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "نام")]
        public string FName { get; set; }
        [MaxLength(100)]
        [Required]
        [Display(Name ="نام خانوادگی")]
        public string LName { get; set; }
        /// <summary>
        /// کد کاربری
        /// این کد تعریف شده توسط سیستم است و به بوردرو پاسارگاد مربوط نمی باشد
        /// </summary>
        [MaxLength(50)]
        [Display(Name ="کد کاربری")]        
        public string Code { get; set; }
        [MaxLength(50)]
        [Display(Name ="کد ملی")]
        
        public string NC { get; set; }
        [MaxLength(50)]
        [Display(Name ="نام پدر")]
        public string FatherName { get; set; }
        [Display(Name ="تاریخ تولد")]        
        public DateTime BirthDate { get; set; }
        [MaxLength(50)]
        [Display(Name ="تلفن همراه")]
        public string Cellphone { get; set; }
        [MaxLength(10)]
        public string CellphoneConfirmCode { get; set; }
        [Display(Name ="تایید تلفن همراه")]
        public bool CellphoneIsConfirmed { get; set; }
        [MaxLength(50)]
        [Display(Name ="تلفن")]
        public string Phone { get; set; }
        [Display(Name = "تحصیلات")]
        [MaxLength(50)]
        public string Education { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name ="رمز عبور")]
        public string Password { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        [MaxLength(50)]
        [Display(Name ="شماره شناسنامه")]
        public string IdNumber { get; set; }
        [MaxLength(50)]
        [Display(Name ="محل صدور شناسنامه")]
        public string IdIssuancePlace { get; set; }
        /// <summary>
        /// جنسیت - صفر زن و یک مرد
        /// </summary>
        [Display(Name = "جنسیت")]
        public int? Sex { get; set; }
        [MaxLength(100)]
        [Display(Name ="ایمیل")]
        public string Email { get; set; }
        [MaxLength(50)]
        [Display(Name ="شعبه ناظر")]
        public string SupBranche { get; set; }
        [MaxLength(100)]
        [Display(Name ="آدرس منزل")]
        public string HomeAddress { get; set; }
        [MaxLength(100)]
        [Display(Name ="آدرس محل کار")]
        public string WorkAddress { get; set; }
        [MaxLength(50)]
        [Display(Name ="کد پستی منزل")]
        public string HomePostalCode { get; set; }
        [MaxLength(50)]
        [Display(Name ="کد پستی محل کار")]
        public string WorkPostalCode { get; set; }
        [Display(Name ="وضعیت کاربری")]
        public bool IsActive { get; set; }
        [Display(Name ="تاریخ ثبت کاربر")]
        public DateTime RegDate { get; set; }
        [StringLength(100)]
        [Display(Name ="عکس")]
        public string Avatar { get; set; }
        /// <summary>
        /// کد سازمانی ناظر
        /// </summary>
        [Display(Name ="کد سازمانی ناظر")]
        public string SponserCode { get; set; }
        /// <summary>
        /// آمار اولیه
        /// </summary>
        [Display(Name ="آمار اولیه")]
        public int InitialStatic { get; set; }
        /// <summary>
        /// پورتفوی اولیه
        /// </summary>
        [Display(Name ="پورتفوی اولیه")]
        public long InitialPortfo { get; set; }
        [Display(Name ="شهرستان")]
        public int? CountyId { get; set; }
        /// <summary>
        /// کد سازمانی کاربر در بردروی پاسارگاد
        /// </summary>
        [Display(Name = "کد سازمان فروش")]
        [StringLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string PasargadOrgCode { get; set; }
        [Display(Name = "شماره حساب")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string BankAccountNumber { get; set; }
        [Display(Name = "شماره کارت")]
        [StringLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string BankCardNumber { get; set; }

        [NotMapped]
        [Display(Name ="نام کامل")]
        public string FullName    // the FullName property
        {
            get
            {
                return FName.Trim() + " " + LName.Trim();
            }
        }

        [Display(Name = "کاربر ایجاد کننده")]
        public string OP_Create { get; set; }

        [Display(Name = "کاربر ویرایش کننده")]
        public string OP_Update { get; set; }

        [Display(Name = "کاربر حذف کننده")]
        public string OP_Remove { get; set; }
        #endregion User
        #region Relations 
        [Display(Name = "شهرستان")]
        public County County { get; set; }
        [Display(Name = "شعبه ناظر")]
        [StringLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد!")]
        public string SponserBranch { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<UserPoolReward> UserPoolRewards { get; set; }
      

        #endregion

    }
}
