using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Menus.MainMenus.Edit;

namespace TranslationHelper.Menus.MainMenus.View
{
    public abstract class OpenPathByExplorerMainMenuViewSubItemBase : MainMenuViewSubItemBase
    {
        // this items
        public abstract string Name { get; }
        public abstract string DirPath { get; }

        // menu's items
        public override string Text => T._("Open") +" " + Name;
        public override string Description => Text;
        public override void OnClick(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(DirPath)) 
                _ = Process.Start("explorer.exe", DirPath);
        }
    }
}
