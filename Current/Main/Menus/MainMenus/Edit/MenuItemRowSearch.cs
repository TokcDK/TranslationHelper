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
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemRowSearch : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Search");

        public override string Description => T._("Open search Window");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppData.Main.THFilesList.GetSelectedIndex() == -1) return;

            try
            {
                if (AppData.Main.search == null || AppData.Main.search.IsDisposed)
                {
                    AppData.Main.search = new THfrmSearch(AppData.Main.THFilesList, AppData.Main.THFileElementsDataGridView, AppData.Main.THTargetRichTextBox);
                }

                if (AppData.Main.search.Visible)
                {
                    AppData.Main.search.Activate();//помещает на передний план
                    AppData.Main.search.GetSelectedText();
                }
                else
                {
                    AppData.Main.search.Show();
                    AppData.Main.search.GetSelectedText();
                    //поместить на передний план
                    //search.TopMost = true;
                    //search.TopMost = false;
                }
            }
            catch
            {
            }
        }

        public override Keys ShortcutKeys => Keys.Control | Keys.F;
    }
}
