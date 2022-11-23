using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{
    public abstract class MainMenuFileSubItemCaseMorphTolower : MainMenuFileSubItemCaseMorphBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphLowerVariated() };
    }

    public abstract class MainMenuFileSubItemCaseMorphTolowerBase : IMenuItem, IChildMenuItem
    {
        public abstract string Text { get; }

        public virtual string Description { get; } = "";

        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);

        public virtual IMenuItem[] Childs { get; } = null;
    }

    internal class MenuItemCaseMorphLowerVariated : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("To lower");

        public override string Description => T._("Change case of rows chars to lower");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count)
            {
                _ = new ToLowerCaseAll().AllT();
            }
            else if (AppData.Main.THFileElementsDataGridView.Rows.Count == AppData.Main.THFileElementsDataGridView.SelectedRows.Count)
            {
                _ = new ToLowerCaseAll().TableT();
            }
            else
            {
                _ = new ToLowerCaseAll().Selected();
            }
        }
    }
}
