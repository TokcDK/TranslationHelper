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
    internal class MenuItemRowSearch : MainMenuEditSubItemBase, IProjectMenuItem
    {
        public override int Order => base.Order - 200;

        public override string Text => T._("Search");

        public override string Description => T._("Open search Window");

        public override void OnClick(object sender, EventArgs e)
        {
            var workspace = AppData.ActiveWorkspace;
            if (workspace?.FilesList == null || workspace.FilesList.GetSelectedIndex() == -1) return;

            try
            {
                if (AppData.Main.search == null || AppData.Main.search.IsDisposed)
                {
                    // The form is given the workspace, not three loose controls: it works on the project
                    // the user is looking at, and reads that project's controls as it goes.
                    AppData.Main.search = new THfrmSearch(workspace);
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
