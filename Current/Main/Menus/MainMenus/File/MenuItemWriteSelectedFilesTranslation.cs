using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemWriteSelectedFilesTranslation : MenuItemWriteTranslation, IProjectMenuItem, IFileListMenuItem
    {
        public override string Text => $"{base.Text} {_suffix}";

        public override string Description => $"{base.Description} {_suffix}";

        public override int Order => base.Order + 5;

        protected override bool WriteSelected => true;

        private readonly string _suffix = T._("(Selected)");
    }
}
