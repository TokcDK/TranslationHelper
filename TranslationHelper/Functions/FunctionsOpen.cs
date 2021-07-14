using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
        readonly ProjectData projectData;
        public FunctionsOpen(ProjectData projectData)
        {
            this.projectData = projectData;
        }

        internal async void OpenProject()
        {
            if (projectData.Main.IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                projectData.Main.IsOpeningInProcess = true;
                var IsProjectFileSelected = false;
                //об сообщении Освобождаемый объект никогда не освобождается и почему using здесь
                //https://stackoverflow.com/questions/2926869/do-you-need-to-dispose-of-objects-and-set-them-to-null
                using (var THFOpen = new OpenFileDialog())
                {
                    THFOpen.InitialDirectory = projectData.Main.Settings.THConfigINI.ReadINI("Paths", "LastPath");
                    THFOpen.Filter = "All compatible|*.exe;RPGMKTRANSPATCH;*.json;*.scn;*.ks|RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker execute(*.exe)|*.exe|KiriKiri engine files|*.scn;*.ks|Txt file|*.txt|All|*.*";

                    if (THFOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (THFOpen.FileName != null)
                        {
                            new CleanupData(projectData).THCleanupThings();

                            projectData.SPath = THFOpen.FileName;
                            IsProjectFileSelected = true;
                        }
                    }
                }

                if (!IsProjectFileSelected)
                {
                    projectData.Main.IsOpeningInProcess = false;
                    return;
                }

                {
                    //THActionProgressBar.Visible = true;
                    //projectData.Main.ProgressInfo(true, T._("opening.."));

                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    //Thread open = new Thread(new ParameterizedThreadStart((obj) => GetSourceType(THFOpen.FileName)));
                    //open.Start();

                    //bool newOpen = false;

                    //if (newOpen)
                    //{
                    //    if (await Task.Run(() => TryToDetectSourceAndOpen()).ConfigureAwait(true))
                    //    {
                    //        AfterOpenActions();

                    //        return;
                    //    }
                    //    else
                    //    {
                    //        /*THMsg*/
                    //        MessageBox.Show(T._("Problem with source opening. Try to report to devs about it."));

                    //        return;
                    //    }
                    //}

                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => RPGMFunctions.THSelectedSourceType = GetSourceType(projectData.SPath)).ConfigureAwait(true);

                    //THSelectedSourceType = GetSourceType(THFOpen.FileName);

                    //THActionProgressBar.Visible = false;
                    projectData.Main.ProgressInfo(false, string.Empty);

                    if (RPGMFunctions.THSelectedSourceType.Length == 0)
                    {
                        projectData.Main.frmMainPanel.Visible = false;

                        /*THMsg*/
                        FunctionsSounds.OpenProjectFailed();
                        MessageBox.Show(T._("Failed to open project"));
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

                projectData.Main.IsOpeningInProcess = false;
            }
        }

        //private bool TryToDetectSourceAndOpen()
        //{
        //    foreach (var Project in projectData.ProjectsList)
        //    {
        //        if (Project.OpenDetect())
        //        {
        //            if (Project.Open())
        //            {
        //                if (projectData.THFilesElementsDataset.Tables.Count > 0)
        //                {
        //                    projectData.Project = Project;
        //                    RPGMFunctions.THSelectedSourceType = Project.ProjectTitle();
        //                    foreach (DataTable file in projectData.THFilesElementsDataset.Tables)
        //                    {
        //                        projectData.Main.Invoke((Action)(() => projectData.Main.THFilesList.Items.Add(file.TableName)));
        //                    }
        //                    return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}

        private static string GetCorrectedGameDIr(string tHSelectedGameDir)
        {
            if (tHSelectedGameDir.Length == 0)
            {
                tHSelectedGameDir = Properties.Settings.Default.THSelectedDir;
            }

            //для rpgmaker mv. если была папка data, которая в папке www
            string pFolderName = Path.GetFileName(tHSelectedGameDir);
            if (string.Compare(pFolderName, "data", true, CultureInfo.InvariantCulture) == 0)
            {
                return Path.GetDirectoryName(Path.GetDirectoryName(tHSelectedGameDir));
            }
            return tHSelectedGameDir;
        }

        internal DirectoryInfo mvdatadir;
        private string GetSourceType(string sPath)
        {
            projectData.SPath = sPath;
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            Properties.Settings.Default.THSelectedDir = dir.FullName;
            Properties.Settings.Default.THSelectedGameDir = dir.FullName;

            projectData.Main.frmMainPanel.Invoke((Action)(() => projectData.Main.frmMainPanel.Visible = true));

            //ShowProjectsList();

            //Try detect and open new type projects
            foreach (ProjectBase Project in projectData.ProjectsList)
            {
                projectData.CurrentProject = Project;
                if (TryDetectProject(Project))
                {
                    return TryOpenProject();
                }
                projectData.CurrentProject = null;
            }

            //Old projects
            return ""; //TryDetectOpenOldProjects(sPath);
        }

        //private void ShowProjectsList()
        //{
        //    StringBuilder titles = new StringBuilder();
        //    foreach (ProjectBase Project in projectData.ProjectsList)
        //    {
        //        titles.AppendLine(Project.ProjectTitle());
        //    }
        //    MessageBox.Show(titles.ToString());
        //}

        private string TryDetectOpenOldProjects(string sPath)
        {
            var dir = Properties.Settings.Default.THSelectedDir;

            if (new KiriKiriOLD(projectData).OpenDetect())
            {
                return new KiriKiriOLD(projectData).KiriKiriScriptScenario();
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS") || sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".SCN"))
            {
                return new KiriKiriOLD(projectData).KiriKiriScenarioOpen(sPath);
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT"))
            {
                return new WRPGOLDOpen(projectData).AnyTxt(sPath);
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).Contains("\\RPGMKTRANSPATCH"))
            {
                return new RPGMTransOLD(projectData).RPGMTransPatchPrepare(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).Contains(".JSON"))
            {
                if (new RPGMMVOLD(projectData).OpenRPGMakerMVjson(sPath))
                {
                    //for (int i = 0; i < projectData.THFilesElementsDataset.Tables.Count; i++)
                    //{
                    //    projectData.Main.THFilesList.Invoke((Action)(() => projectData.Main.THFilesList.Items.Add(projectData.THFilesElementsDataset.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    //}
                    return "RPG Maker MV json";
                }
            }
            else if (Path.GetExtension(sPath) == ".exe" /*sPath.ToLower().Contains("\\game.exe") || dir.GetFiles("*.exe").Length > 0*/)
            {
                if (Directory.Exists(Path.Combine(Path.GetDirectoryName(sPath), "data", "bin")))
                {
                    return new RJ263914OLD(projectData).ProceedRubyRPGGame(Path.GetDirectoryName(sPath));//RJ263914
                }
                else if ((FunctionsProcess.GetExeDescription(sPath) != null && FunctionsProcess.GetExeDescription(sPath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")) && FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.xp3"))
                {
                    if (new KiriKiriOLD(projectData).KiriKiriGame())
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

                            if (new RPGMMVOLD(projectData).OpenRPGMakerMVjson(file.FullName))
                            {
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }

                        for (int i = 0; i < projectData.THFilesElementsDataset.Tables.Count; i++)
                        {
                            //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);
                            projectData.Main.THFilesList.Invoke((Action)(() => projectData.Main.THFilesList.Items.Add(projectData.THFilesElementsDataset.Tables[i].TableName)));
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
                    projectData.Main.extractedpatchpath = string.Empty;
                    bool result = new RPGMGameOLD(projectData).TryToExtractToRPGMakerTransPatch(sPath);
                    //MessageBox.Show("result=" + result);
                    //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                    if (result)
                    {
                        //Cleaning of the type
                        //THRPGMTransPatchFiles.Clear();
                        //THFilesElementsDataset.Clear();

                        //var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
                        //string patchver;
                        bool isv3 = Directory.Exists(projectData.Main.extractedpatchpath + Path.DirectorySeparatorChar + "patch");
                        //MessageBox.Show("isv3=" + isv3+ ", patchdir="+ extractedpatchpath+ ", extractedpatchpath="+ extractedpatchpath);
                        if (isv3) //если есть подпапка patch, тогда это версия патча 3
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "3";
                            projectData.Main.extractedpatchpath += Path.DirectorySeparatorChar + "patch";
                            //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                            dir = new DirectoryInfo(Path.GetDirectoryName(projectData.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName; //Два слеша здесь в конце исправляют проблему возврата информации о неверной папке
                                                                                                                                                       //MessageBox.Show("patchdir1=" + patchdir);
                        }
                        else if (Directory.Exists(projectData.Main.extractedpatchpath + Path.GetFileName(projectData.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch"))
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "3";
                            projectData.Main.extractedpatchpath += Path.GetFileName(projectData.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch";
                            dir = new DirectoryInfo(Path.GetDirectoryName(projectData.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName;
                        }
                        else //иначе это версия 2
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "2";
                        }
                        //MessageBox.Show("patchdir2=" + patchdir);

                        var vRPGMTransPatchFiles = new List<string>();

                        foreach (FileInfo file in (new DirectoryInfo(projectData.Main.extractedpatchpath)).EnumerateFiles("*.txt"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            vRPGMTransPatchFiles.Add(file.FullName);
                        }

                        //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

                        //THFilesDataGridView.Nodes.Add("main");
                        //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
                        //RPGMTransPatch.OpenTransFiles(files, patchver);
                        //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
                        if (new RPGMTransOLD(projectData).OpenRPGMTransPatchFiles(vRPGMTransPatchFiles))
                        {

                            //Запись в dataGridVivwer
                            for (int i = 0; i < projectData.THFilesElementsDataset.Tables.Count; i++)
                            {
                                //MessageBox.Show("ListFiles=" + ListFiles[i]);
                                //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                                //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                                projectData.Main.THFilesList.Invoke((Action)(() => projectData.Main.THFilesList.Items.Add(projectData.THFilesElementsDataset.Tables[i].TableName)));
                                //THFilesDataGridView.Rows.Add();
                                //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                                //dGFiles.Rows.Add();
                                //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                            }

                            Properties.Settings.Default.THSelectedGameDir = Properties.Settings.Default.THSelectedDir;
                            Properties.Settings.Default.THSelectedDir = projectData.Main.extractedpatchpath.Replace(Path.DirectorySeparatorChar + "patch", string.Empty);
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
            projectData.CurrentProject.Init();
            projectData.CurrentProject.BakRestore();
            projectData.OpenFileMode = true;
            if (projectData.CurrentProject.Open())
            {
                projectData.CurrentProject.CreateMenus();//createmenus

                return projectData.CurrentProject.Name();
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
            if (Project.Check())
            {
                projectData.CurrentProject = Project;
                return true;
            }
            return false;
        }

        internal void AfterOpenActions()
        {
            if (!projectData.Main.THWorkSpaceSplitContainer.Visible)
            {
                projectData.Main.THWorkSpaceSplitContainer.Visible = true;
            }

            if (projectData.Main.THFilesList.Items.Count == 0 && projectData.THFilesElementsDataset.Tables.Count > 0)
            {
                //https://stackoverflow.com/questions/11099619/how-to-bind-dataset-to-datagridview-in-windows-application
                //foreach (DataColumn col in projectData.THFilesElementsDataset.Tables[0].Columns)
                //{
                //    projectData.THFilesElementsDataset.Tables["[ALL]"].Columns.Add(col.ColumnName);
                //}
                //using (DataSet newdataset = new DataSet())
                //{
                //    newdataset.Tables.Add("[ALL]");
                //    newdataset.Merge(projectData.THFilesElementsDataset);
                //    projectData.THFilesElementsDataset.Clear();
                //    projectData.THFilesElementsDataset.Merge(newdataset);
                //}
                foreach (DataTable table in projectData.THFilesElementsDataset.Tables)
                {
                    projectData.Main.THFilesList.Items.Add(table.TableName);
                }
            }

            Properties.Settings.Default.THSelectedGameDir = GetCorrectedGameDIr(Properties.Settings.Default.THSelectedGameDir);

            if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker game with RPGMTransPatch") || RPGMFunctions.THSelectedSourceType.Contains("KiriKiri game"))
            {
                projectData.Main.Settings.THConfigINI.WriteINI("Paths", "LastPath", Properties.Settings.Default.THSelectedGameDir);
            }
            else
            {
                try
                {
                    projectData.Main.Settings.THConfigINI.WriteINI("Paths", "LastPath", Properties.Settings.Default.THSelectedDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured:" + Environment.NewLine + ex);
                }
            }
            //_ = /*THMsg*/MessageBox.Show(RPGMFunctions.THSelectedSourceType + T._(" loaded") + "!");

            projectData.Main.editToolStripMenuItem.Enabled = true;
            projectData.Main.viewToolStripMenuItem.Enabled = true;
            projectData.Main.loadTranslationToolStripMenuItem.Enabled = true;
            projectData.Main.loadTrasnlationAsToolStripMenuItem.Enabled = true;
            projectData.Main.loadTrasnlationAsForcedToolStripMenuItem.Enabled = true;
            projectData.Main.exportToolStripMenuItem1.Enabled = true;
            projectData.Main.runTestGameToolStripMenuItem.Enabled = projectData.CurrentProject != null && projectData.CurrentProject.IsTestRunEnabled;
            projectData.Main.openProjectsDirToolStripMenuItem.Enabled = true;
            projectData.Main.openTranslationRulesFileToolStripMenuItem.Enabled = true;
            projectData.Main.openCellFixesFileToolStripMenuItem.Enabled = true;
            projectData.Main.reloadRulesToolStripMenuItem.Enabled = true;

            if (projectData.Main.FVariant.Length == 0)
            {
                projectData.Main.FVariant = " * " + RPGMFunctions.THSelectedSourceType;
            }
            try
            {
                projectData.Main.Text += projectData.Main.FVariant;
            }
            catch
            {
            }

            Properties.Settings.Default.ProjectNewLineSymbol = (projectData != null && projectData.CurrentProject != null)
                ? projectData.CurrentProject.NewlineSymbol
                : Environment.NewLine;

            AfterOpenCleaning();

            Properties.Settings.Default.ProjectIsOpened = true;
            FunctionsSounds.OpenProjectComplete();

            FunctionsLoadTranslationDB.LoadTranslationIfNeed(projectData);
        }

        private void AfterOpenCleaning()
        {
            projectData.TablesLinesDict?.Clear();
            projectData.ENQuotesToJPLearnDataFoundNext?.Clear();
            projectData.ENQuotesToJPLearnDataFoundPrev?.Clear();
        }
    }
}
