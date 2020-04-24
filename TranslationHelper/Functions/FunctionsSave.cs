using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;
using TranslationHelper.Projects.HowToMakeTrueSlavesRiseofaDarkEmpire;
using TranslationHelper.Projects.KiriKiri;
using TranslationHelper.Projects.RJ263914;
using TranslationHelper.Projects.RPGMaker;
using TranslationHelper.Projects.RPGMMV;
using TranslationHelper.Projects.RPGMTrans;
using TranslationHelper.Projects.WolfRPG;

namespace TranslationHelper.Functions
{
    class FunctionsSave
    {
        THDataWork thDataWork;
        public FunctionsSave(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal async void PrepareToWrite()
        {
            if (thDataWork.Main.SaveInAction /*|| !thDataWork.Main.FIleDataWasChanged*/)
            {
                //MessageBox.Show("Saving still in progress. Please wait a little.");
                return;
            }
            thDataWork.Main.SaveInAction = true;
            thDataWork.Main.FIleDataWasChanged = false;

            //MessageBox.Show("THSelectedSourceType=" + THSelectedSourceType);

            if (thDataWork.CurrentProject != null)
            {
                thDataWork.CurrentProject.Save();
            }
            else
            {
                if (RPGMFunctions.THSelectedSourceType == new HowToMakeTrueSlavesRiseofaDarkEmpire(thDataWork).ProjectTitle())
                {
                    await Task.Run(() => new HowToMakeTrueSlavesRiseofaDarkEmpire(thDataWork).Save()).ConfigureAwait(true);
                }
                else if (RPGMFunctions.THSelectedSourceType == "RubyRPGGame")
                {
                    new RJ263914OLD(thDataWork).ProceedRubyRPGGame(Properties.Settings.Default.THSelectedGameDir, true);
                    MessageBox.Show("Finished");
                }
                else if (RPGMFunctions.THSelectedSourceType == "Wolf RPG txt")
                {
                    new WRPGOLDOpen(thDataWork).ProceedWriteWolfRPGtxt();
                    MessageBox.Show("Finished");
                }
                else if (RPGMFunctions.THSelectedSourceType == "WOLF TRANS PATCH")
                {
                    new WRPGOLDOpen(thDataWork).WriteWOLFTRANSPATCH();
                    MessageBox.Show("Finished");
                }
                else if (RPGMFunctions.THSelectedSourceType == "RPGMakerTransPatch" || RPGMFunctions.THSelectedSourceType == "RPG Maker game with RPGMTransPatch")
                {
                    //THActionProgressBar.Visible = true;
                    //THInfolabel.Visible = true;
                    //THInfolabel.Text = "saving..";
                    thDataWork.Main.ProgressInfo(true);

                    //THInfoTextBox.Text = "Saving...";

                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    //Thread save = new Thread(new ParameterizedThreadStart((obj) => SaveRPGMTransPatchFiles(Properties.Settings.Default.THSelectedDir, THRPGMTransPatchver)));
                    //save.Start();

                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => new RPGMTransOLD(thDataWork).SaveRPGMTransPatchFiles(Properties.Settings.Default.THSelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);

                    //MessageBox.Show("Properties.Settings.Default.THSelectedDir=" + Properties.Settings.Default.THSelectedDir);
                    //SaveRPGMTransPatchFiles(Properties.Settings.Default.THSelectedDir, THRPGMTransPatchver);

                    //THInfoTextBox.Text = string.Empty;

                    //THActionProgressBar.Visible = false;

                    if (RPGMFunctions.THSelectedSourceType == "RPG Maker game with RPGMTransPatch")
                    {
                        string rpgmakertranscli = Application.StartupPath + @"\Res\rpgmakertrans\rpgmt.exe";

                        //параметры
                        //parser.add_argument("input", help = "Path of input game to patch")
                        //parser.add_argument("-p", "--patch", help = "Path of patch (directory or zip)"
                        //        "(Defaults to input_directory_patch")
                        //parser.add_argument("-o", "--output", help = "Path to output directory "
                        //        "(will create) (Defaults to input_directory_translated)")
                        //parser.add_argument('-q', '--quiet', help = 'Suppress all output',
                        //        action = 'store_true')
                        //parser.add_argument('-b', '--use-bom', help = 'Use UTF-8 BOM in Patch'
                        //        'files', action = 'store_true')
                        //parser.add_argument('-r', '--rebuild', help = "Rebuild patch against game",
                        //        action = "store_true")
                        //parser.add_argument('-s', '--socket', type = int, default = 27899,
                        //        help = 'Socket to use for XP/VX/VX Ace patching'
                        //        '(default: 27899)')
                        //parser.add_argument('-l', '--dump-labels', action = "store_true",
                        //        help = "Dump labels to patch file")
                        //parser.add_argument('--dump-scripts', type = str, default = None,
                        //        help = "Dump scripts to given directory")
                        string rpgmakertranscliargs = "\"" + Properties.Settings.Default.THSelectedGameDir + "\" -p \"" + Properties.Settings.Default.THSelectedDir + "\"" + " -o \"" + Properties.Settings.Default.THSelectedDir.Remove(Properties.Settings.Default.THSelectedDir.Length - "_patch".Length, "_patch".Length) + "_translated\"";

                        if (FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs))
                        {
                        }
                        else
                        {
                            FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs + " -b");// попытка с параметром -b
                        }
                    }

                    thDataWork.Main.SaveInAction = false;
                }
                else if (RPGMFunctions.THSelectedSourceType == "RPG Maker MV json")
                {
                    //THMsg.Show(Properties.Settings.Default.THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                    new RPGMMVOLD(thDataWork).WriteJson(thDataWork.Main.THFilesList.Items[0] + string.Empty, Properties.Settings.Default.THSelectedDir + "\\" + thDataWork.Main.THFilesList.Items[0] + ".json");
                }
                else if (RPGMFunctions.THSelectedSourceType == "RPG Maker MV")
                {
                    for (int f = 0; f < thDataWork.Main.THFilesList.Items.Count; f++)
                    {
                        //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                        //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                        bool changed = false;
                        for (int r = 0; r < thDataWork.THFilesElementsDataset.Tables[f].Rows.Count; r++)
                        {
                            if ((thDataWork.THFilesElementsDataset.Tables[f].Rows[r]["Translation"] + string.Empty).Length == 0)
                            {
                            }
                            else
                            {
                                changed = true;
                                break;
                            }
                        }
                        //THMsg.Show(Properties.Settings.Default.THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                        if (changed)
                        {

                            //THMsg.Show("start writing");
                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            await Task.Run(() => new RPGMMVOLD(thDataWork).WriteJson(thDataWork.Main.THFilesList.Items[f] + string.Empty, Properties.Settings.Default.THSelectedDir + "\\www\\data\\" + thDataWork.Main.THFilesList.Items[f] + ".json")).ConfigureAwait(true);
                            //WriteJson(THFilesListBox.Items[f].ToString(), Properties.Settings.Default.THSelectedDir + "\\www\\data\\" + THFilesListBox.Items[f].ToString() + ".json");
                        }
                    }
                    THMsg.Show(T._("finished") + "!");
                }
                else if (RPGMFunctions.THSelectedSourceType == "KiriKiri scenario")
                {
                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    //await Task.Run(() => KiriKiriScenarioWrite(Properties.Settings.Default.THSelectedDir + "\\" + THFilesList.Items[0] + ".scn"));
                    await Task.Run(() => new KiriKiriOLD(thDataWork).KiriKiriScriptScenarioWrite(Properties.Settings.Default.THSelectedDir + "\\" + thDataWork.Main.THFilesList.Items[0] + ".scn")).ConfigureAwait(true);
                }
                else if (RPGMFunctions.THSelectedSourceType == "KiriKiri script")
                {
                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => new KiriKiriOLD(thDataWork).KiriKiriScriptScenarioWrite(Properties.Settings.Default.THSelectedDir + "\\" + thDataWork.Main.THFilesList.Items[0] + ".ks")).ConfigureAwait(true);
                }
            }


            thDataWork.Main.SaveInAction = false;
        }
    }
}
