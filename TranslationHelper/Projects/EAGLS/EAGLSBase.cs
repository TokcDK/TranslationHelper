using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.EAGLS
{
    abstract class EAGLSBase : ProjectBase
    {
        protected EAGLSBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string ProjectFolderName()
        {
            return "EAGLS";
        }

        internal override string ProjectTitlePrefix()
        {
            return ProjectFolderName();
        }

        /// <summary>
        /// unpack txt files from SCPACK or pack translated txt to them. pack by default
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        protected bool PackUnpackFiles(bool pack = true)
        {
            try
            {
                Properties.Settings.Default.THProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), ProjectName);
                //Properties.Settings.Default.THProjectWorkDir = @".\" + THSettingsData.WorkDirPath() + @"\" + ProjecFolderName() + @"\" + Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath));
                //Directory.CreateDirectory(Properties.Settings.Default.THProjectWorkDir);
                var pythonexe = THSettingsData.Python37ExePath();
                //var pythonexe = @"C:\Python37\python";
                var scpacker = "\"" + THSettingsData.SCPackerPYPath() + "\"";
                var scriptdir = "\"" + ScriptDir + "\"";
                var workdir = "\"" + Properties.Settings.Default.THProjectWorkDir + "\"";
                var unpackpack = " " + (pack ? string.Empty : "un") + "pack" + " ";
                var arguments = scpacker + unpackpack + scriptdir + " " + workdir + " -t -o";
                var code = FunctionsProcess.RunProgram(pythonexe, arguments);
                if (!code || FunctionsFileFolder.CheckDirectoryNullOrEmpty_Fast(workdir.Trim('"'), "sc_*.txt"))
                {
                    arguments = scpacker + unpackpack + scriptdir + " " + workdir + " -t";
                    code = FunctionsProcess.RunProgram(pythonexe, arguments);

                    if (!code || FunctionsFileFolder.CheckDirectoryNullOrEmpty_Fast(workdir.Trim('"'), "sc_*.txt"))
                    {
                        Directory.Delete(workdir);
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected string ProjectName = string.Empty;
        protected string ScriptDir = string.Empty;

        protected bool OpenFiles()
        {
            var ret = false;
            foreach (var txt in Directory.EnumerateFiles(Properties.Settings.Default.THProjectWorkDir, "*.txt"))
            {
                if (Path.GetFileName(txt).StartsWith("sc_"))
                {
                    thDataWork.Main.ProgressInfo(true, T._("Open") + ": " + Path.GetFileName(txt));
                    thDataWork.FilePath = txt;
                    try
                    {
                        if (new SC_TXT(thDataWork).Open())
                        {
                            ret = true;
                        }
                    }
                    catch { }
                }
            }
            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        protected bool SaveFiles()
        {
            var ret = false;
            foreach (var txt in Directory.EnumerateFiles(Properties.Settings.Default.THProjectWorkDir, "*.txt"))
            {
                if (Path.GetFileName(txt).StartsWith("sc_"))
                {
                    thDataWork.Main.ProgressInfo(true, T._("Save") + ": " + Path.GetFileName(txt));
                    thDataWork.FilePath = txt;
                    try
                    {
                        if (new SC_TXT(thDataWork).Save())
                        {
                            ret = true;
                        }
                    }
                    catch { }
                }
            }
            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        protected string SCPACKpak = string.Empty;
        protected string SCPACKidx = string.Empty;
        protected string WorkTXTDir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "txt");
        internal override bool BakCreate()
        {
            return BuckupFile(SCPACKpak) && BuckupFile(SCPACKidx) && BuckupDir(WorkTXTDir);
        }

        internal override bool BakRestore()
        {
            return RestoreFile(SCPACKpak) && RestoreFile(SCPACKidx) && RestoreDir(WorkTXTDir);
        }
    }
}
