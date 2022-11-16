using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.WPF.Models;

namespace TH.WPF.ViewModels
{
    public partial class MainVM
    {
        /// <summary>
        /// Menu File\Open clicked
        /// </summary>
        private RelayCommand? onOpen;
        public RelayCommand OnOpen => onOpen ??= new RelayCommand(obj => { Menus.Open(); });
    }
}
