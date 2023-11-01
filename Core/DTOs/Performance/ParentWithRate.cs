using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Performance
{
    public class ParentWithRate
    {
        public UserRole UserRole { get; set; }
        public int Rate { get; set; }
    }
}
