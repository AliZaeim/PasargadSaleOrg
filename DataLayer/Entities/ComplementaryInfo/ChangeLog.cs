using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entities.ComplementaryInfo
{
    public class ChangeLog
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        public string EntityName { get; set; }
        [StringLength(20, ErrorMessage = "{0} باید حداکثر {1} کاراکتر باشد!")]
        
        public string PrimaryKeyValue { get; set; }

        public string Description { get; set; }
        
        public DateTime DateChanged { get; set; }
        [MaxLength(50)]
        public string ChangedBy { get; set; }

        public IEnumerable<string> DescriptionList
        {
            get { return (Description ?? string.Empty).Split(Environment.NewLine); }
        }
        
    }
}
