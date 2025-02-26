﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Menus.MainMenus.File;
using WolfTrans.Net.Parsers.Events.Map.Event;

namespace TranslationHelper.Formats
{
    public abstract class FormatBase : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class with no file path.
        /// </summary>
        protected FormatBase()
        {
            BaseInit(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class with a specified file path.
        /// </summary>
        /// <param name="filePath">The file path to set, or null if not provided.</param>
        protected FormatBase(string filePath)
        {
            BaseInit(filePath);
        }

        /// <summary>
        /// Current file path for opening or saving operations.
        /// </summary>
        internal string FilePath { get; set; }

        /// <summary>
        /// Indicates whether the format is in open mode.
        /// </summary>
        public bool OpenFileMode { get; set; } = true;

        /// <summary>
        /// Indicates whether the format is in save mode; inversely tied to <see cref="OpenFileMode"/>.
        /// </summary>
        public bool SaveFileMode { get => !OpenFileMode; set => OpenFileMode = !value; }

        /// <summary>
        /// Main table containing row data.
        /// </summary>
        internal DataTable Data { get; set; }

        /// <summary>
        /// Table containing info for the info box.
        /// </summary>
        internal DataTable Info { get; set; }

        /// <summary>
        /// Current row number in the parsing table.
        /// </summary>
        protected int RowIndex { get; set; } = 0;

        /// <summary>
        /// Indicates whether a translation was set.
        /// </summary>
        public bool RET { get; internal set; }

        /// <summary>
        /// Enumeration defining actions after encountering a keyword during parsing.
        /// </summary>
        protected enum KeywordActionAfter
        {
            Break = -1,
            Continue = 0,
            ReadToEnd = 1
        }

        /// <summary>
        /// Initializes base properties and settings for the format.
        /// </summary>
        /// <param name="filePath">The file path to set, or null if not provided.</param>
        private void BaseInit(string filePath)
        {
            if (AppData.CurrentProject == null) return;

            FilePath = string.IsNullOrWhiteSpace(filePath)
                ? AppData.SelectedProjectFilePath
                : filePath;

            if (!AppData.CurrentProject.DontLoadDuplicates) return;

            if (SaveFileMode)
            {
                AppData.CurrentProject.TablesLinesDict = AppData.CurrentProject.TablesLinesDict ?? new ConcurrentDictionary<string, string>();
            }
            else
            {
                AppData.CurrentProject.Hashes = AppData.CurrentProject.Hashes ?? new ConcurrentSet<string>();
            }
        }

        /// <summary>
        /// Gets the file path based on the current mode.
        /// </summary>
        /// <returns>The file path for the current operation.</returns>
        protected virtual string GetFilePath()
        {
            return OpenFileMode ? GetOpenFilePath() : GetSaveFilePath();
        }

        /// <summary>
        /// Gets the file path for opening operations.
        /// </summary>
        /// <returns>The file path for opening.</returns>
        protected virtual string GetOpenFilePath()
        {
            return FilePath;
        }

        /// <summary>
        /// Gets the file path for saving operations.
        /// </summary>
        /// <returns>The file path for saving.</returns>
        protected virtual string GetSaveFilePath()
        {
            return FilePath;
        }

        /// <summary>
        /// Checks if the format can be parsed.
        /// </summary>
        /// <returns>True if the format can be parsed; otherwise, false.</returns>
        public virtual bool Check() => false;

        /// <summary>
        /// Gets the file extension(s) that can be parsed with this format (e.g., ".txt" or ".txt,.csv").
        /// </summary>
        public abstract string Extension { get; }

        /// <summary>
        /// Gets a short description of the format.
        /// </summary>
        public virtual string Description => string.Empty;

        /// <summary>
        /// Gets a value indicating whether to use the file name without its extension for the table name.
        /// </summary>
        internal virtual bool UseTableNameWithoutExtension => false;

        /// <summary>
        /// Gets the table name derived from the file path.
        /// </summary>
        internal virtual string FileName
        {
            get
            {
                string filePath = GetFilePath();
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    // Warning: This loop may indicate a threading issue where FilePath is set asynchronously.
                    int attempts = 10;
                    while (string.IsNullOrWhiteSpace(filePath) && attempts-- > 0)
                    {
                        filePath = GetFilePath();
                    }
                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        return string.Empty; // Consider throwing an exception if critical.
                    }
                }

                string tableName = AppData.CurrentProject.SubpathInTableName
                    ? Path.GetDirectoryName(filePath).Replace(AppData.CurrentProject.OpenedFilesDir, string.Empty) + Path.DirectorySeparatorChar
                    : string.Empty;

                tableName += UseTableNameWithoutExtension
                    ? Path.GetFileNameWithoutExtension(filePath)
                    : Path.GetFileName(filePath);

                return tableName;
            }
        }

        /// <summary>
        /// Opens the file in open mode.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool Open()
        {
            OpenFileMode = true;
            return TryOpen();
        }

        /// <summary>
        /// Saves the file in save mode if there are translated entries.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool Save()
        {
            OpenFileMode = false;
            // Note: Assuming FilesContent.Tables[FileName].HasAnyTranslated() exists elsewhere.
            return IsAnyTranslated() && TrySave();
        }

        private bool IsAnyTranslated()
        {
            if (!AppData.CurrentProject.FilesContent.Tables.Contains(FileName))
            {
                return false;
            }
            return AppData.CurrentProject.FilesContent.Tables[FileName].Rows.Cast<DataRow>()
                .Any(row => !string.IsNullOrEmpty(GetRowTranslationText(row)));
        }

        /// <summary>
        /// Attempts to open the file by parsing it.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool TryOpen() => ParseFile();

        /// <summary>
        /// Attempts to save the file by parsing it.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool TrySave() => ParseFile();

        /// <summary>
        /// Initializes table content for the current file path.
        /// </summary>
        protected void InitTableContent()
        {
            if (string.IsNullOrEmpty(GetFilePath())) return;
            InitTableContent(FileName);
        }

        /// <summary>
        /// Initializes table content with a specified table name and optional extra columns.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="extraColumns">Additional columns to add to the data table.</param>
        internal void InitTableContent(string tableName, string[] extraColumns = null)
        {
            if (Data != null) return;

            Data = new DataTable { TableName = tableName };
            Data.Columns.Add(THSettings.OriginalColumnName);
            Data.Columns.Add(THSettings.TranslationColumnName);

            if (extraColumns?.Length > 0)
            {
                foreach (var columnName in extraColumns)
                {
                    Data.Columns.Add(columnName);
                }
            }

            Info = new DataTable { TableName = tableName };
            Info.Columns.Add("Info");
        }

        /// <summary>
        /// Adds a string to the table with options. In save mode, replaces <paramref name="rowData"/> with translation.
        /// </summary>
        /// <param name="rowData">Reference to the original string.</param>
        /// <param name="rowInfo">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <param name="existsTranslation">Pre-existing translation, if any.</param>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        internal bool AddRowData(ref string rowData, string rowInfo = "", bool isCheckInput = true, string existsTranslation = null)
        {
            return OpenFileMode
                ? AddRowData(FileName, rowData, rowInfo, isCheckInput)
                : SetTranslation(ref rowData, existsTranslation, isCheckInput);
        }

        /// <summary>
        /// Adds a string to the table with options.
        /// </summary>
        /// <param name="rowData">Original string.</param>
        /// <param name="rowInfo">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string rowData, string rowInfo = "", bool isCheckInput = true)
        {
            return AddRowData(FileName, rowData, rowInfo, isCheckInput);
        }

        /// <summary>
        /// Adds a string to the table with options. In save mode, replaces <paramref name="rowData"/>[0] with translation using <paramref name="rowData"/>[1] as default.
        /// </summary>
        /// <param name="rowData">Array where first value is original, second is translation.</param>
        /// <param name="rowInfo">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added or translation set; otherwise, false.</returns>
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
        /// Adds a string array to the table with options.
        /// </summary>
        /// <param name="rowData">Original string array.</param>
        /// <param name="rowInfo">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string[] rowData, string rowInfo = "", bool isCheckInput = true)
        {
            return AddRowData(FileName, rowData, rowInfo, isCheckInput);
        }

        /// <summary>
        /// Adds a string to the specified table with options.
        /// </summary>
        /// <param name="tablename">File/table name.</param>
        /// <param name="rowData">Original string.</param>
        /// <param name="rowInfo">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string tablename, string rowData, string rowInfo = "", bool isCheckInput = true)
        {
            return AddRowData(tablename, new[] { rowData }, rowInfo, isCheckInput);
        }

        /// <summary>
        /// Adds a string array to the specified table with options.
        /// </summary>
        /// <param name="tablename">File/table name.</param>
        /// <param name="rowData">Original string array.</param>
        /// <param name="rowInfo">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string tablename, string[] rowData, string rowInfo, bool isCheckInput = true)
        {
            if (rowData?.Length == 0 || rowData[0] == null) return false;

            string original = AddRowDataPreAddOriginalStringMod(rowData[0]);
            if (isCheckInput && !IsValidString(original)) return false;

            if (AppData.CurrentProject.DontLoadDuplicates)
            {
                if (AppData.CurrentProject.Hashes?.Contains(original) ?? false) return false;
                AppData.CurrentProject.Hashes?.TryAdd(original);
            }
            else
            {
                var originals = AppData.CurrentProject.OriginalsTableRowCoordinates;
                originals.TryAdd(original, new ConcurrentDictionary<string, ConcurrentSet<int>>());
                originals[original].TryAdd(tablename, new ConcurrentSet<int>());
                
                if (originals[original][tablename].TryAdd(RowIndex)) RowIndex++;
            }

            try
            {
                Data.Rows.Add(rowData);
                Info.Rows.Add(rowInfo?.Length > 500 ? rowInfo.Remove(500) : rowInfo);
                return true;
            }
            catch (Exception ex)
            {
                AppData.AppLog.LogToFile($"Error adding row: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Modifies the original string before adding it to the table.
        /// </summary>
        /// <param name="original">The original string.</param>
        /// <returns>The modified string.</returns>
        protected virtual string AddRowDataPreAddOriginalStringMod(string original) => original;

        /// <summary>
        /// Performs pre-open file actions.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool FilePreOpenActions()
        {
            if (!string.IsNullOrWhiteSpace(Extension) && Path.GetExtension(GetFilePath()) != Extension) return false;
            if (SaveFileMode && !AppData.THFilesList.Items.Contains(FileName)) return false;
            if (OpenFileMode) InitTableContent();
            if (SaveFileMode) SplitTableCellValuesAndTheirLinesToDictionary(FileName, false, false);
            PreOpenExtraActions();
            return true;
        }

        /// <summary>
        /// Performs additional pre-open actions.
        /// </summary>
        protected virtual void PreOpenExtraActions() { }

        /// <summary>
        /// Parses the file based on the current mode.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected bool ParseFile()
        {
            if (!FilePreOpenActions()) return false;
            FileOpen();
            return FilePostOpen();
        }

        /// <summary>
        /// Performs file opening actions.
        /// </summary>
        protected virtual void FileOpen() { }

        /// <summary>
        /// Performs post-open actions.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool FilePostOpen()
        {
            return OpenFileMode ? CheckTablesContent(FileName) : WriteFileData();
        }

        /// <summary>
        /// Checks and processes table content after opening.
        /// </summary>
        /// <param name="tableName">The table name to check.</param>
        /// <param name="isDictionary">Whether to treat as a dictionary (not implemented).</param>
        /// <returns>True if content is valid; otherwise, false.</returns>
        protected bool CheckTablesContent(string tableName, bool isDictionary = false)
        {
            if (isDictionary)
            {
                throw new NotImplementedException("Dictionary processing not implemented.");
            }
            if (Data.Rows.Count > 0)
            {
                AppData.CurrentProject.AddFileData(Data);
                AppData.CurrentProject.AddFileInfo(Info);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes file data after validation.
        /// </summary>
        /// <param name="filePath">The file path to write to.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool WriteFileData(string filePath = "") => DoWriteFile(filePath);

        /// <summary>
        /// Performs the actual file writing.
        /// </summary>
        /// <param name="filePath">The file path to write to.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool DoWriteFile(string filePath = "") => false;

        /// <summary>
        /// Validates if a string is suitable for adding to the table.
        /// </summary>
        /// <param name="inputString">The string to validate.</param>
        /// <returns>True if valid; otherwise, false.</returns>
        internal virtual bool IsValidString(string inputString)
        {
            inputString = AppData.CurrentProject.CleanStringForCheck(inputString);
            return !string.IsNullOrWhiteSpace(inputString) && !inputString.ForJPLangHaveMostOfRomajiOtherChars();
        }

        /// <summary>
        /// Sets the translation for a string if available, updating the original string in place.
        /// </summary>
        /// <param name="valueToTranslate">The original string; replaced with translation if found.</param>
        /// <param name="existsTranslation">An optional pre-existing translation to compare against.</param>
        /// <param name="isCheckInput">Whether to validate the input string.</param>
        /// <returns>True if a translation was set and differs from the original or existing translation; otherwise, false.</returns>
        internal bool SetTranslation(ref string valueToTranslate, string existsTranslation = null, bool isCheckInput = true)
        {

            if (OpenFileMode || (isCheckInput && !IsValidString(valueToTranslate))) return false;

            bool letDuplicates = !AppData.CurrentProject.DontLoadDuplicates;
            string original = valueToTranslate;

            if (letDuplicates && AppData.CurrentProject.OriginalsTableRowCoordinates?.ContainsKey(original) == true)
            {
                // any valid (even empty or equal original) added row in the table will be parsed then increment the row index
                int rowIndex = RowIndex++;

                var coordinates = AppData.CurrentProject.OriginalsTableRowCoordinates[original];

                // standart get the translation by row index
                string tableName = FileName;
                if (coordinates.TryGetValue(tableName, out var rows) && rows.Contains(rowIndex))
                {
                    if(ApplyTranslation(tableName, rowIndex, original, existsTranslation, ref valueToTranslate))
                    {
                        return true;
                    }
                }

                // when row not found in current table, set first translation from available translations
                // AppData.AppLog.LogToFile($"Warning! Row not found. Row: {RowIndex}, Table: {tableName}, Original:\r\n{original}\r\nExisting Translation:\r\n{existsTranslation}");
                // iterate the table coordinates and try get first valid translation
                foreach (var tableRowsIndexes in coordinates)
                {
                    tableName = tableRowsIndexes.Key;
                    var rowIndexes = tableRowsIndexes.Value;

                    foreach (var rIndex in rowIndexes)
                    {
                        if (ApplyTranslation(tableName, rIndex, original, existsTranslation, ref valueToTranslate))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (!letDuplicates && AppData.CurrentProject.TablesLinesDict?.ContainsKey(original) == true)
            {
                return ApplyTranslation(null, -1, original, existsTranslation, ref valueToTranslate);
            }
            return false;
        }

        /// <summary>
        /// Applies a translation to the provided string if available.
        /// </summary>
        /// <param name="tableName">The table name, or null if using dictionary lookup.</param>
        /// <param name="rowNumber">The row number, or -1 if using dictionary lookup.</param>
        /// <param name="original">The original string.</param>
        /// <param name="existsTranslation">An optional pre-existing translation.</param>
        /// <param name="valueToTranslate">The string to update with the translation.</param>
        /// <returns>True if a valid translation was applied; otherwise, false.</returns>
        private bool ApplyTranslation(string tableName, int rowNumber, string original, string existsTranslation, ref string valueToTranslate)
        {
            try
            {
                valueToTranslate = tableName == null
                    ? AppData.CurrentProject.TablesLinesDict[original]
                    :  GetRowTranslationText(AppData.CurrentProject.FilesContent.Tables[tableName].Rows[rowNumber]);
            }
            catch (Exception ex)
            {
                AppData.AppLog.LogToFile($"Error applying translation. Table: {tableName}, Row: {rowNumber}, Original: {original}, Error: {ex.Message}");
                return false;
            }

            valueToTranslate = FixInvalidSymbols(valueToTranslate);
            bool isEqualOriginal = original == valueToTranslate;
            if (!string.IsNullOrEmpty(valueToTranslate) &&
                (!isEqualOriginal || (existsTranslation != null && existsTranslation != valueToTranslate)))
            {
                RET = true;
                SetTranslationIsTranslatedAction();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs an action when a translation is set.
        /// </summary>
        protected virtual void SetTranslationIsTranslatedAction() { }

        /// <summary>
        /// Removes or replaces invalid symbols in a translation string.
        /// </summary>
        /// <param name="str">The string to fix.</param>
        /// <returns>The cleaned string.</returns>
        protected virtual string FixInvalidSymbols(string str) => str.Replace("\u200b", string.Empty);


        #region extra code
        /// <summary>
        /// Splits table cell values into a dictionary for translation lookups.
        /// </summary>
        /// <param name="tableName">The table to process.</param>
        /// <param name="makeLinesCountEqual">Whether to equalize line counts.</param>
        /// <param name="onlyOneTable">Whether to process only the specified table.</param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string tableName, bool makeLinesCountEqual = false, bool onlyOneTable = false)
        {
            if (!AppData.CurrentProject.DontLoadDuplicates || !SaveFileMode) return;

            var dict = AppData.CurrentProject.TablesLinesDict ?? new ConcurrentDictionary<string, string>();
            if (onlyOneTable && !AppData.CurrentProject.FilesContent.Tables.Contains(tableName)) return;

            lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
            {
                if (TablesLinesDictFilled && !onlyOneTable) return;
                if (onlyOneTable) dict.Clear();

                foreach (DataTable table in AppData.CurrentProject.FilesContent.Tables)
                {
                    if (onlyOneTable && table.TableName != tableName) continue;

                    foreach (DataRow row in table.Rows)
                    {
                        string original = GetRowOriginalText(row);
                        string translation = GetRowTranslationText(row);
                        if (string.IsNullOrEmpty(translation) || dict.ContainsKey(original)) continue;

                        int originalLines = original.GetLinesCount();
                        int translationLines = translation.GetLinesCount();
                        bool linesEqual = originalLines == translationLines;

                        if (!linesEqual && makeLinesCountEqual && originalLines <= translation.Length)
                        {
                            translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(translation, originalLines));
                        }

                        if (!dict.TryAdd(original, translation) && translation != original && original == dict[original])
                        {
                            dict[original] = translation;
                        }

                        if (originalLines > 1)
                        {
                            AddMultiLineTranslations(original, translation, makeLinesCountEqual);
                        }
                    }
                    if (onlyOneTable) break;
                }
                TablesLinesDictFilled = true;
            }
        }

        private readonly object SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock = new object();
        private bool TablesLinesDictFilled = false;
        //private bool FirstPassOfDictionaryFilling = true;

        /// <summary>
        /// Adds multi-line translations to the dictionary.
        /// </summary>
        /// <param name="original">The original multi-line string.</param>
        /// <param name="translation">The translation string.</param>
        /// <param name="makeLinesCountEqual">Whether to adjust line counts.</param>
        private static void AddMultiLineTranslations(string original, string translation, bool makeLinesCountEqual)
        {
            var dict = AppData.CurrentProject.TablesLinesDict;
            string[] originalLines = original.GetAllLinesToArray();
            string[] translationLines = translation.GetAllLinesToArray();
            int originalCount = originalLines.Length;
            int translationCount = translationLines.Length;

            if (!makeLinesCountEqual && originalCount > translationCount && originalCount <= translation.Length)
            {
                translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(translation, originalCount));
                translationLines = translation.GetAllLinesToArray();
                translationCount = translationLines.Length;
            }

            List<string> extraLines = new List<string>();
            for (int i = 0; i < translationCount; i++)
            {
                if (originalCount == translationCount)
                {
                    TryAddTranslation(dict, originalLines[i], translationLines[i]);
                }
                else if (i < originalCount - 1)
                {
                    TryAddTranslation(dict, originalLines[i], translationLines[i]);
                }
                else if (i == translationCount - 1)
                {
                    extraLines.Add(translationLines[i]);
                    string result = string.Join(Environment.NewLine, extraLines);
                    if (result.Trim().Length > 0)
                    {
                        TryAddTranslation(dict, originalLines[originalCount - 1], result);
                    }
                }
                else
                {
                    extraLines.Add(translationLines[i]);
                }
            }
        }

        /// <summary>
        /// Attempts to add a translation to the dictionary.
        /// </summary>
        /// <param name="dict">The translation dictionary.</param>
        /// <param name="original">The original string.</param>
        /// <param name="translation">The translation string.</param>
        private static void TryAddTranslation(ConcurrentDictionary<string, string> dict, string original, string translation)
        {
            if (translation.Length > 0 && original != translation && !dict.ContainsKey(original))
            {
                dict.TryAdd(original, translation);
            }
        }

        internal static string GetRowOriginalText(DataRow dataRow)
        {
            return dataRow.Field<string>(AppData.CurrentProject.OriginalColumnIndex);
        }
        internal static string GetRowTranslationText(DataRow dataRow)
        {
            return dataRow.Field<string>(AppData.CurrentProject.TranslationColumnIndex);
        }
        #endregion
    }
}
