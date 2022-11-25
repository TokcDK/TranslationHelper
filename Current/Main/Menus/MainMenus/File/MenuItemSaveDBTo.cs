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
    internal class MenuItemSaveDBTo : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Save DB to");

        public override string Description => T._("Save translated strings into database file into selected locatin");

        public override async void OnClick(object sender, EventArgs e)
        {
            using (SaveFileDialog THFSaveBDAs = new SaveFileDialog())
            {
                THFSaveBDAs.Filter = "DB file|*.xml;*.cmx;*.cmz|XML-file|*.xml|Gzip compressed DB (*.cmx)|*.cmx|Deflate compressed DB (*.cmz)|*.cmz";

                THFSaveBDAs.InitialDirectory = FunctionsDBFile.GetProjectDBFolder();
                THFSaveBDAs.FileName = FunctionsDBFile.GetDBFileName(true) + FunctionsDBFile.GetDBCompressionExt();

                if (THFSaveBDAs.ShowDialog() != DialogResult.OK) return;
                if (THFSaveBDAs.FileName.Length == 0) return;

                //string spath = THFOpenBD.FileName;
                //THFOpenBD.OpenFile().Close();
                //MessageBox.Show(THFOpenBD.FileName);
                //LoadTranslationFromDB();

                AppData.Main.ProgressInfo(true);

                switch (AppData.CurrentProject.Name)
                {
                    case "RPGMakerTransPatch":
                    case "RPG Maker game with RPGMTransPatch":
                        _ = await Task.Run(() => new RPGMTransOLD().SaveRPGMTransPatchFiles(AppData.CurrentProject.SelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);
                        break;
                }

                //SaveNEWDB(THFilesElementsDataset, THFSaveBDAs.FileName);
                //WriteDBFile(THFilesElementsDataset, THFSaveBDAs.FileName);

                await Task.Run(() => AppData.Main.WriteDBFileLite(AppData.CurrentProject.FilesContent, THFSaveBDAs.FileName)).ConfigureAwait(true);
                //Task task = new Task(() => WriteDBFileLite(ProjectData.THFilesElementsDataset, THFSaveBDAs.FileName));
                //task.Start();
                //task.Wait();

                FunctionsSounds.SaveDBComplete();
                AppData.Main.ProgressInfo(false);
            }
        }
        public override int Order => base.Order + 16;
    }
}
