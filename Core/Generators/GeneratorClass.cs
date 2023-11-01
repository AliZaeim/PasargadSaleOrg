
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Core.Generators
{
    public static class GeneratorClass
    {
        public static string GenerateUniqueCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        public static string GenerateUniqueCodeByString(string text)
        {
            Guid guid = new Guid(text);
            return guid.ToString().Replace("-", "");
        }
        public static string GeneratePassword(int length,string mode="all")
        {
            string valid ;
            switch (mode.Trim().ToLower())
            {
                case "full":
                    {
                        valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890@#$%^&*";
                        break;
                    }
                case "all":
                    {
                        valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                        break;
                    }
                case "digit":
                    {
                        valid = "1234567890";
                        break;
                    }
                case "char":
                    {
                        valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        break;
                    }
                default:
                    valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                    break;
            }
            
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static void CreatetxtFile(string fileName, string Text, bool SaveinComputer)
        {
            string textpath ;
            if (string.IsNullOrEmpty(fileName))
            {
                textpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/txtFiles/", DateTime.Now.ToLongDateString() + ".txt");
            }
            else
            {
                textpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/txtFiles/", fileName + ".txt");
            }

            if(string.IsNullOrEmpty(Text))
            {
                Text="You have not sent a text to create.";
            }
            using (FileStream fs = File.Create(textpath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(Text);
                fs.Write(info, 0, info.Length);
            }
            if (SaveinComputer == true)
            {
                
                string txtFilePath=@"C:\"+fileName+".txt";
                using FileStream fs = File.Create(txtFilePath);
                Byte[] info = new UTF8Encoding(true).GetBytes(Text);
                fs.Write(info, 0, info.Length);
            }

        }
        public static string GenerateKey(List<string> Keys,int Length=4)
        {
            string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, Length);
            while (Keys.Any(a => a == key))
            {
                key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, Length);
            }
            return key;
        }
        public static string GenerateKey(List<string> Keys, int Length = 4,string Type="all")
        {
            string valid = "";
            switch (Type.Trim().ToLower())
            {
                case "all":
                    {
                        valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                        break;
                    }
                case "digit":
                    {
                        valid = "1234567890";
                        break;
                    }
                case "char":
                    {
                        valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        break;
                    }
                default:
                    valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                    break;
            }
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < Length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            string key = res.ToString();
            while (Keys.Any(a => a == key))
            {
                while (0 < Length--)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                }
                key = res.ToString();
            }
            return key;
        }
        public static int GenerateUniqueNumber(List<string> ExistNumbers,int min=1000,int max=50000000)
        {
            Random r = new Random();
            int genRand = r.Next(min, max);
            while(ExistNumbers.Any(a=>a==genRand.ToString()))
            {
                genRand = r.Next(min , max);
            }
            return genRand;
        }


    }
}
