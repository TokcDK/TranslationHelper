using System;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Functions;

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
            return Path.GetExtension(AppData.SelectedProjectFilePath) == ".exe"
                &&
                Path.GetFileNameWithoutExtension(AppData.SelectedProjectFilePath) == GameExeName
                &&
                Directory.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "data", "Script"));
        }

        internal override string FileFilter => ProjectTools.GameExeFilter;

        protected override bool TryOpen()
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

            Logger.Info(Path.GetFileName(AppData.SelectedProjectFilePath));
            var openPath = new DirectoryInfo(Path.GetDirectoryName(AppData.SelectedProjectFilePath));
            if (ProjectToolsOpenSave.OpenSaveFilesBase(this, openPath, GameExeFormatType, GameExeName + ".exe", searchOption: SearchOption.TopDirectoryOnly))
            {
                ret = true;
            }

            openPath = new DirectoryInfo(Path.Combine(openPath.FullName, "data"));
            if (!Directory.Exists(openPath.FullName + ".skip") && !File.Exists(openPath.FullName + ".skip"))
            {
                var txtFormat = typeof(Formats.IrisField.TXT);
                if (ProjectToolsOpenSave.OpenSaveFilesBase(this, openPath, txtFormat, "*.txt"))
                {
                    ret = true;
                }
            }

            
            return ret;
        }

        protected override bool TrySaveProject()
        {
            return SaveFiles();
        }

        private bool SaveFiles()
        {
            OpenFilesSerial();
            return true;
        }

        readonly string[] BackupPaths;

        public override bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(BackupPaths);
        }

        public override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(BackupPaths, false);
        }
    }
}
