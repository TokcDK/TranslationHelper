using System;
using System.Windows.Forms;

namespace TranslationHelper.Menus
{
    /// <summary>
    /// Menu item. See also: <seealso cref="IProjectMenuItem"/>, <seealso cref="IProjectSpecifiedMenuItem"/>
    /// </summary>
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
        /// Child menu items. See also <seealso cref="IChildMenuItem"/> description
        /// </summary>
        IMenuItem[] Childs { get; }

        /// <summary>
        /// Priority of the menu
        /// </summary>
        int Order { get; set; }
    }

    /// <summary>
    /// Determine that the menu is child menu and will be loaded from child menu list of parent category menu
    /// </summary>
    public interface IChildMenuItem : IMenuItem
    {
        // detect if not need to add the menu in menus list because it is child menu
    }
    /// <summary>
    /// Determine default menu for categories to ignore it while menu search
    /// </summary>
    public interface IDefaultMenuItem : IMenuItem
    {
        // detect if not need to add the menu in menus list because it is child menu
    }

    /// <summary>
    /// Determine to load the menu while project is not opened
    /// </summary>
    public interface IProjectMenuItem : IMenuItem
    {
        // detect if not need to add the menu in menus list because it is child menu
    }

    /// <summary>
    /// Determine to load this menu only from project
    /// </summary>
    public interface IProjectSpecifiedMenuItem : IMenuItem
    {
        // detect if not need to add the menu in menus list because it is child menu
    }
}
