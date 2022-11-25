using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{
    public class MainMenuFileSubItemSplitLongLines : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphSplitLongLinesVariated() };

        public override string Text => T._("Split Long Lines");

        public override void OnClick(object sender, EventArgs e)
        {
        }
    }


    internal class MenuItemCaseMorphSplitLongLinesVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("Split Long Lines");

        public override string Description => T._("Split row text lines if text is length is longer of linit");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new SplitLongLines().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
            new SplitLongLines().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new SplitLongLines().TableT();
        }
    }
}
