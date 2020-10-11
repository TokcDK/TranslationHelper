using System.IO;
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
                    return true;
                }
            }

            return false;
        }

        private bool TryToExtractToRPGMakerTransPatch(string sPath, string extractdir = "Work")
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            var DBDir = Path.Combine(Application.StartupPath, extractdir, "RPGMakerTrans");
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

                }
            }

            return RPGMTransOther.CreateRPGMakerTransPatch(dir.FullName, workdirPath);
        }

        string patchdir;
        private bool RPGMTransPatchPrepare()
        {
            patchdir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(Properties.Settings.Default.THSelectedGameDir) + "_patch");

            return OpenSaveFilesBase(patchdir, new TXT(thDataWork), "*.txt");
        }

        internal override bool Save()
        {
            return OpenSaveFilesBase(patchdir, new TXT(thDataWork), "*.txt");
        }
        internal override void PreSaveDB()
        {
            OpenSaveFilesBase(patchdir, new TXT(thDataWork), "*.txt");
        }
    }
}
