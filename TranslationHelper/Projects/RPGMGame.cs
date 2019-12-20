using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Formats.RPGMTrans;

namespace TranslationHelper.Projects
{
    class RPGMGame : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        public RPGMGame(THDataWork thData) : base(thData)
        {
        }

        internal override bool OpenDetect(string sPath)
        {
            if (Path.GetExtension(sPath) == ".exe")
            {
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(sPath));

                if (File.Exists(Path.Combine(dir.FullName, "Data", "System.rvdata2")) 
                    || dir.GetFiles("*.rgss3a").Length > 0 
                    || dir.GetFiles("*.rgss2a").Length > 0 
                    || dir.GetFiles("*.rvdata").Length > 0 
                    || dir.GetFiles("*.rgssad").Length > 0 
                    || dir.GetFiles("*.rxdata").Length > 0 
                    || dir.GetFiles("*.lmt").Length > 0 
                    || dir.GetFiles("*.lmu").Length > 0)
                {
                    return true;
                }
            }

            return false;
        }

        internal override string ProjectTitle()
        {
            return "RPG Maker Game";
        }

        internal override string ProjecFolderName()
        {
            return "RPGMTransPatch";
        }

        string extractedpatchpath;

        internal override bool Open()
        {
            extractedpatchpath = string.Empty;

            if (TryToExtractToRPGMakerTransPatch(thDataWork.SPath))
            {
                if (RPGMTransPatchPrepare())
                {

                    //Запись в dataGridVivwer
                    //for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    //{
                    //    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
                        
                    //}

                    Properties.Settings.Default.THSelectedGameDir = Properties.Settings.Default.THSelectedDir;
                    Properties.Settings.Default.THSelectedDir = extractedpatchpath.Replace(Path.DirectorySeparatorChar+"patch", string.Empty);

                    return true;
                }
            }

            return false;
        }

        private bool TryToExtractToRPGMakerTransPatch(string sPath, string extractdir = "Work")
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            string workdir = Path.Combine(Application.StartupPath, extractdir, "RPGMakerTrans");
            if (!Directory.Exists(workdir))
            {
                Directory.CreateDirectory(workdir);
            }
            //MessageBox.Show("tempdir=" + tempdir);
            string outdir = Path.Combine(workdir, Path.GetFileNameWithoutExtension(Path.GetDirectoryName(sPath)));


            if (extractdir == "Work")
            {
                extractedpatchpath = outdir + "_patch";// Распаковывать в Work\ProjectDir\
            }

            bool ret;// = false;
            //if (!Directory.Exists(outdir))
            //{
            //    Directory.CreateDirectory(outdir);

            //    //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

            //}
            if (Directory.Exists(outdir + "_patch") && Directory.GetFiles(outdir + "_patch", "RPGMKTRANSPATCH", SearchOption.AllDirectories).Length > 0)
            {
                DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    //чистка и пересоздание папки
                    Directory.Delete(outdir, true);
                    Directory.CreateDirectory(outdir);

                    //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

                }
            }

            ret = RPGMTransOther.CreateRPGMakerTransPatch(dir.FullName, outdir);

            if (ret)
            {
            }
            else
            {
                //чистка папок 
                Directory.Delete(outdir, true);
            }

            return ret;
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

                _ = thDataWork.THFilesElementsDataset.Tables.Add(fname);
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Original");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Translation");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Context");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Advice");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Status");
                if (thDataWork.THFilesElementsDatasetInfo == null)
                {
                }
                else
                {
                    _ = thDataWork.THFilesElementsDatasetInfo.Tables.Add(fname);
                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[i].Columns.Add("Original");
                }

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
