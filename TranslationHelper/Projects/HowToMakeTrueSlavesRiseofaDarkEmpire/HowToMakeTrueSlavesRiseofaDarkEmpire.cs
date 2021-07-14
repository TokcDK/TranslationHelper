using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.HowToMakeTrueSlavesRiseofaDarkEmpire
{
    class HowToMakeTrueSlavesRiseofaDarkEmpire : ProjectBase
    {
        public HowToMakeTrueSlavesRiseofaDarkEmpire() : base()
        {
        }

        internal override string Name()
        {
            return "How to make true slaves -Rise of a Dark Empire-";
        }

        internal override string GetProjectDBFileName()
        {
            return Name();
        }

        internal override bool Check()
        {
            return Path.GetExtension(ProjectData.SPath) == ".exe"
                &&
                Path.GetFileNameWithoutExtension(ProjectData.SPath) == "正しい性奴隷の使い方"
                &&
                Directory.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "data", "Script"));
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
            return ProjectData.THFilesElementsDataset.Tables.Count > 0;
        }

        /// <summary>
        /// IsOpen=true = Open, else Save
        /// </summary>
        /// <param name="IsOpen"></param>
        private bool OpenFilesSerial()
        {
            var ret = false;

            ProjectData.Main.ProgressInfo(true, Path.GetFileName(ProjectData.SPath));
            ProjectData.FilePath = ProjectData.SPath;
            if (ProjectData.OpenFileMode ? new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.EXE().Open() : new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.EXE().Save())
            {
                ret = true;
            }


            var openPath = Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "data");

            if (!Directory.Exists(openPath + ".skip") && !File.Exists(openPath + ".skip"))
            {
                var txtFormat = new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.TXT();
                if (OpenSaveFilesBase(new DirectoryInfo(openPath), txtFormat, "*.txt"))
                {
                    ret = true;
                }
            }

            //old
            //foreach (string txt in Directory.EnumerateFiles(openPath, "*.txt", SearchOption.AllDirectories))
            //{
            //    ProjectData.FilePath = txt;
            //    ProjectData.Main.ProgressInfo(true, Path.GetFileName(txt));

            //    if (IsOpen)
            //    {
            //        txtFormat.Open();
            //    }
            //    else
            //    {
            //        txtFormat.Save();
            //    }
            //}

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        internal override bool Save()
        {
            return SaveFiles();
        }

        private bool SaveFiles()
        {
            OpenFilesSerial();
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
            return BackupRestorePaths(BackupPaths);
        }

        internal override bool BakRestore()
        {
            return BackupRestorePaths(BackupPaths, false);
        }
    }
}
