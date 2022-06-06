using System;
using System.Collections.Generic;
using System.IO;
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

        internal override bool Check()
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

        internal override string Filters()
        {
            return GameExeFilter;
        }

        internal override string Name()
        {
            return "KiriKiri Game";
        }

        internal override string ProjectFolderName()
        {
            return "KiriKiri";
        }

        internal override bool Open()
        {
            if (ExtractXP3files(AppData.SelectedFilePath))
            {
                var KiriKiriFiles = new List<string>();
                string DirName = Path.GetFileName(Path.GetDirectoryName(AppData.SelectedFilePath));
                string KiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", DirName);

                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).EnumerateFiles("*.scn", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).EnumerateFiles("*.ks", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).EnumerateFiles("*.csv", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).EnumerateFiles("*.tsv", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).EnumerateFiles("*.tjs", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }

                if (KiriKiriGameOpen(KiriKiriFiles))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool KiriKiriGameOpen(List<string> kiriKiriFiles)
        {
            string filename;

            try
            {
                for (int i = 0; i < kiriKiriFiles.Count; i++)
                {
                    filename = Path.GetFileName(kiriKiriFiles[i]);

                    bool ret = false;

                    //ProjectData.FilePath = kiriKiriFiles[i];

                    //_ = ProjectData.THFilesElementsDataset.Tables.Add(filename);
                    //_ = ProjectData.THFilesElementsDataset.Tables[filename].Columns.Add(THSettings.OriginalColumnName());
                    //_ = ProjectData.THFilesElementsDatasetInfo.Tables.Add(filename);
                    //_ = ProjectData.THFilesElementsDatasetInfo.Tables[filename].Columns.Add(THSettings.OriginalColumnName());

                    FormatBase format = null;
                    if (filename.EndsWith(".ks") || filename.EndsWith(".scn") || filename.EndsWith(".tjs"))
                    {
                        format = new SCRIPT();
                    }
                    else if (filename.EndsWith(".csv"))
                    {
                        format = new CSV();
                    }
                    else if (filename.EndsWith(".tsv"))
                    {
                        format = new TSV();
                    }

                    format.FilePath = kiriKiriFiles[i];
                    ret = format.Open();

                    //if (DT == null || DT.Rows.Count == 0)
                    //{
                    //    ProjectData.THFilesElementsDataset.Tables.Remove(filename);
                    //    ProjectData.THFilesElementsDatasetInfo.Tables.Remove(filename);
                    //}
                    //else
                    //{
                    //    THFilesList.Invoke((Action)(() => THFilesList.AddItem(filename)));
                    //    _ = ProjectData.THFilesElementsDataset.Tables[filename].Columns.Add(THSettings.TranslationColumnName());
                    //}
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
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

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}