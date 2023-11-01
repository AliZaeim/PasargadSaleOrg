
using System.Collections.Generic;

namespace Core.DTOs.Chart
{
    public class BarChartModel
    {
        public string ChartTitle { get; set; }
        public int BorderWidth { get; set; }
        public List<string> XLabels { get; set; }
        public List<int?> YValues { get; set; }
        public List<string> BGColors { get; set; }
        public List<string> BorderColors { get; set; }
    }
}
