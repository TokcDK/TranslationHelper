using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemSaveDB : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Save DB");

        public override string Description => T._("Save translated strings into database file");

        public override async void OnClick(object sender, EventArgs e)
        {
            var path = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName() + FunctionsDBFile.GetDBCompressionExt());
            AppData.Main.lastautosavepath = path;

            AppData.Main.ProgressInfo(true);

            //switch (AppData.CurrentProject.Name)
            //{
            //    case "RPGMakerTransPatch":
            //    case "RPG Maker game with RPGMTransPatch":
            //        //_ = await Task.Run(() => new RPGMTransOLD().SaveRPGMTransPatchFiles(AppData.CurrentProject.SelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);
            //        break;
            //}

            await Task.Run(() => AppData.CurrentProject.PreSaveDB()).ConfigureAwait(true);
            await Task.Run(() => AppData.Main.WriteDBFileLite(AppData.CurrentProject.FilesContent, path)).ConfigureAwait(true);

            FunctionsSounds.SaveDBComplete();
            AppData.Main.ProgressInfo(false);
        }
        public override int Order => base.Order + 15;
    }
}
