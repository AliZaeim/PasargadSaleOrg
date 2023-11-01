using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.ComplementaryInfo
{
    /// <summary>
    /// اطلاعات پایه
    /// </summary>
    public class BaseData
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="درصد کل")]
        public float TotalPercent { get; set; }
    }
}
