using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.WPF.Menus;

namespace TH.WPF.Examples
{
    internal static class MainMenuExample
    {
        public static ObservableCollection<MenuItemData> TestMenus
        {
            get => new()
            {
                new("File", "")
                {
                    Childs = new ObservableCollection<MenuItemData> { TestMenu }
                }
            };
        }
        static MenuItemData TestMenu
        {
            get
            {
                var _testOpenMenu = new Menus.Main.File.OpenFileMenuItem();
                return new MenuItemData(_testOpenMenu.Name, _testOpenMenu.Description);
            }
        }
    }
}
