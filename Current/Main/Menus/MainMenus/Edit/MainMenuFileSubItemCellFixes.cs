using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.Edit.CellFixes
{
    public abstract class MainMenuFileSubItemCellFixesBase : MainMenuEditSubItemBase
    {
        public override string CategoryName => T._("CellFixes");
    }

    public abstract class MainMenuFileSubItemCellFixes : MainMenuFileSubItemCellFixesBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseCellFixesVariated() };
    }

    public abstract class MainMenuFileSubItemCellFixesSubBase : IMenuItem, IChildMenuItem
    {
        public abstract string Text { get; }

        public virtual string Description { get; } = "";

        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);

        public virtual IMenuItem[] Childs { get; } = null;
    }

    internal class MenuItemCaseCellFixesVariated : MainMenuFileSubItemCellFixesSubBase
    {
        public override string Text => T._("Cell Fixes");

        public override string Description => T._("Fix cells by regex rules");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count)
            {
                _ = new ToUpperCaseFirst().AllT();
            }
            else if (AppData.Main.THFileElementsDataGridView.Rows.Count == AppData.Main.THFileElementsDataGridView.SelectedRows.Count)
            {
                _ = new ToUpperCaseFirst().TableT();
            }
            else
            {
                _ = new ToUpperCaseFirst().Selected();
            }
        }
    }

    internal class MenuItemCellFixesAll : MainMenuFileSubItemCellFixesSubBase
    {
        public override string Text => T._("All");

        public override string Description => T._("Cell Fixes for all");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new FixCells().AllT();
        }
    }

    internal class MenuItemCellFixesTable : MainMenuFileSubItemCellFixesSubBase
    {
        public override string Text => T._("Table");

        public override string Description => T._("Cell Fixes for selected tables");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new FixCells().TableT();
        }
    }

    internal class MenuItemCellFixesSelected : MainMenuFileSubItemCellFixesSubBase
    {
        public override string Text => T._("Row");

        public override string Description => T._("Cell Fixes for selected rows");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new FixCells().Selected();
        }
    }
}
