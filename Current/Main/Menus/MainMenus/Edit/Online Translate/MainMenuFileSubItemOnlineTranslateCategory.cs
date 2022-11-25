using System;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    public class MainMenuFileSubItemOnlineTranslateCategory : MainMenuEditSubItemBase, IFileRowMenuItem, IFileListMenuItem, IProjectMenuItem
    {
        public override string CategoryName => "";
        public override string Text => T._("Translate online");

        public override void OnClick(object sender, EventArgs e) { }

        public override int Order => base.Order - 500;
    }
}
