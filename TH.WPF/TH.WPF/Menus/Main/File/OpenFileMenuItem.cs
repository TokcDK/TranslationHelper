using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TH.WPF.Models;
using static TH.WPF.ViewModels.MainVM;

namespace TH.WPF.Menus.Main.File
{
    internal class OpenFileMenuItem : IMainMenuItem
    {
        public string ParentMenuName => "File";

        public string CategoryName => "Project";

        public string Name => "Open";

        public string Description => "Open project file";

        private RelayCommand? onOpen;
        public ICommand Command => onOpen ??= new RelayCommand(obj => { MenuCommands.Open(); });
    }
}
