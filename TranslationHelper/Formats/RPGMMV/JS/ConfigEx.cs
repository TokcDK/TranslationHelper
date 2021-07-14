using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class ConfigEx : JSBase
    {
        public ConfigEx(ProjectData projectData) : base(projectData)
        {
        }

        internal override string JSName => "ConfigEx.js";

        internal override bool Open()
        {
            return ParseConfigExJS();
        }

        private bool ParseConfigExJS(bool Iswrite = false)
        {
            if (projectData.FilePath.Length == 0 || !File.Exists(projectData.FilePath))
            {
                return false;
            }

            string line;

            string tablename = Path.GetFileName(projectData.FilePath);

            bool UseDict = false;
            if (Iswrite)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                if (TablesLinesDict != null && TablesLinesDict.Count > 0)
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
                using (StreamReader reader = new StreamReader(projectData.FilePath))
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
                                            if (TablesLinesDict.ContainsKey(StringToAdd) && !string.IsNullOrEmpty(TablesLinesDict[StringToAdd]) && TablesLinesDict[StringToAdd] != StringToAdd)
                                            {
                                                line = line.Replace(StringToAdd, TablesLinesDict[StringToAdd]);
                                            }
                                        }
                                        else
                                        {

                                            var row = projectData.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
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
                                        AddRowData(tablename, StringToAdd, "addCommand\\" + StringForInfo, true);
                                        //projectData.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
                                        //projectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
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
                    File.WriteAllText(projectData.FilePath, ResultForWrite.ToString());
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
