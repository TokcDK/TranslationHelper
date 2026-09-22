using CheckForEmptyDir;
using System;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;
using TranslationHelper.Main.Functions;
using MessageBox = TranslationHelper.Theming.ThemedMessageBox;

namespace TranslationHelper.Projects.EAGLS
{
    abstract class EAGLSBase : ProjectBase
    {
        protected EAGLSBase()
        {
        }

        public override void Init()
        {
            base.Init();

            // The work directory suffix must be the selected game folder name.
            // This used to be `ProjectName = ProjectName()` where ProjectName() was a
            // virtual method returning Path.GetFileName(Path.GetDirectoryName(SPath)).
            // When that method was replaced by the field below the call silently turned
            // into the no-op self-assignment `ProjectName = ProjectName`, which left the
            // suffix empty and made every EAGLS game share the same work directory.
            ProjectName = Path.GetFileName(SelectedGameDir) ?? string.Empty;

            ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, ProjectName);
            WorkTXTDir = Path.Combine(ProjectWorkDir, "txt");
            ScriptDir = Path.Combine(SelectedGameDir, "Script");
            SCPACKpak = Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "Script", "SCPACK.pak");
            SCPACKidx = Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "Script", "SCPACK.idx");
        }

        internal override string ProjectDBFolderName => "EAGLS";

        internal override string ProjectTitlePrefix => ProjectDBFolderName;

        string scriptsMask = "sc_*.txt";

        /// <summary>
        /// unpack txt files from SCPACK or pack translated txt to them. pack by default
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        protected bool PackUnpackFiles()
        {
            try
            {
                ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, ProjectName);
                var workdir = ProjectWorkDir;

                var pythonexe = THSettings.PythonExePath;
                var scpacker = THSettings.SCPackerPYPath;
                var scriptdir = ScriptDir;

                WorkTXTDir = Path.Combine(ProjectWorkDir, "txt");
                var mode = (SaveFileMode ? string.Empty : "un") + "pack";
                //var arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\" -t -o";
                var arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\"";

                Directory.CreateDirectory(WorkTXTDir);

                //write command file
                File.WriteAllText(Path.Combine(ProjectWorkDir, mode + "1.bat"), "\"" + pythonexe + "\" " + arguments + "\npause");

                var code = FunctionsProcess.RunProcess(pythonexe, arguments, "", true, false);
                if (!code || WorkTXTDir.IsNullOrEmptyDirectory(scriptsMask))
                {
                    Directory.Delete(workdir, true);
                    return false;

                    //arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\" -t";

                    ////write command file
                    //File.WriteAllText(Path.Combine(ProjectData.CurrentProject.ProjectWorkDir, mode + "2.bat"), "\"" + pythonexe + "\" " + arguments + "\npause");

                    //code = FunctionsProcess.RunProcess(pythonexe, arguments, "", true, false);
                    //if (!code || FunctionsFileFolder.CheckDirectoryNullOrEmpty_Fast(WorkTXTDir, scriptsFIlter))
                    //{
                    //    Directory.Delete(workdir, true);
                    //    return false;
                    //}
                }

                var errorslog = Path.Combine(THSettings.SCPackerPYPath, "errors.log.txt");
                if (File.Exists(errorslog) && new FileInfo(errorslog).Length > 0)
                {
                    MessageBox.Show(T._("Errors was occcured while packing") + "." + T._("Will be opened log and work dir") + ".");
                    FunctionsProcess.RunProcess(errorslog, "");
                    FunctionsProcess.RunProcess(ProjectWorkDir, "");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(T._("Failed to pack/unpack files") + ": " + ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Name of the project sub directory inside the EAGLS work directory.
        /// Defaults to the selected game folder name and can be replaced by derived
        /// projects (see <see cref="SCPACKpak"/>) before the work directory is used.
        /// </summary>
        protected string ProjectName = string.Empty;
        protected string ScriptDir = string.Empty;

        protected bool OpenFiles()
        {
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, WorkTXTDir, typeof(SC_TXT), scriptsMask);
        }

        protected bool SaveFiles()
        {
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, WorkTXTDir, typeof(SC_TXT), scriptsMask);
        }

        protected string SCPACKpak;
        protected string SCPACKidx;
        protected string WorkTXTDir;
        public override bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(this, new[]
            {
                SCPACKpak,
                SCPACKidx,
                WorkTXTDir
            }, true);
        }

        public override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(this, new[]
            {
                SCPACKpak,
                SCPACKidx,
                WorkTXTDir
            }, false);
        }
    }
}
