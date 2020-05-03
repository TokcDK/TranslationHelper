using System.Collections.Generic;
using System.Data;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects
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
            bool IsAnyFileCompleted = false;
            try
            {
                //Proceeed js-files
                foreach (var JS in ListOfJS)
                {
                    thDataWork.Main.ProgressInfo(true, T._("opening file: ") + JS.JSName);
                    thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);

                    if (!File.Exists(thDataWork.FilePath))
                    {
                        continue;
                    }

                    try
                    {
                        if (JS.Open())
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
                        if (OpenRPGMakerMVjson(file.FullName))
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
            return IsAnyFileCompleted;
        }

        private bool OpenRPGMakerMVjson(string sPath)
        {
            try
            {
                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string Jsonname = Path.GetFileNameWithoutExtension(sPath); // get json file name

                thDataWork.Main.ProgressInfo(true, T._("opening file: ") + Jsonname + ".json");

                string jsondata = File.ReadAllText(sPath); // get json data

                bool ret = true;

                thDataWork.FilePath = sPath;
                //ret = ReadJson(Jsonname, sPath);
                ret = new JSON(thDataWork).Open();

                return ret;
            }
            catch
            {
                return false;
            }

        }

        internal override bool Save()
        {
            //thDataWork.CurrentProject.FillTHFilesElementsDictionary();

            foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
            {
                //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                //var table = thDataWork.THFilesElementsDataset.Tables[f];
                bool changed = false;

                if (!FunctionsTable.IsTableRowsAllEmpty(table))
                {
                    changed = true;
                }

                ///*THMsg*/MessageBox.Show(Properties.Settings.Default.THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                if (changed)
                {
                    if (table.TableName.EndsWith(".js"))
                    {
                        JSBase JS = GetCorrectJSbyName(table.TableName);
                        if (JS != null)
                        {
                            thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", JS.JSSubfolder, JS.JSName);
                            JS.Save();
                        }
                    }
                    else
                    {
                        thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data", table.TableName + ".json");
                        new JSON(thDataWork).Save();
                    }
                }
            }

            //thDataWork.THFilesElementsDictionary.Clear();

            thDataWork.Main.ProgressInfo(false);
            return true;
        }

        private JSBase GetCorrectJSbyName(string tableName)
        {
            foreach (var JS in ListOfJS)
            {
                if (tableName == JS.JSName)
                {
                    return JS;
                }
            }
            return null;
        }

        internal override string NewlineSymbol => "\\n";

    }
}

