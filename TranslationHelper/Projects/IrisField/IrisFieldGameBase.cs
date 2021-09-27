using System;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.IrisField
{
    abstract class IrisFieldGameBase : ProjectBase
    {
        public IrisFieldGameBase()
        {
            BackupPaths = new string[3]
            {
                @".\data\Script",
                @".\data\AdditionalScript",
                @".\" + GameExeName + ".exe"
            };
        }

        internal override string Name()
        {
            return "Irisfield game";
        }

        internal override string GetProjectDBFileName()
        {
            return Name();
        }

        protected abstract string GameExeName { get; }
        protected abstract Type GameExeFormatType { get; }

        internal override bool Check()
        {
            return Path.GetExtension(ProjectData.SelectedFilePath) == ".exe"
                &&
                Path.GetFileNameWithoutExtension(ProjectData.SelectedFilePath) == GameExeName
                &&
                Directory.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "data", "Script"));
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

        private bool OpenFilesSerial()
        {
            var ret = false;

            ProjectData.Main.ProgressInfo(true, Path.GetFileName(ProjectData.SelectedFilePath));
            ProjectData.FilePath = ProjectData.SelectedFilePath;
            var openPath = new DirectoryInfo(Path.GetDirectoryName(ProjectData.SelectedFilePath));
            if (OpenSaveFilesBase(openPath, GameExeFormatType, GameExeName + ".exe", searchOption: SearchOption.TopDirectoryOnly))
            {
                ret = true;
            }

            openPath = new DirectoryInfo(Path.Combine(openPath.FullName, "data"));
            if (!Directory.Exists(openPath.FullName + ".skip") && !File.Exists(openPath.FullName + ".skip"))
            {
                var txtFormat = typeof(Formats.IrisField.TXT);
                if (OpenSaveFilesBase(openPath, txtFormat, "*.txt"))
                {
                    ret = true;
                }
            }

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

        readonly string[] BackupPaths;

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
