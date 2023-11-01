using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Chart
{
    public class ColumnChartModel
    {
        public List<string> Labels { get; set; }
        public List<ColumnDatasetModel> ColumnDatasetModels { get; set; }
        public ColumnDatasetModel ColumnDataset1 { get; set; }
        public ColumnDatasetModel ColumnDataset2 { get; set; }
        public List<int> data1 { get; set; }
        public List<int> data2 { get; set; }
    }
}
