using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVGame : ProjectBase
    {
        readonly List<Type> ListOfJS;
        public RPGMMVGame()
        {
            ListOfJS = JSBase.GetListOfJSTypes();
        }

        internal override bool Check()
        {
            if (Path.GetExtension(ProjectData.SelectedFilePath) == ".exe")
            {
                if (File.Exists(Path.Combine(ProjectData.SelectedDir, "www", "data", "system.json")))
                {
                    return true;
                }
            }

            return false;
        }

        internal override string Filters()
        {
            return GameExeFilter;
        }

        internal override string Name()
        {
            return "RPG Maker MV";
        }

        internal override string ProjectFolderName()
        {
            return "RPGMakerMV";
        }

        internal override bool IsTestRunEnabled => true;

        internal override bool Open()
        {
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
                string cell = ProjectData.THFilesElementsDatasetInfo.Tables[tind].Rows[rind][0] + string.Empty;
                if (cell.Contains("Code=655") || cell.Contains("Code=355") /*|| ProjectData.THFilesElementsDataset.Tables[tind].TableName.ToUpperInvariant().EndsWith(".JS")*/)
                {
                    return true;
                }
            }
            return false;
        }

        string ParseFileMessage;
        /// <summary>
        /// Parsing the Project files
        /// </summary>
        /// <param name="write">Use Save() instead of Open()</param>
        /// <returns></returns>
        private bool ParseProjectFiles()
        {
            if (ProjectData.OpenFileMode)
            {
                BakRestore();
            }

            ParseFileMessage = ProjectData.SaveFileMode ? T._("write file: ") : T._("opening file: ");
            bool isAnyFileCompleted = false;
            try
            {
                //gamefont.css to be possible to change font
                ProjectData.Main.ProgressInfo(true, ParseFileMessage + "gamefont.css");
                ProjectData.FilePath = Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css");

                try
                {
                    if (ProjectData.SaveFileMode && new GAMEFONTCSS().Save())
                    {
                        isAnyFileCompleted = true;
                    }
                    else if (new GAMEFONTCSS().Open())
                    {
                        isAnyFileCompleted = true;
                    }
                }
                catch
                {
                }

                var hardcodedJS = new HashSet<string>();
                //Proceeed js-files
                foreach (var jsType in ListOfJS)
                {
                    var js = (IUseJSLocationInfo)Activator.CreateInstance(jsType); // create instance of class using JSLocationInfo

                    ProjectData.FilePath = Path.Combine(ProjectData.SelectedDir, "www", "js", js.JSSubfolder, js.JSName);

                    if (!File.Exists(ProjectData.FilePath))
                    {
                        continue;
                    }

                    try
                    {

                        hardcodedJS.Add(js.JSName);//add js to exclude from parsing of other js

                        var jsFormat = (StringFileFormatBase)Activator.CreateInstance(jsType); // create format instance for open or save

                        ProjectData.Main.ProgressInfo(true, ParseFileMessage + js.JSName);

                        if (ProjectData.SaveFileMode && jsFormat.Save())
                        {
                            isAnyFileCompleted = true;
                        }
                        else if (jsFormat.Open())
                        {
                            isAnyFileCompleted = true;
                        }
                    }
                    catch
                    {
                    }
                }

                //hardcoded exclusions
                var skipJSList = new HashSet<string>
                {
                    "ConditionallyCore.js",//translation of text in quotes will make skills not executable
                    //---
                    //----//
                };

                //add exclusions from skipjs file
                SetSkipJSLists(skipJSList);

                //Proceeed other js-files with quotes search
                foreach (var jsFileInfo in Directory.EnumerateFiles(Path.Combine(ProjectData.SelectedGameDir, "www", "js", "plugins"), "*.js"))
                {
                    string jsName = Path.GetFileName(jsFileInfo);

                    if (hardcodedJS.Contains(jsName) || skipJSList.Contains(jsName))
                        continue;

                    ProjectData.Main.ProgressInfo(true, ParseFileMessage + jsName);
                    ProjectData.FilePath = jsFileInfo;

                    if (!File.Exists(ProjectData.FilePath))
                    {
                        continue;
                    }

                    try
                    {
                        if (ProjectData.SaveFileMode && new ZZZOtherJS().Save())
                        {
                            isAnyFileCompleted = true;
                        }
                        else if (new ZZZOtherJS().Open())
                        {
                            isAnyFileCompleted = true;
                        }
                    }
                    catch
                    {
                    }
                }



                //Proceed json-files
                var mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(ProjectData.SelectedDir, "www", "data/")));
                foreach (FileInfo file in mvdatadir.GetFiles("*.json"))
                {
                    try
                    {
                        if (ParseRPGMakerMVjson(file.FullName))
                        {
                            isAnyFileCompleted = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            ProjectData.Main.ProgressInfo(false);
            return isAnyFileCompleted; ;
        }

        /// <summary>
        /// read js file names from all default avalaible paths in <paramref name="SkipJSList"/>
        /// </summary>
        /// <param name="SkipJSList"></param>
        private void SetSkipJSLists(HashSet<string> SkipJSList)
        {
            foreach (var skipjsfilePath in THSettings.RPGMakerMVSkipjsRulesFilesList())
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
            if (!File.Exists(skipjsfilePath))
            {
                return;
            }

            var skipjs = File.ReadAllLines(skipjsfilePath);
            foreach (var line in skipjs)
            {
                var jsfile = line.Trim();
                if (jsfile.Length == 0 || jsfile[0] == ';' || SkipJSList.Contains(jsfile))
                {
                    continue;
                }
                SkipJSList.Add(jsfile);
            }
        }

        private bool ParseRPGMakerMVjson(string FilePath)
        {
            try
            {
                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string Jsonname = Path.GetFileNameWithoutExtension(FilePath); // get json file name

                ProjectData.Main.ProgressInfo(true, ParseFileMessage + Jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                ProjectData.FilePath = FilePath;
                //ret = ReadJson(Jsonname, sPath);

                ret = ProjectData.SaveFileMode ? new JSON().Save() : new JSON().Open();

                return ret;
            }
            catch
            {
                return false;
            }
        }

        internal override bool Save()
        {
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
        readonly string[] BakPaths = new string[]
        {
                @".\www\data",
                @".\www\fonts",
                @".\www\js"
        };

        internal override bool BakCreate()
        {
            BakRestore();

            return BackupRestorePaths(BakPaths);

            //old
            //RestoreFromBakIfNeedData();
            //try
            //{
            //    File.Copy(Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css")
            //        , Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css") + ".bak"
            //        );
            //}
            //catch
            //{

            //}
            //try
            //{
            //    string dataPath = Path.Combine(ProjectData.SelectedDir, "www", "data");
            //    dataPath.CopyAll(dataPath + "_bak");
            //}
            //catch
            //{
            //}
            //foreach (JSBase JS in ListOfJS)
            //{
            //    try
            //    {
            //        string jsPath = Path.Combine(ProjectData.SelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);

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
            var dataPath = Path.Combine(ProjectData.SelectedDir, "www", "data");
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

            if (File.Exists(Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css.bak")))
            {
                try
                {
                    File.Delete(Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css"));
                    File.Move(Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css.bak")
                        , Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css")
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
            string jsPath = Path.Combine(ProjectData.SelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);
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
                if (ProjectData.OnlineTranslationCache == null)
                {
                    ProjectData.OnlineTranslationCache = new FunctionsOnlineCache();
                    ProjectData.OnlineTranslationCache.Read();
                }

                if (_filler == null)
                {
                    _filler = new FillEmptyTablesLinesDictForce();
                    _filler.All();
                }

                var name = Regex.Replace(original, @"^\\n<(.+)>[\s\S]*$", "$1");

                var translatedname = ProjectData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(name);

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
            if (Regex.IsMatch(str, @"<Mini Label: ([^>]+)>"))
            {
                //<Mini Label: \\c[10]バット Lv\\v[981]>
                str = Regex.Match(str, @"<Mini Label: ([^>]+)>").Result("$1");
            }

            str = Regex.Replace(str, @"\\C\[[0-9]{1,3}\]", "");

            return str;
        }

        internal override void CreateMenus()
        {
            var category = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = T._("Project")
            };
            var SkipJSMenuName = T._("Skip JS");
            var SkipJSMenu = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "[" + ProjectData.CurrentProject.Name() + "]" + SkipJSMenuName
            };
            category.DropDownItems.Add(SkipJSMenu);
            SkipJSMenu.Click += RPGMMVGameSkipJSMenu_Click;

            var SkipJSFileOpen = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "[" + ProjectData.CurrentProject.Name() + "]" + SkipJSMenuName + "-->" + T._("Open")
            };
            category.DropDownItems.Add(SkipJSFileOpen);
            SkipJSFileOpen.Click += RPGMMVGameSkipJSFileOpen_Click;

            ProjectData.Main.Invoke((Action)(() => ProjectData.Main.CMSFilesList.Items.Add(category)));
        }

        /// <summary>
        /// open skip js list txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPGMMVGameSkipJSFileOpen_Click(object sender, EventArgs e)
        {
            Process.Start(THSettings.RPGMakerMVSkipjsRulesFilePath());
        }

        /// <summary>
        /// add js file from files list to skipjs rules file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPGMMVGameSkipJSMenu_Click(object sender, System.EventArgs e)
        {
            //read and check the name
            var names = ProjectData.THFilesList.CopySelectedNames();
            if (string.IsNullOrWhiteSpace(names) || names.ToUpperInvariant().IndexOf(".JS") == -1)
            {
                return;
            }

            //read only js names
            var SkipJSList = new HashSet<string>();
            SetSkipJSList(SkipJSList, THSettings.RPGMakerMVSkipjsRulesFilePath());

            //read all list
            List<string> SkipJSOveralList;
            if (SkipJSList.Count == 0 || !File.Exists(THSettings.RPGMakerMVSkipjsRulesFilePath()))
            {
                SkipJSOveralList = new List<string>();
            }
            else
            {
                SkipJSOveralList = File.ReadAllLines(THSettings.RPGMakerMVSkipjsRulesFilePath()).ToList();
            }

            bool changed = false;
            foreach (var jsname in names.SplitToLines())
            {
                if (string.IsNullOrWhiteSpace(jsname) || !string.Equals(Path.GetExtension(jsname), ".js", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                //check if name already exists in list
                if (!SkipJSList.Contains(names) && !SkipJSOveralList.Contains(names))
                {
                    changed = true;
                    SkipJSOveralList.Add(names);
                }
            }

            if (!changed)
            {
                return;
            }

            //write list
            File.WriteAllLines(THSettings.RPGMakerMVSkipjsRulesFilePath(), SkipJSOveralList);
        }

        internal override bool IsValidForTranslation(string inputString)
        {
            return
                !inputString.StartsWith("<TE:") // Plugin TemplateEvent.js , mark of event name for use and must not be translated if name of event was not translated
                ;
        }
    }
}

