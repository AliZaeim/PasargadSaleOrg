using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Convertors
{
    public static class ChangeString
    {
        public static string ToBreaf(this string Text, int Charachter_length = 10)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                string Result = "";
                int textLength = Text.Length;
                string Isolatedtext;
                
                if (textLength > Charachter_length)
                {
                    Isolatedtext = Text.Trim().Substring(0, Charachter_length);

                    string[] IsolatedtextArray = Isolatedtext.Split(' ');
                    List<string> IsolList = new List<string>();
                    IsolList.AddRange(IsolatedtextArray);
                    string[] AllTextArray = Text.Trim().Split(' ');
                    List<string> TextList = new List<string>();
                    TextList.AddRange(AllTextArray);
                    List<string> shares = TextList.Intersect(IsolList).ToList();
                    foreach (var item in shares)
                    {
                        Result += item + " ";
                    }
                    Text = Result;
                }
            }
            return Text;
        }
       
    }
}
