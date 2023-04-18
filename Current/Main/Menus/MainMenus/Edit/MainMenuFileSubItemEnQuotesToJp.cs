using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{
    public class MainMenuFileSubItemEnQuotesToJp : MainMenuEditSubItemBase, IFileListMenuItem, IFileRowMenuItem, IProjectMenuItem
    {
        public override IMenuItem[] Childs => new IMenuItem[2] 
        { 
            new MenuItemCaseMorphEnQuotesToJpVariated(),
            new MenuItemCaseMorphEnQuotesToJpVariated2(),
        };

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

    internal class MenuItemCaseMorphEnQuotesToJpVariated2 : AllTableRowsChildMenuBase
    {
        public override string Text => T._("Run2");

        public override string Description => T._("EnQuotesToJp2");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new EnQuotesToJp2().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
            new EnQuotesToJp2().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new EnQuotesToJp2().TableT();
        }
    }
}
