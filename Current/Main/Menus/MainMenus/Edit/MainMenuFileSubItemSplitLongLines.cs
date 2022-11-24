using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{    
    public abstract class MainMenuFileSubItemSplitLongLines : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphSplitLongLinesVariated() };
    }

    public abstract class MainMenuFileSubItemSplitLongLinesBase : MenuItemBase, IChildMenuItem
    {
    }

    internal class MenuItemCaseMorphSplitLongLinesVariated : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("Split Long Lines");

        public override string Description => T._("Split row text lines if text is length is longer of linit");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count)
            {
                _ = new SplitLongLines().AllT();
            }
            else if (AppData.Main.THFileElementsDataGridView.Rows.Count == AppData.Main.THFileElementsDataGridView.SelectedRows.Count)
            {
                _ = new SplitLongLines().TableT();
            }
            else
            {
                _ = new SplitLongLines().Selected();
            }
        }
    }
}
