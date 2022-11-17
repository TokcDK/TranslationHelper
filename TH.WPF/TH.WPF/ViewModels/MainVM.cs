using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using TH.WPF.Examples;
using TH.WPF.Examples;
using TH.WPF.Menus;
using TH.WPF.Menus.Main;

namespace TH.WPF.ViewModels
{
    public partial class MainVM
    {
        static bool _init = false;
        static ProjectsListInfo _projectsList = ProjectExamples.TestProjects;
        public static ProjectsListInfo ProjectsList 
        {
            get
            {
                if (!_init)
                {
                    _init = true;
                    Loader.LoadMainMenus(MainMenusList);
                }
                return _projectsList;
            }
            set
            {
                _projectsList = value;
            }
        }

        public static ObservableCollection<MenuItemData> MainMenusList { get; set; } = MainMenuExample.TestMenus;
    }
}
