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
using TranslationHelper.Workspace;
using MessageBox = TranslationHelper.Theming.ThemedMessageBox;

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
                    THFOpen.InitialDirectory = AppData.ConfigIni.GetKey("Paths", "LastPath");

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

            try
            {
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
            }
            finally
            {
                // The open is over whatever it did, and the project it was for is either open and
                // selected or was not opened at all: from here the selected project is the answer
                // again.
                AppData.OpeningProject = null;

                FunctionsUI.IsOpeningInProcess = false;
            }
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

        private static string GetCorrectedGameDir(ProjectBase project, string tHSelectedGameDir)
        {
            if (string.IsNullOrEmpty(tHSelectedGameDir)) tHSelectedGameDir = project.SelectedDir;

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
            using (var foundForm = new FoundTypesbyExtensionForm())
            {
                foreach (var type in foundTypes)
                {
                    var inst = (IProject)Activator.CreateInstance(type);
                    var instName = !string.IsNullOrWhiteSpace(inst.Name) ? inst.Name + " (" + type.FullName + ")" : type.FullName;
                    foundForm.SelectTypeListBox.Items.Add(instName);
                }

                if (foundForm.ShowDialog() == DialogResult.OK) selectedIndex = foundForm.SelectedTypeIndex;
            }

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

            // The path is recorded before the project is opened because the project's own Init reads it,
            // and the project is not part of the open projects yet: it is added only once it has opened,
            // so a project that fails to parse never appears as a tab.
            AppData.SelectedProjectFilePath = sPath;

            // Same reason, for the project itself: from here until the end of the open this is the
            // project being worked on, and it is the only project that knows what it is opening.
            AppData.OpeningProject = project;

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
                // The project's menus are built when it becomes the selected one, not here: at this
                // point it is not open yet, so a menu built now would describe the project the user
                // was looking at before.
                return true;
            }
            return false;
        }

        private static bool TryGetValidProject(ProjectBase project)
        {
            return project.IsValid();
        }

        internal static async Task AfterOpenActions(ProjectBase project)
        {
            project.SaveFileMode = true;

            // The table order decides the order of the files list entries, and the "[ALL]" entry is
            // built from the files in their final order, so both are settled before the workspace
            // fills the list.
            if (project.FilesContent.Tables.Count > 0)
            {
                var sortedtables = Sort(project.FilesContent.Tables);
                project.FilesContent.Tables.Clear();
                project.FilesContent.Tables.AddRange(sortedtables);

                var sortedtablesinfo = Sort(project.FilesContentInfo.Tables);
                project.FilesContentInfo.Tables.Clear();
                project.FilesContentInfo.Tables.AddRange(sortedtablesinfo);
            }

            // Showing the project is what creates its tab and its workspace, and the workspace is what
            // fills the files list from the parsed content — including the "[ALL]" entry. A project is
            // added only here, once it has opened, so the tab always presents a project that parsed.
            // Selecting it is also what rebuilds the menus for it.
            var workspace = AppData.Main.Workspace.Add(project);
            workspace.Initialize();

            project.SelectedGameDir = GetCorrectedGameDir(project, project.SelectedGameDir);

            if (project.Name.Contains("RPG Maker game with RPGMTransPatch") || project.Name.Contains("KiriKiri game"))
            {
                AppData.ConfigIni.SetKey("Paths", "LastPath", project.SelectedGameDir);
            }
            else
            {
                try
                {
                    AppData.ConfigIni.SetKey("Paths", "LastPath", project.SelectedDir);
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
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to append the project name to the window title");
            }

            AppSettings.ProjectNewLineSymbol = (AppData.CurrentProject != null)
                ? project.NewlineSymbol
                : Environment.NewLine;

            MenuItemRecent.AfterOpenCleaning();

            FunctionsSounds.OpenProjectComplete();

            // The content is complete from here on: the files are parsed, the files list is filled
            // and the menus exist. Automatic operations may work on it now; reading the translation
            // database is the next step and has its own state, which keeps them out while it runs.
            ProjectReadiness.MarkProjectOpened();

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
