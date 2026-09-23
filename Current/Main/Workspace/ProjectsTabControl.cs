using System;
using System.Windows.Forms;
using TranslationHelper.Models;
using TranslationHelper.Projects;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// Every open project, one tab each.
    /// <para>
    /// The tabs are the entries of <see cref="ProjectsData.ProjectsList"/>, and the selected tab is
    /// <see cref="ProjectsData.SelectedProject"/>. That is what makes opening a project a single
    /// action — add it to the list — rather than an action and a separate one that creates its tab.
    /// </para>
    /// <para>
    /// The class does not know what a project's tab contains: the factory that builds a tab page is
    /// supplied by the caller, which is what keeps the composition of a project's workspace out of the
    /// control that presents it.
    /// </para>
    /// <para>
    /// A tab carries a close button, and clicking it is reported to the caller as a request rather than
    /// carried out here. Closing a project is not a change to the strip: it may have to save the
    /// project's database first, and it may be refused, in which case the tab has to stay where it is.
    /// So the strip asks, and the list the tabs present is what removes the tab.
    /// </para>
    /// </summary>
    internal partial class ProjectsTabControl : UserControl
    {
        private TabControlBinder<ProjectBase> _binder;

        /// <summary>
        /// Asked to close the project a tab presents, and answers whether it was closed. Supplied by
        /// the caller that owns the projects, because closing one is its business: it may have to offer
        /// to save the project's database first, and it is the list — not this control — that owns the
        /// tab pages.
        /// </summary>
        private Func<ProjectBase, bool> _closeProject;

        /// <summary>
        /// The project the selected tab presents, or null when no tab is selected.
        /// </summary>
        internal ProjectBase SelectedProject => _binder?.SelectedItem;

        internal ProjectsTabControl()
        {
            InitializeComponent();

            ProjectsTabs.TabCloseRequested += OnTabCloseRequested;
        }

        /// <summary>
        /// Show one tab per entry of <paramref name="projectsData"/>, now and for every later change of
        /// the list. Selecting a tab sets <see cref="ProjectsData.SelectedProject"/>.
        /// </summary>
        /// <param name="projectsData">The open projects.</param>
        /// <param name="createTabPage">Builds the page for one project.</param>
        /// <param name="closeProject">
        /// Closes the project a tab presents when the user closes it. Answers whether the project was
        /// closed; false leaves its tab where it is.
        /// </param>
        internal void Bind(ProjectsData projectsData, Func<ProjectBase, TabPage> createTabPage, Func<ProjectBase, bool> closeProject)
        {
            if (projectsData == null) throw new ArgumentNullException(nameof(projectsData));
            if (createTabPage == null) throw new ArgumentNullException(nameof(createTabPage));

            _closeProject = closeProject;

            _binder?.Dispose();

            _binder = new TabControlBinder<ProjectBase>(ProjectsTabs, projectsData.ProjectsList, createTabPage);
            _binder.SelectionChanged += project => projectsData.SelectedProject = project;

            _binder.Rebuild();
        }

        /// <summary>
        /// Select the tab presenting <paramref name="project"/>, without reporting it back as a change
        /// the user made.
        /// </summary>
        internal void SelectProject(ProjectBase project)
        {
            _binder?.SelectItem(project);
        }

        /// <summary>
        /// Ask for the project of a tab to be closed.
        /// <para>
        /// The project is read from the list the tabs present and not from the page that was clicked:
        /// the list owns both, so it is the only thing that can say which project a tab stands for. The
        /// tab is not removed here — see <see cref="ClosableTabControl"/> — so a refused close leaves
        /// the strip exactly as it was.
        /// </para>
        /// </summary>
        private void OnTabCloseRequested(int index)
        {
            var project = _binder?.ItemAt(index);
            if (project == null) return;

            _closeProject?.Invoke(project);
        }
    }
}
