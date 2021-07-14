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
        
        public FunctionsOpen()
        {
            
        }

        internal async void OpenProject()
        {
            if (ProjectData.Main.IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                ProjectData.Main.IsOpeningInProcess = true;
                var IsProjectFileSelected = false;
                //об сообщении Освобождаемый объект никогда не освобождается и почему using здесь
                //https://stackoverflow.com/questions/2926869/do-you-need-to-dispose-of-objects-and-set-them-to-null
                using (var THFOpen = new OpenFileDialog())
                {
                    THFOpen.InitialDirectory = ProjectData.Main.Settings.THConfigINI.ReadINI("Paths", "LastPath");
                    THFOpen.Filter = "All compatible|*.exe;RPGMKTRANSPATCH;*.json;*.scn;*.ks|RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker execute(*.exe)|*.exe|KiriKiri engine files|*.scn;*.ks|Txt file|*.txt|All|*.*";

                    if (THFOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (THFOpen.FileName != null)
                        {
                            new CleanupData().THCleanupThings();

                            ProjectData.SPath = THFOpen.FileName;
                            IsProjectFileSelected = true;
                        }
                    }
                }

                if (!IsProjectFileSelected)
                {
                    ProjectData.Main.IsOpeningInProcess = false;
                    return;
                }

                {
                    //THActionProgressBar.Visible = true;
                    //ProjectData.Main.ProgressInfo(true, T._("opening.."));

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
                    await Task.Run(() => RPGMFunctions.THSelectedSourceType = GetSourceType(ProjectData.SPath)).ConfigureAwait(true);

                    //THSelectedSourceType = GetSourceType(THFOpen.FileName);

                    //THActionProgressBar.Visible = false;
                    ProjectData.Main.ProgressInfo(false, string.Empty);

                    if (RPGMFunctions.THSelectedSourceType.Length == 0)
                    {
                        ProjectData.Main.frmMainPanel.Visible = false;

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

                ProjectData.Main.IsOpeningInProcess = false;
            }
        }

        //private bool TryToDetectSourceAndOpen()
        //{
        //    foreach (var Project in ProjectData.ProjectsList)
        //    {
        //        if (Project.OpenDetect())
        //        {
        //            if (Project.Open())
        //            {
        //                if (ProjectData.THFilesElementsDataset.Tables.Count > 0)
        //                {
        //                    ProjectData.Project = Project;
        //                    RPGMFunctions.THSelectedSourceType = Project.ProjectTitle();
        //                    foreach (DataTable file in ProjectData.THFilesElementsDataset.Tables)
        //                    {
        //                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFilesList.Items.Add(file.TableName)));
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
                tHSelectedGameDir = ProjectData.SelectedDir;
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
            ProjectData.SPath = sPath;
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            ProjectData.SelectedDir = dir.FullName;
            ProjectData.SelectedGameDir = dir.FullName;

            ProjectData.Main.frmMainPanel.Invoke((Action)(() => ProjectData.Main.frmMainPanel.Visible = true));

            //ShowProjectsList();

            //Try detect and open new type projects
            foreach (ProjectBase Project in ProjectData.ProjectsList)
            {
                ProjectData.CurrentProject = Project;
                if (TryDetectProject(Project))
                {
                    return TryOpenProject();
                }
                ProjectData.CurrentProject = null;
            }

            //Old projects
            return ""; //TryDetectOpenOldProjects(sPath);
        }

        //private void ShowProjectsList()
        //{
        //    StringBuilder titles = new StringBuilder();
        //    foreach (ProjectBase Project in ProjectData.ProjectsList)
        //    {
        //        titles.AppendLine(Project.ProjectTitle());
        //    }
        //    MessageBox.Show(titles.ToString());
        //}

        private string TryDetectOpenOldProjects(string sPath)
        {
            var dir = ProjectData.SelectedDir;

            if (new KiriKiriOLD().OpenDetect())
            {
                return new KiriKiriOLD().KiriKiriScriptScenario();
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS") || sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".SCN"))
            {
                return new KiriKiriOLD().KiriKiriScenarioOpen(sPath);
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT"))
            {
                return new WRPGOLDOpen().AnyTxt(sPath);
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).Contains("\\RPGMKTRANSPATCH"))
            {
                return new RPGMTransOLD().RPGMTransPatchPrepare(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToUpper(CultureInfo.InvariantCulture).Contains(".JSON"))
            {
                if (new RPGMMVOLD().OpenRPGMakerMVjson(sPath))
                {
                    //for (int i = 0; i < ProjectData.THFilesElementsDataset.Tables.Count; i++)
                    //{
                    //    ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.Items.Add(ProjectData.THFilesElementsDataset.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    //}
                    return "RPG Maker MV json";
                }
            }
            else if (Path.GetExtension(sPath) == ".exe" /*sPath.ToLower().Contains("\\game.exe") || dir.GetFiles("*.exe").Length > 0*/)
            {
                if (Directory.Exists(Path.Combine(Path.GetDirectoryName(sPath), "data", "bin")))
                {
                    return new RJ263914OLD().ProceedRubyRPGGame(Path.GetDirectoryName(sPath));//RJ263914
                }
                else if ((FunctionsProcess.GetExeDescription(sPath) != null && FunctionsProcess.GetExeDescription(sPath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")) && FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.xp3"))
                {
                    if (new KiriKiriOLD().KiriKiriGame())
                    {
                        return "KiriKiri game";
                    }
                }
                else if (File.Exists(Path.Combine(ProjectData.SelectedDir, "www", "data", "system.json")))
                {
                    try
                    {
                        //ProjectData.SelectedDir += "\\www\\data";
                        //var MVJsonFIles = new List<string>();
                        mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(ProjectData.SelectedDir, "www", "data/")));
                        foreach (FileInfo file in mvdatadir.EnumerateFiles("*.json"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            //MVJsonFIles.Add(file.FullName);

                            if (new RPGMMVOLD().OpenRPGMakerMVjson(file.FullName))
                            {
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }

                        for (int i = 0; i < ProjectData.THFilesElementsDataset.Tables.Count; i++)
                        {
                            //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);
                            ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.Items.Add(ProjectData.THFilesElementsDataset.Tables[i].TableName)));
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
                    ProjectData.Main.extractedpatchpath = string.Empty;
                    bool result = new RPGMGameOLD().TryToExtractToRPGMakerTransPatch(sPath);
                    //MessageBox.Show("result=" + result);
                    //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                    if (result)
                    {
                        //Cleaning of the type
                        //THRPGMTransPatchFiles.Clear();
                        //THFilesElementsDataset.Clear();

                        //var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
                        //string patchver;
                        bool isv3 = Directory.Exists(ProjectData.Main.extractedpatchpath + Path.DirectorySeparatorChar + "patch");
                        //MessageBox.Show("isv3=" + isv3+ ", patchdir="+ extractedpatchpath+ ", extractedpatchpath="+ extractedpatchpath);
                        if (isv3) //если есть подпапка patch, тогда это версия патча 3
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "3";
                            ProjectData.Main.extractedpatchpath += Path.DirectorySeparatorChar + "patch";
                            //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                            dir = new DirectoryInfo(Path.GetDirectoryName(ProjectData.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName; //Два слеша здесь в конце исправляют проблему возврата информации о неверной папке
                                                                                                                                                       //MessageBox.Show("patchdir1=" + patchdir);
                        }
                        else if (Directory.Exists(ProjectData.Main.extractedpatchpath + Path.GetFileName(ProjectData.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch"))
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "3";
                            ProjectData.Main.extractedpatchpath += Path.GetFileName(ProjectData.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch";
                            dir = new DirectoryInfo(Path.GetDirectoryName(ProjectData.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName;
                        }
                        else //иначе это версия 2
                        {
                            RPGMFunctions.RPGMTransPatchVersion = "2";
                        }
                        //MessageBox.Show("patchdir2=" + patchdir);

                        var vRPGMTransPatchFiles = new List<string>();

                        foreach (FileInfo file in (new DirectoryInfo(ProjectData.Main.extractedpatchpath)).EnumerateFiles("*.txt"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            vRPGMTransPatchFiles.Add(file.FullName);
                        }

                        //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

                        //THFilesDataGridView.Nodes.Add("main");
                        //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
                        //RPGMTransPatch.OpenTransFiles(files, patchver);
                        //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
                        if (new RPGMTransOLD().OpenRPGMTransPatchFiles(vRPGMTransPatchFiles))
                        {

                            //Запись в dataGridVivwer
                            for (int i = 0; i < ProjectData.THFilesElementsDataset.Tables.Count; i++)
                            {
                                //MessageBox.Show("ListFiles=" + ListFiles[i]);
                                //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                                //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                                ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.Items.Add(ProjectData.THFilesElementsDataset.Tables[i].TableName)));
                                //THFilesDataGridView.Rows.Add();
                                //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                                //dGFiles.Rows.Add();
                                //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                            }

                            ProjectData.SelectedGameDir = ProjectData.SelectedDir;
                            ProjectData.SelectedDir = ProjectData.Main.extractedpatchpath.Replace(Path.DirectorySeparatorChar + "patch", string.Empty);
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
            ProjectData.CurrentProject.Init();
            ProjectData.CurrentProject.BakRestore();
            ProjectData.OpenFileMode = true;
            if (ProjectData.CurrentProject.Open())
            {
                ProjectData.CurrentProject.CreateMenus();//createmenus

                return ProjectData.CurrentProject.Name();
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
                ProjectData.CurrentProject = Project;
                return true;
            }
            return false;
        }

        internal void AfterOpenActions()
        {
            if (!ProjectData.Main.THWorkSpaceSplitContainer.Visible)
            {
                ProjectData.Main.THWorkSpaceSplitContainer.Visible = true;
            }

            if (ProjectData.Main.THFilesList.Items.Count == 0 && ProjectData.THFilesElementsDataset.Tables.Count > 0)
            {
                //https://stackoverflow.com/questions/11099619/how-to-bind-dataset-to-datagridview-in-windows-application
                //foreach (DataColumn col in ProjectData.THFilesElementsDataset.Tables[0].Columns)
                //{
                //    ProjectData.THFilesElementsDataset.Tables["[ALL]"].Columns.Add(col.ColumnName);
                //}
                //using (DataSet newdataset = new DataSet())
                //{
                //    newdataset.Tables.Add("[ALL]");
                //    newdataset.Merge(ProjectData.THFilesElementsDataset);
                //    ProjectData.THFilesElementsDataset.Clear();
                //    ProjectData.THFilesElementsDataset.Merge(newdataset);
                //}
                foreach (DataTable table in ProjectData.THFilesElementsDataset.Tables)
                {
                    ProjectData.Main.THFilesList.Items.Add(table.TableName);
                }
            }

            ProjectData.SelectedGameDir = GetCorrectedGameDIr(ProjectData.SelectedGameDir);

            if (RPGMFunctions.THSelectedSourceType.Contains("RPG Maker game with RPGMTransPatch") || RPGMFunctions.THSelectedSourceType.Contains("KiriKiri game"))
            {
                ProjectData.Main.Settings.THConfigINI.WriteINI("Paths", "LastPath", ProjectData.SelectedGameDir);
            }
            else
            {
                try
                {
                    ProjectData.Main.Settings.THConfigINI.WriteINI("Paths", "LastPath", ProjectData.SelectedDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured:" + Environment.NewLine + ex);
                }
            }
            //_ = /*THMsg*/MessageBox.Show(RPGMFunctions.THSelectedSourceType + T._(" loaded") + "!");

            ProjectData.Main.editToolStripMenuItem.Enabled = true;
            ProjectData.Main.viewToolStripMenuItem.Enabled = true;
            ProjectData.Main.loadTranslationToolStripMenuItem.Enabled = true;
            ProjectData.Main.loadTrasnlationAsToolStripMenuItem.Enabled = true;
            ProjectData.Main.loadTrasnlationAsForcedToolStripMenuItem.Enabled = true;
            ProjectData.Main.exportToolStripMenuItem1.Enabled = true;
            ProjectData.Main.runTestGameToolStripMenuItem.Enabled = ProjectData.CurrentProject != null && ProjectData.CurrentProject.IsTestRunEnabled;
            ProjectData.Main.openProjectsDirToolStripMenuItem.Enabled = true;
            ProjectData.Main.openTranslationRulesFileToolStripMenuItem.Enabled = true;
            ProjectData.Main.openCellFixesFileToolStripMenuItem.Enabled = true;
            ProjectData.Main.reloadRulesToolStripMenuItem.Enabled = true;

            if (ProjectData.Main.FVariant.Length == 0)
            {
                ProjectData.Main.FVariant = " * " + RPGMFunctions.THSelectedSourceType;
            }
            try
            {
                ProjectData.Main.Text += ProjectData.Main.FVariant;
            }
            catch
            {
            }

            Properties.Settings.Default.ProjectNewLineSymbol = (ProjectData.CurrentProject != null)
                ? ProjectData.CurrentProject.NewlineSymbol
                : Environment.NewLine;

            AfterOpenCleaning();

            Properties.Settings.Default.ProjectIsOpened = true;
            FunctionsSounds.OpenProjectComplete();

            FunctionsLoadTranslationDB.LoadTranslationIfNeed();
        }

        private void AfterOpenCleaning()
        {
            ProjectData.TablesLinesDict?.Clear();
            ProjectData.ENQuotesToJPLearnDataFoundNext?.Clear();
            ProjectData.ENQuotesToJPLearnDataFoundPrev?.Clear();
        }
    }
}
