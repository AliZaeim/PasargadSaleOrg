using DataLayer.Context;
using DataLayer.Entities.LifeBordro;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DataLayer.Entities.User
{
    public class UserRole
    {
        public UserRole()
        {
            this.Childeren = new HashSet<UserRole>(); 
            
        }
        [Key]
        public int URId { get; set; }
        [Required]
        public int User_ID { get; set; }
        [Required]
        public int RoleId { get; set; } 
        [Display(Name ="تاریخ ثبت نام")]
        [Required]
        public DateTime RegisterDate { get; set; }
        [Display(Name ="وضعیت نقش")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(50)]
        [Display(Name = "کاربر ایجاد کننده")]
        public string OP_Create { get; set; }
        public int? UserRoleParentId { get; set; }
        [ForeignKey(nameof(UserRoleParentId))]
        public UserRole UserRoleParent { get; set; }
        public DateTime? UserRoleParentReceiveDate { get; set; }

        public virtual ICollection<UserRole> Childeren { get; set; }
       
        [StringLength(50)]
        [Display(Name = "کاربر حذف کننده")]
        public string OP_Remove { get; set; }
        #region Relations
        [ForeignKey("User_ID")]
        public User User { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        #endregion
        [NotMapped]
        [Display(Name = "مشخصات کامل")]
        public string FullPro    // the FullProperties property
        {
            get
            {
                return User?.FullName + " - " + Role?.RoleTitle;
            }
        }
        
       
    }
  
}
