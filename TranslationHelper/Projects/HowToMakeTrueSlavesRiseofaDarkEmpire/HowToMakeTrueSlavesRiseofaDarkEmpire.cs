using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.HowToMakeTrueSlavesRiseofaDarkEmpire
{
    class HowToMakeTrueSlavesRiseofaDarkEmpire : ProjectBase
    {
        public HowToMakeTrueSlavesRiseofaDarkEmpire(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Name()
        {
            return "How To Make True Slaves -Rise of a Dark Empire-";
        }

        internal override bool Check()
        {
            return Path.GetExtension(thDataWork.SPath) == ".exe"
                &&
                Path.GetFileNameWithoutExtension(thDataWork.SPath) == "正しい性奴隷の使い方"
                &&
                Directory.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "data", "Script"));
        }

        internal override string Filters()
        {
            return GameExeFilter;
        }

        internal override bool Open()
        {
            return OpenFiles();
        }

        private bool OpenFiles()
        {
            OpenFilesSerial();
            return thDataWork.THFilesElementsDataset.Tables.Count > 0;
        }

        /// <summary>
        /// IsOpen=true = Open, else Save
        /// </summary>
        /// <param name="IsOpen"></param>
        private bool OpenFilesSerial(bool IsOpen = true, string openPath = "")
        {
            var ret = false;

            thDataWork.Main.ProgressInfo(true, Path.GetFileName(thDataWork.SPath));
            thDataWork.FilePath = thDataWork.SPath;
            if (thDataWork.OpenFileMode ? new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.EXE(thDataWork).Open() : new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.EXE(thDataWork).Save())
            {
                ret = true;
            }


            if (openPath.Length == 0)
            {
                openPath = Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "data");
            }

            var txtFormat = new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.TXT(thDataWork);
            if (OpenSaveFilesBase(new DirectoryInfo(openPath), txtFormat, "*.txt"))
            {
                ret = true;
            }


            foreach (string txt in Directory.EnumerateFiles(openPath, "*.txt", SearchOption.AllDirectories))
            {
                thDataWork.FilePath = txt;
                thDataWork.Main.ProgressInfo(true, Path.GetFileName(txt));

                if (IsOpen)
                {
                    txtFormat.Open();
                }
                else
                {
                    txtFormat.Save();
                }
            }

            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        internal override bool Save()
        {
            return SaveFiles();
        }

        private bool SaveFiles()
        {
            OpenFilesSerial(false);
            return true;
        }

        readonly string[] BackupPaths = new string[3]
        {
            @".\data\Script",
            @".\data\AdditionalScript",
            @".\正しい性奴隷の使い方.exe"
        };

        internal override bool BakCreate()
        {
            return BuckupRestorePaths(BackupPaths);
        }

        internal override bool BakRestore()
        {
            return BuckupRestorePaths(BackupPaths, false);
        }
    }
}
