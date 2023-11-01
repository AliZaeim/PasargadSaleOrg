using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Admin
{
    public class FileCheckResultViewModel
    {
        public bool Conf { get; set; }
        public string[] NonExistCol { get; set; }
        public string Message { get; set; }
    }
}
