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
    /// </summary>
    internal partial class ProjectsTabControl : UserControl
    {
        private TabControlBinder<ProjectBase> _binder;

        /// <summary>
        /// The tab control itself. Exposed because a caller that closes a project has to be able to
        /// reach the page it was shown in.
        /// </summary>
        internal TabControl Tabs => ProjectsTabs;

        /// <summary>
        /// The project the selected tab presents, or null when no tab is selected.
        /// </summary>
        internal ProjectBase SelectedProject => _binder?.SelectedItem;

        internal ProjectsTabControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show one tab per entry of <paramref name="projectsData"/>, now and for every later change of
        /// the list. Selecting a tab sets <see cref="ProjectsData.SelectedProject"/>.
        /// </summary>
        /// <param name="projectsData">The open projects.</param>
        /// <param name="createTabPage">Builds the page for one project.</param>
        internal void Bind(ProjectsData projectsData, Func<ProjectBase, TabPage> createTabPage)
        {
            if (projectsData == null) throw new ArgumentNullException(nameof(projectsData));
            if (createTabPage == null) throw new ArgumentNullException(nameof(createTabPage));

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
    }
}
