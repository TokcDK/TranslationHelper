using System;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{    
    public abstract class MainMenuFileSubItemEnQuotesToJp : MainMenuEditSubItemBase
    {
        public override IMenuItem[] Childs => new IMenuItem[1] { new MenuItemCaseMorphEnQuotesToJpVariated() };
    }

    public abstract class MainMenuFileSubItemEnQuotesToJpBase : IMenuItem, IChildMenuItem
    {
        public abstract string Text { get; }

        public virtual string Description { get; } = "";
        public virtual string CategoryName => "";

        public virtual Keys ShortcutKeys => Keys.None;

        public abstract void OnClick(object sender, EventArgs e);

        public virtual IMenuItem[] Childs { get; } = null;
    }

    internal class MenuItemCaseMorphEnQuotesToJpVariated : MainMenuFileSubItemCaseMorphToUpperBase
    {
        public override string Text => T._("EnQuotesToJp");

        public override string Description => T._("EnQuotesToJp");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.THFilesList.SelectedItems.Count == AppData.THFilesList.Items.Count)
            {
                _ = new EnQuotesToJp().AllT();
            }
            else if (AppData.Main.THFileElementsDataGridView.Rows.Count == AppData.Main.THFileElementsDataGridView.SelectedRows.Count)
            {
                _ = new EnQuotesToJp().TableT();
            }
            else
            {
                _ = new EnQuotesToJp().Selected();
            }
        }
    }
}
