using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    internal static class FunctionAutoSave
    {
        static string dbpath;
        internal static string lastautosavepath;

        static bool AutosaveActivated;
        internal static Task Autosave()
        {
            if (!AppSettings.EnableDBAutosave || AutosaveActivated || AppData.CurrentProject.FilesContent == null)
            {
                return Task.CompletedTask;
            }

            AutosaveActivated = true;

            dbpath = THSettings.DBDirPathByLanguage;
            string dbfilename = Path.GetFileNameWithoutExtension(AppData.CurrentProject.SelectedDir) + "_autosave";
            string autosavepath = Path.Combine(dbpath, "Auto", dbfilename + ".bak1" + ".cmx");
            if (File.Exists(autosavepath))
            {
                int saveindexmax = 5;
                for (int index = saveindexmax; index > 0; index--)
                {
                    if (index == saveindexmax)
                    {
                        if (File.Exists(Path.Combine(dbpath, "Auto", dbfilename + ".bak" + index + ".cmx")))
                        {
                            File.Delete(Path.Combine(dbpath, "Auto", dbfilename + ".bak" + index + ".cmx"));
                        }
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(dbpath, "Auto", dbfilename + ".bak" + index + ".cmx")))
                        {
                            File.Move(Path.Combine(dbpath, "Auto", dbfilename + ".bak" + index + ".cmx")
                                , Path.Combine(dbpath, "Auto", dbfilename + ".bak" + (index + 1) + ".cmx"));
                        }
                    }
                }
            }

            Thread IndicateSave = new Thread(new ParameterizedThreadStart((obj) => AppData.Main.IndicateSaveProcess(T._("Saving") + "...")));
            IndicateSave.Start();

            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            Thread trans = new Thread(new ParameterizedThreadStart((obj) => SaveLoop(AppData.CurrentProject.FilesContent, autosavepath)));
            trans.Start();

            //ProgressInfo(true);

            //WriteDBFile(THFilesElementsDataset, lastautosavepath);
            ////THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data

            //Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath);

            //ProgressInfo(false);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Background autosave
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Path"></param>
        internal static async void SaveLoop(DataSet Data, string Path)
        {
            //asdf autosave
            while (AutosaveActivated && Data != null && Path.Length > 0)
            {
                if (FunctionsTable.TheDataSetIsNotEmpty(Data))
                {
                }
                else//если dataset пустой, нет смысла его сохранять
                {
                    AutosaveActivated = false;
                    return;
                }

                int i = 0;
                while (i < AppSettings.DBAutoSaveTimeout && AppSettings.EnableDBAutosave)
                {
                    Thread.Sleep(1000);
                    if (AppSettings.IsTranslationHelperWasClosed || AppData.Main == null || AppData.Main.IsDisposed/* || Data == null || Path.Length == 0*/)
                    {
                        AutosaveActivated = false;
                        return;
                    }
                    i++;
                }
                while (FunctionsUI.IsOpeningInProcess || FunctionsUI.SaveInAction)//не запускать автосохранение, пока утилита занята
                {
                    Thread.Sleep(AppSettings.DBAutoSaveTimeout * 1000);
                }
                await Task.Run(() =>FunctionsDBFile.WriteDBFileLite(Data, new[] { Path }) ).ConfigureAwait(true);
            }
        }
    }
}
