using System.Data;
using System.IO;
using System.Text;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.KiriKiri
{
    public static class TsvOld
    {
        public static DataTable KiriKiriTsvOpen(string sPath, DataTable dt, DataTable dtInfo)
        {
            if (dt == null || dtInfo == null)
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
                        string[] nameValues = line.Split('	');

                        if (nameValues.Length == 2)
                        {
                            string[] values = nameValues[1].Split(',');

                            int valuesLength = values.Length;
                            for (int l = 0; l < valuesLength; l++)
                            {
                                string subline = values[l];
                                if (string.IsNullOrEmpty(subline) || subline == "true" || subline == "false" || subline.StartsWith("0x") || subline.IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    _ = dt.Rows.Add(subline);
                                    _ = dtInfo.Rows.Add(nameValues[0]);
                                }
                            }
                        }
                    }
                }
            }

            return dt;
        }
    }
}
