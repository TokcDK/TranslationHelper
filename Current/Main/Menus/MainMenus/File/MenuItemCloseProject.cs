using System;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Menus.MainMenus.File
{
    /// <summary>
    /// Closes the project being worked on: its tab, its files and the controls that show them.
    /// <para>
    /// It asks the workspace to close the project rather than doing it, because closing may have to
    /// offer to save the project's database first, and because the tab and the project's files have to
    /// go together. This command is the keyboard's way to the same thing the tab's close button does.
    /// </para>
    /// </summary>
    internal class MenuItemCloseProject : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Close project");

        public override string Description => T._("Close the project being worked on");

        public override void OnClick(object sender, EventArgs e)
        {
            var project = AppData.CurrentProject;
            if (project == null) return;

            AppData.Main?.Workspace?.Close(project);
        }

        public override int Order => base.Order + 14;

        public override Keys ShortcutKeys => Keys.Control | Keys.W;
    }
}
