using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    class TSV : FormatBase
    {
        public TSV(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return KiriKiriTSVOpen();
        }

        public bool KiriKiriTSVOpen()
        {
            FunctionsTable.SetTableAndColumns(thDataWork);

            using (StreamReader file = new StreamReader(thDataWork.FilePath, Encoding.GetEncoding(932)))
            {
                string fileName = Path.GetFileName(thDataWork.FilePath);
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
                                if (string.IsNullOrEmpty(subline) || subline == "true" || subline == "false" || subline.StartsWith("0x") || subline.IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    _ = thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(subline);
                                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(NameValues[0]);
                                }
                            }
                        }
                    }
                }
            }

            return FunctionsTable.SetTableAndColumns(thDataWork, false);
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
