using Core.DTOs.Performance;
using Core.Services.Interfaces;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Utility
{
    public static class MyUtility
    {
        
        public static bool IsValidNC(this string NC)
        {

            char[] chArray = NC.ToCharArray();
            int[] numArray = new int[chArray.Length];
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];
            string[] strArray = { "0000000000", "1111111111", "22222222222", "33333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (strArray.Contains(NC))
            {
                return false;
            }
            else
            {
                int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
                int num4 = num3 - ((num3 / 11) * 11);
                if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }


        }
        public static string GetLetterOfText(this string Text, int count)
        {
            int txtL = Text.Length;
            if (count > txtL)
            {
                return Text;
            }
            return Text.Substring(0, count);
        }
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        private static readonly HashSet<char> DefaultNonWordCharacters
          = new HashSet<char> { ',', '.', ':', ';' };

        /// <summary>
        /// Returns a substring from the start of <paramref name="value"/> no 
        /// longer than <paramref name="length"/>.
        /// Returning only whole words is favored over returning a string that 
        /// is exactly <paramref name="length"/> long. 
        /// </summary>
        /// <param name="value">The original string from which the substring 
        /// will be returned.</param>
        /// <param name="length">The maximum length of the substring.</param>
        /// <param name="nonWordCharacters">Characters that, while not whitespace, 
        /// are not considered part of words and therefor can be removed from a 
        /// word in the end of the returned value. 
        /// Defaults to ",", ".", ":" and ";" if null.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="length"/> is negative
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is null
        /// </exception>
        public static string CropWholeWords(
          this string value,
          int length,
          HashSet<char> nonWordCharacters = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (length < 0)
            {
                throw new ArgumentException("Negative values not allowed.", "length");
            }

            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            if (length >= value.Length)
            {
                return value;
            }
            int end = length;

            for (int i = end; i > 0; i--)
            {
                if (value[i].IsWhitespace())
                {
                    break;
                }

                if (nonWordCharacters.Contains(value[i])
                    && (value.Length == i + 1 || value[i + 1] == ' '))
                {
                    //Removing a character that isn't whitespace but not part 
                    //of the word either (ie ".") given that the character is 
                    //followed by whitespace or the end of the string makes it
                    //possible to include the word, so we do that.
                    break;
                }
                end--;
            }

            if (end == 0)
            {
                //If the first word is longer than the length we favor 
                //returning it as cropped over returning nothing at all.
                end = length;
            }

            return value.Substring(0, end);
        }

        private static bool IsWhitespace(this char character)
        {
            return character == ' ' || character == 'n' || character == 't';
        }

        public static string ReadExcel(string root)
        {
            DataTable dtTable = new DataTable();
            List<string> rowList = new List<string>();
            ISheet sheet;
            using (var stream = new FileStream(root, FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                for (int j = 0; j < cellCount; j++)
                {
                    ICell cell = headerRow.GetCell(j);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    {
                        dtTable.Columns.Add(cell.ToString());
                    }
                }
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == NPOI.SS.UserModel.CellType.Blank)) continue;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) & !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                rowList.Add(row.GetCell(j).ToString().Replace("ي", "ی"));
                            }
                            else
                            {
                                rowList.Add(string.Empty);
                            }
                        }
                        else
                        {
                            rowList.Add(string.Empty);
                        }
                    }
                    if (rowList.Count > 0)
                        dtTable.Rows.Add(rowList.ToArray());
                    rowList.Clear();
                }
                xssWorkbook.Close();
                stream.Close();
            }
            return JsonConvert.SerializeObject(dtTable);

        }
        public static string ReadUploadedExcel(IFormFile file)
        {
            var inputstream = file.OpenReadStream();
            XSSFWorkbook workbook = new XSSFWorkbook(inputstream);
            DataTable dtTable = new DataTable();
            List<string> rowList = new List<string>();

            ISheet sheet = workbook.GetSheetAt(0);
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                {
                    dtTable.Columns.Add(cell.ToString());
                }
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == NPOI.SS.UserModel.CellType.Blank)) continue;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        string v = row.GetCell(j).ToString();
                        if (!string.IsNullOrEmpty(row.GetCell(j).ToString()))
                        {
                            rowList.Add(row.GetCell(j).ToString().Replace("ي", "ی"));
                        }
                        else
                        {
                            rowList.Add(row.GetCell(j).ToString());
                        }
                    }
                }
                if (rowList.Count > 0)
                    dtTable.Rows.Add(rowList.ToArray());
                rowList.Clear();
            }

            return JsonConvert.SerializeObject(dtTable);
        }
        public static (bool confirm, string[] exp) ConfirmExcelFile(string type, IFormFile Exfile)
        {
            if (string.IsNullOrEmpty(type))
            {
                return (false, null);
            }
            var inputstream = Exfile.OpenReadStream();
            XSSFWorkbook workbook = new XSSFWorkbook(inputstream);
            List<string> headers = new List<string>();

            if (type.ToLower() == "bordro")
            {
                string[] bheaders = { "Type", "Status", "AgentNC", "AgentCode", "LifeCapital", "Deposit", "PremiumByPay", "SupPremium", "LifePremium", "PayMethod", "Duration", "Insured", "InsuredNC", "StartDate", "InitialStartDate", "Insurer", "InsurerNC", "IssueDate", "InsNO" };
                headers.AddRange(bheaders);
            }
            if (type.ToLower() == "commission")
            {
                string[] bheaders = { "DeductionDesc", "NetCommission", "TotalVat", "ManicipalTax", "Vat", "Deductions", "Tax", "SupCommission", "LifeCommission", "SupPremium", "LifePremium", "Percent", "PaidDate", "DueDate", "InsNO" };
                headers.AddRange(bheaders);
            }
            if (type.ToLower() == "addition")
            {
                string[] bheaders = { "Type", "Status", "AgentNC", "AgentCode", "LifeCapital", "Deposit", "PremiumByPay", "SupPremium", "LifePremium", "PayMethod", "Duration", "Insured", "InsuredNC", "StartDate", "InitialStartDate", "Insurer", "InsurerNC", "IssueDate", "InsNO" };
                headers.AddRange(bheaders);
            }
            if (type.ToLower() == "insuredinfo")
            {
                string[] bheaders = { "Phone", "Cellphone", "Address", "City", "State", "PaymentMethod", "Duration", "IssueDate", "InsuredFullName", "InsuredBirthDate", "AdditionType", "Status", "InsNO" };
                headers.AddRange(bheaders);
            }

            ISheet sheet = workbook.GetSheetAt(0);
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            List<string> FileHeaders = new List<string>();
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                string header = string.Empty;
                if (cell != null && !string.IsNullOrEmpty(cell.StringCellValue))
                {
                    header = cell.StringCellValue;
                    FileHeaders.Add(header);
                }



            }
            workbook.Close();
            inputstream.Close();
            List<string> comp = headers.Except(FileHeaders).ToList();
            string cm = comp.ToString();
            if (comp != null)
            {
                if (comp.Count != 0)
                {
                    return (false, comp.ToArray());
                }

            }
            comp = comp.Intersect(headers).ToList();
            bool hasMatch = headers.All(a => FileHeaders.Contains(a));
            return (hasMatch, headers.ToArray());
        }
        public static string GetStandardInsNO(this string InsNO, string century)
        {
            InsNO = InsNO.Replace("-", "/");
            InsNO = InsNO.Replace(" ", "");
            string[] insno = InsNO.Split("/");
            if (insno.Length > 6)
            {
                insno = insno.Take(insno.Length - 1).ToArray();
            }

            //insno[4] = insno[4].Substring(2, 2);
            string INSNO = string.Empty;
            foreach (var (value, index) in insno.Select((v, i) => (v, i)))
            {

                if (index != insno.Length - 1)
                {
                    if (index == 4)
                    {
                        INSNO += century + value + "/";
                    }
                    else
                    {
                        INSNO += value + "/";
                    }

                }
                else
                {
                    INSNO += value;
                }

            }
            return INSNO;
        }
        public static string GetBranch_and_InsNO(this string InsNO)
        {
            if (string.IsNullOrEmpty(InsNO))
            {
                return InsNO;
            }
            string orgInsNO = InsNO;
            InsNO = InsNO.Replace("-", "/");
            InsNO = InsNO.Replace(" ", "");
            InsNO = InsNO.Replace("_", "/");
            string[] sections = InsNO.Split("/");
            if (sections.Length < 5)
            {
                return orgInsNO;
            }
            string BrIn = sections[1] + sections[5];
            return BrIn;

        }
        public static string GetDisplayName(PropertyInfo propertyInfo)
        {
            string result = null;
            try
            {
                var attrs = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrs.Any())
                    result = ((DisplayAttribute)attrs[0]).Name;
            }
            catch (Exception)
            {
                //eat the exception
            }
            return result;
        }
        public static void GetThumbImage(int Width, int Height, Stream streamImg, string saveFilePath)
        {
            Bitmap sourceImage = new Bitmap(streamImg);
            using Bitmap objBitmap = new Bitmap(Width, Height);
            objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using Graphics objGraphics = Graphics.FromImage(objBitmap);
            // Set the graphic format for better result cropping   
            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            objGraphics.DrawImage(sourceImage, 0, 0, Width, Height);

            // Save the file path, note we use png format to support png file   
            objBitmap.Save(saveFilePath);
        }
        public static string GetStringShamsiNowDateTime(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return "Y" + pc.GetYear(value) +
                   "M" + pc.GetMonth(value).ToString("00") +
                   "D" + pc.GetDayOfMonth(value).ToString("00") +
                   "H" + pc.GetHour(value).ToString("00") +
                   "M" + pc.GetMinute(value).ToString("00") +
                   "S" + pc.GetSecond(value).ToString("00");
        }
        public static bool IsNumeric(this Type type)
        {
            if (type == null) { return false; }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static string GetCardNumberWithSeprator(this string CardNumber)
        {
            if(!string.IsNullOrEmpty(CardNumber))
            {
                string cd = CardNumber.Replace("-", "");
                cd = cd.Replace("-", "");
                cd = Regex.Replace(cd, ".{4}", "$0-");
                int L = cd.Length;
                cd = cd.Remove(L - 1, 1);
                return cd;
            }
            return string.Empty;
        }
        public static string GetMounthShamsiName(this int ShamsiMounth)
        {
            string mName = string.Empty;
            switch (ShamsiMounth)
            {
                case 1:
                    {
                        mName = "فروردین";
                        break;
                    }
                case 2:
                    {
                        mName = "اردیبهشت";
                        break;
                    }
                case 3:
                    {
                        mName = "خرداد";
                        break;
                    }
                case 4:
                    {
                        mName = "تیر";
                        break;
                    }
                case 5:
                    {
                        mName = "مرداد";
                        break;
                    }
                case 6:
                    {
                        mName = "شهریور";
                        break;
                    }
                case 7:
                    {
                        mName = "مهر";
                        break;
                    }
                case 8:
                    {
                        mName = "آبان";
                        break;
                    }
                case 9:
                    {
                        mName = "آذر";
                        break;
                    }
                case 10:
                    {
                        mName = "دی";
                        break;
                    }
                case 11:
                    {
                        mName = "بهمن";
                        break;
                    }
                case 12:
                    {
                        mName = "اسفند";
                        break;
                    }
                default:
                    break;
            }
            return mName;
        }
        public static List<string> GetCurrentSeasionMonthsName(this DateTime date)
        {
            PersianCalendar PC = new PersianCalendar();
            int CurMounth = PC.GetMonth(date);
            List<string> months = new List<string>();
            string month1=string.Empty;string month2=string.Empty;string month3=string.Empty;
            if(CurMounth >=1 && CurMounth<=3)
            {
                month1 = "فروردین";
                month2 = "اردیبهشت";
                month3 = "خرداد";
            }
            if(CurMounth>=4 && CurMounth<=6)
            {
                month1 = "تیر";
                month2 = "مرداد";
                month3 = "شهریور";
            }
            if (CurMounth >= 7 && CurMounth <= 9)
            {
                month1 = "مهر";
                month2 = "آبان";
                month3 = "آذر";
            }
            if (CurMounth >= 10 && CurMounth <= 12)
            {
                month1 = "دی";
                month2 = "بهمن";
                month3 = "اسفند";
            }
            months.Add(month1);
            months.Add(month2);
            months.Add(month3);
            return months;
        }
        public static List<int> GetCurrentSeasionMonthsNumber(this DateTime date)
        {
            PersianCalendar PC = new PersianCalendar();
            int CurMounth = PC.GetMonth(date);
            List<int> months = new List<int>();
            int month1 = 0; int month2 = 0; int month3 = 0;
            if (CurMounth >= 1 && CurMounth <= 3)
            {
                month1 =1;
                month2 = 2;
                month3 = 3;
            }
            if (CurMounth >= 4 && CurMounth <= 6)
            {
                month1 = 4;
                month2 = 5;
                month3 = 6;
            }
            if (CurMounth >= 7 && CurMounth <= 9)
            {
                month1 = 7;
                month2 = 8;
                month3 = 9;
            }
            if (CurMounth >= 10 && CurMounth <= 12)
            {
                month1 = 10;
                month2 = 11;
                month3 = 12;
            }
            months.Add(month1);
            months.Add(month2);
            months.Add(month3);
            return months;
        }
        public static string GetCurrentSeason(this DateTime date)
        {
            PersianCalendar PC = new PersianCalendar();
            int curMonth = PC.GetMonth(date);
            string season = string.Empty;
            if(curMonth >= 1 && curMonth <=3)
            {
                season = "بهار";
            }
            if (curMonth >= 4 && curMonth <= 6)
            {
                season = "تابستان";
            }
            if (curMonth >= 7 && curMonth <= 9)
            {
                season = "پاییز";
            }
            if (curMonth >= 10 && curMonth <= 12)
            {
                season = "زمستان";
            }
            return season;
        }
       
        
    }

}
