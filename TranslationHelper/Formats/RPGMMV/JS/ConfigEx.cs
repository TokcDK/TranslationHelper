using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class ConfigEx : JSBase
    {
        public ConfigEx()
        {
        }

        public override string JSName => "ConfigEx.js";

        internal override bool Open()
        {
            return ParseConfigExJS();
        }

        private bool ParseConfigExJS(bool Iswrite = false)
        {
            if (ProjectData.FilePath.Length == 0 || !File.Exists(ProjectData.FilePath))
            {
                return false;
            }

            string line;

            string tablename = TableName();

            bool UseDict = false;
            if (Iswrite)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(tablename, makeLinesCountEqual: false, onlyOneTable: false);
                if (ProjectData.CurrentProject.TablesLinesDict != null && ProjectData.CurrentProject.TablesLinesDict.Count > 0)
                {
                    UseDict = true;
                }
            }
            else
            {
                AddTables();
            }

            StringBuilder ResultForWrite = new StringBuilder();
            int RowIndex = 0;
            bool IsComment = false;
            try
            {
                using (StreamReader reader = new StreamReader(ProjectData.FilePath))
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
                                            if (ProjectData.CurrentProject.TablesLinesDict.ContainsKey(StringToAdd) && !string.IsNullOrEmpty(ProjectData.CurrentProject.TablesLinesDict[StringToAdd]) && ProjectData.CurrentProject.TablesLinesDict[StringToAdd] != StringToAdd)
                                            {
                                                line = line.Replace(StringToAdd, ProjectData.CurrentProject.TablesLinesDict[StringToAdd]);
                                            }
                                        }
                                        else
                                        {

                                            var row = ProjectData.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
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
                                        AddRowData(tablename, StringToAdd, "addCommand\\" + StringForInfo, CheckInput: true);
                                        //ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
                                        //ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
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
                    File.WriteAllText(ProjectData.FilePath, ResultForWrite.ToString());
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
