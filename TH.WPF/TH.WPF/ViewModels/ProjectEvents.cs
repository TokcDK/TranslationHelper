using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TH.WPF.Core.Data.Project;
using TH.WPF.Models;
using static TH.WPF.ViewModels.MainVM;

namespace TH.WPF.ViewModels
{
    public partial class MainVM
    {
        private static RelayCommand? onProjectClose;
        /// <summary>
        /// When Project tab is closing, delete tab
        /// </summary>
        public static RelayCommand OnProjectClose => onProjectClose ??= new RelayCommand(obj => 
        {
            if (obj is not ProjectInfo p) return;

            // Make here dialog about close project to not close it accidentally

            Project.CloseProject(p); 
        });
    }

}
