using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{    
    public abstract class MainMenuFileSubItemFixJpMessagesTranslation : MainMenuEditSubItemBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphFixJpMessagesTranslationVariated() };
    }

    public abstract class MainMenuFileSubItemFixJpMessagesTranslationBase : MenuItemBase, IChildMenuItem
    {
    }

    internal class MenuItemCaseMorphFixJpMessagesTranslationVariated : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("FixJpMessagesTranslation");

        public override string Description => T._("FixJpMessagesTranslation");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count)
            {
                _ = new FixJpMessagesTranslation().AllT();
            }
            else if (AppData.Main.THFileElementsDataGridView.Rows.Count == AppData.Main.THFileElementsDataGridView.SelectedRows.Count)
            {
                _ = new FixJpMessagesTranslation().TableT();
            }
            else
            {
                _ = new FixJpMessagesTranslation().Selected();
            }
        }
    }
}
