using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.LifeBordro
{
    public class BordroFileSetting
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string ColTitle { get; set; }
        [MaxLength(50)]
        public string ColAddress { get; set; }

        public int ColIndex { get; set; }

    }
}
