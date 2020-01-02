using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;

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

                //for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                //{
                //    //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);
                //    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
                //}

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
                if (thDataWork.THFilesElementsDataset.Tables.Contains(Jsonname))
                {
                    //MessageBox.Show("true!");
                    return true;
                }

                //ProgressInfo(true, T._("opening file: ") + Jsonname + ".json");

                string jsondata = File.ReadAllText(sPath); // get json data

                thDataWork.THFilesElementsDataset.Tables.Add(Jsonname); // create table with json name
                thDataWork.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Original"); //create Original column
                thDataWork.THFilesElementsDatasetInfo.Tables.Add(Jsonname); // create table with json name
                thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Columns.Add("Original"); //create Original column

                bool ret = true;

                thDataWork.FilePath = sPath;
                //ret = ReadJson(Jsonname, sPath);
                ret = new JSON(thDataWork).Open();

                if (thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Count > 0)
                {
                    thDataWork.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Translation");
                }
                else
                {
                    thDataWork.THFilesElementsDataset.Tables.Remove(Jsonname); // remove table if was no items added
                    thDataWork.THFilesElementsDatasetInfo.Tables.Remove(Jsonname); // remove table if was no items added
                }

                return ret;
            }
            catch
            {
                return false;
            }

        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
