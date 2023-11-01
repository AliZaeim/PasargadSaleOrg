using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.ComplementaryInfo
{
    public class State
    {
        public State()
        {
            this.Counties = new HashSet<County>();
        }
        [Key]
        public int StateId { get; set; }
        [Display(Name = "نام استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        public string StateName { get; set; }
        
        [Display(Name = "کاربر ایجاد کننده")]
        public string OP_Create { get; set; }
        
        [Display(Name = "کاربر ویرایش کننده")]
        public string OP_Update { get; set; }
        
        [Display(Name = "کاربر حذف کننده")]
        public string OP_Remove { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<County> Counties { get; set; }

    }
}
