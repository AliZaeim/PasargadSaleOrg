using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;



namespace DataLayer.Entities.ComplementaryInfo
{
    public class County
    {
        public County()
        {

        }
        [Key]
        public int CountyId { get; set; }
        [Display(Name = "نام شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        public string CountyName { get; set; }
        [Display(Name = "نام استان")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        public int StateId { get; set; }
        public bool IsDeleted { get; set; }
        
        [Display(Name = "کاربر ایجاد کننده")]
        public string OP_Create { get; set; }
        
        [Display(Name = "کاربر ویرایش کننده")]
        public string OP_Update { get; set; }
        
        [Display(Name = "کاربر حذف کننده")]
        public string OP_Remove { get; set; }
        #region Relations

        public State State { get; set; }
        public virtual ICollection<User.User> Users { get; set; }


        #endregion


    }
}
