using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemSaveDB : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Save DB");

        public override string Description => T._("Save translated strings into database file");

        public override async void OnClick(object sender, EventArgs e)
        {
            if (AppData.CurrentProject == null) return;

            var fileName = FunctionsDBFile.GetDBFileName();
            var fileExtension = FunctionsDBFile.GetDBCompressionExt();
            var path = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), fileName + fileExtension);

            if (System.IO.File.Exists(path))
            {
                ShiftToBakups(path);
            }

            var pathNextToSource = Path.Combine(AppData.CurrentProject.SelectedDir, Data.AppData.TranslationFileSourceDirSuffix + fileExtension);

            FunctionAutoSave.lastautosavepath = path;

            AppData.Main.ProgressInfo(true);

            //switch (AppData.CurrentProject.Name)
            //{
            //    case "RPGMakerTransPatch":
            //    case "RPG Maker game with RPGMTransPatch":
            //        //_ = await Task.Run(() => new RPGMTransOLD().SaveRPGMTransPatchFiles(AppData.CurrentProject.SelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);
            //        break;
            //}

            await Task.Run(() => AppData.CurrentProject.PreSaveDB()).ConfigureAwait(true);
            await Task.Run(() => AppData.Main.WriteDBFileLite(AppData.CurrentProject.FilesContent, new[] { path, pathNextToSource })).ConfigureAwait(true);

            FunctionsSounds.SaveDBComplete();
            AppData.Main.ProgressInfo(false);
        }

        private void ShiftToBakups(string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                var ext = Path.GetExtension(path);

                int maxBakIndex = 9;
                for (int i = maxBakIndex; i >= 1; i--)
                {
                    MoveFile(dir, name, ext, i, maxBakIndex);
                }

                System.IO.File.Move(path, Path.Combine(dir, name + _saveFileBakSuffix + 1 + ext));
            }
            catch (Exception ex)
            {
            }
        }

        readonly string _saveFileBakSuffix = "_bak";
        private void MoveFile(string dir, string name, string ext, int index, int maxBakIndex)
        {
            var path = Path.Combine(dir, name + _saveFileBakSuffix + index + ext);
            if (!System.IO.File.Exists(path)) return;

            if (index == maxBakIndex)
            {
                System.IO.File.Delete(path);
            }
            else
            {
                System.IO.File.Move(path, Path.Combine(dir, name + _saveFileBakSuffix + (index + 1) + ext));
            }
        }

        public override int Order => base.Order + 15;

        public override Keys ShortcutKeys => Keys.Control | Keys.S;
    }
}
