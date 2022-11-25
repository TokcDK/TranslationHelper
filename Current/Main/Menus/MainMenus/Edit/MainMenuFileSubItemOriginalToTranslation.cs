using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.CaseMorph
{
    public class MainMenuFileSubItemSetOriginalToTranslationCategory : MainMenuEditSubItemBase, IFileRowMenuItem, IFileListMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Original To Translation");
        public override IMenuItem[] Childs => new IMenuItem[2]
        {
            new MenuItemCaseMorphSetOriginalToTranslationVariated(),
            new MenuItemCaseMorphSetOriginalToTranslationForExistFiles()
        };

        public override void OnClick(object sender, EventArgs e) { }
    }

    internal class MenuItemCaseMorphSetOriginalToTranslationVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("Original=Translation");

        public override string Description => T._("Set rows translation equal to original");

        protected override void OnAll(object sender, EventArgs e)
        {
            _ = new SetOriginalToTranslation().AllT();
        }

        protected override void OnRows(object sender, EventArgs e)
        {
            new SetOriginalToTranslation().Rows();
        }

        protected override void OnTable(object sender, EventArgs e)
        {
            _ = new SetOriginalToTranslation().TableT();
        }

        public override Keys ShortcutKeys => Keys.F8;
    }
    internal class MenuItemCaseMorphSetOriginalToTranslationForExistFiles : ChildMenuBaseBase
    {
        public override string Text => T._("All, if exists file/dir with name");

        public override string Description => T._("All, if exists file/dir with name");

        public override void OnClick(object sender, EventArgs e)
        {
            Thread newthread = new Thread(new ParameterizedThreadStart((obj) =>
            SetOriginalToTranslationIfFileExistsInAnyFolder()
            ));
            newthread.Start();
        }

        internal static void SetOriginalToTranslationIfFileExistsInAnyFolder()
        {
            string[] ProjectFilesList = Directory.GetFiles(AppData.CurrentProject.SelectedGameDir, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < ProjectFilesList.Length; i++)
            {
                ProjectFilesList[i] = Path.GetFileNameWithoutExtension(ProjectFilesList[i]);
            }
            ProjectFilesList = ProjectFilesList.Distinct().ToArray();

            int cind = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.OriginalColumnName].Ordinal;// Колонка Original
            int cindTrans = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;// Колонка Original

            int tablesCount = AppData.CurrentProject.FilesContent.Tables.Count;
            for (int t = 0; t < tablesCount; t++)
            {
                int rowsCount = AppData.CurrentProject.FilesContent.Tables[t].Rows.Count;
                for (int r = 0; r < rowsCount; r++)
                {
                    string origCellValue = AppData.CurrentProject.FilesContent.Tables[t].Rows[r][cind] as string;
                    string transCellValue = AppData.CurrentProject.FilesContent.Tables[t].Rows[r][cindTrans] + string.Empty;

                    if ((transCellValue.Length == 0 || origCellValue != transCellValue) && FunctionsFileFolder.GetAnyFileWithTheNameExist(ProjectFilesList, origCellValue))
                    {
                        AppData.CurrentProject.FilesContent.Tables[t].Rows[r][cindTrans] = origCellValue;
                    }
                }
            }
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
