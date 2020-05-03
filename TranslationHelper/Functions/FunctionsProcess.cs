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

        public static bool RunProgram(string ProgramPath, string Arguments = "")
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

            return RunProcess(ProgramPath, Arguments);
        }

        public static bool RunProcess(string ProgramPath, string Arguments = "")
        {
            bool ret = false;
            if (File.Exists(ProgramPath))
            {
                using (Process Program = new Process())
                {
                    //MessageBox.Show("outdir=" + outdir);
                    Program.StartInfo.FileName = ProgramPath;
                    if (Arguments.Length > 0)
                    {
                        Program.StartInfo.Arguments = Arguments;
                    }
                    Program.StartInfo.WorkingDirectory = Path.GetDirectoryName(ProgramPath);

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
    }
}
