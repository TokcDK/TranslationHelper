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
            try
            {
                if (filePath == null || !File.Exists(filePath))
                {
                    using (var THFOpen = new OpenFileDialog())
                    {
                        THFOpen.InitialDirectory = AppData.Settings.THConfigINI.GetKey("Paths", "LastPath");

                        THFOpen.Filter = GetFilters();
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

                AppData.SelectedProjectFilePath = filePath;

                bool isProjectFound = false;
                await Task.Run(() => isProjectFound = GetSourceType(AppData.SelectedProjectFilePath)).ConfigureAwait(true);

                AppData.Main.ProgressInfo(false, string.Empty);

                if (!isProjectFound)
                {
                    AppData.Main.frmMainPanel.Visible = false;

                    FunctionsSounds.OpenProjectFailed();
                    MessageBox.Show(T._("Failed to open project"));
                }
                else
                {
                    AfterOpenActions();
                }

                AppData.Main.IsOpeningInProcess = false;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile("An error occured while opening the project. error:\r\n" + ex);
            }
        }

        private static void AfterOpenActions()
        {
            AppData.CurrentProject.SaveFileMode = true; // project opened. dont need to openfilemode

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

            FunctionsLoadTranslationDB.LoadTranslationIfNeed();
        }

        private static DataTable[] Sort(DataTableCollection tables)
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

            if (foundTypes.Count == 0) return false;

            if (foundTypes.Count == 1) return TryOpenSelectedProject(foundTypes[0], sPath, dir);

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
            AppData.CurrentProject.OpenFileMode = true;
            AppData.SelectedProjectFilePath = sPath;
            AppData.CurrentProject.OpenedFilesDir = dir.FullName;
            AppData.CurrentProject.SelectedDir = dir.FullName;
            AppData.CurrentProject.SelectedGameDir = dir.FullName;

            if (!TryOpenProject()) return false;

            return true;
        }

        private static bool TryOpenProject()
        {
            AppData.CurrentProject.Init();
            AppData.CurrentProject.BakRestore();
            AppData.CurrentProject.OpenFileMode = true;
            if (AppData.CurrentProject.Open())
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

        private static IEnumerable<FormatFilterData> GetFormatsData()
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
    }
}

