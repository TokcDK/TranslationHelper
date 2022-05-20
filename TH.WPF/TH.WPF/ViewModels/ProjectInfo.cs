using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TH.WPF.Models;
using static TH.WPF.ViewModels.MainVM;

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

        private RelayCommand? onProjectClose;
        public RelayCommand OnProjectClose => onProjectClose ??= new RelayCommand(obj => { ModelProject.CloseProject(obj as ProjectInfo); });
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
