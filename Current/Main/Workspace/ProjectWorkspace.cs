using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Models;
using TranslationHelper.Projects;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// One open project: its data, its panel, and the wiring between them.
    /// <para>
    /// This is the composition point of the multi-project workspace, in the same spirit as
    /// <see cref="Functions.FileElementsFunctions.Row.RowServices"/> is for the row framework. It is
    /// the only place that knows a project's files list is filled from
    /// <see cref="OpenedFilesData.OpenedFilesList"/>, that a project's tab page holds a
    /// <see cref="ProjectFilesWorkspacePanel"/>, and that the entry the user picked is the one the
    /// project's single <see cref="OpenedFileWorkspace"/> is bound to. Nothing else has to be told when
    /// a project is opened or closed, or when another file is selected in it.
    /// </para>
    /// <para>
    /// It implements <see cref="IProjectWorkspace"/> so the functions that used to read the single
    /// global set of controls can be handed this project's controls instead.
    /// </para>
    /// </summary>
    internal sealed class ProjectWorkspace : IProjectWorkspace
    {
        private readonly ProjectBase _project;
        private readonly ProjectFilesWorkspacePanel _panel;
        private readonly OpenedFileWorkspace _fileWorkspace;

        internal ProjectWorkspace(ProjectBase project)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));

            _panel = new ProjectFilesWorkspacePanel { Dock = DockStyle.Fill };
            _panel.Initialize(this);

            // One workspace for the whole project: selecting an entry repoints it rather than creating
            // anything, so a project with a thousand files has one grid, not a thousand.
            _fileWorkspace = _panel.OpenedFileWorkspace;
            _fileWorkspace.Initialize(this);

            // The completion label is the project's, so its tooltip is set once here rather than on
            // every refresh of the count.
            FunctionsUI.SetTooltips(_panel.TableCompleteInfoLabel);

            // The selection is owned by the model, so the files list reports into it and this follows
            // it. That is what keeps the list and the grid from disagreeing about which file is shown.
            _project.OpenedFilesData.PropertyChanged += OnOpenedFilesDataChanged;
        }

        #region IProjectWorkspace

        public ProjectBase Project => _project;

        public OpenedFilesData OpenedFilesData => _project.OpenedFilesData;

        public FilesListControlBase FilesList => _panel.FilesListControl;

        public Label CompletionLabel => _panel.TableCompleteInfoLabel;

        public OpenedFileWorkspace ActiveFileWorkspace => _fileWorkspace.File == null ? null : _fileWorkspace;

        #endregion

        /// <summary>
        /// Build the tab page a project is shown in.
        /// </summary>
        internal TabPage CreateProjectTabPage()
        {
            // project tab page name will be project name + project directory name
            string projectDirectoryName = Path.GetFileName(_project.SelectedGameDir);
            string tabName = $"[{_project.Name}] {projectDirectoryName}";
            return new TabPage(tabName) { Controls = { _panel } };
        }

        /// <summary>
        /// Fill the project's entries from its content, leaving nothing selected.
        /// <para>
        /// Called once the project has finished parsing, because the entries are the parsed file
        /// tables: the "[ALL]" entry first, then one entry per file. Rebuilding the aggregate before
        /// this is what makes the "[ALL]" entry present the files as they are rather than as they were.
        /// </para>
        /// </summary>
        internal void Initialize()
        {
            var opened = _project.OpenedFilesData;
            opened.Clear();

            // Builds the aggregate table and marks the "[ALL]" entry as part of the list.
            _project.FilesListContent.Initialize();

            if (_project.FilesContent.Tables.Count > 0)
            {
                opened.InsertFirst(new OpenedFileData(_project.FilesListContent.GetTable(0)));
            }

            foreach (DataTable table in _project.FilesContent.Tables)
            {
                opened.Add(new OpenedFileData(
                    table.TableName,
                    _project.ProjectPath,
                    _project.GetFormatOf(table),
                    table));
            }

            _panel.RefreshFilesList();
            _panel.ShowCompletion();

            // Nothing is shown yet, and that is deliberate: the grid is bound when an entry is
            // selected, and the user has not picked one. The files list is what they pick from.
            // "Nothing is selected" is the state Clear() above has already left behind.
        }

        /// <summary>
        /// Drop the project's files and stop following the list. Called when the project is closed.
        /// </summary>
        internal void Clear()
        {
            _project.OpenedFilesData.PropertyChanged -= OnOpenedFilesDataChanged;

            _project.FilesListContent.Reset();
            _project.OpenedFilesData.Clear();

            _panel.Unbind();
            _panel.RefreshFilesList();

            // The grid is emptied while its controls still exist: removing the project disposes its
            // tab, and binding a disposed grid would touch controls that are gone.
            _fileWorkspace.Bind(null);
        }

        /// <summary>
        /// Show the entry the model has selected: bind the project's grid to it, highlight its row in
        /// the files list, fill the text boxes, and let the window's menus catch up.
        /// </summary>
        private void OnOpenedFilesDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(OpenedFilesData.SelectedOpenedFileData)) return;

            var selected = _project.OpenedFilesData.SelectedOpenedFileData;

            // The "[ALL]" entry is a view over the files, so it is rebuilt from them whenever it is
            // shown and kept in step with them while it stays shown. Every other entry presents one
            // file and needs neither. The rebuild comes first: it is the table the grid is about to be
            // bound to.
            if (selected != null && selected.IsAllFilesAggregate)
            {
                _project.FilesListContent.Refresh();
                _project.FilesListContent.AttachToFiles();
            }
            else
            {
                _project.FilesListContent.DetachFromFiles();
            }

            // Whether a file is being worked on decides which commands the window's menus offer at
            // all: a command that acts on a row is left out while there is no row to act on, and the
            // shortcut key is declared on the command, so a shortcut exists only once its command does.
            // The menus therefore follow this, and it is the change of it that matters, so it is read
            // on both sides of the bind — an entry replacing another entry leaves the menus describing
            // the same commands.
            //
            // Only the selected project is asked, and only the selected project may rebuild: the menus
            // belong to the window and are shared by every open project, so they describe the project
            // the user is looking at. Asking through the application-wide predicate is what makes the
            // two agree — it is the same one the menu builder filters with.
            var isSelected = ReferenceEquals(AppData.ActiveWorkspace, this);
            var wasWorkingOnFile = isSelected && AppSettings.IsFileOpened;

            // One grid for the project, repointed at the entry being worked on. It is prepared again on
            // every bind, because a grid takes its columns from the table it is bound to and every entry
            // has its own table.
            _fileWorkspace.Bind(selected);

            var isWorkingOnFile = isSelected && AppSettings.IsFileOpened;

            _panel.SelectEntry(selected);

            // The text boxes belong to the workspace, so showing another entry has to be followed by
            // pointing them at its selected row. The grid may already have been showing that row, in
            // which case no selection change is raised and this is the only thing that fills them.
            FunctionsUI.UpdateTextboxes(this);

            // The menus are rebuilt only when what they can offer has changed, which is when a file
            // starts or stops being worked on: selecting another entry leaves them describing the same
            // commands, so nothing has to be done for it. The project has to be the selected one to
            // rebuild them at all — see above.
            if (isSelected && wasWorkingOnFile != isWorkingOnFile)
            {
                FunctionsMenus.CreateMenus();
            }
        }
    }
}
