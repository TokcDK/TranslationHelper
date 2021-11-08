using System;

namespace TranslationHelper.Menus.ProjectMenus
{

    public interface IMenu
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
        string Category { get; }

        /// <summary>
        /// Menu item action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClick(object sender, EventArgs e);
    }
}
