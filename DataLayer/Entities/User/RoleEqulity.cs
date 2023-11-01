using DataLayer.Entities.ComplementaryInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.User
{
    /// <summary>
    /// پاداش همسطحی یا برابری
    /// </summary>
    public class RoleEqulity
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int RoleId { get; set; }
        [Display(Name ="درصد برابری سطح یک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public float L1EPercent { get; set; }
        [Display(Name = "درصد برابری سطح دو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public float L2EPercent { get; set; }
        [Display(Name = "درصد برابری سطح سه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public float L3EPercent { get; set; }
        [Display(Name = "درصد برابری سطح چهار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public float L4EPercent { get; set; }

        public bool IsDeleted { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "کاربر ثبت کننده")]
        public string OP_Create { get; set; }
        [Display(Name = "تاریخ حذف")]
        public DateTime? RemoveDate { get; set; }
        [Display(Name = "کاربر حذف کننده")]
        public string OP_FakeRemove { get; set; }
        #region Relations
        [ForeignKey("RoleId")]
        [Display(Name = "نقش")]
        public Role Role { get; set; }
       
        #endregion
    }
}
