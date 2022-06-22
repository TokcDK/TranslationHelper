using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects.IrisField;
using TranslationHelper.Projects.KiriKiri;
using TranslationHelper.Projects.RJ263914;
using TranslationHelper.Projects.RPGMMV;
using TranslationHelper.Projects.RPGMTrans;
using TranslationHelper.Projects.WolfRPG;

namespace TranslationHelper.Functions
{
    class FunctionsSave
    {

        public FunctionsSave()
        {

        }

        internal async Task PrepareToWrite()
        {
            if (AppData.Main.SaveInAction /*|| !ProjectData.Main.FIleDataWasChanged*/)
            {
                //MessageBox.Show("Saving still in progress. Please wait a little.");
                return;
            }
            AppData.Main.SaveInAction = true;
            AppData.Main.FileDataWasChanged = false;
            AppData.CurrentProject.SaveFileMode = true;

            //MessageBox.Show("THSelectedSourceType=" + THSelectedSourceType);

            if (AppData.CurrentProject != null)
            {
                AppData.CurrentProject.BakCreate();
                await Task.Run(() => AppData.CurrentProject.Save()).ConfigureAwait(true);
            }
            else
            {
                //if (ProjectData.CurrentProject.Name() == new IrisFieldGameBase().Name())
                //{
                //    await Task.Run(() => new IrisFieldGameBase().Save()).ConfigureAwait(true);
                //}
                //else 
                //if (ProjectData.CurrentProject.Name() == "RubyRPGGame")
                //{
                //    new RJ263914OLD().ProceedRubyRPGGame(ProjectData.CurrentProject.SelectedGameDir, true);
                //    //MessageBox.Show("Finished");
                //}
                //else if (ProjectData.CurrentProject.Name() == "Wolf RPG txt")
                //{
                //    new WRPGOLDOpen().ProceedWriteWolfRPGtxt();
                //    //MessageBox.Show("Finished");
                //}
                //else if (ProjectData.CurrentProject.Name() == "WOLF TRANS PATCH")
                //{
                //    new WRPGOLDOpen().WriteWOLFTRANSPATCH();
                //    //MessageBox.Show("Finished");
                //}
                //else if (ProjectData.CurrentProject.Name() == "RPGMakerTransPatch" || ProjectData.CurrentProject.Name() == "RPG Maker game with RPGMTransPatch")
                //{
                //    //THActionProgressBar.Visible = true;
                //    //THInfolabel.Visible = true;
                //    //THInfolabel.Text = "saving..";
                //    ProjectData.Main.ProgressInfo(true);

                //    //THInfoTextBox.Text = "Saving...";

                //    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                //    //Thread save = new Thread(new ParameterizedThreadStart((obj) => SaveRPGMTransPatchFiles(ProjectData.CurrentProject.SelectedDir, THRPGMTransPatchver)));
                //    //save.Start();

                //    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                //    await Task.Run(() => new RPGMTransOLD().SaveRPGMTransPatchFiles(ProjectData.CurrentProject.SelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);

                //    //MessageBox.Show("ProjectData.CurrentProject.SelectedDir=" + ProjectData.CurrentProject.SelectedDir);
                //    //SaveRPGMTransPatchFiles(ProjectData.CurrentProject.SelectedDir, THRPGMTransPatchver);

                //    //THInfoTextBox.Text = string.Empty;

                //    //THActionProgressBar.Visible = false;

                //    if (ProjectData.CurrentProject.Name() == "RPG Maker game with RPGMTransPatch")
                //    {
                //        string rpgmakertranscli = Application.StartupPath + @"\Res\rpgmakertrans\rpgmt.exe";

                //        //параметры
                //        //parser.add_argument("input", help = "Path of input game to patch")
                //        //parser.add_argument("-p", "--patch", help = "Path of patch (directory or zip)"
                //        //        "(Defaults to input_directory_patch")
                //        //parser.add_argument("-o", "--output", help = "Path to output directory "
                //        //        "(will create) (Defaults to input_directory_translated)")
                //        //parser.add_argument('-q', '--quiet', help = 'Suppress all output',
                //        //        action = 'store_true')
                //        //parser.add_argument('-b', '--use-bom', help = 'Use UTF-8 BOM in Patch'
                //        //        'files', action = 'store_true')
                //        //parser.add_argument('-r', '--rebuild', help = "Rebuild patch against game",
                //        //        action = "store_true")
                //        //parser.add_argument('-s', '--socket', type = int, default = 27899,
                //        //        help = 'Socket to use for XP/VX/VX Ace patching'
                //        //        '(default: 27899)')
                //        //parser.add_argument('-l', '--dump-labels', action = "store_true",
                //        //        help = "Dump labels to patch file")
                //        //parser.add_argument('--dump-scripts', type = str, default = None,
                //        //        help = "Dump scripts to given directory")
                //        string rpgmakertranscliargs = "\"" + ProjectData.CurrentProject.SelectedGameDir + "\" -p \"" + ProjectData.CurrentProject.SelectedDir + "\"" + " -o \"" + ProjectData.CurrentProject.SelectedDir.Remove(ProjectData.CurrentProject.SelectedDir.Length - "_patch".Length, "_patch".Length) + "_translated\"";

                //        if (FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs))
                //        {
                //        }
                //        else
                //        {
                //            FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs + " -b");// попытка с параметром -b
                //        }
                //    }

                //    ProjectData.Main.SaveInAction = false;
                //}
                //else if (ProjectData.CurrentProject.Name() == "RPG Maker MV json")
                //{
                //    ///*THMsg*/MessageBox.Show(ProjectData.CurrentProject.SelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                //    await Task.Run(() => new RPGMMVOLD().WriteJson(ProjectData.Main.THFilesList.GetItemName(0), ProjectData.CurrentProject.SelectedDir + "\\" + ProjectData.Main.THFilesList.GetItemName(0) + ".json")).ConfigureAwait(true);
                //}
                //else if (ProjectData.CurrentProject.Name() == "RPG Maker MV")
                //{
                //    for (int f = 0; f < ProjectData.Main.THFilesList.GetItemsCount(); f++)
                //    {
                //        //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                //        //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                //        bool changed = false;
                //        for (int r = 0; r < ProjectData.CurrentProject.FilesContent.Tables[f].Rows.Count; r++)
                //        {
                //            if ((ProjectData.CurrentProject.FilesContent.Tables[f].Rows[r][THSettings.TranslationColumnName()] + string.Empty).Length == 0)
                //            {
                //            }
                //            else
                //            {
                //                changed = true;
                //                break;
                //            }
                //        }
                //        ///*THMsg*/MessageBox.Show(ProjectData.CurrentProject.SelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                //        if (changed)
                //        {

                //            ///*THMsg*/MessageBox.Show("start writing");
                //            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                //            await Task.Run(() => new RPGMMVOLD().WriteJson(ProjectData.Main.THFilesList.GetItemName(f), ProjectData.CurrentProject.SelectedDir + "\\www\\data\\" + ProjectData.Main.THFilesList.GetItemName(f) + ".json")).ConfigureAwait(true);
                //            //WriteJson(THFilesListBox.Items[f].ToString(), ProjectData.CurrentProject.SelectedDir + "\\www\\data\\" + THFilesListBox.Items[f].ToString() + ".json");
                //        }
                //    }
                //    /*THMsg*/
                //    //MessageBox.Show(T._("finished") + "!");
                //}
                //else if (ProjectData.CurrentProject.Name() == "KiriKiri scenario")
                //{
                //    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                //    //await Task.Run(() => KiriKiriScenarioWrite(ProjectData.CurrentProject.SelectedDir + "\\" + THFilesList.GetItemName(0) + ".scn"));
                //    await Task.Run(() => new KiriKiriOLD().KiriKiriScriptScenarioWrite(ProjectData.CurrentProject.SelectedDir + "\\" + ProjectData.Main.THFilesList.GetItemName(0) + ".scn")).ConfigureAwait(true);
                //}
                //else if (ProjectData.CurrentProject.Name() == "KiriKiri script")
                //{
                //    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                //    await Task.Run(() => new KiriKiriOLD().KiriKiriScriptScenarioWrite(ProjectData.CurrentProject.SelectedDir + "\\" + ProjectData.Main.THFilesList.GetItemName(0) + ".ks")).ConfigureAwait(true);
                //}
            }


            AppData.Main.SaveInAction = false;
            FunctionsSounds.PlayAsterisk();
        }

        /// <summary>
        /// Write RPGMakerMV event codes stats
        /// </summary>
        public static void WriteRPGMakerMVStats()
        {
            if (AppData.RpgMVAddedCodesStat.Count > 0 || AppData.RpgMVSkippedCodesStat.Count > 0)
            {
                AppData.RpgMVAddedCodesStat = AppData.RpgMVAddedCodesStat.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
                AppData.RpgMVSkippedCodesStat = AppData.RpgMVSkippedCodesStat.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

                foreach (var dict in new Dictionary<string, Dictionary<int, int>>()
                    {
                        {"RPGMakerMV Added codes stats", AppData.RpgMVAddedCodesStat },
                        {"RPGMakerMV Skipped codes stats", AppData.RpgMVSkippedCodesStat }
                    }
                )
                {
                    if (AppData.Main.Settings.THConfigINI.SectionExistsAndNotEmpty(dict.Key))
                    {
                        foreach (var pair in AppData.Main.Settings.THConfigINI.GetSectionKeyValuePairs(dict.Key))
                        {
                            var key = int.Parse(pair.Key);
                            var value = int.Parse(pair.Value);
                            if (!dict.Value.ContainsKey(key))
                            {
                                dict.Value.Add(key, value);
                            }
                            else
                            {
                                dict.Value[key] = dict.Value[key] + value;
                            }
                        }
                    }

                    foreach (var pair in dict.Value)
                    {
                        AppData.Main.Settings.THConfigINI.SetKey(dict.Key, pair.Key + "", pair.Value + "");
                    }
                }

                AppData.Main.Settings.THConfigINI.WriteFile();
            }
        }
    }
}
