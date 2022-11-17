using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using TH.WPF.Menus;
using TH.WPF.Models;

namespace TH.WPF.ViewModels
{
    public partial class MainVM
    {
        public static ProjectsListInfo ProjectsList { get; set; } = TestData.TestProjects;

        public static List<MenuItemData> MainMenusList { get; set; } = new List<MenuItemData>() { new MenuItemData("File", "") { Childs = new ObservableCollection<IMenu> { new OpenFileMenuItem() } } };
    }
}
