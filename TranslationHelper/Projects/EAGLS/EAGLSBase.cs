using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.EAGLS
{
    abstract class EAGLSBase : ProjectBase
    {
        protected EAGLSBase() : base()
        {
        }

        public override void Init()
        {
            base.Init();
            ProjectName = ProjectName();
            ProjectData.ProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), ProjectName);
            WorkTXTDir = Path.Combine(ProjectData.ProjectWorkDir, "txt");
            ScriptDir = Path.Combine(ProjectData.SelectedGameDir, "Script");
            SCPACKpak = Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "Script", "SCPACK.pak");
            SCPACKidx = Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "Script", "SCPACK.idx");
        }

        internal override string ProjectFolderName()
        {
            return "EAGLS";
        }

        internal override string ProjectTitlePrefix()
        {
            return ProjectFolderName();
        }

        string scriptsFIlter = "sc_*.txt";

        /// <summary>
        /// unpack txt files from SCPACK or pack translated txt to them. pack by default
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        protected bool PackUnpackFiles()
        {
            try
            {
                ProjectData.ProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), ProjectName);
                var workdir = ProjectData.ProjectWorkDir;

                var pythonexe = THSettingsData.PythonExePath();
                var scpacker = THSettingsData.SCPackerPYPath();
                var scriptdir = ScriptDir;

                WorkTXTDir = Path.Combine(ProjectData.ProjectWorkDir, "txt");
                var mode = (ProjectData.SaveFileMode ? string.Empty : "un") + "pack";
                //var arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\" -t -o";
                var arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\"";

                Directory.CreateDirectory(WorkTXTDir);

                //write command file
                File.WriteAllText(Path.Combine(ProjectData.ProjectWorkDir, mode + "1.bat"), "\"" + pythonexe + "\" " + arguments + "\npause");

                var code = FunctionsProcess.RunProcess(pythonexe, arguments, "", true, false);
                if (!code || FunctionsFileFolder.CheckDirectoryNullOrEmpty_Fast(WorkTXTDir, scriptsFIlter))
                {
                    Directory.Delete(workdir, true);
                    return false;

                    //arguments = "\"" + scpacker + "\" " + mode + " \"" + scriptdir + "\" \"" + WorkTXTDir + "\" -t";

                    ////write command file
                    //File.WriteAllText(Path.Combine(ProjectData.ProjectWorkDir, mode + "2.bat"), "\"" + pythonexe + "\" " + arguments + "\npause");

                    //code = FunctionsProcess.RunProcess(pythonexe, arguments, "", true, false);
                    //if (!code || FunctionsFileFolder.CheckDirectoryNullOrEmpty_Fast(WorkTXTDir, scriptsFIlter))
                    //{
                    //    Directory.Delete(workdir, true);
                    //    return false;
                    //}
                }

                var errorslog = Path.Combine(THSettingsData.SCPackerPYPath(), "errors.log.txt");
                if (File.Exists(errorslog) && new FileInfo(errorslog).Length > 0)
                {
                    MessageBox.Show(T._("Errors was occcured while packing") + "." + T._("Will be opened log and work dir") + ".");
                    FunctionsProcess.RunProcess(errorslog, "");
                    FunctionsProcess.RunProcess(ProjectData.ProjectWorkDir, "");
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
            return OpenSaveFilesBase(WorkTXTDir, new SC_TXT(), scriptsFIlter);
        }

        protected bool SaveFiles()
        {
            return OpenSaveFilesBase(WorkTXTDir, new SC_TXT(), scriptsFIlter);
        }

        protected string SCPACKpak;
        protected string SCPACKidx;
        protected string WorkTXTDir;
        internal override bool BakCreate()
        {
            return BackupRestorePaths(new[]
            {
                SCPACKpak,
                SCPACKidx,
                WorkTXTDir
            }, true);
        }

        internal override bool BakRestore()
        {
            return BackupRestorePaths(new[]
            {
                SCPACKpak,
                SCPACKidx,
                WorkTXTDir
            }, false);
        }
    }
}
