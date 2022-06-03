using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;

namespace TranslationHelper.Formats
{
    abstract class FormatBase
    {
        protected FormatBase()
        {
            BaseInit();
        }

        bool _filePathIsSet = false;
        protected FormatBase(string filePath)
        {
            FilePath = filePath;
            _filePathIsSet = true;
            BaseInit();
        }

        private void BaseInit()
        {
            if (ProjectData.CurrentProject == null)
            {
                return;
            }

            ProjectData.CurrentProject.CurrentFormat = this; // set format

            // set filepath from projectdata filepath
            if (!_filePathIsSet && string.IsNullOrWhiteSpace(FilePath))
            {
                //if (!string.IsNullOrWhiteSpace(ProjectData.FilePath))// wrong for threaded files open
                //{
                //    FilePath = ProjectData.FilePath;
                //}
                //else
                if (!string.IsNullOrWhiteSpace(ProjectData.SelectedFilePath))
                {
                    FilePath = ProjectData.SelectedFilePath;
                }
            }

            // DontLoadDuplicates options
            if (!ProjectData.CurrentProject.DontLoadDuplicates)
            {
                return;
            }

            if (ProjectData.SaveFileMode)
            {
                if (ProjectData.CurrentProject.TablesLinesDict == null)
                {
                    ProjectData.CurrentProject.TablesLinesDict = new ConcurrentDictionary<string, string>();
                }
            }
            else
            {
                if (ProjectData.CurrentProject.Hashes == null)
                {
                    ProjectData.CurrentProject.Hashes = new ConcurrentSet<string>();
                }
            }
        }

        /// <summary>
        /// current file path for open
        /// </summary>
        internal string FilePath;
        protected virtual string GetFilePath()
        {
            return ProjectData.OpenFileMode ? GetOpenFilePath() : GetSaveFilePath();
        }
        protected virtual string GetOpenFilePath()
        {
            return FilePath;
        }
        protected virtual string GetSaveFilePath()
        {
            return FilePath;
        }

        /// <summary>
        /// check if format can be parsed?
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
        internal virtual int ExtIdentifier()
        {
            return 0; // 0 means not use identifier
        }

        /// <summary>
        /// name of format
        /// </summary>
        /// <returns></returns>
        internal virtual string Name()
        {
            return string.Empty;
        }

        /// <summary>
        /// Open file strings actions executing here
        /// </summary>
        /// <returns></returns>
        internal virtual bool Open() => ParseFile();

        /// <summary>
        /// Save file strings actions executing here
        /// </summary>
        /// <returns></returns>
        internal virtual bool Save() => ParseFile();

        /// <summary>
        /// Means use for table name name of file without extension
        /// </summary>
        internal virtual bool UseTableNameWithoutExtension => false;

        /// <summary>
        /// table name
        /// </summary>
        internal virtual string TableName()
        {
            string tableName = "";
            string filePath = GetOpenFilePath();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                int cnt = 10;
                while (string.IsNullOrWhiteSpace(filePath) && cnt > 0)
                {
                    filePath = GetOpenFilePath();
                    cnt--;
                }
            }
            if (ProjectData.CurrentProject.SubpathInTableName)
            {
                var dirPath = Path.GetDirectoryName(filePath);
                if (dirPath == null)
                {

                }
                tableName = dirPath.Replace(ProjectData.CurrentProject.OpenedFilesDir, string.Empty) + Path.DirectorySeparatorChar;
            }

            if (UseTableNameWithoutExtension)
            {
                tableName += Path.GetFileNameWithoutExtension(filePath);
            }
            else
            {
                tableName += Path.GetFileName(filePath);
            }

            return tableName;
        }

        /// <summary>
        /// Add table to work dataset
        /// </summary>
        protected void AddTables()
        {
            if (!string.IsNullOrEmpty(GetOpenFilePath()))
            {
                AddTables(TableName());
            }
        }

        /// <summary>
        /// Main table with row data
        /// </summary>
        DataTable Data;
        /// <summary>
        /// info table for info box
        /// </summary>
        DataTable Info;

        /// <summary>
        /// Add table to work dataset
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="extraColumns"></param>
        internal void AddTables(string tablename, string[] extraColumns = null)
        {
            if (Data == null)
            {
                Data = new DataTable
                {
                    TableName = tablename
                };
                Data.Columns.Add(THSettings.OriginalColumnName());
                Data.Columns.Add(THSettings.TranslationColumnName());

                if (extraColumns != null && extraColumns.Length > 0)
                {
                    foreach (var columnName in extraColumns)
                    {
                        Data.Columns.Add(columnName);
                    }
                }

                Info = new DataTable
                {
                    TableName = tablename
                };
                Info.Columns.Add("Info");
            }

            //var tables = ProjectData.THFilesElementsDataset.Tables;
            //if (!tables.Contains(tablename))
            //{
            //    tables.Add(tablename);
            //    var table = tables[tablename];
            //    table.Columns.Add(THSettings.OriginalColumnName());
            //    table.Columns.Add(THSettings.TranslationColumnName());

            //    if (extraColumns != null && extraColumns.Length > 0)
            //    {
            //        foreach (var columnName in extraColumns)
            //        {
            //            table.Columns.Add(columnName);
            //        }
            //    }
            //}
            //var tablesInfo = ProjectData.THFilesElementsDatasetInfo.Tables;
            //if (!tablesInfo.Contains(tablename))
            //{
            //    tablesInfo.Add(tablename);
            //    tablesInfo[tablename].Columns.Add("Info");
            //}
        }

        /// <summary>
        /// Add string to table with options. In save mode will replace RowData with translation
        /// </summary>
        /// <param name="RowData">reference to original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(ref string RowData, string RowInfo = "", bool CheckInput = true, string existsTranslation = null)
        {
            if (ProjectData.OpenFileMode)
            {
                return AddRowData(TableName(), RowData, RowInfo, CheckInput);
            }
            else
            {
                if (CheckInput)
                {
                    if (!IsValidString(RowData))
                    {
                        return false;
                    }
                }
                return SetTranslation(ref RowData, existsTranslation);
            }
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(string RowData, string RowInfo = "", bool CheckInput = true)
        {
            return AddRowData(TableName(), RowData, RowInfo, CheckInput);
        }
        /// <summary>
        /// Add string to table with options. In save mode will replace <paramref name="RowData"/>[0] as translation and will use <paramref name="RowData"/>[1] as default translation 
        /// </summary>
        /// <param name="RowData">First value is Original, second value is translation.</param>
        /// <param name="RowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(ref string[] RowData, string RowInfo = "", bool CheckInput = true)
        {
            if (ProjectData.OpenFileMode)
            {
                return AddRowData(TableName(), RowData, RowInfo, CheckInput);
            }
            else
            {
                if (CheckInput && !IsValidString(RowData[0])) return false;

                return SetTranslation(ref RowData[0], RowData[1]);
            }
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(string[] RowData, string RowInfo = "", bool CheckInput = true)
        {
            return AddRowData(TableName(), RowData, RowInfo, CheckInput);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckInput">cheack original string if valid</param>
        /// <returns></returns>
        internal bool AddRowData(string tablename, string RowData, string RowInfo = "", bool CheckInput = true)
        {
            return AddRowData(tablename, new string[] { RowData }, RowInfo, CheckInput);
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

            if (ProjectData.CurrentProject.DontLoadDuplicates)
            {
                if (ProjectData.CurrentProject.Hashes == null || ProjectData.CurrentProject.Hashes.Contains(original))
                {
                    return false;
                }

                // add to hashes when only unique values
                ProjectData.CurrentProject.Hashes.TryAdd(original);
            }
            else
            {
                // variant with duplicates

                // check if original exists
                if (!ProjectData.OriginalsTableRowCoordinates.ContainsKey(original))
                {
                    ProjectData.OriginalsTableRowCoordinates.TryAdd(original, new ConcurrentDictionary<string, ConcurrentSet<int>>());
                }

                // check if tablename is exists
                if (!ProjectData.OriginalsTableRowCoordinates[original].ContainsKey(tablename))
                {
                    ProjectData.OriginalsTableRowCoordinates[original].TryAdd(tablename, new ConcurrentSet<int>());
                }

                // check if current row number is exists
                if (!ProjectData.OriginalsTableRowCoordinates[original][tablename].Contains(RowNumber))
                {
                    ProjectData.OriginalsTableRowCoordinates[original][tablename].TryAdd(RowNumber);
                }

                // raise row number
                RowNumber++;
            }

            try
            {
                Data.Rows.Add(RowData);
                Info.Rows.Add(RowInfo);
            }
            catch (Exception ex)
            {

            }

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
            else if (Data.Rows.Count > 0)
            {
                ProjectData.AddFileData(Data);
                ProjectData.AddFileInfo(Info);

                //#if DEBUG
                //                ProjectData.Main.Invoke((Action)(() => ProjectData.AddTableData(TableData)));
                //                ProjectData.Main.Invoke((Action)(() => ProjectData.AddTableInfo(TableInfo)));
                //#else
                //                ProjectData.AddTableData(TableData);
                //                ProjectData.AddTableInfo(TableInfo);
                //#endif

                return true;
            }
            else
            {
                //if (ProjectData.THFilesElementsDataset.Tables.Contains(tablename))
                //{
                //ProjectData.THFilesElementsDataset.Tables.Remove(tablename); // remove table if was no items added
                //}

                //if (ProjectData.THFilesElementsDatasetInfo.Tables.Contains(tablename))
                //{
                //ProjectData.THFilesElementsDatasetInfo.Tables.Remove(tablename); // remove table if was no items added
                //}

                return false;
            }
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
        /// Pre open file actions
        /// </summary>
        protected virtual bool FilePreOpenActions()
        {
            if (!string.IsNullOrWhiteSpace(Ext()) && Path.GetExtension(GetOpenFilePath()) != Ext()) // extension must be same as set, if set
            {
                return false;
            }

            if (ProjectData.SaveFileMode && !ProjectData.THFilesList.Items.Contains(TableName()))
            {
                return false;
            }

            if (ProjectData.OpenFileMode)
            {
                AddTables();
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
        protected virtual void PreOpenExtraActions() { }

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

        protected enum KeywordActionAfter
        {
            Break = -1,
            Continue = 0,
            ReadToEnd = 1
        }

        /// <summary>
        /// Open file actions
        /// </summary>
        protected virtual void FileOpen() { }

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

        protected virtual bool WriteFileData(string filePath = "") => false;

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
        internal void SplitTableCellValuesToDictionaryLines(string TableName)
        {
            if (!ProjectData.CurrentProject.DontLoadDuplicates || !ProjectData.SaveFileMode || !ProjectData.FilesContent.Tables.Contains(TableName))
                return;

            if (ProjectData.CurrentProject.TablesLinesDict == null)
            {
                ProjectData.CurrentProject.TablesLinesDict = new ConcurrentDictionary<string, string>();
            }

            if (ProjectData.CurrentProject.TablesLinesDict.Count > 0)
            {
                ProjectData.CurrentProject.TablesLinesDict.Clear();
            }

            foreach (DataRow Row in ProjectData.FilesContent.Tables[TableName].Rows)
            {
                string Original;
                string Translation;
                if (ProjectData.CurrentProject.TablesLinesDict.ContainsKey(Original = Row[0] + string.Empty) || (Translation = Row[1] + string.Empty).Length == 0 || Translation == Original)
                {
                    continue;
                }

                ProjectData.CurrentProject.TablesLinesDict.TryAdd(Original, Translation);
            }
        }

        object SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock = new object();
        bool TablesLinesDictFilled = false;
        bool FirstPassOfDictionaryFilling = true;
        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="makeLinesCountEqual">if true, line count will be made equal in translation before add original else it will be made only for multiline and rigth after line by line check</param>
        /// <param name="onlyOneTable">Parse only <paramref name="tableName"/></param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string tableName, bool makeLinesCountEqual = false, bool onlyOneTable = false)
        {
            if (!ProjectData.CurrentProject.DontLoadDuplicates) // skip if do not load duplicates option is disabled
            {
                return;
            }

            if (!FirstPassOfDictionaryFilling && !TablesLinesDictFilled && !onlyOneTable)
            {
                int i = 20;
                while (!TablesLinesDictFilled && --i > 0) // wait until dictionary will be filled
                {
                    Thread.Sleep(100);
                }

                return; // return when filled
            }

            FirstPassOfDictionaryFilling = false;
            if (ProjectData.CurrentProject.TablesLinesDict == null)
            {
                ProjectData.CurrentProject.TablesLinesDict = new ConcurrentDictionary<string, string>();
            }

            if (onlyOneTable)
            {
                if (!ProjectData.FilesContent.Tables.Contains(tableName))
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


            foreach (DataTable Table in ProjectData.FilesContent.Tables)
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
                        try
                        {
                            lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
                            {
                                ProjectData.CurrentProject.TablesLinesDict.TryAdd(Original, Translation/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                            }
                        }
                        catch (ArgumentException) { }

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
                        if (LinesCountisEqual) //когда количество строк равно, просто добавлять валидные строки в словарь
                        {
                            if (!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                            {
                                try
                                {
                                    lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
                                    {
                                        ProjectData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                    }
                                }
                                catch (ArgumentException) { }
                            }
                        }
                        else
                        {
                            if (lineNumber < OriginalLinesCount - 1) // пока номер строки меньше номера последней строки в оригинале
                            {
                                if (/*!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) &&*/ TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                                {
                                    try
                                    {
                                        lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
                                        {
                                            ProjectData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                        }
                                    }
                                    catch (ArgumentException) { }
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


                                        if (/*!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1])*/ //если словарь не содержит последнюю строку оригинала
                                            /*&&*/ result.Trim().Length > 0 // объединенные строки без пробельных символов и символов новой строки - не пустые 
                                            && OriginalLines[OriginalLinesCount - 1] != result) // оригинал не равен переводу
                                        {
                                            try
                                            {
                                                lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
                                                {
                                                    //добавить оригинал с переводом содержащим больше строк, чем в оригинале
                                                    ProjectData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[OriginalLinesCount - 1], result/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                                }
                                            }
                                            catch (ArgumentException) { }
                                        }
                                    }
                                    else
                                    {
                                        // при пустом списке экстра строк добавить в словарь оригинал с переводом, если валидный
                                        if (/*!ProjectData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1]) &&*/ TranslationLines[lineNumber].Length > 0 && OriginalLines[OriginalLinesCount - 1] != TranslationLines[lineNumber])
                                        {
                                            try
                                            {
                                                lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
                                                {
                                                    ProjectData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[OriginalLinesCount - 1], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                                }
                                            }
                                            catch (ArgumentException) { }
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
                    //foreach (string line in Original.SplitToLines())
                    //{
                    //    if (!TableLines.ContainsKey(line) && TranslationLines[lineNumber].Length > 0 && TranslationLines[lineNumber] != line)
                    //    {
                    //        TableLines.Add(line, TranslationLines[lineNumber]);
                    //    }
                    //    lineNumber++;
                    //}
                }

                if (onlyOneTable)
                {
                    break;
                }
            }
            TablesLinesDictFilled = true;
        }

        public bool RET { get; internal set; }

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
            bool letDuplicates = !ProjectData.CurrentProject.DontLoadDuplicates;
            if (letDuplicates
                && ProjectData.OriginalsTableRowCoordinates != null
                && ProjectData.OriginalsTableRowCoordinates.ContainsKey(valueToTranslate) // input value has original's value before it will be changed to translation
                )
            {
                var currentTableName = TableName();
                var pretranslatedOriginal = valueToTranslate;
                if (ProjectData.OriginalsTableRowCoordinates[valueToTranslate].ContainsKey(currentTableName))
                {
                    if (ProjectData.OriginalsTableRowCoordinates[valueToTranslate][currentTableName].Contains(RowNumber))
                    {
                        valueToTranslate = ProjectData.FilesContent.Tables[currentTableName].Rows[RowNumber][1] + "";
                        valueToTranslate = FixInvalidSymbols(valueToTranslate);

                        isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                        if (isTranslated)
                        {
                            RET = true;
                            SetTranslationIsTranslatedAction();
                        }

                        //return ret;
                    }
                    else // set 1st value from avalaible values
                    {
                        ProjectData.AppLog.LogToFile("Warning! Row not found. row number=" + RowNumber + ". table name=" + TableName() + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                        foreach (var rowIndex in ProjectData.OriginalsTableRowCoordinates[valueToTranslate][currentTableName])
                        {
                            valueToTranslate = ProjectData.FilesContent.Tables[currentTableName].Rows[rowIndex][1] + "";
                            valueToTranslate = FixInvalidSymbols(valueToTranslate);

                            isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                            if (isTranslated)
                            {
                                RET = true;
                                SetTranslationIsTranslatedAction();
                            }

                            //return ret;
                        }
                    }
                }
                else // set 1st value from avalaible values
                {
                    ProjectData.AppLog.LogToFile("Warning! Table not found. row number=" + RowNumber + ". table name=" + TableName() + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                    foreach (var existTableName in ProjectData.OriginalsTableRowCoordinates[valueToTranslate].Values)
                    {
                        foreach (var existsRowIndex in existTableName)
                        {
                            valueToTranslate = ProjectData.FilesContent.Tables[currentTableName].Rows[existsRowIndex][1] + "";
                            valueToTranslate = FixInvalidSymbols(valueToTranslate);

                            isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                            if (isTranslated)
                            {
                                RET = true;
                                SetTranslationIsTranslatedAction();
                                break; // translated, dont need to iterate rows anymore
                            }

                            //return ret;
                        }

                        if (isTranslated)
                        {
                            RET = true;
                            SetTranslationIsTranslatedAction();
                            break; // translated, dont need to iterate table names anymore
                        }
                    }
                }

                RowNumber++;
            }
            else if (!letDuplicates && ProjectData.CurrentProject.TablesLinesDict.ContainsKey(valueToTranslate))
            {
                var control = valueToTranslate;
                valueToTranslate = ProjectData.CurrentProject.TablesLinesDict[valueToTranslate];
                valueToTranslate = FixInvalidSymbols(valueToTranslate);

                isTranslated = control != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                if (isTranslated)
                {
                    RET = true;
                    SetTranslationIsTranslatedAction();
                }

                //return ret;
            }

            return isTranslated;
        }

        /// <summary>
        /// action when string was translated
        /// </summary>
        protected virtual void SetTranslationIsTranslatedAction() { }

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
