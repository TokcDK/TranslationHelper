using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TranslationHelper.Projects;

namespace TranslationHelper.Models
{
    /// <summary>
    /// Every project that is open in this session, and which of them is being worked on.
    /// <para>
    /// This is what the application used to keep as a single <c>CurrentProject</c> reference. It is a
    /// list now, so a second project can be opened without the first one being closed, and the
    /// selection is what tells the rest of the application which project a menu item, a save or a row
    /// operation has to act on.
    /// </para>
    /// <para>
    /// The list is a <see cref="BindingList{T}"/> so the projects tab control can follow it: adding a
    /// project is what creates its tab, and there is no second place that has to be told about it.
    /// </para>
    /// <para>
    /// A note on layering: this type names <see cref="ProjectBase"/>, which lives in the project
    /// layer, and <see cref="ProjectBase"/> names <see cref="OpenedFilesData"/> from this one. That is
    /// one edge rather than two, and it is deliberate — the two are the same concept seen from the
    /// collection's side and the project's side. When the application is split into assemblies this
    /// edge is what has to be cut first, by moving this type into the project layer or by reducing
    /// <see cref="ProjectBase"/> to the contract the collection needs.
    /// </para>
    /// </summary>
    public class ProjectsData : ObservableObject
    {
        /// <summary>
        /// The open projects, in the order they were opened.
        /// </summary>
        public BindingList<ProjectBase> ProjectsList { get; } = new BindingList<ProjectBase>();

        private ProjectBase _selectedProject;

        /// <summary>
        /// The project being worked on: the selected tab of the projects tab control, and the project
        /// every operation that does not name one acts on.
        /// </summary>
        public ProjectBase SelectedProject
        {
            get => _selectedProject;
            set => SetProperty(ref _selectedProject, value);
        }

        /// <summary>
        /// The project opened from <paramref name="projectPath"/>, or null when none is.
        /// <para>
        /// Opening the same path twice selects the project that is already open rather than parsing it
        /// again, which is what a caller checking before it opens relies on.
        /// </para>
        /// </summary>
        public ProjectBase Find(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath)) return null;

            return ProjectsList.FirstOrDefault(project => project.ProjectPath == projectPath);
        }

        /// <summary>
        /// True when the project at <paramref name="projectPath"/> is already open.
        /// </summary>
        public bool Contains(string projectPath) => Find(projectPath) != null;

        /// <summary>
        /// Add <paramref name="project"/> to the end of the list and return it, so the caller can
        /// select it without looking it up again.
        /// </summary>
        public ProjectBase Add(ProjectBase project)
        {
            ProjectsList.Add(project);
            return project;
        }

        /// <summary>
        /// Close <paramref name="project"/>: drop it from the list and, when it was the selected one,
        /// select the project that takes its place — the one that followed it, or the one before it.
        /// </summary>
        /// <returns>True when the project was open and has been removed.</returns>
        public bool Remove(ProjectBase project)
        {
            if (project == null) return false;

            int index = ProjectsList.IndexOf(project);
            if (index < 0) return false;

            ProjectsList.RemoveAt(index);

            if (SelectedProject != project) return true;

            // The tab that was selected is gone, so the selection has to move to a tab that exists.
            // The project that followed it keeps the position, which is what a tab control does when a
            // tab is closed; when it was the last one, the new last one is selected instead.
            SelectedProject = ProjectsList.Count == 0
                ? null
                : ProjectsList[index < ProjectsList.Count ? index : ProjectsList.Count - 1];

            return true;
        }

        /// <summary>
        /// Drop every project and the selection.
        /// </summary>
        public void Clear()
        {
            SelectedProject = null;
            ProjectsList.Clear();
        }

        /// <summary>
        /// The paths of the open projects, in list order.
        /// </summary>
        public IEnumerable<string> ProjectPaths => ProjectsList.Select(project => project.ProjectPath);
    }
}
