using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using Core.DTOs.Chart;
using Core.DTOs.General;
using Core.Services.Interfaces;
using Core.Utility;
using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBordroService _bordroService;
        public HomeController(IUserService userService, IBordroService bordroService)
        {
            _userService = userService;
            _bordroService = bordroService;
        }
        public async Task<IActionResult> Index()
        {
            AdminIndexModel adminIndexModel = new AdminIndexModel();
            PersianCalendar PC = new PersianCalendar();
            int weekN = PC.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Saturday);
            int CurMounth = PC.GetMonth(DateTime.Now);
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<UserRole> LoginUserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);
            List<Core.DTOs.Admin.ChildRate> OrguserRoles = new List<Core.DTOs.Admin.ChildRate>();
            List<LifeBordroBase> IndlifeBordroBases = new List<LifeBordroBase>();
            List<NonePaymentBordroesDet> nonePaymentBordroes = await _bordroService.GetNonPaidBordroesAsync(User.Identity.Name);
            
            if(nonePaymentBordroes != null)
            {
                adminIndexModel.NonePaymentBordroesCount = nonePaymentBordroes.Count();
            }
            foreach (var item in LoginUserRoles)
            {
                List<LifeBordroBase> indlife = await _bordroService.GetIndirectBordroBasebyurId(item.URId);
                IndlifeBordroBases.AddRange(indlife);
                OrguserRoles = _userService.GetAllChilds(item.URId).ToList();
            }
            adminIndexModel.SalesOrgCount = OrguserRoles.Where(w => w.UserRole.User.IsActive).GroupBy(g=> g.UserRole.User).Count();
            List<UserRole> CurrentMounthChilds = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive && PC.GetYear(w.UserRole.User.RegDate) == PC.GetYear(DateTime.Now) && PC.GetMonth(w.UserRole.User.RegDate) == PC.GetMonth(DateTime.Now)).Select(x => x.UserRole).ToList();
            adminIndexModel.CurrentMounthRegUserCount = CurrentMounthChilds.GroupBy(x => x.User).Count();
            List<LifeBordroBase> DirectBordroes = await _bordroService.GetDirectBordroBasebyNC(user.NC);
            List<LifeBordroBase> IndirectBordroes = IndlifeBordroBases.ToList();
            List<LifeBordroBase> DirectWeekBordroes = DirectBordroes.Where(w => PC.GetWeekOfYear(w.IssueDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == weekN).ToList();
            List<LifeBordroBase> InDirectWeekBordroes = IndlifeBordroBases.Where(w => PC.GetWeekOfYear(w.IssueDate, CalendarWeekRule.FirstDay, DayOfWeek.Saturday) == weekN).ToList();
            List<LifeBordroBase> AllBrdroes = new List<LifeBordroBase>();
            if (DirectBordroes != null)
            {
                adminIndexModel.DirectSalesCount = DirectBordroes.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive)).Count();
                adminIndexModel.DirectSalesValue = DirectBordroes.SelectMany(x => x.LifeBordroAdditions).Where(w => w.IsActive).Sum(x =>(long) x.PremiumbyPaymentMethod);
                AllBrdroes.AddRange(DirectBordroes);
            }
            if (IndirectBordroes != null)
            {
                adminIndexModel.IndirectSalesCount = IndirectBordroes.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive)).Count();
                adminIndexModel.IndirectSalesValue = IndirectBordroes.SelectMany(x => x.LifeBordroAdditions).Where(w => w.IsActive).AsEnumerable().Sum(x =>(long) x.PremiumbyPaymentMethod);
                //adminIndexModel.IndirectSalesValue = IndirectBordroes.Select(x => x.LifeBordroAdditions.Where(w => w.IsActive)).ToList().Sum(x => x.PremiumbyPaymentMethod);
                AllBrdroes.AddRange(IndirectBordroes);
            }
            adminIndexModel.TotalBorderoesCount = AllBrdroes.Count();
            adminIndexModel.Last10Bordroes = AllBrdroes.OrderByDescending(r => r.IssueDate).Take(10).ToList();
            adminIndexModel.CurrentWeekDirectSalesCount = DirectWeekBordroes.Count();
            adminIndexModel.CurrentWeekDirectSalesValue = DirectWeekBordroes.Sum(x => x.LifeBordroAdditions.Sum(x => x.PremiumbyPaymentMethod));
            adminIndexModel.CurrentWeekInDirectSalesCount = InDirectWeekBordroes.Count();
            adminIndexModel.CurrentWeekInDirectSalesValue = InDirectWeekBordroes.Sum(x => x.LifeBordroAdditions.Sum(x => x.PremiumbyPaymentMethod));
            int LComY = 0; int LComM = 0; long TotalLastCom = 0;
           CommissionBase comissionBase = await _bordroService.GetLastCommissionAsync();
            if (comissionBase != null)
            {
                LComY = comissionBase.Year;
                LComM = comissionBase.Mounth;
                List<Commission> LastDircommissions = await _bordroService.GetCommissionsBySellerNC_Year_Mounth(user.NC, LComY, LComM);

                List<SystemCommissionVM> systemCommissionVMs = await _bordroService.GetUserSystemCommissionAsync(User.Identity.Name, LComY, LComM);
                TotalLastCom = systemCommissionVMs.Sum(x => x.EqulityRewardTotal + x.OrgCommissionsTotal + x.PoolRewardTotal + x.PersonalCommissionsTotal);
            }
            adminIndexModel.LastCommissionValue = TotalLastCom;
            //پراکندگی استانی سازمان فروش
            List<DispersalofData> Dispersalofsalesorganization = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive).Select(s => s.UserRole).GroupBy(g => g.User.County.State)
                .Select(x => new DispersalofData
                {
                    Title = x.Key.StateName,
                    DataCount = x.Count()
                }).ToList();


            List<string> pieChartStateLabels = Dispersalofsalesorganization.OrderByDescending(x => x.DataCount).Select(x => x.Title).ToList();
            List<double?> pieChartStateData = Dispersalofsalesorganization.OrderByDescending(x => x.DataCount).Select(x => x.DataCount).ToList();
            List<string> CBgColors = new List<string>();
            Random rnd = new Random();
            for (int s = 0; s < pieChartStateData.Count(); s++)
            {
                string cc = String.Format("#{0:X6}", rnd.Next(0x1000000));
                CBgColors.Add(cc);
            }
            List<ChartDatasetModel> chartDatasetModels = new List<ChartDatasetModel>();
            ChartDatasetModel chartDataset = new ChartDatasetModel()
            {
                Label = "First",
                Data = pieChartStateData,
                BgColors = CBgColors
            };
            chartDatasetModels.Add(chartDataset);
            ChartDataModel chartDataModel = new ChartDataModel()
            {
                Labels = pieChartStateLabels,
                ChartDatasetModels = chartDatasetModels
            };
           
            SimplePieChartModel simplePieChartModel = new SimplePieChartModel()
            {
                Labels = pieChartStateLabels,
                Data = pieChartStateData,
                BgColors = CBgColors
            };
            adminIndexModel.SimplePieChartModel = simplePieChartModel;
            var test = AllBrdroes.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive)).GroupBy(g => _bordroService.GetBranchfromInsNo(g.InsNO)).ToList();
            List<DispersalofData> dispersalofBrIssuesUnit = AllBrdroes.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive)).GroupBy(g => _bordroService.GetBranchfromInsNo(g.InsNO))
                    .Select(x => new DispersalofData
                    {
                        Title = _bordroService.GetBranchNameByCode(x.Key),
                        DataCount = x.Count()
                    }).ToList();
            Random Brrnd = new Random();
            List<string> BrBgColors = new List<string>();
            for (int s = 0; s < dispersalofBrIssuesUnit.Count(); s++)
            {
                string cc = String.Format("#{0:X6}", Brrnd.Next(0x1000000));
                BrBgColors.Add(cc);
            }
            SimplePieChartModel BrPieChartModel = new SimplePieChartModel()
            {
                Labels = dispersalofBrIssuesUnit.OrderByDescending(x => x.DataCount).Where(w => !string.IsNullOrEmpty(w.Title)).ToList().Select(x => x.Title).ToList(),
                Data = dispersalofBrIssuesUnit.OrderByDescending(x => x.DataCount).Where(w => !string.IsNullOrEmpty(w.Title)).ToList().Select(x => x.DataCount).ToList(),
                BgColors = BrBgColors
            };
            adminIndexModel.BranchesDistChart = BrPieChartModel;
            //========================================
            ColumnChartModel columnChartModel = new ColumnChartModel();
            columnChartModel.Labels = DateTime.Now.GetCurrentSeasionMonthsName();
            List<int> curSeaMonths = DateTime.Now.GetCurrentSeasionMonthsNumber();
            int month1 = curSeaMonths.FirstOrDefault();
            int month2 = curSeaMonths.Skip(1).Take(1).FirstOrDefault();
            int month3 = curSeaMonths.Skip(2).Take(1).FirstOrDefault();
            //Man data set
            int month1Men = 0;
            month1Men = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive && w.UserRole.User.Sex == 1 && PC.GetMonth(w.UserRole.User.RegDate) == month1).Count();
            int month2Men = 0;
            month2Men = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive && w.UserRole.User.Sex == 1 && PC.GetMonth(w.UserRole.User.RegDate) == month2).Count();
            int month3Men = 0;
            month3Men = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive && w.UserRole.User.Sex == 1 && PC.GetMonth(w.UserRole.User.RegDate) == month3).Count();

            List<int> MenData = new List<int>
            {
                month1Men,
                month2Men,
                month3Men
            };
            List<int> Mendata = MenData.ToList();
            
            ColumnDatasetModel ManDataset = new ColumnDatasetModel()
            {
                Label="مرد",
                BgColor = "#673AB7",
                BorderColor= "transparent",
                HoverBgColor= "rgba(103,58,183,.9)",
                Data = Mendata
            };
            columnChartModel.ColumnDataset1 = ManDataset;
            columnChartModel.data1 = Mendata;
            //women
            int month1WoMen = 0;
            month1WoMen = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive && w.UserRole.User.Sex == 0 && PC.GetMonth(w.UserRole.User.RegDate) == month1).Count();
            int month2WoMen = 0;
            month2WoMen = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive && w.UserRole.User.Sex == 0 && PC.GetMonth(w.UserRole.User.RegDate) == month2).Count();
            int month3WoMen = 0;
            month3WoMen = OrguserRoles.Where(w => w.UserRole.User.IsActive && w.UserRole.IsActive && w.UserRole.User.Sex == 0 && PC.GetMonth(w.UserRole.User.RegDate) == month3).Count();
            List<int> WoMenData = new List<int>
            {
                month1WoMen,
                month2WoMen,
                month3WoMen
            };
            string x = DateTime.Now.GetCurrentSeason();
            ColumnDatasetModel WoManDataset = new ColumnDatasetModel()
            {
                Label = "زن",
                BgColor = "#673AB7",
                BorderColor = "transparent",
                HoverBgColor = "rgba(233,30,99,.9)",
                Data = WoMenData
            };
            columnChartModel.ColumnDataset2 = WoManDataset;
            columnChartModel.data2 = WoMenData;
            List<ColumnDatasetModel> columnDatasetModels = new List<ColumnDatasetModel>()
            {
                ManDataset,
                WoManDataset
            };
            columnChartModel.ColumnDatasetModels = columnDatasetModels;
            adminIndexModel.ColumnChartModel = columnChartModel;
            return View(adminIndexModel);

        }
        public IActionResult UploadNC()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadNC(UploadDocumentViewModel uploadDocumentViewModel, IFormFile NCFileName)
        {
            if (NCFileName == null)
            {
                ModelState.AddModelError("NCFileName", "تصویر کارت ملی را انتخاب کنید");
                return View(uploadDocumentViewModel);
            }
            if (NCFileName.Length > 1024 * 1025 * .05)
            {
                ModelState.AddModelError("NCFileName", "حداکثر حجم فایل 50 کیلو بایت باشد");
                return View(uploadDocumentViewModel);
            }
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            string fileName = user.NC + NCFileName.FileName;
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users", fileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                NCFileName.CopyTo(stream);
            }
            Document document = new Document()
            {
                UserId = user.Id,
                CreateDate = DateTime.Now,
                FileName = fileName,
                Name = "کارت ملی",
                Type = "nc"

            };
            _userService.CreateDocument(document);
            await _userService.SaveChangesAsync();
            user.IsActive = true;
            _userService.UpdateUser(user);
            return RedirectToAction("Index");
        }

        private static Chart GeneratePieChart(List<string> zLabels, List<double?> zData, List<ChartColor> BackgroundColors, List<ChartColor> HoverBackgroundColors)
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.Pie
            };

            Data data = new Data
            {
                Labels = zLabels //new List<string>() { "Red", "Blue", "Yellow" }
            };

            PieDataset dataset = new PieDataset()
            {
                Label = "My dataset",
                BackgroundColor = BackgroundColors,
                //new List<ChartColor>() {
                //    ChartColor.FromHexString("#FF6384"),
                //    ChartColor.FromHexString("#36A2EB"),
                //    ChartColor.FromHexString("#FFCE56")
                //},
                HoverBackgroundColor = HoverBackgroundColors,
                //new List<ChartColor>() {
                //    ChartColor.FromHexString("#FF6384"),
                //    ChartColor.FromHexString("#36A2EB"),
                //    ChartColor.FromHexString("#FFCE56")
                //},
                Data = zData //new List<double?>() { 300, 50, 100 }
            };

            data.Datasets = new List<Dataset>
            {
                dataset
            };

            chart.Data = data;

            return chart;
        }
        private static Chart GeneratePieChart2()
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
        //public IActionResult PieChart()
        //{
        //    //var chartData = @"
        //    //{
        //    //    type: 'bar',
        //    //    responsive: true,
        //    //    data:
        //    //    {
        //    //        labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
        //    //        datasets: [{
        //    //            label: '# of Votes',
        //    //            data: [12, 19, 3, 5, 2, 3],
        //    //            backgroundColor: [
        //    //            'rgba(255, 99, 132, 0.2)',
        //    //            'rgba(54, 162, 235, 0.2)',
        //    //            'rgba(255, 206, 86, 0.2)',
        //    //            'rgba(75, 192, 192, 0.2)',
        //    //            'rgba(153, 102, 255, 0.2)',
        //    //            'rgba(255, 159, 64, 0.2)'
        //    //                ],
        //    //            borderColor: [
        //    //            'rgba(255, 99, 132, 1)',
        //    //            'rgba(54, 162, 235, 1)',
        //    //            'rgba(255, 206, 86, 1)',
        //    //            'rgba(75, 192, 192, 1)',
        //    //            'rgba(153, 102, 255, 1)',
        //    //            'rgba(255, 159, 64, 1)'
        //    //                ],
        //    //            borderWidth: 1
        //    //        }]
        //    //    },
        //    //    options:
        //    //    {
        //    //        scales:
        //    //        {
        //    //            yAxes: [{
        //    //                ticks:
        //    //                {
        //    //                    beginAtZero: true
        //    //                }
        //    //            }]
        //    //        }
        //    //    }
        //    //}";
        //    var chartData = GeneratePieChart2();
        //    var chart = JsonConvert.DeserializeObject<ChartJSCore.Models.Chart>(chartData.ToString());
        //    var chartModel = new ChartJsViewModel
        //    {
        //        Chart = chart,
        //        ChartJson = JsonConvert.SerializeObject(chart, new JsonSerializerSettings
        //        {
        //            NullValueHandling = NullValueHandling.Ignore
        //        })
        //    };
        //    return PartialView(chartModel);
        //}
    }
}