using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus;

namespace TranslationHelper.Projects
{
    public abstract class ProjectBase : IProject, IProjectBackupUser
    {
        protected ProjectBase()
        {
            // set value of the parameter for the project work session
            DontLoadDuplicates = AppSettings.DontLoadDuplicates;

            if (AppData.CurrentProject == null) return;

            if (SaveFileMode && DontLoadDuplicates) TablesLinesDict = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// true - when file open, false - when file writing
        /// </summary>
        public bool OpenFileMode = true;
        /// <summary>
        /// true - when file write, false - when file open
        /// </summary>
        public bool SaveFileMode { get => !OpenFileMode; set => OpenFileMode = !value; }

        /// <summary>
        /// Index of Original column
        /// </summary>
        internal int OriginalColumnIndex = 0;
        /// <summary>
        /// Index of main Translation column
        /// </summary>
        internal int TranslationColumnIndex = 1;

        /// <summary>
        /// main work table data
        /// </summary>
        public DataSet FilesContent { get; set; } = new DataSet();

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

        readonly object TableDataAddLocker = new object();
        /// <summary>
        /// add new <paramref name="tableData"/> in tables list
        /// </summary>
        /// <param name="tableData"></param>
        internal void AddFileData(DataTable tableData)
        {
            lock (TableDataAddLocker)
            {
                if (FilesContent.Tables.Contains(tableData.TableName)) return;

                FilesContent.Tables.Add(tableData);
            }
        }

        readonly object TableInfoAddLocker = new object();
        /// <summary>
        /// add new <paramref name="tableInfo"/> in tables list
        /// </summary>
        /// <param name="tableInfo"></param>
        internal void AddFileInfo(DataTable tableInfo)
        {
            lock (TableInfoAddLocker)
            {
                if (FilesContentInfo.Tables.Contains(tableInfo.TableName)) return;

                FilesContentInfo.Tables.Add(tableInfo);
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
            if (string.IsNullOrWhiteSpace(AppData.SelectedFilePath)) return;

            AppData.CurrentProject.SelectedGameDir = Path.GetDirectoryName(AppData.SelectedFilePath);
            AppData.CurrentProject.SelectedDir = Path.GetDirectoryName(AppData.SelectedFilePath);
            AppData.CurrentProject.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, ProjectName);
        }

        /// <summary>
        /// Name of selected project
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectName => Path.GetFileName(Path.GetDirectoryName(AppData.SelectedFilePath));

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
        internal virtual void PreSaveDB() { }

        /// <summary>
        /// Project's Title prefix
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectTitlePrefix => string.Empty;

        /// <summary>
        /// Project title
        /// </summary>
        /// <returns></returns>
        public abstract string Name { get; }

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
        public abstract bool Open();

        /// <summary>
        /// Save project files
        /// </summary>        
        /// <returns></returns>
        public abstract bool Save();

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
            return HideVARSBase(o, HideVarsBase);
        }

        /// <summary>
        /// Post online translation project's actions
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <returns></returns>
        internal virtual string OnlineTranslationProjectSpecificPosttranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            return RestoreVARS(t);
        }

        /// <summary>
        /// list of variables for hide
        /// </summary>
        internal Dictionary<string, string> HideVarsBase;
        /// <summary>
        /// list of found matches collections
        /// </summary>
        internal List<MatchCollection> HideVARSMatchCollectionsList;
        internal string HideVARSBase(string str, Dictionary<string, string> HideVARSPatterns = null)
        {
            HideVARSPatterns = HideVARSPatterns ?? AppData.CurrentProject.HideVarsBase;

            if (HideVARSPatterns == null || HideVARSPatterns.Count == 0) return str;

            var keyfound = false;
            foreach (var key in HideVARSPatterns.Keys)
            {
                if (str.Contains(key)) { keyfound = true; break; }
            }
            if (!keyfound) return str;

            var mc = Regex.Matches(str, "(" + string.Join(")|(", HideVARSPatterns.Values) + ")");
            if (mc.Count == 0) return str;

            if (HideVARSMatchCollectionsList == null)//init list
                HideVARSMatchCollectionsList = new List<MatchCollection>();

            if (mcArrNum != 0)//reset vars count
                mcArrNum = 0;

            HideVARSMatchCollectionsList.Add(mc);

            for (int m = mc.Count - 1; m >= 0; m--)
            {
                try
                {
                    str = str.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, "{VAR" + m.ToString("000") + "}");
                }
                catch { }
            }

            return str;
        }

        int mcArrNum;

        internal string RestoreVARS(string str)
        {
            if (HideVARSMatchCollectionsList == null || HideVARSMatchCollectionsList.Count == 0 || !str.Contains("VAR") || HideVARSMatchCollectionsList[mcArrNum].Count == 0)
            {
                return str;
            }

            //restore broken vars
            str = Regex.Replace(str, @"\{ ?VAR ?([0-9]{3}) ?\}", "{VAR$1}");

            int mi = 0;
            foreach (Match m in HideVARSMatchCollectionsList[mcArrNum])
            {
                str = str.Replace("{VAR" + mi.ToString("000") + "}", m.Value);
                mi++;
            }
            mcArrNum++;
            if (mcArrNum == HideVARSMatchCollectionsList.Count)
            {
                HideVARSMatchCollectionsList.Clear();
            }
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
    }
}
