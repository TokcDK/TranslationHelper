using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Models;
using TranslationHelper.Projects;
using TranslationHelper.Theming;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// Every open project, and the controls that present each of them.
    /// <para>
    /// This is the composition root of the multi-project workspace, in the same spirit as
    /// <see cref="Functions.FileElementsFunctions.Row.RowServices"/> is for the row framework. It is
    /// the only place that knows a tab page of the projects tab control holds a
    /// <see cref="ProjectWorkspace"/>, and the only place that decides which workspace is the active
    /// one. Because of that, opening a project is one call here rather than a sequence of steps spread
    /// over the window, and no caller has to be told when the user changes project.
    /// </para>
    /// <para>
    /// It keeps the map from a project to its workspace rather than looking one up in the tab control,
    /// so a workspace is reachable by the project it belongs to and not only by tab position. A closed
    /// project is dropped from the map in the same call that closes it, so a lookup can never return a
    /// workspace whose controls have been disposed.
    /// </para>
    /// </summary>
    internal sealed class MainWorkspace : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The projects that are open, and which of them is selected.
        /// </summary>
        private readonly ProjectsData _projectsData;

        /// <summary>
        /// The control that shows the projects, one tab each.
        /// </summary>
        private readonly ProjectsTabControl _projectsTabControl;

        /// <summary>
        /// The workspace of each open project, keyed by the project itself.
        /// </summary>
        private readonly Dictionary<ProjectBase, ProjectWorkspace> _workspaces
            = new Dictionary<ProjectBase, ProjectWorkspace>();

        private bool _disposed;

        internal MainWorkspace(ProjectsData projectsData)
        {
            _projectsData = projectsData ?? throw new ArgumentNullException(nameof(projectsData));

            _projectsTabControl = new ProjectsTabControl { Dock = DockStyle.Fill };

            // Binding is what makes a tab appear per project, so adding a project below needs no
            // second step. The factory passed here is the one that builds a project's workspace, which
            // is how the map above stays complete; the second one is what a click on a tab's close
            // button reaches, which is how closing a project goes through the same door as opening it.
            _projectsTabControl.Bind(_projectsData, CreateProjectTabPage, Close);

            // The selection is owned by the model, so the active workspace follows it rather than
            // being set by whoever changed the tab.
            _projectsData.PropertyChanged += OnProjectsDataChanged;

            SyncActiveWorkspace();
        }

        /// <summary>
        /// The control to host in the window.
        /// </summary>
        internal Control Control => _projectsTabControl;

        /// <summary>
        /// The workspace of <paramref name="project"/>, or null when it is not open.
        /// </summary>
        internal ProjectWorkspace WorkspaceOf(ProjectBase project)
        {
            if (project == null) return null;

            return _workspaces.TryGetValue(project, out var workspace) ? workspace : null;
        }

        /// <summary>
        /// The workspace of the selected project, or null when no project is open.
        /// </summary>
        internal ProjectWorkspace SelectedWorkspace => WorkspaceOf(_projectsData.SelectedProject);

        /// <summary>
        /// Show <paramref name="project"/> as an open project: add it to the list, which creates its
        /// tab and its workspace, and select it.
        /// <para>
        /// The project has to be opened — parsed — before this is called, because the workspace is
        /// filled from the parsed content. A project that is already open is selected rather than added
        /// a second time, which is what makes opening the same file twice harmless.
        /// </para>
        /// </summary>
        /// <returns>The project's workspace.</returns>
        internal ProjectWorkspace Add(ProjectBase project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));

            var existing = WorkspaceOf(project);
            if (existing != null)
            {
                _projectsData.SelectedProject = project;
                return existing;
            }

            _projectsData.Add(project);
            _projectsData.SelectedProject = project;

            // Creating the tab page is what registered the workspace, so the map is filled by now.
            return WorkspaceOf(project);
        }

        /// <summary>
        /// Close <paramref name="project"/> as the user asked: offer to save what its database does not
        /// have yet, then drop its files, remove its tab, and move the selection to the project that
        /// takes its place.
        /// <para>
        /// This is what a tab's close button and the Close project command both reach. It is here
        /// rather than in either of them because the two have to do the same thing, and because what
        /// has to be saved is a property of the project rather than of the way it was closed.
        /// </para>
        /// </summary>
        /// <returns>True when the project was open and has been closed.</returns>
        internal bool Close(ProjectBase project)
        {
            if (project == null) return false;

            var workspace = WorkspaceOf(project);
            if (workspace == null) return false;

            if (!ConfirmClose(workspace)) return false;

            return Remove(project);
        }

        /// <summary>
        /// Close <paramref name="project"/>: drop its files, remove its tab, and move the selection to
        /// the project that takes its place.
        /// </summary>
        /// <returns>True when the project was open and has been closed.</returns>
        internal bool Remove(ProjectBase project)
        {
            if (project == null) return false;

            var workspace = WorkspaceOf(project);
            if (workspace == null) return false;

            // The workspace is emptied while its controls still exist: removing the project disposes
            // its tab, and clearing a disposed workspace would touch controls that are gone.
            workspace.Clear();
            _workspaces.Remove(project);

            _projectsData.Remove(project);

            return true;
        }

        /// <summary>
        /// Ask the user about the translations of <paramref name="workspace"/> that its database does
        /// not have yet, and write them out when that is what they chose.
        /// <para>
        /// The question is asked only when there is something to lose. A project whose translations are
        /// already in its database is closed without a word, which is what keeps the prompt meaning
        /// something: it appears exactly when closing would throw work away.
        /// </para>
        /// </summary>
        /// <returns>False when the close has to be abandoned, either because the user cancelled or
        /// because the save they asked for did not happen.</returns>
        private static bool ConfirmClose(ProjectWorkspace workspace)
        {
            var project = workspace.Project;

            if (!project.HasUnsavedTranslations) return true;

            var answer = ThemedMessageBox.Show(
                string.Format(
                    CultureInfo.CurrentCulture,
                    T._("The project \"{0}\" has translations which its database file does not have yet."),
                    project.Name) + Environment.NewLine + Environment.NewLine + T._("Save them before closing the project?"),
                T._("Close project"),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            switch (answer)
            {
                case DialogResult.Yes:
                    return SaveDatabase(workspace);

                case DialogResult.No:
                    return true;

                default:
                    // Closing the box is not an answer, and neither is the Escape key: a close that was
                    // backed out of has to leave the project where it was rather than throw the work
                    // away because no button was pressed.
                    return false;
            }
        }

        /// <summary>
        /// Write the database of <paramref name="workspace"/> and report whether it was written.
        /// </summary>
        private static bool SaveDatabase(ProjectWorkspace workspace)
        {
            try
            {
                // Waited for rather than awaited: the caller is deciding whether to close the project,
                // and its answer has to be known before the tab that presents it is disposed. The save
                // itself does not come back to the UI thread, so waiting here cannot deadlock it.
                FunctionsDBFile.SaveDB(workspace).GetAwaiter().GetResult();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save the database of the project being closed");

                return false;
            }
        }

        /// <summary>
        /// Build the tab page of one project: its workspace, which is the panel and the opened files.
        /// </summary>
        private TabPage CreateProjectTabPage(ProjectBase project)
        {
            var workspace = new ProjectWorkspace(project);
            _workspaces[project] = workspace;

            return workspace.CreateProjectTabPage();
        }

        private void OnProjectsDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProjectsData.SelectedProject)) return;

            // Selecting a project has to show its tab as well as publish its workspace: the two are
            // kept in step here rather than by the control that raised the change.
            _projectsTabControl.SelectProject(_projectsData.SelectedProject);
            SyncActiveWorkspace();
            RefreshProjectMenus();
        }

        /// <summary>
        /// Publish the selected project's workspace as the application's active one, so the callers
        /// that ask for "the" files list or "the" grid follow the user's choice.
        /// </summary>
        private void SyncActiveWorkspace()
        {
            AppData.ActiveWorkspace = SelectedWorkspace;
        }

        /// <summary>
        /// Rebuild the menus that depend on the selected project.
        /// <para>
        /// The menu strips belong to the window and are shared by every open project, so the items
        /// that are the project's own — its extra file list and row entries — have to be replaced when
        /// the user moves to another project. Doing it here is what keeps them from describing the
        /// project that was selected before: the window is never told that the selection changed, it
        /// is told that the projects changed and this is what follows from that.
        /// </para>
        /// </summary>
        private void RefreshProjectMenus()
        {
            if (AppData.Main == null) return;

            FunctionsMenus.CreateMenus();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _projectsData.PropertyChanged -= OnProjectsDataChanged;

            foreach (var workspace in _workspaces.Values)
            {
                workspace.Clear();
            }

            _workspaces.Clear();
        }
    }
}
