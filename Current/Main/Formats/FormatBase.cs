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
        #region Fields

        /// <summary>
        /// Logger instance for this class.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Lock object for thread safety when filling the translation dictionary.
        /// </summary>
        private readonly object SplitTableCellValuesAndTheirLinesToDictionaryThreadsLock = new object();

        /// <summary>
        /// Indicates that the translation dictionary is filled.
        /// </summary>
        private volatile bool TablesLinesDictFilled = false;

        #endregion

        #region Enums

        /// <summary>
        /// Actions to take after finding a keyword during parsing.
        /// </summary>
        protected enum KeywordActionAfter
        {
            Break = -1,
            Continue = 0,
            ReadToEnd = 1
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class without a file path.
        /// </summary>
        /// <param name="parentProject">The parent project.</param>
        protected FormatBase(ProjectBase parentProject)
        {
            ParentProject = parentProject ?? throw new ArgumentNullException(nameof(parentProject), "Parent project cannot be null.");
            if (ParentProject == null)
            {
                var ex = new ArgumentNullException(nameof(parentProject), "Parent project cannot be null.");
                Logger.Error(ex, "Parent project is null in {0} constructor.", nameof(FormatBase));
                throw ex;
            }
            BaseInit();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the file extension(s) that this format can parse (e.g., ".txt" or ".txt,.csv").
        /// </summary>
        public abstract string Extension { get; }

        /// <summary>
        /// Gets a short description of the format.
        /// </summary>
        public virtual string Description => string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether a translation was set.
        /// </summary>
        public bool RET { get; internal set; }

        /// <summary>
        /// Gets the current file path for open/save operations.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the file is in open mode.
        /// </summary>
        public bool OpenFileMode { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether the file is in save mode (inverse of OpenFileMode).
        /// </summary>
        public bool SaveFileMode { get => !OpenFileMode; private set => OpenFileMode = !value; }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the parent project.
        /// </summary>
        internal ProjectBase ParentProject { get; }

        /// <summary>
        /// Gets or sets the main data table with string data.
        /// </summary>
        internal DataTable Data { get; set; }

        /// <summary>
        /// Gets or sets the table with info for the infobox.
        /// </summary>
        internal DataTable Info { get; set; }

        /// <summary>
        /// Gets or sets the current row index during parsing.
        /// </summary>
        protected int RowIndex { get; set; } = 0;

        /// <summary>
        /// Gets a value indicating whether to use the file name without extension for the table name.
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
                    int attempts = 10;
                    while (string.IsNullOrWhiteSpace(filePath) && attempts-- > 0)
                    {
                        filePath = GetFilePath();
                    }
                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        return string.Empty;
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens a file in open mode.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        public bool Open(string filePath)
        {
            if (!IsValidFilePath(filePath)) return false;

            FilePath = filePath;
            OpenFileMode = true;
            return TryOpen() && CheckTablesContent(FileName);
        }

        /// <summary>
        /// Saves a file in save mode if there are translated entries.
        /// </summary>
        /// <param name="filePath">The file path (optional).</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
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
            return IsAnyTranslated() && TrySave();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Checks if a string is valid for adding to the table.
        /// </summary>
        /// <param name="inputString">The string to check.</param>
        /// <returns>True if the string is valid; otherwise, false.</returns>
        internal virtual bool IsValidString(string inputString)
        {
            inputString = ParentProject.CleanStringForCheck(inputString);
            return !string.IsNullOrWhiteSpace(inputString) && !inputString.ForJPLangHaveMostOfRomajiOtherChars();
        }

        /// <summary>
        /// Adds a string to the table with options. In save mode, replaces <paramref name="value"/> with the translation.
        /// </summary>
        /// <param name="value">Reference to the original string.</param>
        /// <param name="info">String info.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
        /// <param name="existsTranslation">Existing translation, if any.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
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
        /// <param name="info">String info.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string value, string info = "", bool isCheckInput = true)
        {
            return AddRowData(FileName, value, info, isCheckInput);
        }

        /// <summary>
        /// Adds an array of strings to the table with options. In save mode, replaces <paramref name="values"/>[0] with the translation, using <paramref name="values"/>[1] by default.
        /// </summary>
        /// <param name="values">Array: first element is the original, second is the translation.</param>
        /// <param name="info">String info.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
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
        /// Adds an array of strings to the table with options.
        /// </summary>
        /// <param name="values">Array of original strings.</param>
        /// <param name="info">String info.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
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
        /// <param name="info">String info.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
        /// <returns>True if added; otherwise, false.</returns>
        internal bool AddRowData(string tablename, string value, string info = "", bool isCheckInput = true)
        {
            return AddRowData(tablename, new[] { value }, info, isCheckInput);
        }

        /// <summary>
        /// Adds an array of strings to the specified table with options.
        /// </summary>
        /// <param name="tablename">File/table name.</param>
        /// <param name="values">Array of original strings.</param>
        /// <param name="info">String info.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
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
        /// Sets the translation for a string if found, updating the original by reference.
        /// </summary>
        /// <param name="valueToTranslate">Original string; replaced with translation if found.</param>
        /// <param name="existsTranslation">Optional existing translation for comparison.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
        /// <returns>True if translation was set and differs from the original or existing translation; otherwise, false.</returns>
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
        /// Initializes the table content for the current file path.
        /// </summary>
        protected void InitTableContent()
        {
            if (string.IsNullOrEmpty(GetFilePath())) return;
            InitTableContent(FileName);
        }

        /// <summary>
        /// Initializes the table content with the specified name and extra columns.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="extraColumns">Extra columns.</param>
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
        /// Splits table cell values and their lines into a dictionary for translation lookup.
        /// </summary>
        /// <param name="tableName">Table to process.</param>
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

        /// <summary>
        /// Gets the original text from a data row.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <returns>The original text.</returns>
        internal string GetRowOriginalText(DataRow dataRow)
        {
            return dataRow.Field<string>(ParentProject.OriginalColumnIndex);
        }

        /// <summary>
        /// Gets the translation text from a data row.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <returns>The translation text.</returns>
        internal string GetRowTranslationText(DataRow dataRow)
        {
            return dataRow.Field<string>(ParentProject.TranslationColumnIndex);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Adds a string to the data and info tables.
        /// </summary>
        /// <param name="values">Array of values.</param>
        /// <param name="info">String info.</param>
        protected void AddString(string[] values, string info = "")
        {
            Data.Rows.Add(values);
            Info.Rows.Add(info);
        }

        /// <summary>
        /// Gets the file path depending on the current mode.
        /// </summary>
        /// <returns>The file path for the current operation.</returns>
        protected virtual string GetFilePath()
        {
            return OpenFileMode ? GetOpenFilePath() : GetSaveFilePath();
        }

        /// <summary>
        /// Gets the file path for open operations.
        /// </summary>
        /// <returns>The file path for opening.</returns>
        protected virtual string GetOpenFilePath()
        {
            return FilePath;
        }

        /// <summary>
        /// Gets the file path for save operations.
        /// </summary>
        /// <returns>The file path for saving.</returns>
        protected virtual string GetSaveFilePath()
        {
            return FilePath;
        }

        /// <summary>
        /// Modifies the original string before adding to the table.
        /// </summary>
        /// <param name="original">Original string.</param>
        /// <returns>Modified string.</returns>
        protected virtual string AddRowDataPreAddOriginalStringMod(string original) => original;

        /// <summary>
        /// Performs actions before opening a file.
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
        /// Performs extra actions before opening.
        /// </summary>
        protected virtual void PreOpenExtraActions() { }

        /// <summary>
        /// Parses the file depending on the current mode.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected bool ParseFile()
        {
            if (!FilePreOpenActions()) return false;
            FileOpen();
            return FilePostOpen();
        }

        /// <summary>
        /// Performs file open actions.
        /// </summary>
        protected virtual void FileOpen() { }

        /// <summary>
        /// Performs actions after opening a file.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool FilePostOpen()
        {
            return (!OpenFileMode && WriteFileData()) || OpenFileMode;
        }

        /// <summary>
        /// Checks and processes table content after opening.
        /// </summary>
        /// <param name="tableName">Table name to check.</param>
        /// <param name="isDictionary">Process as dictionary (not implemented).</param>
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
        /// Writes file data after checking.
        /// </summary>
        /// <param name="filePath">File path to write.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool WriteFileData(string filePath = "") => DoWriteFile(filePath);

        /// <summary>
        /// Performs the actual file write.
        /// </summary>
        /// <param name="filePath">File path to write.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool DoWriteFile(string filePath = "") => false;

        /// <summary>
        /// Performs an action when a translation is set.
        /// </summary>
        protected virtual void SetTranslationIsTranslatedAction() { }

        /// <summary>
        /// Removes or replaces invalid symbols in a translation string.
        /// </summary>
        /// <param name="str">String to clean.</param>
        /// <returns>Cleaned string.</returns>
        protected virtual string FixInvalidSymbols(string str)
        {
            if (str == null)
            {
                return str;
            }
            return str.Replace("\u200b", string.Empty);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes base properties and format settings.
        /// </summary>
        /// <param name="filePath">File path or null.</param>
        private void BaseInit()
        {
            SetupWhenDontLoadDuplicates();
        }

        /// <summary>
        /// Sets up when the "Don't load duplicates" option is enabled.
        /// </summary>
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
        /// Checks if the file path is valid.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>True if the path is valid; otherwise, false.</returns>
        private bool IsValidFilePath(string filePath)
        {
            bool isValid = !string.IsNullOrWhiteSpace(filePath)
                && File.Exists(filePath);

            if (!isValid)
            {
                Logger.Debug($"{this.GetType().Name}: Invalid file path: {filePath}");
            }

            return isValid;
        }

        /// <summary>
        /// Checks if there are any translated rows.
        /// </summary>
        /// <returns>True if there are translations; otherwise, false.</returns>
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
        /// Checks if translation is not allowed.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <param name="isCheckInput">Whether to check string validity.</param>
        /// <returns>True if translation is not allowed; otherwise, false.</returns>
        private bool IsTranslationNotAllowed(string value, bool isCheckInput)
        {
            return OpenFileMode || (isCheckInput && !IsValidString(value));
        }

        /// <summary>
        /// Applies translation if duplicates are allowed.
        /// </summary>
        private bool TryApplyTranslationWithDuplicatesAllowed(ref string valueToTranslate, string original, string existsTranslation)
        {
            if (!ParentProject.OriginalsTableRowCoordinates?.ContainsKey(original) == true)
                return false;

            int rowIndex = RowIndex++;
            var coordinates = ParentProject.OriginalsTableRowCoordinates[original];

            if (TryApplyTranslationFromCurrentTable(ref valueToTranslate, original, existsTranslation, coordinates, rowIndex))
                return true;

            return TryApplyTranslationFromAnyTable(ref valueToTranslate, original, existsTranslation, coordinates);
        }

        /// <summary>
        /// Applies translation from the current table and row index.
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
        /// Applies translation from any table and row.
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
        /// Applies translation if duplicates are not allowed.
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
        /// Applies translation to the provided string if found.
        /// </summary>
        /// <param name="tableName">Table name or null for dictionary lookup.</param>
        /// <param name="rowNumber">Row number or -1 for dictionary lookup.</param>
        /// <param name="original">Original string.</param>
        /// <param name="existsTranslation">Optional existing translation.</param>
        /// <param name="valueToTranslate">String to update with translation.</param>
        /// <returns>True if translation applied; otherwise, false.</returns>
        private bool ApplyTranslation(string tableName, int rowNumber, string original, string existsTranslation, ref string valueToTranslate)
        {
            try
            {
                valueToTranslate = tableName == null
                    ? ParentProject.TablesLinesDict[original]
                    : GetRowTranslationText(ParentProject.FilesContent.Tables[tableName].Rows[rowNumber]);
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
        /// Adds translations for multi-line originals.
        /// </summary>
        /// <param name="original">Original multi-line string.</param>
        /// <param name="translation">Translation string.</param>
        /// <param name="makeLinesCountEqual">Whether to equalize line counts.</param>
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
        /// Tries to add a translation to the dictionary.
        /// </summary>
        /// <param name="dict">Translation dictionary.</param>
        /// <param name="original">Original string.</param>
        /// <param name="translation">Translation string.</param>
        private static void TryAddTranslation(ConcurrentDictionary<string, string> dict, string original, string translation)
        {
            if (translation.Length > 0 && original != translation && !dict.ContainsKey(original))
            {
                dict.TryAdd(original, translation);
            }
        }

        #endregion

        #region Protected Virtual Methods for File Operations

        /// <summary>
        /// Tries to open a file by parsing.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool TryOpen() => ParseFile();

        /// <summary>
        /// Tries to save a file by parsing.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool TrySave() => ParseFile();

        #endregion
    }
}
