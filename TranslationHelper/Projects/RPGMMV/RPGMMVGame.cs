using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Functions;

namespace TranslationHelper.Projects.RPGMMV
{
    class RpgmmvGame : ProjectBase
    {
        readonly List<Type> _listOfJs;
        public RpgmmvGame()
        {
            _listOfJs = JsBase.GetListOfJsTypes();
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
                string cell = ProjectData.ThFilesElementsDatasetInfo.Tables[tind].Rows[rind][0] + string.Empty;
                if (cell.Contains("Code=655") || cell.Contains("Code=355") /*|| ProjectData.THFilesElementsDataset.Tables[tind].TableName.ToUpperInvariant().EndsWith(".JS")*/)
                {
                    return true;
                }
            }
            return false;
        }

        string _parseFileMessage;
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

            _parseFileMessage = ProjectData.SaveFileMode ? T._("write file: ") : T._("opening file: ");
            bool isAnyFileCompleted = false;
            try
            {
                //gamefont.css to be possible to change font
                ProjectData.Main.ProgressInfo(true, _parseFileMessage + "gamefont.css");
                ProjectData.FilePath = Path.Combine(ProjectData.SelectedDir, "www", "fonts", "gamefont.css");

                try
                {
                    if (ProjectData.SaveFileMode && new Gamefontcss().Save())
                    {
                        isAnyFileCompleted = true;
                    }
                    else if (new Gamefontcss().Open())
                    {
                        isAnyFileCompleted = true;
                    }
                }
                catch
                {
                }

                var hardcodedJs = new HashSet<string>();
                //Proceeed js-files
                foreach (var jsType in _listOfJs)
                {
                    var js = (IUseJsLocationInfo)Activator.CreateInstance(jsType); // create instance of class using JSLocationInfo

                    ProjectData.FilePath = Path.Combine(ProjectData.SelectedDir, "www", "js", js.JsSubfolder, js.JsName);

                    if (!File.Exists(ProjectData.FilePath))
                    {
                        continue;
                    }

                    try
                    {

                        hardcodedJs.Add(js.JsName);//add js to exclude from parsing of other js

                        var jsFormat = (FormatBase)Activator.CreateInstance(jsType); // create format instance for open or save

                        ProjectData.Main.ProgressInfo(true, _parseFileMessage + js.JsName);

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
                var skipJsList = new HashSet<string>
                {
                    "ConditionallyCore.js",//translation of text in quotes will make skills not executable
                    //---
                    //----//
                };

                //add exclusions from skipjs file
                SetSkipJsLists(skipJsList);

                //Proceeed other js-files with quotes search
                foreach (var jsFileInfo in Directory.EnumerateFiles(Path.Combine(ProjectData.SelectedGameDir, "www", "js", "plugins"), "*.js"))
                {
                    string jsName = Path.GetFileName(jsFileInfo);

                    if (hardcodedJs.Contains(jsName) || skipJsList.Contains(jsName))
                        continue;

                    ProjectData.Main.ProgressInfo(true, _parseFileMessage + jsName);
                    ProjectData.FilePath = jsFileInfo;

                    if (!File.Exists(ProjectData.FilePath))
                    {
                        continue;
                    }

                    try
                    {
                        if (ProjectData.SaveFileMode && new ZzzOtherJs().Save())
                        {
                            isAnyFileCompleted = true;
                        }
                        else if (new ZzzOtherJs().Open())
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
                        if (ParseRpgMakerMVjson(file.FullName))
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

        private void SetSkipJsLists(HashSet<string> skipJsList)
        {
            foreach (var skipjsfilePath in ThSettings.RpgMakerMvSkipjsRulesFilesList())
            {
                SetSkipJsList(skipJsList, skipjsfilePath);
            }
        }

        private void SetSkipJsList(HashSet<string> skipJsList, string skipjsfilePath)
        {
            if (File.Exists(skipjsfilePath))
            {
                var skipjs = File.ReadAllLines(skipjsfilePath);
                foreach (var line in skipjs)
                {
                    var jsfile = line.Trim();
                    if (jsfile.Length == 0 || jsfile[0] == ';' || skipJsList.Contains(jsfile))
                    {
                        continue;
                    }
                    skipJsList.Add(jsfile);
                }

            }
        }

        private bool ParseRpgMakerMVjson(string filePath)
        {
            try
            {
                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string jsonname = Path.GetFileNameWithoutExtension(filePath); // get json file name

                ProjectData.Main.ProgressInfo(true, _parseFileMessage + jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                ProjectData.FilePath = filePath;
                //ret = ReadJson(Jsonname, sPath);

                ret = ProjectData.SaveFileMode ? new Json().Save() : new Json().Open();

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
        readonly string[] _bakPaths = new string[]
        {
                @".\www\data",
                @".\www\fonts",
                @".\www\js"
        };

        internal override bool BakCreate()
        {
            BakRestore();

            return BackupRestorePaths(_bakPaths);

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
            foreach (var js in _listOfJs)
            {
                RestoreFromBakIfNeedJs((JsBase)Activator.CreateInstance(js));
            }

            return BackupRestorePaths(_bakPaths, false);
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
        /// <param name="js"></param>
        internal static void RestoreFromBakIfNeedJs(JsBase js)
        {
            string jsPath = Path.Combine(ProjectData.SelectedDir, "www", "js", js.JsSubfolder, js.JsName);
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

                FillTablesLinesDict();

                var name = Regex.Replace(original, @"^\\n<(.+)>[\s\S]*$", "$1");

                var translatedname = ProjectData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(name);

                if (translatedname.Length > 0)
                {
                    return translation.Insert(3, translatedname);
                }
                else
                {
                    if (ProjectData.TablesLinesDict.ContainsKey(name))
                    {
                        return translation.Insert(3, ProjectData.TablesLinesDict[name]);
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
            var skipJsMenuName = T._("Skip JS");
            var skipJsMenu = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "[" + ProjectData.CurrentProject.Name() + "]" + skipJsMenuName
            };
            category.DropDownItems.Add(skipJsMenu);
            skipJsMenu.Click += RPGMMVGameSkipJSMenu_Click;

            var skipJsFileOpen = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "[" + ProjectData.CurrentProject.Name() + "]" + skipJsMenuName + "-->" + T._("Open")
            };
            category.DropDownItems.Add(skipJsFileOpen);
            skipJsFileOpen.Click += RPGMMVGameSkipJSFileOpen_Click;

            ProjectData.Main.Invoke((Action)(() => ProjectData.Main.CMSFilesList.Items.Add(category)));
        }

        /// <summary>
        /// open skip js list txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPGMMVGameSkipJSFileOpen_Click(object sender, EventArgs e)
        {
            Process.Start(ThSettings.RpgMakerMvSkipjsRulesFilePath());
        }

        /// <summary>
        /// add js file from files list to skipjs rules file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPGMMVGameSkipJSMenu_Click(object sender, System.EventArgs e)
        {
            //read and check the name
            var name = ProjectData.Main.GetFilesListSelectedName();
            if (string.IsNullOrWhiteSpace(name) || !name.ToUpperInvariant().EndsWith(".JS"))
            {
                return;
            }

            //read only js names
            var skipJsList = new HashSet<string>();
            SetSkipJsList(skipJsList, ThSettings.RpgMakerMvSkipjsRulesFilePath());

            //read all list
            List<string> skipJsOveralList;
            if (skipJsList.Count == 0 || !File.Exists(ThSettings.RpgMakerMvSkipjsRulesFilePath()))
            {
                skipJsOveralList = new List<string>();
            }
            else
            {
                skipJsOveralList = File.ReadAllLines(ThSettings.RpgMakerMvSkipjsRulesFilePath()).ToList();
            }

            //check if name already exists in list
            if (!skipJsList.Contains(name))
            {
                skipJsOveralList.Add(name);
            }

            //write list
            File.WriteAllLines(ThSettings.RpgMakerMvSkipjsRulesFilePath(), skipJsOveralList);
        }
    }
}

