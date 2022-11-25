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
    internal class MenuItemRowTextPaste : MainMenuEditSubItemBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Paste");

        public override string Description => T._("Paste translation into selected rows");

        public override void OnClick(object sender, EventArgs e)
        {
            if (AppSettings.DGVCellInEditMode) AppData.Main.ControlsSwitch(); //если ячейка в режиме редактирования выключение действий для ячеек при выходе из режима редактирования  

            new PasteTranslation().Rows();
        }

        public override Keys ShortcutKeys => Keys.Control | Keys.V;
        public override int Order => base.Order - 98;
    }
}
