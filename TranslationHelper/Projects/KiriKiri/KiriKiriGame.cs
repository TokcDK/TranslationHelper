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
        public KiriKiriGame(ProjectData projectData) : base(projectData)
        {
        }

        internal override bool Check()
        {
            return false;
            //Path.GetExtension(projectData.SPath) == ".exe"
            //    &&
            //    FunctionsProcess.GetExeDescription(projectData.SPath) != null
            //    &&
            //    FunctionsProcess.GetExeDescription(projectData.SPath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")
            //    &&
            //    FunctionsFileFolder.IsInDirExistsAnyFile(Path.GetDirectoryName(projectData.SPath), "*.xp3");
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
            if (ExtractXP3files(projectData.SPath))
            {
                var KiriKiriFiles = new List<string>();
                string DirName = Path.GetFileName(Path.GetDirectoryName(projectData.SPath));
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

        private bool KiriKiriGameOpen(List<string> kiriKiriFiles)
        {
            string filename;

            try
            {
                for (int i = 0; i < kiriKiriFiles.Count; i++)
                {
                    filename = Path.GetFileName(kiriKiriFiles[i]);

                    bool ret = false;

                    projectData.FilePath = kiriKiriFiles[i];

                    //_ = projectData.THFilesElementsDataset.Tables.Add(filename);
                    //_ = projectData.THFilesElementsDataset.Tables[filename].Columns.Add("Original");
                    //_ = projectData.THFilesElementsDatasetInfo.Tables.Add(filename);
                    //_ = projectData.THFilesElementsDatasetInfo.Tables[filename].Columns.Add("Original");

                    if (filename.EndsWith(".ks") || filename.EndsWith(".scn") || filename.EndsWith(".tjs"))
                    {
                        ret = new SCRIPT(projectData).Open();
                    }
                    else if (filename.EndsWith(".csv"))
                    {
                        ret = new CSV(projectData).Open();
                    }
                    else if (filename.EndsWith(".tsv"))
                    {
                        ret = new TSV(projectData).Open();
                    }

                    //if (DT == null || DT.Rows.Count == 0)
                    //{
                    //    projectData.THFilesElementsDataset.Tables.Remove(filename);
                    //    projectData.THFilesElementsDatasetInfo.Tables.Remove(filename);
                    //}
                    //else
                    //{
                    //    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                    //    _ = projectData.THFilesElementsDataset.Tables[filename].Columns.Add("Translation");
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