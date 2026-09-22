using System;
using TranslationHelper.Data;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemResetColumnSorting : MainMenuViewSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Reset column sorting");

        public override string Description => Text;

        public override void OnClick(object sender, EventArgs e)
        {
            // The entry being shown owns its table, so the sorting is reset on what the user is looking
            // at rather than on whatever index the files list of some project happens to hold.
            var table = AppData.ActiveWorkspace?.OpenedFilesData?.SelectedOpenedFileData?.Table;
            if (table == null) return;

            table.DefaultView.Sort = string.Empty;
        }
    }
}
