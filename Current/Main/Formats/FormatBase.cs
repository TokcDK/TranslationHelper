using NLog;
using System;
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
using TranslationHelper.Projects;
using WolfTrans.Net.Parsers.Events.Map.Event;

namespace TranslationHelper.Formats
{
    public abstract class FormatBase : IFormat
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal ProjectBase ParentProject { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class with no file path.
        /// </summary>
        protected FormatBase(ProjectBase parentProject)
        {
            ParentProject = parentProject;


            if(ParentProject == null)
            {
                var ex = new ArgumentNullException(nameof(parentProject), "Parent project cannot be null.");

                Logger.Error(ex, "Parent project is null in FormatBase constructor.");

                throw ex;
            }

            BaseInit();
        }

        /// <summary>
        /// Current file path for opening or saving operations.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Indicates whether the format is in open mode.
        /// </summary>
        public bool OpenFileMode { get; private set; } = true;

        /// <summary>
        /// Indicates whether the format is in save mode; inversely tied to <see cref="OpenFileMode"/>.
        /// </summary>
        public bool SaveFileMode { get => !OpenFileMode; private set => OpenFileMode = !value; }

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

        protected void AddString(string[] values, string info = "")
        {
            Data.Rows.Add(values);
            Info.Rows.Add(info);
        }

        /// <summary>
        /// Initializes base properties and settings for the format.
        /// </summary>
        /// <param name="filePath">The file path to set, or null if not provided.</param>
        private void BaseInit()
        {
            SetupWhenDontLoadDuplicates();
        }

        private void SetupWhenDontLoadDuplicates()
        {
            if (!ParentProject.DontLoadDuplicates) return;

            if (SaveFileMode)
            {
                ParentProject.TablesLinesDict = ParentProject.TablesLinesDict ?? new ConcurrentDictionary<string, string>();
            }
            else
            {
                ParentProject.Hashes = ParentProject.Hashes ?? new ConcurrentSet<string>();
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

                string tableName = ParentProject.SubpathInTableName
                    ? Path.GetDirectoryName(filePath).Replace(ParentProject.OpenedFilesDir, string.Empty) + Path.DirectorySeparatorChar
                    : string.Empty;

                tableName += UseTableNameWithoutExtension
                    ? Path.GetFileNameWithoutExtension(filePath)
                    : Path.GetFileName(filePath);

                return tableName;
            }
        }

        protected bool IsValidFilePath(string filePath)
        {
            bool isValid = !string.IsNullOrWhiteSpace(filePath)
                && File.Exists(filePath);

            if(!isValid)
            {
                Logger.Debug($"{this.GetType().Name}: Invalid file path: {filePath}");
            }

            return isValid;
        }

        /// <summary>
        /// Opens the file in open mode.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool Open(string filePath)
        {
            if (!IsValidFilePath(filePath)) return false;

            FilePath = filePath;

            OpenFileMode = true;
            return TryOpen() && CheckTablesContent(FileName);
        }

        /// <summary>
        /// Saves the file in save mode if there are translated entries.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool Save(string filePath = null)
        {
            if (IsValidFilePath(filePath))
            {
                Logger.Debug($"{this.GetType().Name}: using input {nameof(filePath)}");
                FilePath = filePath;
            }
            else
            {
                Logger.Debug($"{this.GetType().Name}: {nameof(filePath)} is invalid, using internal {nameof(FilePath)}");
            }

            if (!IsValidFilePath(FilePath))
            {
                Logger.Debug($"{this.GetType().Name}: Invalid file path: {FilePath}. Exit.");
                return false;
            }

            OpenFileMode = false;
            // Note: Assuming FilesContent.Tables[FileName].HasAnyTranslated() exists elsewhere.
            return IsAnyTranslated() && TrySave();
        }

        private bool IsAnyTranslated()
        {
            if (!ParentProject.FilesContent.Tables.Contains(FileName))
            {
                return false;
            }
            return ParentProject.FilesContent.Tables[FileName].Rows.Cast<DataRow>()
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
        /// Adds a string to the table with options. In save mode, replaces <paramref name="value"/> with translation.
        /// </summary>
        /// <param name="value">Reference to the original string.</param>
        /// <param name="info">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <param name="existsTranslation">Pre-existing translation, if any.</param>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        internal bool AddRowData(ref string value, string info = "", bool isCheckInput = true, string existsTranslation = null)
        {
            return OpenFileMode
                ? AddRowData(FileName, value, info, isCheckInput)
                : SetTranslation(ref value, existsTranslation, isCheckInput);
        }

        /// <summary>
        /// Adds a string to the table with options.
        /// </summary>
        /// <param name="value">Original string.</param>
        /// <param name="info">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string value, string info = "", bool isCheckInput = true)
        {
            return AddRowData(FileName, value, info, isCheckInput);
        }

        /// <summary>
        /// Adds a string to the table with options. In save mode, replaces <paramref name="values"/>[0] with translation using <paramref name="values"/>[1] as default.
        /// </summary>
        /// <param name="values">Array where first value is original, second is translation.</param>
        /// <param name="info">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added or translation set; otherwise, false.</returns>
        internal bool AddRowData(ref string[] values, string info = "", bool isCheckInput = true)
        {
            if (OpenFileMode)
            {
                return AddRowData(FileName, values, info, isCheckInput);
            }
            else
            {
                if (isCheckInput && !IsValidString(values[0])) return false;
                return SetTranslation(ref values[0], values[1], isCheckInput);
            }
        }

        /// <summary>
        /// Adds a string array to the table with options.
        /// </summary>
        /// <param name="values">Original string array.</param>
        /// <param name="info">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string[] values, string info = "", bool isCheckInput = true)
        {
            return AddRowData(FileName, values, info, isCheckInput);
        }

        /// <summary>
        /// Adds a string to the specified table with options.
        /// </summary>
        /// <param name="tablename">File/table name.</param>
        /// <param name="value">Original string.</param>
        /// <param name="info">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string tablename, string value, string info = "", bool isCheckInput = true)
        {
            return AddRowData(tablename, new[] { value }, info, isCheckInput);
        }

        /// <summary>
        /// Adds a string array to the specified table with options.
        /// </summary>
        /// <param name="tablename">File/table name.</param>
        /// <param name="values">Original string array.</param>
        /// <param name="info">Info about the string.</param>
        /// <param name="isCheckInput">Whether to check the input string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string tablename, string[] values, string info, bool isCheckInput = true)
        {
            if (values?.Length == 0 || values[0] == null) return false;

            string original = AddRowDataPreAddOriginalStringMod(values[0]);
            if (isCheckInput && !IsValidString(original)) return false;

            if (ParentProject.DontLoadDuplicates)
            {
                if (ParentProject.Hashes?.Contains(original) ?? false) return false;
                ParentProject.Hashes?.TryAdd(original);
            }
            else
            {
                var originals = ParentProject.OriginalsTableRowCoordinates;
                originals.TryAdd(original, new ConcurrentDictionary<string, ConcurrentSet<int>>());
                originals[original].TryAdd(tablename, new ConcurrentSet<int>());
                
                if (originals[original][tablename].TryAdd(RowIndex)) RowIndex++;
            }

            try
            {
                AddString(values, info?.Length > 500 ? info.Remove(500) : info);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Error adding row: {ex.Message}");
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
            if (SaveFileMode && !ParentProject.FilesContent.Tables.Contains(FileName)) return false;
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
            return (!OpenFileMode && WriteFileData()) || OpenFileMode;
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
                ParentProject.AddTable(Data, Info);
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
            inputString = ParentProject.CleanStringForCheck(inputString);
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
            if (IsTranslationNotAllowed(valueToTranslate, isCheckInput))
                return false;

            string original = valueToTranslate;
            bool letDuplicates = !ParentProject.DontLoadDuplicates;

            if (letDuplicates)
            {
                return TryApplyTranslationWithDuplicatesAllowed(ref valueToTranslate, original, existsTranslation);
            }
            else
            {
                return TryApplyTranslationWithNoDuplicates(ref valueToTranslate, original, existsTranslation);
            }
        }

        /// <summary>
        /// Checks if translation should not be attempted based on current mode and input validation.
        /// </summary>
        private bool IsTranslationNotAllowed(string value, bool isCheckInput)
        {
            return OpenFileMode || (isCheckInput && !IsValidString(value));
        }

        /// <summary>
        /// Attempts to find and apply a translation when duplicates are allowed in the project.
        /// </summary>
        private bool TryApplyTranslationWithDuplicatesAllowed(ref string valueToTranslate, string original, string existsTranslation)
        {
            if (!ParentProject.OriginalsTableRowCoordinates?.ContainsKey(original) == true)
                return false;

            // Any valid row in the table will be parsed, incrementing the row index
            int rowIndex = RowIndex++;
            var coordinates = ParentProject.OriginalsTableRowCoordinates[original];

            // Try to get translation using the current row index in the current table
            if (TryApplyTranslationFromCurrentTable(ref valueToTranslate, original, existsTranslation, coordinates, rowIndex))
                return true;

            // Try to get translation from any available table/row combination
            return TryApplyTranslationFromAnyTable(ref valueToTranslate, original, existsTranslation, coordinates);
        }

        /// <summary>
        /// Attempts to apply translation from the current table at the specified row index.
        /// </summary>
        private bool TryApplyTranslationFromCurrentTable(ref string valueToTranslate, string original, string existsTranslation,
                                                       ConcurrentDictionary<string, ConcurrentSet<int>> coordinates, int rowIndex)
        {
            string tableName = FileName;
            if (coordinates.TryGetValue(tableName, out var rows) && rows.Contains(rowIndex))
            {
                return ApplyTranslation(tableName, rowIndex, original, existsTranslation, ref valueToTranslate);
            }
            return false;
        }

        /// <summary>
        /// Attempts to apply translation from any available table and row.
        /// </summary>
        private bool TryApplyTranslationFromAnyTable(ref string valueToTranslate, string original, string existsTranslation,
                                                   ConcurrentDictionary<string, ConcurrentSet<int>> coordinates)
        {
            foreach (var tableRowsIndexes in coordinates)
            {
                string tableName = tableRowsIndexes.Key;
                var rowIndexes = tableRowsIndexes.Value;

                foreach (var rIndex in rowIndexes)
                {
                    if (ApplyTranslation(tableName, rIndex, original, existsTranslation, ref valueToTranslate))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to find and apply a translation when duplicates are not allowed in the project.
        /// </summary>
        private bool TryApplyTranslationWithNoDuplicates(ref string valueToTranslate, string original, string existsTranslation)
        {
            if (ParentProject.TablesLinesDict?.ContainsKey(original) == true)
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
                    ? ParentProject.TablesLinesDict[original]
                    :  GetRowTranslationText(ParentProject.FilesContent.Tables[tableName].Rows[rowNumber]);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Error applying translation. Table: {tableName}, Row: {rowNumber}, Original: {original}, Error: {ex.Message}");
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
        protected virtual string FixInvalidSymbols(string str)
        {
            if(str == null)
            {
                return str;
            }

           return str.Replace("\u200b", string.Empty);
        }


        #region extra code
        /// <summary>
        /// Splits table cell values into a dictionary for translation lookups.
        /// </summary>
        /// <param name="tableName">The table to process.</param>
        /// <param name="makeLinesCountEqual">Whether to equalize line counts.</param>
        /// <param name="onlyOneTable">Whether to process only the specified table.</param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string tableName, bool makeLinesCountEqual = false, bool onlyOneTable = false)
        {
            if (!ParentProject.DontLoadDuplicates || !SaveFileMode) return;

            var dict = ParentProject.TablesLinesDict ?? new ConcurrentDictionary<string, string>();
            if (onlyOneTable && !ParentProject.FilesContent.Tables.Contains(tableName)) return;

            lock (SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock)
            {
                if (TablesLinesDictFilled && !onlyOneTable) return;
                if (onlyOneTable) dict.Clear();

                foreach (DataTable table in ParentProject.FilesContent.Tables)
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
        private volatile bool TablesLinesDictFilled = false;
        //private bool FirstPassOfDictionaryFilling = true;

        /// <summary>
        /// Adds multi-line translations to the dictionary.
        /// </summary>
        /// <param name="original">The original multi-line string.</param>
        /// <param name="translation">The translation string.</param>
        /// <param name="makeLinesCountEqual">Whether to adjust line counts.</param>
        private void AddMultiLineTranslations(string original, string translation, bool makeLinesCountEqual)
        {
            var dict = ParentProject.TablesLinesDict;
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

        internal string GetRowOriginalText(DataRow dataRow)
        {
            return dataRow.Field<string>(ParentProject.OriginalColumnIndex);
        }
        internal string GetRowTranslationText(DataRow dataRow)
        {
            return dataRow.Field<string>(ParentProject.TranslationColumnIndex);
        }
        #endregion
    }
}
