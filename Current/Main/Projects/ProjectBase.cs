using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Functions;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Projects
{
    public abstract class ProjectBase : IProject
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
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(string DirForSearch, List<Type> formatType, string[] masks = null, bool getNewestFiles = false)
        {
            return OpenSaveFilesBase(new DirectoryInfo(DirForSearch), formatType, masks, getNewestFiles);
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(string DirForSearch, Type formatType, string mask = "*")
        {
            return OpenSaveFilesBase(new DirectoryInfo(DirForSearch), formatType, mask);
        }

        /// <summary>
        /// open save project files for several file formats/masks
        /// </summary>
        /// <param name="DirForSearch"></param>
        /// <param name="format"></param>
        /// <param name="masks"></param>
        /// <param name="newest"></param>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(DirectoryInfo DirForSearch, List<Type> format, string[] masks = null, bool getNewestFiles = false)
        {
            if (masks == null || format == null || (masks.Length != 1 && format.Count != masks.Length)) return false;

            var ret = false;
            bool hasSingleMask = masks.Length == 1;
            for (int i = 0; i < masks.Length; i++)
            {
                var mask = hasSingleMask ? masks[0] : masks[i];
                if (OpenSaveFilesBase(DirForSearch, format[i], mask, getNewestFiles)) ret = true;
            }

            return ret;
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(DirectoryInfo DirForSearch, Type formatType, string mask = "*", bool getNewestFiles = false, string[] exclusions = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            //if (mask == "*")
            //{
            //    mask += format.Ext();
            //}

            if (!DirForSearch.Exists) return false;

            exclusions = exclusions ?? new[] { ".bak" };//set to skip bat if exclusions is null

            var ret = false;
            var existsTables = AppData.CurrentProject.FilesContent.Tables;
            var filesList = getNewestFiles ? GetNewestFilesList(DirForSearch, mask) : DirForSearch.EnumerateFiles(mask, searchOption);
            Parallel.ForEach(filesList, file =>
            {
                //skip exclusions
                if (/*exclusions != null &&*/ file.FullName.ContainsAnyFromArray(exclusions)) return;

                //ProjectData.FilePath = file.FullName;

                var format = (FormatBase)Activator.CreateInstance(formatType); // create instance of format
                format.FilePath = file.FullName;

                // check extension for case im mask was "*.*" or kind of
                if (!string.IsNullOrWhiteSpace(format.Extension) && file.Extension != format.Extension) return;

                // check if exist table has any translated
                if (SaveFileMode && existsTables.Contains(format.FileName) && !format.FileName.HasAnyTranslated()) return;

                AppData.Main.ProgressInfo(true, (OpenFileMode ? T._("Opening") : T._("Saving")) + " " + file.Name);

                bool isOpenSuccess = false;
                try
                {
                    if (OpenFileMode ? (isOpenSuccess = format.Open()) : format.Save()) ret = true;
                }
                catch (Exception ex)
                { 
                }

                // add to bak paths for default backup
                if (OpenFileMode && isOpenSuccess && !BakPaths.Contains(file.FullName)) BakPaths.Add(file.FullName);
            });

            AppData.Main.ProgressInfo(false);

            return ret;
        }
        protected static List<FileInfo> GetNewestFilesList(DirectoryInfo dir, string mask = "*.*")
        {
            var newestfiles = new Dictionary<string, FileInfo>();

            foreach (var file in dir.EnumerateFiles(mask, SearchOption.AllDirectories))
            {
                var name = file.Name;
                bool isAlreadyContains = newestfiles.ContainsKey(name);
                if (isAlreadyContains)
                {
                    if (file.LastWriteTime <= newestfiles[name].LastWriteTime) continue;

                    newestfiles[name] = file;
                }
                else
                {
                    newestfiles.Add(name, file);
                }
            }

            return newestfiles.Values.ToList();
        }

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
        protected virtual List<string> BakPaths { get; set; } = new List<string>();

        /// <summary>
        /// Must make buckup of project translating original files<br/>if any code exit here else will return false
        /// </summary>
        /// <returns></returns>
        internal virtual bool BakCreate()
        {
            return BackupRestorePaths(BakPaths);
        }

        /// <summary>
        /// Will restore made buckup of project translating original files<br/>if any code exit here and buckup exists<br/>else will return false
        /// </summary>
        /// <returns></returns>
        internal virtual bool BakRestore()
        {
            return BackupRestorePaths(BakPaths, false);
        }

        /// <summary>
        /// restore selected dir from it buckup .bak copy
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        protected static bool RestoreDir(string dir)
        {
            try
            {

                if (!Directory.Exists(dir + ".bak")) return false;

                bool tmp = false;
                if (Directory.Exists(dir))
                {
                    tmp = true;

                    if (Directory.Exists(dir + ".tmp"))
                    {
                        new DirectoryInfo(dir + ".tmp").Attributes = FileAttributes.Normal;
                        Directory.Delete(dir + ".tmp", true);
                    }

                    Directory.Move(dir, dir + ".tmp");
                }
                Directory.Move(dir + ".bak", dir);
                if (tmp && Directory.Exists(dir + ".tmp") && Directory.Exists(dir))
                {
                    new DirectoryInfo(dir + ".tmp").Attributes = FileAttributes.Normal;
                    Directory.Delete(dir + ".tmp", true);
                    return true;
                }
                else if (!tmp && Directory.Exists(dir))
                {
                    return true;
                }
                return false;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// restore selected file from it buckup .bak copy
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        protected bool RestoreFile(string file)
        {
            AppData.Main.ProgressInfo(true, T._("restore") + ":" + Path.GetFileName(file));

            try
            {
                if (File.Exists(file + ".bak"))
                {
                    bool tmp = false;
                    if (File.Exists(file))
                    {
                        tmp = true;

                        if (File.Exists(file + ".tmp"))
                        {
                            new FileInfo(file + ".tmp").Attributes = FileAttributes.Normal;
                            File.Delete(file + ".tmp");
                        }

                        File.Move(file, file + ".tmp");
                    }
                    File.Move(file + ".bak", file);
                    if (tmp && File.Exists(file + ".tmp") && File.Exists(file))
                    {
                        new FileInfo(file + ".tmp").Attributes = FileAttributes.Normal;
                        File.Delete(file + ".tmp");
                        AppData.Main.ProgressInfo(false);
                        return true;
                    }
                    else if (!tmp && File.Exists(file))
                    {
                        AppData.Main.ProgressInfo(false);
                        return true;
                    }
                }
            }
            catch
            {
            }

            AppData.Main.ProgressInfo(false);
            return false;
        }

        /// <summary>
        /// make buckup .bak copy of selected paths
        /// </summary>
        /// <param name="paths">file paths</param>
        /// <param name="bak">true = backup, false = restore</param>
        /// <returns>true if was processed atleast one file\dir</returns>
        protected bool BackupRestorePaths(IEnumerable<string> paths, bool bak = true)
        {
            if (paths == null) return false;

            var ret = false;

            var added = new HashSet<string>();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;

                string fullPath = (path.StartsWith(@".\") || path.StartsWith(@"..\")) ? Path.GetFullPath(Path.Combine(AppData.CurrentProject.SelectedDir, path)) : path;

                if (string.IsNullOrWhiteSpace(fullPath) || added.Contains(fullPath)) continue;

                added.Add(fullPath);//add path to backuped

                var target = fullPath.EndsWith(".bak") ? fullPath.Remove(fullPath.Length - 4, 4) : fullPath;
                if (bak)
                {
                    if ((File.Exists(target) && BackupFile(target)) || (Directory.Exists(target) && BackupDir(target)))
                        ret = true;
                }
                else if ((File.Exists(target + ".bak") && RestoreFile(target)) || (Directory.Exists(target + ".bak") && RestoreDir(target)))
                {
                    ret = true;
                }
            }

            return ret;
        }
        /// <summary>
        /// make buckup .bak copy of selected dir
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        protected static bool BackupDir(string dir)
        {
            try
            {
                if (Directory.Exists(dir + ".bak")) RestoreDir(dir);

                if (!Directory.Exists(dir + ".bak") && Directory.Exists(dir)) dir.CopyAll(dir + ".bak");
            }
            catch
            {
            }
            return Directory.Exists(dir + ".bak");
        }

        /// <summary>
        /// make buckup .bak copy of selected files
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        protected bool BackupFile(string[] filePaths)
        {
            var ret = false;
            foreach (var file in filePaths)
            {
                if (!BackupFile(file)) continue;

                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// make buckup .bak copy of selected file
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        protected bool BackupFile(string file)
        {
            AppData.Main.ProgressInfo(true, T._("backup") + ":" + Path.GetFileName(file));

            try
            {
                if (File.Exists(file + ".bak")) RestoreFile(file);

                if (File.Exists(file) && !File.Exists(file + ".bak")) File.Copy(file, file + ".bak");
            }
            catch
            {
            }
            AppData.Main.ProgressInfo(false);
            return File.Exists(file + ".bak");
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
        internal virtual void AfterTranslationWriteActions() { System.Diagnostics.Process.Start("explorer.exe", AppData.CurrentProject.SelectedDir); }

        /// <summary>
        /// Main menus which need to be added for the project
        /// </summary>
        /// <returns></returns>
        internal virtual List<IMainMenuItem> MainMenuItemMenusList() { return new List<IMainMenuItem>(); }
        /// <summary>
        /// Files list item menus which must be added for the project
        /// </summary>
        /// <returns></returns>
        internal virtual List<IFileListMenuItem> FilesListItemMenusList() { return new List<IFileListMenuItem>(); }
        /// <summary>
        /// FileRow menus which must be added for the project
        /// </summary>
        /// <returns></returns>
        internal virtual List<IFileRowMenuItem> FileRowItemMenusList() { return new List<IFileRowMenuItem>(); }
    }
}
