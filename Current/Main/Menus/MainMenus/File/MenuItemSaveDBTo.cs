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
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemSaveDBTo : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Save DB to");

        public override string Description => T._("Save translated strings into database file into selected locatin");

        public override async void OnClick(object sender, EventArgs e)
        {
            // The project being worked on, named once: the folder and the file name are its, and the
            // dialog below can outlive the selection it was opened from.
            var workspace = AppData.ActiveWorkspace;
            var project = workspace?.Project;
            if (project == null) return;

            using (SaveFileDialog THFSaveBDAs = new SaveFileDialog())
            {
                var selectedFormat = FunctionsInterfaces.GetCurrentDBFormat();
                THFSaveBDAs.Filter = $"{selectedFormat.Description}|*.{selectedFormat.Ext}";

                THFSaveBDAs.InitialDirectory = FunctionsDBFile.GetProjectDBFolder(project);
                THFSaveBDAs.FileName = FunctionsDBFile.GetDBFileName(workspace, true) + FunctionsDBFile.GetDBCompressionExt();

                if (THFSaveBDAs.ShowDialog() != DialogResult.OK) return;
                if (THFSaveBDAs.FileName.Length == 0) return;

                //string spath = THFOpenBD.FileName;
                //THFOpenBD.OpenFile().Close();
                //MessageBox.Show(THFOpenBD.FileName);
                //LoadTranslationFromDB();

                

                switch (project.Name)
                {
                    case "RPGMakerTransPatch":
                    case "RPG Maker game with RPGMTransPatch":
                        // RPGMTransOLD is marked [Obsolete] but is still the only implementation
                        // that can write an RPG Maker Trans patch, so the legacy call stays.
                        // Scoped suppression instead of a project-wide one keeps it visible.
#pragma warning disable CS0612 // Type or member is obsolete
                        _ = await Task.Run(() => new RPGMTransOLD().SaveRPGMTransPatchFiles(project.SelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);
#pragma warning restore CS0612 // Type or member is obsolete
                        break;
                }

                //SaveNEWDB(THFilesElementsDataset, THFSaveBDAs.FileName);
                //WriteDBFile(THFilesElementsDataset, THFSaveBDAs.FileName);

                // Written into a file the user chose, which is a copy and not the project's database:
                // the project stays marked as having translations its own database does not have, so
                // closing it still offers to save them where they belong.
                await Task.Run(() => FunctionsDBFile.WriteDBFileLite(project.FilesContent, new[] { THFSaveBDAs.FileName })).ConfigureAwait(true);
                //Task task = new Task(() => WriteDBFileLite(ProjectData.THFilesElementsDataset, THFSaveBDAs.FileName));
                //task.Start();
                //task.Wait();

                FunctionsSounds.SaveDBComplete();
                
            }
        }

        public override int Order => base.Order + 16;

        public override Keys ShortcutKeys => Keys.Control | Keys.Shift | Keys.S;
    }
}
