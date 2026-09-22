using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Models;
using TranslationHelper.Projects;

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
            // is how the map above stays complete.
            _projectsTabControl.Bind(_projectsData, CreateProjectTabPage);

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
