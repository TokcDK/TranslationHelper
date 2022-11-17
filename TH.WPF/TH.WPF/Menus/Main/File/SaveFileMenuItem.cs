﻿using System;
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
    internal class SaveFileMenuItem : IMainMenuItem
    {
        public string ParentMenuName => "File";

        public string CategoryName => "";

        public string Name => "Write";

        public string Description => "Write project translation";

        private RelayCommand? onOpen;
        public ICommand Command => onOpen ??= new RelayCommand(obj => { });
    }
}
