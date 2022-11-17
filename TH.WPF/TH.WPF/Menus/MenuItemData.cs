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
        public MenuItemData(string name, string description, ICommand? command)
        {
            Name = name;
            Description = description;
            Command = command;
        }

        public string Name { get; }
        public string Description { get; }
        public ICommand? Command { get; }

        public ObservableCollection<MenuItemData> Childs { get; set; } = new ObservableCollection<MenuItemData>();
    }
}
