using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{
    public abstract class MainMenuFileSubItemCaseMorphBase : MainMenuEditSubItemBase
    {
        public override string CategoryName => T._("Case morph");
    }
    
    public abstract class MainMenuFileSubItemCaseMorphToUpperFirst : MainMenuFileSubItemCaseMorphBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphUpperVariated() };
    }

    public abstract class MainMenuFileSubItemCaseMorphToUpperBase : MenuItemBase, IChildMenuItem
    {
    }

    internal class MenuItemCaseMorphUpperVariated : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("To Upper");

        public override string Description => T._("Change case of first char of rows to Upper");

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

    internal class MenuItemCaseMorphUpperAll : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("All");

        public override string Description => T._("Change case of first char of all rows to Upper");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new ToUpperCaseFirst().AllT();
        }
    }

    internal class MenuItemCaseMorphUpperTable : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("Table");

        public override string Description => T._("Change case of first char of each row in table to Upper");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new ToUpperCaseFirst().TableT();
        }
    }

    internal class MenuItemCaseMorphUpperSelected : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("Table");

        public override string Description => T._("Change case of first char of each row in table to Upper");

        public override void OnClick(object sender, EventArgs e)
        {
            _ = new ToUpperCaseFirst().Selected();
        }
    }
}
