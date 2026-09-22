using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Main.Functions;
using TranslationHelper.Models;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// The files of one project: the files list on the left, the entry being worked on on the right,
    /// and the count of translated rows below them.
    /// <para>
    /// One instance is created per open project and lives in that project's tab, so the list a user
    /// sees always belongs to the project they are looking at. The panel is a view: it wires the
    /// controls it owns to the functions that own the behaviour, and hands those functions itself so
    /// they read this project's controls.
    /// </para>
    /// <para>
    /// The right-hand side is one <see cref="OpenedFileWorkspace"/> for the whole project, not one per
    /// file: there is no tab per file any more. Selecting an entry in the list repoints that workspace
    /// at the entry's table, which is what <see cref="ProjectWorkspace"/> does when the selection
    /// changes.
    /// </para>
    /// </summary>
    internal partial class ProjectFilesWorkspacePanel : UserControl
    {
        /// <summary>
        /// True while this panel is the one changing the list selection. Without it, following the
        /// model would look like the user having chosen an entry.
        /// </summary>
        private bool _syncingSelection;

        /// <summary>
        /// The project this panel presents. Set by <see cref="Initialize"/>.
        /// </summary>
        internal IProjectWorkspace Workspace { get; private set; }

        /// <summary>
        /// The adapter that reads and writes <see cref="FilesList"/>. Built by
        /// <see cref="Initialize"/>, because it needs to know which project the list belongs to.
        /// </summary>
        internal FilesListControlBase FilesListControl { get; private set; }

        internal ProjectFilesWorkspacePanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Wire the files list to <paramref name="workspace"/>.
        /// </summary>
        internal void Initialize(IProjectWorkspace workspace)
        {
            Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));

            FilesListControl = new FilesListControlListBox(
                FilesList,
                TableOfEntry,
                OnFilesListSelectionChanged);

            // A file that is opened or closed changes the list, so the list follows the entries rather
            // than a caller having to remember to add or remove a line.
            Workspace.OpenedFilesData.OpenedFilesList.ListChanged += OnOpenedFilesListChanged;
        }

        /// <summary>
        /// Stop following the project's files. Called when the project is closed, so the panel does not
        /// outlive the list it was showing.
        /// </summary>
        internal void Unbind()
        {
            Workspace.OpenedFilesData.OpenedFilesList.ListChanged -= OnOpenedFilesListChanged;
        }

        private void OnOpenedFilesListChanged(object sender, ListChangedEventArgs e)
        {
            RefreshFilesList();
        }

        /// <summary>
        /// Show one line per entry of <see cref="OpenedFilesData.OpenedFilesList"/>, in list order.
        /// </summary>
        internal void RefreshFilesList()
        {
            _syncingSelection = true;
            try
            {
                int selectedIndex = FilesList.SelectedIndex;

                FilesList.BeginUpdate();
                FilesList.Items.Clear();

                foreach (var file in Workspace.OpenedFilesData.OpenedFilesList)
                {
                    FilesList.Items.Add(file.FileName);
                }

                FilesList.EndUpdate();

                if (selectedIndex >= 0 && selectedIndex < FilesList.Items.Count)
                {
                    FilesList.SelectedIndex = selectedIndex;
                }
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        /// <summary>
        /// Highlight the line presenting <paramref name="file"/>, without reporting it as a choice the
        /// user made.
        /// </summary>
        internal void SelectEntry(OpenedFileData file)
        {
            int index = file == null ? -1 : Workspace.OpenedFilesData.OpenedFilesList.IndexOf(file);
            if (index < 0 || index == FilesList.SelectedIndex) return;

            _syncingSelection = true;
            try
            {
                FilesList.SelectedIndex = index;
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        /// <summary>
        /// The table the files list entry at <paramref name="entryIndex"/> presents, or null when the
        /// project has no such entry.
        /// </summary>
        private DataTable TableOfEntry(int entryIndex)
        {
            return Workspace.Project?.FilesListContent?.GetTable(entryIndex);
        }

        /// <summary>
        /// The user selected a different entry: make it the entry being worked on.
        /// <para>
        /// This is also what points the project's grid at it. Nothing is shown for an entry until it is selected, so the
        /// selection is the only thing that shows a file.
        /// </para>
        /// </summary>
        private void OnFilesListSelectionChanged()
        {
            if (_syncingSelection) return;

            int index = FilesList.SelectedIndex;
            var entries = Workspace.OpenedFilesData.OpenedFilesList;

            if (index < 0 || index >= entries.Count) return;

            Workspace.OpenedFilesData.SelectedOpenedFileData = entries[index];
        }

        /// <summary>
        /// Show how much of the project is translated.
        /// </summary>
        internal void ShowCompletion()
        {
            FunctionsUI.ShowNonEmptyRowsCount(Workspace.Project, TableCompleteInfoLabel);
        }

        private void TableCompleteInfoLabel_Click(object sender, EventArgs e)
        {
            FunctionsTable.ShowFirstRowWithEmptyTranslation(Workspace);
        }
    }
}

