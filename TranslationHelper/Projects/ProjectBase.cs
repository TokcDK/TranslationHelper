using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects
{
    internal abstract class ProjectBase
    {
        protected THDataWork thDataWork;

        protected ProjectBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
            if (!string.IsNullOrWhiteSpace(thDataWork.SPath))
            {
                Properties.Settings.Default.THSelectedGameDir = Path.GetDirectoryName(thDataWork.SPath);
                Properties.Settings.Default.THSelectedDir = Path.GetDirectoryName(thDataWork.SPath);
            }
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
        /// <param name="thData"></param>
        /// <returns></returns>
        internal abstract bool Open();

        /// <summary>
        /// open or save project files
        /// </summary>
        /// <returns></returns>
        protected bool OpenSaveFilesBase(DirectoryInfo DirForSearch, FormatBase format, string mask="*")
        {
            var ret = false;
            foreach (var file in DirForSearch.EnumerateFiles(mask, SearchOption.AllDirectories))
            {
                thDataWork.FilePath = file.FullName;
                thDataWork.Main.ProgressInfo(true, thDataWork.OpenFileMode ? T._("Opening") : T._("Saving") + " " + file.Name);
                if (thDataWork.OpenFileMode ? format.Open() : format.Save())
                {
                    ret = true;
                }
            }

            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        /// <summary>
        /// Save project files
        /// </summary>
        /// <param name="thData"></param>
        /// <returns></returns>
        internal abstract bool Save();

        /// <summary>
        /// Project folder name to locate files in DB and Work folders
        /// </summary>
        /// <returns></returns>
        internal virtual string NewlineSymbol => Environment.NewLine;

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
        protected Dictionary<string, string> HideVarsBase;
        /// <summary>
        /// list of found matches collections
        /// </summary>
        internal List<MatchCollection> HideVARSMatchCollectionsList;
        protected string HideVARSBase(string str, Dictionary<string, string> HideVARSPatterns)
        {
            if (HideVARSPatterns == null)
            {
                return string.Empty;
            }

            var keyfound = false;
            foreach (var key in HideVARSPatterns.Keys)
            {
                if (!str.Contains(key))
                {
                    keyfound = true;
                    break;
                }
            }
            if (!keyfound)
            {
                return string.Empty;
            }

            List<string> Patterns = new List<string>();
            foreach (var pattern in HideVARSPatterns.Values)
            {
                Patterns.Add("(" + pattern + ")");
            }

            var mc = Regex.Matches(str, string.Join("|", Patterns));
            if (mc.Count == 0)
            {
                return string.Empty;
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
                    str = str.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, "{VAR" + m + "}");
                }
                catch (System.ArgumentOutOfRangeException)
                {

                }
            }

            return str;
        }

        int mcArrNum;
        protected string RestoreVARS(string str)
        {
            if (HideVARSMatchCollectionsList == null || HideVARSMatchCollectionsList.Count == 0 || !str.Contains("VAR") || HideVARSMatchCollectionsList[mcArrNum].Count == 0)
            {
                return string.Empty;
            }

            //restore broken vars
            foreach (Match p in Regex.Matches(str, @"\{ ?VAR ?[0-9]{1,2} ?\}"))
            {
                str = str.Replace(p.Value, p.Value.Replace(" ", string.Empty));
            }

            int mi = 0;
            foreach (Match m in HideVARSMatchCollectionsList[mcArrNum])
            {
                str = str.Replace("{VAR" + mi + "}", m.Value);
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
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<ProjectBase> GetListOfProjects(THDataWork thDataWork)
        {
            //ProjectsList = new List<ProjectBase>()
            //{
            //    new RPGMTransPatch(this)
            //    ,
            //    new RPGMGame(this)
            //    ,
            //    new RPGMMVGame(this)
            //    ,
            //    new KiriKiriGame(this)
            //    ,
            //    new Raijin7Game(this)
            //    ,
            //    new HowToMakeTrueSlavesRiseofaDarkEmpire(this)
            //};

            //https://stackoverflow.com/a/5411981
            //Get all inherited classes of an abstract class
            IEnumerable<ProjectBase> SubclassesOfProjectBase = typeof(ProjectBase)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ProjectBase)) && !t.IsAbstract)
            .Select(t => (ProjectBase)Activator.CreateInstance(t, thDataWork));

            return (from ProjectBase SubClass in SubclassesOfProjectBase
                    select SubClass).ToList();
        }

        internal void FillTHFilesElementsDictionary()
        {
            if (thDataWork.TablesLinesDict == null || thDataWork.TablesLinesDict.Count == 0)
            {
                foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string orig;
                        if (!string.IsNullOrEmpty(orig = row[0] + string.Empty) && !thDataWork.TablesLinesDict.ContainsKey(orig))
                        {
                            thDataWork.TablesLinesDict.Add(orig, row[1] + string.Empty);
                        }
                    }
                }
            }
        }

        internal void FillTablesLinesDict()
        {
            bool notnull;
            if ((notnull = thDataWork.TablesLinesDict != null) && thDataWork.TablesLinesDict.Count > 0)
            {
                return;
            }
            else if (!notnull)
            {
                thDataWork.TablesLinesDict = new Dictionary<string, string>();
            }

            foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    string orig;
                    string trans;
                    if (!string.IsNullOrWhiteSpace(orig = row[0] + string.Empty) && !string.IsNullOrEmpty(trans = row[1] + string.Empty))
                    {
                        thDataWork.TablesLinesDict.AddTry(orig, trans);

                        if (!trans.StartsWith(@"\n<>") && Regex.IsMatch(orig, @"\\n<.+>[\s\S]*$"))
                        {
                            orig = Regex.Replace(orig, @"\\n<(.+)>[\s\S]*$", "$1");
                            trans = Regex.Replace(trans, @"\\n<(.+)>[\s\S]*$", "$1");
                            if (orig != trans)
                            {
                                thDataWork.TablesLinesDict.AddTry(
                                orig
                                ,
                                trans
                                );
                            }
                        }
                    }
                }
            }
        }

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
            return false;
        }

        /// <summary>
        /// Will restore made buckup of project translating original files<br/>if any code exit here and buckup exists<br/>else will return false
        /// </summary>
        /// <returns></returns>
        internal virtual bool BakRestore()
        {
            return false;
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
                        Directory.Move(dir, dir + ".tmp");
                    }
                    Directory.Move(dir + ".bak", dir);
                    if (tmp && Directory.Exists(dir + ".tmp") && Directory.Exists(dir))
                    {
                        Directory.Delete(dir + ".tmp");
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
        protected static bool RestoreFile(string file)
        {
            try
            {
                if (File.Exists(file + ".bak"))
                {
                    bool tmp = false;
                    if (File.Exists(file))
                    {
                        tmp = true;
                        File.Move(file, file + ".tmp");
                    }
                    File.Move(file + ".bak", file);
                    if (tmp && File.Exists(file + ".tmp") && File.Exists(file))
                    {
                        File.Delete(file + ".tmp");
                        return true;
                    }
                    else if (!tmp && File.Exists(file))
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
        /// make buckup .bak copy of selected paths
        /// </summary>
        /// <param name="paths">file paths</param>
        /// <param name="bak">true = backup, false = restore</param>
        /// <returns>true if was processed atleast one file\dir</returns>
        protected static bool BuckupRestorePaths(string[] paths, bool bak = true)
        {

            if (paths == null || paths.Length == 0)
            {
                return false;
            }

            var ret = false;

            foreach (var path in paths)
            {
                var target = path.EndsWith(".bak") ? path.Remove(path.Length - 4, 4) : path;
                if (bak)
                {
                    if ((File.Exists(target) && BuckupFile(target)) || (Directory.Exists(target) && BuckupDir(target)))
                    {
                        ret = true;
                    }
                }
                else
                {
                    if ((File.Exists(target + ".bak") && RestoreFile(target)) || (Directory.Exists(target + ".bak") && RestoreDir(target)))
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }
        /// <summary>
        /// make buckup .bak copy of selected dir
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        protected static bool BuckupDir(string dir)
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
        protected static bool BuckupFile(string[] filePaths)
        {
            var ret = false;
            foreach (var file in filePaths)
            {
                if (BuckupFile(file))
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
        protected static bool BuckupFile(string file)
        {
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
                    return true;
                }
            }
            catch
            {
            }
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
    }
}
