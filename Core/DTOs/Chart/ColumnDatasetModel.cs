using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Core.DTOs.Chart
{
    public class ColumnDatasetModel
    {
        public string Label { get; set; }
        public List<int> Data { get; set; }
        public string BgColor { get; set; }
        public string HoverBgColor { get; set; }
        public string BorderColor { get; set; }

        
    }
}
