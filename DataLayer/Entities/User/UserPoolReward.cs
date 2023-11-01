using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.User
{
    /// <summary>
    /// کارمزدهای استخر اختصاص یافته به کاربر
    /// </summary>
    public class UserPoolReward
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int UserId { get; set; }
        [Display(Name = "درصد کارمزد")]
        public float PoolPercent { get; set; }
        [Display(Name = "مبلغ کارمزد")]
        public long PoolValue { get; set; }
        [Display(Name = "مبلغ پورتفو")]
        public long PortfoValue { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public DateTime? RegDate { get; set; }
        [Display(Name = "توضیحات")]
        public string Comment { get; set; }
        #region Relations
        [Display(Name = "کاربر")]
        [ForeignKey("UserId")]
        public User User { get; set; }
        #endregion
    }
}
