using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.CellFixes
{
    public class MainMenuFileSubItemCellFixesBase : MainMenuEditSubItemBase, IFileListMenuItem, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Cell Fixes");

        public override void OnClick(object sender, EventArgs e) { }
        public override IMenuItem[] Childs => new IMenuItem[2]
        {
            new MenuItemCaseCellFixesVariated(),
            new MenuItemCaseCellFixesVariatedForce()
        };
    }

    internal class MenuItemCaseCellFixesVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("Cell Fixes");

        public override string Description => T._("Fix cells by regex rules");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new FixCells().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
             new FixCells().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new FixCells().TableT();
        }
    }

    internal class MenuItemCaseCellFixesVariatedForce : AllTableRowsChildMenuBase
    {
        public override string Text => T._("Cell Fixes (Force)");

        public override string Description => T._("Force fix cells by regex rules");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new FixCellsForce().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
            new FixCellsForce().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new FixCellsForce().TableT();
        }
    }
}
