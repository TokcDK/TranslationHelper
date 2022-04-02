using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TH.WPF.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        static FileInfo TestFile()
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

        public class FileInfo
        {
            public string? Name { get; set; }
            public string Info { get; set; } = "";
            public FileContent? Content { get; set; }
        }

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

        public class FileContent : ObservableCollection<FileRow>
        {
        }

        public class FileRow
        {
            public string? Original { get; set; }
            public string? Translation { get; set; }
            public string Info { get; set; } = "";
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object?> execute;
            private readonly Func<object?, bool>? canExecute;

            public event EventHandler? CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                return canExecute == null || canExecute(parameter!);
            }

            public void Execute(object? parameter)
            {
                execute(parameter);
            }
        }
    }
}
