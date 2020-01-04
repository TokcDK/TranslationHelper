using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Projects
{
    class RPGMTransPatch : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        public RPGMTransPatch(THDataWork thData) : base(thData)
        {
        }

        internal override bool OpenDetect()
        {
            if (Path.GetFileName(thDataWork.SPath) == "RPGMKTRANSPATCH")
            {
                return true;
            }

            return false;
        }

        internal override string ProjectTitle()
        {
            return "RPG Maker Trans Patch";
        }

        internal override string ProjecFolderName()
        {
            return "RPGMTransPatch";
        }

        internal override bool Open()
        {
            if (RPGMTransPatchPrepare())
            {
                return true;
            }

            return false;
        }

        private bool RPGMTransPatchPrepare()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(thDataWork.SPath));

            //Properties.Settings.Default.THSelectedDir = dir + string.Empty;

            var patchdir = dir;
            StreamReader patchfile = new StreamReader(thDataWork.SPath);

            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "patch"))) //если есть подпапка patch, тогда это версия патча 3
            {
                RPGMTransPatchVersion = 3;
                patchdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "patch"));
            }
            else //иначе это версия 2
            {
                RPGMTransPatchVersion = 2;
            }
            patchfile.Close();

            var vRPGMTransPatchFiles = new List<string>();

            foreach (FileInfo file in patchdir.GetFiles("*.txt"))
            {
                //MessageBox.Show("file.FullName=" + file.FullName);
                vRPGMTransPatchFiles.Add(file.FullName);
            }

            //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

            //THFilesDataGridView.Nodes.Add("main");
            //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
            //RPGMTransPatch.OpenTransFiles(files, RPGMTransPatchVersion);
            if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles))
            {
                return true;
            }
            //}
            return false;
        }

        public bool OpenRPGMTransPatchFiles(List<string> ListFiles)
        {
            if (ListFiles == null || thDataWork.THFilesElementsDataset == null)
                return false;

            bool successCreated = false;

            //Читаем все файлы
            for (int i = 0; i < ListFiles.Count; i++)   //Обрабатываем всю строку
            {
                string fname = Path.GetFileNameWithoutExtension(ListFiles[i]);

                //ProgressInfo(true, T._("opening file: ") + fname + ".txt");

                //_ = thDataWork.THFilesElementsDataset.Tables.Add(fname);
                //_ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Original");
                //_ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Translation");
                //_ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Context");
                //_ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Advice");
                //_ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Status");
                //if (thDataWork.THFilesElementsDatasetInfo == null)
                //{
                //}
                //else
                //{
                //    _ = thDataWork.THFilesElementsDatasetInfo.Tables.Add(fname);
                //    _ = thDataWork.THFilesElementsDatasetInfo.Tables[i].Columns.Add("Original");
                //}

                thDataWork.FilePath = ListFiles[i];
                switch (RPGMTransPatchVersion)
                {
                    case 3:
                        successCreated = new TXTv3(thDataWork, null).Open();
                        break;
                    case 2:
                        successCreated = new TXTv2(thDataWork, null).Open();
                        break;
                    case 0:
                        return false;
                }
            }

            if (!successCreated)
            {
                return false;
            }

            return true;
        }

        internal override bool Save()
        {
            if (SaveRPGMTransPatchFiles())
            {
                return true;
            }

            return false;
        }

        public bool SaveRPGMTransPatchFiles()
        {
            try
            {
                StringBuilder buffer = new StringBuilder();
                
                for (int i = 0; i < thDataWork.THFilesElementsDataset.Tables.Count; i++)
                {
                    //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

                    bool successCreated = false;

                    switch (RPGMTransPatchVersion)
                    {
                        case 3:
                            successCreated = new TXTv3(thDataWork, buffer).Save();
                            break;
                        case 2:
                            successCreated = new TXTv2(thDataWork, buffer).Save();
                            break;
                        case 0:
                            return false;
                    }

                    string FIleData = buffer.ToString();
                    if (successCreated && !string.IsNullOrWhiteSpace(FIleData))
                    {
                        if (Directory.Exists(Properties.Settings.Default.THSelectedDir + Path.DirectorySeparatorChar + "patch"))
                        {
                        }
                        else
                        {
                            Directory.CreateDirectory(Properties.Settings.Default.THSelectedDir + Path.DirectorySeparatorChar + "patch");
                        }

                        if (FIleData.Length>2)
                        {
                            FIleData = FIleData.Remove(FIleData.Length - 2, 2);//удаление лишнего символа \r\n с конца строки
                        }

                        string _path = Path.Combine(Properties.Settings.Default.THSelectedDir, "patch", thDataWork.THFilesElementsDataset.Tables[i].TableName + ".txt");
                        File.WriteAllText(_path, FIleData);
                    }

                    buffer.Clear();
                }

                //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "RPGMKTRANSPATCH")))
                {
                }
                else
                {
                    File.WriteAllText(Path.Combine(Properties.Settings.Default.THSelectedDir, "RPGMKTRANSPATCH"), RPGMTransPatchVersion == 3 ? "> RPGMAKER TRANS PATCH V3" : string.Empty);
                }
            }
            catch
            {
                //ProgressInfo(false, string.Empty);
                //SaveInAction = false;
                return false;
            }
            finally
            {
                //ProgressInfo(false, string.Empty);
            }

            return true;

        }
    }
}
