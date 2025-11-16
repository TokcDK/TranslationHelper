using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Forms.Search;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemRowSearchAlt : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override int Order => base.Order - 200;

        public override string Text => T._("Search (Alt)");

        public override string Description => T._("Open search Window");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.Main.THFilesList.GetSelectedIndex() == -1) return;

            try
            {
                if (AppData.Main.searchformNew == null || AppData.Main.searchformNew.IsDisposed)
                {
                    AppData.Main.searchformNew = new SearchForm(AppData.CurrentProject);
                    AppData.Main.searchformNew.Show();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public override Keys ShortcutKeys => Keys.Control | Keys.Shift | Keys.F;
    }
}
