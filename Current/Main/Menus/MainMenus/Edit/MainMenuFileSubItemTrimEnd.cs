using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    public class MainMenuFileSubItemTrimEnd : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override IMenuItem[] Childs => new IMenuItem[1] 
        { 
            new MenuItemCaseMorphTrimEndVariated() 
        };

        public override string Text => T._("Trim end");

        public override void OnClick(object sender, EventArgs e)
        {
        }
    }

    internal class MenuItemCaseMorphTrimEndVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("Trim end");

        public override string Description => T._("Trim end spaces of rows");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new TrimEndSpace().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
            new TrimEndSpace().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new TrimEndSpace().TableT();
        }
    }
}
