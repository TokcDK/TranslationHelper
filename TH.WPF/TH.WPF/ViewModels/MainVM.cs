using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace TH.WPF.ViewModels
{
    public partial class MainVM
    {
        public ProjectsListInfo ProjectsList { get; set; } = TestData.TestProjects;
    }
}
