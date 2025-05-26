using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TH.WPF.Menus
{
    public interface IMenu
    {
        /// <summary>
        /// Name of menu item
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description of the menu item. For using in tooltip for example.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Menu category if it is submenu
        /// </summary>
        string CategoryName { get; }
        /// <summary>
        /// Command for menu
        /// </summary>
        void Command();
    }
}
