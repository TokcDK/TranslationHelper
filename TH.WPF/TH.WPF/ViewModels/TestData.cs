using System.Collections.ObjectModel;
using System.IO;

namespace TH.WPF.ViewModels
{
    public class TestData
    {
        public static ProjectFileInfo TestFile { get; } = new ProjectFileInfo()
        {
            File = new FileInfo(@"c:\file1.txt"),
            Info = "some file info\ndfgdfgdfg\vvvvvvvv\ninfo\r\ninfoinfo\nIfileinfo",
            Content = new FileContent()
            {
                new FileRow(){Original="test o1", Translation="test t1", Info="some row info"},
                new FileRow(){Original="test o2", Translation="test t2"},
                new FileRow(){Original="test o3", Translation="test t3", Info="some row info 3\nsdfsdfds\nhhhhh\nddfffdfdf\n"},
            }
        };

        public static ProjectFileInfo TestFile2 { get; } = new ProjectFileInfo()
        {
            File = new FileInfo(@"c:\file2.txt"),
            Info = "some file info22222\ndfgdfgdfg\vvvvvvvv\ninfo\r\ninfoinfo\nIfileinfo",
            Content = new FileContent()
            {
                new FileRow(){Original="test2 o1", Translation="test2 t1", Info="some row info"},
                new FileRow(){Original="test2 o2", Translation="test2 t2"},
                new FileRow(){Original="test2 o3", Translation="test2 t3", Info="some row info 3\nsdfsdfds\nhhhhh\nddfffdfdf\n"},
            }
        };

        public static ProjectInfo TestProject { get; } = new ProjectInfo()
        {
            Name = "TestProject1",
            Files = new ObservableCollection<ProjectFileInfo>() { TestFile }
        };

        public static ProjectInfo TestProject2 { get; } = new ProjectInfo()
        {
            Name = "TestProject2",
            Files = new ObservableCollection<ProjectFileInfo>() { TestFile2 }
        };

        public static ProjectsListInfo TestProjects { get; } = new ProjectsListInfo()
        {
            Projects = new ObservableCollection<ProjectInfo>()
            {
                TestProject,
                TestProject2
            }
        };
    }
}
