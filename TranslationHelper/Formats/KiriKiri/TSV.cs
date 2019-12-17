using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    public static class TSV
    {
        public static DataTable KiriKiriTSVOpen(string sPath, DataTable DT, DataTable DTInfo)
        {
            if (DT == null || DTInfo == null)
                return null;

            using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
            {
                string line;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    if (line.StartsWith(";") || string.IsNullOrWhiteSpace(line))
                    {
                    }
                    else
                    {
                        string[] NameValues = line.Split('	');

                        if (NameValues.Length == 2)
                        {
                            string[] Values = NameValues[1].Split(',');

                            int ValuesLength = Values.Length;
                            for (int l = 0; l < ValuesLength; l++)
                            {
                                string subline = Values[l];
                                if (string.IsNullOrEmpty(subline) || subline == "true" || subline == "false" || subline.StartsWith("0x") || FunctionsStrings.IsDigitsOnly(subline))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(subline);
                                    _ = DTInfo.Rows.Add(NameValues[0]);
                                }
                            }
                        }
                    }
                }
            }

            return DT;
        }
    }
}
