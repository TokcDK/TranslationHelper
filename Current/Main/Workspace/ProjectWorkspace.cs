using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
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
    /// <see cref="OpenedFilesData.OpenedFilesList"/>, that a tab page holds a
    /// <see cref="ProjectFilesWorkspacePanel"/>, and that a file's tab page holds an
    /// <see cref="OpenedFileWorkspace"/>. Nothing else has to be told when a project is opened or
    /// closed.
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

        internal ProjectWorkspace(ProjectBase project)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));

            _panel = new ProjectFilesWorkspacePanel { Dock = DockStyle.Fill };
            _panel.Initialize(this);

            _panel.OpenedFilesTabControl.Bind(_project.OpenedFilesData, CreateFileTabPage);

            // The completion label is the project's, so its tooltip is set once here rather than on
            // every refresh of the count.
            FunctionsUI.SetTooltips(_panel.TableCompleteInfoLabel);

            // The selection is owned by the model, so both the files list and the tabs report into it
            // and both follow it. That is what keeps them from disagreeing about which file is shown.
            _project.OpenedFilesData.PropertyChanged += OnOpenedFilesDataChanged;
        }

        #region IProjectWorkspace

        public ProjectBase Project => _project;

        public OpenedFilesData OpenedFilesData => _project.OpenedFilesData;

        public FilesListControlBase FilesList => _panel.FilesListControl;

        public TabControl OpenedFilesTabs => _panel.OpenedFilesTabControl.Tabs;

        public Label CompletionLabel => _panel.TableCompleteInfoLabel;

        public OpenedFileWorkspace ActiveFileWorkspace
        {
            get
            {
                var page = OpenedFilesTabs.SelectedTab;
                if (page == null || page.Controls.Count == 0) return null;

                return page.Controls[0] as OpenedFileWorkspace;
            }
        }

        #endregion

        /// <summary>
        /// Build the tab page a project is shown in.
        /// </summary>
        internal TabPage CreateProjectTabPage()
        {
            return new TabPage(_project.Name) { Tag = _project, Controls = { _panel } };
        }

        /// <summary>
        /// Fill the project's opened files from its content and show the first one.
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

            opened.SelectedOpenedFileData = opened.OpenedFilesList.Count > 0
                ? opened.OpenedFilesList[0]
                : null;
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
            _panel.OpenedFilesTabControl.Unbind();
            _panel.RefreshFilesList();
        }

        /// <summary>
        /// Show the entry the model has selected: its tab, its row in the files list, and its content.
        /// </summary>
        private void OnOpenedFilesDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(OpenedFilesData.SelectedOpenedFileData)) return;

            var selected = _project.OpenedFilesData.SelectedOpenedFileData;

            _panel.OpenedFilesTabControl.SelectFile(selected);
            _panel.SelectEntry(selected);

            // The "[ALL]" entry is a view over the files, so it is rebuilt from them whenever it is
            // shown and kept in step with them while it stays shown. Every other entry presents one
            // file and needs neither.
            if (selected != null && selected.IsAllFilesAggregate)
            {
                _project.FilesListContent.Refresh();
                _project.FilesListContent.AttachToFiles();
                _panel.OpenedFilesTabControl.RefreshSelectedTab();
            }
            else
            {
                _project.FilesListContent.DetachFromFiles();
            }

            // The text boxes belong to the entry, so showing another one has to be followed by pointing
            // them at its selected row. The tab may already have been showing that row, in which case
            // no selection change is raised and this is the only thing that fills them.
            FunctionsUI.UpdateTextboxes(this);
        }

        /// <summary>
        /// Build the tab page of one opened file: the file's own grid, source box and target box.
        /// </summary>
        private TabPage CreateFileTabPage(OpenedFileData file)
        {
            var workspace = new OpenedFileWorkspace { Dock = DockStyle.Fill };
            workspace.Initialize(this, file);

            // One file's grid is prepared once, when its controls are built: which columns it shows,
            // what its two work columns are called and that they may be edited. It used to be redone
            // every time the files list selection changed, because there was a single grid being
            // repointed at another table.
            FunctionsUI.PrepareElementsGrid(workspace);

            return new TabPage(file.FileName) { Tag = file, Controls = { workspace } };
        }
    }
}
