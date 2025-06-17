using NLog;
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
using TranslationHelper.Projects.zzzOtherProject;
using TranslationHelper.Projects.ZZZZFormats;

namespace TranslationHelper.Functions
{
    class FunctionsOpen
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static async Task OpenProject(string filePath = null)
        {
            if (filePath == null || !File.Exists(filePath))
            {
                using (var THFOpen = new OpenFileDialog())
                {
                    THFOpen.InitialDirectory = AppData.Settings.THConfigINI.GetKey("Paths", "LastPath");

                    THFOpen.Filter = GetFilters();

                    if (THFOpen.ShowDialog() != DialogResult.OK || THFOpen.FileName == null)
                    {
                        FunctionsUI.IsOpeningInProcess = false;
                        return;
                    }

                    filePath = THFOpen.FileName;
                }
            }

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                FunctionsUI.IsOpeningInProcess = false;
                Logger.Info(T._("Nothing to open"));
                return;
            }

            FunctionsCleanup.THCleanupThings();

            AppData.SelectedProjectFilePath = filePath;

            ProjectBase project = null;
            await Task.Run(() => project = TrySearchAndOpenProject(AppData.SelectedProjectFilePath)).ConfigureAwait(true);

            if (project == null)
            {
                AppData.Main.frmMainPanel.Visible = false;
                FunctionsSounds.OpenProjectFailed();
                Logger.Info(T._("Nothing to open"));
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

                await AfterOpenActions(project);
            }

            FunctionsUI.IsOpeningInProcess = false;
        }

        public class FormatFilterData
        {
            public string Name { get; set; }
            public string[] ExtensionMasks { get; set; }
        }

        static IEnumerable<FormatFilterData> GetFormatsData()
        {
            var dummyProject = new DummyProject();
            foreach (var format in GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IFormatMetadata>(assembly: null, dummyProject))
            {
                if (string.IsNullOrWhiteSpace(format.Extension)) continue;

                var formatData = new FormatFilterData
                {
                    Name = format.Description
                };
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

            string pFolderName = Path.GetFileName(tHSelectedGameDir);
            if (string.Compare(pFolderName, "data", true, CultureInfo.InvariantCulture) == 0) return Path.GetDirectoryName(Path.GetDirectoryName(tHSelectedGameDir));

            return tHSelectedGameDir;
        }

        private static ProjectBase TrySearchAndOpenProject(string projectPath)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(projectPath));

            AppData.Main.frmMainPanel.Invoke((Action)(() => AppData.Main.frmMainPanel.Visible = true));

            var foundTypes = new List<Type>();

            foreach (Type Project in AppData.ProjectsList)
            {
                var project = (ProjectBase)Activator.CreateInstance(Project);
                project.ProjectPath = projectPath;
                project.OpenFileMode = true;
                project.OpenedFilesDir = dir.FullName;
                project.SelectedDir = dir.FullName;
                project.SelectedGameDir = dir.FullName;
                AppData.SelectedProjectFilePath = projectPath;

                if (TryGetValidProject(project))
                {
                    foundTypes.Add(Project);
                }
            }

            if (foundTypes.Count == 0)
            {
                Logger.Info(T._("No projects found to open with"));
                return null;
            }


            if (foundTypes.Count == 1)
            {
                Logger.Info(T._("Found one project. Trying to open with it.."));
                return TryOpenSelectedProject(foundTypes[0], projectPath, dir);
            }
            else
            {
                Logger.Info(T._("Found {0} projects to open with."), foundTypes.Count);
            }

                int selectedIndex = -1;
            var foundForm = new FoundTypesbyExtensionForm();
            foreach (var type in foundTypes)
            {
                var inst = (IProject)Activator.CreateInstance(type);
                var instName = !string.IsNullOrWhiteSpace(inst.Name) ? inst.Name + " (" + type.FullName + ")" : type.FullName;
                foundForm.listBox1.Items.Add(instName);
            }

            var result = foundForm.ShowDialog();
            if (result == DialogResult.OK) selectedIndex = foundForm.SelectedTypeIndex;

            foundForm.Dispose();

            if (selectedIndex > -1) return TryOpenSelectedProject(foundTypes[selectedIndex], projectPath, dir);

            return TryOpenSelectedProject(foundTypes[0], projectPath, dir);
        }

        private static ProjectBase TryOpenSelectedProject(Type type, string sPath, DirectoryInfo dir)
        {
            var project = (ProjectBase)Activator.CreateInstance(type);
            Logger.Info(T._("Open with {0}"), project.Name);

            project.OpenFileMode = true;
            project.OpenedFilesDir = dir.FullName;
            project.SelectedDir = dir.FullName;
            project.SelectedGameDir = dir.FullName;

            AppData.CurrentProject = project;
            AppData.SelectedProjectFilePath = sPath;

            if (!TryOpenProject(project))
            {
                Logger.Warn(T._("Failed to open project"));
                return null;
            }

            return project;
        }

        private static bool TryOpenProject(ProjectBase project)
        {
            if (project == null)
            {
                Logger.Error(T._("Current project is null"));
                return false;
            }

            project.Init();
            project.BakRestore();
            project.OpenFileMode = true;
            if (project.Open())
            {
                MenusCreator.CreateMenus();
                return true;
            }
            return false;
        }

        private static bool TryGetValidProject(ProjectBase project)
        {
            if (project.IsValid())
            {
                return true;
            }
            return false;
        }

        internal static async Task AfterOpenActions(ProjectBase project)
        {
            project.SaveFileMode = true;

            if (!AppData.Main.THWorkSpaceSplitContainer.Visible) AppData.Main.THWorkSpaceSplitContainer.Visible = true;

            if (AppData.Main.THFilesList.GetItemsCount() == 0 && project.FilesContent.Tables.Count > 0)
            {
                var sortedtables = Sort(AppData.CurrentProject.FilesContent.Tables);
                project.FilesContent.Tables.Clear();
                project.FilesContent.Tables.AddRange(sortedtables);

                var sortedtablesinfo = Sort(project.FilesContentInfo.Tables);
                project.FilesContentInfo.Tables.Clear();
                project.FilesContentInfo.Tables.AddRange(sortedtablesinfo);

                foreach (DataTable table in project.FilesContent.Tables)
                {
                    AppData.Main.THFilesList.AddItem(table.TableName);
                }
            }

            FunctionsMenus.CreateMainMenus();
            FunctionsMenus.CreateFilesListMenus();

            project.SelectedGameDir = GetCorrectedGameDIr(project.SelectedGameDir);

            if (project.Name.Contains("RPG Maker game with RPGMTransPatch") || project.Name.Contains("KiriKiri game"))
            {
                AppData.Settings.THConfigINI.SetKey("Paths", "LastPath", project.SelectedGameDir);
            }
            else
            {
                try
                {
                    AppData.Settings.THConfigINI.SetKey("Paths", "LastPath", project.SelectedDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured:" + Environment.NewLine + ex);
                }
            }

            if (AppData.Main.FVariant.Length == 0)
            {
                AppData.Main.FVariant = " * " + project.Name;
            }
            try
            {
                AppData.Main.Text += AppData.Main.FVariant;
            }
            catch { }

            AppSettings.ProjectNewLineSymbol = (AppData.CurrentProject != null)
                ? project.NewlineSymbol
                : Environment.NewLine;

            MenuItemRecent.AfterOpenCleaning();

            FunctionsSounds.OpenProjectComplete();

            await FunctionsLoadTranslationDB.LoadTranslationIfNeed().ConfigureAwait(true);
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
