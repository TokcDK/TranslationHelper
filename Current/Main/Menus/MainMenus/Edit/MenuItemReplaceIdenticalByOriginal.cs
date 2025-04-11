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
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemReplaceIdenticalByOriginal : MainMenuEditSubItemBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Set all identical to original");

        public override string Description => T._("Set the all rows with the same translations to original value");

        public override void OnClick(object sender, EventArgs e)
        {
            new ReplaceIdenticalByOriginalGetSelected().Rows();
        }

        public override int Order => base.Order + 100;
    }
}
