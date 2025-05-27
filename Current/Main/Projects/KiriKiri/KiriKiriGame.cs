using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats;
using TranslationHelper.Formats.KiriKiri;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.KiriKiri
{
    internal class KiriKiriGame : ProjectBase
    {
        public KiriKiriGame()
        {
        }

        internal override bool IsValid()
        {
            return false; // old code
            //Path.GetExtension(ProjectData.SPath) == ".exe"
            //    &&
            //    FunctionsProcess.GetExeDescription(ProjectData.SPath) != null
            //    &&
            //    FunctionsProcess.GetExeDescription(ProjectData.SPath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")
            //    &&
            //    FunctionsFileFolder.IsInDirExistsAnyFile(Path.GetDirectoryName(ProjectData.SPath), "*.xp3");
        }

        internal override string FileFilter => ProjectTools.GameExeFilter;

        public override string Name => "KiriKiri Game";

        internal override string ProjectDBFolderName => "KiriKiri";

        readonly (string Pattern, Type FileType)[] _fileTypes = new (string Pattern, Type FileType)[]
        {
                ("*.ks", typeof(SCRIPT)),
                ("*.tjs", typeof(SCRIPT)),
                ("*.scn", typeof(SCRIPT)),
                ("*.csv", typeof(CSV)),
                ("*.tsv", typeof(TSV))
        };

        protected override bool TryOpen()
        {
            if (!ExtractXP3files(AppData.SelectedProjectFilePath))
            {
                return false;
            }

            string dirName = Path.GetFileName(Path.GetDirectoryName(AppData.SelectedProjectFilePath));
            string workFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", dirName);
            var dirInfo = new DirectoryInfo(workFolder);
            bool result = false;

            foreach (var (pattern, fileType) in _fileTypes)
            {
                var files = dirInfo.EnumerateFiles(pattern, SearchOption.AllDirectories);
                if (files.Any() && this.OpenSaveFilesBase(files.Select(f => (f, fileType))))
                {
                    result = true;
                }
            }

            return result;
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

        protected override bool TrySave()
        {
            throw new NotImplementedException();
        }
    }
}