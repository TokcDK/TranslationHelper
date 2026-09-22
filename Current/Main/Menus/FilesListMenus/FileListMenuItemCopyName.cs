using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class FileListMenuItemCopyName : FileListMenuItemBase
    {
        public override string Text => T._("Copy name");

        public override string Description => T._("Copy name of selected tables");

        public override void OnClick(object sender, EventArgs e)
        {
            // The menu belongs to a project's list, so the names come from that project's list rather
            // than from a single application-wide one.
            Clipboard.SetText(AppData.ActiveWorkspace?.FilesList?.GetSelectedItemNames() ?? string.Empty);
        }
    }
}
