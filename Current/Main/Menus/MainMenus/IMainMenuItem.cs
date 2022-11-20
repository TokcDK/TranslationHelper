using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus
{
    public interface IMainMenuItem : IMenuItem
    {
        /// <summary>
        /// Parent Category in main menu like File, Edit, View,
        /// Set to null to place as one of main menu.
        /// </summary>
        string ParentMenuName { get; }
    }
}
