using System.Collections.Generic;
using System.Data;
using System.IO;
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
                    HardcodedJS.Add(JS.JSName);//add js to exclude from parsing of other js

                    thDataWork.Main.ProgressInfo(true, ParseFileMessage + JS.JSName);
                    thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);

                    if (!File.Exists(thDataWork.FilePath))
                    {
                        continue;
                    }

                    try
                    {
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
                var Exclusions = new HashSet<string>
                {
                    "ConditionallyCore.js",//translation of text in quotes will make skills not executable
                    //---
                    //----//
                };

                //add exclusions from skipjs file
                foreach (var skipjsfile in THSettingsData.RPGMakerMVSkipjsRulesFilesList())
                {
                    if (File.Exists(skipjsfile))
                    {
                        var skipjs = File.ReadAllLines(skipjsfile);
                        foreach (var line in skipjs)
                        {
                            var jsfile = line.Trim();
                            if (jsfile.Length == 0 || jsfile[0]==';' || Exclusions.Contains(jsfile))
                            {
                                continue;
                            }
                            Exclusions.Add(jsfile);
                        }

                    }
                }

                //Proceeed other js-files with quotes search
                foreach (var JS in Directory.EnumerateFiles(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "www", "js", "plugins"), "*.js"))
                {
                    if (HardcodedJS.Contains(Path.GetFileName(JS)) || Exclusions.Contains(Path.GetFileName(JS)))
                        continue;

                    thDataWork.Main.ProgressInfo(true, ParseFileMessage + Path.GetFileName(JS));
                    thDataWork.FilePath = JS;

                    if (!File.Exists(thDataWork.FilePath))
                    {
                        continue;
                    }

                    try
                    {
                        if (Write && new TEMPLATE(thDataWork).Save())
                        {
                            IsAnyFileCompleted = true;
                        }
                        else if (new TEMPLATE(thDataWork).Open())
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

    }
}

