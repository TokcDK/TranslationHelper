using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Functions;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVGame : ProjectBase
    {
        readonly List<JSBase> ListOfJS;
        public RPGMMVGame(THDataWork thDataWork) : base(thDataWork)
        {
            ListOfJS = JSBase.GetListOfJS(thDataWork);
        }

        internal override bool Check()
        {
            if (Path.GetExtension(thDataWork.SPath) == ".exe")
            {
                if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data", "system.json")))
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
                string cell = thDataWork.THFilesElementsDatasetInfo.Tables[tind].Rows[rind][0] + string.Empty;
                if (cell.Contains("Code=655") || cell.Contains("Code=355") /*|| thDataWork.THFilesElementsDataset.Tables[tind].TableName.ToUpperInvariant().EndsWith(".JS")*/)
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
        /// <param name="Write">Use Save() instead of Open()</param>
        /// <returns></returns>
        private bool ParseProjectFiles(bool Write = false)
        {
            if (!Write)
            {
                BakRestore();
            }

            ParseFileMessage = Write ? T._("write file: ") : T._("opening file: ");
            bool IsAnyFileCompleted = false;
            try
            {
                //gamefont.css to be possible to change font
                thDataWork.Main.ProgressInfo(true, ParseFileMessage + "gamefont.css");
                thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "fonts", "gamefont.css");

                try
                {
                    if (Write && new GAMEFONTCSS(thDataWork).Save())
                    {
                        IsAnyFileCompleted = true;
                    }
                    else if (new GAMEFONTCSS(thDataWork).Open())
                    {
                        IsAnyFileCompleted = true;
                    }
                }
                catch
                {
                }

                var HardcodedJS = new HashSet<string>();
                //Proceeed js-files
                foreach (var JS in ListOfJS)
                {
                    thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);

                    if (!File.Exists(thDataWork.FilePath))
                    {
                        continue;
                    }

                    try
                    {
                        thDataWork.Main.ProgressInfo(true, ParseFileMessage + JS.JSName);

                        HardcodedJS.Add(JS.JSName);//add js to exclude from parsing of other js

                        if (Write && JS.Save())
                        {
                            IsAnyFileCompleted = true;
                        }
                        else if (JS.Open())
                        {
                            IsAnyFileCompleted = true;
                        }
                    }
                    catch
                    {
                    }
                }

                //hardcoded exclusions
                var SkipJSList = new HashSet<string>
                {
                    "ConditionallyCore.js",//translation of text in quotes will make skills not executable
                    //---
                    //----//
                };

                //add exclusions from skipjs file
                SetSkipJSLists(SkipJSList);

                //Proceeed other js-files with quotes search
                foreach (var JS in Directory.EnumerateFiles(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "www", "js", "plugins"), "*.js"))
                {
                    string JSName = Path.GetFileName(JS);

                    if (HardcodedJS.Contains(JSName) || SkipJSList.Contains(JSName))
                        continue;

                    thDataWork.Main.ProgressInfo(true, ParseFileMessage + JSName);
                    thDataWork.FilePath = JS;

                    if (!File.Exists(thDataWork.FilePath))
                    {
                        continue;
                    }

                    try
                    {
                        if (Write && new ZZZOtherJS(thDataWork).Save())
                        {
                            IsAnyFileCompleted = true;
                        }
                        else if (new ZZZOtherJS(thDataWork).Open())
                        {
                            IsAnyFileCompleted = true;
                        }
                    }
                    catch
                    {
                    }
                }



                //Proceed json-files
                var mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data/")));
                foreach (FileInfo file in mvdatadir.GetFiles("*.json"))
                {
                    try
                    {
                        if (ParseRPGMakerMVjson(file.FullName, Write))
                        {
                            IsAnyFileCompleted = true;
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

            thDataWork.Main.ProgressInfo(false);
            return IsAnyFileCompleted; ;
        }

        private void SetSkipJSLists(HashSet<string> SkipJSList)
        {
            foreach (var skipjsfilePath in THSettingsData.RPGMakerMVSkipjsRulesFilesList())
            {
                SetSkipJSList(SkipJSList, skipjsfilePath);
            }
        }

        private void SetSkipJSList(HashSet<string> SkipJSList, string skipjsfilePath)
        {
            if (File.Exists(skipjsfilePath))
            {
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
        }

        private bool ParseRPGMakerMVjson(string FilePath, bool Write = false)
        {
            try
            {
                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string Jsonname = Path.GetFileNameWithoutExtension(FilePath); // get json file name

                thDataWork.Main.ProgressInfo(true, ParseFileMessage + Jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                thDataWork.FilePath = FilePath;
                //ret = ReadJson(Jsonname, sPath);

                ret = Write ? new JSON(thDataWork).Save() : new JSON(thDataWork).Open();

                return ret;
            }
            catch
            {
                return false;
            }
        }

        internal override bool Save()
        {
            return ParseProjectFiles(true);
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
            //    File.Copy(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "fonts", "gamefont.css")
            //        , Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "fonts", "gamefont.css") + ".bak"
            //        );
            //}
            //catch
            //{

            //}
            //try
            //{
            //    string dataPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data");
            //    dataPath.CopyAll(dataPath + "_bak");
            //}
            //catch
            //{
            //}
            //foreach (JSBase JS in ListOfJS)
            //{
            //    try
            //    {
            //        string jsPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);

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
            foreach (JSBase JS in ListOfJS)
            {
                RestoreFromBakIfNeedJS(JS);
            }

            return BackupRestorePaths(BakPaths, false);
        }

        /// <summary>
        /// old variant of restore still here because beck compatibility
        /// </summary>
        internal static void RestoreFromBakIfNeedData()
        {
            var dataPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data");
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

            if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "fonts", "gamefont.css.bak")))
            {
                try
                {
                    File.Delete(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "fonts", "gamefont.css"));
                    File.Move(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "fonts", "gamefont.css.bak")
                        , Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "fonts", "gamefont.css")
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
            string jsPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);
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
                if (thDataWork.OnlineTranslationCache == null)
                {
                    thDataWork.OnlineTranslationCache = new FunctionsOnlineCache(thDataWork);
                    thDataWork.OnlineTranslationCache.Read();
                }

                FillTablesLinesDict();

                var name = Regex.Replace(original, @"^\\n<(.+)>[\s\S]*$", "$1");

                var translatedname = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(name);

                if (translatedname.Length > 0)
                {
                    return translation.Insert(3, translatedname);
                }
                else
                {
                    if (thDataWork.TablesLinesDict.ContainsKey(name))
                    {
                        return translation.Insert(3, thDataWork.TablesLinesDict[name]);
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
                Text = "[" + thDataWork.CurrentProject.Name() + "]" + SkipJSMenuName
            };
            category.DropDownItems.Add(SkipJSMenu);
            SkipJSMenu.Click += RPGMMVGameSkipJSMenu_Click;

            var SkipJSFileOpen = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "[" + thDataWork.CurrentProject.Name() + "]" + SkipJSMenuName + "-->" + T._("Open")
            };
            category.DropDownItems.Add(SkipJSFileOpen);
            SkipJSFileOpen.Click += RPGMMVGameSkipJSFileOpen_Click;

            thDataWork.Main.Invoke((Action)(() => thDataWork.Main.CMSFilesList.Items.Add(category)));
        }

        /// <summary>
        /// open skip js list txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPGMMVGameSkipJSFileOpen_Click(object sender, EventArgs e)
        {
            Process.Start(THSettingsData.RPGMakerMVSkipjsRulesFilePath());
        }

        /// <summary>
        /// add js file from files list to skipjs rules file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPGMMVGameSkipJSMenu_Click(object sender, System.EventArgs e)
        {
            //read and check the name
            var name = thDataWork.Main.GetFilesListSelectedName();
            if (string.IsNullOrWhiteSpace(name) || !name.ToUpperInvariant().EndsWith(".JS"))
            {
                return;
            }

            //read only js names
            var SkipJSList = new HashSet<string>();
            SetSkipJSList(SkipJSList, THSettingsData.RPGMakerMVSkipjsRulesFilePath());

            //read all list
            List<string> SkipJSOveralList;
            if (SkipJSList.Count == 0 || !File.Exists(THSettingsData.RPGMakerMVSkipjsRulesFilePath()))
            {
                SkipJSOveralList = new List<string>();
            }
            else
            {
                SkipJSOveralList = File.ReadAllLines(THSettingsData.RPGMakerMVSkipjsRulesFilePath()).ToList();
            }

            //check if name already exists in list
            if (!SkipJSList.Contains(name))
            {
                SkipJSOveralList.Add(name);
            }

            //write list
            File.WriteAllLines(THSettingsData.RPGMakerMVSkipjsRulesFilePath(), SkipJSOveralList);
        }
    }
}

