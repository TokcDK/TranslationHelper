using System;

namespace Menus
{

    public interface IMenu
    {
        /// <summary>
        /// Menu item text
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Menu item tooltip text
        /// </summary>
        string ToolTipText { get; }

        /// <summary>
        /// Main menu(like File, Edit, View) name where it must be placed.
        /// Set to null to place as one of main menu.
        /// </summary>
        string Parent { get; }

        /// <summary>
        /// Category submenu name, where it must be placed in droplist.
        /// Set to null if no category submenu
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Menu item action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClick(object sender, EventArgs e);
    }
}
