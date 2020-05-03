using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class ConfigEx : JSBase
    {
        public ConfigEx(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string JSName => "ConfigEx.js";

        internal override bool Open()
        {
            return ParseConfigExJS();
        }

        private bool ParseConfigExJS(bool Iswrite = false)
        {
            if (thDataWork.FilePath.Length == 0 || !File.Exists(thDataWork.FilePath))
            {
                return false;
            }

            string line;

            string tablename = Path.GetFileName(thDataWork.FilePath);

            bool UseDict = false;
            if (Iswrite)
            {
                SplitTableCellValuesToDictionaryLines(tablename);
                if (TableLines != null && TableLines.Count > 0)
                {
                    UseDict = true;
                }
            }
            else
            {
                AddTables(tablename);
            }

            StringBuilder ResultForWrite = new StringBuilder();
            int RowIndex = 0;
            bool IsComment = false;
            try
            {
                using (StreamReader reader = new StreamReader(thDataWork.FilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        if (IsComment)
                        {
                            if (line.Contains("*/"))
                            {
                                IsComment = false;
                            }
                        }
                        else
                        {
                            if (line.Contains("/*"))
                            {
                                IsComment = true;
                            }

                            if (!line.TrimStart().StartsWith("//") && line.Contains("addCommand('"))
                            {
                                string StringToAdd = Regex.Replace(line, @".*addCommand\(\'([^']*)\'\, \'.*\'\)\;.*", "$1");
                                if (!string.IsNullOrWhiteSpace(StringToAdd))
                                {
                                    if (Iswrite)
                                    {
                                        if (UseDict)
                                        {
                                            if (TableLines.ContainsKey(StringToAdd) && !string.IsNullOrEmpty(TableLines[StringToAdd]) && TableLines[StringToAdd] != StringToAdd)
                                            {
                                                line = line.Replace(StringToAdd, TableLines[StringToAdd]);
                                            }
                                        }
                                        else
                                        {

                                            var row = thDataWork.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
                                            if (row[0] as string == StringToAdd && row[1] != null && !string.IsNullOrEmpty(row[1] as string))
                                            {
                                                line = line.Replace(StringToAdd, row[1] as string);
                                            }

                                            RowIndex++;
                                        }
                                    }
                                    else
                                    {
                                        string StringForInfo = Regex.Replace(line, @"\.addCommand\('[^']+', '([^']+)'\);", "$2");
                                        AddRowData(tablename, StringToAdd, "addCommand\\" + StringForInfo);
                                        //thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
                                        //thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
                                    }
                                }
                            }
                        }

                        if (Iswrite)
                        {
                            ResultForWrite.AppendLine(line);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            if (Iswrite)
            {
                try
                {
                    File.WriteAllText(thDataWork.FilePath, ResultForWrite.ToString());
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
            {
                return CheckTablesContent(tablename);
            }
        }

        internal override bool Save()
        {
            return ParseConfigExJS(true);
        }
    }
}
