using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TH.WPF.ViewModel
{
    public class VM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public static ObservableCollection<FileInfo> FilesList { get; set; } = new ObservableCollection<FileInfo>()
        {
            new FileInfo() {Name="test file1"},
        };

        public static FileInfo? GetSelectedFile() => _selectedItem;

        static FileInfo? _selectedItem;

        public FileInfo? SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public class FileInfo
        {
            public string? Name { get; set; }
        }

        public static ObservableCollection<FileRow> FileContent { get; set; } = new ObservableCollection<FileRow>()
        {
            new FileRow(){Original="test o1", Translation="test t1"},
            new FileRow(){Original="test o2", Translation="test t2"},
            new FileRow(){Original="test o3", Translation="test t3"},
        };

        public class FileRow
        {
            public string? Original { get; set; }
            public string? Translation { get; set; }
        }
    }
}
