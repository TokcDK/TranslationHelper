using System.IO;
using System.Windows.Forms;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    public static class XP3
    {
        public static bool ExtractXP3files2(string sPath)
        {
            bool ret = false;

            string KiriKiriEXEpath = Path.Combine(Application.StartupPath, "Res", "kirikiriunpacker", "kikiriki.exe");
            string DirName = Path.GetFileName(Path.GetDirectoryName(sPath));
            string KiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", DirName);
            DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(sPath) + "\\");
            string xp3name = "data";
            string xp3path = Path.Combine(directory.FullName, xp3name + ".xp3");
            string KiriKiriEXEargs = "-i \"" + xp3path + "\" -o \"" + KiriKiriWorkFolder + "\"";

            if (File.Exists(Path.Combine(DirName, "Data.xp3")))
            {

            }


            return ret;
        }


        public static bool ExtractXP3files(string sPath)
        {
            bool ret = false;

            try
            {
                string KiriKiriEXEpath = Path.Combine(Application.StartupPath, "Res", "kirikiriunpacker", "kikiriki.exe");
                string DirName = Path.GetFileName(Path.GetDirectoryName(sPath));
                string KiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", DirName);
                DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(sPath) + "\\");
                string xp3name = "data";
                string xp3path = Path.Combine(directory.FullName, xp3name + ".xp3");
                string KiriKiriEXEargs = "-i \"" + xp3path + "\" -o \"" + KiriKiriWorkFolder + "\"";

                if (Directory.Exists(KiriKiriWorkFolder))
                {
                    if ((new DirectoryInfo(KiriKiriWorkFolder + Path.DirectorySeparatorChar)).GetFiles("*", SearchOption.AllDirectories).Length > 0)
                    {
                        DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            ret = true;
                        }
                        else
                        {
                            //Удаление и пересоздание папки
                            Directory.Delete(KiriKiriWorkFolder, true);
                            Directory.CreateDirectory(KiriKiriWorkFolder);

                            ret = FunctionsProcess.RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);
                            //if (RunProcess(KiriKiriEXEpath, KiriKiriEXEargs))
                            //{
                            //    xp3name = "patch";
                            //    xp3path = Path.Combine(directory.FullName, xp3name + ".xp3");
                            //    ret = RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);
                            //}
                        }
                    }
                    else
                    {
                        ret = FunctionsProcess.RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);
                    }
                }
                else
                {
                    Directory.CreateDirectory(KiriKiriWorkFolder);
                    ret = FunctionsProcess.RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);
                }
            }
            catch
            {

            }

            return ret;
        }
    }
}
