
using System.Collections.Generic;

namespace Core.DTOs.Chart
{
    public class ChartDataModel
    {
        public List<string> Labels { get; set; }
        public List<ChartDatasetModel> ChartDatasetModels { get; set; }
    }
}
