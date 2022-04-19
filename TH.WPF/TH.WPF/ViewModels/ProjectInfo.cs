using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TH.WPF.ViewModels
{
    /// <summary>
    /// ProjectInfo
    /// </summary>
    public class ProjectInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

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
    public class ProjectsListInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

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
