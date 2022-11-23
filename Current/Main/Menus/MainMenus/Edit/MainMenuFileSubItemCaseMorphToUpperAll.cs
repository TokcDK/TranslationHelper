using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{    
    public abstract class MainMenuFileSubItemCaseMorphToUPPERAll : MainMenuFileSubItemCaseMorphBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphToUPPERAllVariated() };
    }

    public abstract class MainMenuFileSubItemCaseMorphToUPPERAllBase : IMenuItem, IChildMenuItem
    {
        public abstract string Text { get; }

        public virtual string Description { get; } = "";
        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);

        public virtual IMenuItem[] Childs { get; } = null;
    }

    internal class MenuItemCaseMorphToUPPERAllVariated  : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("To UPPER");

        public override string Description => T._("Change case of first char of rows to Upper");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count)
            {
                _ = new ToUpperCaseAll().AllT();
            }
            else if (AppData.Main.THFileElementsDataGridView.Rows.Count == AppData.Main.THFileElementsDataGridView.SelectedRows.Count)
            {
                _ = new ToUpperCaseAll().TableT();
            }
            else
            {
                _ = new ToUpperCaseAll().Selected();
            }
        }
    }
}
