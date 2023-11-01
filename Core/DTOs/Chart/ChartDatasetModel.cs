
using System.Collections.Generic;


namespace Core.DTOs.Chart
{
   public class ChartDatasetModel
    {
        public string Label { get; set; }
        public List<double?> Data { get; set; }
        public List<string> BgColors { get; set; }
    }
}
