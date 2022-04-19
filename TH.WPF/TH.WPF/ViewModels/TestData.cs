using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.WPF.ViewModels;

namespace TH.WPF.ViewModels
{
    public class TestData
    {
        /// <summary>
        /// Test file data
        /// </summary>
        /// <returns></returns>
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

        public static ProjectInfo TestProject { get; } = new ProjectInfo()
        {
            Files = new ObservableCollection<ProjectFileInfo>() { TestFile }
        };

        public static ProjectsListInfo TestProjects { get; } = new ProjectsListInfo()
        {
            Projects = new ObservableCollection<ProjectInfo>()
            {
                TestProject
            }
            , 
            SelectedProject = TestProject
        };
    }
}
