using System;
using System.Diagnostics;
using TranslationHelper.Data;
using TranslationHelper.Menus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus.Edit;

namespace TranslationHelper.Projects.RPGMMV.Menus
{
    class MainMenuTryReadEnglishTranslations : MainMenuEditSubItemBase, IProjectSpecifiedMenuItem
    {
        public override string Text => "[" + AppData.CurrentProject.Name+ "]" + T._("Try read English language strings into translations");

        public override string Description => "Try read translation from same elements of English language if present";

        public override void OnClick(object sender, EventArgs e)
        {
        }
    }
}
