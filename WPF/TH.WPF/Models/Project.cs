using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.WPF.Core.Data.Project;
using TH.WPF.ViewModels;

namespace TH.WPF.Models
{
    internal class Project
    {
        public static void CloseProject(ProjectInfo project)
        {
            if (project == null) return;
            if (MainVM.ProjectsList.Projects == null || MainVM.ProjectsList.Projects.Count==0) return;

            MainVM.ProjectsList.Projects.Remove(project);
        }
    }
}
