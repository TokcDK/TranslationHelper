using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{
    public class MainMenuFileSubItemEnQuotesToJp : MainMenuEditSubItemBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphEnQuotesToJpVariated() };

        public override string Text => T._("En Quotes To Jp");

        public override void OnClick(object sender, EventArgs e)
        {
        }
    }


    internal class MenuItemCaseMorphEnQuotesToJpVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("Run");

        public override string Description => T._("EnQuotesToJp");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new EnQuotesToJp().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
            new EnQuotesToJp().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new EnQuotesToJp().TableT();
        }
    }
}
