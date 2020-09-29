using System.IO;
using System.Text;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTrans;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects
{
    class RPGMGame : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        public RPGMGame(THDataWork thData) : base(thData)
        {
        }

        internal override bool Check()
        {
            if (Path.GetExtension(thDataWork.SPath) == ".exe")
            {
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(thDataWork.SPath));

                if (File.Exists(Path.Combine(dir.FullName, "Data", "System.rvdata2"))
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rgss3a")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rgss2a")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rvdata")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rgssad")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.rxdata")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.lmt")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir.FullName, "*.lmu")
                    )
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
            return "RPG Maker Game";
        }

        internal override string ProjectFolderName()
        {
            return "RPGMakerTransPatch";
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
                    Properties.Settings.Default.THSelectedDir = extractedpatchpath.Replace(Path.DirectorySeparatorChar + "patch", string.Empty);

                    return true;
                }
            }

            return false;
        }

        private bool TryToExtractToRPGMakerTransPatch(string sPath, string extractdir = "Work")
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            var DBDir = Path.Combine(Application.StartupPath, extractdir, "RPGMakerTrans");
            //if (!Directory.Exists(DBDir))
            //{
            //    Directory.CreateDirectory(DBDir);
            //}
            //MessageBox.Show("tempdir=" + tempdir);
            var workdirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(sPath));
            var workdirPath = Path.Combine(DBDir, workdirName);
            Properties.Settings.Default.THProjectWorkDir = workdirPath;
            var outdirpatchPath = Path.Combine(workdirPath, workdirName + "_patch");
            if (!Directory.Exists(workdirPath))
            {
                Directory.CreateDirectory(workdirPath);
            }

            if (extractdir == "Work")
            {
                extractedpatchpath = outdirpatchPath;// Распаковывать в Work\ProjectDir\
            }

            if (Directory.Exists(workdirPath + "_patch"))
            {
                Directory.Move(workdirPath + "_patch", outdirpatchPath);
            }

            if (Directory.Exists(workdirPath + "_translated"))
            {
                Directory.Move(workdirPath + "_translated", Path.Combine(workdirPath, workdirName + "_translated"));
            }

            //if (!Directory.Exists(outdir))
            //{
            //    Directory.CreateDirectory(outdir);

            //    //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

            //}
            if (Directory.Exists(outdirpatchPath) && FunctionsFileFolder.IsInDirExistsAnyFile(outdirpatchPath, "RPGMKTRANSPATCH", true, true))
            {
                DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    //чистка и пересоздание папки
                    RPGMTransOther.CleanInvalidRPGMakerTransPatchFolders(workdirPath);
                    Directory.CreateDirectory(workdirPath);

                    //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

                }
            }

            return RPGMTransOther.CreateRPGMakerTransPatch(dir.FullName, workdirPath);
        }

        private bool RPGMTransPatchPrepare()
        {
            //var dir = new DirectoryInfo(Path.GetDirectoryName(thDataWork.SPath));

            //Properties.Settings.Default.THSelectedDir = dir + string.Empty;

            var patchdir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(Properties.Settings.Default.THProjectWorkDir) + "_patch");
            if (!Directory.Exists(patchdir))
            {
                return false;
            }
            Properties.Settings.Default.THSelectedDir = Path.GetDirectoryName(thDataWork.SPath);
            Properties.Settings.Default.THSelectedGameDir = Path.GetDirectoryName(thDataWork.SPath);
            using (var patchfile = new StreamReader(Path.Combine(patchdir, "RPGMKTRANSPATCH")))
            {
                if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(Path.Combine(patchdir, "patch"))) //если есть подпапка patch, тогда это версия патча 3
                {
                    RPGMTransPatchVersion = 3;
                    patchdir = Path.Combine(patchdir, "patch");
                }
                else //иначе это версия 2
                {
                    RPGMTransPatchVersion = 2;
                }
            }

            //var vRPGMTransPatchFiles = new List<string>();

            //foreach (var file in Directory.EnumerateFileSystemEntries(patchdir, "*.txt"))
            //{
            //    //MessageBox.Show("file.FullName=" + file.FullName);
            //    vRPGMTransPatchFiles.Add(file);
            //}

            if (OpenRPGMTransPatchFiles(patchdir))
            {
                return true;
            }

            return false;
        }

        public bool OpenRPGMTransPatchFiles(string patchdir)
        {
            if (string.IsNullOrWhiteSpace(patchdir) || thDataWork.THFilesElementsDataset == null)
                return false;

            var successCreated = false;

            foreach (var file in Directory.EnumerateFileSystemEntries(patchdir, "*.txt"))
            {
                thDataWork.FilePath = file;
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

                        if (FIleData.Length > 2)
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
