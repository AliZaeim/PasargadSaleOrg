using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.User
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Display(Name = "نام نقش")]
        [MaxLength(30, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string RoleName { get; set; }
        [Display(Name = "عنوان نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(30, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        public string RoleTitle { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "ضریب نقش")]
        public float RoleRate { get; set; } = 1;
        public bool IsDeleted { get; set; }
        [StringLength(50)]
        [Display(Name = "کاربر ایجاد کننده")]
        public string OP_Create { get; set; }

        [StringLength(50)]
        [Display(Name = "کاربر حذف کننده")]
        public string OP_Remove { get; set; }
        #region Relations
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleCommission> RoleCommissions { get; set; }
        public virtual ICollection<RolePool> RolePools { get; set; }
        public virtual ICollection<RoleEqulity> RoleEqulities { get; set; }
        #endregion
    }
}
