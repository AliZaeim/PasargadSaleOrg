using DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class ChildRate
    {
        public UserRole UserRole { get; set; }
        public int Rate { get; set; }
    }
}
