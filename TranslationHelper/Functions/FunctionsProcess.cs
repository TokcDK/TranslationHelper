using System.Diagnostics;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Main.Functions
{
    class FunctionsProcess
    {
        public static string GetExeDescription(string exepath)
        {
            if (exepath.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                var exeInfo = FileVersionInfo.GetVersionInfo(exepath);
                return exeInfo.FileDescription;
            }
        }

        /// <summary>
        /// same as RunProcess(). Run selected programm.
        /// </summary>
        /// <param name="programPath"></param>
        /// <param name="arguments"></param>
        /// <param name="workDir"></param>
        /// <returns></returns>
        public static bool RunProgram(string programPath, string arguments = "", string workDir = "", bool createNoWindow = false, bool useShellExecute = true)
        {
            //bool ret = false;
            //using (Process Program = new Process())
            //{
            //    //MessageBox.Show("outdir=" + outdir);
            //    Program.StartInfo.FileName = ProgramPath;
            //    Program.StartInfo.Arguments = Arguments;
            //    ret = Program.Start();
            //    Program.WaitForExit();
            //}

            return RunProcess(programPath, arguments, workDir, createNoWindow, useShellExecute);
        }

        /// <summary>
        /// Run selected programm
        /// </summary>
        /// <param name="programPath"></param>
        /// <param name="arguments"></param>
        /// <param name="workDir"></param>
        /// <returns></returns>
        public static bool RunProcess(string programPath, string arguments = "", string workDir = "", bool createNoWindow = false, bool useShellExecute = true)
        {
            bool ret = false;
            if (File.Exists(programPath))
            {
                using (Process program = new Process())
                {
                    program.StartInfo.ErrorDialog = true;
                    program.EnableRaisingEvents = true;
                    program.StartInfo.CreateNoWindow = createNoWindow;
                    program.StartInfo.UseShellExecute = useShellExecute;

                    //MessageBox.Show("outdir=" + outdir);
                    program.StartInfo.FileName = programPath;
                    if (arguments.Length > 0)
                    {
                        program.StartInfo.Arguments = arguments;
                    }
                    program.StartInfo.WorkingDirectory = workDir.Length == 0 ? Path.GetDirectoryName(programPath) : workDir;

                    //http://www.cyberforum.ru/windows-forms/thread31052.html
                    // свернуть
                    //WindowState = FormWindowState.Minimized;
                    //if (LinksForm == null || LinksForm.IsDisposed)
                    //{
                    //}
                    //else
                    //{
                    //    LinksForm.WindowState = FormWindowState.Minimized;
                    //}

                    ret = program.Start();
                    program.WaitForExit();

                    // Показать
                    //WindowState = FormWindowState.Normal;
                    //if (LinksForm == null || LinksForm.IsDisposed)
                    //{
                    //}
                    //else
                    //{
                    //    LinksForm.WindowState = FormWindowState.Normal;
                    //}
                }
            }

            return ret;
        }

        /// <summary>
        /// open project's dir
        /// </summary>
        internal static void OpenProjectsDir()
        {
            string folder;
            if (Directory.Exists(folder = ProjectData.ProjectWorkDir))
            {
            }
            else
            {
                folder = ProjectData.SelectedDir;
            }
            Process.Start("explorer.exe", folder);
        }

        /// <summary>
        /// execute with cmd.exe
        /// </summary>
        /// <param name="programexe"></param>
        /// <param name="arguments"></param>
        /// <param name="workdir"></param>
        /// <param name="createNoWindow"></param>
        /// <param name="useShellExecute"></param>
        /// <returns></returns>
        internal static bool RunCmd(string programexe, string arguments, string workdir, bool createNoWindow = false, bool useShellExecute = true)
        {
            return RunBat(programexe, arguments, workdir, createNoWindow, useShellExecute);
        }

        /// <summary>
        /// execute with cmd.exe
        /// </summary>
        /// <param name="programexe"></param>
        /// <param name="arguments"></param>
        /// <param name="workdir"></param>
        /// <param name="createNoWindow"></param>
        /// <param name="useShellExecute"></param>
        /// <returns></returns>
        internal static bool RunBat(string cmdline, string workdir, bool createNoWindow = false, bool useShellExecute = true)
        {
            return RunProcess("cmd.exe", "\\C " + cmdline, workdir, createNoWindow, useShellExecute);
        }

        /// <summary>
        /// execute with cmd.exe
        /// </summary>
        /// <param name="programexe"></param>
        /// <param name="arguments"></param>
        /// <param name="workdir"></param>
        /// <param name="createNoWindow"></param>
        /// <param name="useShellExecute"></param>
        /// <returns></returns>
        internal static bool RunBat(string programexe, string arguments, string workdir, bool createNoWindow = false, bool useShellExecute = true)
        {
            arguments = "\\C \"\"" + programexe + "\"\" " + arguments;
            return RunProcess("cmd.exe", arguments, workdir, createNoWindow, useShellExecute);
        }
    }
}
