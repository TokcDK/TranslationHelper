﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Functions;
using TranslationHelper.Menus.ProjectMenus;

namespace TranslationHelper.Projects
{
    internal abstract class ProjectBase
    {
        protected ProjectBase()
        {
            DontLoadDuplicates = Properties.Settings.Default.DontLoadDuplicates; // set value of the parameter for the project work session

            if (ProjectData.SaveFileMode && DontLoadDuplicates)
            {
                TablesLinesDict = new ConcurrentDictionary<string, string>();
            }
        }

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
                if (!FilesContent.Tables.Contains(tableData.TableName))
                {
                    FilesContent.Tables.Add(tableData);
                }
                else
                {

                }
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
                if (!FilesContentInfo.Tables.Contains(tableInfo.TableName))
                {
                    FilesContentInfo.Tables.Add(tableInfo);
                }
                else
                {

                }
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

        /// <summary>
        /// Current parsing format
        /// </summary>
        public FormatBase CurrentFormat;

        /// <summary>
        /// set here som vars before open or kind of
        /// </summary>
        public virtual void Init()
        {
            if (!string.IsNullOrWhiteSpace(ProjectData.SelectedFilePath))
            {
                ProjectData.CurrentProject.SelectedGameDir = Path.GetDirectoryName(ProjectData.SelectedFilePath);
                ProjectData.CurrentProject.SelectedDir = Path.GetDirectoryName(ProjectData.SelectedFilePath);
                ProjectData.CurrentProject.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath(), this.ProjectFolderName(), ProjectName());
            }
        }

        /// <summary>
        /// Name of selected project
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectName()
        {
            return Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath));
        }

        /// <summary>
        /// return if selected file of project is exe
        /// </summary>
        /// <returns></returns>
        protected static bool IsExe()
        {
            return Path.GetExtension(ProjectData.SelectedFilePath).ToUpperInvariant() == ".EXE";
        }

        /// <summary>
        /// Conditions to detect on open
        /// </summary>
        /// <returns></returns>
        internal abstract bool Check();

        /// <summary>
        /// exe files *.exe
        /// </summary>
        protected static string GameExeFilter { get => "Game execute|\"*.exe\""; }
        /// <summary>
        /// Project's filter for fileopen dialog
        /// </summary>
        /// <returns></returns>
        internal virtual string Filters()
        {
            return string.Empty;
        }

        /// <summary>
        /// executed before DB will be saved
        /// </summary>
        internal virtual void PreSaveDB()
        {
        }

        /// <summary>
        /// Project's Title prefix
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectTitlePrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// Project title
        /// </summary>
        /// <returns></returns>
        internal abstract string Name();

        /// <summary>
        /// Project folder name to locate files in DB and Work folders
        /// </summary>
        /// <returns></returns>
        internal virtual string ProjectFolderName()
        {
            return "Other";
        }

        /// <summary>
        /// Returns project's DB file name for save/load
        /// </summary>
        /// <returns></returns>
        internal virtual string GetProjectDBFileName()
        {
            return string.Empty;
        }

        /// <summary>
        /// Open project files
        /// </summary>        
        /// <returns></returns>
        internal abstract bool Open();

        /// <summary>
        /// Save project files
        /// </summary>        
        /// <returns></returns>
        internal abstract bool Save();

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(string DirForSearch, List<Type> formatType, string[] masks = null, bool Newest = false)
        {
            return OpenSaveFilesBase(new DirectoryInfo(DirForSearch), formatType, masks, Newest);
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
        /// <param name="Newest"></param>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(DirectoryInfo DirForSearch, List<Type> format, string[] masks = null, bool Newest = false)
        {
            if (masks == null || format == null || format.Count != masks.Length)
            {
                return false;
            }

            var ret = false;
            for (int i = 0; i < masks.Length; i++)
            {
                if (OpenSaveFilesBase(DirForSearch, format[i], masks[i], Newest))
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(DirectoryInfo DirForSearch, Type formatType, string mask = "*", bool Newest = false, string[] exclusions = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            //if (mask == "*")
            //{
            //    mask += format.Ext();
            //}

            if (!DirForSearch.Exists)
            {
                return false;
            }

            exclusions = exclusions ?? new[] { ".bak" };//set to skip bat if exclusions is null

            var ret = false;
            var existsTables = ProjectData.CurrentProject.FilesContent.Tables;
            var filesList = Newest ? GetNewestFilesList(DirForSearch, mask) : DirForSearch.EnumerateFiles(mask, searchOption);
            Parallel.ForEach(filesList, file =>
            {
                if (/*exclusions != null &&*/ file.FullName.ContainsAnyFromArray(exclusions))//skip exclusions
                    return;

                //ProjectData.FilePath = file.FullName;

                var format = (FormatBase)Activator.CreateInstance(formatType); // create instance of format
                format.FilePath = file.FullName;
                if (!string.IsNullOrWhiteSpace(format.Ext()) && file.Extension != format.Ext()) // check extension for case im mask was "*.*" or kind of
                {
                    return;
                }

                if (ProjectData.SaveFileMode && existsTables.Contains(format.TableName())) // check if exist table has any translated
                {
                    if (!format.TableName().HasAnyTranslated())
                    {
                        return;
                    }
                }

                ProjectData.Main.ProgressInfo(true, (ProjectData.OpenFileMode ? T._("Opening") : T._("Saving")) + " " + file.Name);
                if (ProjectData.OpenFileMode ? format.Open() : format.Save())
                {
                    ret = true;
                }
            });

            ProjectData.Main.ProgressInfo(false);

            return ret;
        }
        protected static List<FileInfo> GetNewestFilesList(DirectoryInfo dir, string mask = "*.*")
        {
            var newestfiles = new Dictionary<string, FileInfo>();

            foreach (var file in dir.EnumerateFiles(mask, SearchOption.AllDirectories))
            {
                var name = file.Name;
                bool contains = newestfiles.ContainsKey(name);
                if (contains)
                {
                    if (file.LastWriteTime <= newestfiles[name].LastWriteTime)
                    {
                        continue;
                    }
                    else
                    {
                        newestfiles[name] = file;
                    }
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
            HideVARSPatterns = HideVARSPatterns ?? ProjectData.CurrentProject.HideVarsBase;

            if (HideVARSPatterns == null || HideVARSPatterns.Count == 0)
            {
                return str;
            }

            var keyfound = false;
            foreach (var key in HideVARSPatterns.Keys)
            {
                if (str.Contains(key))
                {
                    keyfound = true;
                    break;
                }
            }
            if (!keyfound)
            {
                return str;
            }

            var mc = Regex.Matches(str, "(" + string.Join(")|(", HideVARSPatterns.Values) + ")");
            if (mc.Count == 0)
            {
                return str;
            }

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
                catch (System.ArgumentOutOfRangeException)
                {

                }
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
        /// Get all types of inherited classes of ProjectBase class
        /// </summary>
        /// <returns></returns>
        internal static List<Type> GetListOfProjectTypes()
        {
            return GetListOfSubClasses.Inherited.GetListOfInheritedTypes(typeof(ProjectBase));
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<ProjectBase> GetListOfProjects()
        {
            return GetListOfSubClasses.Inherited.GetListOfinheritedSubClasses<ProjectBase>();
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
        /// Must make buckup of project translating original files<br/>if any code exit here else will return false
        /// </summary>
        /// <returns></returns>
        internal virtual bool BakCreate()
        {
            return BackupRestorePaths(new[] { ProjectData.SelectedFilePath, ProjectData.FilePath });
        }

        /// <summary>
        /// Will restore made buckup of project translating original files<br/>if any code exit here and buckup exists<br/>else will return false
        /// </summary>
        /// <returns></returns>
        internal virtual bool BakRestore()
        {
            return BackupRestorePaths(new[] { ProjectData.SelectedFilePath, ProjectData.FilePath }, false);
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

                if (Directory.Exists(dir + ".bak"))
                {
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
                }
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
            ProjectData.Main.ProgressInfo(true, T._("restore") + ":" + Path.GetFileName(file));

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
                        ProjectData.Main.ProgressInfo(false);
                        return true;
                    }
                    else if (!tmp && File.Exists(file))
                    {
                        ProjectData.Main.ProgressInfo(false);
                        return true;
                    }
                }
            }
            catch
            {
            }

            ProjectData.Main.ProgressInfo(false);
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

            var bakuped = new HashSet<string>();

            foreach (var subpath in paths)
            {
                if (string.IsNullOrWhiteSpace(subpath)) continue;

                string path = (subpath.StartsWith(@".\") || subpath.StartsWith(@"..\")) ? Path.GetFullPath(Path.Combine(ProjectData.CurrentProject.SelectedDir, subpath)) : subpath;

                if (string.IsNullOrWhiteSpace(path) || bakuped.Contains(path)) continue;

                bakuped.Add(path);//add path to backuped

                var target = path.EndsWith(".bak") ? path.Remove(path.Length - 4, 4) : path;
                if (bak)
                {
                    if ((File.Exists(target) && BackupFile(target)) || (Directory.Exists(target) && BackupDir(target))) 
                        ret = true;
                }
                else if((File.Exists(target + ".bak") && RestoreFile(target)) || (Directory.Exists(target + ".bak") && RestoreDir(target)))
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
                if (Directory.Exists(dir + ".bak"))
                {
                    RestoreDir(dir);
                }
                if (!Directory.Exists(dir + ".bak") && Directory.Exists(dir))
                {
                    dir.CopyAll(dir + ".bak");
                }
                if (Directory.Exists(dir + ".bak"))
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
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
                if (BackupFile(file))
                {
                    ret = true;
                }
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
            ProjectData.Main.ProgressInfo(true, T._("backup") + ":" + Path.GetFileName(file));

            try
            {
                if (File.Exists(file + ".bak"))
                {
                    RestoreFile(file);
                }
                if (File.Exists(file) && !File.Exists(file + ".bak"))
                {
                    File.Copy(file, file + ".bak");
                }
                if (File.Exists(file + ".bak"))
                {
                    ProjectData.Main.ProgressInfo(false);
                    return true;
                }
            }
            catch
            {
            }
            ProjectData.Main.ProgressInfo(false);
            return false;
        }

        /// <summary>
        /// specific project's row issue check
        /// Using in row issue checking from search options
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal virtual bool CheckForRowIssue(DataRow row)
        {
            return false;
        }

        /// <summary>
        /// true if <paramref name="inputString"/> is valid for translation
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal virtual bool IsValidForTranslation(string inputString)
        {
            return true;
        }

        /// <summary>
        /// here can be set actions to execute after write of translation
        /// </summary>
        internal virtual void AfterTranslationWriteActions()
        {
            System.Diagnostics.Process.Start("explorer.exe", ProjectData.CurrentProject.SelectedDir);
        }

        internal virtual List<IProjectMenu> FilesListItemMenusList()
        {
            return new List<IProjectMenu>();
        }

        internal virtual List<IProjectMenu> GridItemMenusList()
        {
            return new List<IProjectMenu>();
        }

        /// <summary>
        /// project specific string file's line modification
        /// </summary>
        /// <param name="line"></param>
        internal virtual void ReadLineMod(ref string line)
        {
        }
    }
}
