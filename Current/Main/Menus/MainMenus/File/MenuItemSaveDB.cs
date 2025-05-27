using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemSaveDB : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Save DB");

        public override string Description => T._("Save translated strings into database file");

        public override async void OnClick(object sender, EventArgs e)
        {
            if (AppData.CurrentProject == null) return;

            await FunctionsDBFile.SaveDB();
        }

        public override int Order => base.Order + 15;

        public override Keys ShortcutKeys => Keys.Control | Keys.S;
    }
}
