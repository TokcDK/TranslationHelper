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
using TranslationHelper.Menus.MainMenus.File;
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
                    THFOpen.InitialDirectory = AppData.Settings.THConfigINI.GetKey("Paths", "LastPath");

                    //bool tempMode = AppData.CurrentProject.OpenFileMode;
                    //AppData.CurrentProject.OpenFileMode = true;// temporary set open file mode to true if on save mode trying to open files to prevent errors in time of get extensions for filter

                    THFOpen.Filter = GetFilters();
                    //THFOpen.Filter = string.Join(";", THFOpen.Filter.Split(';').Distinct());

                    //AppData.CurrentProject.OpenFileMode = tempMode;
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

            FunctionsCleanup.THCleanupThings();

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

        public class FormatFilterData
        {
            public string Name { get; set; }
            public string[] ExtensionMasks { get; set; }
        }

        static IEnumerable<FormatFilterData> GetFormatsData()
        {
            foreach (var format in GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IFormat>())
            {
                if (string.IsNullOrWhiteSpace(format.Extension)) continue;

                var formatData = new FormatFilterData();

                formatData.Name = format.Name;
                if (string.IsNullOrWhiteSpace(formatData.Name))
                {
                    formatData.Name = format
                        .GetType()
                        .ToString()
                        .Substring($"{nameof(TranslationHelper)}.{nameof(TranslationHelper.Formats)}.".Length);
                }

                formatData.ExtensionMasks = format.Extension
                    .Split(',')
                    .Where(e => e.Length > 0)
                    .Select(e => (e[0] == '.' ? "*" : "") + e)
                    .ToArray();

                yield return formatData;
            }
        }

        private static string GetFilters()
        {
            var formatsDataList = GetFormatsData().ToArray();

            var filtersFromFormats = "Found formats|" + string.Join(";",
                        formatsDataList
                        .Select(d => string.Join(";", d.ExtensionMasks))
                        );
            var filtersFromFormatsSplitted = string.Join("|",
                        formatsDataList
                        .Select(d => $"{d.Name}|{string.Join(";", d.ExtensionMasks)}")
                        );
            return $"{filtersFromFormats}|{filtersFromFormatsSplitted}|RPGMakerTrans patch|RPGMKTRANSPATCH|Application EXE|*.exe|KiriKiri engine files|*.scn;*.ks|Txt file|*.txt|All|*.*";
        }

        private static string GetCorrectedGameDIr(string tHSelectedGameDir)
        {
            if (tHSelectedGameDir.Length == 0) tHSelectedGameDir = AppData.CurrentProject.SelectedDir;

            //для rpgmaker mv. если была папка data, которая в папке www
            string pFolderName = Path.GetFileName(tHSelectedGameDir);
            if (string.Compare(pFolderName, "data", true, CultureInfo.InvariantCulture) == 0) return Path.GetDirectoryName(Path.GetDirectoryName(tHSelectedGameDir));

            return tHSelectedGameDir;
        }

        internal DirectoryInfo mvdatadir;
        private static bool GetSourceType(string sPath)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));

            AppData.Main.frmMainPanel.Invoke((Action)(() => AppData.Main.frmMainPanel.Visible = true));

            var foundTypes = new List<Type>();

            //Try detect and open new type projects
            foreach (Type Project in AppData.ProjectsList) // iterate projectbase types
            {
                AppData.CurrentProject = (ProjectBase)Activator.CreateInstance(Project);// create instance of project
                AppData.CurrentProject.OpenFileMode = true;
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
            AppData.CurrentProject.OpenFileMode = true;
            AppData.SelectedFilePath = sPath;
            AppData.CurrentProject.OpenedFilesDir = dir.FullName;
            AppData.CurrentProject.SelectedDir = dir.FullName;
            AppData.CurrentProject.SelectedGameDir = dir.FullName;

            if (!TryOpenProject()) return false;

            return true;
        }

        /// <summary>
        /// Try to open project and return project name if open is success
        /// </summary>
        /// <returns></returns>
        private static bool TryOpenProject()
        {
            AppData.CurrentProject.Init();
            AppData.CurrentProject.BakRestore();
            AppData.CurrentProject.OpenFileMode = true;
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
            if (Project.IsValid())
            {
                AppData.CurrentProject = Project;
                return true;
            }
            return false;
        }

        internal static void AfterOpenActions()
        {
            AppData.CurrentProject.SaveFileMode = true; // project opened. dont need to openfilemode

            if (!AppData.Main.THWorkSpaceSplitContainer.Visible) AppData.Main.THWorkSpaceSplitContainer.Visible = true;

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

            FunctionsMenus.CreateMainMenus();
            FunctionsMenus.CreateFilesListMenus();

            MenuItemRecent.UpdateRecentFiles();

            AppData.CurrentProject.SelectedGameDir = GetCorrectedGameDIr(AppData.CurrentProject.SelectedGameDir);

            if (AppData.CurrentProject.Name.Contains("RPG Maker game with RPGMTransPatch") || AppData.CurrentProject.Name.Contains("KiriKiri game"))
            {
                AppData.Settings.THConfigINI.SetKey("Paths", "LastPath", AppData.CurrentProject.SelectedGameDir);
            }
            else
            {
                try
                {
                    AppData.Settings.THConfigINI.SetKey("Paths", "LastPath", AppData.CurrentProject.SelectedDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured:" + Environment.NewLine + ex);
                }
            }
            //_ = /*THMsg*/MessageBox.Show(ProjectData.CurrentProject.Name() + T._(" loaded") + "!");

            //AppData.Main.EditToolStripMenuItem.Enabled = true;
            //AppData.Main.ViewToolStripMenuItem.Enabled = true;
            //AppData.Main.OpenProjectsDirToolStripMenuItem.Enabled = true;
            //AppData.Main.OpenTranslationRulesFileToolStripMenuItem.Enabled = true;
            //AppData.Main.OpenCellFixesFileToolStripMenuItem.Enabled = true;
            //AppData.Main.ReloadRulesToolStripMenuItem.Enabled = true;

            if (AppData.Main.FVariant.Length == 0)
            {
                AppData.Main.FVariant = " * " + AppData.CurrentProject.Name;
            }
            try
            {
                AppData.Main.Text += AppData.Main.FVariant;
            }
            catch { }

            AppSettings.ProjectNewLineSymbol = (AppData.CurrentProject != null)
                ? AppData.CurrentProject.NewlineSymbol
                : Environment.NewLine;

            MenuItemRecent.AfterOpenCleaning();

            //AppSettings.ProjectIsOpened = true;
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
            foreach (var list in sortedByExt.Values) result.AddRange(list.OrderBy(t => t.TableName));

            return result.ToArray();
        }
    }
}
