using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;
using TranslationHelper.Projects.KiriKiri;
using TranslationHelper.Projects.RJ263914;
using TranslationHelper.Projects.RPGMaker;
using TranslationHelper.Projects.RPGMMV;
using TranslationHelper.Projects.RPGMTrans;
using TranslationHelper.Projects.WolfRPG;

namespace TranslationHelper.Functions
{
    class FunctionsOpen
    {
        THDataWork thDataWork;
        public FunctionsOpen(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal async void OpenProject()
        {
            if (thDataWork.Main.IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                thDataWork.Main.IsOpeningInProcess = true;

                //об сообщении Освобождаемый объект никогда не освобождается и почему using здесь
                //https://stackoverflow.com/questions/2926869/do-you-need-to-dispose-of-objects-and-set-them-to-null
                using (OpenFileDialog THFOpen = new OpenFileDialog())
                {
                    THFOpen.InitialDirectory = thDataWork.Main.Settings.THConfigINI.ReadINI("Paths", "LastPath");
                    THFOpen.Filter = "All compatible|*.exe;RPGMKTRANSPATCH;*.json;*.scn;*.ks|RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker execute(*.exe)|*.exe|KiriKiri engine files|*.scn;*.ks|Txt file|*.txt";

                    if (THFOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (THFOpen.OpenFile() != null)
                        {
                            //THActionProgressBar.Visible = true;
                            thDataWork.Main.ProgressInfo(true, T._("opening.."));

                            new CleanupData(thDataWork).THCleanupThings();

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            //Thread open = new Thread(new ParameterizedThreadStart((obj) => GetSourceType(THFOpen.FileName)));
                            //open.Start();

                            bool newOpen = false;
                            thDataWork.SPath = THFOpen.FileName;

                            if (newOpen)
                            {
                                if (await Task.Run(() => TryToDetectSourceAndOpen()).ConfigureAwait(true))
                                {
                                    AfterOpenActions();

                                    return;
                                }
                                else
                                {
                                    /*THMsg*/
                                    MessageBox.Show(T._("Problem with source opening. Try to report to devs about it."));

                                    return;
                                }
                            }

                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            await Task.Run(() => RPGMFunctions.THSelectedSourceType = GetSourceType(THFOpen.FileName)).ConfigureAwait(true);

                            //THSelectedSourceType = GetSourceType(THFOpen.FileName);

                            //THActionProgressBar.Visible = false;
                            thDataWork.Main.ProgressInfo(false, string.Empty);

                            if (RPGMFunctions.THSelectedSourceType.Length == 0)
                            {
                                /*THMsg*/
                                MessageBox.Show(T._("Problem with source opening. Try to report to devs about it."));
                            }
                            else
                            {
                                //if (THSelectedSourceType == "RPG Maker MV")
                                //{
                                //    THMakeRPGMakerMVWorkProjectDir(THFOpen.FileName);
                                //}

                                //Попытка добавить открытие сразу всех таблиц в одной
                                //if (setAsDatasourceAllToolStripMenuItem.Visible)
                                //{
                                //    for (int c = 0; c < THFilesElementsDataset.Tables[0].Columns.Count; c++)
                                //    {
                                //        THFilesElementsALLDataTable.Columns.Add(THFilesElementsDataset.Tables[0].Columns[c].ColumnName);//asdfgh
                                //    }

                                //    for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
                                //    {
                                //        for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
                                //        {
                                //            THFilesElementsALLDataTable.Rows.Add(THFilesElementsDataset.Tables[t].Rows[r].ItemArray);
                                //        }
                                //    }
                                //}

                                AfterOpenActions();
                            }

                        }
                    }
                }

                thDataWork.Main.IsOpeningInProcess = false;
            }
        }

        private bool TryToDetectSourceAndOpen()
        {
            foreach (var Project in thDataWork.ProjectsList)
            {
                if (Project.OpenDetect())
                {
                    if (Project.Open())
                    {
                        if (thDataWork.THFilesElementsDataset.Tables.Count > 0)
                        {
                            thDataWork.Project = Project;
                            RPGMFunctions.THSelectedSourceType = Project.ProjectTitle();
                            foreach (DataTable file in thDataWork.THFilesElementsDataset.Tables)
                            {
                                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFilesList.Items.Add(file.TableName)));
                            }
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static string GetCorrectedGameDIr(string tHSelectedGameDir)
        {
            if (tHSelectedGameDir.Length == 0)
            {
                tHSelectedGameDir = Properties.Settings.Default.THSelectedDir;
            }

            //для rpgmaker mv. если была папка data, которая в папке www
            string pFolderName = Path.GetFileName(tHSelectedGameDir);
            if (string.Compare(pFolderName, "data", true, CultureInfo.GetCultureInfo("en-US")) == 0)
            {
                return Path.GetDirectoryName(Path.GetDirectoryName(tHSelectedGameDir));
            }
            return tHSelectedGameDir;
        }

        internal DirectoryInfo mvdatadir;
        private string GetSourceType(string sPath)
        {
            thDataWork.SPath = sPath;
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            Properties.Settings.Default.THSelectedDir = dir.FullName;
            Properties.Settings.Default.THSelectedGameDir = dir.FullName;

            //ShowProjectsList();

            //Try detect and open new type projects
            foreach (ProjectBase Project in thDataWork.ProjectsList)
            {
                if (TryDetectProject(Project))
                {
                    return TryOpenProject();
                }
            }

            //Old projects
            return TryDetectOpenOldProjects(sPath);
        }

        private void ShowProjectsList()
        {
            StringBuilder titles = new StringBuilder();
            foreach (ProjectBase Project in thDataWork.ProjectsList)
            {
                titles.AppendLine(Project.ProjectTitle());
            }
            MessageBox.Show(titles.ToString());
        }

        private string TryDetectOpenOldProjects(string sPath)
        {
            var dir = Properties.Settings.Default.THSelectedDir;

            if (new KiriKiriOLD(thDataWork).OpenDetect())
            {
                return new KiriKiriOLD(thDataWork).KiriKiriScriptScenario();
            }
            else if (sPath.ToUpper(CultureInfo.GetCultureInfo("en-US")).EndsWith(".TJS") || sPath.ToUpper(CultureInfo.GetCultureInfo("en-US")).EndsWith(".SCN"))
            {
                return new KiriKiriOLD(thDataWork).KiriKiriScenarioOpen(sPath);
            }
            else if (sPath.ToUpper(CultureInfo.GetCultureInfo("en-US")).EndsWith(".TXT"))
            {
                return new WRPGOLDOpen(thDataWork).AnyTxt(sPath);
            }
            else if (sPath.ToUpper(CultureInfo.GetCultureInfo("en-US")).Contains("\\RPGMKTRANSPATCH"))
            {
                return new RPGMTransOLD(thDataWork).RPGMTransPatchPrepare(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToUpper(CultureInfo.GetCultureInfo("en-US")).Contains(".JSON"))
            {
                if (new RPGMMVOLD(thDataWork).OpenRPGMakerMVjson(sPath))
                {
                    //for (int i = 0; i < thDataWork.THFilesElementsDataset.Tables.Count; i++)
                    //{
                    //    thDataWork.Main.THFilesList.Invoke((Action)(() => thDataWork.Main.THFilesList.Items.Add(thDataWork.THFilesElementsDataset.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    //}
                    return "RPG Maker MV json";
                }
            }
            else if (Path.GetExtension(sPath) == ".exe" /*sPath.ToLower().Contains("\\game.exe") || dir.GetFiles("*.exe").Length > 0*/)
            {
                if (Directory.Exists(Path.Combine(Path.GetDirectoryName(sPath), "data", "bin")))
                {
                    return new RJ263914OLD(thDataWork).ProceedRubyRPGGame(Path.GetDirectoryName(sPath));//RJ263914
                }
                else if ((FunctionsProcess.GetExeDescription(sPath) != null && FunctionsProcess.GetExeDescription(sPath).ToUpper(CultureInfo.GetCultureInfo("en-US")).Contains("KIRIKIRI")) && FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.xp3"))
                {
                    if (new KiriKiriOLD(thDataWork).KiriKiriGame())
                    {
                        return "KiriKiri game";
                    }
                }
                else if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data", "system.json")))
                {
                    try
                    {
                        //Properties.Settings.Default.THSelectedDir += "\\www\\data";
                        //var MVJsonFIles = new List<string>();
                        mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data/")));
                        foreach (FileInfo file in mvdatadir.EnumerateFiles("*.json"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            //MVJsonFIles.Add(file.FullName);

                            if (new RPGMMVOLD(thDataWork).OpenRPGMakerMVjson(file.FullName))
                            {
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }

                        for (int i = 0; i < thDataWork.THFilesElementsDataset.Tables.Count; i++)
                        {
                            //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);
                            thDataWork.Main.THFilesList.Invoke((Action)(() => thDataWork.Main.THFilesList.Items.Add(thDataWork.THFilesElementsDataset.Tables[i].TableName)));
                        }

                        return "RPG Maker MV";
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                else if (File.Exists(Path.Combine(dir, "Data", "System.rvdata2"))
                    || File.Exists(Path.Combine(dir, "Data", "System.rxdata"))
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rgss3a")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rgss2a")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rvdata")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rgssad")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rxdata")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.lmt")
                    || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.lmu"))
                {
                    thDataWork.Main.extractedpatchpath = string.Empty;
                    bool result = new RPGMGameOLD(thDataWork).TryToExtractToRPGMakerTransPatch(sPath);
                    //MessageBox.Show("result=" + result);
                    //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                    if (result)
                    {
                        //Cleaning of the type
                        //THRPGMTransPatchFiles.Clear();
                        //THFilesElementsDataset.Clear();

                        //var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
                        //string patchver;
                        bool isv3 = Directory.Exists(thDataWork.Main.extractedpatchpath + Path.DirectorySeparatorChar + "patch");
                        //MessageBox.Show("isv3=" + isv3+ ", patchdir="+ extractedpatchpath+ ", extractedpatchpath="+ extractedpatchpath);
                        if (isv3) //если есть подпапка patch, тогда это версия патча 3
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "3";
                            thDataWork.Main.extractedpatchpath += Path.DirectorySeparatorChar + "patch";
                            //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                            dir = new DirectoryInfo(Path.GetDirectoryName(thDataWork.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName; //Два слеша здесь в конце исправляют проблему возврата информации о неверной папке
                                                                                                                       //MessageBox.Show("patchdir1=" + patchdir);
                        }
                        else if (Directory.Exists(thDataWork.Main.extractedpatchpath + Path.GetFileName(thDataWork.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch"))
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "3";
                            thDataWork.Main.extractedpatchpath += Path.GetFileName(thDataWork.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch";
                            dir = new DirectoryInfo(Path.GetDirectoryName(thDataWork.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName;
                        }
                        else //иначе это версия 2
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "2";
                        }
                        //MessageBox.Show("patchdir2=" + patchdir);

                        var vRPGMTransPatchFiles = new List<string>();

                        foreach (FileInfo file in (new DirectoryInfo(thDataWork.Main.extractedpatchpath)).EnumerateFiles("*.txt"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            vRPGMTransPatchFiles.Add(file.FullName);
                        }

                        //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

                        //THFilesDataGridView.Nodes.Add("main");
                        //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
                        //RPGMTransPatch.OpenTransFiles(files, patchver);
                        //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
                        if (new RPGMTransOLD(thDataWork).OpenRPGMTransPatchFiles(vRPGMTransPatchFiles))
                        {

                            //Запись в dataGridVivwer
                            for (int i = 0; i < thDataWork.THFilesElementsDataset.Tables.Count; i++)
                            {
                                //MessageBox.Show("ListFiles=" + ListFiles[i]);
                                //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                                //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                                thDataWork.Main.THFilesList.Invoke((Action)(() => thDataWork.Main.THFilesList.Items.Add(thDataWork.THFilesElementsDataset.Tables[i].TableName)));
                                //THFilesDataGridView.Rows.Add();
                                //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                                //dGFiles.Rows.Add();
                                //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                            }

                            Properties.Settings.Default.THSelectedGameDir = Properties.Settings.Default.THSelectedDir;
                            Properties.Settings.Default.THSelectedDir = thDataWork.Main.extractedpatchpath.Replace(Path.DirectorySeparatorChar + "patch", string.Empty);
                            //MessageBox.Show(THSelectedSourceType + " loaded!");
                            //ProgressInfo(false, string.Empty);
                            return "RPG Maker game with RPGMTransPatch";
                        }
                    }
                }

            }

            return string.Empty;
        }

        /// <summary>
        /// Try to open project and return project name if open is success
        /// </summary>
        /// <returns></returns>
        private string TryOpenProject()
        {
            if (thDataWork.CurrentProject.Open())
            {
                return thDataWork.CurrentProject.ProjectTitle();
            }
            return string.Empty;
        }

        /// <summary>
        /// Try to detect project and if success<br/> then set it to current and return true
        /// </summary>
        /// <param name="Project"></param>
        /// <returns></returns>
        private bool TryDetectProject(ProjectBase Project)
        {
            if (Project.OpenDetect())
            {
                thDataWork.CurrentProject = Project;
                return true;
            }
            return false;
        }

        internal void AfterOpenActions()
        {
            if (thDataWork.Main.THFilesList.Items.Count == 0 && thDataWork.THFilesElementsDataset.Tables.Count > 0)
            {
                foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
                {
                    thDataWork.Main.THFilesList.Items.Add(table.TableName);
                }
            }

            Properties.Settings.Default.THSelectedGameDir = GetCorrectedGameDIr(Properties.Settings.Default.THSelectedGameDir);

            if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker game with RPGMTransPatch") || RPGMFunctions.THSelectedSourceType.Contains("KiriKiri game"))
            {
                thDataWork.Main.Settings.THConfigINI.WriteINI("Paths", "LastPath", Properties.Settings.Default.THSelectedGameDir);
            }
            else
            {
                try
                {
                    thDataWork.Main.Settings.THConfigINI.WriteINI("Paths", "LastPath", Properties.Settings.Default.THSelectedDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured:" + Environment.NewLine + ex);
                }
            }
            _ = /*THMsg*/MessageBox.Show(RPGMFunctions.THSelectedSourceType + T._(" loaded") + "!");

            thDataWork.Main.editToolStripMenuItem.Enabled = true;
            thDataWork.Main.viewToolStripMenuItem.Enabled = true;
            thDataWork.Main.loadTranslationToolStripMenuItem.Enabled = true;
            thDataWork.Main.loadTrasnlationAsToolStripMenuItem.Enabled = true;
            thDataWork.Main.runTestGameToolStripMenuItem.Enabled = thDataWork.CurrentProject != null && thDataWork.CurrentProject.IsTestRunEnabled;

            if (thDataWork.Main.FVariant.Length == 0)
            {
                thDataWork.Main.FVariant = " * " + RPGMFunctions.THSelectedSourceType;
            }
            try
            {
                thDataWork.Main.Text += thDataWork.Main.FVariant;
            }
            catch
            {
            }

            Properties.Settings.Default.ProjectIsOpened = true;
        }
    }
}
