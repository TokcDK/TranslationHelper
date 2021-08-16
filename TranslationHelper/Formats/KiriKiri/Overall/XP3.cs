using System.IO;
using System.Windows.Forms;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    public static class Xp3
    {
        public static bool ExtractXp3Files2(string sPath)
        {
            bool ret = false;

            string kiriKiriExEpath = Path.Combine(Application.StartupPath, "Res", "kirikiriunpacker", "kikiriki.exe");
            string dirName = Path.GetFileName(Path.GetDirectoryName(sPath));
            string kiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", dirName);
            DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(sPath) + "\\");
            string xp3Name = "data";
            string xp3Path = Path.Combine(directory.FullName, xp3Name + ".xp3");
            string kiriKiriExEargs = "-i \"" + xp3Path + "\" -o \"" + kiriKiriWorkFolder + "\"";

            if (File.Exists(Path.Combine(dirName, "Data.xp3")))
            {

            }


            return ret;
        }


        public static bool ExtractXp3Files(string sPath)
        {
            bool ret = false;

            try
            {
                string kiriKiriExEpath = Path.Combine(Application.StartupPath, "Res", "kirikiriunpacker", "kikiriki.exe");
                string dirName = Path.GetFileName(Path.GetDirectoryName(sPath));
                string kiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", dirName);
                DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(sPath) + "\\");
                string xp3Name = "data";
                string xp3Path = Path.Combine(directory.FullName, xp3Name + ".xp3");
                string kiriKiriExEargs = "-i \"" + xp3Path + "\" -o \"" + kiriKiriWorkFolder + "\"";

                if (Directory.Exists(kiriKiriWorkFolder))
                {
                    if ((new DirectoryInfo(kiriKiriWorkFolder + Path.DirectorySeparatorChar)).GetFiles("*", SearchOption.AllDirectories).Length > 0)
                    {
                        DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            ret = true;
                        }
                        else
                        {
                            //Удаление и пересоздание папки
                            Directory.Delete(kiriKiriWorkFolder, true);
                            Directory.CreateDirectory(kiriKiriWorkFolder);

                            ret = FunctionsProcess.RunProcess(kiriKiriExEpath, kiriKiriExEargs);
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
                        ret = FunctionsProcess.RunProcess(kiriKiriExEpath, kiriKiriExEargs);
                    }
                }
                else
                {
                    Directory.CreateDirectory(kiriKiriWorkFolder);
                    ret = FunctionsProcess.RunProcess(kiriKiriExEpath, kiriKiriExEargs);
                }
            }
            catch
            {

            }

            return ret;
        }
    }
}
