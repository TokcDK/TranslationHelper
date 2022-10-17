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
    public abstract class FormatBase : IFormat
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
            if (AppData.CurrentProject == null) return;

            //AppData.CurrentProject.CurrentFormat = this; // set format , wrong for threaded open of files

            // set filepath from projectdata filepath
            if (!_filePathIsSet && string.IsNullOrWhiteSpace(FilePath))
            {
                //if (!string.IsNullOrWhiteSpace(ProjectData.FilePath))// wrong for threaded files open
                //{
                //    FilePath = ProjectData.FilePath;
                //}
                //else
                if (!string.IsNullOrWhiteSpace(AppData.SelectedFilePath)) FilePath = AppData.SelectedFilePath;
            }

            // DontLoadDuplicates options
            if (!AppData.CurrentProject.DontLoadDuplicates) return;

            if (SaveFileMode)
            {
                if (AppData.CurrentProject.TablesLinesDict == null) AppData.CurrentProject.TablesLinesDict = new ConcurrentDictionary<string, string>();
            }
            else if (AppData.CurrentProject.Hashes == null)
            {
                AppData.CurrentProject.Hashes = new ConcurrentSet<string>();
            }
        }

        /// <summary>
        /// current file path for open
        /// </summary>
        internal string FilePath;
        protected virtual string GetFilePath()
        {
            return OpenFileMode ? GetOpenFilePath() : GetSaveFilePath();
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
        public virtual bool Check()
        {
            return false;
        }

        /// <summary>
        /// extension which can be parsed with the format, ".txt" or ".txt,.csv" for example.
        /// override ExtIdentifier() to determine when a file with the extension can be opened
        /// </summary>
        /// <returns></returns>
        public virtual string Ext => null;

        /// <summary>
        /// identifier to check how to identify if selected extension must be parsed with this format.
        /// in result can be added new project which will be used Ext and this identifier to open valid standalone files.
        /// </summary>
        /// <returns></returns>
        internal virtual int ExtIdentifier => 0; // 0 means not use identifier

        /// <summary>
        /// name of format
        /// </summary>
        /// <returns></returns>
        public virtual string Name => string.Empty;

        public bool OpenFileMode { get; set; } = true;
        public bool SaveFileMode { get => !OpenFileMode; set => OpenFileMode = !value; }

        /// <summary>
        /// Open file strings actions executing here
        /// </summary>
        /// <returns></returns>
        public bool Open() { OpenFileMode = true; return TryOpen(); }

        /// <summary>
        /// Save file strings actions executing here
        /// </summary>
        /// <returns></returns>
        public bool Save() { OpenFileMode = false; return TrySave(); }

        /// <summary>
        /// Open file strings actions executing here
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryOpen() => ParseFile();

        /// <summary>
        /// Save file strings actions executing here
        /// </summary>
        /// <returns></returns>
        protected virtual bool TrySave() => ParseFile();

        /// <summary>
        /// Means use for table name name of file without extension
        /// </summary>
        internal virtual bool UseTableNameWithoutExtension => false;

        /// <summary>
        /// file name
        /// </summary>
        internal virtual string FileName
        {
            get
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
                if (AppData.CurrentProject.SubpathInTableName)
                {
                    var dirPath = Path.GetDirectoryName(filePath);
                    tableName = dirPath.Replace(AppData.CurrentProject.OpenedFilesDir, string.Empty) + Path.DirectorySeparatorChar;
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
        }

        /// <summary>
        /// Add table to work dataset
        /// </summary>
        protected void AddTables()
        {
            if (!string.IsNullOrEmpty(GetOpenFilePath())) AddTables(FileName);
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
                Data.Columns.Add(THSettings.OriginalColumnName);
                Data.Columns.Add(THSettings.TranslationColumnName);

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
            //    table.Columns.Add(THSettings.OriginalColumnName);
            //    table.Columns.Add(THSettings.TranslationColumnName);

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
            if (OpenFileMode)
            {
                return AddRowData(FileName, RowData, RowInfo, CheckInput);
            }
            else
            {
                if (CheckInput && !IsValidString(RowData)) return false;

                return SetTranslation(ref RowData, existsTranslation);
            }
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(string rowData, string rowInfo = "", bool isCheckInput = true)
        {
            return AddRowData(FileName, rowData, rowInfo, isCheckInput);
        }
        /// <summary>
        /// Add string to table with options. In save mode will replace <paramref name="rowData"/>[0] as translation and will use <paramref name="rowData"/>[1] as default translation 
        /// </summary>
        /// <param name="rowData">First value is Original, second value is translation.</param>
        /// <param name="rowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(ref string[] rowData, string rowInfo = "", bool isCheckInput = true)
        {
            if (OpenFileMode)
            {
                return AddRowData(FileName, rowData, rowInfo, isCheckInput);
            }
            else
            {
                if (isCheckInput && !IsValidString(rowData[0])) return false;

                return SetTranslation(ref rowData[0], rowData[1], isCheckInput);
            }
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <returns></returns>
        internal bool AddRowData(string[] rowData, string rowInfo = "", bool isCheckInput = true)
        {
            return AddRowData(FileName, rowData, rowInfo, isCheckInput);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <param name="isCheckInput">cheack original string if valid</param>
        /// <returns></returns>
        internal bool AddRowData(string tablename, string rowData, string rowInfo = "", bool isCheckInput = true)
        {
            return AddRowData(tablename, new string[] { rowData }, rowInfo, isCheckInput);
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <param name="isCheckInput">cheack original string if valid</param>
        /// <returns></returns>
        internal bool AddRowData(string tablename, string[] rowData, string rowInfo, bool isCheckInput = true)
        {
            var original = AddRowDataPreAddOriginalStringMod(rowData[0]);

            if (isCheckInput && !IsValidString(original)) return false;

            if (AppData.CurrentProject.DontLoadDuplicates)
            {
                if (AppData.CurrentProject.Hashes == null 
                    || AppData.CurrentProject.Hashes.Contains(original)) return false;

                // add to hashes when only unique values
                AppData.CurrentProject.Hashes.TryAdd(original);
            }
            else
            {
                // variant with duplicates

                // check if original exists
                if (!AppData.CurrentProject.OriginalsTableRowCoordinates.ContainsKey(original))
                {
                    AppData.CurrentProject.OriginalsTableRowCoordinates.TryAdd(original, new ConcurrentDictionary<string, ConcurrentSet<int>>());
                }

                // check if tablename is exists
                if (!AppData.CurrentProject.OriginalsTableRowCoordinates[original].ContainsKey(tablename))
                {
                    AppData.CurrentProject.OriginalsTableRowCoordinates[original].TryAdd(tablename, new ConcurrentSet<int>());
                }

                // check if current row number is exists
                if (!AppData.CurrentProject.OriginalsTableRowCoordinates[original][tablename].Contains(RowNumber))
                {
                    AppData.CurrentProject.OriginalsTableRowCoordinates[original][tablename].TryAdd(RowNumber);
                }

                // raise row number
                RowNumber++;
            }

            try
            {
                Data.Rows.Add(rowData);
                Info.Rows.Add(rowInfo);
            }
            catch { }

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
                AppData.CurrentProject.AddFileData(Data);
                AppData.CurrentProject.AddFileInfo(Info);

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
            if (!string.IsNullOrWhiteSpace(Ext) && Path.GetExtension(GetOpenFilePath()) != Ext) return false; // extension must be same as set, if set

            if (SaveFileMode && !AppData.THFilesList.Items.Contains(FileName)) return false;

            if (OpenFileMode) AddTables();

            if (SaveFileMode) SplitTableCellValuesAndTheirLinesToDictionary(FileName, false, false);

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
            if (OpenFileMode)
            {
                return CheckTablesContent(FileName);
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
            inputString = AppData.CurrentProject.CleanStringForCheck(inputString);

            return !string.IsNullOrWhiteSpace(inputString) && !inputString.ForJPLangHaveMostOfRomajiOtherChars();
        }
        internal void SplitTableCellValuesToDictionaryLines(string TableName)
        {
            if (!AppData.CurrentProject.DontLoadDuplicates || !SaveFileMode || !AppData.CurrentProject.FilesContent.Tables.Contains(TableName))
                return;

            if (AppData.CurrentProject.TablesLinesDict == null)
            {
                AppData.CurrentProject.TablesLinesDict = new ConcurrentDictionary<string, string>();
            }

            if (AppData.CurrentProject.TablesLinesDict.Count > 0)
            {
                AppData.CurrentProject.TablesLinesDict.Clear();
            }

            foreach (DataRow Row in AppData.CurrentProject.FilesContent.Tables[TableName].Rows)
            {
                string Original;
                string Translation;
                if (AppData.CurrentProject.TablesLinesDict.ContainsKey(Original = Row[0] + string.Empty) || (Translation = Row[1] + string.Empty).Length == 0 || Translation == Original)
                continue;

                AppData.CurrentProject.TablesLinesDict.TryAdd(Original, Translation);
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
            if (!AppData.CurrentProject.DontLoadDuplicates) // skip if do not load duplicates option is disabled
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
            if (AppData.CurrentProject.TablesLinesDict == null)
            {
                AppData.CurrentProject.TablesLinesDict = new ConcurrentDictionary<string, string>();
            }

            if (onlyOneTable)
            {
                if (!AppData.CurrentProject.FilesContent.Tables.Contains(tableName))
                    return;

                if (AppData.CurrentProject.TablesLinesDict.Count > 0)
                {
                    AppData.CurrentProject.TablesLinesDict.Clear();
                }
            }
            else
            {
                if (TablesLinesDictFilled /*|| ProjectData.CurrentProject.TablesLinesDict != null && ProjectData.CurrentProject.TablesLinesDict.Count > 0*/)
                {
                    return;
                }
            }


            foreach (DataTable Table in AppData.CurrentProject.FilesContent.Tables)
            {
                if (onlyOneTable && Table.TableName != tableName)
                continue;

                foreach (DataRow Row in Table.Rows)
                {
                    string Original = (Row[0] + string.Empty);
                    int OriginalLinesCount = Original.GetLinesCount();
                    if (OriginalLinesCount == 1 && AppData.CurrentProject.TablesLinesDict.ContainsKey(Original))
                    continue;

                    string Translation = (Row[1] + string.Empty);
                    if (Translation.Length == 0)
                    continue;

                    int TranslationLinesCount = Translation.GetLinesCount();
                    bool LinesCountisEqual = OriginalLinesCount == TranslationLinesCount;
                    if (!LinesCountisEqual && makeLinesCountEqual)
                    {
                        if (OriginalLinesCount > Translation.Length)
                        continue;

                        Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                    }

                    //Сначала добавить полный вариант
                    if (!AppData.CurrentProject.TablesLinesDict.ContainsKey(Original) /*&& ((!ProjectData.CurrentProject.ProjectData.CurrentProject.TablesLinesDictAddEqual && Translation != Original) || ProjectData.CurrentProject.ProjectData.CurrentProject.TablesLinesDictAddEqual)*/)
                    {
                        try
                        {
                            lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
                            {
                                AppData.CurrentProject.TablesLinesDict.TryAdd(Original, Translation/*.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit)*/);
                            }
                        }
                        catch (ArgumentException) { }

                        if (OriginalLinesCount == 1)
                        continue;
                    }
                    else if (Translation != Original && Original == AppData.CurrentProject.TablesLinesDict[Original])
                    {
                        AppData.CurrentProject.TablesLinesDict[Original] = Translation;
                        if (OriginalLinesCount == 1)
                        continue;
                    }

                    if (!makeLinesCountEqual && OriginalLinesCount > TranslationLinesCount)
                    {
                        if (OriginalLinesCount > Translation.Length)
                        continue;

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
                            if (!AppData.CurrentProject.TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                            {
                                try
                                {
                                    lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
                                    {
                                        AppData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit)*/);
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
                                            AppData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit)*/);
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
                                                    AppData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[OriginalLinesCount - 1], result/*.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit)*/);
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
                                                    AppData.CurrentProject.TablesLinesDict.TryAdd(OriginalLines[OriginalLinesCount - 1], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit)*/);
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

        string IFormat.Name => throw new NotImplementedException();

        string IFormat.Ext => throw new NotImplementedException();

        /// <summary>
        /// check if translation is exists and set str return true if found.
        /// <paramref name="valueToTranslate"/> = input string, must contain original value for search.
        /// <paramref name="existsTranslation"/> = control translation value, if was loaded translation from file
        /// </summary>
        /// <param name="valueToTranslate">input=original, output=translation</param>
        /// <param name="existsTranslation">control translation value if was loaded translation from file</param>
        /// <returns>true if translation was set and not equal to input original</returns>
        internal bool SetTranslation(ref string valueToTranslate, string existsTranslation = null, bool isCheckInput = true)
        {
            if (OpenFileMode) return false;
            if (isCheckInput && !IsValidString(valueToTranslate)) return false;

            var isTranslated = false;
            bool letDuplicates = !AppData.CurrentProject.DontLoadDuplicates;
            if (letDuplicates
                && AppData.CurrentProject.OriginalsTableRowCoordinates != null
                && AppData.CurrentProject.OriginalsTableRowCoordinates.ContainsKey(valueToTranslate) // input value has original's value before it will be changed to translation
                )
            {
                var currentTableName = FileName;
                var pretranslatedOriginal = valueToTranslate;
                if (AppData.CurrentProject.OriginalsTableRowCoordinates[valueToTranslate].ContainsKey(currentTableName))
                {
                    if (AppData.CurrentProject.OriginalsTableRowCoordinates[valueToTranslate][currentTableName].Contains(RowNumber))
                    {
                        isTranslated = SetIfTranslated(currentTableName, RowNumber, pretranslatedOriginal, existsTranslation, ref valueToTranslate);

                        //return ret;
                    }
                    else // set 1st value from avalaible values
                    {
                        AppData.AppLog.LogToFile("Warning! Row not found. row number=" + RowNumber + ". table name=" + FileName + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                        foreach (var rowIndex in AppData.CurrentProject.OriginalsTableRowCoordinates[valueToTranslate][currentTableName])
                        {
                            isTranslated = SetIfTranslated(currentTableName, rowIndex, pretranslatedOriginal, existsTranslation, ref valueToTranslate);

                            if (isTranslated) break;

                            //return ret;
                        }
                    }
                }
                else // set 1st value from avalaible values
                {
                    AppData.AppLog.LogToFile("Warning! Table not found. row number=" + RowNumber + ". table name=" + FileName + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                    foreach (var existTableName in AppData.CurrentProject.OriginalsTableRowCoordinates[valueToTranslate].Values)
                    {
                        foreach (var existsRowIndex in existTableName)
                        {
                            isTranslated = SetIfTranslated(currentTableName, existsRowIndex, pretranslatedOriginal, existsTranslation, ref valueToTranslate);

                            if (isTranslated) break; // translated, dont need to iterate rows anymore
                        }

                        if (isTranslated) break; // translated, dont need to iterate table names anymore
                    }
                }

                RowNumber++;
            }
            else if (!letDuplicates && AppData.CurrentProject.TablesLinesDict.ContainsKey(valueToTranslate))
            {
                isTranslated = SetIfTranslated(null, -1, valueToTranslate, existsTranslation, ref valueToTranslate);
            }

            return isTranslated;
        }

        private bool SetIfTranslated(string currentTableName, int RowNumber, string pretranslatedOriginal, string existsTranslation, ref string valueToTranslate)
        {
            valueToTranslate = currentTableName == null ? AppData.CurrentProject.TablesLinesDict[valueToTranslate] : AppData.CurrentProject.FilesContent.Tables[currentTableName].Rows[RowNumber][1] + "";
            valueToTranslate = FixInvalidSymbols(valueToTranslate);

            if (!string.IsNullOrEmpty(valueToTranslate) && (pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate)))
            {
                RET = true;
                SetTranslationIsTranslatedAction();
                return true;
            }

            return false;
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
