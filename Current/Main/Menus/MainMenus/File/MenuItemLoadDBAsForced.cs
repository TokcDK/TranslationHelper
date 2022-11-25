using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemLoadDBAsForced : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Load DB as (forced)");

        public override string Description => T._("Load translated strings from database file of selected location");

        public override void OnClick(object sender, EventArgs e)
        {
            AppData.Main.LoadDBAs(true);
        }
        public override int Order => base.Order + 12;
    }
}
