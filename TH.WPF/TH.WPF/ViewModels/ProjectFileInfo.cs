using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace TH.WPF.ViewModels
{
    /// <summary>
    /// Info about opened file
    /// </summary>
    public class ProjectFileInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private List<object> selectedItems = new();
        public List<object> SelectedItems
        {
            get { return selectedItems; }
            set
            {
                if (selectedItems == value) return;

                selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        public FileInfo? File { get; set; }
        public string Info { get; set; } = "";
        public FileContent? Content { get; set; }

        /// <summary>
        /// Rows list. Selected row value
        /// </summary>
        static FileRow? selectedRow;
        public FileRow? SelectedRow
        {
            get
            {
                return selectedRow;
            }
            set
            {
                selectedRow = value;
                OnPropertyChanged(nameof(SelectedRow));
            }
        }
    }

    /// <summary>
    /// File's content. Rows
    /// </summary>
    public class FileContent : ObservableCollection<FileRow> { }

    /// <summary>
    /// File's row nofo
    /// </summary>
    public class FileRow
    {
        public string? Original { get; set; }
        public string? Translation { get; set; }
        public string Info { get; set; } = "";
    }
}
