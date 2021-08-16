using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    class Tsv : FormatBase
    {
        public Tsv()
        {
        }

        internal override bool Open()
        {
            return KiriKiriTsvOpen();
        }

        public bool KiriKiriTsvOpen()
        {
            FunctionsTable.SetTableAndColumns();

            using (StreamReader file = new StreamReader(ProjectData.FilePath, Encoding.GetEncoding(932)))
            {
                string fileName = Path.GetFileName(ProjectData.FilePath);
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
                                    _ = ProjectData.ThFilesElementsDataset.Tables[fileName].Rows.Add(subline);
                                    _ = ProjectData.ThFilesElementsDatasetInfo.Tables[fileName].Rows.Add(nameValues[0]);
                                }
                            }
                        }
                    }
                }
            }

            return FunctionsTable.SetTableAndColumns(false);
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
