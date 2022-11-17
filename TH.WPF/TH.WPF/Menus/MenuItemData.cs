using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TH.WPF.Menus;

namespace TH.WPF.Menus
{
    public class MenuItemData
    {
        public MenuItemData(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public ICommand? Command { get; set; }

        public ObservableCollection<MenuItemData> Childs { get; set; } = new ObservableCollection<MenuItemData>();
    }
}
