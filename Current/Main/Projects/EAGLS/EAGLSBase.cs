using CheckForEmptyDir;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;
using TranslationHelper.Main.Functions;

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
            ProjectName = ProjectName;
            AppData.CurrentProject.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, ProjectName);
            WorkTXTDir = Path.Combine(AppData.CurrentProject.ProjectWorkDir, "txt");
            ScriptDir = Path.Combine(AppData.CurrentProject.SelectedGameDir, "Script");
            SCPACKpak = Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "Script", "SCPACK.pak");
            SCPACKidx = Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "Script", "SCPACK.idx");
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
                AppData.CurrentProject.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, ProjectName);
                var workdir = AppData.CurrentProject.ProjectWorkDir;

                var pythonexe = THSettings.PythonExePath;
                var scpacker = THSettings.SCPackerPYPath;
                var scriptdir = ScriptDir;

                WorkTXTDir = Path.Combine(AppData.CurrentProject.ProjectWorkDir, "txt");
                var mode = (SaveFileMode ? string.Empty : "un") + "pack";
                //var arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\" -t -o";
                var arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\"";

                Directory.CreateDirectory(WorkTXTDir);

                //write command file
                File.WriteAllText(Path.Combine(AppData.CurrentProject.ProjectWorkDir, mode + "1.bat"), "\"" + pythonexe + "\" " + arguments + "\npause");

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
                    FunctionsProcess.RunProcess(AppData.CurrentProject.ProjectWorkDir, "");
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected new string ProjectName = string.Empty;
        protected string ScriptDir = string.Empty;

        protected bool OpenFiles()
        {
            return OpenSaveFilesBase(WorkTXTDir, typeof(SC_TXT), scriptsMask);
        }

        protected bool SaveFiles()
        {
            return OpenSaveFilesBase(WorkTXTDir, typeof(SC_TXT), scriptsMask);
        }

        protected string SCPACKpak;
        protected string SCPACKidx;
        protected string WorkTXTDir;
        internal override bool BakCreate()
        {
            return ProjectTools.BackupRestorePaths(new[]
            {
                SCPACKpak,
                SCPACKidx,
                WorkTXTDir
            }, true);
        }

        internal override bool BakRestore()
        {
            return ProjectTools.BackupRestorePaths(new[]
            {
                SCPACKpak,
                SCPACKidx,
                WorkTXTDir
            }, false);
        }
    }
}
