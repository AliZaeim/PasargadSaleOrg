using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Account
{
    public class CheckCodeViewModel
    {
        public bool Validate { get; set; }
        public string Message { get; set; }
    }
}
