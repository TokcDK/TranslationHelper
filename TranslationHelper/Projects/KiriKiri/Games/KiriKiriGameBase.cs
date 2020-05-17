using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    abstract class KiriKiriGameBase : ProjectBase
    {
        protected KiriKiriGameBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected bool DetectBaseFiles()
        {
            return
            Path.GetExtension(thDataWork.SPath).ToUpperInvariant() == ".EXE"
                &&
                FunctionsProcess.GetExeDescription(thDataWork.SPath) != null
                &&
                FunctionsProcess.GetExeDescription(thDataWork.SPath).ToUpper(CultureInfo.GetCultureInfo("en-US")).Contains("KIRIKIRI")
                &&
                FunctionsFileFolder.IsInDirExistsAnyFile(Path.GetDirectoryName(thDataWork.SPath), "*.xp3");
        }


        internal string[] ProjectXP3List;

        protected void ExtractXP3Data()
        {
            //Properties.Settings.Default.THSelectedGameDir

            string KiriKiriEXEpath = Path.Combine(Application.StartupPath, "Res", "kirikiriunpacker", "kikiriki.exe");
            string DirName = Path.GetFileName(Properties.Settings.Default.THSelectedGameDir);
            string KiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", DirName);
            Properties.Settings.Default.THProjectWorkDir = KiriKiriWorkFolder;


            if (!Directory.Exists(KiriKiriWorkFolder))
            {
                Directory.CreateDirectory(KiriKiriWorkFolder);
            }

            ProjectXP3List = GetDataFiles();

            foreach (var name in ProjectXP3List)
            {
                string xp3path = Path.Combine(Properties.Settings.Default.THSelectedGameDir, name);
                if (File.Exists(xp3path))
                {
                    string targetSubFolder = KiriKiriWorkFolder + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(xp3path);
                    string KiriKiriEXEargs = "-i \"" + xp3path + "\" -o \"" + targetSubFolder + "\"";
                    if (!Directory.Exists(targetSubFolder))
                    {
                        Directory.CreateDirectory(targetSubFolder);
                    }
                    else
                    {
                        if (FunctionsFileFolder.IsInDirExistsAnyFile(targetSubFolder, "*.*"))
                        {
                            continue;
                        }
                    }

                    FunctionsProcess.RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);
                }
            }

        }

        protected void PackTranslatedFilesInPatch()
        {
            //PatchDir
            var PatchDir = Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "_patch"));

            if (!FunctionsFileFolder.IsInDirExistsAnyFile(PatchDir.FullName))
            {
                return;
            }

            string KiriKiriEXEpath = Path.Combine(Application.StartupPath, "Res", "kirikiriunpacker", "kikiriki.exe");


            if (ProjectXP3List == null)
            {
                ProjectXP3List = GetDataFiles();
            }

            string PatchName = "patch" + GetLastIndexOfPatch();

            string KiriKiriEXEargs = "-c -i \"" + PatchDir.FullName + "\" -o \"" + Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchName + ".xp3") + "\"";

            FunctionsProcess.RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);

            if (File.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchName + ".xp3")))
            {
                Directory.Move(PatchDir.FullName, Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchName));
                File.Copy(Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchName + ".xp3"), Path.Combine(Properties.Settings.Default.THSelectedGameDir, PatchName + ".xp3"));
            }
        }

        protected string GetLastIndexOfPatch()
        {
            string name;
            for (int i = 99; i > 0; i--)
            {
                name = "patch" + i + ".xp3";
                if (!File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name)))
                {
                    return i + string.Empty;
                }
            }

            return string.Empty;
        }

        protected string[] GetDataFiles()
        {
            List<string> dataFiles = new List<string>();
            if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, "data.xp3")))
            {
                dataFiles.Add("data.xp3");
            }

            int number = -1;
            int notFound = 0;
            string name;
            while (number < 99 && notFound < 10)
            {
                number++;
                foreach (var prefix in new[] { " ", "_", string.Empty })
                {
                    if (number > 0)
                    {
                        name = "patch" + prefix + number + ".xp3";
                    }
                    else
                    {
                        name = "patch.xp3";
                    }

                    if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name)))
                    {
                        if (File.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, name)))
                        {
                            File.Delete(Path.Combine(Properties.Settings.Default.THSelectedGameDir, name));
                            File.Delete(Path.Combine(Properties.Settings.Default.THProjectWorkDir, name));
                            Directory.Delete(Path.Combine(Properties.Settings.Default.THProjectWorkDir, name.Replace(".xp3", string.Empty)));
                        }
                        else
                        {
                            dataFiles.Add(name);
                        }
                    }
                    else
                    {
                        notFound++;
                    }
                }
            }

            return dataFiles.ToArray();
        }
    }
}
