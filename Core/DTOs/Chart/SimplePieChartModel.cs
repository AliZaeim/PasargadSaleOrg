using System.Collections.Generic;

namespace Core.DTOs.Chart
{
    public class SimplePieChartModel
    {
        public List<string> Labels { get; set; }
        public List<double?> Data { get; set; }
        public List<string> BgColors { get; set; }
    }
}
