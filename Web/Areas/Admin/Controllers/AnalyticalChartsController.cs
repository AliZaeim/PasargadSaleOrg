using System;
using System.Collections.Generic;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using Core.DTOs.Chart;

using Microsoft.AspNetCore.Mvc;


namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AnalyticalChartsController : Controller
    {

        public IActionResult Index()
        {

            Random rnd = new Random();

            var lstModel = new List<SimpleReportViewModel>
            {
                new SimpleReportViewModel
                {
                    DimensionOne = "فروردین",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "اردیبهشت",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "خرداد",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "تیر",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "مرداد",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "شهریور",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "مهر",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "آبان",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "آذر",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "دی",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "بهمن",
                    Quantity = rnd.Next(100)
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "اسفند",
                    Quantity = rnd.Next(100)
                }
            };
            return View(lstModel);
        }
        public IActionResult BarChart()
        {
            List<double?> zdata = new List<double?> { 100, 110, 90, 50, 70, 38, 68, 72, 70, 100, 50, 120 };
            List<ChartColor> barcolors = new List<ChartColor>
                {
                    ChartColor.FromRgba(255, 99, 132, 0.2),
                    ChartColor.FromRgba(54, 162, 235, 0.2),
                    ChartColor.FromRgba(255, 206, 86, 0.2),
                    ChartColor.FromRgba(75, 192, 192, 0.2),
                    ChartColor.FromRgba(153, 102, 255, 0.2),
                    ChartColor.FromRgba(255, 159, 64, 0.2),
                    ChartColor.FromRgba(55, 159, 64, 0.2),
                    ChartColor.FromRgba(255, 15, 64, 0.2),
                    ChartColor.FromRgba(25, 59, 64, 0.2),
                    ChartColor.FromRgba(5, 159, 64, 0.2),
                    ChartColor.FromRgba(255, 159, 4, 0.2),
                    ChartColor.FromRgba(55, 159, 4, 0.2)
                };
            List<ChartColor> borderColors = new List<ChartColor>
                {
                    ChartColor.FromRgb(255, 99, 132),
                    ChartColor.FromRgb(54, 162, 235),
                    ChartColor.FromRgb(255, 206, 86),
                    ChartColor.FromRgb(75, 192, 192),
                    ChartColor.FromRgb(153, 102, 255),
                    ChartColor.FromRgb(255, 159, 64),
                    ChartColor.FromRgb(220, 160, 64),
                    ChartColor.FromRgb(210, 11, 64),
                    ChartColor.FromRgb(110, 159, 64),
                    ChartColor.FromRgb(255, 59, 64),
                    ChartColor.FromRgb(255, 159, 4),
                     ChartColor.FromRgb(255, 9, 6)

                };
            Chart Barchart = GenerateBarChart(
                "", zdata, new List<string> { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" }, 30,barcolors,borderColors);

            return View(Barchart);
        }
        public IActionResult LineChart()
        {
            Chart LineChart = GenerateLineChart();
            return View(LineChart);
        }
        public IActionResult PieChart()
        {
            Chart PieChart = GeneratePieChart();
            return View(PieChart);
        }
        public IActionResult PolarChart()
        {
            Chart PolarChart = GeneratePolarChart();
            return View(PolarChart);
        }
        public IActionResult LineScatterChart()
        {
            Chart LineScatterChart = GenerateLineScatterChart();
            return View(LineScatterChart);
        }
        public IActionResult RadarChart()
        {
            Chart RadarChart = GenerateRadarChart();
            return View(RadarChart);
        }
        private static Chart GenerateBarChart(string Title, List<double?> zdata, List<string> xLabels, int width,List<ChartColor> BGColors,List<ChartColor> BorderColors)
        {
            var chart = new Chart { Type = Enums.ChartType.Bar };

            var data = new Data
            {
                Labels = xLabels
            };

            var dataset = new BarDataset
            {

                Label = Title,
                Data = zdata,
                BackgroundColor = BGColors,
                BorderColor =BorderColors,
                BorderWidth = new List<int> { 1 },

            };

            data.Datasets = new List<Dataset> { dataset };

            chart.Data = data;


            var options = new Options
            {
                Title = new Title { Text = "نمودار میله ای ماهانه بر اساس فروش", Display = true, FontFamily = "yekan", FontSize = 18, FontStyle = "bold" },
                Responsive = true,
                MaintainAspectRatio = false,
                ResponsiveAnimationDuration = 500,
                Scales = new Scales(),
                Animation = new Animation()
                {
                    Duration = 200,
                    Easing = Enums.Easing.EaseInBounce,

                },


            };

            var legend = new Legend { Display = true, Position = "bottom", FullWidth = true };
            var legendLabel = new LegendLabel { BoxWidth = 0, FontFamily = "yekan", FontSize = 14, FontStyle = "bold", FontColor = ChartColor.FromRgb(255, 0, 0) };
            legend.Labels = legendLabel;
            options.Legend = legend;

            var scales = new Scales
            {
                YAxes = new List<Scale>
                {
                    new CartesianScale
                    {
                        Ticks = new CartesianLinearTick
                        {
                            BeginAtZero = true
                        }
                    }
                },
                XAxes = new List<Scale>
                {
                    new BarScale
                    {

                        BarPercentage = 0.4,
                        BarThickness = width,
                        MaxBarThickness = width+2,                        
                        MinBarLength = 2,
                        GridLines = new GridLine()
                        {
                            OffsetGridLines = true
                        }
                    }
                }
            };

            options.Scales = scales;

            chart.Options = options;
            chart.Options.MaintainAspectRatio = true;
            chart.Options.Responsive = true;

            chart.Options.Layout = new Layout
            {
                Padding = new Padding
                {

                    PaddingObject = new PaddingObject
                    {
                        Left = 5,
                        Right = 6
                    }
                }
            };

            return chart;
        }
        private static Chart GenerateLineChart()
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.Line
            };

            Data data = new Data
            {
                Labels = new List<string>() { "January", "February", "March", "April", "May", "June", "July" }
            };

            LineDataset dataset = new LineDataset()
            {
                Label = "My First dataset",
                Data = new List<double?>() { 65, 59, 80, 81, 56, 55, 40 },
                Fill = "false",
                LineTension = 0.1,
                BackgroundColor = ChartColor.FromRgba(75, 192, 192, 0.4),
                BorderColor = ChartColor.FromRgba(75, 192, 192, 1),
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor>() { ChartColor.FromRgba(75, 192, 192, 1) },
                PointBackgroundColor = new List<ChartColor>() { ChartColor.FromHexString("#fff") },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<ChartColor>() { ChartColor.FromRgba(75, 192, 192, 1) },
                PointHoverBorderColor = new List<ChartColor>() { ChartColor.FromRgba(220, 220, 220, 1) },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false,

            };

            data.Datasets = new List<Dataset>
            {
                dataset
            };

            Options options = new Options()
            {
                Scales = new Scales(),

            };

            Scales scales = new Scales()
            {
                YAxes = new List<Scale>()
                {
                    new CartesianScale()
                }
            };

            CartesianScale yAxes = new CartesianScale()
            {
                Ticks = new Tick()
            };

            Tick tick = new Tick()
            {
                Callback = "function(value, index, values) {return '$' + value;}",

            };

            yAxes.Ticks = tick;
            scales.YAxes = new List<Scale>() { yAxes };
            options.Scales = scales;
            chart.Options = options;

            chart.Data = data;

            return chart;
        }

        private static Chart GenerateLineScatterChart()
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.Line
            };

            Data data = new Data();

            LineScatterDataset dataset = new LineScatterDataset()
            {
                Label = "Scatter Dataset",
                Data = new List<LineScatterData>()
            };

            LineScatterData scatterData1 = new LineScatterData();
            LineScatterData scatterData2 = new LineScatterData();
            LineScatterData scatterData3 = new LineScatterData();

            scatterData1.X = "-10";
            scatterData1.Y = "0";
            dataset.Data.Add(scatterData1);

            scatterData2.X = "0";
            scatterData2.Y = "10";
            dataset.Data.Add(scatterData2);

            scatterData3.X = "10";
            scatterData3.Y = "5";
            dataset.Data.Add(scatterData3);

            data.Datasets = new List<Dataset>
            {
                dataset
            };

            chart.Data = data;

            Options options = new Options()
            {
                Scales = new Scales()
            };

            Scales scales = new Scales()
            {
                XAxes = new List<Scale>()
                {
                    new CartesianScale()
                    {
                        Type = "linear",
                        Position = "bottom",
                        Ticks = new CartesianLinearTick()
                        {
                            BeginAtZero = true
                        }
                    }
                }
            };

            options.Scales = scales;

            chart.Options = options;

            return chart;
        }

        private static Chart GenerateRadarChart()
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.Radar
            };

            Data data = new Data
            {
                Labels = new List<string>() { "Eating", "Drinking", "Sleeping", "Designing", "Coding", "Cycling", "Running" }
            };

            RadarDataset dataset1 = new RadarDataset()
            {
                Label = "My First dataset",
                BackgroundColor = ChartColor.FromRgba(179, 181, 198, 0.2),
                BorderColor = ChartColor.FromRgba(179, 181, 198, 1),
                PointBackgroundColor = new List<ChartColor>() { ChartColor.FromRgba(179, 181, 198, 1) },
                PointBorderColor = new List<ChartColor>() { ChartColor.FromHexString("#fff") },
                PointHoverBackgroundColor = new List<ChartColor>() { ChartColor.FromHexString("#fff") },
                PointHoverBorderColor = new List<ChartColor>() { ChartColor.FromRgba(179, 181, 198, 1) },
                Data = new List<double?>() { 65, 59, 80, 81, 56, 55, 40 }
            };

            RadarDataset dataset2 = new RadarDataset()
            {
                Label = "My Second dataset",
                BackgroundColor = ChartColor.FromRgba(255, 99, 132, 0.2),
                BorderColor = ChartColor.FromRgba(255, 99, 132, 1),
                PointBackgroundColor = new List<ChartColor>() { ChartColor.FromRgba(255, 99, 132, 1) },
                PointBorderColor = new List<ChartColor>() { ChartColor.FromHexString("#fff") },
                PointHoverBackgroundColor = new List<ChartColor>() { ChartColor.FromHexString("#fff") },
                PointHoverBorderColor = new List<ChartColor>() { ChartColor.FromRgba(255, 99, 132, 1) },
                Data = new List<double?>() { 28, 48, 40, 19, 96, 27, 100 }
            };

            data.Datasets = new List<Dataset>
            {
                dataset1, //add to list
                dataset2 // add to list
            };

            chart.Data = data;

            return chart;
        }

        private static Chart GeneratePolarChart()
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.PolarArea
            };

            Data data = new Data
            {
                Labels = new List<string>() { "Red", "Green", "Yellow", "Grey", "Blue" }
            };

            PolarDataset dataset = new PolarDataset()
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#4BC0C0"),
                    ChartColor.FromHexString("#FFCE56"),
                    ChartColor.FromHexString("#E7E9ED"),
                    ChartColor.FromHexString("#36A2EB")
                },
                Data = new List<double?>() { 11, 16, 7, 3, 14 }
            };

            data.Datasets = new List<Dataset>
            {
                dataset
            };

            chart.Data = data;

            return chart;
        }

        private static Chart GeneratePieChart()
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.Pie
            };

            Data data = new Data
            {
                Labels = new List<string>() { "Red", "Blue", "Yellow" }
            };

            PieDataset dataset = new PieDataset()
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56")
                },
                HoverBackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56")
                },
                Data = new List<double?>() { 300, 50, 100 }
            };
            var options = new Options
            {
                Title = new Title { Text = "نمودار دایره ای ماهانه بر اساس فروش", Display = true, FontFamily = "yekan", FontSize = 18, FontStyle = "bold" },
                Responsive = true,
                MaintainAspectRatio = false,
                ResponsiveAnimationDuration = 500,
                Scales = new Scales(),
                Animation = new Animation()
                {
                    Duration = 200,
                    Easing = Enums.Easing.EaseInBounce,

                },


            };
            data.Datasets = new List<Dataset>
            {
                dataset
            };

            chart.Data = data;

            return chart;
        }

        private static Chart GenerateNestedDoughnutChart()
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.Doughnut
            };

            Data data = new Data
            {
                Labels = new List<string>() {
                "resource-group-1",
                "resource-group-2",
                "Data Services - Basic Database Days",
                "Data Services - Basic Database Days",
                "Azure App Service - Basic Small App Service Hours",
                "resource-group-2 - Other"
            }
            };

            PieDataset outerDataset = new PieDataset()
            {
                BackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#3366CC"),
                    ChartColor.FromHexString("#DC3912"),
                    ChartColor.FromHexString("#FF9900"),
                    ChartColor.FromHexString("#109618"),
                    ChartColor.FromHexString("#990099"),
                    ChartColor.FromHexString("#3B3EAC")
                },
                HoverBackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#3366CC"),
                    ChartColor.FromHexString("#DC3912"),
                    ChartColor.FromHexString("#FF9900"),
                    ChartColor.FromHexString("#109618"),
                    ChartColor.FromHexString("#990099"),
                    ChartColor.FromHexString("#3B3EAC")
                },
                Data = new List<double?>() {
                    0.0,
                    0.0,
                    8.31,
                    10.43,
                    84.69,
                    0.84
                }
            };

            PieDataset innerDataset = new PieDataset()
            {
                BackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#3366CC"),
                    ChartColor.FromHexString("#DC3912"),
                    ChartColor.FromHexString("#FF9900"),
                    ChartColor.FromHexString("#109618"),
                    ChartColor.FromHexString("#990099"),
                    ChartColor.FromHexString("#3B3EAC")
                },
                HoverBackgroundColor = new List<ChartColor>() {
                    ChartColor.FromHexString("#3366CC"),
                    ChartColor.FromHexString("#DC3912"),
                    ChartColor.FromHexString("#FF9900"),
                    ChartColor.FromHexString("#109618"),
                    ChartColor.FromHexString("#990099"),
                    ChartColor.FromHexString("#3B3EAC")
                },
                Data = new List<double?>() {
                    8.31,
                    95.96
                }
            };

            data.Datasets = new List<Dataset>
            {
                outerDataset,
                innerDataset
            };

            chart.Data = data;

            return chart;
        }
    }


}
