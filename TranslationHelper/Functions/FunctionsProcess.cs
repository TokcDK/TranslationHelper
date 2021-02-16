using System;
using System.Diagnostics;
using System.IO;

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
                var ExeInfo = FileVersionInfo.GetVersionInfo(exepath);
                return ExeInfo.FileDescription;
            }
        }

        /// <summary>
        /// same as RunProcess(). Run selected programm.
        /// </summary>
        /// <param name="ProgramPath"></param>
        /// <param name="Arguments"></param>
        /// <param name="WorkDir"></param>
        /// <returns></returns>
        public static bool RunProgram(string ProgramPath, string Arguments = "", string WorkDir = "")
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

            return RunProcess(ProgramPath, Arguments, WorkDir);
        }

        /// <summary>
        /// Run selected programm
        /// </summary>
        /// <param name="ProgramPath"></param>
        /// <param name="Arguments"></param>
        /// <param name="WorkDir"></param>
        /// <returns></returns>
        public static bool RunProcess(string ProgramPath, string Arguments = "", string WorkDir = "")
        {
            bool ret = false;
            if (File.Exists(ProgramPath))
            {
                using (Process Program = new Process())
                {
                    Program.StartInfo.ErrorDialog = true;
                    Program.EnableRaisingEvents = true;
                    //MessageBox.Show("outdir=" + outdir);
                    Program.StartInfo.FileName = ProgramPath;
                    if (Arguments.Length > 0)
                    {
                        Program.StartInfo.Arguments = Arguments;
                    }
                    Program.StartInfo.WorkingDirectory = WorkDir.Length == 0 ? Path.GetDirectoryName(ProgramPath) : WorkDir;

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

                    ret = Program.Start();
                    Program.WaitForExit();

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
            if (Directory.Exists(folder = Properties.Settings.Default.THProjectWorkDir))
            {
            }
            else
            {
                folder = Properties.Settings.Default.THSelectedDir;
            }
            Process.Start("explorer.exe", folder);
        }
    }
}
