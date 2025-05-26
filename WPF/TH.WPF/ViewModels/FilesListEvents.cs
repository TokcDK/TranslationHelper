using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TH.WPF.ViewModels
{
    public partial class MainVM
    {
        /// <summary>
        /// Files list item selected
        /// </summary>
        private RelayCommand? onSelected;
        public RelayCommand OnSelected => onSelected ??= new RelayCommand(obj => { });
    }
}
