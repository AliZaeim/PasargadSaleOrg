using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entities.Permissions
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        [Display(Name = "عنوان دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد!")]
        public string PermissionTitle { get; set; }
        [Display(Name = "والد")]
        public int? ParentId { get; set; }
        
        [Display(Name = "کاربر ایجاد کننده")]
        public string OP_Create { get; set; }
       
        [Display(Name = "کاربر ویرایش کننده")]
        public string OP_Update { get; set; }
        
        [Display(Name = "کاربر حذف کننده")]
        public string OP_Remove { get; set; }
        #region Relations
        [ForeignKey("ParentId")]
        public List<Permission> Permissions { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        #endregion
    }
}
