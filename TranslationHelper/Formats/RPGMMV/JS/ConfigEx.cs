using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class ConfigEx : JsBase
    {
        public ConfigEx()
        {
        }

        public override string JsName => "ConfigEx.js";

        internal override bool Open()
        {
            return ParseConfigExJs();
        }

        private bool ParseConfigExJs(bool iswrite = false)
        {
            if (ProjectData.FilePath.Length == 0 || !File.Exists(ProjectData.FilePath))
            {
                return false;
            }

            string line;

            string tablename = Path.GetFileName(ProjectData.FilePath);

            bool useDict = false;
            if (iswrite)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                if (TablesLinesDict != null && TablesLinesDict.Count > 0)
                {
                    useDict = true;
                }
            }
            else
            {
                AddTables(tablename);
            }

            StringBuilder resultForWrite = new StringBuilder();
            int rowIndex = 0;
            bool isComment = false;
            try
            {
                using (StreamReader reader = new StreamReader(ProjectData.FilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        if (isComment)
                        {
                            if (line.Contains("*/"))
                            {
                                isComment = false;
                            }
                        }
                        else
                        {
                            if (line.Contains("/*"))
                            {
                                isComment = true;
                            }

                            if (!line.TrimStart().StartsWith("//") && line.Contains("addCommand('"))
                            {
                                string stringToAdd = Regex.Replace(line, @".*addCommand\(\'([^']*)\'\, \'.*\'\)\;.*", "$1");
                                if (!string.IsNullOrWhiteSpace(stringToAdd))
                                {
                                    if (iswrite)
                                    {
                                        if (useDict)
                                        {
                                            if (TablesLinesDict.ContainsKey(stringToAdd) && !string.IsNullOrEmpty(TablesLinesDict[stringToAdd]) && TablesLinesDict[stringToAdd] != stringToAdd)
                                            {
                                                line = line.Replace(stringToAdd, TablesLinesDict[stringToAdd]);
                                            }
                                        }
                                        else
                                        {

                                            var row = ProjectData.ThFilesElementsDataset.Tables[tablename].Rows[rowIndex];
                                            if (row[0] as string == stringToAdd && row[1] != null && !string.IsNullOrEmpty(row[1] as string))
                                            {
                                                line = line.Replace(stringToAdd, row[1] as string);
                                            }

                                            rowIndex++;
                                        }
                                    }
                                    else
                                    {
                                        string stringForInfo = Regex.Replace(line, @"\.addCommand\('[^']+', '([^']+)'\);", "$2");
                                        AddRowData(tablename, stringToAdd, "addCommand\\" + stringForInfo, true);
                                        //ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
                                        //ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
                                    }
                                }
                            }
                        }

                        if (iswrite)
                        {
                            resultForWrite.AppendLine(line);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            if (iswrite)
            {
                try
                {
                    File.WriteAllText(ProjectData.FilePath, resultForWrite.ToString());
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
            return ParseConfigExJs(true);
        }
    }
}
