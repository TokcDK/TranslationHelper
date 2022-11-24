using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{    
    public abstract class MainMenuFileSubItemTrimEnd : MainMenuEditSubItemBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphTrimEndVariated() };
    }

    public abstract class MainMenuFileSubItemTrimEndBase : MenuItemBase, IChildMenuItem
    {
    }

    internal class MenuItemCaseMorphTrimEndVariated : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("Trim end");

        public override string Description => T._("Trim end spaces of rows");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count)
            {
                _ = new TrimEndSpace().AllT();
            }
            else if (AppData.Main.THFileElementsDataGridView.Rows.Count == AppData.Main.THFileElementsDataGridView.SelectedRows.Count)
            {
                _ = new TrimEndSpace().TableT();
            }
            else
            {
                _ = new TrimEndSpace().Selected();
            }
        }
    }
}
