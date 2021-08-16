using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
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
            return false;
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
            if (ExtractXp3Files(ProjectData.SelectedFilePath))
            {
                var kiriKiriFiles = new List<string>();
                string dirName = Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath));
                string kiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", dirName);

                foreach (FileInfo file in (new DirectoryInfo(kiriKiriWorkFolder)).EnumerateFiles("*.scn", SearchOption.AllDirectories))
                {
                    kiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(kiriKiriWorkFolder)).EnumerateFiles("*.ks", SearchOption.AllDirectories))
                {
                    kiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(kiriKiriWorkFolder)).EnumerateFiles("*.csv", SearchOption.AllDirectories))
                {
                    kiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(kiriKiriWorkFolder)).EnumerateFiles("*.tsv", SearchOption.AllDirectories))
                {
                    kiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(kiriKiriWorkFolder)).EnumerateFiles("*.tjs", SearchOption.AllDirectories))
                {
                    kiriKiriFiles.Add(file.FullName);
                }

                if (KiriKiriGameOpen(kiriKiriFiles))
                {
                    return true;
                }
            }

            return false;
        }

        private bool KiriKiriGameOpen(List<string> kiriKiriFiles)
        {
            string filename;

            try
            {
                for (int i = 0; i < kiriKiriFiles.Count; i++)
                {
                    filename = Path.GetFileName(kiriKiriFiles[i]);

                    bool ret = false;

                    ProjectData.FilePath = kiriKiriFiles[i];

                    //_ = ProjectData.THFilesElementsDataset.Tables.Add(filename);
                    //_ = ProjectData.THFilesElementsDataset.Tables[filename].Columns.Add("Original");
                    //_ = ProjectData.THFilesElementsDatasetInfo.Tables.Add(filename);
                    //_ = ProjectData.THFilesElementsDatasetInfo.Tables[filename].Columns.Add("Original");

                    if (filename.EndsWith(".ks") || filename.EndsWith(".scn") || filename.EndsWith(".tjs"))
                    {
                        ret = new Script().Open();
                    }
                    else if (filename.EndsWith(".csv"))
                    {
                        ret = new Csv().Open();
                    }
                    else if (filename.EndsWith(".tsv"))
                    {
                        ret = new Tsv().Open();
                    }

                    //if (DT == null || DT.Rows.Count == 0)
                    //{
                    //    ProjectData.THFilesElementsDataset.Tables.Remove(filename);
                    //    ProjectData.THFilesElementsDatasetInfo.Tables.Remove(filename);
                    //}
                    //else
                    //{
                    //    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                    //    _ = ProjectData.THFilesElementsDataset.Tables[filename].Columns.Add("Translation");
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

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}