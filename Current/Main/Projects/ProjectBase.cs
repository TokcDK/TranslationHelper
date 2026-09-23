using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats;
using TranslationHelper.Formats.Abstractions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus;
using TranslationHelper.Models;
using TranslationHelper.SimpleHelpers;

namespace TranslationHelper.Projects
{
    /// <summary>
    /// Base class for project implementations.
    /// <para>
    /// A project owns the translation data and decides which formats run over which files. It is
    /// also the host a format runs in, which is what <see cref="IFormatHost"/> publishes; the
    /// members below stay as they are and are exposed explicitly, so the host contract is a
    /// deliberate list rather than "whatever happens to be visible".
    /// </para>
    /// </summary>
    public abstract class ProjectBase : IProject, IProjectBackupUser, IFormatHost
    {
        #region Fields

        // Public Fields
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
        VarsHideRestore HideRestoreVarsInstance;

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
        /// Cached selected files content.
        /// </summary>
        private DataSet _selectedFilesContent = null;

        /// <summary>
        /// Backing field for FilesContent property.
        /// </summary>
        private DataSet _filesContent = new DataSet();

        /// <summary>
        /// Locker for selected files content.
        /// </summary>
        private readonly object _selectedFilesContentLocker = new object();

        /// <summary>
        /// Locker used in AddTable.
        /// </summary>
        private readonly object _addTableLocker = new object();

        /// <summary>
        /// Locker used during saving.
        /// </summary>
        private readonly object _saveLocker = new object();

        /// <summary>
        /// The format that produced each file table, so a table can be written back by the format that
        /// read it. Filled while the project is parsed and read when a file is opened in the workspace.
        /// </summary>
        private readonly Dictionary<DataTable, FormatBase> _formatOfTable = new Dictionary<DataTable, FormatBase>();

        /// <summary>
        /// True when a translation was written into the project since its database file was last
        /// written. Written from whichever thread a row operation happens to run on, hence volatile.
        /// </summary>
        private volatile bool _hasUnsavedTranslations;

        /// <summary>
        /// The tables whose changes are already being followed. It is what makes following a table
        /// twice impossible, and therefore makes it safe to follow tables wherever they appear without
        /// the caller having to know whether they were followed before.
        /// </summary>
        private readonly HashSet<DataTable> _watchedTables = new HashSet<DataTable>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectBase"/> class.
        /// </summary>
        protected ProjectBase()
        {
            // set value of the parameter for the project work session
            DontLoadDuplicates = AppSettings.DontLoadDuplicates;

            // Both are per project: the list of opened files, and the relation between a files list
            // entry and the content it presents. Creating them here rather than once for the whole
            // application is what lets two projects be open without sharing either.
            OpenedFilesData = new OpenedFilesData();
            FilesListContent = new FilesListContent(() => FilesContent);
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

        /// <summary>
        /// add equal lines to TablesLinesDict while save translation
        /// </summary>
        internal virtual bool TablesLinesDictAddEqual => false;

        /// <summary>
        /// In some cases like opened file by extension it can be useful to detect when need to save file n place where it was opened
        /// </summary>
        public virtual bool IsSaveToSourceFile => false;

        /// <summary>
        /// The files of this project that are open, and which of them is being worked on.
        /// <para>
        /// It belongs to the project rather than to the application, so opening a second project does
        /// not disturb the files of the first one. Created in the constructor, which is what makes it
        /// non-null for every project.
        /// </para>
        /// </summary>
        internal OpenedFilesData OpenedFilesData { get; }

        /// <summary>
        /// Relates an entry of the files list to the content it presents: one entry per file, preceded
        /// by the "[ALL]" entry that presents every file at once.
        /// <para>
        /// It belongs to the project for the same reason <see cref="OpenedFilesData"/> does: the list
        /// and the content it indexes are the project's, and two projects must not share either.
        /// </para>
        /// </summary>
        internal FilesListContent FilesListContent { get; }

        /// <summary>
        /// True when a translation was written into the project since its database file was last
        /// written. This is what closing the project asks about.
        /// <para>
        /// The fact is read from the tables rather than reported by the code that writes them. A
        /// translation reaches a row through the grid, through a row operation, through the search
        /// window, through an undo, and through a format writing a file back; a flag that each of those
        /// had to remember to set would be a flag that is wrong the first time a sixth one is added.
        /// <see cref="DataTable.ColumnChanged"/> is raised by the row itself, whichever code wrote it,
        /// so this is taken from where the change is true rather than from every place that makes it
        /// true.
        /// </para>
        /// </summary>
        internal bool HasUnsavedTranslations => _hasUnsavedTranslations;

        /// <summary>
        /// The format that produced <paramref name="dataTable"/>, or null when the table was not
        /// produced by a format.
        /// <para>
        /// A file is written back by the format that read it, so the format a table came from has to
        /// be remembered while the project is parsed. It is recorded through
        /// <see cref="ITranslationStore.RegisterFormat"/>, which a format calls from the host it runs
        /// in, and read when the file is opened in the workspace.
        /// </para>
        /// </summary>
        internal FormatBase GetFormatOf(DataTable dataTable)
        {
            if (dataTable == null) return null;

            lock (_formatOfTable)
            {
                return _formatOfTable.TryGetValue(dataTable, out var format) ? format : null;
            }
        }

        #endregion

        #region Methods

        // Note: IsValid below is declared as abstract, retaining its original accessibility.
        /// <summary>
        /// Determines whether the project is valid to be opened.
        /// </summary>
        internal abstract bool IsValid();

        // Public Methods
        /// <summary>
        /// Initializes the project and sets up working directories.
        /// <para>
        /// The directories are set on this project rather than on the selected one. A project is
        /// initialised while it is still being opened, which is before it becomes the selected project,
        /// so writing to the selection would set up whichever project happened to be open before and
        /// leave this one without directories.
        /// </para>
        /// </summary>
        public virtual void Init()
        {
            if (string.IsNullOrWhiteSpace(AppData.SelectedProjectFilePath))
                return;

            SelectedGameDir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            SelectedDir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, Name);

            // Allocated when the project is opened rather than in the constructor. A project instance
            // is also created to ask whether it can open a path — one throwaway instance per project
            // type, per open — and those have no session to fill a dictionary for. It used to be
            // allocated when the application already had a project, which is a question the
            // constructor had no business asking and which was answered wrongly for the first project
            // of a session.
            if (DontLoadDuplicates && TablesLinesDict == null)
            {
                TablesLinesDict = new ConcurrentDictionary<string, string>();
            }
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
                // The content exists now, which is the first moment its tables can be followed. Every
                // translation written from here on goes into one of them.
                WatchTables();

                FunctionAutoSave.StartAutoSave(
                    AutosaveTimer = new System.Timers.Timer(),
                    // The project is named rather than looked up when the timer fires: with more than
                    // one project open there is a timer per project, and a save that asked for "the"
                    // project would write whichever one the user happens to be looking at.
                    () => FunctionAutoSave.SaveDBByAutosave(this, _saveLocker),
                    AppSettings.DBAutoSaveTimeout
                );
            }

            return result;
        }

        /// <summary>
        /// Report the project's translations as written into its database file. Called by the save
        /// itself, because only the save knows that the file now holds them.
        /// </summary>
        internal void MarkTranslationsSaved() => _hasUnsavedTranslations = false;

        /// <summary>
        /// Follow every table the project has for changes to its translation column.
        /// </summary>
        private void WatchTables()
        {
            foreach (DataTable table in FilesContent.Tables)
            {
                WatchTable(table);
            }
        }

        /// <summary>
        /// Follow one table for changes to its translation column. Doing nothing when it is already
        /// followed is what makes this safe to call wherever a table appears.
        /// </summary>
        private void WatchTable(DataTable table)
        {
            if (table == null) return;
            if (!_watchedTables.Add(table)) return;

            table.ColumnChanged += OnTableColumnChanged;
        }

        /// <summary>
        /// A cell of one of the project's tables changed. Marks the project when what changed is a
        /// translation and the project is in a state where a translation means work the user did.
        /// <para>
        /// That state is what keeps the mark off the project's own opening. A project fills its tables
        /// while it parses, and reading a database writes a translation into every row it has, so
        /// without it a project would come up already marked and closing it would always ask. Neither
        /// of those is work the user did, and neither is thrown away by closing.
        /// </para>
        /// <para>
        /// The column is recognised by its name rather than by its position, because the name is what a
        /// database file is written from: a table whose translation column is named otherwise is one
        /// whose translations no save would put anywhere, and asking to save them would be asking about
        /// nothing.
        /// </para>
        /// </summary>
        private void OnTableColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (!ProjectReadiness.IsReady) return;
            if (e == null || e.Column == null) return;
            if (e.Column.ColumnName != THSettings.TranslationColumnName) return;

            _hasUnsavedTranslations = true;
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
            return ProjectToolsBackup.BackupRestorePaths(this, BakPaths);
        }

        /// <summary>
        /// Restores the backup of the project files.
        /// </summary>
        /// <returns>True if the backup was successfully restored; otherwise, false.</returns>
        public virtual bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(this, BakPaths, false);
        }

        // Internal Methods
        /// <summary>
        /// Adds a new table to the project.
        /// </summary>
        /// <param name="dataTable">The data table to add.</param>
        /// <param name="infoTable">The information table associated with the data.</param>
        internal void AddTable(DataTable dataTable, DataTable infoTable)
        {
            lock (_addTableLocker)
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                    return;
                if (FilesContent.Tables.Contains(dataTable.TableName))
                    return;

                FilesContent.Tables.Add(dataTable);
                FilesContentInfo.Tables.Add(infoTable);

                // Followed from the moment it exists, rather than only when the project has finished
                // opening: a table added to a project that is already open is one the tables collected
                // when it opened do not include.
                WatchTable(dataTable);
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
            HideRestoreVarsInstance = new VarsHideRestore(HideVarsBase);
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

        #region IFormatHost (explicit implementation)

        // A format is only allowed to know the members below. They are implemented explicitly so
        // the rest of the project's state stays internal and no call site has to change.

        DataSet ITranslationStore.FilesContent => FilesContent;

        DataSet ITranslationStore.FilesContentInfo => FilesContentInfo;

        int ITranslationStore.OriginalColumnIndex => OriginalColumnIndex;

        int ITranslationStore.TranslationColumnIndex => TranslationColumnIndex;

        bool ITranslationStore.DontLoadDuplicates => DontLoadDuplicates;

        ConcurrentDictionary<string, string> ITranslationStore.TablesLinesDict
        {
            get => TablesLinesDict;
            set => TablesLinesDict = value;
        }

        ConcurrentSet<string> ITranslationStore.Hashes
        {
            get => Hashes;
            set => Hashes = value;
        }

        ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>> ITranslationStore.OriginalsTableRowCoordinates
            => OriginalsTableRowCoordinates;

        void ITranslationStore.AddTable(DataTable dataTable, DataTable infoTable)
            => AddTable(dataTable, infoTable);

        void ITranslationStore.RegisterFormat(DataTable dataTable, FormatBase format)
        {
            if (dataTable == null || format == null) return;

            lock (_formatOfTable)
            {
                _formatOfTable[dataTable] = format;
            }
        }

        string IFormatHost.SelectedGameDir => SelectedGameDir;

        string IFormatHost.ProjectWorkDir => ProjectWorkDir;

        string IFormatHost.OpenedFilesDir => OpenedFilesDir;

        bool IFormatHost.SubpathInTableName => SubpathInTableName;

        bool IFormatHost.IsSaveToSourceFile => IsSaveToSourceFile;

        string IFormatHost.CleanStringForCheck(string str) => CleanStringForCheck(str);

        #endregion
    }
}
