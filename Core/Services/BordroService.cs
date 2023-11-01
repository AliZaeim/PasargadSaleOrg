using Core.Convertors;
using Core.DTOs.Admin;
using Core.DTOs.General;
using Core.Services.Interfaces;
using Core.Utility;
using DataLayer.Context;
using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Core.Services
{
    public class BordroService : IBordroService
    {
        private readonly MyContext _Context;

        public BordroService(MyContext Context)
        {
            _Context = Context;

        }
        #region generic
        public void SaveChange()
        {
            _Context.SaveChanges();
        }

        public async Task SaveChangeAsync()
        {
            await _Context.SaveChangesAsync();
        }
        public bool CreateTextFile(string Filename, string data)
        {
            //@"E:\AppServ\Example.txt";

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/txtFiles", Filename + ".txt");
            if (!File.Exists(filePath))
            {
                using var txtFile = File.AppendText(filePath);

                txtFile.WriteLine(data);
                return true;
            }
            else if (File.Exists(filePath))
            {
                return false;
            }
            return false;
        }
        public DataTable ConvertListToDataTable<T>(List<T> Data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in Data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public IWorkbook WriteExcelWithNPOI<T>(T Entity, List<T> data, string title, string extension = "xlsx")
        {
            // Get DataTable
            DataTable dt = ConvertListToDataTable(data);
            // Instantiate Wokrbook
            IWorkbook workbook;
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (extension == "xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                throw new Exception("The format '" + extension + "' is not supported.");
            }
            //make top row
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            sheet1.IsRightToLeft = true;
            IFont TopRowFont = workbook.CreateFont();
            TopRowFont.FontName = "topFont";
            TopRowFont.IsBold = true;
            TopRowFont.FontHeight = 350;

            IRow topRow = sheet1.CreateRow(0);
            var CellStyleTop = workbook.CreateCellStyle();
            CellStyleTop.Alignment = HorizontalAlignment.Center;
            CellStyleTop.VerticalAlignment = VerticalAlignment.Center;
            CellStyleTop.SetFont(TopRowFont);
            ICell cellTop = topRow.CreateCell(0);
            cellTop.CellStyle = CellStyleTop;
            cellTop.SetCellValue(title);

            var cra = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dt.Columns.Count - 1);
            sheet1.AddMergedRegion(cra);

            //make a header row
            IFont font1 = workbook.CreateFont();
            font1.FontName = "Font1";
            font1.IsBold = true;
            font1.Color = IndexedColors.Black.Index;



            IRow row1 = sheet1.CreateRow(1);
            var CellStyleHeader = workbook.CreateCellStyle();
            CellStyleHeader.Alignment = HorizontalAlignment.Center;
            CellStyleHeader.VerticalAlignment = VerticalAlignment.Center;

            // center-align currency values
            CellStyleHeader.Alignment = HorizontalAlignment.Center;
            CellStyleHeader.VerticalAlignment = VerticalAlignment.Center;
            CellStyleHeader.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            CellStyleHeader.FillPattern = FillPattern.SolidForeground;
            CellStyleHeader.SetFont(font1);



            var CellStyleBody = workbook.CreateCellStyle();
            // center-align currency values
            CellStyleBody.Alignment = HorizontalAlignment.Center;
            CellStyleBody.VerticalAlignment = VerticalAlignment.Center;




            PropertyInfo[] props = Entity.GetType().GetProperties();
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string Title = MyUtility.GetDisplayName(props[j]);
                if (!string.IsNullOrEmpty(Title))
                {
                    cell.SetCellValue(Title);
                    cell.CellStyle = CellStyleHeader;
                }


            }

            //loops through data
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    string columnName = dt.Columns[j].ToString();
                    string columnValue = dt.Rows[i][columnName].ToString();
                    string Title = MyUtility.GetDisplayName(props[j]);
                    if (columnName == "SalesOrg")
                    {
                        string[] cellval = columnValue.Split("|");
                        string nstr = string.Empty;
                        int loop = 1;
                        foreach (var item in cellval)
                        {
                            if (item != cellval.LastOrDefault())
                            {
                                nstr += $"{item}\n";
                            }
                            else
                            {
                                nstr += item;
                            }
                            loop++;
                        }
                        cell.SetCellValue(nstr);

                        ICellStyle cs = workbook.CreateCellStyle();
                        cs.Alignment = HorizontalAlignment.Center;
                        cs.VerticalAlignment = VerticalAlignment.Center;
                        cs.WrapText = true;
                        cs.ShrinkToFit = true;
                        cell.CellStyle = cs;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Title))
                        {
                            cell.CellStyle.WrapText = true;
                            cell.CellStyle = CellStyleBody;
                            if (dt.Columns[j].DataType.IsNumeric())
                            {
                                cell.SetCellValue(double.Parse(columnValue));
                            }
                            else
                            {
                                cell.SetCellValue(columnValue);
                            }


                        }

                    }

                }
            }
            // Auto size columns
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < row1.LastCellNum; j++)
                {
                    sheet1.AutoSizeColumn(j);
                }
            }
            return workbook;
        }
        public (string InsNo, int AddNo) GetInsNo_and_AddNoFromString(string ComplexInsNo, string Seperator = "_")
        {
            if (string.IsNullOrEmpty(ComplexInsNo))
            {
                return (null, 0);
            }
            if (ComplexInsNo.Contains(Seperator))
            {
                string[] ins = ComplexInsNo.Split(Seperator);
                if (ins.Length >= 2)
                {
                    string insN = ins[0].ToString();
                    int No = int.Parse(ins[1].ToString());
                    return (insN.Replace(" ", ""), No);
                }
                else
                {
                    return (null, 0);
                }
            }
            else
            {
                return (null, 0);
            }
        }
        public string GetBranchfromInsNo(string InsNo)
        {
            if (string.IsNullOrEmpty(InsNo))
            {
                return string.Empty;
            }
            if (!InsNo.Contains("/"))
            {
                return string.Empty;
            }
            string[] arr = InsNo.Split("/");
            if (arr.Length != 0)
            {
                return arr[1];
            }
            return string.Empty;
        }
        public string GetBranchNamefromInsNo(string InsNo)
        {
            string brCode = GetBranchfromInsNo(InsNo);
            return _Context.Branches.FirstOrDefault(f => f.BrCode == brCode).BrName;
        }
        public string GetBranchNameByCode(string code)
        {
            Branch branch = _Context.Branches.FirstOrDefault(f => f.BrCode == code.Trim());
            if (branch == null)
            {
                return null;
            }
            return branch.BrName;
        }
        #endregion
        #region Uploadinfo
        public void CreateUploadInfo(UploadInfo uploadInfo)
        {
            _Context.UploadInfos.Add(uploadInfo);
        }
        #endregion

        #region bordro
        public void CreateLifeBordroAdditionCollection(List<LifeBordroAddition> lifeBordroAdditions)
        {
            _Context.LifeBordroAdditions.AddRange(lifeBordroAdditions);
        }

        public void CreateLifeBordroBase(LifeBordroBase lifeBordroBase)
        {
            _Context.LifeBordroBases.Add(lifeBordroBase);
        }

        public void CreateLifeBordroBaseCollection(List<LifeBordroBase> lifeBordroBases)
        {
            _Context.LifeBordroBases.AddRange(lifeBordroBases);
        }
        public void CreateCommissionBase(CommissionBase commissionBase)
        {
            _Context.CommissionBases.Add(commissionBase);
        }
        public int GetBordroExcelFileColumnIndex(string ColTitle)
        {
            return _Context.BordroFileSettings.FirstOrDefault(f => f.ColTitle.Trim() == ColTitle).ColIndex;
        }
        public List<Object> ReadExcel(string root)
        {
            using ExcelPackage package = new ExcelPackage(new FileInfo(root));
            ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
            List<object> obj = new List<object>();
            for (int i = 10; i <= workSheet.Dimension.Rows; i++)
            {
                LifeBordroAddition lfb = new LifeBordroAddition()
                {
                    InsurerNC = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد ملی بیمه گذار")].Value.ToString(),
                    InsurerFullName = workSheet.Cells[i, GetBordroExcelFileColumnIndex("بیمه گذار")].Value.ToString(),
                    Duration = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("مدت بیمه")].Value.ToString()),
                    PaymentMethod = workSheet.Cells[i, GetBordroExcelFileColumnIndex("روش پرداخت")].Value.ToString(),
                    LFPremium = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("حق بیمه عمر  وتامبن آتیه")].Value.ToString()),
                    SupPremium = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("حق بیمه تکمیلی")].Value.ToString()),
                    PremiumbyPaymentMethod = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("حق بیمه بر حسب روش پرداخت")].Value.ToString()),
                    Deposit = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("سپرده")].Value.ToString()),
                    CapitalDied = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("سرمایه خطر فوت")].Value.ToString()),
                    SellerCode = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد پایه نماینده")].Value.ToString(),
                    SellerNC = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد ملی نماینده")].Value.ToString(),
                    Seller = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد و نام نماینده فروش")].Value.ToString(),
                    Status = workSheet.Cells[i, GetBordroExcelFileColumnIndex("وضعیت بیمه نامه")].Value.ToString(),
                    InsuredNC = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد ملی بیمه شده")].Value.ToString(),
                    InsuredFullName = workSheet.Cells[i, GetBordroExcelFileColumnIndex("بیمه شده")].Value.ToString(),
                    Type = workSheet.Cells[i, GetBordroExcelFileColumnIndex("نوع الحاقیه")].Value.ToString(),

                };



                if (!string.IsNullOrEmpty(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع اولیه")].Value.ToString()))
                {
                    lfb.InitialStartDate = DateConvertor.ChangeToMiladiWithoutTime(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع اولیه")].Value.ToString());
                }
                if (!string.IsNullOrEmpty(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع الحاقیه")].Value.ToString()))
                {
                    lfb.InitialStartDate = DateConvertor.ChangeToMiladiWithoutTime(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع الحاقیه")].Value.ToString());
                }
                obj.Add(lfb);
            }
            return obj;
        }

        public List<LifeBordroAddition> Read_and_PrepareLifeBordroAdditionList_FromExcelFile(string SheetName, string FileName, string Root)
        {
            string rootFolder = Root;
            string fileName = FileName;
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            using ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet workSheet = package.Workbook.Worksheets[SheetName];
            int totalRows = workSheet.Dimension.Rows;

            List<LifeBordroAddition> MyList = new List<LifeBordroAddition>();

            for (int i = 10; i <= totalRows; i++)
            {

                LifeBordroAddition lfb = new LifeBordroAddition()
                {
                    InsurerNC = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد ملی بیمه گذار")].Value.ToString(),
                    InsurerFullName = workSheet.Cells[i, GetBordroExcelFileColumnIndex("بیمه گذار")].Value.ToString(),
                    Duration = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("مدت بیمه")].Value.ToString()),
                    PaymentMethod = workSheet.Cells[i, GetBordroExcelFileColumnIndex("روش پرداخت")].Value.ToString(),
                    LFPremium = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("حق بیمه عمر  وتامبن آتیه")].Value.ToString()),
                    SupPremium = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("حق بیمه تکمیلی")].Value.ToString()),
                    PremiumbyPaymentMethod = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("حق بیمه بر حسب روش پرداخت")].Value.ToString()),
                    Deposit = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("سپرده")].Value.ToString()),
                    CapitalDied = int.Parse(workSheet.Cells[i, GetBordroExcelFileColumnIndex("سرمایه خطر فوت")].Value.ToString()),
                    SellerCode = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد پایه نماینده")].Value.ToString(),
                    SellerNC = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد ملی نماینده")].Value.ToString(),
                    Seller = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد و نام نماینده فروش")].Value.ToString(),
                    Status = workSheet.Cells[i, GetBordroExcelFileColumnIndex("وضعیت بیمه نامه")].Value.ToString(),
                    InsuredNC = workSheet.Cells[i, GetBordroExcelFileColumnIndex("کد ملی بیمه شده")].Value.ToString(),
                    InsuredFullName = workSheet.Cells[i, GetBordroExcelFileColumnIndex("بیمه شده")].Value.ToString(),
                    Type = workSheet.Cells[i, GetBordroExcelFileColumnIndex("نوع الحاقیه")].Value.ToString(),

                };


                //if (!string.IsNullOrEmpty(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ صدور")].Value.ToString()))
                //{
                //    lfb.IssueDate = DateConvertor.ChangeToMiladiWithoutTime(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ صدور")].Value.ToString());
                //}
                if (!string.IsNullOrEmpty(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع اولیه")].Value.ToString()))
                {
                    lfb.InitialStartDate = DateConvertor.ChangeToMiladiWithoutTime(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع اولیه")].Value.ToString());
                }
                if (!string.IsNullOrEmpty(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع الحاقیه")].Value.ToString()))
                {
                    lfb.InitialStartDate = DateConvertor.ChangeToMiladiWithoutTime(workSheet.Cells[i, GetBordroExcelFileColumnIndex("تاریخ شروع الحاقیه")].Value.ToString());
                }
                MyList.Add(lfb);

            }



            return MyList;
        }
        public async Task<bool> RemoveBordroByYearMounthAsync(int year, int mounth)
        {
            List<LifeBordroBase> lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).Where(s => s.Year == year && s.Mounth == mounth).ToListAsync();
            if (lifeBordroBases == null)
            {
                return false;
            }
            _Context.LifeBordroBases.RemoveRange(lifeBordroBases);
            return true;
        }
        public Task<bool> ExistInsNOinBordroAdditionAsync(string InsNO)
        {
            throw new NotImplementedException();
        }
        public async Task<CompareTwoBaseBordroListsVM> GetDiffrenceNewBordroWithDbAsync(List<PasargadBordroViewModel> pasargadbordroes, string action, bool FileIsValid, int Year, int Mounth)
        {
            if (FileIsValid == true)
            {
                int TotalFileRecordCount = pasargadbordroes.Count();
                pasargadbordroes = pasargadbordroes.Where(w => !string.IsNullOrEmpty(w.AgentNC)).GroupBy(g => g.InsNO).Select(s => s.First()).ToList();
                pasargadbordroes = pasargadbordroes.Where(w => _Context.Users.Select(s => s.NC).Any(a => a == w.AgentNC)).ToList();
                List<LifeBordroBase> bordroBseses = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToListAsync();
                List<LifeBordroBase> Exist_indB = bordroBseses.Where(w => pasargadbordroes.Select(s => s.InsNO).Any(a => a == w.InsNO)).ToList();
                List<LifeBordroBase> AllP = pasargadbordroes.Select(s => new LifeBordroBase { InsNO = s.InsNO.Replace(" ", string.Empty), IssueDate = s.IssueDate.ToMiladi(), Year = Year, Mounth = Mounth, IsActive = true, CreateDate = DateTime.Now }).ToList();
                List<LifeBordroBase> Additional_inFile = AllP.Where(w => !bordroBseses.Select(s => s.InsNO.Replace(" ", string.Empty)).Any(a => a == w.InsNO.Replace(" ", string.Empty))).ToList();
                List<LifeBordroBase> Additional_inDb = bordroBseses.Where(w => !AllP.Select(s => s.InsNO.Replace(" ", string.Empty)).Any(a => a == w.InsNO.Replace(" ", string.Empty)) && w.IsActive == true).ToList();
                //List<LifeBordroBase> 
                CompareTwoBaseBordroListsVM compareTwoBaseBordroListsVM = new CompareTwoBaseBordroListsVM()
                {
                    AdditionalDatainUploadedFile = Additional_inFile,
                    AdditionalDatainDb = Additional_inDb,
                };
                bool AdditionalDatainDb = false;
                bool AdditionalDatainUploadedFile = false;
                if (compareTwoBaseBordroListsVM.AdditionalDatainDb != null)
                {
                    if (compareTwoBaseBordroListsVM.AdditionalDatainDb.Count() != 0)
                    {
                        AdditionalDatainDb = true;
                    }
                }
                if (compareTwoBaseBordroListsVM.AdditionalDatainUploadedFile != null)
                {
                    if (compareTwoBaseBordroListsVM.AdditionalDatainUploadedFile.Count != 0)
                    {
                        AdditionalDatainUploadedFile = true;
                    }
                }
                if (AdditionalDatainUploadedFile == true && AdditionalDatainDb == true)
                {
                    if (action == "add")
                    {
                        compareTwoBaseBordroListsVM.ActiveSubmit = true;
                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-success upl m-t-10'>" + "تعداد " + " " + compareTwoBaseBordroListsVM.AdditionalDatainUploadedFile.Count() + " " + "رکورد" + " " + "از" + " " + TotalFileRecordCount.ToString() + " " + "رکورد موجود در فایل به سیستم اضافه خواهد شد </p>";

                    }
                    if (action == "replace")
                    {
                        compareTwoBaseBordroListsVM.ActiveSubmit = true;
                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-warning upl m-t-10'>" + "تعداد  " + " " + compareTwoBaseBordroListsVM.AdditionalDatainDb.Count() + " " + "رکورد در سیستم مازاد بر فایل وجود دارد </p>"
                        + "<p class='text-justify alert alert-danger upl m-t-10'>" + "توجه : فایل انتخابی جایگزین اطلاعات موجود خواهد شد" + "</p>";

                    }

                    compareTwoBaseBordroListsVM.ActiveSubmit = true;
                    compareTwoBaseBordroListsVM.ConfAdd = true;
                    compareTwoBaseBordroListsVM.ConfUpdate = true;
                    compareTwoBaseBordroListsVM.Action = action;
                    return compareTwoBaseBordroListsVM;
                }

                if (AdditionalDatainDb == false && AdditionalDatainUploadedFile == false)
                {
                    if (action == "add")
                    {
                        compareTwoBaseBordroListsVM.ConfAdd = false;
                        compareTwoBaseBordroListsVM.ConfUpdate = false;
                        compareTwoBaseBordroListsVM.Action = "none";
                        compareTwoBaseBordroListsVM.ActiveSubmit = false;
                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-danger upl m-t-10'>" + "در این فایل رکورد جدیدی وجود ندارد !" + "</p>";
                    }
                    if (action == "replace")
                    {
                        compareTwoBaseBordroListsVM.ConfAdd = false;
                        compareTwoBaseBordroListsVM.ConfUpdate = true;
                        compareTwoBaseBordroListsVM.Action = "replace";
                        compareTwoBaseBordroListsVM.ActiveSubmit = true;
                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-warning upl m-t-10'>" + "در این فایل رکورد جدیدی وجود ندارد !" + "</p>"
                            + "<p class='text-justify alert alert-danger upl m-t-10'>" + "توجه : فایل انتخابی جایگزین اطلاعات موجود خواهد شد" + "</p>"; ;
                    }

                    return compareTwoBaseBordroListsVM;
                }
                if (AdditionalDatainDb == true)
                {
                    if (action == "add")
                    {
                        compareTwoBaseBordroListsVM.ActiveSubmit = false;
                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-warning upl m-t-10'>" + "تعداد  " + " " + compareTwoBaseBordroListsVM.AdditionalDatainDb.Count() + " " + "رکورد در سیستم مازاد بر فایل وجود دارد </p>" + " "
                        + "<p class='text-justify alert alert-danger upl m-t-10'>" + "در این فایل رکورد جدیدی وجود ندارد !" + "</p>";
                    }
                    if (action == "replace")
                    {
                        compareTwoBaseBordroListsVM.ActiveSubmit = true;
                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-warning upl m-t-10'>" + "تعداد  " + " " + compareTwoBaseBordroListsVM.AdditionalDatainDb.Count() + " " + "رکورد در سیستم مازاد بر فایل وجود دارد </p>" + " "
                            + "<p class='text-justify alert alert-danger upl m-t-10'>" + "توجه : فایل انتخابی جایگزین اطلاعات موجود خواهد شد" + "</p>";
                        ;

                    }
                    compareTwoBaseBordroListsVM.ConfAdd = false;
                    compareTwoBaseBordroListsVM.ConfUpdate = false;
                    compareTwoBaseBordroListsVM.Action = action;
                    return compareTwoBaseBordroListsVM;
                }

                if (AdditionalDatainUploadedFile == true)
                {
                    if (action == "add")
                    {
                        compareTwoBaseBordroListsVM.ActiveSubmit = true;
                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-success upl m-t-10'>" + "تعداد " + " " + compareTwoBaseBordroListsVM.AdditionalDatainUploadedFile.Count() + " " + "رکورد" + " " + "از" + " " + TotalFileRecordCount.ToString() + " " + "رکورد موجود در فایل به سیستم اضافه خواهد شد </p>";

                    }
                    if (action == "replace")
                    {
                        compareTwoBaseBordroListsVM.ActiveSubmit = true;

                        compareTwoBaseBordroListsVM.Message = "<p class='text-justify alert alert-warning upl m-t-10'>" + " " + "تعداد" + " " + compareTwoBaseBordroListsVM.AdditionalDatainUploadedFile.Count + " " + "رکورد در فایل مازاد بر سیستم وجود دارد" + "</p>"
                            + "<p class='text-justify alert alert-danger upl m-t-10'>" + " " + "توجه : فایل انتخابی جایگزین اطلاعات موجود خواهد شد" + "</p>";


                    }
                    compareTwoBaseBordroListsVM.ActiveSubmit = true;
                    compareTwoBaseBordroListsVM.ConfAdd = true;
                    compareTwoBaseBordroListsVM.ConfUpdate = true;
                    compareTwoBaseBordroListsVM.Action = action;
                    return compareTwoBaseBordroListsVM;
                }

                return compareTwoBaseBordroListsVM;
            }
            else
            {
                CompareTwoBaseBordroListsVM compareTwoBaseBordroListsVM = new CompareTwoBaseBordroListsVM()
                {
                    ActiveSubmit = false,

                };
                return compareTwoBaseBordroListsVM;
            }



        }
        public AdditionFileToUploadResultModel GetDiffrenceNewAdditionWithDbAsync(List<PasargadBordroViewModel> pasargadadditions, string action, bool FileIsValid, string FormActionType)
        {
            if (FileIsValid == true)
            {
                int TotalFileRecordCount = pasargadadditions.Count();
                string mess = string.Empty;
                List<LifeBordroAddition> CommonlifeBordroAdditions_inDB = null;
                //Get additions that agents are in database
                pasargadadditions = pasargadadditions.Where(w => _Context.Users.Select(s => s.NC).Any(a => a == w.AgentNC)).ToList();
                if (action == "add")
                {
                    //Get additions that InsNo is in db  
                    pasargadadditions = pasargadadditions.Where(w => _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).ToList().Any(a => a.LifeBordroBase.InsNO == GetInsNo_and_AddNoFromString(w.InsNO).InsNo
                    )).ToList();
                    //Get pasargadAdditions that number in not in db
                    pasargadadditions = pasargadadditions.Where(w => _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToList().Any(a => a.InsNO == GetInsNo_and_AddNoFromString(w.InsNO).InsNo
                    && !a.LifeBordroAdditions.Any(n => n.Number == GetInsNo_and_AddNoFromString(w.InsNO).AddNo))).ToList();

                    if (FormActionType == "select")
                    {
                        mess = " تعداد " + pasargadadditions.Count + " رکورد به دیتابیس اضافه خواهد شد ";
                    }
                    if (FormActionType == "upload")
                    {
                        mess = " تعداد " + pasargadadditions.Count + " رکورد به دیتابیس اضافه  شد ";
                    }

                }
                if (action == "replace")
                {
                    //Get additions that InsNo is in db and addition number is not in db
                    pasargadadditions = pasargadadditions.Where(w => _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).ToList().Any(a => a.LifeBordroBase.InsNO == GetInsNo_and_AddNoFromString(w.InsNO).InsNo)).ToList();
                    CommonlifeBordroAdditions_inDB = _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).ToList().Where(w => pasargadadditions
                     .Any(a => w.LifeBordroBase.InsNO == GetInsNo_and_AddNoFromString(a.InsNO).InsNo && w.Number == GetInsNo_and_AddNoFromString(a.InsNO).AddNo)).ToList();
                    if (FormActionType == "select")
                    {
                        mess = "<span class='text-danger'>" + "تعداد " + CommonlifeBordroAdditions_inDB.Count + " رکورد از دیتابیس حذف " + "</span>" + "<br />" + "و" + "<br />" + " تعداد " + pasargadadditions.Count + " " + "رکورد به دیتابیس اضافه خواهد شد";
                    }
                    if (FormActionType == "upload")
                    {
                        mess = "<span class='text-danger'>" + "تعداد " + CommonlifeBordroAdditions_inDB.Count + " رکورد از دیتابیس حذف " + "</span>" + "<br />" + "و" + "<br />" + " تعداد " + pasargadadditions.Count + " " + "رکورد به دیتابیس اضافه شد";
                    }

                }
                string result = string.Empty;

                List<LifeBordroAddition> QlifeBordroAdditions = pasargadadditions.Where(w => _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).ToList().Any(a => a.LifeBordroBase.InsNO == GetInsNo_and_AddNoFromString(w.InsNO).InsNo && a.Number != GetInsNo_and_AddNoFromString(w.InsNO).AddNo)).ToList()
                        .Select(w => new LifeBordroAddition
                        {

                            LifeBordroBase = _Context.LifeBordroBases.ToList().FirstOrDefault(f => f.InsNO == GetInsNo_and_AddNoFromString(w.InsNO).InsNo),
                            LBBId = _Context.LifeBordroBases.ToList().FirstOrDefault(f => f.InsNO == GetInsNo_and_AddNoFromString(w.InsNO).InsNo).Id,
                            InsurerFullName = w.Insurer,
                            InsurerNC = w.InsurerNC,
                            InsuredFullName = w.Insured,
                            InsuredNC = w.InsuredNC,
                            CreateDate = DateTime.Now,
                            InitialStartDate = w.InitialStartDate.ChangeToMiladiWithoutTime(),
                            StartDate = w.StartDate.ChangeToMiladiWithoutTime(),
                            Duration = int.Parse(w.Duration),
                            PaymentMethod = w.PayMethod,
                            LFPremium = int.Parse(w.LifePremium),
                            SupPremium = int.Parse(w.SupPremium),
                            PremiumbyPaymentMethod = int.Parse(w.PremiumByPay),
                            Deposit = int.Parse(w.Deposit),
                            CapitalDied = int.Parse(w.LifeCapital),
                            Seller = w.AgentName,
                            SellerCode = w.AgentCode,
                            SellerNC = w.AgentNC,
                            Status = w.Status,
                            Type = w.Type,
                            SalesSeries = GetStringParentSeries(GetUserRoleByNC(w.AgentNC).URId, 1, new List<Seller_Rate_PercentViewModel>(), result),
                            Number = GetInsNo_and_AddNoFromString(w.InsNO).AddNo,
                            IsActive = true

                        }).ToList();


                if (pasargadadditions.Count() != 0)
                {
                    AdditionFileToUploadResultModel additionFileToUploadResultModel = new AdditionFileToUploadResultModel()
                    {
                        ActiveSubmit = true,
                        QualifiedAdditions = QlifeBordroAdditions,
                        AdditionalRecordinDb = CommonlifeBordroAdditions_inDB,
                        Message = "<h6 class='text-xs-center alert alert-success m-t-10 upl'>" + mess + "</h6>"
                    };

                    return additionFileToUploadResultModel;
                }
                else
                {
                    AdditionFileToUploadResultModel additionFileToUploadResultModel = new AdditionFileToUploadResultModel()
                    {
                        ActiveSubmit = false,
                        QualifiedAdditions = QlifeBordroAdditions,
                        AdditionalRecordinDb = CommonlifeBordroAdditions_inDB,
                        Message = "<h6 class='text-xs-center alert alert-warning m-t-10 upl'>" + "رکورد معتبری برای آپلود یافت نشد !" + "</h6>"
                    };



                    return additionFileToUploadResultModel;
                }


            }
            else
            {
                AdditionFileToUploadResultModel additionFileToUploadResultModel = new AdditionFileToUploadResultModel()
                {
                    ActiveSubmit = false
                };
                return additionFileToUploadResultModel;
            }

        }
        public InsuredFileToUpoadResultModel GetDiffrenceNewInsuredInfoWithDbAsync(List<PasargadInsuredInfoModel> pasargadInsuredInfos, string action, bool FileIsValid)
        {
            InsuredFileToUpoadResultModel insuredFileToUpoadResultModel = new InsuredFileToUpoadResultModel();
            if (FileIsValid)
            {
                int TotalFileRecordCount = pasargadInsuredInfos.Count();
                string mess = string.Empty;

                List<InsuredInformation> InsuredInfoDbRecords_areinDB = null;
                if (action == "add")
                {
                    //action Method : only that records of file there are not in db added to db
                    pasargadInsuredInfos = pasargadInsuredInfos.Where(w => _Context.LifeBordroBases.Any(a => a.InsNO == w.InsNO.Replace(" ", ""))
                    && !_Context.InsuredInformation.Any(n => n.InsNO == w.InsNO.Replace(" ", ""))
                    ).ToList();

                    mess = " تعداد " + pasargadInsuredInfos.Count + " رکورد به دیتابیس اضافه خواهد شد ";
                }
                if (action == "replace")
                {
                    // new verify record(records that are in bordro table and not in insuredInfo table) in file will be add to db
                    // duplicare InsNo that are in bordro table and insuredInfo table, first delete and again add to db 

                    //1-verify file records that are be in bordro table
                    pasargadInsuredInfos = pasargadInsuredInfos.Where(w => _Context.LifeBordroBases.Any(a => a.InsNO == w.InsNO.Replace(" ", ""))).ToList();
                    InsuredInfoDbRecords_areinDB = _Context.InsuredInformation.ToList().Where(w => pasargadInsuredInfos.Any(a => a.InsNO.Replace(" ", "") == w.InsNO)).ToList();
                    pasargadInsuredInfos = pasargadInsuredInfos.Where(w => !_Context.InsuredInformation.Any(a => a.InsNO == w.InsNO)).ToList();

                    mess = "<span class='text-danger'>" + "تعداد " + InsuredInfoDbRecords_areinDB.Count() + " رکورد از دیتابیس حذف " + "</span>" + "<br />" + "و" + "<br />" + " تعداد " + pasargadInsuredInfos.Count + " " + "رکورد به دیتابیس اضافه خواهد شد";

                }
                List<InsuredInformation> QinsuredInformationList = pasargadInsuredInfos.Select(s => new InsuredInformation
                {
                    InsNO = s.InsNO.Replace(" ", ""),
                    InsuredBirthDate = s.InsuredBirthDate.ChangeToMiladiWithoutTime(),
                    State = s.State,
                    City = s.City,
                    Address = s.Address,
                    Cellphone = s.Cellphone,
                    Phone = s.Phone

                }).ToList();
                if (pasargadInsuredInfos.Count() != 0)
                {
                    insuredFileToUpoadResultModel.ActiveSubmit = true;
                    insuredFileToUpoadResultModel.QualifiedRecords = QinsuredInformationList;
                    insuredFileToUpoadResultModel.AdditionalRecordinDb = InsuredInfoDbRecords_areinDB;
                    insuredFileToUpoadResultModel.Message = "<h6 class='text-xs-center alert alert-success m-t-10 upl'>" + mess + "</h6>";
                }
                else
                {
                    insuredFileToUpoadResultModel.ActiveSubmit = false;
                    insuredFileToUpoadResultModel.Message = "<h6 class='text-xs-center alert alert-warning m-t-10 upl'>" + "رکورد معتبری برای آپلود یافت نشد !" + "</h6>";
                }
            }
            else
            {
                insuredFileToUpoadResultModel.ActiveSubmit = false;
            }
            return insuredFileToUpoadResultModel;
        }
        private UserRole GetUserRoleByNC(string NC)
        {
            return _Context.UserRoles.Include(r => r.User).FirstOrDefault(f => f.User.NC == NC && f.IsActive == true);
        }
        public async Task<bool> ExistBordroABaseByYearMounthAsync(int Year, int Mounth)
        {
            return await _Context.LifeBordroBases.AnyAsync(a => a.Year == Year && a.Mounth == Mounth);
        }
        public async Task<List<LifeBordroBase>> GetNewLifeBordroBasesFromBFile(List<PasargadBordroViewModel> NewBordro)
        {
            List<LifeBordroBase> lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToListAsync();

            List<LifeBordroBase> Exist_lifeBordroBases = lifeBordroBases.Where(w => NewBordro.Select(s => s.InsNO.Replace(" ", string.Empty)).Any(a => w.InsNO.Replace(" ", string.Empty) == a)).ToList();
            List<LifeBordroBase> Dif_lifeBordroBases = lifeBordroBases.Except(Exist_lifeBordroBases).ToList();
            return Dif_lifeBordroBases;
        }
        public async Task<bool> CreateLifeBordroCollectionAsync(int Year, int Mounth, string LoginName, List<PasargadBordroViewModel> pasargadBordroViewModels)
        {
            if (pasargadBordroViewModels == null)
            {
                return false;
            }
            if (pasargadBordroViewModels.Count() == 0)
            {
                return false;
            }

            foreach (var item in pasargadBordroViewModels)
            {
                if (string.IsNullOrEmpty(item.IssueDate))
                {
                    string k = item.InsNO;
                }
                List<UserRole> SellerUserRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role)
                .Where(w => w.User.NC.Trim() == item.AgentNC.Trim()).ToListAsync();
                if (SellerUserRoles != null)
                {
                    if (SellerUserRoles.Count() != 0)
                    {
                        if (SellerUserRoles.Where(w => w.IsActive == true && w.RoleId != 2 && w.RoleId != 3) != null)
                        {
                            //continue;
                            UserRole Myseller = SellerUserRoles.FirstOrDefault(f => f.IsActive == true & f.RoleId != 2 && f.RoleId != 3);
                            if (Myseller != null)
                            {
                                string result = string.Empty;
                                LifeBordroBase lifeBordroBase = new LifeBordroBase
                                {
                                    InsNO = item.InsNO.Replace(" ", string.Empty),
                                    IssueDate = item.IssueDate.ChangeToMiladiWithoutTime(),
                                    CreateDate = DateTime.Now,
                                    Year = Year,
                                    Mounth = Mounth,
                                    IsActive = true,
                                    OPCreate = LoginName,
                                    LifeBordroAdditions = new List<LifeBordroAddition>
                                    {
                                        new LifeBordroAddition
                                        {
                                            Number = 0,
                                            InsurerNC = item.InsuredNC,
                                            InsurerFullName = item.Insurer,
                                            InitialStartDate = item.InitialStartDate.ChangeToMiladi("00:00"),
                                            InsuredNC = item.InsuredNC,
                                            InsuredFullName = item.Insured,
                                            Duration = int.Parse(item.Duration),
                                            PaymentMethod = item.PayMethod,
                                            LFPremium = Math.Abs(int.Parse(item.LifePremium)),
                                            SupPremium =Math.Abs(int.Parse(item.SupPremium)),
                                            PremiumbyPaymentMethod =Math.Abs(int.Parse(item.PremiumByPay)),
                                            Deposit = long.Parse(item.Deposit),
                                            CapitalDied = int.Parse(item.LifeCapital),
                                            SellerNC = item.AgentNC,
                                            SellerCode=item.AgentCode,
                                            Seller = item.AgentName,
                                            Status = item.Status,
                                            Type = item.Type,
                                            CreateDate = DateTime.Now,
                                            IsActive = true,

                                            SalesSeries = GetStringParentSeries(Myseller.URId,1,new List<Seller_Rate_PercentViewModel>(),result)

                                        }

                                    }

                                };
                                CreateLifeBordroBase(lifeBordroBase);
                            }
                        }
                    }

                }
            }
            return true;
        }
        public string GetStringParentSeries(int urId, int rate, List<Seller_Rate_PercentViewModel> seller_Rate_PercentViewModels, string res)
        {
            UserRole current = _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
               .SingleOrDefault(s => s.URId == urId && s.IsActive == true);
            if (current == null)
            {
                return string.Empty;
            }
            float per = 0;
            float Eqper = 0;
            long SumPremium = GetUserRoleActivityStatics(urId).Result.SalesSum;
            int SumStatic = GetUserRoleActivityStatics(urId).Result.SalesCount;
            RoleCommission roleCommission = _Context.RoleCommissions.OrderByDescending(r => r.MinSaleValue).OrderByDescending(r => r.MinSaleCount).FirstOrDefault(w => w.RoleId == current.RoleId && w.MinSaleCount <= SumStatic && w.MinSaleValue <= SumPremium);
            if (roleCommission != null)
            {
                if (rate == 1)
                {
                    per = roleCommission.PersonalSalesPercent;
                }
                if (rate == 2)
                {

                    Seller_Rate_PercentViewModel firsteller_Rate_PercentViewModel = seller_Rate_PercentViewModels.FirstOrDefault(w => w.Rate == 1);
                    per = roleCommission.OrganizationSalesPercent - firsteller_Rate_PercentViewModel.Percent;
                }
                if (rate > 2)
                {
                    float sumBeforRatesPercent = seller_Rate_PercentViewModels.Select(s => s.Percent).Sum();
                    per = roleCommission.OrganizationSalesPercent - sumBeforRatesPercent;
                }

            }
            if (rate > 1 && per == 0)
            {
                Seller_Rate_PercentViewModel seller_Rate_PercentViewModel_Eq = seller_Rate_PercentViewModels.FirstOrDefault(f => f.Rate == rate - 1);



                RoleEqulity roleEqulity = _Context.RoleEqulities.FirstOrDefault(f => f.RoleId == current.RoleId);
                if (roleEqulity != null)
                {
                    Eqper = roleEqulity.L1EPercent;
                }


            }
            Seller_Rate_PercentViewModel seller_Rate_PercentViewModel = new Seller_Rate_PercentViewModel { Seller = current, Rate = rate, Percent = per, UrId = urId, EqPercent = Eqper };

            seller_Rate_PercentViewModels.Add(seller_Rate_PercentViewModel);
            if (current.UserRoleParent != null)
            {
                return GetStringParentSeries((int)current.UserRoleParentId, rate + 1, seller_Rate_PercentViewModels, res);
            }
            else
            {
                int ind = 1;
                foreach (var item in seller_Rate_PercentViewModels.OrderByDescending(r => r.Rate))
                {
                    if (ind < seller_Rate_PercentViewModels.Count())
                    {
                        res += item.UrId + "-" + item.Rate + "-" + item.Percent + "-" + item.EqPercent + Environment.NewLine;
                    }
                    else
                    {
                        res += item.UrId + "-" + item.Rate + "-" + item.Percent + "-" + item.EqPercent;
                    }
                    ind++;

                }
                return res;
            }

        }

        public async Task<(long SalesSum, int SalesCount)> GetUserRoleActivityStatics(int urId)
        {
            UserRole current = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).FirstOrDefaultAsync(f => f.URId == urId);
            List<ChildRate> childRates = GetAllChilds(urId).ToList();
            long SumSales = _Context.LifeBordroAdditions.Where(w => w.SellerNC == current.User.NC).Sum(s => s.PremiumbyPaymentMethod) + current.User.InitialPortfo;
            int CountSales = _Context.LifeBordroAdditions.Where(w => w.SellerNC == current.User.NC).Count() + current.User.InitialStatic; ;
            foreach (var item in childRates.Select(s => s.UserRole).Where(w => w.IsActive == true))
            {
                SumSales += _Context.LifeBordroAdditions.Where(w => w.SellerNC == item.User.NC).Sum(s => s.PremiumbyPaymentMethod);
                CountSales += _Context.LifeBordroAdditions.Where(w => w.SellerNC == item.User.NC).Count();
            }

            return (SumSales, CountSales);
        }
        public IEnumerable<ChildRate> GetAllChilds(int urId, int Level = 0, int Period = 0)
        {
            UserRole current = _Context.UserRoles.Include(p => p.User).Include(p => p.Role).Include(r => r.Childeren)
               .Include(p => p.UserRoleParent).SingleOrDefault(f => f.URId == urId);

            Period += 1;
            List<UserRole> userRoles = current.Childeren.ToList();
            if (userRoles.Any())
            {

                Level = 1;
                foreach (var item in userRoles)
                {

                    yield return new ChildRate { UserRole = item, Rate = Level };

                    if (GetAllChilds(item.URId) != null)
                    {

                        Level += 1;
                        foreach (var subcategory in GetAllChilds(item.URId))
                        {

                            yield return new ChildRate { UserRole = subcategory.UserRole, Rate = Level };
                        }
                    }
                    else
                    {
                        Level = 1;
                    }
                }
            }
            else
            {
                Level = 1;
            }
        }
        public async Task<List<PasargadBordroViewModel>> GetFileNewRecords(IFormFile File)
        {
            string res = MyUtility.ReadUploadedExcel(File);
            List<PasargadBordroViewModel> bordros = JsonConvert.DeserializeObject<List<PasargadBordroViewModel>>(res);
            List<LifeBordroBase> lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToListAsync();
            bordros = bordros.Where(w => _Context.Users.Any(a => a.NC == w.AgentNC)).ToList();
            List<PasargadBordroViewModel> Exist_lifeBordroBases = bordros.Where(w => !lifeBordroBases.Select(s => s.InsNO.Replace(" ", string.Empty)).Any(a => w.InsNO.Replace(" ", string.Empty) == a)).ToList();
            return Exist_lifeBordroBases;
        }
        public bool UpdateLifeBordroCollectionAsync(List<LifeBordroBase> UploadBodro)
        {
            UploadBodro = UploadBodro.Where(w => w.LifeBordroAdditions.Count() == 1 && w.LifeBordroAdditions.Any(a => a.Number == 0)).ToList();
            if (UploadBodro == null)
            {
                return false;
            }
            if (UploadBodro.Count() == 0)
            {
                return false;
            }
            foreach (var item in UploadBodro)
            {
                item.IsActive = false;
                item.LifeBordroAdditions.FirstOrDefault().IsActive = false;
                _Context.LifeBordroBases.Update(item);
            }
            return true;

        }
        public async Task<List<LifeBordroBase>> GetLifeBordroBasesBySellerNC(string NC, int? RecCount, int? page, bool all, string search, int IsDateRange)
        {
            int acCount = RecCount.GetValueOrDefault(30);
            int curPage = page.GetValueOrDefault(1);
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.NC.Trim() == NC.Trim());
            if (user == null)
            {
                return null;
            }
            List<UserRole> userRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role)
                                        .Where(w => w.User.NC.Trim() == NC.Trim()).ToListAsync();
            bool UserIsAdmin = false;
            if (userRoles != null)
            {
                if (userRoles.Count() != 0)
                {
                    if (userRoles.Any(a => a.RoleId == 1))
                    {
                        UserIsAdmin = true;
                    }
                }
            }
            List<LifeBordroBase> lifeBordroBases = null;

            if (UserIsAdmin == true)
            {
                if (all == false)
                {
                    lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToListAsync();
                    if (!string.IsNullOrEmpty(search))
                    {
                        if (IsDateRange == 0)
                        {
                            lifeBordroBases = lifeBordroBases.Where(w => w.InsNO.Contains(search) || w.IssueDate.ToShamsi().Contains(search) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsurerFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsuredFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.PaymentMethod.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.Seller.Contains(search)))
                            .ToList();
                        }
                        if (IsDateRange == 1)
                        {
                            string[] srch = search.Split("-");
                            string sdate = string.Empty;
                            if (srch.Length >= 1)
                            {
                                sdate = srch[0].Trim();
                            }
                            string edate = string.Empty;
                            if (srch.Length >= 2)
                            {
                                edate = srch[1].Trim();
                            }

                            string[] svdate = sdate.Split("/");
                            string[] evdate = edate.Split("/");
                            if ((svdate.Length > 1 && svdate.Length <= 3) && (evdate.Length > 1 && evdate.Length <= 3))
                            {
                                DateTime MSdate = sdate.GetMiladiDateWithoutTime(false);

                                DateTime MEDate = edate.GetMiladiDateWithoutTime(true);
                                lifeBordroBases = lifeBordroBases.Where(w => w.IssueDate >= MSdate && w.IssueDate <= MEDate).ToList();
                            }
                            else
                            {
                                return null;
                            }
                        }


                    }
                    lifeBordroBases = lifeBordroBases.OrderByDescending(r => r.IssueDate).ThenByDescending(r => r.InsNO).ToList();

                    lifeBordroBases = lifeBordroBases.Skip(RecCount.GetValueOrDefault(30) * (page.GetValueOrDefault(1) - 1)).Take(RecCount.GetValueOrDefault(30)).ToList();

                    return lifeBordroBases;
                }
                else
                {
                    lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToListAsync();
                    if (!string.IsNullOrEmpty(search))
                    {
                        if (search.Count(f => f == '-') == 1)
                        {
                            string[] srch = search.Split("-");
                            string sdate = string.Empty;
                            if (srch.Length >= 1)
                            {
                                sdate = srch[0].Trim();
                            }
                            string edate = string.Empty;
                            if (srch.Length >= 2)
                            {
                                edate = srch[1].Trim();
                            }
                            string[] svdate = sdate.Split("/");
                            string[] evdate = edate.Split("/");
                            if ((svdate.Length > 1 && svdate.Length <= 3) && (evdate.Length > 1 && evdate.Length <= 3))
                            {
                                DateTime MSdate = sdate.GetMiladiDateWithoutTime(false);
                                DateTime MEDate = edate.GetMiladiDateWithoutTime(true);
                                lifeBordroBases = lifeBordroBases.Where(w => w.IssueDate >= MSdate && w.IssueDate <= MEDate).ToList();
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            lifeBordroBases = lifeBordroBases.Where(w => w.InsNO.Contains(search) || w.IssueDate.ToShamsi().Contains(search) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsurerFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsuredFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.PaymentMethod.Contains(search)) ||
                             w.LifeBordroAdditions.Any(a => a.IsActive == true && a.Seller.Contains(search)))
                            .ToList();
                        }

                    }
                    lifeBordroBases = lifeBordroBases.OrderByDescending(r => r.IssueDate).ThenByDescending(r => r.InsNO).ToList();
                    return lifeBordroBases;
                }

            }
            else
            {
                if (all == false)
                {
                    lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions)
                    .Where(w => w.LifeBordroAdditions.Any(a => a.SellerNC == NC && a.IsActive == true)).ToListAsync();
                    if (!string.IsNullOrEmpty(search))
                    {
                        if (search.Contains("-"))
                        {
                            string[] srch = search.Split("-");
                            string sdate = string.Empty;
                            if (srch.Length >= 1)
                            {
                                sdate = srch[0].Trim();
                            }
                            string edate = string.Empty;
                            if (srch.Length >= 2)
                            {
                                edate = srch[1].Trim();
                            }
                            string[] svdate = sdate.Split("/");
                            string[] evdate = edate.Split("/");
                            if (svdate.Length - 1 > 0 && evdate.Length - 1 > 0)
                            {
                                DateTime MSdate = sdate.GetMiladiDateWithoutTime(false);
                                DateTime MEDate = edate.GetMiladiDateWithoutTime(true);
                                lifeBordroBases = lifeBordroBases.Where(w => w.IssueDate >= MSdate && w.IssueDate <= MEDate).ToList();
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            lifeBordroBases = lifeBordroBases.Where(w => w.InsNO.Contains(search) || w.IssueDate.ToShamsi().Contains(search) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsurerFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsuredFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.PaymentMethod.Contains(search)) ||
                             w.LifeBordroAdditions.Any(a => a.IsActive == true && a.Seller.Contains(search)))
                            .ToList();
                        }


                    }
                    lifeBordroBases = lifeBordroBases.OrderByDescending(r => r.IssueDate).ThenByDescending(r => r.InsNO).ToList();
                    lifeBordroBases = lifeBordroBases.Skip(acCount * (curPage - 1)).Take(acCount).ToList();
                    return lifeBordroBases;
                }
                else
                {
                    lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions)
                    .Where(w => w.LifeBordroAdditions.Any(a => a.SellerNC == NC && a.IsActive == true)).ToListAsync();
                    if (!string.IsNullOrEmpty(search))
                    {
                        if (IsDateRange == 1)
                        {
                            if (search.Count(f => f == '-') == 1)
                            {
                                string[] srch = search.Split("-");
                                string sdate = string.Empty;
                                if (srch.Length >= 1)
                                {
                                    sdate = srch[0].Trim();
                                }
                                string edate = string.Empty;
                                if (srch.Length >= 2)
                                {
                                    edate = srch[1].Trim();
                                }
                                string[] svdate = sdate.Split("/");
                                string[] evdate = edate.Split("/");
                                if ((svdate.Length > 1 && svdate.Length <= 3) && (evdate.Length > 1 && evdate.Length <= 3))
                                {
                                    DateTime MSdate = sdate.GetMiladiDateWithoutTime(false);
                                    DateTime MEDate = edate.GetMiladiDateWithoutTime(true);
                                    lifeBordroBases = lifeBordroBases.Where(w => w.IssueDate >= MSdate && w.IssueDate <= MEDate).ToList();
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return null;
                            }

                        }
                        else
                        {
                            lifeBordroBases = lifeBordroBases.Where(w => w.InsNO.Contains(search) || w.IssueDate.ToShamsi().Contains(search) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsurerFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsuredFullName.Contains(search)) ||
                             w.LifeBordroAdditions.Any(a => a.IsActive == true && a.Seller.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.PaymentMethod.Contains(search)))
                                .ToList();
                        }

                    }
                    lifeBordroBases = lifeBordroBases.OrderByDescending(r => r.IssueDate).ThenByDescending(r => r.InsNO).ToList();
                    return lifeBordroBases;
                }

            }

        }
        public async Task<LifeBordroBase> GetLifeBordroBaseById(Guid id)
        {
            return await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).FirstOrDefaultAsync(f => f.Id == id);
        }
        public async Task<List<LifeBordroBase>> GetIndirectBordroBasebyurId(int urId)
        {
            UserRole userRole = await _Context.UserRoles.AsNoTracking().Include(r => r.User).Include(r => r.Role).Include(r => r.Childeren)
                .Include(r => r.UserRoleParent).Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role)
                .SingleOrDefaultAsync(s => s.URId == urId);
            if (userRole == null)
            {
                return null;
            }
            List<LifeBordroAddition> additions = await _Context.LifeBordroAdditions.AsNoTracking().Include(r => r.LifeBordroBase).Include(r => r.LifeBordroBase.Commissions)
                .Where(w => w.IsActive && w.SellerNC != userRole.User.NC).ToListAsync();
            additions = additions.Where(w => w.SalesUsers.Any(a => a.Substring(0, a.IndexOf("-")) == urId.ToString())).ToList();
            var Selusers = additions.Select(s => new { InsNN = s.LifeBordroBase.InsNO, susers = s.SalesUsers }).ToList();


            return additions.Select(s => s.LifeBordroBase).ToList();
        }
        public async Task<List<LifeBordroBase>> GetDirectBordroBasebyurIdAsync(int urId)
        {
            UserRole userRole = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.Childeren)
                .Include(r => r.UserRoleParent).Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role)
                .SingleOrDefaultAsync(s => s.URId == urId);
            if (userRole == null)
            {
                return null;
            }
            List<LifeBordroAddition> additions = await _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).Include(r => r.LifeBordroBase.Commissions).Where(w => w.IsActive).ToListAsync();
            // additions = additions.Where(w => w.SalesUsers.Any(a =>a. a.Substring(0, a.IndexOf("-")) == urId.ToString())).ToList();
            additions = additions.Where(w => GetSalesObjectsofBordroAsync(w.LifeBordroBase.Id).Result?.FirstOrDefault(f => f.SRate == 1).UrId == urId).ToList();
            return additions.Select(s => s.LifeBordroBase).ToList();
        }
        public async Task<List<LifeBordroBase>> GetLifeBordroBasesAsync()
        {
            return await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToListAsync();
        }
        public async Task<LifeBordroAddition> GetLifeBordroAdditionByIdAsync(int id)
        {
            return await _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
        public List<LifeBordroBase> GetLifeBordroBases()
        {
            return _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).Include(r => r.Commissions).ToList();
        }
        public async Task<List<LifeBordroBase>> GetDirectBordroBasebyNC(string NC)
        {
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.NC == NC);
            if (user == null)
            {
                return null;
            }
            List<LifeBordroBase> lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions)
                                                    .Where(w => w.LifeBordroAdditions.Any(a => a.SellerNC == NC && a.IsActive == true)).ToListAsync();
            return lifeBordroBases;
        }
        public async Task<List<LifeBordroBase>> GetInDirectBordroBasebyNC(string NC)
        {
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.NC == NC);
            if (user == null)
            {
                return null;
            }
            List<UserRole> userRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role)
                                        .Where(w => w.User.NC.Trim() == NC.Trim()).ToListAsync();
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();
            foreach (var item in userRoles)
            {
                List<LifeBordroAddition> lifeBordroAdditions = await _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase)
                                                    .Where(w => w.SellerNC != NC).ToListAsync();
                lifeBordroAdditions = lifeBordroAdditions.Where(w => w.IsActive == true && w.SalesUsers.Any(a => a.Substring(0, a.IndexOf("-")) == item.URId.ToString())).ToList();
                if (lifeBordroAdditions != null)
                {
                    if (lifeBordroAdditions.Count() != 0)
                    {
                        List<LifeBordroBase> lifeBordroBases1 = lifeBordroAdditions.Select(s => s.LifeBordroBase).ToList();
                        lifeBordroBases.AddRange(lifeBordroBases1);
                    }
                }

            }

            return lifeBordroBases;
        }
        public async Task<List<LifeBordroAddition>> GetDirectLifeBordroAdditionin12MounthAgoAsync(int year, int mounth, string nc)
        {
            DateTime StartDate = GetStartEnd_12MounthBefore(year, mounth).startDate;
            DateTime EndDate = GetStartEnd_12MounthBefore(year, mounth).endDate;

            return await _Context.LifeBordroAdditions.AsNoTracking().Include(r => r.LifeBordroBase)
                .Where(w => w.IsActive == true && w.SellerNC == nc && w.LifeBordroBase.IssueDate >= StartDate && w.LifeBordroBase.IssueDate <= EndDate).ToListAsync();


        }

        public async Task<List<LifeBordroAddition>> GetIndirectLifeBordroAdditionin12MounthAgoAsync(int year, int mounth, string nc)
        {
            DateTime StartDate = GetStartEnd_12MounthBefore(year, mounth).startDate;
            DateTime EndDate = GetStartEnd_12MounthBefore(year, mounth).endDate;
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.NC.Trim() == nc.Trim());
            if (user == null)
            {
                return null;
            }
            List<UserRole> userRoles = await _Context.UserRoles.AsNoTracking().Include(r => r.User).Include(r => r.Role)
                                        .Where(w => w.User.NC.Trim() == nc.Trim()).ToListAsync();
            List<LifeBordroAddition> additions = await _Context.LifeBordroAdditions.AsNoTracking().Include(r => r.LifeBordroBase).Include(r => r.LifeBordroBase.Commissions)
                                            .Where(w => w.SellerNC != nc && w.LifeBordroBase.IssueDate >= StartDate && w.LifeBordroBase.IssueDate <= EndDate).ToListAsync();

            List<LifeBordroAddition> lifeBordroAdditions = new List<LifeBordroAddition>();
            foreach (var item in userRoles)
            {
                List<LifeBordroAddition> result = additions.Where(w => w.SalesUsers.Any(a => a.Substring(0, a.IndexOf("-")) == item.URId.ToString())).ToList();
                if (result != null)
                {
                    lifeBordroAdditions.AddRange(result);
                }
            }
            return lifeBordroAdditions;
        }
        public (DateTime startDate, DateTime endDate) GetStartEnd_12MounthBefore(int Year, int Mounth)
        {
            int EDay = 31;

            int EMounth = Mounth;
            int EYear = Year;
            int SMounth = Mounth + 1;
            if (SMounth > 12)
            {
                SMounth -= 12;
            }
            int SDay = 1;
            int SYear = Year - 1;
            if (Mounth > 6)
            {
                EDay = 30;
            }


            if ((EYear + 1) % 4 != 0 && EMounth == 12)
            {
                EDay = 29;
            }
            string sDate = SYear.ToString() + "/" + SMounth.ToString("0#") + "/" + SDay.ToString("0#");
            string EDate = EYear.ToString() + "/" + EMounth.ToString("0#") + "/" + EDay.ToString("0#");

            DateTime StartDate = sDate.ChangeToMiladi("00:00");
            DateTime EndDate = EDate.ChangeToMiladi("00:00");
            return (StartDate, EndDate);
        }

        public async Task<List<ExcelBordroModel>> GetPersonalExcelBordroModelsByNCAsync(string NC)
        {
            List<LifeBordroBase> lifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions)
                                                    .Where(w => w.LifeBordroAdditions.Any(a => a.SellerNC == NC && a.IsActive == true)).ToListAsync();
            List<ExcelBordroModel> personalBordroModels = new List<ExcelBordroModel>();
            if (lifeBordroBases == null)
            {
                return null;
            }
            personalBordroModels = lifeBordroBases.Select(lbb => new ExcelBordroModel
            {
                InsNO = lbb.InsNO,
                IssueDate = lbb.IssueDate.ToShamsi(),
                InitialStartDate = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InitialStartDate.ToShamsi(),
                AdditionNumber = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Number,
                Insurer = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsurerFullName,
                InsurerNC = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsurerNC,
                Insured = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsuredFullName,
                InsuredNC = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsuredNC,
                Duration = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Duration.ToString(),
                PayMethod = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).PaymentMethod,
                PremiumByPay = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).PremiumbyPaymentMethod,
                LifePremium = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).LFPremium,
                SupPremium = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).SupPremium,
                Deposit = (int)lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Deposit,
                LifeCapital = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).CapitalDied,
                AgentName = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Seller,
                AgentNC = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).SellerNC,
                SalesOrg = GetStringofSalesLevelsAsync(GetSalesObjectsofBordroAsync(lbb.Id).Result),
                Status = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Status,
                Type = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Type

            }).ToList();
            return personalBordroModels;
        }
        public async Task<List<ExcelBordroModel>> GetOrgExcelBordroModelsByNCAsync(string NC)
        {
            List<LifeBordroBase> lifeBordroBases = await GetInDirectBordroBasebyNC(NC);
            List<ExcelBordroModel> orgBordroModels = new List<ExcelBordroModel>();
            if (lifeBordroBases == null)
            {
                return null;
            }
            orgBordroModels = lifeBordroBases.Select(lbb => new ExcelBordroModel
            {
                InsNO = lbb.InsNO,
                IssueDate = lbb.IssueDate.ToShamsi(),
                InitialStartDate = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InitialStartDate.ToShamsi(),
                AdditionNumber = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Number,
                Insurer = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsurerFullName,
                InsurerNC = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsurerNC,
                Insured = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsuredFullName,
                InsuredNC = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsuredNC,
                Duration = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Duration.ToString(),
                PayMethod = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).PaymentMethod,
                PremiumByPay = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).PremiumbyPaymentMethod,
                LifePremium = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).LFPremium,
                SupPremium = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).SupPremium,
                Deposit = (int)lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Deposit,
                LifeCapital = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).CapitalDied,
                AgentName = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Seller,
                AgentNC = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).SellerNC,
                SalesOrg = GetStringofSalesLevelsAsync(GetSalesObjectsofBordroAsync(lbb.Id).Result),

                Status = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Status,
                Type = lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Type

            }).ToList();
            return orgBordroModels;
        }
        public async Task<bool> DeactiveBordrosActiveAddition_BaseonAdditionFileInsNO_Async(List<string> InsNOs)
        {
            List<LifeBordroAddition> additions = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions)
                .Where(w => InsNOs.Any(a => a == w.InsNO)).SelectMany(s => s.LifeBordroAdditions.Where(h => h.IsActive == true)).ToListAsync();
            if (additions == null)
            {
                return false;
            }
            additions.ForEach(f => f.IsActive = false);

            _Context.LifeBordroAdditions.UpdateRange(additions);
            return true;
        }
        public async Task<List<NonePaymentBordroesDet>> GetNonPaidBordroesAsync(string Name)
        {
            List<UserRole> userRoles = await _Context.UserRoles.Include(r => r.User).Where(w => w.User.Code == Name).ToListAsync();
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();
            foreach (var item in userRoles)
            {
                List<LifeBordroBase> IndbordroBases = await GetIndirectBordroBasebyurId(item.URId);
                List<LifeBordroBase> DirbordroBase = await GetDirectBordroBasebyNC(item.User.NC);

                if (DirbordroBase != null)
                {
                    lifeBordroBases.AddRange(DirbordroBase);
                }
                if (IndbordroBases != null)
                {
                    if (IndbordroBases.Count() != 0)
                    {
                        lifeBordroBases.AddRange(IndbordroBases);
                    }

                }


            }
            lifeBordroBases = lifeBordroBases.Where(w => !w.LifeBordroAdditions.Any(a => a.Status.Replace(" ", "") == "ابطال") &&
                !w.LifeBordroAdditions.Any(a => a.Status.Replace(" ", "") == "بازخرید") &&
                !w.LifeBordroAdditions.Any(a => a.Status.Replace(" ", "") == "سررسید") &&
                !w.LifeBordroAdditions.Any(a => a.Status.Replace(" ", "") == "خسارتفوت") &&
                !w.LifeBordroAdditions.Any(a => a.Status.Replace(" ", "") == "خسارتفوتوحادثه")
            ).ToList();
            List<LifeBordroBase> OverduelifeBordros = lifeBordroBases.Where(w => GetNumberofIns_OverdueInstallment(w.InsNO).Result - w.Commissions.Count() > 1).ToList();
            List<NonePaymentBordroesDet> nonePaymentBordroes = OverduelifeBordros.Select(x => new NonePaymentBordroesDet()
            {
                InsNO = x.InsNO,
                IssueDate = x.IssueDate,
                Insurer = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsurerFullName,
                Insured = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsuredFullName,
                Deposit = (int)x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Deposit,
                PaymentMethod = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).PaymentMethod,
                PaymentMethodValue = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).PremiumbyPaymentMethod,
                Seller = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Seller,
                Status = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Status,
                Type = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Type,
                LastReceiveDate = x.Commissions.OrderByDescending(r => r.PaidDate).FirstOrDefault().PaidDate,
                NonReceivedCount = GetNumberofIns_OverdueInstallment(x.InsNO).Result - x.Commissions.Count(),
                TotalPremiumReceived = x.Commissions.Sum(m => m.LifePremium + m.SupPermium),
                InsuredPhone = _Context.InsuredInformation.FirstOrDefault(f => f.InsNO == x.InsNO)?.Cellphone
            }
            ).ToList();
            return nonePaymentBordroes;
        }

        #endregion bordro
        #region commission
        /// <summary>
        /// تعداد کارمزدهای پرداخت شده بیمه نامه
        /// </summary>
        /// <param name="InsNo"></param>
        /// <returns></returns>
        public async Task<int> GetNumberofInsCommissionPaid(string InsNo)
        {
            LifeBordroBase lifeBordroBase = await _Context.LifeBordroBases.Include(r => r.Commissions).Include(r => r.LifeBordroAdditions)
                                            .SingleOrDefaultAsync(s => s.InsNO == InsNo.Trim());
            if (lifeBordroBase == null)
            {
                return -1;
            }
            int paidCom = 0;
            paidCom = lifeBordroBase.Commissions.Count();
            return paidCom;
        }
        /// <summary>
        ///تعداد اقساط سررسید شده بیمه نامه
        /// </summary>
        /// <param name="InsNo"></param>
        /// <returns></returns>
        public async Task<int> GetNumberofIns_OverdueInstallment(string InsNo)
        {
            LifeBordroBase lifeBordroBase = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).Include(r => r.Commissions)
                .Where(w => w.LifeBordroAdditions.Any(a => a.Status.Contains("فعال") || a.Status.Contains("انصراف از بازخرید") || a.Status.Contains("مخفف")))
                                            .SingleOrDefaultAsync(s => s.InsNO == InsNo.Trim());

            if (lifeBordroBase == null)
            {
                return -1;
            }
            PersianCalendar PC = new PersianCalendar();
            int issueYear = PC.GetYear(lifeBordroBase.IssueDate);
            int issueMounth = PC.GetMonth(lifeBordroBase.IssueDate);
            CommissionBase LastCommissionBase = await _Context.CommissionBases.OrderByDescending(r => r.Year).ThenByDescending(r => r.Mounth).FirstOrDefaultAsync();

            int DifYear = LastCommissionBase.Year - issueYear;
            int DifMounth = LastCommissionBase.Mounth - issueMounth;

            int Mounth = DifYear * 12 + DifMounth;
            List<Commission> commissions = lifeBordroBase.Commissions.ToList();


            int OverCount = 0;
            string pm = lifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).PaymentMethod.Replace(" ", "");
            if (pm.Contains("ماهانه"))
            {

                OverCount = Mounth + 1;
            }
            if (pm.Contains("سهماهه"))
            {
                float div3 = Mounth / 3;
                OverCount = (int)div3 + 1;
            }
            if (pm.Contains("ششماهه"))
            {
                float div6 = Mounth / 6;
                OverCount = (int)div6 + 1;
            }
            if (pm.Contains("سالانه"))
            {
                float div12 = Mounth / 12;
                OverCount = (int)div12 + 1;
            }

            return OverCount;
        }

        public async Task<bool> ExistCommissionBaseByYearMounthAsync(int Year, int Mounth)
        {
            return await _Context.CommissionBases.AnyAsync(f => f.Mounth == Mounth && f.Year == Year);
        }
        public async Task<bool> ExistAllCommissionInsNO_in_Bordro(List<PasargadCommissionVM> commissions)
        {
            commissions = commissions.Where(w => !string.IsNullOrEmpty(w.InsNO)).ToList();
            List<string> BordroInsNOs = await _Context.LifeBordroBases.Select(s => s.InsNO.GetBranch_and_InsNO()).ToListAsync();
            List<string> ComInsNOs = commissions.Select(s => s.InsNO.GetBranch_and_InsNO()).ToList();
            List<string> IntSec = ComInsNOs.Intersect(BordroInsNOs).ToList();
            if (IntSec.Count() == 0) return false;
            List<string> Exc = IntSec.Except(ComInsNOs).ToList();
            bool result = Exc.Count() == 0;
            return result;

        }
        public bool CreateCommissionCollection(int Mounth, int Year, string LoginName, List<PasargadCommissionVM> Pasargadcommissions, PasargadCommissionVM pasargadCommissionBase)
        {
            if (Pasargadcommissions == null)
            {
                return false;
            }
            if (Pasargadcommissions.Count() == 0)
            {
                return false;
            }
            //Commission Base record => and if be null all commissions dont save
            PasargadCommissionVM pasargadCommissionVMBase = Pasargadcommissions.FirstOrDefault(f => string.IsNullOrEmpty(f.InsNO));
            if (pasargadCommissionBase == null)
            {
                return false;
            }
            List<Commission> Newcommissions = new List<Commission>();
            foreach (var item in Pasargadcommissions.Where(w => !string.IsNullOrEmpty(w.InsNO)))
            {
                LifeBordroBase lifeBordroBase = _Context.LifeBordroBases.FirstOrDefault(f => f.InsNO == item.InsNO);
                if (lifeBordroBase != null)
                {
                    Commission commission = new Commission()
                    {

                        LifeBordroBase = lifeBordroBase,
                        DueDate = item.DueDate.ChangeToMiladi("00:00"),
                        PaidDate = item.PaidDate.ChangeToMiladi("00:00"),
                        Percent = float.Parse(item.Percent),
                        LifePremium = long.Parse(item.LifePremium),
                        SupPermium = long.Parse(item.SupPremium),
                        LifeCommission = long.Parse(item.LifeCommission),
                        SupCommission = long.Parse(item.SupCommission),
                    };
                    Newcommissions.Add(commission);
                }

            }
            CommissionBase commissionBase = new CommissionBase()
            {
                CreateDate = DateTime.Now,
                Year = Year,
                Mounth = Mounth,
                Tax = long.Parse(pasargadCommissionBase.Tax),
                ManicipalTax = long.Parse(pasargadCommissionBase.ManicipalTax),
                Deductions = long.Parse(pasargadCommissionBase.Deductions),
                DeductionDesc = pasargadCommissionBase.DeductionDesc,
                Vat = long.Parse(pasargadCommissionBase.Vat),
                TotalVat = long.Parse(pasargadCommissionBase.TotalVat),
                NetCommission = long.Parse(pasargadCommissionBase.NetCommission),
                CommissionSum = long.Parse(pasargadCommissionBase.SumCommission),
                Commissions = Newcommissions

            };

            _Context.CommissionBases.Add(commissionBase);
            _Context.SaveChanges();
            return true;
        }
        public async Task<CompareCommissionFileWithDbVM> GetDiffrenceNewCommissonWithDbAsync(List<PasargadCommissionVM> pasargadCommissionVMs)
        {
            if (!pasargadCommissionVMs.Any(a => string.IsNullOrEmpty(a.InsNO) && !string.IsNullOrEmpty(a.NetCommission)))
            {
                CompareCommissionFileWithDbVM compareCommissionFileWithDbVM1 = new CompareCommissionFileWithDbVM
                {
                    ActiveSubmit = false,
                    Message = "<h5 class='text-justify alert alert-warning upl m-t-10'>" + "فایل حاوی اطلاعات پایه کامزد نمی باشد !" + "</h5>"
                };
                return compareCommissionFileWithDbVM1;
            }
            int TotalFileRecordCount = pasargadCommissionVMs.Where(w => !string.IsNullOrEmpty(w.InsNO)).Count();
            List<CommissionBase> commissionBases = await _Context.CommissionBases.Include(r => r.Commissions).ToListAsync();
            List<string> FileInsNOs = pasargadCommissionVMs.Select(s => s.InsNO.GetBranch_and_InsNO()).ToList();

            List<string> DbOrginalInsNO = await _Context.LifeBordroBases.Select(s => s.InsNO).ToListAsync();

            List<string> DbInsNOs = await _Context.LifeBordroBases.Select(s => s.InsNO.GetBranch_and_InsNO()).ToListAsync();
            List<PasargadCommissionVM> CommonFileWithDB = pasargadCommissionVMs.Where(w => DbInsNOs.Any(a => a == w.InsNO.GetBranch_and_InsNO())).ToList();
            List<string> commonInsNOs = DbInsNOs.Intersect(FileInsNOs).ToList();
            List<PasargadCommissionVM> ExceptFileWithDb = pasargadCommissionVMs.Except(CommonFileWithDB).ToList();

            List<string> ExcInsNOs = FileInsNOs.Except(commonInsNOs).ToList();

            //جهت اصلاح شماره بیمه نامه
            CommonFileWithDB.Select(c => { c.InsNO = DbOrginalInsNO.FirstOrDefault(f => f.GetBranch_and_InsNO() == c.InsNO.GetBranch_and_InsNO()); return c; }).ToList();
            //جهت اصلاح شماره بیمه نامه

            CompareCommissionFileWithDbVM compareCommissionFileWithDbVM = new CompareCommissionFileWithDbVM()
            {
                CommonData = CommonFileWithDB,
                ExludeFileandDb = ExceptFileWithDb
            };
            if (CommonFileWithDB != null)
            {
                if (CommonFileWithDB.Count() != 0)
                {
                    compareCommissionFileWithDbVM.ActiveSubmit = true;
                    compareCommissionFileWithDbVM.Message = "<h5 class='text-justify alert alert-success upl m-t-10'>" + "تعداد" + " " + CommonFileWithDB.Count() + " " + "رکورد از " + TotalFileRecordCount + " " + "رکورد برای ذخیره سازی معتبر است" + "</h5>";
                }
                else
                {
                    compareCommissionFileWithDbVM.ActiveSubmit = false;
                    compareCommissionFileWithDbVM.Message = "<h5 class='text-justify alert alert-warning upl m-t-10'>" + "در این فایل رکوردی منطبق با سیستم وجود ندارد" + "</h5>";
                }
            }
            else
            {
                compareCommissionFileWithDbVM.ActiveSubmit = false;
                compareCommissionFileWithDbVM.Message = "<h5 class='text-justify alert alert-warning upl m-t-10'>" + "در این فایل رکوردی منطبق با سیستم وجود ندارد" + "</h5>";
            }
            return compareCommissionFileWithDbVM;
        }
        public async Task<string> GetStandardInsNOfromDb(string Brach_InsNO)
        {
            if (string.IsNullOrEmpty(Brach_InsNO))
            {
                return null;
            }
            LifeBordroBase lifeBordroBase = await _Context.LifeBordroBases.FirstOrDefaultAsync(f => f.InsNO.GetBranch_and_InsNO() == Brach_InsNO);
            if (lifeBordroBase == null)
            {
                return null;
            }
            return lifeBordroBase.InsNO;
        }
        public async Task<bool> RemoveCommissonBaseAsync(int Year, int Mounth)
        {
            CommissionBase commissionBase = await _Context.CommissionBases.FirstOrDefaultAsync(f => f.Year == Year && f.Mounth == Mounth);
            if (commissionBase == null)
            {
                return false;
            }
            _Context.CommissionBases.Remove(commissionBase);
            return true;
        }
        public async Task<CommissionBase> GetCommissionBaseByYearMounthAsync(int Year, int Mounth)
        {
            return await _Context.CommissionBases.Include(r => r.Commissions).SingleOrDefaultAsync(s => s.Year == Year && s.Mounth == Mounth);
        }
        public async Task<List<Commission>> GetCommissionsBySellerNC_Year_Mounth(string SellerNC, int Year, int Mounth)
        {
            CommissionBase commissionBase = await _Context.CommissionBases.Include(r => r.Commissions)
                .SingleOrDefaultAsync(s => s.Year == Year && s.Mounth == Mounth);
            if (commissionBase == null)
            {
                return null;
            }

            List<Commission> commissions = await _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).Include(r => r.LifeBordroBase.LifeBordroAdditions)
                                           .Where(w => w.CBId == commissionBase.Id && w.LifeBordroBase.LifeBordroAdditions.Any(a => a.IsActive == true && a.SellerNC == SellerNC)).ToListAsync();
            return commissions;
        }
        public async Task<Commission> GetCommissionByIdAsync(int id)
        {
            return await _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase)
                            .FirstOrDefaultAsync(f => f.Id == id);
        }
        public async Task<List<Commission>> GetCommissionsByYear_and_Mounth(int Year, int Mounth)
        {
            return await _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase)
                        .Where(w => w.CommissionBase.Year == Year && w.CommissionBase.Mounth == Mounth).ToListAsync();
        }
        public async Task<List<OrgUserComVM>> GetOrgCommissionsAsync(List<int> SelectedSubUserUrIds, string LoginIdentityName, int Year = 0, int Mounth = 0)
        {

            List<UserRole> userRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent).Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role)
                .Where(w => w.User.Code == LoginIdentityName.Trim()).ToListAsync();
            List<LifeBordroBase> IndlifeBordroBases = new List<LifeBordroBase>();


            //List<LifeBordroAddition> lifeBordroAdditions = _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).Include(r => r.LifeBordroBase.Commissions)
            //        .ToList();
            //lifeBordroAdditions = lifeBordroAdditions.Where(w => w.SalesUsers.Any(a => a.Substring(0, a.IndexOf("-")) == "3")).ToList();


            foreach (var item in userRoles)
            {
                List<LifeBordroBase> bordroBases = await GetIndirectBordroBasebyurId(item.URId);
                if (bordroBases != null)
                {
                    if (bordroBases.Count() != 0)
                    {
                        IndlifeBordroBases.AddRange(bordroBases);
                    }
                }
            }
            List<OrgUserComVM> orgUserComVMs = new List<OrgUserComVM>();
            foreach (var item in SelectedSubUserUrIds)
            {
                OrgUserComVM orgUserComVM = new OrgUserComVM();
                UserRole SubUserRole = await _Context.UserRoles.Include(r => r.User).SingleOrDefaultAsync(s => s.URId == item);
                orgUserComVM.UserRole = SubUserRole;

                List<LifeBordroAddition> PersonalAdditions = await _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).ToListAsync();
                PersonalAdditions = PersonalAdditions.Where(w => w.SalesUsers != null && w.SalesUsers.LastOrDefault().Split("-")[0] == item.ToString() && w.SalesUsers.LastOrDefault().Split("-")[1] == "1").ToList();
                List<LifeBordroBase> PersoanlBordroBases = PersonalAdditions.Select(s => s.LifeBordroBase).ToList();
                List<LifeBordroBase> PersoanllifeBordroBases = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToListAsync();
                if (PersoanlBordroBases != null)
                {
                    if (!userRoles.Select(s => s.URId).Any(a => a == item))
                    {
                        PersoanlBordroBases = PersoanlBordroBases.Intersect(IndlifeBordroBases).ToList();
                    }

                }

                List<LifeBordroBase> IndirectBordoBasees = await GetIndirectBordroBasebyurId(item);
                if (IndirectBordoBasees != null)
                {
                    IndirectBordoBasees = IndirectBordoBasees.Intersect(IndlifeBordroBases).ToList();
                }
                orgUserComVM.UserRole = SubUserRole;

                //List<LifeBordroAddition> Padditions = _Context.LifeBordroAdditions.Include(r => r.LifeBordroBase).Include(r => r.LifeBordroBase.Commissions)
                //   .ToList();
                //Padditions = Padditions.Where(w => w.SalesUsers.Any(a => a.Substring(0, a.IndexOf("-")) == item.ToString())).ToList();
                //var inter = Padditions.Select(s => s.LifeBordroBase).Intersect(PersoanlBordroBases).ToList();
                List<Commission> UserCommission = await _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase)
                                .Where(w => w.CommissionBase.Year == Year && w.CommissionBase.Mounth == Mounth).ToListAsync();
                orgUserComVM.PersoanlCommissions = UserCommission.Where(w => PersoanlBordroBases.Any(a => a.InsNO == w.LifeBordroBase.InsNO)).ToList();
                orgUserComVM.OrgCommissions = UserCommission.Where(w => IndirectBordoBasees.Any(a => a.InsNO == w.LifeBordroBase.InsNO)).ToList();
                orgUserComVMs.Add(orgUserComVM);

            }
            return orgUserComVMs;
        }
        public async Task<List<SystemCommissionVM>> GetUserSystemCommissionAsync(string Name, int year, int mounth)
        {
            List<SystemCommissionVM> systemCommissionVMs = new List<SystemCommissionVM>();

            User user = await _Context.Users.FirstOrDefaultAsync(f => f.Code == Name);
            List<UserRole> LoginUserRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Where(w => w.User == user).ToListAsync();
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);

            PoolRewardReportResultVM poolRewardReportResultVM = await GetUserPoolRewardAsync(null, year, mounth);
            CommissionBase commissionBase = await GetCommissionBaseByYearMounthAsync(year, mounth);
            List<PoolRewardReport_TotalVM> poolRewardReport_TotalVMs = null;
            if (commissionBase != null)
            {
                poolRewardReportResultVM.CommissionBase = commissionBase;
                poolRewardReport_TotalVMs = GetTotalPoolRewardReport(poolRewardReportResultVM);
            }
            foreach (var item in LoginUserRoles)
            {
                SystemCommissionVM systemCommissionVM = new SystemCommissionVM();
                systemCommissionVM.User = item.User;
                if (commissionBase != null)
                {
                    long poolrewardT = 0;
                    if (poolRewardReport_TotalVMs.Any(a => a.User == item.User))
                    {
                        poolrewardT = poolRewardReport_TotalVMs.Where(w => w.User == item.User).Sum(s => s.ShareValue);
                    }
                    systemCommissionVM.PoolRewardTotal = poolrewardT;
                    systemCommissionVM.Title = Active_userRole.Role.RoleTitle;
                    List<LifeBordroBase> IndbordroBases = await GetIndirectBordroBasebyurId(item.URId);
                    List<LifeBordroBase> DirbordroBases = await GetDirectBordroBasebyurIdAsync(item.URId);
                    long PersonalComSum = 0;
                    long OrgComSum = 0; long EqRewardSum = 0;
                    //List<Commission> Dcoms = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase)
                    //                        .Where(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth && DirbordroBases.Any(a => a.Id == w.LifeBordroBase.Id)).ToList();
                    foreach (var dirB in DirbordroBases.ToList())
                    {
                        List<Commission> Dcommissions = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).Where(f => f.CommissionBase.Year == year && f.CommissionBase.Mounth == mounth && f.LifeBordroBase == dirB).ToList();
                        if (Dcommissions != null)
                        {
                            foreach (var Dcom in Dcommissions)
                            {
                                LifeBordroAddition ActiveAddition = dirB.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);

                                float NewPer = 0;
                                float userBper = 0;
                                long LifeCom = 0;
                                long SupCom = 0;
                                int loop = 1;
                                foreach (var up in ActiveAddition.SalesUsers.LastOrDefault().Split("-").ToList())
                                {
                                    if (loop == 3)
                                    {
                                        userBper = float.Parse(up.ToString());
                                    }
                                    loop++;
                                }
                                NewPer = (userBper / Dcom.Percent);

                                LifeCom = (long)(NewPer * (Dcom.LifeCommission * 100 / Dcom.Percent));
                                SupCom = (long)(NewPer * (Dcom.SupCommission * 100 / Dcom.Percent));
                                PersonalComSum += (LifeCom + SupCom);
                            }

                        }

                    }
                    systemCommissionVM.PersonalCommissionsTotal = PersonalComSum;
                    systemCommissionVM.Comment += item.Role.RoleTitle + " | " + " شخصی: " + PersonalComSum;
                    foreach (var indB in IndbordroBases?.ToList())
                    {
                        if (indB.Commissions.Select(x => x.CommissionBase) != null)
                        {
                            //List<Commission> commissions = await _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).ToListAsync();
                            //Commission commissionX = commissions.FirstOrDefault(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth);
                            Commission commission = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).ToList().FirstOrDefault(f => f.CommissionBase.Year == year && f.CommissionBase.Mounth == mounth && f.LifeBordroBase == indB);

                            if (commission != null)
                            {
                                List<Commission> indBComs = indB.Commissions.Where(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth).ToList();
                                if (indBComs != null)
                                {
                                    foreach (var inCom in indBComs?.ToList())
                                    {
                                        LifeBordroAddition ActiveAddition = indB.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);

                                        float NewPer = 0;
                                        long LifeCom = 0;
                                        long SupCom = 0;
                                        float Orgper = 0;

                                        var su = ActiveAddition.SalesUsers.FirstOrDefault(a => a.Substring(0, a.IndexOf("-")) == item.URId.ToString()).ToString();
                                        List<SalesObject> Sos = await GetSalesObjectsofBordroAsync(inCom.LifeBordroBase.Id);
                                        float orgPercent = (float)Sos?.FirstOrDefault(w => w.UrId == item.URId).SPercent;
                                        float eqPercent = (float)Sos?.FirstOrDefault(w => w.UrId == item.URId).SEqPercent;

                                        if (eqPercent == 0)
                                        {
                                            Orgper = orgPercent;
                                        }
                                        else
                                        {
                                            Orgper = eqPercent;
                                        }
                                        float TotalOrgPer = 0;
                                        UserRole SelleruserRole = Sos.FirstOrDefault(f => f.SRate == 1).UserRole;

                                        if (SelleruserRole != null)
                                        {
                                            TotalOrgPer = Orgper * SelleruserRole.Role.RoleRate;
                                        }
                                        if (eqPercent == 0)
                                        {
                                            NewPer = (TotalOrgPer / inCom.Percent);

                                            LifeCom = (long)(NewPer * (inCom.LifeCommission * 100 / inCom.Percent));
                                            SupCom = (long)(NewPer * (inCom.SupCommission * 100 / inCom.Percent));
                                            OrgComSum += (LifeCom + SupCom);
                                        }
                                        else
                                        {
                                            NewPer = (eqPercent / inCom.Percent);

                                            LifeCom = (long)(NewPer * (inCom.LifeCommission * 100 / inCom.Percent));
                                            SupCom = (long)(NewPer * (inCom.SupCommission * 100 / inCom.Percent));
                                            EqRewardSum += (LifeCom + SupCom);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    systemCommissionVM.OrgCommissionsTotal = OrgComSum;
                    systemCommissionVM.EqulityRewardTotal = EqRewardSum;
                    if (string.IsNullOrEmpty(systemCommissionVM.Comment))
                    {
                        systemCommissionVM.Comment += item.Role.RoleTitle + "|" + "شخصی : " + PersonalComSum + "سازمانی : " + OrgComSum;
                    }
                    else
                    {
                        systemCommissionVM.Comment += "\n" + item.Role.RoleTitle + "|" + "شخصی : " + PersonalComSum + "سازمانی : " + OrgComSum;
                    }

                    systemCommissionVMs.Add(systemCommissionVM);
                }

            }
            return systemCommissionVMs;
        }
        public async Task<List<SystemCommissionVM>> ZGetUserSystemCommissionAsync(string Name, int year, int mounth)
        {
            List<SystemCommissionVM> systemCommissionVMs = new List<SystemCommissionVM>();

            User user = await _Context.Users.FirstOrDefaultAsync(f => f.Code == Name);
            List<UserRole> LoginUserRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Where(w => w.User == user).ToListAsync();
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);

            PoolRewardReportResultVM poolRewardReportResultVM = await GetUserPoolRewardAsync(null, year, mounth);
            CommissionBase commissionBase = await GetCommissionBaseByYearMounthAsync(year, mounth);
            List<PoolRewardReport_TotalVM> poolRewardReport_TotalVMs = null;
            if (commissionBase != null)
            {
                poolRewardReportResultVM.CommissionBase = commissionBase;
                poolRewardReport_TotalVMs = GetTotalPoolRewardReport(poolRewardReportResultVM);
            }
            foreach (var item in LoginUserRoles)
            {
                SystemCommissionVM systemCommissionVM = new SystemCommissionVM();
                systemCommissionVM.User = item.User;
                if (commissionBase != null)
                {
                    long poolrewardT = 0;
                    if (poolRewardReport_TotalVMs.Any(a => a.User == item.User))
                    {
                        poolrewardT = poolRewardReport_TotalVMs.Where(w => w.User == item.User).Sum(s => s.ShareValue);
                    }
                    systemCommissionVM.PoolRewardTotal = poolrewardT;
                    systemCommissionVM.Title = Active_userRole.Role.RoleTitle;
                    List<LifeBordroBase> IndbordroBases = await GetIndirectBordroBasebyurId(item.URId);
                    List<LifeBordroBase> DirbordroBases = await GetDirectBordroBasebyurIdAsync(item.URId);
                    long PersonalComSum = 0;
                    long OrgComSum = 0; long EqRewardSum = 0;
                    List<Commission> Dcoms = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase)
                                            .Where(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth && DirbordroBases.Any(a => a.Id == w.LifeBordroBase.Id)).ToList();
                    foreach (var dirB in DirbordroBases.ToList())
                    {
                        List<Commission> Dcommissions = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).Where(f => f.CommissionBase.Year == year && f.CommissionBase.Mounth == mounth && f.LifeBordroBase == dirB).ToList();
                        if (Dcommissions != null)
                        {
                            foreach (var Dcom in Dcommissions)
                            {
                                LifeBordroAddition ActiveAddition = dirB.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);

                                float NewPer = 0;
                                float userBper = 0;
                                long LifeCom = 0;
                                long SupCom = 0;
                                int loop = 1;
                                foreach (var up in ActiveAddition.SalesUsers.LastOrDefault().Split("-").ToList())
                                {
                                    if (loop == 3)
                                    {
                                        userBper = float.Parse(up.ToString());
                                    }
                                    loop++;
                                }
                                NewPer = (userBper / Dcom.Percent);

                                LifeCom = (long)(NewPer * (Dcom.LifeCommission * 100 / Dcom.Percent));
                                SupCom = (long)(NewPer * (Dcom.SupCommission * 100 / Dcom.Percent));
                                PersonalComSum += (LifeCom + SupCom);
                            }

                        }

                    }
                    systemCommissionVM.PersonalCommissionsTotal = PersonalComSum;
                    systemCommissionVM.Comment += item.Role.RoleTitle + " | " + " شخصی: " + PersonalComSum;
                    foreach (var indB in IndbordroBases?.ToList())
                    {
                        if (indB.Commissions.Select(x => x.CommissionBase) != null)
                        {
                            //List<Commission> commissions = await _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).ToListAsync();
                            //Commission commissionX = commissions.FirstOrDefault(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth);
                            Commission commission = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).ToList().FirstOrDefault(f => f.CommissionBase.Year == year && f.CommissionBase.Mounth == mounth && f.LifeBordroBase == indB);

                            if (commission != null)
                            {
                                List<Commission> indBComs = indB.Commissions.Where(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth).ToList();
                                if (indBComs != null)
                                {
                                    foreach (var inCom in indBComs?.ToList())
                                    {
                                        LifeBordroAddition ActiveAddition = indB.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);

                                        float NewPer = 0;
                                        long LifeCom = 0;
                                        long SupCom = 0;
                                        float Orgper = 0;

                                        var su = ActiveAddition.SalesUsers.FirstOrDefault(a => a.Substring(0, a.IndexOf("-")) == item.URId.ToString()).ToString();
                                        List<SalesObject> Sos = await GetSalesObjectsofBordroAsync(inCom.LifeBordroBase.Id);
                                        float orgPercent = (float)Sos?.FirstOrDefault(w => w.UrId == item.URId).SPercent;
                                        float eqPercent = (float)Sos?.FirstOrDefault(w => w.UrId == item.URId).SEqPercent;

                                        if (eqPercent == 0)
                                        {
                                            Orgper = orgPercent;
                                        }
                                        else
                                        {
                                            Orgper = eqPercent;
                                        }
                                        float TotalOrgPer = 0;
                                        UserRole SelleruserRole = Sos.FirstOrDefault(f => f.SRate == 1).UserRole;

                                        if (SelleruserRole != null)
                                        {
                                            TotalOrgPer = Orgper * SelleruserRole.Role.RoleRate;
                                        }
                                        if (eqPercent == 0)
                                        {
                                            NewPer = (TotalOrgPer / inCom.Percent);

                                            LifeCom = (long)(NewPer * (inCom.LifeCommission * 100 / inCom.Percent));
                                            SupCom = (long)(NewPer * (inCom.SupCommission * 100 / inCom.Percent));
                                            OrgComSum += (LifeCom + SupCom);
                                        }
                                        else
                                        {
                                            NewPer = (eqPercent / inCom.Percent);

                                            LifeCom = (long)(NewPer * (inCom.LifeCommission * 100 / inCom.Percent));
                                            SupCom = (long)(NewPer * (inCom.SupCommission * 100 / inCom.Percent));
                                            EqRewardSum += (LifeCom + SupCom);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    systemCommissionVM.OrgCommissionsTotal = OrgComSum;
                    systemCommissionVM.EqulityRewardTotal = EqRewardSum;
                    if (string.IsNullOrEmpty(systemCommissionVM.Comment))
                    {
                        systemCommissionVM.Comment += item.Role.RoleTitle + "|" + "شخصی : " + PersonalComSum + "سازمانی : " + OrgComSum;
                    }
                    else
                    {
                        systemCommissionVM.Comment += "\n" + item.Role.RoleTitle + "|" + "شخصی : " + PersonalComSum + "سازمانی : " + OrgComSum;
                    }

                    systemCommissionVMs.Add(systemCommissionVM);
                }

            }
            return systemCommissionVMs;
        }
        public async Task<List<SystemCommissionVM>> GetSystemCommissonsAsync(List<int> SelectedSubUserUrIds, int year, int mounth)
        {


            List<SystemCommissionVM> systemCommissionVMs = new List<SystemCommissionVM>();
            PoolRewardReportResultVM poolRewardReportResultVM = await GetUserPoolRewardAsync(null, year, mounth);
            CommissionBase commissionBase = await GetCommissionBaseByYearMounthAsync(year, mounth);

            List<PoolRewardReport_TotalVM> poolRewardReport_TotalVMs = null;
            if (commissionBase != null)
            {
                poolRewardReportResultVM.CommissionBase = commissionBase;
                poolRewardReport_TotalVMs = GetTotalPoolRewardReport(poolRewardReportResultVM);

            }

            foreach (var item in SelectedSubUserUrIds)
            {
                SystemCommissionVM systemCommissionVM = new SystemCommissionVM();
                UserRole SelectedUserRole = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
                    .FirstOrDefaultAsync(f => f.URId == item);
                int userId = SelectedUserRole.User_ID;


                UserRole ActiveUserRole = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).FirstOrDefaultAsync(f => f.User_ID == userId && f.IsActive == true);
                if (SelectedUserRole != null)
                {
                    systemCommissionVM.User = SelectedUserRole.User;
                    if (commissionBase != null)
                    {
                        long poolrewardT = 0;
                        if (poolRewardReport_TotalVMs.Any(a => a.User.Id == userId))
                        {
                            poolrewardT = poolRewardReport_TotalVMs.Where(w => w.User.Id == userId).Sum(s => s.ShareValue);
                        }
                        systemCommissionVM.PoolRewardTotal = poolrewardT;
                    }
                }
                if (ActiveUserRole != null)
                {
                    systemCommissionVM.Title = ActiveUserRole.Role.RoleTitle;
                }
                List<LifeBordroBase> IndbordroBases = await GetIndirectBordroBasebyurId(item);
                List<LifeBordroBase> DirbordroBases = await GetDirectBordroBasebyurIdAsync(SelectedUserRole.URId);

                long PersonalComSum = 0;
                long OrgComSum = 0; long EqRewardSum = 0;
                foreach (var dirB in DirbordroBases.ToList())
                {
                    //Commission commission = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).FirstOrDefault(f => f.CommissionBase.Year == year && f.CommissionBase.Mounth == mounth && f.LifeBordroBase == dirB);
                    List<Commission> Dcommissions = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).Where(f => f.CommissionBase.Year == year && f.CommissionBase.Mounth == mounth && f.LifeBordroBase == dirB).ToList();
                    if (Dcommissions != null)
                    {
                        foreach (var Dcom in Dcommissions)
                        {
                            LifeBordroAddition ActiveAddition = dirB.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);

                            float NewPer = 0;
                            float userBper = 0;
                            long LifeCom = 0;
                            long SupCom = 0;
                            int loop = 1;
                            foreach (var up in ActiveAddition.SalesUsers.LastOrDefault().Split("-").ToList())
                            {
                                if (loop == 3)
                                {
                                    userBper = float.Parse(up.ToString());
                                }
                                loop++;
                            }
                            NewPer = (userBper / Dcom.Percent);

                            LifeCom = (long)(NewPer * (Dcom.LifeCommission * 100 / Dcom.Percent));
                            SupCom = (long)(NewPer * (Dcom.SupCommission * 100 / Dcom.Percent));
                            PersonalComSum += (LifeCom + SupCom);
                        }

                    }

                }
                systemCommissionVM.PersonalCommissionsTotal = PersonalComSum;
                if (IndbordroBases != null)
                {
                    if (IndbordroBases.Count() != 0)
                    {
                        foreach (var indB in IndbordroBases?.ToList())
                        {
                            Commission commission = _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase).FirstOrDefault(f => f.CommissionBase.Year == year && f.CommissionBase.Mounth == mounth && f.LifeBordroBase == indB);

                            if (commission != null)
                            {
                                //List<Commission> indBComs = indB.Commissions.ToList().Where(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth).ToList();
                                List<Commission> indBComs = await _Context.Commissions.Include(r => r.CommissionBase).Include(r => r.LifeBordroBase)
                                                .Where(w => w.CommissionBase.Year == year && w.CommissionBase.Mounth == mounth && w.LifeBordroBase == indB).ToListAsync();
                                if (indBComs != null)
                                {
                                    foreach (var inCom in indBComs?.ToList())
                                    {
                                        LifeBordroAddition ActiveAddition = indB.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);

                                        float NewPer = 0;
                                        long LifeCom = 0;
                                        long SupCom = 0;
                                        float Orgper = 0;

                                        var su = ActiveAddition.SalesUsers.FirstOrDefault(a => a.Substring(0, a.IndexOf("-")) == SelectedUserRole.URId.ToString()).ToString();
                                        List<SalesObject> Sos = await GetSalesObjectsofBordroAsync(inCom.LifeBordroBase.Id);
                                        float orgPercent = (float)Sos?.FirstOrDefault(w => w.UrId == SelectedUserRole.URId).SPercent;
                                        float eqPercent = (float)Sos?.FirstOrDefault(w => w.UrId == SelectedUserRole.URId).SEqPercent;

                                        if (eqPercent == 0)
                                        {
                                            Orgper = orgPercent;
                                        }
                                        else
                                        {
                                            Orgper = eqPercent;
                                        }
                                        float TotalOrgPer = 0;
                                        UserRole SelleruserRole = Sos.FirstOrDefault(f => f.SRate == 1).UserRole;

                                        if (SelleruserRole != null)
                                        {
                                            TotalOrgPer = Orgper * SelleruserRole.Role.RoleRate;
                                        }
                                        if (eqPercent == 0)
                                        {
                                            NewPer = (TotalOrgPer / inCom.Percent);

                                            LifeCom = (long)(NewPer * (inCom.LifeCommission * 100 / inCom.Percent));
                                            SupCom = (long)(NewPer * (inCom.SupCommission * 100 / inCom.Percent));
                                            OrgComSum += (LifeCom + SupCom);
                                        }
                                        else
                                        {
                                            NewPer = (eqPercent / inCom.Percent);

                                            LifeCom = (long)(NewPer * (inCom.LifeCommission * 100 / inCom.Percent));
                                            SupCom = (long)(NewPer * (inCom.SupCommission * 100 / inCom.Percent));
                                            EqRewardSum += (LifeCom + SupCom);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                systemCommissionVM.OrgCommissionsTotal = OrgComSum;
                systemCommissionVM.EqulityRewardTotal = EqRewardSum;
                if (string.IsNullOrEmpty(systemCommissionVM.Comment))
                {
                    systemCommissionVM.Comment += SelectedUserRole.Role.RoleTitle + "|" + "شخصی : " + PersonalComSum + " سازمانی : " + OrgComSum;
                }
                else
                {
                    systemCommissionVM.Comment += "\n" + SelectedUserRole.Role.RoleTitle + "|" + "شخصی : " + PersonalComSum + " سازمانی : " + OrgComSum;
                }
                systemCommissionVMs.Add(systemCommissionVM);
            }
            return systemCommissionVMs;
        }
        public async Task<List<OrgCommissionReportModel>> ConvertOrgUserComVM_To_OrgCommissionReportModelAsync(List<OrgUserComVM> orgUserComVMs, int Year, int Mounth)
        {
            //use for Prepare model for export excel org commission file
            List<OrgCommissionReportModel> orgCommissionReportModels = new List<OrgCommissionReportModel>();

            foreach (var item in orgUserComVMs)
            {

                foreach (var DirCom in item.PersoanlCommissions)
                {
                    List<SalesObject> salesObjects = await GetSalesObjectsofBordroAsync(DirCom.LifeBordroBase.Id);
                    float NewPer = 0;
                    float per = 0;
                    if (salesObjects.Any(a => a.SRate == 1))
                    {
                        per = salesObjects.FirstOrDefault(f => f.SRate == 1).SPercent;
                    }
                    NewPer = per / DirCom.Percent;
                    int LCom = (int)(NewPer * (DirCom.LifeCommission * 100 / DirCom.Percent));
                    int SupCom = (int)(NewPer * (DirCom.SupCommission * 100 / DirCom.Percent));
                    orgCommissionReportModels.Add(new OrgCommissionReportModel
                    {
                        InsNO = DirCom.LifeBordroBase.InsNO,
                        Insurer = DirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsurerFullName,
                        Insured = DirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsuredFullName,
                        DueDate = DirCom.LifeBordroBase.IssueDate.ToShamsi(),
                        PaidDate = DirCom.PaidDate.ToShamsi(),
                        LifePermium = (int)DirCom.LifePremium,
                        SupPremium = (int)DirCom.SupPermium,
                        LifeCommission = LCom,
                        SupCommission = SupCom,
                        SumCommision = LCom + SupCom,
                        Percent = per,
                        EqPercent = 0,
                        Seller = DirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Seller,
                        SellerNC = DirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).SellerNC,
                        OwnerUser = item.UserRole.FullPro,
                        CommissionType = "مستقیم",
                        Year = Year.ToString(),
                        Mounth = Mounth.ToString()
                    });
                }
                foreach (var InDirCom in item.OrgCommissions)
                {
                    LifeBordroBase OrglifeBordroBase = await GetLifeBordroBaseById(InDirCom.LifeBordroBase.Id);
                    LifeBordroAddition OrgActiveAddition = OrglifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);
                    List<SalesObject> salesObjects = await GetSalesObjectsofBordroAsync(InDirCom.LifeBordroBase.Id);
                    SalesObject currentUserSaleObject = salesObjects.FirstOrDefault(f => f.UrId == item.UserRole.URId);

                    float Orgper = 0;
                    float Eqper = 0;
                    float TotalOrgPer = 0;
                    if (salesObjects.Any(a => a.SRate == 3))
                    {
                        Orgper = currentUserSaleObject.SPercent;
                    }
                    if (salesObjects.Any(a => a.SRate == 4))
                    {
                        Eqper = currentUserSaleObject.SEqPercent;
                    }
                    if (currentUserSaleObject != null)
                    {
                        TotalOrgPer = currentUserSaleObject.UserRole.Role.RoleRate * (Orgper + Eqper);
                    }
                    double OrgLifeCommission = ((TotalOrgPer / InDirCom.Percent) * (InDirCom.LifeCommission * 100 / InDirCom.Percent));
                    double OrgSupCommission = ((TotalOrgPer / InDirCom.Percent) * (InDirCom.SupCommission * 100 / InDirCom.Percent));
                    double SumComm = OrgLifeCommission + OrgSupCommission;
                    orgCommissionReportModels.Add(new OrgCommissionReportModel
                    {
                        InsNO = InDirCom.LifeBordroBase.InsNO,
                        Insurer = InDirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsurerFullName,
                        Insured = InDirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsuredFullName,
                        DueDate = InDirCom.LifeBordroBase.IssueDate.ToShamsi(),
                        PaidDate = InDirCom.PaidDate.ToShamsi(),
                        LifePermium = (int)InDirCom.LifePremium,
                        SupPremium = (int)InDirCom.SupPermium,
                        LifeCommission = (int)OrgLifeCommission,
                        SupCommission = (int)OrgSupCommission,
                        SumCommision = (int)SumComm,
                        Percent = Orgper,
                        EqPercent = Eqper,
                        Seller = InDirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).Seller,
                        SellerNC = InDirCom.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).SellerNC,
                        OwnerUser = item.UserRole.FullPro,
                        CommissionType = "سازمانی",
                        Year = Year.ToString(),
                        Mounth = Mounth.ToString()
                    });

                }
            }
            return orgCommissionReportModels;
        }
        public async Task<List<PersonalCommissionReportModel>> CovertCommissions_To_PersonalCommissionReportModelAsync(List<Commission> PersoanlCommisssions, int Year, int Mounth)
        {
            List<PersonalCommissionReportModel> personalCommissionReportModels = new List<PersonalCommissionReportModel>();
            foreach (var item in PersoanlCommisssions)
            {
                LifeBordroBase lifeBordroBase = await GetLifeBordroBaseById(item.LifeBordroBase.Id);
                LifeBordroAddition ActiveAddition = lifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true);
                List<SalesObject> salesObjects = await GetSalesObjectsofBordroAsync(item.LifeBordroBase.Id);
                SalesObject currentUserSaleObject = salesObjects.FirstOrDefault(f => f.UserRole.User.NC == ActiveAddition.SellerNC);
                int LCom = (int)(currentUserSaleObject.SPercent / item.Percent * (item.LifeCommission * 100 / item.Percent));
                int SupCom = (int)(currentUserSaleObject.SPercent / item.Percent * (item.SupCommission * 100 / item.Percent));
                personalCommissionReportModels.Add(new PersonalCommissionReportModel
                {
                    InsNO = item.LifeBordroBase.InsNO,
                    Insurer = item.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsurerFullName,
                    Insured = item.LifeBordroBase.LifeBordroAdditions.FirstOrDefault(f => f.IsActive == true).InsuredFullName,
                    DueDate = item.DueDate.ToShamsi(),
                    PaidDate = item.PaidDate.ToShamsi(),
                    LifePermium = (int)item.LifePremium,
                    SupPremium = (int)item.SupPermium,
                    LifeCommission = LCom,
                    SupCommission = SupCom,
                    SumCommisison = LCom + SupCom,
                    Percent = currentUserSaleObject.SPercent,
                    Seller = ActiveAddition.Seller,
                    SellerNC = ActiveAddition.SellerNC,
                    Year = Year.ToString(),
                    Mounth = Mounth.ToString()
                });
            }
            return personalCommissionReportModels;
        }
        public async Task<CommissionBase> GetLastCommissionAsync()
        {
            return await _Context.CommissionBases.Include(r => r.Commissions)
                .OrderByDescending(r => r.Year).ThenByDescending(r => r.Mounth).FirstOrDefaultAsync();
        }

        #endregion commission
        #region SalesPackage


        public async Task<List<SalesObject>> GetSalesObjectsofBordroAsync(Guid BordroGId)
        {
            LifeBordroBase lifeBordroBase = await _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions)
                .SingleOrDefaultAsync(s => s.Id == BordroGId);
            LifeBordroAddition lifeBordroAdditionActive = lifeBordroBase.LifeBordroAdditions.ToList().FirstOrDefault(f => f.IsActive == true);
            List<SalesObject> salesObjects = new List<SalesObject>();
            foreach (var item in lifeBordroAdditionActive.SalesUsers.ToList())
            {
                string[] pk = item.Split("-");
                if (pk != null && pk.Length != 0)
                {
                    SalesObject salesObject = new SalesObject();

                    int urId = 0;
                    
                    urId = int.Parse(Regex.Match(pk[0], @"\d+").Value);


                    UserRole userRole = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent).Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role).SingleOrDefaultAsync(s => s.URId == urId);
                    if (userRole != null)
                    {
                        salesObject.UrId = urId;
                        salesObject.UserRole = userRole;
                        if (pk.Length >= 1)
                        {
                            if(!string.IsNullOrEmpty(pk[1]))
                            {
                                salesObject.SRate = int.Parse(Regex.Match(pk[1], @"\d+").Value);
                            }
                            else
                            {
                                salesObject.SRate = 0;
                            } 
                        }
                        if (pk.Length >= 2)
                        {
                            if(!string.IsNullOrEmpty(pk[2]))
                            {
                                salesObject.SPercent = float.Parse(pk[2]);
                            }
                            else
                            {
                                salesObject.SPercent = 0;
                            }
                            
                        }
                        if (pk.Length >= 3)
                        {
                            if(!string.IsNullOrEmpty(pk[3]))
                            {
                                salesObject.SEqPercent = float.Parse(pk[3]);
                            }
                            else
                            {
                                salesObject.SEqPercent = 0;
                            }
                            
                        }

                        salesObjects.Add(salesObject);
                    }


                }

            }
            return salesObjects;
        }
        public async Task<bool> UpdateLifeBordroCollectionByNewFileAsync(List<PasargadBordroViewModel> bordros, int Year, int Mounth, string LoginName)
        {
            List<LifeBordroBase> lifeBordroBases = await _Context.LifeBordroBases.Where(w => w.Year == Year && w.Mounth == Mounth).Include(r => r.LifeBordroAdditions).ToListAsync();
            if (lifeBordroBases != null)
            {
                if (lifeBordroBases.Count() != 0)
                {
                    _Context.LifeBordroBases.RemoveRange(lifeBordroBases);
                    _Context.SaveChanges();
                }
            }

            List<LifeBordroAddition> NewlifeBordroAdditions = new List<LifeBordroAddition>();
            bool Issave = await CreateLifeBordroCollectionAsync(Year, Mounth, LoginName, bordros);
            return Issave;

        }
        public async Task<List<UploadInfo>> GetUploadInfosAsync()
        {
            return await _Context.UploadInfos.ToListAsync();
        }

        public string GetStringofSalesLevelsAsync(List<SalesObject> salesObjects)
        {
            string res = string.Empty;
            //"\2190" is css content code => https://www.toptal.com/designers/htmlarrows/
            //"\u200F" is right-to-left mark
            foreach (var item in salesObjects.OrderByDescending(r => r.SRate))
            {
                if (item != salesObjects.LastOrDefault())
                {
                    res += "\u200F" + item.SRate.ToString() + " - " + item.UserRole.FullPro + " \u2190 " + item.UserRole.User.Code + " |";
                }
                else
                {
                    res += "\u200F" + item.SRate.ToString() + " - " + item.UserRole.FullPro + " \u2190 " + item.UserRole.User.Code;
                }
            }
            return res;
        }

        #endregion

        #region RoolPool
        public async Task<PoolRewardReportResultVM> GetUserPoolRewardAsync(int? userId, int? year, int? mounth)
        {
            RolePool DRolePool = await _Context.RolePools.OrderBy(r => r.Value).FirstOrDefaultAsync(w => w.ByDirectSale == true && w.ByIndirectSale == false);
            RolePool IndRolePool = await _Context.RolePools.OrderBy(r => r.Value).FirstOrDefaultAsync(w => w.ByDirectSale == true && w.ByIndirectSale == true);
            long DirPoolValue = 1500000000; long IndPoolValue = 1500000000000;
            if (DRolePool != null)
            {
                DirPoolValue = DRolePool.Value;
            }
            if (IndRolePool != null)
            {
                IndPoolValue = IndRolePool.Value;
            }
            if (DRolePool != null || IndRolePool != null)
            {
                List<User> Allusers = await _Context.Users.Include(r => r.UserRoles)
                    .Where(w => w.IsActive == true).ToListAsync();
                RolePool DirrolePool = null; RolePool inDirrolePool = null;
                PoolRewardReportResultVM poolRewardReportResultVM = new PoolRewardReportResultVM();
                CommissionBase commissionBase = await GetCommissionBaseByYearMounthAsync((int)year, (int)mounth);
                if (commissionBase != null)
                {
                    List<User> Selectedusers = null;
                    if (userId == null)
                    {
                        Selectedusers = Allusers.Where(w => w.Code != "290070").ToList();

                    }
                    else
                    {
                        User user = await _Context.Users.Include(r => r.UserRoles).Include(r => r.UserPoolRewards).SingleOrDefaultAsync(s => s.Id == userId);
                        List<LifeBordroAddition> directBordro = await GetDirectLifeBordroAdditionin12MounthAgoAsync((int)year, (int)mounth, user.NC.Trim());
                        long dirsum = (long)directBordro.Sum(x => x.PremiumbyPaymentMethod + (.1 * x.Deposit));
                        List<LifeBordroAddition> inDirectBordro = await GetIndirectLifeBordroAdditionin12MounthAgoAsync((int)year, (int)mounth, user.NC.Trim());
                        long indSum = (long)inDirectBordro.Sum(x => x.PremiumbyPaymentMethod + (.1 * x.Deposit));
                        long netCom = 0;
                        if (commissionBase != null)
                        {
                            netCom = commissionBase.NetCommission;
                        }
                        DirrolePool = await _Context.RolePools.OrderByDescending(r => r.Value).Where(w => w.ByDirectSale == true && w.ByIndirectSale == false).FirstOrDefaultAsync(f => dirsum >= f.Value);
                        inDirrolePool = await _Context.RolePools.OrderByDescending(r => r.Value).Where(w => w.ByDirectSale == true && w.ByIndirectSale == true).FirstOrDefaultAsync(f => indSum >= f.Value);
                        poolRewardReportResultVM.Loguser = user;
                        poolRewardReportResultVM.LogUserDirPool = DirrolePool;
                        poolRewardReportResultVM.LogUserInDirPool = inDirrolePool;
                        poolRewardReportResultVM.LoguserDirSales = dirsum;
                        poolRewardReportResultVM.LogUserIndirSales = indSum;

                        Selectedusers = Allusers.Where(w => w.Code != "290070" && w.Id != user.Id).ToList();
                    }


                    List<RolePool_UserInfo> rolePool_UserInfos = Selectedusers.Select(s => new RolePool_UserInfo
                    {
                        User = s,
                        DirectSalesValue = (long)GetDirectLifeBordroAdditionin12MounthAgoAsync((int)year, (int)mounth, s.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit),
                        InDirectSalesValue = (long)GetIndirectLifeBordroAdditionin12MounthAgoAsync((int)year, (int)mounth, s.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit),

                    }).ToList();


                    foreach (var pUser in rolePool_UserInfos.Where(w => w.DirectSalesValue >= DirPoolValue || w.InDirectSalesValue + w.DirectSalesValue >= IndPoolValue).ToList())
                    {
                        DirrolePool = await _Context.RolePools.OrderByDescending(r => r.Value).Where(w => w.ByDirectSale == true && w.ByIndirectSale == false).FirstOrDefaultAsync(f => pUser.DirectSalesValue >= f.Value);
                        if (DirrolePool != null)
                        {
                            poolRewardReportResultVM.PoolRewardUserInfoVMs.Add(new PoolRewardUserInfoVM
                            {
                                User = pUser.User,
                                UserDirSales = pUser.DirectSalesValue,
                                UserIndirSales = pUser.InDirectSalesValue,
                                SelectedByDirPool = true,
                                SelectedByIndirPool = false,
                                RolePool = DirrolePool
                            }
                            );
                        }
                        inDirrolePool = await _Context.RolePools.OrderByDescending(r => r.Value).Where(w => w.ByDirectSale == true && w.ByIndirectSale == true).FirstOrDefaultAsync(f => pUser.DirectSalesValue + pUser.InDirectSalesValue >= f.Value);
                        if (inDirrolePool != null)
                        {
                            poolRewardReportResultVM.PoolRewardUserInfoVMs.Add(new PoolRewardUserInfoVM
                            {
                                User = pUser.User,
                                UserDirSales = pUser.DirectSalesValue,
                                UserIndirSales = pUser.InDirectSalesValue,
                                SelectedByDirPool = false,
                                SelectedByIndirPool = true,
                                RolePool = inDirrolePool
                            }
                            );
                        }
                    }
                }


                return poolRewardReportResultVM;
            }
            return null;
        }
        public List<PoolRewardReport_TotalVM> GetTotalPoolRewardReport(PoolRewardReportResultVM poolRewardReportResultVM)
        {
            List<PoolRewardReport_TotalVM> poolRewardReport_TotalVMs = new List<PoolRewardReport_TotalVM>();
            PoolRewardUserInfoVM poolRewardUserInfoVM = new PoolRewardUserInfoVM()
            {
                User = poolRewardReportResultVM.Loguser,
                UserDirSales = poolRewardReportResultVM.LoguserDirSales,
                UserIndirSales = poolRewardReportResultVM.LogUserIndirSales
            };
            foreach (var item in poolRewardReportResultVM.PoolRewardUserInfoVMs)
            {
                PoolRewardReport_TotalVM poolRewardReport_TotalVM = new PoolRewardReport_TotalVM
                {
                    User = item.User,
                    SelectedByDirPool = item.SelectedByDirPool,
                    SelectedByIndirPool = item.SelectedByIndirPool,
                    RolePool = item.RolePool,
                    UserDirSales = item.UserDirSales,
                    UserIndirSales = item.UserIndirSales
                };

                long TotalSales = 0;
                decimal Decshare = 0;
                int IntShare = 0;
                long netCommission = 0;
                if (poolRewardReportResultVM.CommissionBase != null)
                {
                    netCommission = poolRewardReportResultVM.CommissionBase.NetCommission;
                }
                if (item.RolePool.ByDirectSale == true && item.RolePool.ByIndirectSale == false)
                {
                    TotalSales = poolRewardReportResultVM.PoolRewardUserInfoVMs.Where(w => w.RolePool == item.RolePool).Sum(s => s.UserDirSales);
                    Decshare = decimal.Divide(item.UserDirSales, TotalSales);


                }
                if (item.RolePool.ByDirectSale == true && item.RolePool.ByIndirectSale == true)
                {
                    TotalSales = poolRewardReportResultVM.PoolRewardUserInfoVMs.Where(w => w.RolePool == item.RolePool).Sum(s => s.UserDirSales + s.UserIndirSales);
                    Decshare = decimal.Divide(item.UserDirSales + item.UserIndirSales, TotalSales);

                }
                IntShare = (int)(Decshare * 100);
                poolRewardReport_TotalVM.SharePercent = IntShare;

                long svalue = (long)((IntShare * item.RolePool.Percent * netCommission) / 3000);
                poolRewardReport_TotalVM.ShareValue = svalue;
                poolRewardReport_TotalVMs.Add(poolRewardReport_TotalVM);
            }
            return poolRewardReport_TotalVMs.ToList();
        }
        public async Task<List<RolePool_Users>> GetRolePoolUsersAsync(int year, int mounth)
        {
            CommissionBase commissionBase = await GetCommissionBaseByYearMounthAsync(year, mounth);
            if (commissionBase == null)
            {
                return null;
            }
            List<User> Allusers = await _Context.Users.Where(w => w.IsActive == true).ToListAsync();
            Allusers = Allusers.Where(w => w.Code != "290070").ToList();
            List<RolePool_Users> list = new List<RolePool_Users>();
            List<RolePool> rolePools = await _Context.RolePools.ToListAsync();
            if (rolePools != null)
            {
                if (commissionBase != null)
                {
                    foreach (var pool in rolePools)
                    {
                        if ((bool)pool.ByDirectSale && (bool)pool.ByIndirectSale == false)
                        {
                            List<User> Pusers = Allusers.Where(w => GetDirectLifeBordroAdditionin12MounthAgoAsync(year, mounth, w.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit) >= pool.Value).ToList();
                            if (Pusers != null)
                            {
                                List<RolePool_UserInfo> rolePool_UserInfos = Pusers.Select(s => new RolePool_UserInfo
                                {
                                    User = s,
                                    DirectSalesValue = (long)GetDirectLifeBordroAdditionin12MounthAgoAsync(year, mounth, s.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit),
                                    InDirectSalesValue = (long)GetIndirectLifeBordroAdditionin12MounthAgoAsync(year, mounth, s.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit),

                                }).ToList();
                                list.Add(new RolePool_Users { RolePool = pool, rolePool_UserInfos = rolePool_UserInfos.ToList() });
                            }

                        }
                        if ((bool)pool.ByDirectSale && (bool)pool.ByIndirectSale)
                        {
                            List<User> Pusers = Allusers.Where(w => GetIndirectLifeBordroAdditionin12MounthAgoAsync(year, mounth, w.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit) + GetDirectLifeBordroAdditionin12MounthAgoAsync(year, mounth, w.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit) >= pool.Value).ToList();
                            if (Pusers != null)
                            {
                                List<RolePool_UserInfo> rolePool_UserInfos = Pusers.Select(s => new RolePool_UserInfo
                                {
                                    User = s,
                                    DirectSalesValue = (long)GetDirectLifeBordroAdditionin12MounthAgoAsync(year, mounth, s.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit),
                                    InDirectSalesValue = (long)GetIndirectLifeBordroAdditionin12MounthAgoAsync(year, mounth, s.NC).Result.Sum(x => x.PremiumbyPaymentMethod + .1 * x.Deposit),

                                }).ToList();
                                list.Add(new RolePool_Users { RolePool = pool, rolePool_UserInfos = rolePool_UserInfos.ToList() });
                            }

                        }

                    }
                    return list;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }


        }
        public async Task<List<RolePool_Users>> GetRolePool_UsersInfoAsync(int year, int mounth)
        {
            List<RolePool_Users> rolePool_Users = await GetRolePoolUsersAsync(year, mounth);
            if (rolePool_Users != null)
            {
                CommissionBase commissionBase = await _Context.CommissionBases.FirstOrDefaultAsync(f => f.Year == year && f.Mounth == mounth);
                if (commissionBase != null)
                {
                    List<RolePool> rolePools = await _Context.RolePools.ToListAsync();

                    RolePool BigDirRolePool = rolePools.Where(w => w.ByDirectSale == true && w.ByIndirectSale == false).OrderByDescending(r => r.Value).FirstOrDefault();
                    RolePool BigInDirRolePool = rolePools.Where(w => w.ByDirectSale == true && w.ByIndirectSale == true).OrderByDescending(r => r.Value).FirstOrDefault();

                    List<RolePool_Users> DrolePool_Users = rolePool_Users.Where(w => w.RolePool.ByDirectSale == true && w.RolePool.ByIndirectSale == false).ToList();
                    RolePool_Users BigDRPUsers = DrolePool_Users.FirstOrDefault(f => f.RolePool == BigDirRolePool);

                    List<RolePool_Users> TrolePool_Users = rolePool_Users.Where(w => w.RolePool.ByDirectSale == true && w.RolePool.ByIndirectSale == true).ToList();
                    RolePool_Users BigTRPUsers = TrolePool_Users.FirstOrDefault(f => f.RolePool == BigInDirRolePool);

                    foreach (var item in DrolePool_Users)
                    {

                        if (item.RolePool != BigDirRolePool)
                        {
                            List<RolePool_UserInfo> pool_UserInfos = item.rolePool_UserInfos.Intersect(BigDRPUsers.rolePool_UserInfos).ToList();

                            item.rolePool_UserInfos = pool_UserInfos;

                        }
                    }
                    foreach (var item in TrolePool_Users)
                    {

                        if (item.RolePool != BigInDirRolePool)
                        {
                            List<RolePool_UserInfo> pool_UserInfos = item.rolePool_UserInfos.Intersect(BigDRPUsers.rolePool_UserInfos).ToList();

                            item.rolePool_UserInfos = pool_UserInfos;

                        }
                    }

                    List<RolePool_Users> rolePool_UserInfosT = new List<RolePool_Users>();
                    rolePool_UserInfosT.AddRange(DrolePool_Users);
                    rolePool_UserInfosT.AddRange(TrolePool_Users);
                    foreach (var itemA in rolePool_UserInfosT)
                    {
                        foreach (var itemB in itemA.rolePool_UserInfos)
                        {
                            long TotalSales = 0;
                            decimal Decshare = 0;
                            int IntShare = 0;
                            long netCommission = 0;
                            if (commissionBase != null)
                            {
                                netCommission = commissionBase.NetCommission;
                            }
                            if (itemA.RolePool.ByDirectSale == true && itemA.RolePool.ByIndirectSale == false)
                            {
                                //poolRewardReportResultVM.PoolRewardUserInfoVMs.Where(w => w.RolePool == item.RolePool).Sum(s => s.UserDirSales);
                                TotalSales = DrolePool_Users.SelectMany(s => s.rolePool_UserInfos).Sum(s => s.DirectSalesValue);
                                Decshare = decimal.Divide(itemB.DirectSalesValue, TotalSales);
                                IntShare = (int)(Decshare * 100);
                                long svalue = (long)((IntShare * itemA.RolePool.Percent * netCommission) / 3000);
                                itemB.Percent = IntShare;
                                itemB.Value = svalue;

                            }
                            if (itemA.RolePool.ByDirectSale == true && itemA.RolePool.ByIndirectSale == true)
                            {

                                TotalSales = TrolePool_Users.SelectMany(s => s.rolePool_UserInfos).Sum(s => s.DirectSalesValue + s.InDirectSalesValue);
                                Decshare = decimal.Divide(itemB.DirectSalesValue + itemB.InDirectSalesValue, TotalSales);
                                IntShare = (int)(Decshare * 100);
                                long svalue = (long)((IntShare * itemA.RolePool.Percent * netCommission) / 3000);
                                itemB.Percent = IntShare;
                                itemB.Value = svalue;

                            }
                        }
                    }
                    return rolePool_UserInfosT;

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }


        }

        public bool RemoveLifeBordroAdditionCollection(List<LifeBordroAddition> lifeBordroAdditions)
        {
            if (lifeBordroAdditions != null)
            {
                if (lifeBordroAdditions.Count() != 0)
                {
                    _Context.LifeBordroAdditions.RemoveRange(lifeBordroAdditions);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
        #region InsuredInfo
        public void CreateInsuredInfo(InsuredInformation insuredInformation)
        {
            _Context.InsuredInformation.Add(insuredInformation);
        }

        public void CreateInsuredInfoColection(List<InsuredInformation> insuredInformationList)
        {
            _Context.InsuredInformation.AddRange(insuredInformationList);
        }

        public void RemoveInsuredInfoCollection(List<InsuredInformation> insuredInformationList)
        {
            _Context.InsuredInformation.RemoveRange(insuredInformationList);
        }

        public async Task<List<InsuredInfoReportModel>> PrepareInseredInfoReportModelAsync(string NC)
        {
            List<InsuredInformation> insuredInformationList = await _Context.InsuredInformation.ToListAsync();
            List<LifeBordroBase> lifeBordroBases = _Context.LifeBordroBases.Include(r => r.LifeBordroAdditions).ToList()
                                                    .Where(w => GetSalesObjectsofBordroAsync(w.Id).Result.Any(a => a.UserRole.User.NC == NC)).ToList();


            var result = (from insuredsInfo in insuredInformationList
                          join lbb in lifeBordroBases
                          on insuredsInfo.InsNO equals lbb.InsNO
                          where insuredsInfo.InsNO == lbb.InsNO
                          select new { insuredsInfo, lbb }).ToList();

            List<InsuredInfoReportModel> insuredInfoReportModels = result.Select(result => new InsuredInfoReportModel
            {
                InsNO = result.lbb.InsNO,
                InsurerNC = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsurerNC,
                InsurerFullName = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsurerFullName,
                InsuredNC = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsuredNC,
                InsuredFullName = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsuredFullName,
                InsuredBirthDate = result.insuredsInfo.InsuredBirthDate.ToShamsi(),
                IssueDate = result.lbb.IssueDate.ToShamsi(),
                Duration = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Duration.ToString(),
                Seller = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Seller,
                PaymentMethodValue = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).PremiumbyPaymentMethod,
                PaymentMethod = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).PaymentMethod,
                Status = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Status,
                State = result.insuredsInfo.State,
                City = result.insuredsInfo.City,
                Address = result.insuredsInfo.Address,
                AdditionType = result.lbb.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Type,
                Cellphone = result.insuredsInfo.Cellphone,
                Phone = result.insuredsInfo.Phone,
                LifeBordroBase = result.lbb
            }).ToList();



            return insuredInfoReportModels;
        }



        #endregion InsuredInfo
    }


}

