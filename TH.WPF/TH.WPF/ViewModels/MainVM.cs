using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TH.WPF.ViewModels
{
    public partial class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Test file data
        /// </summary>
        /// <returns></returns>
        public static FileInfo TestFile()
        {
            return new FileInfo() { Name = "test file1", Info = "some file info\ndfgdfgdfg\vvvvvvvv\ninfo\r\ninfoinfo\nIfileinfo",
            Content = new FileContent()
            {
                new FileRow(){Original="test o1", Translation="test t1", Info="some row info"},
                new FileRow(){Original="test o2", Translation="test t2"},
                new FileRow(){Original="test o3", Translation="test t3", Info="some row info 3\nsdfsdfds\nhhhhh\nddfffdfdf\n"},
            }};
        }

        public static ObservableCollection<FileInfo> FilesList { get; set; } = new ObservableCollection<FileInfo>()
        {
            TestFile()
        };

        public static FileInfo? GetSelectedFile() => _selectedFile;

        /// <summary>
        /// Files list. Selected file value
        /// </summary>
        static FileInfo? _selectedFile;
        public FileInfo? SelectedFile
        {
            get
            {
                return _selectedFile;
            }
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Rows list. Selected row value
        /// </summary>
        static FileRow? _selectedRow;
        public FileRow? SelectedRow
        {
            get
            {
                return _selectedRow;
            }
            set
            {
                _selectedRow = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Info about opened file
        /// </summary>
        public class FileInfo
        {
            public string? Name { get; set; }
            public string Info { get; set; } = "";
            public FileContent? Content { get; set; }
        }

        /// <summary>
        /// Files list item selected
        /// </summary>
        private RelayCommand? onSelected;
        public RelayCommand OnSelected
        {
            get
            {
                return onSelected ??= new RelayCommand(obj =>
                {
                });
            }
        }

        /// <summary>
        /// Menu File\Open clicked
        /// </summary>
        private RelayCommand? onOpen;
        public RelayCommand OnOpen
        {
            get
            {
                return onOpen ??= new RelayCommand(obj =>
                {
                });
            }
        }

        /// <summary>
        /// File's content. Rows
        /// </summary>
        public class FileContent : ObservableCollection<FileRow>
        {
        }

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
}
