using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{
    public class MainMenuFileSubItemFixJpMessagesTranslation : MainMenuEditSubItemBase, IFileListMenuItem, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("FixJpMessagesTranslation");
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphFixJpMessagesTranslationVariated() };

        public override void OnClick(object sender, EventArgs e) { }
    }


    internal class MenuItemCaseMorphFixJpMessagesTranslationVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("JP->EN");

        public override string Description => T._("Corrects japanese state message translation by static translations list");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new FixJpMessagesTranslation().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
            new FixJpMessagesTranslation().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new FixJpMessagesTranslation().TableT();
        }
    }
}
