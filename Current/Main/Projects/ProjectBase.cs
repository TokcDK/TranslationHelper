using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus;

namespace TranslationHelper.Projects
{
    public abstract class ProjectBase : IProject, IProjectBackupUser
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected ProjectBase()
        {
            // set value of the parameter for the project work session
            DontLoadDuplicates = AppSettings.DontLoadDuplicates;

            if (AppData.CurrentProject == null) return;

            if (SaveFileMode && DontLoadDuplicates) TablesLinesDict = new ConcurrentDictionary<string, string>();
        }

        private static System.Timers.Timer AutosaveTimer;

        // for the project work session
        // tables must store in the selected project
        // and binded to the selected table
        //internal virtual void BindFileContent(DataGridView visibleTable)
        //{
        //// visibleTable.DataSource = TheProjectCurrentTable binding code with any way; 
        //}

        /// <summary>
        /// Can be set state if DB is loading for the project
        /// </summary>
        public bool IsLoadingDB = false;

        /// <summary>
        /// true - when file open, false - when file writing
        /// </summary>
        public bool OpenFileMode = true;
        /// <summary>
        /// true - when file write, false - when file open
        /// </summary>
        public bool SaveFileMode { get => !OpenFileMode; set => OpenFileMode = !value; }

        /// <summary>
        /// Index of file in files list to save
        /// </summary>
        public HashSet<int> FileIndexesToSave { get; private set; } = null;

        /// <summary>
        /// Index of Original column
        /// </summary>
        internal int OriginalColumnIndex = 0;
        /// <summary>
        /// Index of main Translation column
        /// </summary>
        internal int TranslationColumnIndex = 1;

        private readonly object _selectedFilesContentLocker = new object();
        private DataSet _selectedFilesContent = null;
        private DataSet _filesContent = new DataSet();
        /// <summary>
        /// main work table data
        /// </summary>
        public DataSet FilesContent 
        {
            get
            {
                if(FileIndexesToSave != null)
                {
                    lock (_selectedFilesContentLocker)
                    {
                        if (this._selectedFilesContent != null)
                        {
                            return this._selectedFilesContent; // return selected files content if it was set
                        }

                        // copy only table which need to be saved
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
                if (_filesContent == value) return;

                _filesContent = value ?? new DataSet();
            } 
        }

        /// <summary>
        /// main work table infos
        /// </summary>
        public DataSet FilesContentInfo { get; set; } = new DataSet();

        /// <summary>
        /// main work table data for all (wip)
        /// </summary>
        public DataSet FilesContentAll { get; set; }

        /// <summary>
        /// main table/row index coordinates data for same translation for identical and for write functions.
        /// Format:
        ///     original value:
        ///         list of table names:
        ///             list of row numbers:
        /// </summary>
        internal ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>> OriginalsTableRowCoordinates { get; set; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>>();


        readonly object AddTableLocker = new object();
        /// <summary>
        /// add new <paramref name="tableData"/> in tables list
        /// </summary>
        /// <param name="tableData"></param>
        internal void AddTable(DataTable dataTable, DataTable infoTable)
        {
            lock (AddTableLocker)
            {
                if (dataTable == null || dataTable.Rows.Count == 0) return;
                if (FilesContent.Tables.Contains(dataTable.TableName)) return;

                FilesContent.Tables.Add(dataTable);
                FilesContentInfo.Tables.Add(infoTable);
            }
        }

        /// <summary>
        /// Set on project open and will be used for all project's session.
        /// Even if value of original option was changed in program Settings after project was opened will be used this old value for the project.
        /// </summary>
        public readonly bool DontLoadDuplicates;

        /// <summary>
        /// In some cases like opened file by extension it can be useful to detect when need to save file n place where it was opened
        /// </summary>
        public virtual bool IsSaveToSourceFile => false;

        ///// <summary>
        ///// Current parsing format
        ///// </summary>
        //public FormatBase CurrentFormat;

        /// <summary>
        /// set here som vars before open or kind of
        /// </summary>
        public virtual void Init()
        {
            if (string.IsNullOrWhiteSpace(AppData.SelectedProjectFilePath)) return;

            AppData.CurrentProject.SelectedGameDir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            AppData.CurrentProject.SelectedDir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            AppData.CurrentProject.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, Name);
        }

        /// <summary>
        /// The project's name
        /// </summary>
        /// <returns></returns>
        public abstract string Name { get; }

        /// <summary>
        /// Conditions to detect on open
        /// </summary>
        /// <returns></returns>
        internal abstract bool IsValid();

        /// <summary>
        /// Project's filter for fileopen dialog
        /// </summary>
        /// <returns></returns>
        internal virtual string FileFilter => string.Empty;

        /// <summary>
        /// executed before DB will be saved
        /// </summary>
        internal virtual Task PreSaveDB() { return Task.CompletedTask; }

        /// <summary>
        /// Project's Title prefix
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectTitlePrefix => string.Empty;

        /// <summary>
        /// Project folder name to locate files in DB and Work folders
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectDBFolderName => "Other";

        /// <summary>
        /// Returns project's DB file name for save/load
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectDBFileName => string.Empty;

        /// <summary>
        /// Open project files
        /// </summary>        
        /// <returns></returns>
        public bool Open()
        {
            bool result = TryOpen();

            if (result == true)
            {
                FunctionAutoSave.StartAutoSave(
                    AutosaveTimer = new System.Timers.Timer(), 
                    FunctionsDBFile.SaveDB, 
                    AppSettings.DBAutoSaveTimeout
                    );
            }

            return result;
        }
        protected abstract bool TryOpen();

        /// <summary>
        /// Save project files
        /// </summary>        
        /// <returns></returns>
        public bool Save(HashSet<int> fileIndexesToSave = null)
        {
            // easy way maybe is to replace the FilesContent tables with fake tables where only tables to save will contain data and cotent of other tables will be empty
            // TODO:better to make the selected files save using more correct way not to break the data integrity

            if (fileIndexesToSave != null && fileIndexesToSave.Count > 0)
            {
                fileIndexesToSave = fileIndexesToSave.Where(i=> i >= 0 && i < FilesContent.Tables.Count).ToHashSet();
            }
            
            FileIndexesToSave = fileIndexesToSave;

            bool result = false;
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

            return result;
        }
        protected abstract bool TrySave();

        /// <summary>
        /// Project folder name to locate files in DB and Work folders
        /// </summary>
        /// <returns></returns>
        internal virtual string NewlineSymbol => Environment.NewLine;

        /// <summary>
        /// test run menu. maybe it is obsolete and will be removed later
        /// </summary>
        internal virtual bool IsTestRunEnabled => false;

        /// <summary>
        /// Project specific line skip rules for line split function. When check line.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal virtual bool LineSplitProjectSpecificSkipForLine(string o, string t, int tind = -1, int rind = -1)
        {
            return false;
        }

        /// <summary>
        /// Project specific line skip rules for line split function. When check table.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal virtual bool LineSplitProjectSpecificSkipForTable(DataTable table)
        {
            return false;
        }

        /// <summary>
        /// Project specific rules of text extraction for translation
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal virtual string OnlineTranslationProjectSpecificExtractionRules(string o, string t, int tind = -1, int rind = -1)
        {
            return string.Empty;
        }

        ProjectHideRestoreVarsInstance HideRestoreVarsInstance;
        protected Dictionary<string, string> HideVarsBase { get; set; }

        /// <summary>
        /// Pre online translation project's actions
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <returns></returns>
        internal virtual string OnlineTranslationProjectSpecificPretranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            HideRestoreVarsInstance = new ProjectHideRestoreVarsInstance(HideVarsBase);

            return HideRestoreVarsInstance.HideVARSBase(o);
        }

        /// <summary>
        /// Post online translation project's actions
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <returns></returns>
        internal virtual string OnlineTranslationProjectSpecificPostTranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            var str = HideRestoreVarsInstance.RestoreVARS(t);
            HideRestoreVarsInstance.Dispose();

            return str;
        }

        /// <summary>
        /// Path for selected game dir which is translating
        /// </summary>
        internal string SelectedGameDir;
        /// <summary>
        /// Path for selected dir where to translate.
        /// In most causes it is game's dir.
        /// </summary>
        internal string SelectedDir;
        /// <summary>
        /// Path for selected dir where to translate.
        /// In most causes it is game's dir.
        /// </summary>
        internal string OpenedFilesDir;
        /// <summary>
        /// Path to dir where project's files are located
        /// </summary>
        internal string ProjectWorkDir;

        /// <summary>
        /// Project specific skip rules for Online Translation. Skip line when True
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal virtual bool OnlineTranslationProjectSpecificSkipLine(string o, string t, int tind = -1, int rind = -1)
        {
            return false;
        }

        /// <summary>
        /// cleaning string before check to make checking more correct and check with no specsymbols
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal virtual string CleanStringForCheck(string str)
        {
            return str;
        }

        /// <summary>
        /// For save mode. File the dictionary with row's original,translation values
        /// </summary>
        internal ConcurrentDictionary<string, string> TablesLinesDict;

        /// <summary>
        /// Hash of adding original values to prevent duplicates to be added.
        /// filtering records duplicates while adding to main work data table.
        /// </summary>
        internal ConcurrentSet<string> Hashes;

        /// <summary>
        /// add equal lines to TablesLinesDict while save translation
        /// </summary>
        internal virtual bool TablesLinesDictAddEqual => false;

        /// <summary>
        /// include subpath in table name for project where is can be included files with identical names but different folders
        /// </summary>
        public virtual bool SubpathInTableName => false;

        /// <summary>
        /// Hardcoded fixes for cells for specific projects
        /// </summary>
        /// <returns></returns>
        internal virtual string HardcodedFixes(string original, string translation)
        {
            return translation;
        }

        /// <summary>
        /// Backup paths
        /// When empty, will be added all files parsed in OpenSaveFilesBase
        /// </summary>
        /// <returns></returns>
        public virtual List<string> BakPaths { get; set; } = new List<string>();

        /// <summary>
        /// Must make buckup of project translating original files<br/>if any code exit here else will return false
        /// </summary>
        /// <returns></returns>
        public virtual bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(BakPaths);
        }

        /// <summary>
        /// Will restore made buckup of project translating original files<br/>if any code exit here and buckup exists<br/>else will return false
        /// </summary>
        /// <returns></returns>
        public virtual bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(BakPaths, false);
        }

        /// <summary>
        /// specific project's row issue check
        /// Using in row issue checking from search options
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal virtual bool CheckForRowIssue(DataRow row) { return false; }

        /// <summary>
        /// true if <paramref name="inputString"/> is valid for translation
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal virtual bool IsValidForTranslation(string inputString) { return true; }

        /// <summary>
        /// here can be set actions to execute after write of translation
        /// </summary>
        internal virtual void AfterTranslationWriteActions() { FunctionsProcess.OpenProjectsDir(); }

        /// <summary>
        /// Main menus which need to be added for the project
        /// </summary>
        /// <returns></returns>
        internal virtual IMainMenuItem[] MainMenuItemMenusList => Array.Empty<IMainMenuItem>();
        /// <summary>
        /// Files list item menus which must be added for the project
        /// </summary>
        /// <returns></returns>
        internal virtual IFileListMenuItem[] FilesListItemMenusList => Array.Empty<IFileListMenuItem>();
        /// <summary>
        /// FileRow menus which must be added for the project
        /// </summary>
        /// <returns></returns>
        internal virtual IFileRowMenuItem[] FileRowItemMenusList => Array.Empty<IFileRowMenuItem>();

        /// <summary>
        /// Row menus cache for project to not search them each time
        /// </summary>
        public IFileRowMenuItem[] RowMenusCache { get; internal set; } = null;

        /// <summary>
        /// Path to the project file which was selected for the project
        /// </summary>
        public string ProjectPath { get; internal set; } = "";
    }
}
