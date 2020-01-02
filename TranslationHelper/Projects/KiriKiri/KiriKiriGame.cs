using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.KiriKiri
{
    class KiriKiriGame : ProjectBase
    {
        public KiriKiriGame(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool OpenDetect()
        {
            throw new NotImplementedException();
        }

        internal override string ProjectTitle()
        {
            throw new NotImplementedException();
        }

        internal override string ProjecFolderName()
        {
            return "KiriKiri";
        }

        internal override bool Open()
        {
            bool ret = false;
            if (ExtractXP3files(thDataWork.SPath))
            {
                var KiriKiriFiles = new List<string>();
                string DirName = Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath));
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

                    thDataWork.FilePath = kiriKiriFiles[i];

                    _ = thDataWork.THFilesElementsDataset.Tables.Add(filename);
                    _ = thDataWork.THFilesElementsDataset.Tables[filename].Columns.Add("Original");
                    _ = thDataWork.THFilesElementsDatasetInfo.Tables.Add(filename);
                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[filename].Columns.Add("Original");

                    if (filename.EndsWith(".ks") || filename.EndsWith(".scn") || filename.EndsWith(".tjs"))
                    {
                        ret = new SCRIPT(thDataWork).Open();
                    }
                    else if (filename.EndsWith(".csv"))
                    {
                        ret = new CSV(thDataWork).Open();
                    }
                    else if (filename.EndsWith(".tsv"))
                    {
                        ret = new TSV(thDataWork).Open();
                    }

                    //if (DT == null || DT.Rows.Count == 0)
                    //{
                    //    thDataWork.THFilesElementsDataset.Tables.Remove(filename);
                    //    thDataWork.THFilesElementsDatasetInfo.Tables.Remove(filename);
                    //}
                    //else
                    //{
                    //    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                    //    _ = thDataWork.THFilesElementsDataset.Tables[filename].Columns.Add("Translation");
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
