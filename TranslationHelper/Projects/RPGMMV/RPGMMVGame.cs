using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Formats.RPGMMV.Other;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict;
using TranslationHelper.Menus.ProjectMenus;
using TranslationHelper.Projects.RPGMMV.Menus;

namespace TranslationHelper.Projects.RPGMMV
{
    internal class RPGMMVGame : ProjectBase
    {
        protected readonly List<Type> ListOfJS;
        public RPGMMVGame()
        {
            ListOfJS = JSBase.GetListOfJSTypes();
        }

        protected virtual bool HasWWWDir { get; } = true;

        internal override bool Check()
        {
            return IsExe() && File.Exists(Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "", "data", "system.json"));
        }

        internal override string Filters => GameExeFilter;

        internal override string Name => "RPG Maker MV";

        internal override string ProjectFolderName => "RPGMakerMV";

        internal override bool IsTestRunEnabled => true;

        protected static string WWWDir;
        public override bool Open()
        {
            WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");
            return ParseProjectFiles();
        }

        internal override bool LineSplitProjectSpecificSkipForTable(DataTable table)
        {
            return table.TableName.ToUpperInvariant().EndsWith(".JS");
        }
        internal override bool LineSplitProjectSpecificSkipForLine(string o, string t, int tind = -1, int rind = -1)
        {
            if (tind != -1 && rind != -1)
            {
                string cell = AppData.CurrentProject.FilesContentInfo.Tables[tind].Rows[rind][0] + string.Empty;
                if (cell.Contains("Code=655") || cell.Contains("Code=355") /*|| ProjectData.THFilesElementsDataset.Tables[tind].TableName.ToUpperInvariant().EndsWith(".JS")*/)
                {
                    return true;
                }
            }
            return false;
        }

        protected string ParseFileMessage;
        /// <summary>
        /// Parsing the Project files
        /// </summary>
        /// <param name="write">Use Save() instead of Open()</param>
        /// <returns></returns>
        private bool ParseProjectFiles()
        {
            if (OpenFileMode) BakRestore();

            ParseFileMessage = SaveFileMode ? T._("write file: ") : T._("opening file: ");
            bool isAnyFileCompleted = false;
            try
            {
                if (ParseFontsCS()) isAnyFileCompleted = true;

                //FormatBase format;
                var hardcodedJS = new HashSet<string>();
                //Proceeed js-files
                if (ParsePlugins(hardcodedJS)) isAnyFileCompleted = true;

                //hardcoded exclusions
                var skipJSList = new HashSet<string>();

                //add exclusions from skipjs file
                SetSkipJSLists(skipJSList);

                //Proceeed other js-files with quotes search
                if (ParseOtherQuotedJsPlugins(hardcodedJS, skipJSList)) isAnyFileCompleted = true;

                //Proceed json-files
                if (ParseJsons()) isAnyFileCompleted = true;

                //other strings
                if (ParseOther()) isAnyFileCompleted = true;
            }
            catch { }

            AppData.Main.ProgressInfo(false);
            return isAnyFileCompleted;
        }

        private bool ParseOther()
        {
            var mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(WWWDir, "data/")));
            return OpenSaveFilesBase(mvdatadir, typeof(ExternMessageCSV), "*.csv");
        }

        protected virtual bool ParseJsons()
        {
            var mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(WWWDir, "data/")));
            return OpenSaveFilesBase(mvdatadir, MVJsonFormats(), MVJsonFormatsMasks());
            //foreach (FileInfo file in mvdatadir.GetFiles("*.json")) try { if (ParseRPGMakerMVjson(file.FullName)) isAnyFileCompleted = true; } catch { }
        }

        protected virtual List<Type> MVJsonFormats()
        {
            return new List<Type>() { typeof(JSON) };
        }

        protected virtual string[] MVJsonFormatsMasks()
        {
            return new[] { "*.json" };
        }

        private bool ParsePlugins(HashSet<string> hardcodedJS)
        {
            bool isAnyFileCompleted = false;
            foreach (var jsType in ListOfJS)
            {
                if (IsTypeExcluded(jsType)) continue;

                var js = (IUseJSLocationInfo)Activator.CreateInstance(jsType); // create instance of class using JSLocationInfo

                var filePath = Path.Combine(WWWDir, "js", js.JSSubfolder, js.JSName);

                if (!File.Exists(filePath)) continue;

                try
                {
                    hardcodedJS.Add(js.JSName);//add js to exclude from parsing of other js

                    var format = (FormatBase)Activator.CreateInstance(jsType); // create format instance for open or save
                    format.FilePath = filePath;

                    AppData.Main.ProgressInfo(true, ParseFileMessage + js.JSName);

                    try
                    {
                        if ((OpenFileMode && format.Open())
                            || (SaveFileMode && format.FileName.HasAnyTranslated() && format.Save()))
                        {
                            isAnyFileCompleted = true;
                        }
                    }
                    catch { }
                }
                catch
                {
                }
            }

            return isAnyFileCompleted;
        }

        protected virtual bool IsTypeExcluded(Type jsType)
        {
            return false;
        }

        private bool ParseOtherQuotedJsPlugins(HashSet<string> hardcodedJS, HashSet<string> skipJSList)
        {
            bool isAnyFileCompleted = false;
            foreach (var jsFileInfo in Directory.EnumerateFiles(Path.Combine(WWWDir, "js", "plugins"), "*.js"))
            {
                string jsName = Path.GetFileName(jsFileInfo);

                if (hardcodedJS.Contains(jsName) || skipJSList.Contains(jsName)) continue;

                AppData.Main.ProgressInfo(true, ParseFileMessage + jsName);

                if (!File.Exists(jsFileInfo)) continue;

                var format = new ZZZOtherJS
                {
                    FilePath = jsFileInfo
                };

                try
                {
                    if ((OpenFileMode && format.Open())
                        || (SaveFileMode && format.FileName.HasAnyTranslated() && format.Save()))
                    {
                        isAnyFileCompleted = true;
                    }
                }
                catch
                {
                }
            }

            return isAnyFileCompleted;
        }

        /// <summary>
        /// gamefont.css to be possible to change font
        /// </summary>
        /// <returns></returns>
        private bool ParseFontsCS()
        {
            AppData.Main.ProgressInfo(true, ParseFileMessage + "gamefont.css");
            var filePath = Path.Combine(WWWDir, "fonts", "gamefont.css");

            if (!File.Exists(filePath)) return false;

            var format = new GAMEFONTCSS
            {
                FilePath = filePath
            };

            try
            {
                if ((OpenFileMode && format.Open())
                    || (SaveFileMode && format.FileName.HasAnyTranslated() && format.Save()))
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
        /// read js file names from all default avalaible paths in <paramref name="SkipJSList"/>
        /// </summary>
        /// <param name="SkipJSList"></param>
        private void SetSkipJSLists(HashSet<string> SkipJSList)
        {
            foreach (var skipjsfilePath in THSettings.RPGMakerMVSkipjsRulesFilesList)
            {
                SetSkipJSList(SkipJSList, skipjsfilePath);
            }
        }

        /// <summary>
        /// read js file names from <paramref name="skipjsfilePath"/> in <paramref name="SkipJSList"/>
        /// </summary>
        /// <param name="SkipJSList"></param>
        /// <param name="skipjsfilePath"></param>
        private static void SetSkipJSList(HashSet<string> SkipJSList, string skipjsfilePath)
        {
            if (!File.Exists(skipjsfilePath)) return;

            var skipjs = File.ReadAllLines(skipjsfilePath);
            foreach (var line in skipjs)
            {
                var jsfile = line.Trim();
                if (jsfile.Length == 0 || jsfile[0] == ';' || SkipJSList.Contains(jsfile)) continue;

                SkipJSList.Add(jsfile);
            }
        }

        private bool ParseRPGMakerMVjson(string filePath)
        {
            try
            {
                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string Jsonname = Path.GetFileNameWithoutExtension(filePath); // get json file name

                AppData.Main.ProgressInfo(true, ParseFileMessage + Jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                //ret = ReadJson(Jsonname, sPath);

                var format = new JSON
                {
                    FilePath = filePath
                };

                ret = SaveFileMode ? (format.FileName.HasAnyTranslated() && format.Save()) : format.Open();

                return ret;
            }
            catch
            {
                return false;
            }
        }

        public override bool Save()
        {
            if(string.IsNullOrWhiteSpace(WWWDir)) WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");
            
            return ParseProjectFiles();
        }

        //private JSBase GetCorrectJSbyName(string tableName)
        //{
        //    foreach (var JS in ListOfJS)
        //    {
        //        if (tableName == JS.JSName)
        //        {
        //            return JS;
        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        /// data, font and js folders
        /// </summary>
        protected virtual string[] BakPaths { get; } = new string[]
        {
                @".\www\data",
                @".\www\fonts",
                @".\www\js"
        };

        internal override bool BakCreate()
        {
            if (string.IsNullOrWhiteSpace(WWWDir)) WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");

            BakRestore();

            return BackupRestorePaths(BakPaths);

            //old
            //RestoreFromBakIfNeedData();
            //try
            //{
            //    File.Copy(Path.Combine(ProjectData.CurrentProject.SelectedDir, "www", "fonts", "gamefont.css")
            //        , Path.Combine(ProjectData.CurrentProject.SelectedDir, "www", "fonts", "gamefont.css") + ".bak"
            //        );
            //}
            //catch
            //{

            //}
            //try
            //{
            //    string dataPath = Path.Combine(ProjectData.CurrentProject.SelectedDir, "www", "data");
            //    dataPath.CopyAll(dataPath + "_bak");
            //}
            //catch
            //{
            //}
            //foreach (JSBase JS in ListOfJS)
            //{
            //    try
            //    {
            //        string jsPath = Path.Combine(ProjectData.CurrentProject.SelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);

            //        RestoreFromBakIfNeedJS(JS);
            //        if (File.Exists(jsPath))
            //        {
            //            File.Copy(jsPath, jsPath + ".bak");
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}
            //return true;
        }

        internal override bool BakRestore()
        {
            if (string.IsNullOrWhiteSpace(WWWDir)) WWWDir = Path.Combine(AppData.CurrentProject.SelectedDir, HasWWWDir ? "www" : "");

            RestoreFromBakIfNeedData();
            foreach (var JS in ListOfJS)
            {
                RestoreFromBakIfNeedJS((JSBase)Activator.CreateInstance(JS));
            }

            return BackupRestorePaths(BakPaths, false);
        }

        /// <summary>
        /// old variant of restore still here because beck compatibility
        /// </summary>
        internal static void RestoreFromBakIfNeedData()
        {
            var dataPath = Path.Combine(WWWDir, "data");
            if (Directory.Exists(dataPath + "_bak"))
            {
                try
                {
                    if (Directory.Exists(dataPath))
                    {
                        Directory.Delete(dataPath, true);
                    }

                    Directory.Move(dataPath + "_bak", dataPath);
                }
                catch
                {
                }
            }

            if (File.Exists(Path.Combine(WWWDir, "fonts", "gamefont.css.bak")))
            {
                try
                {
                    File.Delete(Path.Combine(WWWDir, "fonts", "gamefont.css"));
                    File.Move(Path.Combine(WWWDir, "fonts", "gamefont.css.bak")
                        , Path.Combine(WWWDir, "fonts", "gamefont.css")
                        );
                }
                catch
                {
                }
            }

        }

        /// <summary>
        /// old variant of restore still here because beck compatibility
        /// </summary>
        /// <param name="JS"></param>
        internal static void RestoreFromBakIfNeedJS(JSBase JS)
        {
            string jsPath = Path.Combine(WWWDir, "js", JS.JSSubfolder, JS.JSName);
            if (File.Exists(jsPath + ".bak"))
            {
                try
                {
                    if (File.Exists(jsPath))
                    {
                        File.Delete(jsPath);
                    }

                    File.Move(jsPath + ".bak", jsPath);
                }
                catch
                {
                }
            }
        }

        FillEmptyTablesLinesDictBase _filler;

        /// <summary>
        /// Hardcoded fixes for RPGMakerMV
        /// </summary>
        /// <returns></returns>
        internal override string HardcodedFixes(string original, string translation)
        {
            if (translation.StartsWith(@"\n<>"))
            {
                if (AppData.OnlineTranslationCache == null)
                {
                    AppData.OnlineTranslationCache = new FunctionsOnlineCache();
                    AppData.OnlineTranslationCache.Read();
                }

                if (_filler == null)
                {
                    _filler = new FillEmptyTablesLinesDictForce();
                    _filler.All();
                }

                var name = Regex.Replace(original, @"^\\n<(.+)>[\s\S]*$", "$1");

                var translatedname = AppData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(name);

                if (translatedname.Length > 0)
                {
                    return translation
                        .Remove(3, name.Length)
                        .Insert(3, translatedname);
                }
                else
                {
                    if (_filler.Translations.ContainsKey(name))
                    {
                        return translation
                            .Remove(3, name.Length)
                            .Insert(3, _filler.Translations[name]);
                    }
                    return translation.Insert(3, name);
                }
            }

            return translation;
        }
        internal override string CleanStringForCheck(string str)
        {
            if (str == null) return str;

            if (Regex.IsMatch(str, @"<Mini Label: ([^>]+)>"))
            {
                //<Mini Label: \\c[10]バット Lv\\v[981]>
                str = Regex.Match(str, @"<Mini Label: ([^>]+)>").Result("$1");
            }

            str = Regex.Replace(str, @"\\C\[[0-9]{1,3}\]", "");

            return str;
        }

        internal override List<IProjectMenu> FilesListItemMenusList()
        {
            return new List<IProjectMenu>()
            {
                new AddToSkipJS(),
                new SkipJSFileOpen(),
                new OpenJsonSkipCodesList(),
            };
        }

        internal override bool IsValidForTranslation(string inputString)
        {
            return
                !inputString.StartsWith("<TE:") // Plugin TemplateEvent.js , mark of event name for use and must not be translated if name of event was not translated
                ;
        }
    }
}
