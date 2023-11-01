using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class SendMessageViewModel
    {
        public string SecurityCode { get; set; }
        public string Key { get; set; }
        public string SMSirLineNumber { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public List<string> MobileNumbers { get; set; } = new List<string>();
    }
}
