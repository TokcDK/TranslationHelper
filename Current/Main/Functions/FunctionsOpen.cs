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

            bool isProjectFound = false;
            await Task.Run(() => isProjectFound = GetSourceType(AppData.SelectedProjectFilePath)).ConfigureAwait(true);

            if (!isProjectFound)
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

                await AfterOpenActions();
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
            foreach (var format in GetListOfSubClasses.Inherited.GetInterfaceImplimentations<IFormat>())
            {
                if (string.IsNullOrWhiteSpace(format.Extension)) continue;

                var formatData = new FormatFilterData();

                formatData.Name = format.Description;
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

        private static bool GetSourceType(string sPath)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));

            AppData.Main.frmMainPanel.Invoke((Action)(() => AppData.Main.frmMainPanel.Visible = true));

            var foundTypes = new List<Type>();

            foreach (Type Project in AppData.ProjectsList)
            {
                AppData.CurrentProject = (ProjectBase)Activator.CreateInstance(Project);
                AppData.CurrentProject.OpenFileMode = true;
                AppData.SelectedProjectFilePath = sPath;
                AppData.CurrentProject.OpenedFilesDir = dir.FullName;
                AppData.CurrentProject.SelectedDir = dir.FullName;
                AppData.CurrentProject.SelectedGameDir = dir.FullName;

                if (TryDetectProject(AppData.CurrentProject)) foundTypes.Add(Project);
            }

            if (foundTypes.Count == 0)
            {
                Logger.Info(T._("No projects found to open with"));
                return false;
            }


            if (foundTypes.Count == 1)
            {
                Logger.Info(T._("Found one project. Trying to open with it.."));
                return TryOpenSelectedProject(foundTypes[0], sPath, dir);
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

            if (selectedIndex > -1) return TryOpenSelectedProject(foundTypes[selectedIndex], sPath, dir);

            return TryOpenSelectedProject(foundTypes[0], sPath, dir);
        }

        private static bool TryOpenSelectedProject(Type type, string sPath, DirectoryInfo dir)
        {
            AppData.CurrentProject = (ProjectBase)Activator.CreateInstance(type);
            Logger.Info(T._("Open with {0}"), AppData.CurrentProject.Name);

            AppData.CurrentProject.OpenFileMode = true;
            AppData.SelectedProjectFilePath = sPath;
            AppData.CurrentProject.OpenedFilesDir = dir.FullName;
            AppData.CurrentProject.SelectedDir = dir.FullName;
            AppData.CurrentProject.SelectedGameDir = dir.FullName;

            if (!TryOpenProject(AppData.CurrentProject))
            {
                Logger.Warn(T._("Failed to open project"));
                return false;
            }

            return true;
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
            if (project.TryOpen())
            {
                MenusCreator.CreateMenus();
                return true;
            }
            return false;
        }

        private static bool TryDetectProject(ProjectBase Project)
        {
            if (Project.IsValid())
            {
                AppData.CurrentProject = Project;
                return true;
            }
            return false;
        }

        internal static async Task AfterOpenActions()
        {
            AppData.CurrentProject.SaveFileMode = true;

            if (!AppData.Main.THWorkSpaceSplitContainer.Visible) AppData.Main.THWorkSpaceSplitContainer.Visible = true;

            if (AppData.Main.THFilesList.GetItemsCount() == 0 && AppData.CurrentProject.FilesContent.Tables.Count > 0)
            {
                var sortedtables = Sort(AppData.CurrentProject.FilesContent.Tables);
                AppData.CurrentProject.FilesContent.Tables.Clear();
                AppData.CurrentProject.FilesContent.Tables.AddRange(sortedtables);

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
