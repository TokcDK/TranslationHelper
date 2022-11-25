using System;
using System.Windows.Forms;

namespace TranslationHelper.Menus
{
    public interface IMenuItem
    {
        /// <summary>
        /// Menu item text
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Menu item description text
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Submenu list name where it must be placed.
        /// Set to null to place as one of main menu.
        /// </summary>
        string CategoryName { get; }

        /// <summary>s
        /// Menu item action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClick(object sender, EventArgs e);

        /// <summary>
        /// Shortcut keys to activate the menu
        /// </summary>
        Keys ShortcutKeys { get; }

        /// <summary>
        /// Child menu items
        /// </summary>
        IMenuItem[] Childs { get; }

        /// <summary>
        /// Priority of the menu
        /// </summary>
        int Order { get; set; }
    }
    public interface IChildMenuItem : IMenuItem
    {
        // detect if not need to add the menu in menus list because it is child menu
    }
    public interface IDefaultMenuItem : IMenuItem
    {
        // detect if not need to add the menu in menus list because it is child menu
    }
    public interface IProjectMenuItem : IMenuItem
    {
        // detect if not need to add the menu in menus list because it is child menu
    }
}
