using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TH.WPF.Core.Data.Project
{
    /// <summary>
    /// ProjectInfo
    /// </summary>
    public class ProjectInfo : ObservableObject
    {

        public string? Name { get; set; }

        public ObservableCollection<ProjectFileInfo>? Files { get; set; }

        /// <summary>
        /// Files list. Selected file value
        /// </summary>
        static ProjectFileInfo? selectedFile;
        public ProjectFileInfo? SelectedFile
        {
            get
            {
                return selectedFile;
            }
            set
            {
                selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }
    }

    /// <summary>
    /// ProjectInfo
    /// </summary>
    public class ProjectsListInfo : ObservableObject
    {

        public ObservableCollection<ProjectInfo>? Projects { get; set; }

        static ProjectInfo? selectedProject;
        public ProjectInfo? SelectedProject
        {
            get
            {
                return selectedProject;
            }
            set
            {
                selectedProject = value;
                OnPropertyChanged(nameof(SelectedProject));
            }
        }
    }
}
