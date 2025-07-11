using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus;

namespace TranslationHelper.Projects
{
    /// <summary>
    /// Base class for project implementations.
    /// </summary>
    public abstract class ProjectBase : IProject, IProjectBackupUser
    {
        #region Fields

        // Public Fields
        /// <summary>
        /// Indicates whether the database is currently loading.
        /// </summary>
        public bool IsLoadingDB = false;

        /// <summary>
        /// True when file is in open mode.
        /// </summary>
        public bool OpenFileMode = true;

        /// <summary>
        /// Gets the backup paths. When empty, all parsed files will be added.
        /// </summary>
        public virtual List<string> BakPaths { get; set; } = new List<string>();

        /// <summary>
        /// The project file path that was selected.
        /// </summary>
        public string ProjectPath { get; internal set; } = "";

        /// <summary>
        /// Indicates if subpath should be included in the table name.
        /// </summary>
        public virtual bool SubpathInTableName => false;

        /// <summary>
        /// Holds cached file row menus.
        /// </summary>
        public IFileRowMenuItem[] RowMenusCache { get; internal set; } = null;

        /// <summary>
        /// Immutable setting for loading duplicates.
        /// </summary>
        public readonly bool DontLoadDuplicates;

        // Protected Fields
        /// <summary>
        /// Logger for the project.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Base dictionary for hide patterns.
        /// </summary>
        protected Dictionary<string, string> HideVarsBase { get; set; }

        // Internal Fields
        /// <summary>
        /// Index of the Original column.
        /// </summary>
        internal int OriginalColumnIndex = 0;

        /// <summary>
        /// Index of the main Translation column.
        /// </summary>
        internal int TranslationColumnIndex = 1;

        /// <summary>
        /// Coordinates data for rows with identical translations.
        /// </summary>
        internal ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>> OriginalsTableRowCoordinates { get; set; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>>();

        /// <summary>
        /// Contains the project's variable to be hidden/restored during translation actions.
        /// </summary>
        ProjectHideRestoreVarsInstance HideRestoreVarsInstance;

        /// <summary>
        /// Holds dictionary of row original and translation values for saving.
        /// </summary>
        internal ConcurrentDictionary<string, string> TablesLinesDict;

        /// <summary>
        /// Set used to prevent duplicate original values.
        /// </summary>
        internal ConcurrentSet<string> Hashes;

        /// <summary>
        /// Selected game directory path.
        /// </summary>
        internal string SelectedGameDir;

        /// <summary>
        /// Selected directory path where translations occur.
        /// </summary>
        internal string SelectedDir;

        /// <summary>
        /// Directory path for opened files.
        /// </summary>
        internal string OpenedFilesDir;

        /// <summary>
        /// Directory where the project's files are located.
        /// </summary>
        internal string ProjectWorkDir;

        // Private Fields
        /// <summary>
        /// Timer for autosave functionality.
        /// </summary>
        private static System.Timers.Timer AutosaveTimer;

        /// <summary>
        /// Locker for selected files content.
        /// </summary>
        private readonly object _selectedFilesContentLocker = new object();

        /// <summary>
        /// Cached selected files content.
        /// </summary>
        private DataSet _selectedFilesContent = null;

        /// <summary>
        /// Backing field for FilesContent property.
        /// </summary>
        private DataSet _filesContent = new DataSet();

        /// <summary>
        /// Locker used in AddTable.
        /// </summary>
        private readonly object AddTableLocker = new object();

        /// <summary>
        /// Locker used during saving.
        /// </summary>
        private readonly object _saveLocker = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectBase"/> class.
        /// </summary>
        protected ProjectBase()
        {
            // set value of the parameter for the project work session
            DontLoadDuplicates = AppSettings.DontLoadDuplicates;

            if (AppData.CurrentProject == null)
                return;

            if (SaveFileMode && DontLoadDuplicates)
                TablesLinesDict = new ConcurrentDictionary<string, string>();
        }

        #endregion

        #region Properties

        // Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether the project is in save file mode.
        /// True indicates file write mode; false indicates file open mode.
        /// </summary>
        public bool SaveFileMode
        {
            get => !OpenFileMode;
            set => OpenFileMode = !value;
        }

        /// <summary>
        /// Gets the set of file indexes to save.
        /// </summary>
        public HashSet<int> FileIndexesToSave { get; private set; } = null;

        /// <summary>
        /// Gets or sets the main work table data.
        /// </summary>
        public DataSet FilesContent
        {
            get
            {
                if (FileIndexesToSave != null)
                {
                    lock (_selectedFilesContentLocker)
                    {
                        if (this._selectedFilesContent != null)
                            return this._selectedFilesContent; // return selected files content if it was set

                        // copy only tables which need to be saved
                        _selectedFilesContent = new DataSet();
                        int tableCount = _filesContent.Tables.Count;
                        for (int i = 0; i < tableCount; i++)
                        {
                            var table = FileIndexesToSave.Contains(i)
                                ? _filesContent.Tables[i].Copy()
                                : _filesContent.Tables[i].Clone();

                            _selectedFilesContent.Tables.Add(table);
                        }

                        return _selectedFilesContent;
                    }
                }
                else
                {
                    return _filesContent;
                }
            }
            set
            {
                if (_filesContent == value)
                    return;

                _filesContent = value ?? new DataSet();
            }
        }

        /// <summary>
        /// Gets or sets the work table information.
        /// </summary>
        public DataSet FilesContentInfo { get; set; } = new DataSet();

        /// <summary>
        /// Gets or sets the additional work table data (work in progress).
        /// </summary>
        public DataSet FilesContentAll { get; set; }

        /// <summary>
        /// Gets the project's name.
        /// </summary>
        public abstract string Name { get; }

        // Internal Properties
        /// <summary>
        /// Gets a filter string for file open dialogs.
        /// </summary>
        internal virtual string FileFilter => string.Empty;

        /// <summary>
        /// Gets a task that is executed before the database is saved.
        /// </summary>
        internal virtual Task PreSaveDB() { return Task.CompletedTask; }

        /// <summary>
        /// Gets a prefix for the project title.
        /// </summary>
        internal virtual string ProjectTitlePrefix => string.Empty;

        /// <summary>
        /// Gets the folder name used for the project database.
        /// </summary>
        internal virtual string ProjectDBFolderName => "Other";

        /// <summary>
        /// Gets the project database file name used for save/load.
        /// </summary>
        internal virtual string ProjectDBFileName => string.Empty;

        /// <summary>
        /// Gets the newline symbol for the project.
        /// </summary>
        internal virtual string NewlineSymbol => Environment.NewLine;

        /// <summary>
        /// Gets a value indicating whether test run is enabled.
        /// </summary>
        internal virtual bool IsTestRunEnabled => false;

        // Note: IsValid below is declared as abstract, retaining its original accessibility.
        /// <summary>
        /// Determines whether the project is valid to be opened.
        /// </summary>
        internal abstract bool IsValid();

        #endregion

        #region Methods

        // Public Methods
        /// <summary>
        /// Initializes the project and sets up working directories.
        /// </summary>
        public virtual void Init()
        {
            if (string.IsNullOrWhiteSpace(AppData.SelectedProjectFilePath))
                return;

            AppData.CurrentProject.SelectedGameDir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            AppData.CurrentProject.SelectedDir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            AppData.CurrentProject.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, Name);
        }

        /// <summary>
        /// Opens the project files.
        /// </summary>
        /// <returns>True if the project files were successfully opened; otherwise, false.</returns>
        public bool Open()
        {
            bool result = TryOpen();

            if (result == true)
            {
                FunctionAutoSave.StartAutoSave(
                    AutosaveTimer = new System.Timers.Timer(),
                    () => FunctionAutoSave.SaveDBByAutosave(_saveLocker),
                    AppSettings.DBAutoSaveTimeout
                );
            }

            return result;
        }

        /// <summary>
        /// Saves the project files.
        /// </summary>
        /// <param name="fileIndexesToSave">Optional set of file indexes to save.</param>
        /// <returns>True if the project was successfully saved; otherwise, false.</returns>
        public bool Save(HashSet<int> fileIndexesToSave = null)
        {
            // Ensure valid indexes only
            if (fileIndexesToSave != null && fileIndexesToSave.Count > 0)
            {
                fileIndexesToSave = fileIndexesToSave.Where(i => i >= 0 && i < FilesContent.Tables.Count).ToHashSet();
            }

            FileIndexesToSave = fileIndexesToSave;

            bool result = false;

            lock (_saveLocker)
            {
                try
                {
                    result = TrySave();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while saving project files" + ": " + ex.Message, ex);
                }
                finally
                {
                    FileIndexesToSave = null; // reset indexes after save
                    _selectedFilesContent = null; // reset selected files content
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a backup of the project files.
        /// </summary>
        /// <returns>True if the backup was successfully created; otherwise, false.</returns>
        public virtual bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(BakPaths);
        }

        /// <summary>
        /// Restores the backup of the project files.
        /// </summary>
        /// <returns>True if the backup was successfully restored; otherwise, false.</returns>
        public virtual bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(BakPaths, false);
        }

        // Internal Methods
        /// <summary>
        /// Adds a new table to the project.
        /// </summary>
        /// <param name="dataTable">The data table to add.</param>
        /// <param name="infoTable">The information table associated with the data.</param>
        internal void AddTable(DataTable dataTable, DataTable infoTable)
        {
            lock (AddTableLocker)
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                    return;
                if (FilesContent.Tables.Contains(dataTable.TableName))
                    return;

                FilesContent.Tables.Add(dataTable);
                FilesContentInfo.Tables.Add(infoTable);
            }
        }

        /// <summary>
        /// Skip rule for splitting lines in project-specific scenarios.
        /// </summary>
        /// <param name="o">The original text.</param>
        /// <param name="t">The translation text.</param>
        /// <param name="tind">Translation column index.</param>
        /// <param name="rind">Row index.</param>
        /// <returns>True to skip processing the line; otherwise, false.</returns>
        internal virtual bool LineSplitProjectSpecificSkipForLine(string o, string t, int tind = -1, int rind = -1)
        {
            return false;
        }

        /// <summary>
        /// Skip rule for splitting tables in project-specific scenarios.
        /// </summary>
        /// <param name="table">The data table to check.</param>
        /// <returns>True to skip processing the table; otherwise, false.</returns>
        internal virtual bool LineSplitProjectSpecificSkipForTable(DataTable table)
        {
            return false;
        }

        /// <summary>
        /// Extraction rules for online translation specific to the project.
        /// </summary>
        /// <param name="o">The original text.</param>
        /// <param name="t">The translation text.</param>
        /// <param name="tind">Translation column index.</param>
        /// <param name="rind">Row index.</param>
        /// <returns>Extracted string based on custom rules.</returns>
        internal virtual string OnlineTranslationProjectSpecificExtractionRules(string o, string t, int tind = -1, int rind = -1)
        {
            return string.Empty;
        }

        /// <summary>
        /// Pre-translation action for online translation specific to the project.
        /// </summary>
        /// <param name="o">The original text.</param>
        /// <param name="t">The translation text.</param>
        /// <param name="tind">Translation column index.</param>
        /// <param name="rind">Row index.</param>
        /// <returns>The transformed original text with variables hidden.</returns>
        internal virtual string OnlineTranslationProjectSpecificPretranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            HideRestoreVarsInstance = new ProjectHideRestoreVarsInstance(HideVarsBase);
            return HideRestoreVarsInstance.HideVARSBase(o);
        }

        /// <summary>
        /// Post-translation action for online translation specific to the project.
        /// </summary>
        /// <param name="o">The original text.</param>
        /// <param name="t">The translated text.</param>
        /// <param name="tind">Translation column index.</param>
        /// <param name="rind">Row index.</param>
        /// <returns>The translation text with variables restored.</returns>
        internal virtual string OnlineTranslationProjectSpecificPostTranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            var str = HideRestoreVarsInstance.RestoreVARS(t);
            HideRestoreVarsInstance.Dispose();
            return str;
        }

        /// <summary>
        /// Skip rule for online translation specific to the project.
        /// </summary>
        /// <param name="o">The original text.</param>
        /// <param name="t">The translation text.</param>
        /// <param name="tind">Translation column index.</param>
        /// <param name="rind">Row index.</param>
        /// <returns>True to skip processing the line; otherwise, false.</returns>
        internal virtual bool OnlineTranslationProjectSpecificSkipLine(string o, string t, int tind = -1, int rind = -1)
        {
            return false;
        }

        /// <summary>
        /// Cleans a string before processing to remove special characters.
        /// </summary>
        /// <param name="str">The string to be cleaned.</param>
        /// <returns>The cleaned string.</returns>
        internal virtual string CleanStringForCheck(string str)
        {
            return str;
        }

        /// <summary>
        /// Applies hardcoded fixes to a given translation.
        /// </summary>
        /// <param name="original">The original text.</param>
        /// <param name="translation">The translation text.</param>
        /// <returns>The modified translation after fixes.</returns>
        internal virtual string HardcodedFixes(string original, string translation)
        {
            return translation;
        }

        /// <summary>
        /// Checks for row issues specific to the project.
        /// </summary>
        /// <param name="row">The data row to check.</param>
        /// <returns>True if there is an issue; otherwise, false.</returns>
        internal virtual bool CheckForRowIssue(DataRow row) { return false; }

        /// <summary>
        /// Determines if a string is valid for translation.
        /// </summary>
        /// <param name="inputString">The string to validate.</param>
        /// <returns>True if valid; otherwise, false.</returns>
        internal virtual bool IsValidForTranslation(string inputString) { return true; }

        /// <summary>
        /// Executes actions after writing a translation.
        /// </summary>
        internal virtual void AfterTranslationWriteActions() { FunctionsProcess.OpenProjectsDir(); }

        /// <summary>
        /// Gets the list of main menu items for the project.
        /// </summary>
        /// <returns>An array of <see cref="IMainMenuItem"/>.</returns>
        internal virtual IMainMenuItem[] MainMenuItemMenusList => Array.Empty<IMainMenuItem>();

        /// <summary>
        /// Gets the list of file list menu items for the project.
        /// </summary>
        /// <returns>An array of <see cref="IFileListMenuItem"/>.</returns>
        internal virtual IFileListMenuItem[] FilesListItemMenusList => Array.Empty<IFileListMenuItem>();

        /// <summary>
        /// Gets the list of file row menu items for the project.
        /// </summary>
        /// <returns>An array of <see cref="IFileRowMenuItem"/>.</returns>
        internal virtual IFileRowMenuItem[] FileRowItemMenusList => Array.Empty<IFileRowMenuItem>();

        // Protected Methods
        /// <summary>
        /// Tries to open the project files.
        /// </summary>
        /// <returns>True if the project files were successfully opened; otherwise, false.</returns>
        protected abstract bool TryOpen();

        /// <summary>
        /// Tries to save the project files.
        /// </summary>
        /// <returns>True if the project files were successfully saved; otherwise, false.</returns>
        protected abstract bool TrySave();

        #endregion
    }
}
