﻿using System.Data;
using System.IO;
using System.Text;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.KiriKiri
{
    public static class TSVOld
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
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
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
                                if (string.IsNullOrEmpty(subline) || subline == "true" || subline == "false" || subline.StartsWith("0x") || subline.IsDigitsOnly())
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
