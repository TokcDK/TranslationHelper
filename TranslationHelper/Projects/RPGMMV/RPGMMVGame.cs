using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Functions;
using TranslationHelper.Translators;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVGame : ProjectBase
    {
        readonly List<JSBase> ListOfJS;
        public RPGMMVGame(THDataWork thDataWork) : base(thDataWork)
        {
            ListOfJS = JSBase.GetListOfJS(thDataWork);
        }

        internal override bool OpenDetect()
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

        internal override string ProjectTitle()
        {
            return "RPG Maker MV";
        }

        internal override string ProjecFolderName()
        {
            return "RPGMakerMV";
        }

        internal override bool IsTestRunEnabled => true;

        internal override bool Open()
        {
            return ParseProjectFiles();
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
                BuckupRestore();
            }

            ParseFileMessage = Write ? T._("write file: ") : T._("opening file: ");
            bool IsAnyFileCompleted = false;
            try
            {
                //Proceeed js-files
                foreach (var JS in ListOfJS)
                {
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

        internal override bool BuckupCreate()
        {
            RestoreFromBakIfNeedData();
            try
            {
                string dataPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data");
                CopyFolder.Copy(dataPath, dataPath + "_bak");
            }
            catch
            {
            }
            foreach (JSBase JS in ListOfJS)
            {
                try
                {
                    string jsPath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);

                    RestoreFromBakIfNeedJS(JS);
                    if (File.Exists(jsPath))
                    {
                        File.Copy(jsPath, jsPath + ".bak");
                    }
                }
                catch
                {
                }
            }
            return true;
        }

        internal override bool BuckupRestore()
        {
            RestoreFromBakIfNeedData();
            foreach (JSBase JS in ListOfJS)
            {
                RestoreFromBakIfNeedJS(JS);
            }
            return true;
        }

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
        }

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
                    thDataWork.OnlineTranslationCache.ReadCache();
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

    }
}

