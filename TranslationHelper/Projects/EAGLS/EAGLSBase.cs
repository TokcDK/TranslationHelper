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
            WorkTXTDir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "txt");
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
                var workdir = Properties.Settings.Default.THProjectWorkDir;
                WorkTXTDir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "txt");
                var unpackpack = " " + (pack ? string.Empty : "un") + "pack" + " ";
                var arguments = scpacker + unpackpack + scriptdir + " \"" + WorkTXTDir + "\" -t -o";
                Directory.CreateDirectory(WorkTXTDir);
                var code = FunctionsProcess.RunProcess(pythonexe, arguments, "", true, false);
                if (!code || FunctionsFileFolder.CheckDirectoryNullOrEmpty_Fast(WorkTXTDir, "*.txt"))
                {
                    arguments = scpacker + unpackpack + scriptdir + " \"" + WorkTXTDir + "\" -t";
                    code = FunctionsProcess.RunProcess(pythonexe, arguments, "", true, false);

                    if (!code || FunctionsFileFolder.CheckDirectoryNullOrEmpty_Fast(WorkTXTDir, "*.txt"))
                    {
                        Directory.Delete(workdir, true);
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
            return OpenSaveFilesBase(WorkTXTDir, new SC_TXT(thDataWork), "*.txt");


            //var ret = false;
            //foreach (var txt in Directory.EnumerateFiles(Properties.Settings.Default.THProjectWorkDir, "*.txt"))
            //{
            //    if (Path.GetFileName(txt).StartsWith("sc_"))
            //    {
            //        thDataWork.Main.ProgressInfo(true, T._("Open") + ": " + Path.GetFileName(txt));
            //        thDataWork.FilePath = txt;
            //        try
            //        {
            //            if (new SC_TXT(thDataWork).Open())
            //            {
            //                ret = true;
            //            }
            //        }
            //        catch { }
            //    }
            //}
            //thDataWork.Main.ProgressInfo(false);
            //return ret;
        }

        protected bool SaveFiles()
        {
            return OpenSaveFilesBase(WorkTXTDir, new SC_TXT(thDataWork), "*.txt");

            //var ret = false;
            //foreach (var txt in Directory.EnumerateFiles(Properties.Settings.Default.THProjectWorkDir, "*.txt"))
            //{
            //    if (Path.GetFileName(txt).StartsWith("sc_"))
            //    {
            //        thDataWork.Main.ProgressInfo(true, T._("Save") + ": " + Path.GetFileName(txt));
            //        thDataWork.FilePath = txt;
            //        try
            //        {
            //            if (new SC_TXT(thDataWork).Save())
            //            {
            //                ret = true;
            //            }
            //        }
            //        catch { }
            //    }
            //}
            //thDataWork.Main.ProgressInfo(false);
            //return ret;
        }

        protected string SCPACKpak = string.Empty;
        protected string SCPACKidx = string.Empty;
        protected string WorkTXTDir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "txt");
        internal override bool BakCreate()
        {
            return BackupFile(SCPACKpak) && BackupFile(SCPACKidx) && BackupDir(WorkTXTDir);
        }

        internal override bool BakRestore()
        {
            return RestoreFile(SCPACKpak) && RestoreFile(SCPACKidx) && RestoreDir(WorkTXTDir);
        }
    }
}
