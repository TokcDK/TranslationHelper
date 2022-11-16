using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.WPF.Menus;

namespace TH.WPF.ViewModels
{
    public class MenuItemVM
    {
        public MenuItemVM(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }

        public ObservableCollection<IMenu> Childs { get; set; } = new ObservableCollection<IMenu>();
    }
}
