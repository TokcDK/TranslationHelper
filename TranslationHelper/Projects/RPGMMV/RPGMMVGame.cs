using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects
{
    class RPGMMVGame : ProjectBase
    {
        public RPGMMVGame(THDataWork thDataWork) : base(thDataWork)
        {
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

        internal override bool Open()
        {
            try
            {
                thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", "plugins.js");
                new PLUGINS_JS(thDataWork).Open();

                var mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data/")));
                foreach (FileInfo file in mvdatadir.GetFiles("*.json"))
                {
                    if (OpenRPGMakerMVjson(file.FullName))
                    {
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool OpenRPGMakerMVjson(string sPath)
        {
            try
            {
                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string Jsonname = Path.GetFileNameWithoutExtension(sPath); // get json file name

                //ProgressInfo(true, T._("opening file: ") + Jsonname + ".json");

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
            for (int f = 0; f < thDataWork.Main.THFilesList.Items.Count; f++)
            {
                //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                var table = thDataWork.THFilesElementsDataset.Tables[f];
                bool changed = false;

                if (!FunctionsTable.IsTableRowsAllEmpty(table))
                {
                    changed = true;
                }

                //THMsg.Show(Properties.Settings.Default.THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                if (changed)
                {
                    if (table.TableName == "plugins")
                    {
                        thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "js", table.TableName + ".js");
                        new PLUGINS_JS(thDataWork).Save();
                    }
                    else
                    {
                        thDataWork.FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data", table.TableName + ".json");
                        new JSON(thDataWork).Save();
                    }
                }
            }

            return true;
        }

        internal override string NewlineSymbol()
        {
            return "\\n";
        }

    }
}

