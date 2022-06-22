using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Menus;
using TranslationHelper.Projects;
using TranslationHelper.Projects.ZZZZFormats;

namespace TranslationHelper.Functions
{
    class FunctionsOpen
    {
        internal static async void OpenProject(string filePath = null)
        {
            if (filePath == null || !File.Exists(filePath))
            {
                using (var THFOpen = new OpenFileDialog())
                {
                    THFOpen.InitialDirectory = AppData.Main.Settings.THConfigINI.GetKey("Paths", "LastPath");

                    bool tempMode = AppData.OpenFileMode;
                    AppData.OpenFileMode = true;// temporary set open file mode to true if on save mode trying to open files to prevent errors in time of get extensions for filter

                    var possibleExtensions = string.Join(";*", GetListOfSubClasses.Inherited.GetListOfinheritedSubClasses<FormatBase>().Where(f => !string.IsNullOrWhiteSpace(f.Ext()) && f.Ext()[0] == '.').Select(f => f.Ext()).Distinct());
                    THFOpen.Filter = "All|*" + possibleExtensions + ";RPGMKTRANSPATCH|RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker execute(*.exe)|*.exe|KiriKiri engine files|*.scn;*.ks|Txt file|*.txt|All|*.*";

                    AppData.OpenFileMode = tempMode;
                    if (THFOpen.ShowDialog() != DialogResult.OK || THFOpen.FileName == null)
                    {
                        AppData.Main.IsOpeningInProcess = false;
                        return;
                    }

                    filePath = THFOpen.FileName;
                }
            }

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                AppData.Main.IsOpeningInProcess = false;
                return;
            }

            CleanupData.THCleanupThings();

            AppData.SelectedFilePath = filePath;

            bool isProjectFound = false;
            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
            await Task.Run(() => isProjectFound = GetSourceType(AppData.SelectedFilePath)).ConfigureAwait(true);

            AppData.Main.ProgressInfo(false, string.Empty);

            if (!isProjectFound)
            {
                AppData.Main.frmMainPanel.Visible = false;

                /*THMsg*/
                FunctionsSounds.OpenProjectFailed();
                MessageBox.Show(T._("Failed to open project"));
            }
            else
            {
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

            AppData.Main.IsOpeningInProcess = false;
        }

        private static string GetCorrectedGameDIr(string tHSelectedGameDir)
        {
            if (tHSelectedGameDir.Length == 0)
            {
                tHSelectedGameDir = AppData.CurrentProject.SelectedDir;
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
        private static bool GetSourceType(string sPath)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));

            AppData.Main.frmMainPanel.Invoke((Action)(() => AppData.Main.frmMainPanel.Visible = true));

            List<Type> foundTypes = new List<Type>();

            //Try detect and open new type projects
            foreach (Type Project in AppData.ProjectsList) // iterate projectbase types
            {
                AppData.CurrentProject = (ProjectBase)Activator.CreateInstance(Project);// create instance of project
                AppData.OpenFileMode = true;
                AppData.SelectedFilePath = sPath;
                AppData.CurrentProject.OpenedFilesDir = dir.FullName;
                AppData.CurrentProject.SelectedDir = dir.FullName;
                AppData.CurrentProject.SelectedGameDir = dir.FullName;

                if (TryDetectProject(AppData.CurrentProject)) foundTypes.Add(Project); // add all project which check returned true

                //ProjectData.CurrentProject = null;
            }

            if (foundTypes.Count == 0) return false; // nothing found

            if (foundTypes.Count == 1) return TryOpenSelectedProject(foundTypes[0], sPath, dir); // try open with found project when only one found

            int selectedIndex = -1;
            var foundForm = new FoundTypesbyExtensionForm(); // use form from formats
            foreach (var type in foundTypes) foundForm.listBox1.Items.Add(type.FullName);

            var result = foundForm.ShowDialog();
            if (result == DialogResult.OK) selectedIndex = foundForm.SelectedTypeIndex;

            foundForm.Dispose();

            if (selectedIndex > -1) return TryOpenSelectedProject(foundTypes[selectedIndex], sPath, dir); // try open with selected project

            return TryOpenSelectedProject(foundTypes[0], sPath, dir); // try open with first of project if not selected
        }

        private static bool TryOpenSelectedProject(Type type, string sPath, DirectoryInfo dir)
        {
            AppData.CurrentProject = (ProjectBase)Activator.CreateInstance(type);// create instance of project
            AppData.OpenFileMode = true;
            AppData.SelectedFilePath = sPath;
            AppData.CurrentProject.OpenedFilesDir = dir.FullName;
            AppData.CurrentProject.SelectedDir = dir.FullName;
            AppData.CurrentProject.SelectedGameDir = dir.FullName;

            if (!TryOpenProject()) return false;

            return true;
        }

        //private string TryDetectOpenOldProjects(string sPath)
        //{
        //    var dir = ProjectData.CurrentProject.SelectedDir;

        //    if (new KiriKiriOLD().OpenDetect())
        //    {
        //        return new KiriKiriOLD().KiriKiriScriptScenario();
        //    }
        //    else if (sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS") || sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".SCN"))
        //    {
        //        return new KiriKiriOLD().KiriKiriScenarioOpen(sPath);
        //    }
        //    else if (sPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT"))
        //    {
        //        return new WRPGOLDOpen().AnyTxt(sPath);
        //    }
        //    else if (sPath.ToUpper(CultureInfo.InvariantCulture).Contains("\\RPGMKTRANSPATCH"))
        //    {
        //        return new RPGMTransOLD().RPGMTransPatchPrepare(sPath);
        //        //return "RPGMakerTransPatch";
        //    }
        //    else if (sPath.ToUpper(CultureInfo.InvariantCulture).Contains(".JSON"))
        //    {
        //        if (new RPGMMVOLD().OpenRPGMakerMVjson(sPath))
        //        {
        //            //for (int i = 0; i < ProjectData.THFilesElementsDataset.Tables.Count; i++)
        //            //{
        //            //    ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.AddItem(ProjectData.THFilesElementsDataset.Tables[i].TableName)));//add all dataset tables names to the ListBox

        //            //}
        //            return "RPG Maker MV json";
        //        }
        //    }
        //    else if (Path.GetExtension(sPath) == ".exe" /*sPath.ToLower().Contains("\\game.exe") || dir.GetFiles("*.exe").Length > 0*/)
        //    {
        //        if (Directory.Exists(Path.Combine(Path.GetDirectoryName(sPath), "data", "bin")))
        //        {
        //            return new RJ263914OLD().ProceedRubyRPGGame(Path.GetDirectoryName(sPath));//RJ263914
        //        }
        //        else if ((FunctionsProcess.GetExeDescription(sPath) != null && FunctionsProcess.GetExeDescription(sPath).ToUpper(CultureInfo.InvariantCulture).Contains("KIRIKIRI")) && FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.xp3"))
        //        {
        //            if (new KiriKiriOLD().KiriKiriGame())
        //            {
        //                return "KiriKiri game";
        //            }
        //        }
        //        else if (File.Exists(Path.Combine(ProjectData.CurrentProject.SelectedDir, "www", "data", "system.json")))
        //        {
        //            try
        //            {
        //                //ProjectData.CurrentProject.SelectedDir += "\\www\\data";
        //                //var MVJsonFIles = new List<string>();
        //                mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(ProjectData.CurrentProject.SelectedDir, "www", "data/")));
        //                foreach (FileInfo file in mvdatadir.EnumerateFiles("*.json"))
        //                {
        //                    //MessageBox.Show("file.FullName=" + file.FullName);
        //                    //MVJsonFIles.Add(file.FullName);

        //                    if (new RPGMMVOLD().OpenRPGMakerMVjson(file.FullName))
        //                    {
        //                    }
        //                    else
        //                    {
        //                        return string.Empty;
        //                    }
        //                }

        //                for (int i = 0; i < ProjectData.THFilesElementsDataset.Tables.Count; i++)
        //                {
        //                    //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);
        //                    ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.AddItem(ProjectData.THFilesElementsDataset.Tables[i].TableName)));
        //                }

        //                return "RPG Maker MV";
        //            }
        //            catch
        //            {
        //                return string.Empty;
        //            }
        //        }
        //        else if (File.Exists(Path.Combine(dir, "Data", "System.rvdata2"))
        //            || File.Exists(Path.Combine(dir, "Data", "System.rxdata"))
        //            || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rgss3a")
        //            || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rgss2a")
        //            || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rvdata")
        //            || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rgssad")
        //            || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.rxdata")
        //            || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.lmt")
        //            || FunctionsFileFolder.IsInDirExistsAnyFile(dir, "*.lmu"))
        //        {
        //            ProjectData.Main.extractedpatchpath = string.Empty;
        //            bool result = new RPGMGameOLD().TryToExtractToRPGMakerTransPatch(sPath);
        //            //MessageBox.Show("result=" + result);
        //            //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
        //            if (result)
        //            {
        //                //Cleaning of the type
        //                //THRPGMTransPatchFiles.Clear();
        //                //THFilesElementsDataset.Clear();

        //                //var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
        //                //string patchver;
        //                bool isv3 = Directory.Exists(ProjectData.Main.extractedpatchpath + Path.DirectorySeparatorChar + "patch");
        //                //MessageBox.Show("isv3=" + isv3+ ", patchdir="+ extractedpatchpath+ ", extractedpatchpath="+ extractedpatchpath);
        //                if (isv3) //если есть подпапка patch, тогда это версия патча 3
        //                {
        //                    RPGMFunctions.RPGMTransPatchVersion = "3";
        //                    ProjectData.Main.extractedpatchpath += Path.DirectorySeparatorChar + "patch";
        //                    //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
        //                    dir = new DirectoryInfo(Path.GetDirectoryName(ProjectData.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName; //Два слеша здесь в конце исправляют проблему возврата информации о неверной папке
        //                                                                                                                                                //MessageBox.Show("patchdir1=" + patchdir);
        //                }
        //                else if (Directory.Exists(ProjectData.Main.extractedpatchpath + Path.GetFileName(ProjectData.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch"))
        //                {
        //                    RPGMFunctions.RPGMTransPatchVersion = "3";
        //                    ProjectData.Main.extractedpatchpath += Path.GetFileName(ProjectData.Main.extractedpatchpath) + Path.DirectorySeparatorChar + "patch";
        //                    dir = new DirectoryInfo(Path.GetDirectoryName(ProjectData.Main.extractedpatchpath + Path.DirectorySeparatorChar)).FullName;
        //                }
        //                else //иначе это версия 2
        //                {
        //                    RPGMFunctions.RPGMTransPatchVersion = "2";
        //                }
        //                //MessageBox.Show("patchdir2=" + patchdir);

        //                var vRPGMTransPatchFiles = new List<string>();

        //                foreach (FileInfo file in (new DirectoryInfo(ProjectData.Main.extractedpatchpath)).EnumerateFiles("*.txt"))
        //                {
        //                    //MessageBox.Show("file.FullName=" + file.FullName);
        //                    vRPGMTransPatchFiles.Add(file.FullName);
        //                }

        //                //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

        //                //THFilesDataGridView.Nodes.Add("main");
        //                //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
        //                //RPGMTransPatch.OpenTransFiles(files, patchver);
        //                //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
        //                if (new RPGMTransOLD().OpenRPGMTransPatchFiles(vRPGMTransPatchFiles))
        //                {

        //                    //Запись в dataGridVivwer
        //                    for (int i = 0; i < ProjectData.THFilesElementsDataset.Tables.Count; i++)
        //                    {
        //                        //MessageBox.Show("ListFiles=" + ListFiles[i]);
        //                        //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
        //                        //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
        //                        ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.AddItem(ProjectData.THFilesElementsDataset.Tables[i].TableName)));
        //                        //THFilesDataGridView.Rows.Add();
        //                        //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
        //                        //dGFiles.Rows.Add();
        //                        //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
        //                    }

        //                    ProjectData.CurrentProject.SelectedGameDir = ProjectData.CurrentProject.SelectedDir;
        //                    ProjectData.CurrentProject.SelectedDir = ProjectData.Main.extractedpatchpath.Replace(Path.DirectorySeparatorChar + "patch", string.Empty);
        //                    //MessageBox.Show(THSelectedSourceType + " loaded!");
        //                    //ProgressInfo(false, string.Empty);
        //                    return "RPG Maker game with RPGMTransPatch";
        //                }
        //            }
        //        }

        //    }

        //    return string.Empty;
        //}

        /// <summary>
        /// Try to open project and return project name if open is success
        /// </summary>
        /// <returns></returns>
        private static bool TryOpenProject()
        {
            AppData.CurrentProject.Init();
            AppData.CurrentProject.BakRestore();
            AppData.OpenFileMode = true;
            if (AppData.CurrentProject.Open())
            {
                MenusCreator.CreateMenus();//createmenus

                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to detect project and if success<br/> then set it to current and return true
        /// </summary>
        /// <param name="Project"></param>
        /// <returns></returns>
        private static bool TryDetectProject(ProjectBase Project)
        {
            if (Project.Check())
            {
                AppData.CurrentProject = Project;
                return true;
            }
            return false;
        }

        internal static void AfterOpenActions()
        {
            AppData.SaveFileMode = true; // project opened. dont need to openfilemode

            if (!AppData.Main.THWorkSpaceSplitContainer.Visible)
            {
                AppData.Main.THWorkSpaceSplitContainer.Visible = true;
            }

            if (AppData.Main.THFilesList.GetItemsCount() == 0 && AppData.CurrentProject.FilesContent.Tables.Count > 0)
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

                //var sortedtables = ProjectData.CurrentProject.FilesContent.Tables.Cast<DataTable>().OrderBy(table => Path.GetExtension(table.TableName)).ToArray();
                var sortedtables = Sort(AppData.CurrentProject.FilesContent.Tables);
                AppData.CurrentProject.FilesContent.Tables.Clear();
                AppData.CurrentProject.FilesContent.Tables.AddRange(sortedtables);

                //var sortedtablesinfo = ProjectData.CurrentProject.FilesContentInfo.Tables.Cast<DataTable>().OrderBy(table => Path.GetExtension(table.TableName)).ToArray();
                var sortedtablesinfo = Sort(AppData.CurrentProject.FilesContentInfo.Tables);
                AppData.CurrentProject.FilesContentInfo.Tables.Clear();
                AppData.CurrentProject.FilesContentInfo.Tables.AddRange(sortedtablesinfo);


                foreach (DataTable table in AppData.CurrentProject.FilesContent.Tables)
                {
                    AppData.Main.THFilesList.AddItem(table.TableName);
                }
            }

            UpdateRecentFiles();

            AppData.CurrentProject.SelectedGameDir = GetCorrectedGameDIr(AppData.CurrentProject.SelectedGameDir);

            if (AppData.CurrentProject.Name.Contains("RPG Maker game with RPGMTransPatch") || AppData.CurrentProject.Name.Contains("KiriKiri game"))
            {
                AppData.Main.Settings.THConfigINI.SetKey("Paths", "LastPath", AppData.CurrentProject.SelectedGameDir);
            }
            else
            {
                try
                {
                    AppData.Main.Settings.THConfigINI.SetKey("Paths", "LastPath", AppData.CurrentProject.SelectedDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured:" + Environment.NewLine + ex);
                }
            }
            //_ = /*THMsg*/MessageBox.Show(ProjectData.CurrentProject.Name() + T._(" loaded") + "!");

            AppData.Main.EditToolStripMenuItem.Enabled = true;
            AppData.Main.ViewToolStripMenuItem.Enabled = true;
            AppData.Main.LoadTranslationToolStripMenuItem.Enabled = true;
            AppData.Main.LoadTrasnlationAsToolStripMenuItem.Enabled = true;
            AppData.Main.LoadTrasnlationAsForcedToolStripMenuItem.Enabled = true;
            AppData.Main.ExportToolStripMenuItem1.Enabled = true;
            AppData.Main.runTestGameToolStripMenuItem.Enabled = AppData.CurrentProject != null && AppData.CurrentProject.IsTestRunEnabled;
            AppData.Main.OpenProjectsDirToolStripMenuItem.Enabled = true;
            AppData.Main.OpenTranslationRulesFileToolStripMenuItem.Enabled = true;
            AppData.Main.OpenCellFixesFileToolStripMenuItem.Enabled = true;
            AppData.Main.ReloadRulesToolStripMenuItem.Enabled = true;

            if (AppData.Main.FVariant.Length == 0)
            {
                AppData.Main.FVariant = " * " + AppData.CurrentProject.Name;
            }
            try
            {
                AppData.Main.Text += AppData.Main.FVariant;
            }
            catch
            {
            }

            Properties.Settings.Default.ProjectNewLineSymbol = (AppData.CurrentProject != null)
                ? AppData.CurrentProject.NewlineSymbol
                : Environment.NewLine;

            AfterOpenCleaning();

            Properties.Settings.Default.ProjectIsOpened = true;
            FunctionsSounds.OpenProjectComplete();

            FunctionsLoadTranslationDB.LoadTranslationIfNeed();
        }

        static DataTable[] Sort(DataTableCollection tables)
        {
            Dictionary<string, List<DataTable>> sortedByExt = new Dictionary<string, List<DataTable>>();
            List<DataTable> sortedNoExt = new List<DataTable>();
            foreach (DataTable table in tables)
            {
                var ext = Path.GetExtension(table.TableName);
                if (string.IsNullOrWhiteSpace(ext))
                {
                    sortedNoExt.Add(table);
                }
                else
                {
                    if (!sortedByExt.ContainsKey(ext)) sortedByExt.Add(ext, new List<DataTable>());

                    sortedByExt[ext].Add(table);
                }
            }

            List<DataTable> result = new List<DataTable>();
            result.AddRange(sortedNoExt.OrderBy(t => t.TableName));
            sortedByExt = sortedByExt.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
            foreach (var list in sortedByExt.Values)
            {
                result.AddRange(list.OrderBy(t => t.TableName));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Add last successfully opened project to recent files list
        /// </summary>
        internal static void UpdateRecentFiles()
        {
            string[] items;
            bool changed = false;
            if (!AppData.ConfigIni.SectionExistsAndNotEmpty("RecentFiles"))
            {
                changed = true;
                items = new[] { AppData.SelectedFilePath };
            }
            else
            {
                var values = AppData.ConfigIni.GetSectionValues("RecentFiles").Where(path => File.Exists(path) || Directory.Exists(path)).ToList();

                // max 20 items
                while (values.Count >= 20)
                {
                    changed = true;
                    values.RemoveAt(values.Count - 1); // remove last when more of limit
                }

                // check if last value is on first place else update
                if (!values.Contains(AppData.SelectedFilePath) && !string.IsNullOrWhiteSpace(AppData.SelectedFilePath))
                {
                    changed = true;
                    values.Insert(0, AppData.SelectedFilePath);
                }
                else if (values.IndexOf(AppData.SelectedFilePath) > 0)
                {
                    changed = true;
                    values.Remove(AppData.SelectedFilePath);
                    values.Insert(0, AppData.SelectedFilePath);
                }

                items = values.ToArray();
            }


            if (changed) // write only when changed
            {
                // save values in ini
                AppData.ConfigIni.SetArrayToSectionValues("RecentFiles", items);
            }

            AddRecentMenuItems(items);
        }

        private static void AddRecentMenuItems(string[] items)
        {
            var recentMenuName = T._("Recent");

            // search old menu
            ToolStripMenuItem category = null;
            bool foundOld = false;
            foreach (ToolStripMenuItem menuCategory in AppData.Main.fileToolStripMenuItem.DropDownItems)
            {
                if (menuCategory.Text == recentMenuName)
                {
                    foundOld = true;
                    category = menuCategory;
                    break;
                }
            }

            //ProjectData.Main.fileToolStripMenuItem.DropDownItems
            if (!foundOld)
            {
                category = new System.Windows.Forms.ToolStripMenuItem
                {
                    Text = recentMenuName
                };
            }
            else
            {
                category.DropDownItems.Clear();
            }

            foreach (var item in items)
            {
                var ItemMenu = new System.Windows.Forms.ToolStripMenuItem
                {
                    Text = item
                };
                category.DropDownItems.Add(ItemMenu);
                ItemMenu.Click += RecentFilesOpen_Click;
            }

            if (!foundOld)
            {
                AppData.Main.Invoke((Action)(() => AppData.Main.fileToolStripMenuItem.DropDownItems.Add(category)));
            }
        }

        private static void RecentFilesOpen_Click(object sender, EventArgs e)
        {
            OpenProject((sender as ToolStripMenuItem).Text);
        }

        private static void AfterOpenCleaning()
        {
            AppData.CurrentProject.TablesLinesDict?.Clear();
            AppData.ENQuotesToJPLearnDataFoundNext?.Clear();
            AppData.ENQuotesToJPLearnDataFoundPrev?.Clear();
        }
    }
}
