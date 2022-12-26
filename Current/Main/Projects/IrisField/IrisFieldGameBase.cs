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

        public override string Name => "Irisfield game";

        internal override string ProjectDBFileName => Name;

        protected abstract string GameExeName { get; }
        protected abstract Type GameExeFormatType { get; }
        /// <summary>
        /// Maximum length of line
        /// </summary>
        internal virtual int MaxLineLength { get => 60; }

        internal override bool IsValid()
        {
            return Path.GetExtension(AppData.SelectedFilePath) == ".exe"
                &&
                Path.GetFileNameWithoutExtension(AppData.SelectedFilePath) == GameExeName
                &&
                Directory.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "data", "Script"));
        }

        internal override string FileFilter => ProjectTools.GameExeFilter;

        public override bool Open()
        {
            return OpenFiles();
        }

        private bool OpenFiles()
        {
            OpenFilesSerial();
            return AppData.CurrentProject.FilesContent.Tables.Count > 0;
        }

        private bool OpenFilesSerial()
        {
            var ret = false;

            AppData.Main.ProgressInfo(true, Path.GetFileName(AppData.SelectedFilePath));
            var openPath = new DirectoryInfo(Path.GetDirectoryName(AppData.SelectedFilePath));
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

            AppData.Main.ProgressInfo(false);
            return ret;
        }

        public override bool Save()
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
            return ProjectToolsBackup.BackupRestorePaths(BackupPaths);
        }

        internal override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(BackupPaths, false);
        }
    }
}
