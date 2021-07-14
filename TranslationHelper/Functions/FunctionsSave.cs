using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects.HowToMakeTrueSlavesRiseofaDarkEmpire;
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
            if (ProjectData.Main.SaveInAction /*|| !ProjectData.Main.FIleDataWasChanged*/)
            {
                //MessageBox.Show("Saving still in progress. Please wait a little.");
                return;
            }
            ProjectData.Main.SaveInAction = true;
            ProjectData.Main.FileDataWasChanged = false;

            //MessageBox.Show("THSelectedSourceType=" + THSelectedSourceType);

            if (ProjectData.CurrentProject != null)
            {
                ProjectData.CurrentProject.BakCreate();
                ProjectData.SaveFileMode = true;
                await Task.Run(() => ProjectData.CurrentProject.Save()).ConfigureAwait(true);
            }
            else
            {
                if (RPGMFunctions.THSelectedSourceType == new HowToMakeTrueSlavesRiseofaDarkEmpire().Name())
                {
                    await Task.Run(() => new HowToMakeTrueSlavesRiseofaDarkEmpire().Save()).ConfigureAwait(true);
                }
                else if (RPGMFunctions.THSelectedSourceType == "RubyRPGGame")
                {
                    new RJ263914OLD().ProceedRubyRPGGame(ProjectData.SelectedGameDir, true);
                    //MessageBox.Show("Finished");
                }
                else if (RPGMFunctions.THSelectedSourceType == "Wolf RPG txt")
                {
                    new WRPGOLDOpen().ProceedWriteWolfRPGtxt();
                    //MessageBox.Show("Finished");
                }
                else if (RPGMFunctions.THSelectedSourceType == "WOLF TRANS PATCH")
                {
                    new WRPGOLDOpen().WriteWOLFTRANSPATCH();
                    //MessageBox.Show("Finished");
                }
                else if (RPGMFunctions.THSelectedSourceType == "RPGMakerTransPatch" || RPGMFunctions.THSelectedSourceType == "RPG Maker game with RPGMTransPatch")
                {
                    //THActionProgressBar.Visible = true;
                    //THInfolabel.Visible = true;
                    //THInfolabel.Text = "saving..";
                    ProjectData.Main.ProgressInfo(true);

                    //THInfoTextBox.Text = "Saving...";

                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    //Thread save = new Thread(new ParameterizedThreadStart((obj) => SaveRPGMTransPatchFiles(ProjectData.SelectedDir, THRPGMTransPatchver)));
                    //save.Start();

                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => new RPGMTransOLD().SaveRPGMTransPatchFiles(ProjectData.SelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);

                    //MessageBox.Show("ProjectData.SelectedDir=" + ProjectData.SelectedDir);
                    //SaveRPGMTransPatchFiles(ProjectData.SelectedDir, THRPGMTransPatchver);

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
                        string rpgmakertranscliargs = "\"" + ProjectData.SelectedGameDir + "\" -p \"" + ProjectData.SelectedDir + "\"" + " -o \"" + ProjectData.SelectedDir.Remove(ProjectData.SelectedDir.Length - "_patch".Length, "_patch".Length) + "_translated\"";

                        if (FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs))
                        {
                        }
                        else
                        {
                            FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs + " -b");// попытка с параметром -b
                        }
                    }

                    ProjectData.Main.SaveInAction = false;
                }
                else if (RPGMFunctions.THSelectedSourceType == "RPG Maker MV json")
                {
                    ///*THMsg*/MessageBox.Show(ProjectData.SelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                    await Task.Run(() => new RPGMMVOLD().WriteJson(ProjectData.Main.THFilesList.Items[0] + string.Empty, ProjectData.SelectedDir + "\\" + ProjectData.Main.THFilesList.Items[0] + ".json")).ConfigureAwait(true);
                }
                else if (RPGMFunctions.THSelectedSourceType == "RPG Maker MV")
                {
                    for (int f = 0; f < ProjectData.Main.THFilesList.Items.Count; f++)
                    {
                        //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                        //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                        bool changed = false;
                        for (int r = 0; r < ProjectData.THFilesElementsDataset.Tables[f].Rows.Count; r++)
                        {
                            if ((ProjectData.THFilesElementsDataset.Tables[f].Rows[r]["Translation"] + string.Empty).Length == 0)
                            {
                            }
                            else
                            {
                                changed = true;
                                break;
                            }
                        }
                        ///*THMsg*/MessageBox.Show(ProjectData.SelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                        if (changed)
                        {

                            ///*THMsg*/MessageBox.Show("start writing");
                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            await Task.Run(() => new RPGMMVOLD().WriteJson(ProjectData.Main.THFilesList.Items[f] + string.Empty, ProjectData.SelectedDir + "\\www\\data\\" + ProjectData.Main.THFilesList.Items[f] + ".json")).ConfigureAwait(true);
                            //WriteJson(THFilesListBox.Items[f].ToString(), ProjectData.SelectedDir + "\\www\\data\\" + THFilesListBox.Items[f].ToString() + ".json");
                        }
                    }
                    /*THMsg*/
                    //MessageBox.Show(T._("finished") + "!");
                }
                else if (RPGMFunctions.THSelectedSourceType == "KiriKiri scenario")
                {
                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    //await Task.Run(() => KiriKiriScenarioWrite(ProjectData.SelectedDir + "\\" + THFilesList.Items[0] + ".scn"));
                    await Task.Run(() => new KiriKiriOLD().KiriKiriScriptScenarioWrite(ProjectData.SelectedDir + "\\" + ProjectData.Main.THFilesList.Items[0] + ".scn")).ConfigureAwait(true);
                }
                else if (RPGMFunctions.THSelectedSourceType == "KiriKiri script")
                {
                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => new KiriKiriOLD().KiriKiriScriptScenarioWrite(ProjectData.SelectedDir + "\\" + ProjectData.Main.THFilesList.Items[0] + ".ks")).ConfigureAwait(true);
                }
            }


            ProjectData.Main.SaveInAction = false;
            FunctionsSounds.PlayAsterisk();
        }
    }
}
