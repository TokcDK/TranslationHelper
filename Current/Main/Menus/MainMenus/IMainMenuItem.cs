using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Menus.MainMenus
{
    /// <summary>
    /// Determine to use the menu item as one of main menus or submenus
    /// </summary>
    public interface IMainMenuItem : IMenuItem
    {
        /// <summary>
        /// Parent Category in main menu like File, Edit, View,
        /// Set to null to place as one of main menu.
        /// </summary>
        string ParentMenuName { get; }
    }
}
