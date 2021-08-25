using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;

namespace TranslationHelper.Formats
{
    abstract class FormatBase
    {
        protected FormatBase()
        {
            if (ProjectData.CurrentProject != null)
            {
                ProjectData.CurrentProject.CurrentFormat = this;

                if (Properties.Settings.Default.DontLoadDuplicates)
                {
                    ProjectData.CurrentProject.Hashes = new HashSet<string>();
                }
            }
        }

        /// <summary>
        /// chack if format can be parsed?
        /// </summary>
        /// <returns></returns>
        internal virtual bool Check()
        {
            return false;
        }

        /// <summary>
        /// extension which can be parsed with the format, ".txt" or ".txt,.csv" for example.
        /// override ExtIdentifier() to determine when a file with the extension can be opened
        /// </summary>
        /// <returns></returns>
        internal virtual string Ext()
        {
            return null;
        }

        /// <summary>
        /// identifier to check how to identify if selected extension must be parsed with this format.
        /// in result can be added new project which will be used Ext and this identifier to open valid standalone files.
        /// </summary>
        /// <returns></returns>
        internal virtual bool ExtIdentifier()
        {
            return false;
        }

        /// <summary>
        /// name of format
        /// </summary>
        /// <returns></returns>
        internal virtual string Name()
        {
            return string.Empty;
        }

        internal virtual bool Open() { return false; }

        internal virtual bool Save() { return false; }

        /// <summary>
        /// Means use for table name name of file without extension
        /// </summary>
        internal virtual bool UseTableNameWithoutExtension => false;

        /// <summary>
        /// table name
        /// </summary>
        protected virtual string TableName()
        {
            return UseTableNameWithoutExtension ? Path.GetFileNameWithoutExtension(ProjectData.FilePath) : Path.GetFileName(ProjectData.FilePath);
        }

        /// <summary>
        /// file destination for write
        /// usually it is same path as open but some time it can be other dir
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFilePath()
        {
            return ProjectData.FilePath;
        }

        /// <summary>
        /// Add table to work dataset
        /// </summary>
        protected void AddTables()
        {
            if (!string.IsNullOrEmpty(ProjectData.FilePath))
            {
                FormatUtils.AddTables(TableName());
            }
        }

        /// <summary>
        /// Pre open file actions
        /// </summary>
        protected virtual bool FilePreOpenActions()
        {
            if (ProjectData.SaveFileMode && !ProjectData.THFilesList.Items.Contains(TableName()))
            {
                return false;
            }

            if (ProjectData.OpenFileMode)
            {
                FormatUtils.AddTables(TableName());
            }

            if (ProjectData.SaveFileMode)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(TableName(), false, false);
            }

            PreOpenExtraActions();

            return true;
        }

        /// <summary>
        /// Pre open file extra actions
        /// </summary>
        protected virtual void PreOpenExtraActions()
        {
        }

        /// <summary>
        /// Base Parse File function
        /// </summary>
        /// <param name="IsOpen"></param>
        /// <returns></returns>
        protected bool ParseFile()
        {
            if (!FilePreOpenActions())
            {
                return false;
            }

            FileOpen();

            return FilePostOpen();
        }

        /// <summary>
        /// Open file actions
        /// </summary>
        protected virtual void FileOpen()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool FilePostOpen()
        {
            if (ProjectData.OpenFileMode)
            {
                return CheckTablesContent(TableName());
            }
            else
            {
                return WriteFileData();
            }
        }

        protected virtual bool WriteFileData(string filePath = "")
        {
            return false;
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(string RowData, string RowInfo = "", bool CheckInput = true)
        {
            return AddRowData(Path.GetFileName(ProjectData.FilePath), RowData, RowInfo, CheckInput);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(string[] RowData, string RowInfo, bool CheckInput = true)
        {
            return AddRowData(Path.GetFileName(ProjectData.FilePath), RowData, RowInfo, CheckInput);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckInput">cheack original string if valid</param>
        /// <returns></returns>
        internal bool AddRowData(string tablename, string RowData, string RowInfo, bool CheckInput = true)
        {
            return AddRowData(tablename, new string[] { RowData }, RowInfo, CheckInput);
        }

        /// <summary>
        /// original string modification before add it with AddRowData.
        /// default will be returned same string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected virtual string AddRowDataPreAddOriginalStringMod(string str)
        {
            return str;
        }

        /// <summary>
        /// Check string if it is valid for add to work table.
        /// Usually it is not empty string. For japanese language it is also string contain most of japanese chars
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal virtual bool IsValidString(string inputString)
        {
            //preclean string
            inputString = ProjectData.CurrentProject.CleanStringForCheck(inputString);

            return !string.IsNullOrWhiteSpace(inputString) && !inputString.ForJPLangHaveMostOfRomajiOtherChars();
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckInput">cheack original string if valid</param>
        /// <returns></returns>
        internal bool AddRowData(string tablename, string[] RowData, string RowInfo, bool CheckInput = true)
        {
            var original = AddRowDataPreAddOriginalStringMod(RowData[0]);

            if (CheckInput && !IsValidString(original))
            {
                return false;
            }

            if (Properties.Settings.Default.DontLoadDuplicates)
            {
                if (ProjectData.CurrentProject.Hashes == null || ProjectData.CurrentProject.Hashes.Contains(original))
                {
                    return false;
                }

                // add to hashes when only unique values
                ProjectData.CurrentProject.Hashes.Add(original);
            }
            else
            {
                // variant with duplicates

                // check if original exists
                if (!ProjectData.OriginalsTableRowCoordinats.ContainsKey(original))
                {
                    ProjectData.OriginalsTableRowCoordinats.Add(original, new Dictionary<string, HashSet<int>>());
                }

                // check if tablename is exists
                if (!ProjectData.OriginalsTableRowCoordinats[original].ContainsKey(tablename))
                {
                    ProjectData.OriginalsTableRowCoordinats[original].Add(tablename, new HashSet<int>());
                }

                // check if current row number is exists
                if (!ProjectData.OriginalsTableRowCoordinats[original][tablename].Contains(RowNumber))
                {
                    ProjectData.OriginalsTableRowCoordinats[original][tablename].Add(RowNumber);
                }

                // raise row number
                RowNumber++;
            }

            ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Add(RowData);
            ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add(RowInfo);

            return true;
        }

        /// <summary>
        /// current row number in parsing table
        /// </summary>
        protected int RowNumber = 0;

        protected bool CheckTablesContent(string tablename, bool IsDictionary = false)
        {
            if (IsDictionary /*&& ProjectData.THFilesElementsDictionary != null && ProjectData.THFilesElementsDictionary.Count > 0 && ProjectData.THFilesElementsDataset.Tables[tablename] != null && ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Count == 0*/)
            {
                throw new NotImplementedException("Dictionary not implemented");
                //return ProjectData.THFilesElementsDataset.Tables[tablename].FillWithDictionary(ProjectData.THFilesElementsDictionary);
            }
            else if (ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                if (ProjectData.THFilesElementsDataset.Tables.Contains(tablename))
                {
                    ProjectData.THFilesElementsDataset.Tables.Remove(tablename); // remove table if was no items added
                }

                if (ProjectData.THFilesElementsDatasetInfo.Tables.Contains(tablename))
                {
                    ProjectData.THFilesElementsDatasetInfo.Tables.Remove(tablename); // remove table if was no items added
                }

                return false;
            }
        }

        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="TableName"></param>
        internal void SplitTableCellValuesToDictionaryLines(string TableName)
        {
            if (!Properties.Settings.Default.DontLoadDuplicates || !ProjectData.SaveFileMode || !ProjectData.THFilesElementsDataset.Tables.Contains(TableName))
                return;

            if (ProjectData.CurrentProject.TablesLinesDict == null)
            {
                ProjectData.CurrentProject.TablesLinesDict = new Dictionary<string, string>();
            }

            if (ProjectData.CurrentProject.TablesLinesDict.Count > 0)
            {
                ProjectData.CurrentProject.TablesLinesDict.Clear();
            }

            foreach (DataRow Row in ProjectData.THFilesElementsDataset.Tables[TableName].Rows)
            {
                string Original;
                string Translation;
                if (ProjectData.CurrentProject.TablesLinesDict.ContainsKey(Original = Row[0] + string.Empty) || (Translation = Row[1] + string.Empty).Length == 0 || Translation == Original)
                {
                    continue;
                }

                ProjectData.CurrentProject.TablesLinesDict.Add(Original, Translation);
            }
        }

        bool TablesLinesDictFilled;
        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="makeLinesCountEqual">if true, line count will be made equal in translation before add original else it will be made only for multiline and rigth after line by line check</param>
        /// <param name="onlyOneTable">Parse only <paramref name="tableName"/></param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string tableName, bool makeLinesCountEqual = true, bool onlyOneTable = true)
        {
            if (!Properties.Settings.Default.DontLoadDuplicates) // skip if do not load duplicates option is disabled
            {
                return;
            }

            if (onlyOneTable)
            {
                if (!ProjectData.THFilesElementsDataset.Tables.Contains(tableName))
                    return;

                if (ProjectData.CurrentProject.TablesLinesDict.Count > 0)
                {
                    ProjectData.CurrentProject.TablesLinesDict.Clear();
                }
            }
            else
            {
                if (TablesLinesDictFilled /*|| ProjectData.CurrentProject.TablesLinesDict != null && ProjectData.CurrentProject.TablesLinesDict.Count > 0*/)
                {
                    return;
                }
            }


            foreach (DataTable Table in ProjectData.THFilesElementsDataset.Tables)
            {
                if (onlyOneTable && Table.TableName != tableName)
                {
                    continue;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    string Original = (Row[0] + string.Empty);
                    int OriginalLinesCount = Original.GetLinesCount();
                    if (OriginalLinesCount == 1 && ProjectData.CurrentProject.TablesLinesDict.ContainsKey(Original))
                    {
                        continue;
                    }

                    string Translation = (Row[1] + string.Empty);
                    if (Translation.Length == 0)
                    {
                        continue;
                    }

                    int TranslationLinesCount = Translation.GetLinesCount();
                    bool LinesCountisEqual = OriginalLinesCount == TranslationLinesCount;
                    if (!LinesCountisEqual && makeLinesCountEqual)
                    {
                        if (OriginalLinesCount > Translation.Length)
                        {
                            continue;//skip lines where translation is incosistent to original
                        }

                        Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                    }

                    //Сначала добавить полный вариант
                    if (!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(Original) /*&& ((!ProjectData.CurrentProject.ProjectData.CurrentProject.TablesLinesDictAddEqual && Translation != Original) || ProjectData.CurrentProject.ProjectData.CurrentProject.TablesLinesDictAddEqual)*/)
                    {
                        ProjectData.CurrentProject.TablesLinesDict.Add(Original, Translation/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                        if (OriginalLinesCount == 1)
                        {
                            continue;//когда одна строка не тратить время на её разбор
                        }
                    }
                    else if (Translation != Original && Original == ProjectData.CurrentProject.TablesLinesDict[Original])
                    {
                        ProjectData.CurrentProject.TablesLinesDict[Original] = Translation;
                        if (OriginalLinesCount == 1)
                        {
                            continue;//когда одна строка не тратить время на её разбор
                        }
                    }

                    if (!makeLinesCountEqual && OriginalLinesCount > TranslationLinesCount)
                    {
                        if (OriginalLinesCount > Translation.Length)
                        {
                            continue;//skip lines where translation is incosistent to original
                        }

                        Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                    }

                    //string[] OriginalLines = FunctionsString.SplitStringToArray(Original);
                    string[] TranslationLines = Translation.GetAllLinesToArray();
                    string[] OriginalLines = Original.GetAllLinesToArray();
                    List<string> extralines = new List<string>();
                    for (int lineNumber = 0; lineNumber < TranslationLinesCount; lineNumber++)
                    {
                        try
                        {
                            if (LinesCountisEqual) //когда количество строк равно, просто добавлять валидные строки в словарь
                            {
                                if (!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                                {
                                    ProjectData.CurrentProject.TablesLinesDict.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                }
                            }
                            else
                            {
                                if (lineNumber < OriginalLinesCount - 1) // пока номер строки меньше номера последней строки в оригинале
                                {
                                    if (!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                                    {
                                        ProjectData.CurrentProject.TablesLinesDict.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                    }
                                }
                                else // номер строки равен номеру последней строки оригинала
                                {
                                    if (lineNumber == TranslationLinesCount - 1) //если номер строки равен номеру последней строки в переводе
                                    {
                                        if (extralines.Count > 0) // если список экстра строк не пустой
                                        {
                                            extralines.Add(TranslationLines[lineNumber]); // добавить последнюю строку в переводе
                                            string result = string.Join(Environment.NewLine, extralines); // объединить экстра строки в одну


                                            if (!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1]) //если словарь не содержит последнюю строку оригинала
                                                && result.Trim().Length > 0 // объединенные строки без пробельных символов и символов новой строки - не пустые 
                                                && OriginalLines[OriginalLinesCount - 1] != result) // оригинал не равен переводу
                                            {
                                                //добавить оригинал с переводом содержащим больше строк, чем в оригинале
                                                ProjectData.CurrentProject.TablesLinesDict.Add(OriginalLines[OriginalLinesCount - 1], result/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                            }
                                        }
                                        else
                                        {
                                            // при пустом списке экстра строк добавить в словарь оригинал с переводом, если валидный
                                            if (!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[OriginalLinesCount - 1] != TranslationLines[lineNumber])
                                            {
                                                ProjectData.CurrentProject.TablesLinesDict.Add(OriginalLines[OriginalLinesCount - 1], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                            }
                                        }
                                    }
                                    else // пока номер текущей строки меньше номера последней строки в переводе, добавлять экстра строки в один список
                                    {
                                        extralines.Add(TranslationLines[lineNumber]);
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                    //foreach (string line in Original.SplitToLines())
                    //{
                    //    if (!TableLines.ContainsKey(line) && TranslationLines[lineNumber].Length > 0 && TranslationLines[lineNumber] != line)
                    //    {
                    //        TableLines.Add(line, TranslationLines[lineNumber]);
                    //    }
                    //    lineNumber++;
                    //}
                }
            }
            TablesLinesDictFilled = true;
        }

        /// <summary>
        /// check if translation is exists and set str return true if found.
        /// <paramref name="valueToTranslate"/> = input string, must contain original value for search.
        /// <paramref name="existsTranslation"/> = control translation value, if was loaded translation from file
        /// </summary>
        /// <param name="valueToTranslate">input=original, output=translation</param>
        /// <param name="existsTranslation">control translation value if was loaded translation from file</param>
        /// <returns>true if translation was set and not equal to input original</returns>
        internal bool SetTranslation(ref string valueToTranslate, string existsTranslation = null)
        {
            var isTranslated = false;

            if (!Properties.Settings.Default.DontLoadDuplicates
                && ProjectData.OriginalsTableRowCoordinats != null
                && ProjectData.OriginalsTableRowCoordinats.ContainsKey(valueToTranslate) // input value has original's value before it will be changed to translation
                )
            {
                var currentTableName = TableName();
                var pretranslatedOriginal = valueToTranslate;
                if (ProjectData.OriginalsTableRowCoordinats[valueToTranslate].ContainsKey(currentTableName))
                {
                    if (ProjectData.OriginalsTableRowCoordinats[valueToTranslate][currentTableName].Contains(RowNumber))
                    {
                        valueToTranslate = ProjectData.THFilesElementsDataset.Tables[currentTableName].Rows[RowNumber][1] + "";
                        valueToTranslate = FixInvalidSymbols(valueToTranslate);

                        isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                        if (isTranslated)
                        {
                            SetTranslationIsTranslatedAction();
                        }

                        //return ret;
                    }
                    else // set 1st value from avalaible values
                    {
                        ProjectData.AppLog.LogToFile("Warning! Row not found. row number=" + RowNumber + ". table name=" + TableName() + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                        foreach (var rowIndex in ProjectData.OriginalsTableRowCoordinats[valueToTranslate][currentTableName])
                        {
                            valueToTranslate = ProjectData.THFilesElementsDataset.Tables[currentTableName].Rows[rowIndex][1] + "";
                            valueToTranslate = FixInvalidSymbols(valueToTranslate);

                            isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                            if (isTranslated)
                            {
                                SetTranslationIsTranslatedAction();
                            }

                            //return ret;
                        }
                    }
                }
                else // set 1st value from avalaible values
                {
                    ProjectData.AppLog.LogToFile("Warning! Table not found. row number=" + RowNumber + ". table name=" + TableName() + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                    foreach (var existTableName in ProjectData.OriginalsTableRowCoordinats[valueToTranslate].Values)
                    {
                        foreach (var existsRowIndex in existTableName)
                        {
                            valueToTranslate = ProjectData.THFilesElementsDataset.Tables[currentTableName].Rows[existsRowIndex][1] + "";
                            valueToTranslate = FixInvalidSymbols(valueToTranslate);

                            isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                            if (isTranslated)
                            {
                                SetTranslationIsTranslatedAction();
                                break; // translated, dont need to iterate rows anymore
                            }

                            //return ret;
                        }

                        if (isTranslated)
                        {
                            SetTranslationIsTranslatedAction();
                            break; // translated, dont need to iterate table names anymore
                        }
                    }
                }

                RowNumber++;
            }
            else if (ProjectData.CurrentProject.TablesLinesDict.ContainsKey(valueToTranslate))
            {
                var control = valueToTranslate;
                valueToTranslate = ProjectData.CurrentProject.TablesLinesDict[valueToTranslate];
                valueToTranslate = FixInvalidSymbols(valueToTranslate);

                isTranslated = control != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                if (isTranslated)
                {
                    SetTranslationIsTranslatedAction();
                }

                //return ret;
            }

            return isTranslated;
        }

        protected virtual void SetTranslationIsTranslatedAction()
        {
        }

        /// <summary>
        /// remove invalid symbols for the project or replace them to some valid.
        /// applied to found translation before add it.
        /// default is same string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected virtual string FixInvalidSymbols(string str)
        {
            return str
                .Replace("\u200b", string.Empty)//remove zero-length-space (can be produced by online translator)
                ;
        }
    }
}
