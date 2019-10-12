using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TranslationHelper.Main.Functions;

namespace TranslationHelper
{
    public partial class THMain : Form
    {
        //string THLog;
        //public IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");
        public THSettings Settings;
        //public const string THStrDGTranslationColumnName = "Translation";
        //public const string THStrDGOriginalColumnName = "Original";
        //private readonly THLang LangF;

        //public readonly static string apppath = Application.StartupPath;
        //о разнице между "" и string.Empty и использовании string.lenght==0 вместо ==string.Empty
        //https://stackoverflow.com/a/7872957
        private string extractedpatchpath = string.Empty;

        private string FVariant = string.Empty;
        //private BindingList<THRPGMTransPatchFile> THRPGMTransPatchFiles; //Все файлы
        //DataTable fileslistdt = new DataTable();
        public DataSet THFilesElementsDataset;
        public DataTable THFilesElementsALLDataTable;
        public DataSet THFilesElementsDatasetInfo;
        //DataTable THFilesElementsDatatable;
        //private BindingSource THBS = new BindingSource();

        public string THSelectedDir;
        public string THSelectedGameDir;
        public string THRPGMTransPatchver;
        public string THSelectedSourceType;

        //Language strings
        //public string THMainDGVOriginalColumnName;
        //public string THMainDGVTranslationColumnName;

        //про primary key взял отсюда: https://stackoverflow.com/questions/3567552/table-doesnt-have-a-primary-key
        readonly DataColumn[] keyColumns = new DataColumn[1];

        //Translation cache
        //DataSet THTranslationCache;
        public static string THTranslationCachePath;
        
        public THMain()
        {
            InitializeComponent();
            //LangF = new THLang();

            SetSettings();

            SetUIStrings();

            //LangF.THReadLanguageFileToStrings();

            THFilesElementsDataset = new DataSet();
            THFilesElementsALLDataTable = new DataTable();
            THFilesElementsDatasetInfo = new DataSet();
            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            THFilesList.DrawMode = DrawMode.OwnerDrawFixed;

            //DataSet THTranslationCache; THTranslationCache = new DataSet();
            THTranslationCachePath = Path.Combine(Application.StartupPath,"DB","THTranslationCache.cmx");

            //THRPGMTransPatchFiles = new BindingList<THRPGMTransPatchFile>();
            //dt = new DataTable();

            //THFileElementsDataGridView set doublebuffered to true
            SetDoublebuffered(true);
            if (File.Exists(Path.Combine(Application.StartupPath,"TranslationHelper.log")))
            {
                File.Delete(Path.Combine(Application.StartupPath, "TranslationHelper.log"));
            }

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        //Settings
        public bool IsTranslationCacheEnabled = true;
        public string WebTranslationLink = string.Empty;
        public bool DontLoadStringIfRomajiPercent = true;
        public int DontLoadStringIfRomajiPercentNum = 90;
        public bool AutotranslationForSimular = true;
        public bool IsFullComprasionDBloadEnabled = true;
        private void SetSettings()
        {
            Settings = new THSettings(this);
            IsTranslationCacheEnabled = Settings.EnableTranslationCacheINI;
            WebTranslationLink = Settings.WebTransLinkINI;
            DontLoadStringIfRomajiPercent = Settings.DontLoadStringIfRomajiPercentINI;
            DontLoadStringIfRomajiPercentNum = Settings.DontLoadStringIfRomajiPercentNumINI;
            AutotranslationForSimular = Settings.AutotranslationForIdenticalINI;
            IsFullComprasionDBloadEnabled = Settings.FullComprasionDBloadINI;
            Settings.Dispose();
        }

        private void SetUIStrings()
        {
            //language strings setup
            //THMainDGVOriginalColumnName = LangF.THStrDGOriginalColumnName;
            //THMainDGVTranslationColumnName = LangF.THStrDGTranslationColumnName;
            //Menu File
            this.fileToolStripMenuItem.Text = T._("File");
            this.openToolStripMenuItem.Text = T._("Open");
            this.saveToolStripMenuItem.Text = T._("Save");
            this.saveAsToolStripMenuItem.Text = T._("Save As");
            this.writeTranslationInGameToolStripMenuItem.Text = T._("Write translation");
            this.saveTranslationToolStripMenuItem.Text = T._("Save translation");
            this.saveTranslationAsToolStripMenuItem.Text = T._("Save Translation as");
            this.loadTranslationToolStripMenuItem.Text = T._("Load Translation");
            this.loadTrasnlationAsToolStripMenuItem.Text = T._("Load Translation as");
            this.runTestGameToolStripMenuItem.Text = T._("Run Test RPGMaker MV Game");
            //Menu Edit
            this.editToolStripMenuItem.Text = T._("Edit");
            this.openInWebToolStripMenuItem.Text = T._("Open in Web");
            this.tryToTranslateOnlineToolStripMenuItem.Text = T._("Translate Online");
            this.selectedToolStripMenuItem1.Text = T._("Selected");
            this.tableToolStripMenuItem1.Text = T._("Table");
            this.allToolStripMenuItem1.Text = T._("All");
            this.translationInteruptToolStripMenuItem.Text = T._("Interupt");
            this.fixCellSpecialSymbolsToolStripMenuItem.Text = T._("Fix cell special symbols");
            this.fixCellsSelectedToolStripMenuItem.Text = T._("Selected");
            this.fixCellsTableToolStripMenuItem.Text = T._("Table");
            this.allToolStripMenuItem.Text = T._("All");
            this.setOriginalValueToTranslationToolStripMenuItem.Text = T._("Translation=Original");
            this.completeRomajiotherLinesToolStripMenuItem.Text = T._("Complete Romaji/Other lines");
            this.completeRomajiotherLinesToolStripMenuItem1.Text = T._("Complete Romaji/Other lines");
            this.forceSameForSimularToolStripMenuItem.Text = T._("Force same for simular");
            this.forceSameForSimularToolStripMenuItem1.Text = T._("Force same for simular");
            this.cutToolStripMenuItem1.Text = T._("Cut");
            this.copyCellValuesToolStripMenuItem.Text = T._("Copy");
            this.pasteCellValuesToolStripMenuItem.Text = T._("Paste");
            this.clearSelectedCellsToolStripMenuItem.Text = T._("Clear selected cells");
            this.toUPPERCASEToolStripMenuItem.Text = T._("UPPERCASE");
            this.firstCharacterToUppercaseToolStripMenuItem.Text = T._("Uppercase");
            this.toLOWERCASEToolStripMenuItem.Text = T._("lowercase");
            this.searchToolStripMenuItem.Text = T._("Search");
            //Menu View
            this.viewToolStripMenuItem.Text = T._("View");
            this.setColumnSortingToolStripMenuItem.Text = T._("Reset column sorting");
            //Menu Options
            this.optionsToolStripMenuItem.Text = T._("Options");
            this.settingsToolStripMenuItem.Text = T._("Settings");
            //Menu Help
            this.helpToolStripMenuItem.Text = T._("Help");
            this.aboutToolStripMenuItem.Text = T._("About");
            //Contex menu
            this.OpenInWebContextToolStripMenuItem.Text = T._("Open in web");
            this.toolStripMenuItem6.Text = T._("Translate Online");
            this.TranslateSelectedContextToolStripMenuItem.Text = T._("Selected");
            this.TranslateTableContextToolStripMenuItem.Text = T._("Table");
            this.toolStripMenuItem9.Text = T._("All");
            this.translationInteruptToolStripMenuItem1.Text = T._("Interupt");
            this.toolStripMenuItem2.Text = T._("Fix cell special symbols");
            this.fixSymbolsContextToolStripMenuItem.Text = T._("Selected");
            this.fixSymbolsTableContextToolStripMenuItem.Text = T._("Table");
            this.toolStripMenuItem5.Text = T._("All");
            this.OriginalToTransalationContextToolStripMenuItem.Text = T._("Translation=Original");
            this.CutToolStripMenuItem.Text = T._("Cut");
            this.CopyToolStripMenuItem.Text = T._("Copy");
            this.pasteToolStripMenuItem.Text = T._("Paste");
            this.toolStripMenuItem11.Text = T._("Clear selected cells");
            this.toolStripMenuItem14.Text = T._("UPPERCASE");
            this.uppercaseToolStripMenuItem.Text = T._("Uppercase");
            this.lowercaseToolStripMenuItem.Text = T._("lowercase");
        }

        private void THMain_Load(object sender, EventArgs e)
        {
            SetTooltips();            
        }

        private void SetTooltips()
        {
            //http://qaru.site/questions/47162/c-how-do-i-add-a-tooltip-to-a-control
            //THMainResetTableButton
            ToolTip THToolTip = new ToolTip
            {

                // Set up the delays for the ToolTip.
                AutoPopDelay = 10000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                UseAnimation = true,
                UseFading = true,
                // Force the ToolTip text to be displayed whether or not the form is active.
                ShowAlways = true
            };

            //Main
            THToolTip.SetToolTip(THMainResetTableButton, T._("Resets filters and tab sorting"));
            THToolTip.SetToolTip(THFiltersDataGridView, T._("Filters for columns of main table"));
            ////////////////////////////
        }

        readonly bool THdebug = true;
        StringBuilder THsbLog = new StringBuilder();
        public void LogToFile(string s, bool w = false)
        {
            if (THdebug)
            {
                if (w)
                {
                    if (THsbLog.Length == 0)
                    {
                        FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", DateTime.Now + " >>" + s + Environment.NewLine, true);
                    }
                    else
                    {
                        FileWriter.WriteData(Path.Combine(Application.StartupPath,"TranslationHelper.log"), DateTime.Now + " >>" + THsbLog + Environment.NewLine, true);
                        //File.Move(Application.StartupPath + "\\TranslationHelper.log", Application.StartupPath + "\\TranslationHelper" + DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss") + ".log");
                        THsbLog.Clear();
                    }
                }
                else
                {
                    THsbLog.Append(DateTime.Now + " >>" + s + Environment.NewLine);
                }
            }
        }

        public static void StartLoadingForm()
        {
            try
            {
                Application.Run(new THLoading());
            }
            catch (ThreadAbortException)
            {
            }
        }

        bool IsOpeningInProcess = false;
        private async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                IsOpeningInProcess = true;

                //об сообщении Освобождаемый объект никогда не освобождается и почему using здесь
                //https://stackoverflow.com/questions/2926869/do-you-need-to-dispose-of-objects-and-set-them-to-null
                using (OpenFileDialog THFOpen = new OpenFileDialog())
                {
                    THFOpen.InitialDirectory = Settings.THConfigINI.ReadINI("Paths","LastPath");
                    THFOpen.Filter = "All compatible|*.exe;RPGMKTRANSPATCH;*.json;*.scn;*.ks|RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker execute(*.exe)|*.exe|KiriKiri engine files|*.scn;*.ks";

                    if (THFOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (THFOpen.OpenFile() != null)
                        {
                            //THActionProgressBar.Visible = true;
                            ProgressInfo(true, T._("opening.."));

                            THCleanupThings();

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            //Thread open = new Thread(new ParameterizedThreadStart((obj) => GetSourceType(THFOpen.FileName)));
                            //open.Start();

                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            await Task.Run(() => THSelectedSourceType = GetSourceType(THFOpen.FileName));

                            //THSelectedSourceType = GetSourceType(THFOpen.FileName);

                            //THActionProgressBar.Visible = false;
                            ProgressInfo(false, string.Empty);

                            if (THSelectedSourceType.Length == 0)
                            {
                                THMsg.Show(T._("Problem with source opening. Try to report to devs about it."));
                            }
                            else
                            {
                                //if (THSelectedSourceType == "RPG Maker MV")
                                //{
                                //    THMakeRPGMakerMVWorkProjectDir(THFOpen.FileName);
                                //}
                                
                                //Попытка добавить открытие сразу всех таблиц в одной
                                if (setAsDatasourceAllToolStripMenuItem.Visible)
                                {
                                    for (int c = 0; c < THFilesElementsDataset.Tables[0].Columns.Count; c++)
                                    {
                                        THFilesElementsALLDataTable.Columns.Add(THFilesElementsDataset.Tables[0].Columns[c].ColumnName);//asdfgh
                                    }

                                    for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
                                    {
                                        for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
                                        {
                                            THFilesElementsALLDataTable.Rows.Add(THFilesElementsDataset.Tables[t].Rows[r].ItemArray);
                                        }
                                    }
                                }

                                if (THSelectedSourceType.Contains("RPG Maker game with RPGMTransPatch") || THSelectedSourceType.Contains("KiriKiri game"))
                                {
                                    Settings.THConfigINI.WriteINI("Paths", "LastPath", THSelectedGameDir);
                                }
                                else
                                {
                                    Settings.THConfigINI.WriteINI("Paths", "LastPath", THSelectedDir);
                                }
                                _ = THMsg.Show(THSelectedSourceType + T._(" loaded") + "!");

                                editToolStripMenuItem.Enabled = true;
                                viewToolStripMenuItem.Enabled = true;
                                loadTranslationToolStripMenuItem.Enabled = true;
                                loadTrasnlationAsToolStripMenuItem.Enabled = true;
                                runTestGameToolStripMenuItem.Enabled = true;
                                runTestGameToolStripMenuItem.Visible = true;

                                if (FVariant.Length == 0)
                                {
                                    FVariant = " * "+THSelectedSourceType;
                                }
                                try
                                {
                                    ActiveForm.Text += FVariant;
                                }
                                catch
                                {
                                }
                            }

                        }
                    }
                }

                IsOpeningInProcess = false;
            }
        }

        private void THCleanupThings()
        {
            try
            {

                //Reset vars
                ActiveForm.Text = "Translation Helper by DenisK";
                THInfoTextBox.Text = string.Empty;
                THSourceRichTextBox.Text = string.Empty;
                THTargetRichTextBox.Text = string.Empty;
                TableCompleteInfoLabel.Text = string.Empty;
                ControlsSwitchActivated = false;

                //Clean data
                THFilesList.Items.Clear();
                THFilesElementsDataset.Reset();
                THFilesElementsALLDataTable.Reset();
                THFilesElementsDatasetInfo.Reset();
                THFileElementsDataGridView.Columns.Clear();
                //THFileElementsDataGridView.Rows.Clear();

                //Dispose objects
                THFilesElementsDataset.Dispose();
                THFilesElementsALLDataTable.Dispose();
                THFilesElementsDatasetInfo.Dispose();

                //Disable items
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                editToolStripMenuItem.Enabled = false;
                viewToolStripMenuItem.Enabled = false;
                saveTranslationToolStripMenuItem.Enabled = false;
                writeTranslationInGameToolStripMenuItem.Enabled = false;
                loadTranslationToolStripMenuItem.Enabled = false;
                loadTrasnlationAsToolStripMenuItem.Enabled = false;
                saveTranslationAsToolStripMenuItem.Enabled = false;
                THSourceRichTextBox.Enabled = false;
                THTargetRichTextBox.Enabled = false;
                openInWebToolStripMenuItem.Enabled = false;
                selectedToolStripMenuItem1.Enabled = false;
                tableToolStripMenuItem1.Enabled = false;
                fixCellsSelectedToolStripMenuItem.Enabled = false;
                fixCellsTableToolStripMenuItem.Enabled = false;
                setOriginalValueToTranslationToolStripMenuItem.Enabled = false;
                completeRomajiotherLinesToolStripMenuItem.Enabled = false;
                completeRomajiotherLinesToolStripMenuItem1.Enabled = false;
                forceSameForSimularToolStripMenuItem.Enabled = false;
                forceSameForSimularToolStripMenuItem1.Enabled = false;
                cutToolStripMenuItem1.Enabled = false;
                copyCellValuesToolStripMenuItem.Enabled = false;
                pasteCellValuesToolStripMenuItem.Enabled = false;
                clearSelectedCellsToolStripMenuItem.Enabled = false;
                toUPPERCASEToolStripMenuItem.Enabled = false;
                firstCharacterToUppercaseToolStripMenuItem.Enabled = false;
                toLOWERCASEToolStripMenuItem.Enabled = false;
                setColumnSortingToolStripMenuItem.Enabled = false;
                OpenInWebContextToolStripMenuItem.Enabled = false;
                TranslateSelectedContextToolStripMenuItem.Enabled = false;
                TranslateTableContextToolStripMenuItem.Enabled = false;
                fixSymbolsContextToolStripMenuItem.Enabled = false;
                fixSymbolsTableContextToolStripMenuItem.Enabled = false;
                OriginalToTransalationContextToolStripMenuItem.Enabled = false;
                CutToolStripMenuItem.Enabled = false;
                CopyToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                toolStripMenuItem11.Enabled = false;
                toolStripMenuItem14.Enabled = false;
                uppercaseToolStripMenuItem.Enabled = false;
                lowercaseToolStripMenuItem.Enabled = false;

                runTestGameToolStripMenuItem.Enabled = false;

                //reset vars
                istpptransfile = false;
            }
            catch
            {

            }
        }

        public DirectoryInfo mvdatadir;
        public string THWorkProjectDir;
        bool istpptransfile = false;
        private string GetSourceType(string sPath)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            THSelectedDir = dir + string.Empty;
            THSelectedGameDir = dir + string.Empty;
            //MessageBox.Show("sPath=" + sPath);
            if (sPath.ToLower().EndsWith(".ks") || sPath.ToLower().EndsWith(".scn"))
            {
                return KiriKiriScriptScenario(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToLower().EndsWith(".tjs"))
            {
                return KiriKiriScenarioOpen(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToLower().EndsWith(".scn"))
            {
                return KiriKiriScenarioOpen(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToUpper().Contains("\\RPGMKTRANSPATCH"))
            {
                return RPGMTransPatchPrepare(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToLower().Contains(".transSSSSSSS"))
            {
                istpptransfile = true;
                if (OpentppTransFile(sPath))
                {
                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    }
                    return "T++ trans";
                }
            }
            else if (sPath.ToLower().Contains(".json"))
            {
                if (OpenRPGMakerMVjson(sPath))
                {
                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    }
                    return "RPG Maker MV json";
                }
            }
            else if (Path.GetExtension(sPath) == ".exe" /*sPath.ToLower().Contains("\\game.exe") || dir.GetFiles("*.exe").Length > 0*/)
            {
                if ((GetExeDescription(sPath) != null && GetExeDescription(sPath).ToUpper().Contains("KIRIKIRI")) && dir.GetFiles("*.xp3").Length > 0)
                {
                    if (KiriKiriGame(sPath))
                    {
                        return "KiriKiri game";
                    }
                }
                else if (File.Exists(Path.Combine(THSelectedDir,"www","data","system.json")))
                {
                    try
                    {
                        //THSelectedDir += "\\www\\data";
                        //var MVJsonFIles = new List<string>();
                        mvdatadir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(THSelectedDir,"www","data/")));
                        foreach (FileInfo file in mvdatadir.GetFiles("*.json"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            //MVJsonFIles.Add(file.FullName);

                            if (OpenRPGMakerMVjson(file.FullName))
                            {
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }

                        for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                        {
                            //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);
                            THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
                        }

                        return "RPG Maker MV";
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                else if (File.Exists(Path.Combine(dir.FullName, "Data", "System.rvdata2")) || dir.GetFiles("*.rgss3a").Length > 0 || dir.GetFiles("*.rgss2a").Length > 0 || dir.GetFiles("*.rvdata").Length > 0 || dir.GetFiles("*.rgssad").Length > 0 || dir.GetFiles("*.rxdata").Length > 0 || dir.GetFiles("*.lmt").Length > 0 || dir.GetFiles("*.lmu").Length > 0)
                {

                    extractedpatchpath = string.Empty;
                    bool result = TryToExtractToRPGMakerTransPatch(sPath);
                    //MessageBox.Show("result=" + result);
                    //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                    if (result)
                    {
                        //Cleaning of the type
                        //THRPGMTransPatchFiles.Clear();
                        //THFilesElementsDataset.Clear();

                        //var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
                        //string patchver;
                        bool isv3 = Directory.Exists(extractedpatchpath + "\\patch");
                        //MessageBox.Show("isv3=" + isv3+ ", patchdir="+ extractedpatchpath+ ", extractedpatchpath="+ extractedpatchpath);
                        if (isv3) //если есть подпапка patch, тогда это версия патча 3
                        {
                            THRPGMTransPatchver = "3";
                            extractedpatchpath += "\\patch";
                            //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                            dir = new DirectoryInfo(Path.GetDirectoryName(extractedpatchpath + "\\")); //Два слеша здесь в конце исправляют проблему возврата информации о неверной папке
                                                                                                       //MessageBox.Show("patchdir1=" + patchdir);
                        }
                        else if (Directory.Exists(extractedpatchpath + Path.GetFileName(extractedpatchpath) + "\\patch"))
                        {
                            THRPGMTransPatchver = "3";
                            extractedpatchpath += Path.GetFileName(extractedpatchpath) + "\\patch";
                            dir = new DirectoryInfo(Path.GetDirectoryName(extractedpatchpath + "\\"));
                        }
                        else //иначе это версия 2
                        {
                            THRPGMTransPatchver = "2";
                        }
                        //MessageBox.Show("patchdir2=" + patchdir);

                        var vRPGMTransPatchFiles = new List<string>();

                        foreach (FileInfo file in (new DirectoryInfo(extractedpatchpath)).GetFiles("*.txt"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            vRPGMTransPatchFiles.Add(file.FullName);
                        }

                        //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

                        //THFilesDataGridView.Nodes.Add("main");
                        //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
                        //RPGMTransPatch.OpenTransFiles(files, patchver);
                        //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
                        if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, THRPGMTransPatchver, THFilesElementsDataset, THFilesElementsDatasetInfo))
                        {

                            //Запись в dataGridVivwer
                            for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                            {
                                //MessageBox.Show("ListFiles=" + ListFiles[i]);
                                //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                                //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                                THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
                                //THFilesDataGridView.Rows.Add();
                                //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                                //dGFiles.Rows.Add();
                                //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                            }

                            THSelectedGameDir = THSelectedDir;
                            THSelectedDir = extractedpatchpath.Replace("\\patch", string.Empty);
                            //MessageBox.Show(THSelectedSourceType + " loaded!");
                            //ProgressInfo(false, string.Empty);
                            return "RPG Maker game with RPGMTransPatch";
                        }
                    }
                }

            }

            //MessageBox.Show("Uncompatible source or problem with opening.");
            return string.Empty;
        }

        private string KiriKiriScriptScenario(string sPath)
        {
            string filename = Path.GetFileNameWithoutExtension(sPath);
            string extension = Path.GetExtension(sPath);

            _ = THFilesElementsDataset.Tables.Add(filename);
            _ = THFilesElementsDataset.Tables[0].Columns.Add("Original");
            _ = THFilesElementsDatasetInfo.Tables.Add(filename);
            _ = THFilesElementsDatasetInfo.Tables[0].Columns.Add("Original");

            DataTable DT = KiriKiriScriptScenarioOpen(sPath, THFilesElementsDataset.Tables[0], THFilesElementsDatasetInfo.Tables[0]);
            if (DT == null || DT.Rows.Count == 0)
            {
                THFilesElementsDataset.Tables.Remove(filename);
                THFilesElementsDatasetInfo.Tables.Remove(filename);
            }
            else
            {
                _ = THFilesElementsDataset.Tables[0].Columns.Add("Translation");
                THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                if (extension == ".ks")
                {
                    return "KiriKiri script";
                }
                else if (extension == ".scn")
                {
                    return "KiriKiri script";
                }
            }
            return string.Empty;
        }

        private bool KiriKiriGame(string sPath)
        {
            bool ret = false;
            if (ExtractXP3files(sPath))
            {
                var KiriKiriFiles = new List<string>();
                string DirName = Path.GetFileName(Path.GetDirectoryName(sPath));
                string KiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", DirName);

                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.scn", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.ks", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.csv", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.tsv", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.tjs", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                //foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*", SearchOption.AllDirectories))
                //{
                //    if (file.Extension == ".ks" || file.Extension == ".scn")
                //    {
                //    }
                //    else
                //    {
                //        File.Delete(file.FullName);
                //    }
                //}

                if (KiriKiriGameOpen(KiriKiriFiles))
                {
                    return true;
                }
            }
            return ret;
        }

        private bool KiriKiriGameOpen(List<string> kiriKiriFiles)
        {
            bool ret = false;
            string filename;

            try
            {
                for (int i = 0; i < kiriKiriFiles.Count; i++)
                {
                    filename = Path.GetFileName(kiriKiriFiles[i]);

                    _ = THFilesElementsDataset.Tables.Add(filename);
                    _ = THFilesElementsDataset.Tables[filename].Columns.Add("Original");
                    _ = THFilesElementsDatasetInfo.Tables.Add(filename);
                    _ = THFilesElementsDatasetInfo.Tables[filename].Columns.Add("Original");

                    DataTable DT=null;
                    if (filename.EndsWith(".ks") || filename.EndsWith(".scn") || filename.EndsWith(".tjs"))
                    {
                        DT = KiriKiriScriptScenarioOpen(kiriKiriFiles[i], THFilesElementsDataset.Tables[filename], THFilesElementsDatasetInfo.Tables[filename]);
                    }
                    else if (filename.EndsWith(".csv"))
                    {
                        DT = KiriKiriCSVOpen(kiriKiriFiles[i], THFilesElementsDataset.Tables[filename], THFilesElementsDatasetInfo.Tables[filename]);
                    }
                    else if (filename.EndsWith(".tsv"))
                    {
                        DT = KiriKiriTSVOpen(kiriKiriFiles[i], THFilesElementsDataset.Tables[filename], THFilesElementsDatasetInfo.Tables[filename]);
                    }

                    if (DT == null || DT.Rows.Count == 0)
                    {
                        THFilesElementsDataset.Tables.Remove(filename);
                        THFilesElementsDatasetInfo.Tables.Remove(filename);
                    }
                    else
                    {
                        THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                        _ = THFilesElementsDataset.Tables[filename].Columns.Add("Translation");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ret;
        }

        private DataTable KiriKiriTSVOpen(string sPath, DataTable DT, DataTable DTInfo)
        {
            using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
            {
                string line;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    if (line.StartsWith(";") || string.IsNullOrWhiteSpace(line))
                    {
                    }
                    else
                    {
                        string[] NameValues = line.Split('	');

                        if (NameValues.Length == 2)
                        {
                            string[] Values = NameValues[1].Split(',');

                            int ValuesLength = Values.Length;
                            for (int l=0;l < ValuesLength; l++)
                            {
                                string subline = Values[l];
                                if (string.IsNullOrEmpty(subline) || subline=="true" || subline=="false" || subline.StartsWith("0x") || IsDigitsOnly(subline))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(subline);
                                    _ = DTInfo.Rows.Add(NameValues[0]);
                                }
                            }
                        }
                    }
                }
            }

            return DT;
        }

        private DataTable KiriKiriCSVOpen(string sPath, DataTable DT, DataTable DTInfo)
        {
            using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
            {
                string line;
                //string original = string.Empty;
                //_ = THFilesElementsDataset.Tables.Add(filename);
                //_ = THFilesElementsDataset.Tables[0].Columns.Add("Original");
                //_ = THFilesElementsDatasetInfo.Tables.Add(filename);
                //_ = THFilesElementsDatasetInfo.Tables[0].Columns.Add("Original");
                //_ = THFilesElementsDataset.Tables[0].Columns.Add("Translation");
                //THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                bool IsFirstLineWasNotRead = true;
                int name = -1;
                int detail = -1;
                int type = -1;
                int field = -1;
                int comment = -1;
                string[] columns;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();

                    if (line.StartsWith(";") || string.IsNullOrWhiteSpace(line))
                    {
                    }
                    else
                    {
                        columns = line.Split(new string[] { "	" }, StringSplitOptions.None);
                        if (IsFirstLineWasNotRead)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (columns[i]== "name")
                                {
                                    name = i;
                                }
                                else if(columns[i] == "detail")
                                {
                                    detail = i;
                                }
                                else if(columns[i] == "type")
                                {
                                    type = i;
                                }
                                else if(columns[i] == "field")
                                {
                                    field = i;
                                }
                                else if(columns[i] == "comment")
                                {
                                    comment = i;
                                }
                            }
                            IsFirstLineWasNotRead = false;
                        }
                        else
                        {
                            if (name > -1)
                            {
                                if (string.IsNullOrEmpty(columns[name]) || IsDigitsOnly(columns[name]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[name]);
                                    _ = DTInfo.Rows.Add("name");
                                }
                            }
                            if (detail > -1)
                            {
                                if (string.IsNullOrEmpty(columns[detail]) || IsDigitsOnly(columns[detail]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[detail]);
                                    _ = DTInfo.Rows.Add("detail");
                                }
                            }
                            if (type > -1)
                            {
                                if (string.IsNullOrEmpty(columns[type]) || IsDigitsOnly(columns[type]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[type]);
                                    _ = DTInfo.Rows.Add("type");
                                }
                            }
                            if (field > -1)
                            {
                                if (string.IsNullOrEmpty(columns[field]) || IsDigitsOnly(columns[field]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[field]);
                                    _ = DTInfo.Rows.Add("field");
                                }
                            }
                            if (comment > -1)
                            {
                                if (string.IsNullOrEmpty(columns[comment]) || IsDigitsOnly(columns[comment]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[comment]);
                                    _ = DTInfo.Rows.Add("comment");
                                }
                            }
                        }
                    }
                }
            }

            return DT;
        }

        static bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            str = str.Replace(".", string.Empty);
            //https://stackoverflow.com/questions/7461080/fastest-way-to-check-if-string-contains-only-digits
            //наибыстрейший метод 
            int strLength = str.Length;//и моя оптимизация, ускоряющая с 2.19 до 1.62 при 100млн. итераций
            for (int i = 0; i < strLength; i++)
            {
                if ((str[i] ^ '0') > 9)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ExtractXP3files(string sPath)
        {
            bool ret = false;

            try
            {
                string KiriKiriEXEpath = Path.Combine(Application.StartupPath, "Res", "kirikiriunpacker", "kikiriki.exe");
                string DirName = Path.GetFileName(Path.GetDirectoryName(sPath));
                string KiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", DirName);
                DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(sPath)+"\\");
                string xp3name = "data";
                string xp3path = Path.Combine(directory.FullName, xp3name + ".xp3");
                string KiriKiriEXEargs = "-i \"" + xp3path + "\" -o \"" + KiriKiriWorkFolder + "\"";

                if (Directory.Exists(KiriKiriWorkFolder))
                {
                    if ((new DirectoryInfo(KiriKiriWorkFolder + "\\")).GetFiles("*", SearchOption.AllDirectories).Length > 0)
                    {
                        DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            ret = true;
                        }
                        else
                        {
                            //Удаление и пересоздание папки
                            Directory.Delete(KiriKiriWorkFolder, true);
                            Directory.CreateDirectory(KiriKiriWorkFolder);

                            ret = RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);
                            //if (RunProcess(KiriKiriEXEpath, KiriKiriEXEargs))
                            //{
                            //    xp3name = "patch";
                            //    xp3path = Path.Combine(directory.FullName, xp3name + ".xp3");
                            //    ret = RunProcess(KiriKiriEXEpath, KiriKiriEXEargs);
                            //}
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(KiriKiriWorkFolder);
                }                
            }
            catch
            {

            }

            return ret;
        }

        private bool RunProcess(string ProgramPath, string Arguments)
        {
            bool ret = false;
            if (File.Exists(ProgramPath))
            {
                using (Process Program = new Process())
                {
                    //MessageBox.Show("outdir=" + outdir);
                    Program.StartInfo.FileName = ProgramPath;
                    if (Arguments.Length > 0)
                    {
                        Program.StartInfo.Arguments = Arguments;
                    }
                    Program.StartInfo.WorkingDirectory = Path.GetDirectoryName(ProgramPath);

                    //http://www.cyberforum.ru/windows-forms/thread31052.html
                    // свернуть
                    //WindowState = FormWindowState.Minimized;
                    //if (LinksForm == null || LinksForm.IsDisposed)
                    //{
                    //}
                    //else
                    //{
                    //    LinksForm.WindowState = FormWindowState.Minimized;
                    //}

                    ret = Program.Start();
                    Program.WaitForExit();

                    // Показать
                    //WindowState = FormWindowState.Normal;
                    //if (LinksForm == null || LinksForm.IsDisposed)
                    //{
                    //}
                    //else
                    //{
                    //    LinksForm.WindowState = FormWindowState.Normal;
                    //}
                }
            }

            return ret;
        }

        private string GetExeDescription(string exepath)
        {
            if (exepath.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                FileVersionInfo ExeInfo = FileVersionInfo.GetVersionInfo(exepath);
                return ExeInfo.FileDescription;
            }
        }

        string KiriKiriVariableSearchRegexPattern = string.Empty;
        string KiriKiriVariableSearchRegexFullPattern = string.Empty;
        string KiriKiriQuotePattern = string.Empty;
        private DataTable KiriKiriScriptScenarioOpen(string sPath, DataTable DT = null, DataTable DTInfo = null)
        {
            try
            {
                //ks scn tjs open
                KiriKiriVariableSearchRegexPattern = @"( |'|\(|\[|,|\.)o\.";
                KiriKiriVariableSearchRegexFullPattern = @"((^o\.)|(" + KiriKiriVariableSearchRegexPattern + @"))([^\.|\s|'|\)|\]|;|:|,]+)";
                string Quote1 = "\"";
                string Quote2 = "\'";
                KiriKiriQuotePattern = Quote1+"(.+)"+ Quote1+";$";
                using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    bool iscomment = false;
                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith("/*"))
                        {
                            iscomment = true;
                        }

                        if (iscomment || string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("//") || Regex.IsMatch(line, @"\s*//"))
                        {
                        }
                        else
                        {
                            if (line.EndsWith("[k]")) // text
                            {
                                line = line.Replace("[k]", string.Empty);
                                if (string.IsNullOrEmpty(line) || IsDigitsOnly(line))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(line);
                                    _ = DTInfo.Rows.Add("[k] = end of line");
                                }
                            }
                            else if (line.StartsWith("*")) // text
                            {
                                line = line.Remove(0, 1);
                                if (string.IsNullOrEmpty(line) || IsDigitsOnly(line))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(line);
                                    _ = DTInfo.Rows.Add(string.Empty);
                                }
                            }
                            else if (line.EndsWith("[r]")) //text, first line
                            {
                                line = line.Replace("[r]", string.Empty).Trim();
                                if (string.IsNullOrEmpty(line) || IsDigitsOnly(line))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(line);
                                    _ = DTInfo.Rows.Add("[r] = carriage return");
                                }
                            }
                            else if (line.StartsWith("o.") || Regex.IsMatch(line, KiriKiriVariableSearchRegexPattern)) //variable, which is using even for displaing and should be translated in all files
                            {
                                MatchCollection matches = Regex.Matches(line, KiriKiriVariableSearchRegexFullPattern);

                                bool startswith = line.StartsWith("o.");
                                for (int m = 0; m < matches.Count; m++)
                                {
                                    _ = DT.Rows.Add(matches[m].Value.Remove(0, startswith ? 2 : 3));
                                    _ = DTInfo.Rows.Add(T._("Variable>Must be Identical in all files>Only A-Za-z0-9" + Environment.NewLine + "line: " + line));
                                    if (startswith)
                                    {
                                        startswith = false;//o. в начале встречается только в первый раз
                                    }
                                }
                            }
                            else if (line.StartsWith("@notice text="))
                            {
                                line = line.Remove(0, 13);//удаление "@notice text="
                                if (string.IsNullOrEmpty(line) || IsDigitsOnly(line))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(line);
                                    _ = DTInfo.Rows.Add("@notice text=");
                                }
                            }
                            else if (line.StartsWith("Name = '"))
                            {
                                line = line.Remove(line.Length-2, 2).Remove(0, 8);
                                if (string.IsNullOrEmpty(line) || IsDigitsOnly(line))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(line);
                                    _ = DTInfo.Rows.Add("Name = '");
                                }
                            }
                            else if (Regex.IsMatch(line, "\\\"(.*?)\\\""))
                            {
                                //https://stackoverflow.com/questions/13024073/regex-c-sharp-extract-text-within-double-quotes
                                var matches = Regex.Matches(line, "\\\"(.*?)\\\"");
                                string subline;
                                foreach (var m in matches)
                                {
                                    subline = m.ToString();
                                    if (string.IsNullOrWhiteSpace(subline.Replace("\"", string.Empty)) || IsDigitsOnly(line.Replace("\"", string.Empty)))
                                    {
                                    }
                                    else
                                    {
                                        _ = DT.Rows.Add(subline.Remove(subline.Length-1,1).Remove(0,1));
                                        _ = DTInfo.Rows.Add(line);
                                    }
                                }
                            }
                        }

                        if (line.EndsWith("*/"))
                        {
                            iscomment = false;
                        }
                    }

                    return DT;
                }
            }
            catch
            {

            }

            return null;
        }

        private string KiriKiriScenarioOpen(string sPath)
        {
            try
            {
                using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    //string original = string.Empty;
                    string filename = Path.GetFileNameWithoutExtension(sPath);
                    _ = THFilesElementsDataset.Tables.Add(filename);
                    _ = THFilesElementsDataset.Tables[0].Columns.Add("Original");
                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith(";") || line.StartsWith("@") || Equals(line, string.Empty))
                        {
                        }
                        else
                        {
                            if (line.EndsWith("[k]"))
                            {
                                THFilesElementsDataset.Tables[0].Rows.Add(line.Remove(line.Length - 3, 3));

                                //int i = 0;
                                //while (line.EndsWith("[k]"))
                                //{
                                //    if (i > 0)
                                //    {
                                //        original += Environment.NewLine;
                                //    }

                                //    original = original + line.Replace("[k]", string.Empty);

                                //    line = file.ReadLine();

                                //    if (line.EndsWith("[k]") && line.StartsWith("["))
                                //    {
                                //        THFilesElementsDataset.Tables[0].Rows.Add(original);
                                //        original = string.Empty;
                                //        i = 0;
                                //    }
                                //    else
                                //    {
                                //        i++;
                                //    }
                                //}
                                //if (original.Length > 0)
                                //{
                                //    THFilesElementsDataset.Tables[0].Rows.Add(original);
                                //    original = string.Empty;
                                //}
                            }
                        }
                    }

                    if (THFilesElementsDataset.Tables[0].Rows.Count > 0)
                    {
                        _ = THFilesElementsDataset.Tables[0].Columns.Add("Translation");
                        THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                    }
                    else
                    {
                        THMsg.Show(T._("Nothing to add"));
                        return string.Empty;
                    }
                }

                return "KiriKiri scenario";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return string.Empty;
        }

        private string RPGMTransPatchPrepare(string sPath)
        {

            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            //MessageBox.Show("THFOpen.FileName=" + THFOpen.FileName);
            //MessageBox.Show("dir=" + dir);
            THSelectedDir = dir + string.Empty;
            //MessageBox.Show("THSelectedDir=" + THSelectedDir);

            //MessageBox.Show("sType=" + sType);

            //if (THSelectedSourceType == "RPGMakerTransPatch")
            //{
            //Cleaning of the type
            //THRPGMTransPatchFiles.Clear();
            //THFilesElementsDataset.Clear();

            //string patchver;
            var patchdir = dir;
            StreamReader patchfile = new StreamReader(sPath);
            //MessageBox.Show(patchfile.ReadLine());
            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(Path.Combine(THSelectedDir,"patch"))) //если есть подпапка patch, тогда это версия патча 3
            {
                THRPGMTransPatchver = "3";
                patchdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(sPath),"patch"));
            }
            else //иначе это версия 2
            {
                THRPGMTransPatchver = "2";
            }
            patchfile.Close();

            var vRPGMTransPatchFiles = new List<string>();

            foreach (FileInfo file in patchdir.GetFiles("*.txt"))
            {
                //MessageBox.Show("file.FullName=" + file.FullName);
                vRPGMTransPatchFiles.Add(file.FullName);
            }

            //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

            //THFilesDataGridView.Nodes.Add("main");
            //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
            //RPGMTransPatch.OpenTransFiles(files, patchver);
            if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, THRPGMTransPatchver, THFilesElementsDataset, THFilesElementsDatasetInfo))
            {
                //MessageBox.Show(THSelectedSourceType + " loaded!");
                //THShowMessage(THSelectedSourceType + " loaded!");
                //ProgressInfo(false, string.Empty);
                //LogToFile(string.Empty, true);

                //Запись в dataGridVivwer
                for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                {
                    //MessageBox.Show("ListFiles=" + ListFiles[i]);
                    //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                    //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
                    //THFilesDataGridView.Rows.Add();
                    //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                    //dGFiles.Rows.Add();
                    //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                }

                return "RPGMakerTransPatch";
            }
            //}
            return string.Empty;
        }

        private string RPGMTransPatchPrepareTranslated(string sPath)
        {
            THFilesElementsDatasetTranslated.Reset();
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            var patchdir = dir;
            THSelectedDir = dir + string.Empty;
            StreamReader patchfile = new StreamReader(sPath);
            //MessageBox.Show(patchfile.ReadLine());
            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(dir + "\\patch")) //если есть подпапка patch, тогда это версия патча 3
            {
                THRPGMTransPatchverTranslated = "3";
                patchdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(sPath),"patch"));
            }
            else //иначе это версия 2
            {
                THRPGMTransPatchverTranslated = "2";
            }
            patchfile.Close();

            var vRPGMTransPatchFiles = new List<string>();

            foreach (FileInfo file in patchdir.GetFiles("*.txt"))
            {
                vRPGMTransPatchFiles.Add(file.FullName);
            }
            if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, THRPGMTransPatchverTranslated, THFilesElementsDatasetTranslated, null))
            {

                //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                THLoadDBCompare(THFilesElementsDatasetTranslated);
                return "ok";
                //return LoadOriginalToTranslation(THFilesElementsDatasetTranslated);
            }
            //}
            return string.Empty;
        }

        private bool OpentppTransFile(string sPath)
        {
            try
            {
                string filename = Path.GetFileNameWithoutExtension(sPath); // get json file name
                if (THFilesElementsDataset.Tables.Contains(filename))
                {
                    return true;
                }
                ProgressInfo(true, T._("opening file: ") + filename + ".trans");
                string jsondata = File.ReadAllText(sPath); // get json data

                THFilesElementsDataset.Tables.Add(filename); // create table with json name
                THFilesElementsDataset.Tables[filename].Columns.Add("Original"); //create Original column
                THFilesElementsDataset.Tables[filename].Columns.Add("Translation"); //create Translation column for trans file
                THFilesElementsDatasetInfo.Tables.Add(filename); // create table with json name
                THFilesElementsDatasetInfo.Tables[filename].Columns.Add("Original"); //create Original column

                bool ret = true;

                ret = ReadJson(filename, sPath);

                if (THFilesElementsDataset.Tables[filename].Rows.Count > 0)
                {
                }
                else
                {
                    THFilesElementsDataset.Tables.Remove(filename); // remove table if was no items added
                    THFilesElementsDatasetInfo.Tables.Remove(filename); // remove table if was no items added
                }

                return ret;
            }
            catch
            {
                return false;
            }

        }

        private bool OpenRPGMakerMVjson(string sPath)
        {
            //StreamReader _file = new StreamReader(sPath);
            //while (!_file.EndOfStream)
            //{
            //string jsonline = _file.ReadLine();
            //string jsonstring = _file.ReadToEnd();
            //JObject o1 = JObject.Parse(File.ReadAllText(@"test.json"));
            //var parsedObject = JObject.Parse(o1);
            //THFileElementsDataGridView.DataSource = o1;
            //if (parsedObject.ToString().ToLower().Contains("name"))
            //{
            //RPGMakerMVjsonFile o = JsonConvert.DeserializeObject<RPGMakerMVjsonFile>(jsonline);
            //MessageBox.Show(o.Name);
            //}
            //}

            //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
            /*using (var reader = new StreamReader(sPath))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var root = JToken.Load(jsonReader);
                THFileElementsDataGridView.DataSource = root;
            }*/

            try
            {

                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string Jsonname = Path.GetFileNameWithoutExtension(sPath); // get json file name
                if (THFilesElementsDataset.Tables.Contains(Jsonname))
                {
                    //MessageBox.Show("true!");
                    return true;
                }
                ProgressInfo(true, T._("opening file: ") + Jsonname + ".json");
                string jsondata = File.ReadAllText(sPath); // get json data

                THFilesElementsDataset.Tables.Add(Jsonname); // create table with json name
                THFilesElementsDataset.Tables[Jsonname].Columns.Add("Original"); //create Original column
                THFilesElementsDatasetInfo.Tables.Add(Jsonname); // create table with json name
                THFilesElementsDatasetInfo.Tables[Jsonname].Columns.Add("Original"); //create Original column
                //MessageBox.Show("Added table:"+Jsonname);

                /*
                bool name = false;
                bool description = false;
                bool displayname = false;
                bool note = false;
                bool message1 = false;
                bool message2 = false;
                bool message3 = false;
                bool message4 = false;
                bool nickname = false;
                bool profile = false;
                bool maps = false;
                bool cmnevents = false;
                bool system = false;
                */

                bool ret = true;

                ret = ReadJson(Jsonname, sPath);

                /*
                string jsonname = Jsonname.ToLower(); //set jsonname to lower registry
                if (jsonname == "items" || jsonname == "armors" || jsonname == "weapons")
                {
                    //("name", "description", "note")
                    //name = true;
                    //description = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonItemsArmorsWeapons(jsonname, jsondata);
                }
                else if (jsonname == "skills")
                {
                    //("name", "description", "message1", "message2", "note")
                    //name = true;
                    //description = true;
                    //message1 = true;
                    //message2 = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonSkills(jsonname, jsondata);
                }
                else if (jsonname == "states")
                {
                    //("name", "message1", "message2", "message3", "message4", "note")
                    //name = true;
                    //message1 = true;
                    //message2 = true;
                    //message3 = true;
                    //message4 = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonStates(jsonname, jsondata);
                }
                else if (jsonname == "classes" || jsonname == "enemies" || jsonname == "tilesets")
                {
                    //("name", "note")
                    //name = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonClassesEnemiesTilesets(jsonname, jsondata);
                }
                else if (jsonname == "animations" || jsonname == "mapinfos" || jsonname == "troops")
                {
                    //("name")
                    //name = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(jsonname, jsondata);
                }
                else if (jsonname == "actors")
                {
                    //("name", "nickname", "note", "profile")
                    //name = true;
                    //nickname = true;
                    //note = true;
                    //profile = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(jsonname, jsondata);
                }
                else if (jsonname.StartsWith("map"))
                {
                    //['displayName'] / ['note'] / ['events'][$eIndex]['name'] / ['events'][$eIndex]['note']
                    //displayname = true;
                    //name = true;
                    //note = true;
                    //maps = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonMap(jsonname, jsondata);
                }
                else if (jsonname == "commonevents")
                {
                    //"name" / 
                    //name = true;
                    //cmnevents = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonCommonEvents(jsonname, jsondata);
                }
                else if (jsonname == "system")
                {
                    //"name" /
                    //name = true;
                    //system = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonSystem(jsonname, jsondata);
                }
                */

                //MessageBox.Show("ret="+ret+ ",jsonname="+ jsonname);

                /*
                bool ret = FillDSTableWithJsonValues(
                    Jsonname,
                    jsondata,
                    name,
                    description,
                    displayname,
                    note, message1,
                    message2,
                    message3,
                    message4,
                    nickname,
                    profile,
                    maps,
                    cmnevents,
                    system);
                */

                //var result = JsonConvert.DeserializeObject<List<RPGMakerMVjson>>(File.ReadAllText(sPath));
                //var resultdescriptions = JsonConvert.DeserializeObject<List<RPGMakerMVjsonFileDescriptions>>(File.ReadAllText(sPath));
                //var resultparameters = JsonConvert.DeserializeObject<List<RPGMakerMVjsonFileParameters>>(File.ReadAllText(sPath));

                //THFileElementsDataGridView.DataSource = ds.Tables[0];
                //THFileElementsDataGridView.Columns[0].ReadOnly = true;

                if (THFilesElementsDataset.Tables[Jsonname].Rows.Count > 0)
                {
                    THFilesElementsDataset.Tables[Jsonname].Columns.Add("Translation");
                }
                else
                {
                    THFilesElementsDataset.Tables.Remove(Jsonname); // remove table if was no items added
                    THFilesElementsDatasetInfo.Tables.Remove(Jsonname); // remove table if was no items added
                }

                return ret;
            }
            catch
            {
                return false;
            }

        }

        public bool GetAlreadyAddedInTable(DataTable table, string value)
        {
            //про primary key взял отсюда: https://stackoverflow.com/questions/3567552/table-doesnt-have-a-primary-key
            //DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["Original"];
            table.PrimaryKey = keyColumns;

            //очень быстрый способ поиска дубликата значения, два нижник в разы медленней, этот почти не заметен
            if (table.Rows.Contains(value))
            {
                //LogToFile("Value already in table: \r\n"+ value);
                //MessageBox.Show("found! value=" + value);
                return true;
            }
            /*самый медленный способ, заметно медленней нижнего и непомерно критически медленней верхнего
            if (ds.Tables[tablename].Select("Original = '" + value.Replace("'", "''") + "'").Length > 0)
            {
                //MessageBox.Show("found! value=" + value);
                return true;
            }
            */
            /*довольно медленный способ, быстрее того, что перед этим с Select, но критически медленней верхнего первого
            for (int i=0; i < ds.Tables[tablename].Rows.Count; i++)
            {
                if (ds.Tables[tablename].Rows[i][0].ToString() == value)
                {
                    return true;
                }
            }
            */
            //LogToFile("Value still not in table: \r\n" + value);
            return false;
        }

        public bool HasNOJPcharacters(string str)
        {
            return RomajiKana.GetLocaleLangCount(str, "kanji") < 1 && RomajiKana.GetLocaleLangCount(str, "katakana") < 1 && RomajiKana.GetLocaleLangCount(str, "hiragana") < 1;
        }

        private bool TryToExtractToRPGMakerTransPatch(string sPath, string extractdir="Work")
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            string workdir = Path.Combine(Application.StartupPath,extractdir,"RPGMakerTrans/");
            if (!Directory.Exists(workdir))
            {
                Directory.CreateDirectory(workdir);
            }
            //MessageBox.Show("tempdir=" + tempdir);
            string outdir = workdir + Path.GetFileNameWithoutExtension(Path.GetDirectoryName(sPath));
            

            if (extractdir == "Work")
            {
                extractedpatchpath = outdir + "_patch";// Распаковывать в Work\ProjectDir\
            }

            bool ret;// = false;
            if (!Directory.Exists(outdir))
            {
                Directory.CreateDirectory(outdir);

                //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

            }
            else if (Directory.GetFiles(outdir+"_patch", "RPGMKTRANSPATCH", SearchOption.AllDirectories).Length > 0)
            {
                DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    //чистка и пересоздание папки
                    Directory.Delete(outdir, true);
                    Directory.CreateDirectory(outdir);

                    //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

                }
            }

            ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

            if (ret)
            {
            }
            else
            {
                //чистка папок 
                Directory.Delete(outdir, true);
            }

            return ret;
        }

        private bool CreateRPGMakerTransPatch(string inputdir, string outdir)
        {
            string rpgmakertranscli = Path.Combine(Application.StartupPath, "Res", "rpgmakertrans", "rpgmt.exe");
            bool ret;
            //string projectname = Path.GetFileName(outdir);

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
            string rpgmakertranscliargs = "\"" + inputdir + "\" -p \"" + outdir + "_patch\"" + " -o \"" + outdir + "_translated\"";

            ret = RunProgram(rpgmakertranscli, rpgmakertranscliargs);

            if (ret)
            {
            }
            else
            {
                //чистка папок 
                Directory.Delete(outdir + "_patch", true);
                Directory.Delete(outdir + "_translated", true);

                //попытка с параметром -b - Use UTF-8 BOM in Patch files
                ret = RunProgram(rpgmakertranscli, rpgmakertranscliargs + " -b");
            }

            return ret;
        }

        private bool RunProgram(string ProgramPath, string Arguments)
        {
            bool ret = false;
            using (Process Program = new Process())
            {
                //MessageBox.Show("outdir=" + outdir);
                Program.StartInfo.FileName = ProgramPath;
                Program.StartInfo.Arguments = Arguments;
                ret = Program.Start();
                Program.WaitForExit();
            }

            return ret;
        }

        private int invalidformat;

        public bool OpenRPGMTransPatchFiles(List<string> ListFiles, string patchver, DataSet DS, DataSet DSInfo)
        {
            //измерение времени выполнения
            //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
            //Stopwatch swatch = new Stopwatch();
            //swatch.Start();

            //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
            //MessageBox.Show("ListFiles=" + ListFiles);
            //MessageBox.Show("ListFiles[0]=" + ListFiles[0]);

            StreamReader _file;   //Через что читаем
            string _context = string.Empty;           //Комментарий
            string _advice = string.Empty;            //Предел длины строки
            string _string;// = string.Empty;            //Переменная строки
            string _original = string.Empty;           //Непереведенный текст
            string _translation = string.Empty;             //Переведенный текст
            int _status = 0;             //Статус

            int verok = 0;                  //версия патча
            //THMain Main = new THMain();
            //var Main = (THMain)MainForm;
            //THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            //THFileElementsDataGridView.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //THFilesDataGridView.Columns.Add("Filename", "Text");

            //прогрессбар
            //progressBar.Maximum = ListFiles.Count;
            //progressBar.Value = 0;
            //List<RPGMTransPatchFile> THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            //Читаем все файлы
            for (int i = 0; i < ListFiles.Count; i++)   //Обрабатываем всю строку
            {
                string fname = Path.GetFileNameWithoutExtension(ListFiles[i]);
                ProgressInfo(true, T._("opening file: ") + fname+".txt");
                _file = new StreamReader(ListFiles[i]); //Задаем файл
                //THRPGMTransPatchFiles.Add(new THRPGMTransPatchFile(Path.GetFileNameWithoutExtension(ListFiles[i]), ListFiles[i].ToString(), string.Empty));    //Добaвляем файл
                _ = DS.Tables.Add(fname);
                _ = DS.Tables[i].Columns.Add("Original");
                _ = DS.Tables[i].Columns.Add("Translation");
                _ = DS.Tables[i].Columns.Add("Context");
                _ = DS.Tables[i].Columns.Add("Advice");
                _ = DS.Tables[i].Columns.Add("Status");
                if (DSInfo == null)
                {
                }
                else
                {
                    _ = DSInfo.Tables.Add(fname);
                    _ = DSInfo.Tables[i].Columns.Add("Original");
                }

                if (patchver == "3")
                {
                    verok = 1; //Версия опознана
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
                        //Код для версии патча 3
                        if (_string.StartsWith("> BEGIN STRING"))
                        {
                            invalidformat = 2; //если нашло строку
                            _string = _file.ReadLine();

                            int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной                            
                            while (!_string.StartsWith("> CONTEXT:"))  //Ждем начало следующего блока
                            {
                                if (untranslines > 0)
                                {
                                    _original += Environment.NewLine;
                                }
                                _original += _string;            //Пишем весь текст
                                _string = _file.ReadLine();
                                untranslines++;
                            }

                            int contextlines = 0;
                            while (_string.StartsWith("> CONTEXT:"))
                            {
                                if (contextlines > 0)
                                {
                                    _context += Environment.NewLine;
                                }

                                _context += _string.Replace("> CONTEXT: ", string.Empty).Replace(" < UNTRANSLATED", string.Empty);// +"\r\n";//Убрал символ переноса, так как он остается при сохранении //Сохраняем коментарий
                                
                                _string = _file.ReadLine();
                                contextlines++;
                            }

                            int translines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                            while (!_string.StartsWith("> END"))      //Ждем конец блока
                            {
                                if (translines > 0)
                                {
                                    _translation += Environment.NewLine;
                                }
                                _translation += _string;
                                _string = _file.ReadLine();
                                translines++;
                            }

                            if (_original == Environment.NewLine)
                            {
                            }
                            else
                            {
                                //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));  //Пишем
                                _ = DS.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                if (DSInfo == null)
                                {
                                }
                                else
                                {
                                    _ = DSInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
                                }
                            }

                            _context = string.Empty;  //Чистим
                            _original = string.Empty;  //Чистим
                            _translation = string.Empty;    //Чистим
                        }
                    }
                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        invalidformat = 1;
                    }
                    _file.Close();  //Закрываем файл
                }
                else if (patchver == "2")
                {
                    string UNUSED = string.Empty;
                    verok = 1; //Версия опознана
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
                        if (Equals(_string, "# UNUSED TRANSLATABLES"))//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
                        {
                            //MessageBox.Show(_string);
                            UNUSED = _string;
                        }
                        //Код для версии патча 2.0
                        if (_string.StartsWith("# CONTEXT"))               //Ждем начало блока
                        {
                            invalidformat = 2;//строка найдена, формат верен

                            if (_string.Split(' ')[3] != "itemAttr/UsageMessage")
                            {
                                _context = _string.Replace("# CONTEXT : ", string.Empty); //Сохраняем коментарий

                                _string = _file.ReadLine();

                                //asdf advice Иногда advice отсутствует, например когда "# CONTEXT : Dialogue/SetHeroName" в патче VH
                                if (_string.StartsWith("# ADVICE"))
                                {
                                    _advice = _string.Replace("# ADVICE : ", string.Empty);   //Вытаскиваем число предела
                                    _string = _file.ReadLine();
                                }
                                else
                                {
                                    _advice = string.Empty;
                                }

                                if (UNUSED.Length == 0)
                                {
                                }
                                else
                                {                                    
                                    _advice += UNUSED;//добавление информации о начале блока неиспользуемых строк
                                    UNUSED = string.Empty;//очистка переменной в целях оптимизации, чтобы не писать во все ADVICE
                                }

                                int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                                while (!_string.StartsWith("# TRANSLATION"))  //Ждем начало следующего блока
                                {
                                    if (untranslines > 0)
                                    {
                                        _original += Environment.NewLine;
                                    }
                                    _original += _string;            //Пишем весь текст
                                    _string = _file.ReadLine();
                                    untranslines++;
                                }
                                if (_original.Length > 0)                    //Если текст есть, ищем перевод
                                {
                                    _string = _file.ReadLine();
                                    int _translationlinescount = 0;
                                    while (!_string.StartsWith("# END"))      //Ждем конец блока
                                    {
                                        if (_translationlinescount > 0)
                                        {
                                            _translation += Environment.NewLine;
                                        }
                                        _translation += _string;
                                        _string = _file.ReadLine();
                                        _translationlinescount++;
                                    }
                                    if (_original != Environment.NewLine)
                                    {
                                        //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));//Пишем
                                        _ = DS.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                        if (DSInfo == null)
                                        {
                                        }
                                        else
                                        {
                                            _ = DSInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
                                        }
                                    }
                                }
                                _original = string.Empty;  //Чистим
                                _translation = string.Empty;    //Чистим
                            }
                        }
                    }
                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        invalidformat = 1;
                    }
                    _file.Close();  //Закрываем файл
                }
                else
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    //THMsg.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    _file.Close();  //Закрываем файл
                    return false;
                }

                if (invalidformat == 1)
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    //THMsg.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    invalidformat = 0;
                    return false;
                }

                //progressBar.Value++;
            }

            //MessageBox.Show("111");

            if (verok == 1 & invalidformat != 1)
            {
                //ConnnectLinesToGrid(0); //подозрения, что вызывается 2 раза
                //MessageBox.Show("Готово!");
                FVariant = " * RPG Maker Trans Patch " + patchver;
            }
            else if (invalidformat == 1)
            {
                //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                //THMsg.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                return false;
            }

            //MessageBox.Show("111");
            //progressBar.Value = 0;

            //остановка таймера и запись времени
            //swatch.Stop();
            //LogToFile("time=" + swatch.Elapsed.ToString(), true);//asdf
            //THMsg.Show("time=" + time);

            return true;
        }

        private void SetDoublebuffered(bool value)
        {
            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                //THFileElementsDataGridView
                Type dgvType = THFileElementsDataGridView.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(THFileElementsDataGridView, value, null);

                //THFilesList
                //вроде не пашет для listbox
                Type lbxType = THFilesList.GetType();
                PropertyInfo pi1 = lbxType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi1.SetValue(THFilesList, value, null);
            }
        }

        //int numberOfRows=500;
        private bool THFilesListBox_MouseClickBusy;
        private void THFilesListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (THFilesListBox_MouseClickBusy && THFilesList.SelectedIndex > -1) //THFilesList.SelectedIndex > -1 - фикс исключения сразу после загрузки таблицы, когда индекс выбранной таблицы равен -1 
            {
                //return;
            }
            else
            {
                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                //int index = THFilesListBox.SelectedIndex;
                //Thread actions = new Thread(new ParameterizedThreadStart((obj) => THFilesListBoxMouseClickEventActions(index)));
                //actions.Start();

                THFilesListBox_MouseClickBusy = true;

                //Пример с присваиванием Dataset. Они вроде быстро открываются, в отличие от List. Проверка не подтвердила ускорения, всё также
                //https://stackoverflow.com/questions/11099619/how-to-bind-dataset-to-datagridview-in-windows-application

                //THFileElementsDataGridView.DataSource = null;
                //THFileElementsDataGridView.RowCount = 100;

                //Пробовал также отсюда через BindingList
                //https://stackoverflow.com/questions/44433428/how-to-use-virtual-mode-for-large-data-in-datagridview
                //не помогает
                //var dataPopulateList = new BindingList<Block>(THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks);
                //THFileElementsDataGridView.DataSource = dataPopulateList;

                //Еще источник с рекомендацией ниже, но тоже от нее не заметил эффекта
                //https://stackoverflow.com/questions/3580237/best-way-to-fill-datagridview-with-large-amount-of-data
                //THFileElementsDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                //or even better .DisableResizing.
                //Most time consumption enum is DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
                //THFileElementsDataGridView.RowHeadersVisible = false; // set it to false if not needed

                //https://www.codeproject.com/Questions/784355/How-to-solve-performance-issue-in-Datagridview-Min
                // Поменял List на BindingList и вроде чуть быстрее стало загружаться
                try
                {
                    //ProgressInfo(true);

                    /*
                    if (THSelectedSourceType.Contains("RPGMakerTransPatch"))
                    {
                    }
                    else if (THSelectedSourceType.Contains("RPG Maker MV"))
                    {
                        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex];

                        //https://10tec.com/articles/why-datagridview-slow.aspx
                        //ds.Tables[THFilesListBox.SelectedIndex].DefaultView.RowFilter = string.Format("Text LIKE '%{0}%'", "FIltering string");
                        
                    }
                    */

                    //THFiltersDataGridView.Columns.Clear();

                    //сунул под try так как один раз здесь была ошибка о выходе за диапахон


                    //https://www.youtube.com/watch?v=wZ4BkPyZllY
                    //Thread t = new Thread(new ThreadStart(StartLoadingForm));
                    //t.Start();
                    //Thread.Sleep(100);

                    //this.Cursor = Cursors.WaitCursor; // Поменять курсор на часики

                    //измерение времени выполнения
                    //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
                    //System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch();
                    //swatch.Start();

                    //https://stackoverflow.com/questions/778095/windows-forms-using-backgroundimage-slows-down-drawing-of-the-forms-controls
                    //THFileElementsDataGridView.SuspendDrawing();//используются оба SuspendDrawing и SuspendLayout для возможного ускорения
                    //THFileElementsDataGridView.SuspendLayout();//с этим вроде побыстрее чем с SuspendDrawing из ControlHelper

                    //THsplitContainerFilesElements.Panel2.Visible = false;//сделать невидимым родительский элемент на время

                    //Советы, после которых отображение ячеек стало во много раз быстрее, 
                    //https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/best-practices-for-scaling-the-windows-forms-datagridview-control
                    //Конкретно, поменял режим отображения строк(Rows) c AllCells на DisplayerCells, что ускорило отображение 3400к. строк в таблице в 100 раз, с 9с. до 0.09с. !

                    //THBS.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks;
                    //THFileElementsDataGridView.DataSource = THBS;

                    //THFileElementsDataGridView.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    //THFileElementsDataGridView.Invoke((Action)(() => THFileElementsDataGridView.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks));
                    //THFileElementsDataGridView.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks;//.GetRange(0, THRPGMTransPatchFilesFGetCellCount());
                    //if (THFilesListBox.SelectedIndex >= 0)//предотвращает исключение "Невозможно найти таблицу -1"
                    //{
                    //    THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex];//.GetRange(0, THRPGMTransPatchFilesFGetCellCount());
                    //}

                    BindToDataTableGridView(THFilesElementsDataset.Tables[THFilesList.SelectedIndex]);

                    ShowNonEmptyRowsCount(THFilesElementsDataset);

                    /*
                    //Virtual mode implementation
                    THFileElementsDataGridView.Rows.Clear();
                    THFileElementsDataGridView.Columns.Clear();
                    THFileElementsDataGridView.Columns.Add("Original", THMainDGVOriginalColumnName);
                    THFileElementsDataGridView.Columns.Add("Translation", THMainDGVTranslationColumnName);
                    if (THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count < numberOfRows)
                    {
                        THFileElementsDataGridView.RowCount = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count;
                    }
                    else
                    {
                        THFileElementsDataGridView.RowCount = numberOfRows;
                    }
                    */

                    //foreach (var sblock in THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks)
                    //{
                    //    THFileElementsDataGridView.Rows.Add(sblock.Original, sblock.Translation);
                    //}

                    //iGrid1.FillWithData(THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks);


                    //t.Abort();

                    //THsplitContainerFilesElements.Panel2.Visible = true;

                    //swatch.Stop();
                    //string time = swatch.Elapsed.ToString();
                    //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", DateTime.Now + " >>:" + THFilesListBox.SelectedItem.ToString() + "> Time:\"" + time + "\"\r\n", true);
                    //MessageBox.Show("Time: "+ time); // тут выводим результат в консоль

                    if (THSelectedSourceType.Contains("RPGMakerTransPatch") || THSelectedSourceType.Contains("RPG Maker game with RPGMTransPatch")) //Additional tweaks for RPGMTransPatch table
                    {
                        THFileElementsDataGridView.Columns["Context"].Visible = false;
                        THFileElementsDataGridView.Columns["Status"].Visible = false;
                        if (THFileElementsDataGridView.Columns["Advice"]!=null)
                        {
                            THFileElementsDataGridView.Columns["Advice"].Visible = false;
                        }
                    }

                    //this.Cursor = Cursors.Default; ;//Поменять курсор обратно на обычный

                    //THFileElementsDataGridView.ResumeLayout();
                    //THFileElementsDataGridView.ResumeDrawing();

                    SetFilterDGV(); //Init Filters datagridview

                    SetOnTHFileElementsDataGridViewWasLoaded(); //Additional actions when elements of file was loaded in datagridview

                    CheckFilterDGV(); //Apply filters if they is not empty

                    //ProgressInfo(false, string.Empty);
                }
                catch (Exception)
                {
                }

                //THFileElementsDataGridView.RowHeadersVisible = true; // set it to false if not needed

                THFilesListBox_MouseClickBusy = false;
            }
        }

        public void BindToDataTableGridView(DataTable DT)
        {
            if (THFilesList != null && THFilesList.SelectedIndex > -1)//вторая попытка исправить исключение при выборе элемента списка
            {
                THFileElementsDataGridView.DataSource = DT;
            }
        }

        private void SetOnTHFileElementsDataGridViewWasLoaded()
        {
            ControlsSwitchActivated = true;
            ControlsSwitchIsOn = (cutToolStripMenuItem1.ShortcutKeys != Keys.None);

            THFileElementsDataGridView.Columns["Original"].HeaderText = T._("Original");//THMainDGVOriginalColumnName;
            THFileElementsDataGridView.Columns["Translation"].HeaderText = T._("Translation");//THMainDGVTranslationColumnName;
            THFileElementsDataGridView.Columns["Original"].ReadOnly = true;
            THFiltersDataGridView.Enabled = true;
            THSourceRichTextBox.Enabled = true;
            THTargetRichTextBox.Enabled = true;
            //THTargetRichTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


            openInWebToolStripMenuItem.Enabled = true;
            selectedToolStripMenuItem1.Enabled = true;
            tableToolStripMenuItem1.Enabled = true;
            fixCellsSelectedToolStripMenuItem.Enabled = true;
            fixCellsTableToolStripMenuItem.Enabled = true;
            setOriginalValueToTranslationToolStripMenuItem.Enabled = true;
            completeRomajiotherLinesToolStripMenuItem.Enabled = true;
            completeRomajiotherLinesToolStripMenuItem1.Enabled = true;
            forceSameForSimularToolStripMenuItem.Enabled = true;
            forceSameForSimularToolStripMenuItem1.Enabled = true;
            cutToolStripMenuItem1.Enabled = true;
            copyCellValuesToolStripMenuItem.Enabled = true;
            pasteCellValuesToolStripMenuItem.Enabled = true;
            clearSelectedCellsToolStripMenuItem.Enabled = true;
            toUPPERCASEToolStripMenuItem.Enabled = true;
            firstCharacterToUppercaseToolStripMenuItem.Enabled = true;
            toLOWERCASEToolStripMenuItem.Enabled = true;
            setColumnSortingToolStripMenuItem.Enabled = true;
            OpenInWebContextToolStripMenuItem.Enabled = true;
            TranslateSelectedContextToolStripMenuItem.Enabled = true;
            TranslateTableContextToolStripMenuItem.Enabled = true;
            fixSymbolsContextToolStripMenuItem.Enabled = true;
            fixSymbolsTableContextToolStripMenuItem.Enabled = true;
            OriginalToTransalationContextToolStripMenuItem.Enabled = true;
            CutToolStripMenuItem.Enabled = true;
            CopyToolStripMenuItem.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            toolStripMenuItem11.Enabled = true;
            toolStripMenuItem14.Enabled = true;
            uppercaseToolStripMenuItem.Enabled = true;
            lowercaseToolStripMenuItem.Enabled = true;

            //saveToolStripMenuItem.Enabled = true; //эти активируются при внесении изменений
            //saveAsToolStripMenuItem.Enabled = true;
            //saveTranslationToolStripMenuItem.Enabled = true;
            //editToolStripMenuItem.Enabled = true;//а эти активируются сразу после успешного открытия файлов
            //viewToolStripMenuItem.Enabled = true;
            //loadTranslationToolStripMenuItem.Enabled = true;
            //loadTrasnlationAsToolStripMenuItem.Enabled = true;
        }

        private void SetFilterDGV()
        {
            //MessageBox.Show("THFiltersDataGridView.Columns.Count=" + THFiltersDataGridView.Columns.Count
            //    + "\r\nTHFileElementsDataGridView visible Columns Count=" + THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible));
            if (THFiltersDataGridView.Columns.Count != THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible))
            {
                THFiltersDataGridView.Columns.Clear();
                THFiltersDataGridView.Rows.Clear();
                //int visibleindex = -1;
                for (int cindx = 0; cindx < THFileElementsDataGridView.Columns.Count; cindx++)
                {
                    if (THFileElementsDataGridView.Columns[cindx].Visible)
                    {
                        //visibleindex += 1;
                        //MessageBox.Show("THFileElementsDataGridView.Columns[cindx].Name="+ THFileElementsDataGridView.Columns[cindx].Name
                        //    + "\r\nTHFileElementsDataGridView.Columns[cindx].HeaderText="+ THFileElementsDataGridView.Columns[cindx].HeaderText);
                        THFiltersDataGridView.Columns.Add(THFileElementsDataGridView.Columns[cindx].Name, THFileElementsDataGridView.Columns[cindx].HeaderText);
                        //THFiltersDataGridView.Columns[visibleindex].Width = THFileElementsDataGridView.Columns[cindx].Width;
                    }
                }
                THFiltersDataGridView.Rows.Add(1);
                THFiltersDataGridView.CurrentRow.Selected = false;
            }
        }

        private void THFileElementsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //Считывание значения ячейки в текстовое поле 1, вариант 2, для DataSet, ds.Tables[0]
                if (THSourceRichTextBox.Enabled && THFileElementsDataGridView.Rows.Count > 0 && e.RowIndex >= 0 && e.ColumnIndex >= 0) //Проверка на размер индексов, для избежания ошибки при попытке сортировки " должен быть положительным числом и его размер не должен превышать размер коллекции"
                {                    
                    THTargetRichTextBox.Invoke((Action)(() => THTargetRichTextBox.Clear()));//здесь была ошибка о попытке доступа из другого потока

                    if ((THFileElementsDataGridView.Rows[e.RowIndex].Cells["Original"].Value + string.Empty).Length == 0)
                    {

                    }
                    else//проверить, не пуста ли ячейка, иначе была бы ошибка //THStrDGTranslationColumnName ошибка при попытке сортировки по столбцу
                    {
                        //wrap words fix: https://stackoverflow.com/questions/1751371/how-to-use-n-in-a-textbox
                        THSourceRichTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells["Original"].Value + string.Empty; //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                        //https://github.com/caguiclajmg/WanaKanaSharp
                        //if (GetLocaleLangCount(THSourceTextBox.Text, "hiragana") > 0)
                        //{
                        //    GetWords(THSourceTextBox.Text);
                        //   var hepburnConverter = new HepburnConverter();
                        //   WanaKana.ToRomaji(hepburnConverter, THSourceTextBox.Text); // hiragana
                        //}
                        //также по японо ыфуригане
                        //https://docs.microsoft.com/en-us/uwp/api/windows.globalization.japanesephoneticanalyzer
                    }
                    if ((THFileElementsDataGridView.Rows[e.RowIndex].Cells["Translation"].Value + string.Empty).Length == 0)
                    {

                    }
                    else//проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                    {
                        if ((THFileElementsDataGridView.Rows[e.RowIndex].Cells["Translation"].Value + string.Empty).Length == 0)
                        {
                            THTargetRichTextBox.Clear();
                        }

                        THTargetRichTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells["Translation"].Value + string.Empty; //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                    }

                    THInfoTextBox.Text = string.Empty;

                    if ((THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + string.Empty).Length == 0)
                    {

                    }
                    else
                    {
                        //gem furigana
                        //https://github.com/helephant/Gem
                        //var furigana = new Furigana(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        //THInfoTextBox.Text += furigana.Reading + "\r\n";
                        //THInfoTextBox.Text += furigana.Expression + "\r\n";
                        //THInfoTextBox.Text += furigana.Hiragana + "\r\n";
                        //THInfoTextBox.Text += furigana.ReadingHtml + "\r\n";
                        THInfoTextBox.Text += T._("rowinfo:") + Environment.NewLine + THFilesElementsDatasetInfo.Tables[THFilesList.SelectedIndex].Rows[e.RowIndex][0];
                        if (THSelectedSourceType == "RPG Maker MV")
                        {
                            THInfoTextBox.Text += Environment.NewLine + Environment.NewLine + T._("Several strings also can be in Plugins.js in 'www\\js' folder and referred plugins in plugins folder.");
                        }
                        THInfoTextBox.Text += Environment.NewLine+ Environment.NewLine;
                        THInfoTextBox.Text += RomajiKana.THShowLangsOfString(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + string.Empty, "all"); //Show all detected languages count info
                    }
                }
                //--------Считывание значения ячейки в текстовое поле 1
            }
            catch
            {
            }
        }

        private void ShowNonEmptyRowsCount(DataSet tHFilesElementsDataset)
        {
            int RowsCount = Table.GetDatasetRowsCount(tHFilesElementsDataset);
            if (RowsCount == 0)
            {
                TableCompleteInfoLabel.Visible = false;
            }
            else
            {
                TableCompleteInfoLabel.Visible = true;
                TableCompleteInfoLabel.Text = Table.GetDatasetNonEmptyRowsCount(tHFilesElementsDataset) + "/" + RowsCount;
            }
        }

        //Для функции перевода, чтобы не переводить, когда в тексте нет иероглифов
        //private bool NoKanjiHiraganaKatakanaInTheString(string target)
        //{
        //    if (GetLocaleLangCount(target, "kanji") == 0 && GetLocaleLangCount(target, "hiragana") == 0 && GetLocaleLangCount(target, "katakana") == 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// True if procent of Romaji or Other characters in input string is lesser of set value in Settings
        /// </summary>
        /// <param name="extractedvalue"></param>
        /// <returns></returns>
        public bool SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(string extractedvalue)
        {
            return SelectedLocalePercentFromStringIsNotValid(extractedvalue)
                || SelectedLocalePercentFromStringIsNotValid(extractedvalue, "other")
                || (RomajiKana.GetLocaleLangCount(extractedvalue, "romaji") + RomajiKana.GetLocaleLangCount(extractedvalue, "other")) == extractedvalue.Length
                || RomajiKana.GetLocaleLangCount(extractedvalue, "romaji") == extractedvalue.Length
                || RomajiKana.GetLocaleLangCount(extractedvalue, "other") == extractedvalue.Length;
        }

        /// <summary>
        /// True if procent of selected locale characters in target string is lesser of set value in Settings
        /// </summary>
        /// <param name="target"></param>
        /// <param name="langlocale"></param>
        /// <returns></returns>
        public bool SelectedLocalePercentFromStringIsNotValid(string target, string langlocale = "romaji")
        {
            try
            {
                if (DontLoadStringIfRomajiPercent && !string.IsNullOrEmpty(target))
                {
                    return ((RomajiKana.GetLocaleLangCount(target, langlocale) * 100) / RomajiKana.GetLocaleLangCount(target, "all")) > DontLoadStringIfRomajiPercentNum;
                }
            }
            catch
            {

            }
            return false;
        }

        bool SaveInAction = false;
        bool FIleDataWasChanged = false;
        private async void WriteTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveInAction)
            {
                //MessageBox.Show("Saving still in progress. Please wait a little.");
            }
            SaveInAction = true;
            
            if (FIleDataWasChanged)
            {
                SaveInAction = true;
                FIleDataWasChanged = false;
                //MessageBox.Show("THSelectedSourceType=" + THSelectedSourceType);
                if (THSelectedSourceType == "RPGMakerTransPatch" || THSelectedSourceType == "RPG Maker game with RPGMTransPatch")
                {
                    //THActionProgressBar.Visible = true;
                    //THInfolabel.Visible = true;
                    //THInfolabel.Text = "saving..";
                    ProgressInfo(true);

                    //THInfoTextBox.Text = "Saving...";

                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    //Thread save = new Thread(new ParameterizedThreadStart((obj) => SaveRPGMTransPatchFiles(THSelectedDir, THRPGMTransPatchver)));
                    //save.Start();

                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => SaveRPGMTransPatchFiles(THSelectedDir, THRPGMTransPatchver));

                    //MessageBox.Show("THSelectedDir=" + THSelectedDir);
                    //SaveRPGMTransPatchFiles(THSelectedDir, THRPGMTransPatchver);

                    //THInfoTextBox.Text = string.Empty;

                    //THActionProgressBar.Visible = false;

                    if (THSelectedSourceType == "RPG Maker game with RPGMTransPatch")
                    {
                        DialogResult result = MessageBox.Show("Write in _translated variant of the game? This will not overwrite original game folder but can be used for test game.", "Write in translated variant", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
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
                            string rpgmakertranscliargs = "\"" + THSelectedGameDir + "\" -p \"" + THSelectedDir + "\"" + " -o \"" + THSelectedDir.Remove(THSelectedDir.Length - "_patch".Length, "_patch".Length) + "_translated\"";

                            if (RunProgram(rpgmakertranscli, rpgmakertranscliargs))
                            {
                            }
                            else
                            {
                                RunProgram(rpgmakertranscli, rpgmakertranscliargs+" -b");// попытка с параметром -b
                            }
                        }
                        else if (result == DialogResult.No)
                        {
                            //code for No
                        }
                    }

                    SaveInAction = false;
                }
                else if (istpptransfile)
                {
                    //THMsg.Show(THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                    WriteJson(THFilesList.Items[0] + string.Empty, THSelectedDir + THFilesList.Items[0] + ".trans");
                }
                else if (THSelectedSourceType == "RPG Maker MV json")
                {
                    //THMsg.Show(THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                    WriteJson(THFilesList.Items[0] + string.Empty, THSelectedDir + "\\" + THFilesList.Items[0] + ".json");
                }
                else if (THSelectedSourceType == "RPG Maker MV")
                {
                    for (int f=0;f < THFilesList.Items.Count; f++)
                    {
                        //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                        //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                        bool changed = false;
                        for (int r = 0; r < THFilesElementsDataset.Tables[f].Rows.Count; r++)
                        {
                            if ((THFilesElementsDataset.Tables[f].Rows[r]["Translation"] + string.Empty).Length == 0)
                            {
                            }
                            else
                            {
                                changed = true;
                                break;
                            }
                        }
                        //THMsg.Show(THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                        if (changed)
                        {

                            //THMsg.Show("start writing");
                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            await Task.Run(() => WriteJson(THFilesList.Items[f] + string.Empty, THSelectedDir + "\\www\\data\\" + THFilesList.Items[f] + ".json"));
                            //WriteJson(THFilesListBox.Items[f].ToString(), THSelectedDir + "\\www\\data\\" + THFilesListBox.Items[f].ToString() + ".json");
                        }
                    }
                    THMsg.Show(T._("finished") + "!");
                }
                else if (THSelectedSourceType == "KiriKiri scenario")
                {
                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    //await Task.Run(() => KiriKiriScenarioWrite(THSelectedDir + "\\" + THFilesList.Items[0] + ".scn"));
                    await Task.Run(() => KiriKiriScriptScenarioWrite(THSelectedDir + "\\" + THFilesList.Items[0] + ".scn"));
                }
                else if (THSelectedSourceType == "KiriKiri script")
                {
                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => KiriKiriScriptScenarioWrite(THSelectedDir + "\\" + THFilesList.Items[0] + ".ks"));
                }
            }
            SaveInAction = false;
        }

        private void KiriKiriScriptScenarioWrite(string sPath)
        {
            try
            {
                //ks scn tjs open
                StringBuilder buffer = new StringBuilder();

                using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    //string original = string.Empty;
                    string filename = Path.GetFileNameWithoutExtension(sPath);
                    int elementnumber = 0;
                    bool iscomment = false;
                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith("/*"))
                        {
                            iscomment = true;
                        }
                        else if (line.EndsWith("*/"))
                        {
                            iscomment = false;
                        }

                        if (iscomment || string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("//"))
                        {
                        }
                        else
                        {
                            if (line.EndsWith("[k]")) // text
                            {
                                string cline = line.Replace("[r]", string.Empty).Remove(line.Length - 3, 3);
                                if (string.IsNullOrEmpty(cline) || IsDigitsOnly(cline))
                                {
                                }
                                else
                                {
                                    var row = THFilesElementsDataset.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = row[1] + "[k]";
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.StartsWith("*")) // text
                            {
                                string cline = line.Remove(0, 1);
                                if (string.IsNullOrEmpty(cline) || IsDigitsOnly(cline))
                                {
                                }
                                else
                                {
                                    var row = THFilesElementsDataset.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = row[1] as string;
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.EndsWith("[r]")) //text, first line
                            {
                                string cline = line.Replace("[r]", string.Empty);
                                if (string.IsNullOrEmpty(cline) || IsDigitsOnly(cline))
                                {
                                }
                                else
                                {
                                    var row = THFilesElementsDataset.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = row[1] as string;
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.StartsWith("o.") || Regex.IsMatch(line, KiriKiriVariableSearchRegexPattern)) //variable, which is using even for displaing and should be translated in all files
                            {
                                MatchCollection matches = Regex.Matches(line, KiriKiriVariableSearchRegexFullPattern);

                                bool startswith = line.StartsWith("o.");
                                for (int m = 0; m < matches.Count; m++)
                                {
                                    var row = THFilesElementsDataset.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(matches[m].Value.Remove(0, startswith ? 2 : 3), row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = line.Remove(matches[m].Index, matches[m].Value.Length).Insert(matches[m].Index, matches[m].Value.Replace(row[0] as string, row[1] as string));
                                    }
                                    if (startswith)
                                    {
                                        startswith = false;//o. в начале встречается только раз
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.StartsWith("@notice text="))
                            {
                                string cline = line.Remove(0, 13);//удаление "@notice text="
                                if (string.IsNullOrEmpty(cline) || IsDigitsOnly(cline))
                                {
                                }
                                else
                                {
                                    var row = THFilesElementsDataset.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = "@notice text=" + row[1];
                                    }
                                    elementnumber++;
                                }
                            }
                        }

                        buffer.AppendLine(line);
                    }
                }

                File.WriteAllText(sPath, buffer.ToString(), Encoding.GetEncoding(932));

                _ = MessageBox.Show(T._("finished") + "!");
            }
            catch
            {
            }
        }

        //private void KiriKiriScenarioWrite(string sPath)
        //{
        //    try
        //    {
        //        StringBuilder buffer = new StringBuilder();

        //        using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
        //        {
        //            string line;
        //            string original = string.Empty;
        //            string filename = Path.GetFileNameWithoutExtension(sPath);
        //            int elementnumber = 0;
        //            while (!file.EndOfStream)
        //            {
        //                line = file.ReadLine();

        //                if (line.StartsWith(";") || line.StartsWith("@") || Equals(line, string.Empty))
        //                {
        //                }
        //                else
        //                {
        //                    if (line.EndsWith("[k]"))
        //                    {
        //                        if (
        //                            (THFilesElementsDataset.Tables[0].Rows[elementnumber][1] + string.Empty).Length > 0
        //                            && Equals(line.Remove(line.Length - 3, 3), THFilesElementsDataset.Tables[0].Rows[elementnumber][0] + string.Empty)
        //                            && !Equals(THFilesElementsDataset.Tables[0].Rows[elementnumber][0] + string.Empty, THFilesElementsDataset.Tables[0].Rows[elementnumber][1] + string.Empty)
        //                           )
        //                        {
        //                            line = THFilesElementsDataset.Tables[0].Rows[elementnumber][1] + "[k]";
        //                        }

        //                        elementnumber++;
        //                    }
        //                }

        //                buffer.AppendLine(line);
        //            }
        //        }

        //        File.Move(sPath, sPath + ".bak");
        //        File.WriteAllText(sPath, buffer.ToString(), Encoding.GetEncoding(932));

        //        _ = MessageBox.Show(T._("finished") + "!");
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = MessageBox.Show(ex.ToString());
        //    }
        //}

        private void ProgressInfo(bool status, string statustext = "")
        {
            statustext = statustext.Length == 0 ? T._("working..") : statustext;
            try
            {
                THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Visible = status));
                THInfolabel.Invoke((Action)(() => THInfolabel.Visible = status));
                THInfolabel.Invoke((Action)(() => THInfolabel.Text = statustext));
            }
            catch
            {
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog THSaveFolderBrowser = new FolderBrowserDialog())
            {

                if (SaveInAction)
                {
                    return;
                    //MessageBox.Show("Saving still in progress. Please wait a little.");
                }

                //MessageBox.Show(dirpath);
                THSaveFolderBrowser.SelectedPath = THSelectedDir; //Установить начальный путь на тот, что был установлен при открытии.

                if (THSaveFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (THSelectedSourceType == "RPGMakerTransPatch")
                    {
                        if (SaveRPGMTransPatchFiles(THSaveFolderBrowser.SelectedPath, THRPGMTransPatchver))
                        {
                            THSelectedDir = THSaveFolderBrowser.SelectedPath;
                            //MessageBox.Show("Сохранение завершено!");
                            THMsg.Show(T._("Save complete!"));
                        }
                    }
                }
            }
        }

        public bool SaveRPGMTransPatchFiles(string SelectedDir, string patchver = "2")
        {
            //измерение времени выполнения
            //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
            //Stopwatch swatch = new Stopwatch();
            //swatch.Start();

            try
            {
                StringBuilder buffer = new StringBuilder();

                //Прогресс
                //pbstatuslabel.Visible = true;
                //pbstatuslabel.Text = "сохранение..";
                //progressBar.Maximum = 0;
                //for (int i = 0; i < ArrayTransFilses.Count; i++)
                //    for (int y = 0; y < ArrayTransFilses[i].blocks.Count; y++)
                //        progressBar.Maximum = progressBar.Maximum + ArrayTransFilses[i].blocks.Count;
                //MessageBox.Show(progressBar.Maximum.ToString());
                //progressBar.Value = 0;

                int originalcolumnindex = THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
                int translationcolumnindex = THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
                int contextcolumnindex = THFilesElementsDataset.Tables[0].Columns["Context"].Ordinal;
                int advicecolumnindex = THFilesElementsDataset.Tables[0].Columns["Advice"].Ordinal;
                int statuscolumnindex = THFilesElementsDataset.Tables[0].Columns["Status"].Ordinal;

                if (patchver == "3")
                {
                    //запись в файл RPGMKTRANSPATCH строки > RPGMAKER TRANS PATCH V3
                    //StreamWriter RPGMKTRANSPATCHwriter = new StreamWriter("RPGMKTRANSPATCH", true);
                    //RPGMKTRANSPATCHwriter.WriteLine("> RPGMAKER TRANS PATCH V3");
                    //RPGMKTRANSPATCHwriter.Close();

                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        ProgressInfo(true, T._("saving file: ") + THFilesElementsDataset.Tables[i].TableName);

                        buffer.AppendLine("> RPGMAKER TRANS PATCH FILE VERSION 3.2");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < THFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + Environment.NewLine;
                            buffer.AppendLine(THFilesElementsDataset.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine);
                            //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                            //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                            //string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                            string[] CONTEXT = (THFilesElementsDataset.Tables[i].Rows[y][contextcolumnindex] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);
                            //string str1 = string.Empty;
                            string TRANSLATION = THFilesElementsDataset.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
                            for (int g = 0; g < CONTEXT.Count(); g++)
                            {
                                /*CONTEXT[g] = CONTEXT[g].Replace("\r", string.Empty);*///очистка от знака переноса, возникающего после разбития на строки по \n
                                if (CONTEXT.Count() > 1)
                                {
                                    buffer.AppendLine("> CONTEXT: " + CONTEXT[g]);// + Environment.NewLine);
                                }
                                else
                                {   //if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                    if (TRANSLATION.Length == 0) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                    {
                                        buffer.AppendLine("> CONTEXT: " + CONTEXT[g] + " < UNTRANSLATED");// + Environment.NewLine);
                                    }
                                    else
                                    {
                                        buffer.AppendLine("> CONTEXT: " + CONTEXT[g]);// + Environment.NewLine);
                                    }
                                }
                            }
                            //buffer += Environment.NewLine;
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation + Environment.NewLine;
                            buffer.AppendLine(TRANSLATION);// + Environment.NewLine);
                            buffer.AppendLine("> END STRING" + Environment.NewLine);// + Environment.NewLine);

                            //progressBar.Value++;
                            //MessageBox.Show(progressBar.Value.ToString());
                        }

                        if (string.IsNullOrWhiteSpace(buffer.ToString()))
                        {
                        }
                        else
                        {
                            if (Directory.Exists(SelectedDir + "\\patch"))
                            {
                            }
                            else
                            {
                                Directory.CreateDirectory(SelectedDir + "\\patch");
                            }
                            buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки
                            //String _path = SelectedDir + "\\patch\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            string _path = SelectedDir + "\\patch\\" + THFilesElementsDataset.Tables[i].TableName + ".txt";
                            File.WriteAllText(_path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }

                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(SelectedDir,"RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(SelectedDir,"RPGMKTRANSPATCH"), "> RPGMAKER TRANS PATCH V3");
                    }
                }
                else if (patchver == "2")
                {
                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        ProgressInfo(true, T._("saving file: ") + THFilesElementsDataset.Tables[i].TableName);

                        bool unusednotfound = true;//для проверки начала неиспользуемых строк, в целях оптимизации

                        buffer.AppendLine("# RPGMAKER TRANS PATCH FILE VERSION 2.0");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < THFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            string ADVICE = THFilesElementsDataset.Tables[i].Rows[y][advicecolumnindex] + string.Empty;
                            //Если в advice была информация о начале блоков неиспользуемых, то вставить эту строчку
                            if (unusednotfound && ADVICE.Contains("# UNUSED TRANSLATABLES"))
                            {
                                buffer.AppendLine("# UNUSED TRANSLATABLES");// + Environment.NewLine;
                                ADVICE = ADVICE.Replace("# UNUSED TRANSLATABLES", string.Empty);
                                unusednotfound = false;//в целях оптимизации, проверка двоичного значения быстрее, чемискать в строке
                            }
                            buffer.AppendLine("# TEXT STRING");// + Environment.NewLine;
                            //if (THRPGMTransPatchFiles[i].blocks[y].Translation == "\r\n")
                            string TRANSLATION = THFilesElementsDataset.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
                            if (TRANSLATION.Length == 0)
                            {
                                buffer.AppendLine("# UNTRANSLATED");// + Environment.NewLine;
                            }
                            //buffer += "# CONTEXT : " + THRPGMTransPatchFiles[i].blocks[y].Context + Environment.NewLine;
                            buffer.AppendLine("# CONTEXT : " + THFilesElementsDataset.Tables[i].Rows[y][contextcolumnindex]);// + Environment.NewLine;
                            if (ADVICE.Length == 0)
                            {
                                //иногда # ADVICE отсутствует и при записи нужно пропускать запись этого пункта
                            }
                            else
                            {
                                //buffer += "# ADVICE : " + THRPGMTransPatchFiles[i].blocks[y].Advice + Environment.NewLine;
                                buffer.AppendLine("# ADVICE : " + ADVICE);// + Environment.NewLine;
                            }
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original;
                            buffer.AppendLine(THFilesElementsDataset.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine;
                            buffer.AppendLine("# TRANSLATION ");// + Environment.NewLine;
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation;
                            buffer.AppendLine(TRANSLATION);// + Environment.NewLine;
                            buffer.AppendLine("# END STRING" + Environment.NewLine);// + Environment.NewLine;
                        }
                        if (string.IsNullOrWhiteSpace(buffer.ToString()))
                        {
                        }
                        else
                        {
                            buffer.Remove(buffer.Length-2,2);//удаление лишнего символа \r\n с конца строки
                            //String _path = SelectedDir + "\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            string _path = Path.Combine(SelectedDir,THFilesElementsDataset.Tables[i].TableName + ".txt");
                            File.WriteAllText(_path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }


                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(SelectedDir,"RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(SelectedDir, "RPGMKTRANSPATCH"), string.Empty); ;
                    }
                }
                //pbstatuslabel.Visible = false;
                //pbstatuslabel.Text = string.Empty;
            }
            catch
            {
                //THInfoTextBox.Text = string.Empty;
                //THActionProgressBar.Visible = false;
                //THInfolabel.Invoke((Action)(() => THInfolabel.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Text = string.Empty));
                ProgressInfo(false, string.Empty);
                SaveInAction = false;
                return false;
            }
            finally
            {
                //THInfoTextBox.Text = string.Empty;
                //THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Text = string.Empty));
                ProgressInfo(false, string.Empty);
            }

            SaveInAction = false;


            //остановка таймера и запись времени
            //swatch.Stop();
            //LogToFile("time=" + swatch.Elapsed.ToString(), true);//asdf
            //THMsg.Show("time=" + time);

            return true;

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THAboutForm AboutForm = new THAboutForm();
            AboutForm.Show();
        }

        private void THTargetTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+Del function
            //https://stackoverflow.com/questions/18543198/why-cant-i-press-ctrla-or-ctrlbackspace-in-my-textbox
            if (e.Control)
            {
                //if (e.KeyCode == Keys.A)
                //{
                //    THTargetTextBox.SelectAll();
                //}
                if (e.KeyCode == Keys.Back)
                {
                    e.SuppressKeyPress = true;
                    int selStart = THTargetRichTextBox.SelectionStart;
                    while (selStart > 0 && THTargetRichTextBox.Text.Substring(selStart - 1, 1) == " ")
                    {
                        selStart--;
                    }
                    int prevSpacePos = -1;
                    if (selStart != 0)
                    {
                        prevSpacePos = THTargetRichTextBox.Text.LastIndexOf(' ', selStart - 1);
                    }
                    THTargetRichTextBox.Select(prevSpacePos + 1, THTargetRichTextBox.SelectionStart - prevSpacePos - 1);
                    THTargetRichTextBox.SelectedText = string.Empty;
                }
            }
        }

        private void THFiltersDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CheckFilterDGV();
            /*
            int cindx = e.ColumnIndex;
            //MessageBox.Show("e.ColumnIndex" + cindx);
            for (int i = 0; i < THFileElementsDataGridView.Rows.Count; i++) //сделать все видимыми
            {
                THFileElementsDataGridView.Rows[i].Visible = true;
            }

            bool allfiltersisempty = true;
            for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)//check if all filters is empty
            {
                if (THFiltersDataGridView.Rows[0].Cells[c].Value == null || string.IsNullOrEmpty(THFiltersDataGridView.Rows[0].Cells[c].Value.ToString()))
                {
                }
                else
                {
                    allfiltersisempty = false;
                    break;
                }
            }

            if (allfiltersisempty)//Возврат, если все фильтры пустые
            {
                return;
            }

            //http://www.cyberforum.ru/post5844571.html
            THFileElementsDataGridView.CurrentCell = null;
            for (int i = 0; i < THFileElementsDataGridView.Rows.Count; i++)
            {
                bool stringfound = false;//по умолчанию скрыть, не найдено
                for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)
                {
                    if (THFiltersDataGridView.Rows[0].Cells[c].Value == null)
                    {
                    }
                    else
                    {
                        string THFilteringColumnValue = THFiltersDataGridView.Rows[0].Cells[c].Value.ToString();
                        if (string.IsNullOrEmpty(THFilteringColumnValue))
                        {
                        }
                        else
                        {
                            if (THFiltersDataGridView.Rows[0].Cells[c].Value == null)
                            {
                            }
                            else
                            {
                                foreach (DataGridViewColumn column in THFileElementsDataGridView.Columns)
                                {
                                    //MessageBox.Show("THFiltersDataGridView.Columns[cindx].Name=" + THFiltersDataGridView.Columns[e.ColumnIndex].Name
                                    //    + "\r\nTHFileElementsDataGridView.Columns[cindx].Name=" + THFileElementsDataGridView.Columns[cindx].Name);
                                    if (cindx < THFileElementsDataGridView.Columns.Count - 1 Контроль на превышение лимита колонок, на всякий && THFiltersDataGridView.Columns[e.ColumnIndex].Name == THFileElementsDataGridView.Columns[cindx].Name)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        cindx += 1;
                                    }
                                }

                                if (THFileElementsDataGridView.Rows[i].Cells[cindx].Value == null)
                                {
                                    //THFileElementsDataGridView.Rows[i].Visible = false;
                                }
                                else if (THFileElementsDataGridView.Rows[i].Cells[cindx].Value.ToString().Contains(THFilteringColumnValue))
                                {
                                    stringfound = true; //строка найдена, показать
                                }
                                else
                                {
                                    //MessageBox.Show("THFileElementsDataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString()=" + THFileElementsDataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString());
                                    //THFileElementsDataGridView.Rows[i].Visible = false;
                                }
                            }
                        }
                    }
                }

                if (stringfound)
                {
                    THFileElementsDataGridView.Rows[i].Visible = true;
                }
                else
                {
                    THFileElementsDataGridView.Rows[i].Visible = false;
                }
            }
            */
        }

        private void CheckFilterDGV()
        {
            try
            {
                //private void DGVFilter()
                string OverallFilter = string.Empty;
                for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)
                {
                    if ((THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty).Length == 0)
                    {

                    }
                    else
                    {
                        //об экранировании спецсимволов
                        //http://skillcoding.com/Default.aspx?id=159
                        //https://webcache.googleusercontent.com/search?q=cache:irqjhHKbiFMJ:https://www.syncfusion.com/kb/4492/how-to-filter-special-characters-like-by-typing-it-in-dynamic-filter+&cd=6&hl=ru&ct=clnk&gl=ru
                        if (OverallFilter.Length == 0)
                        {
                            OverallFilter += "["+THFiltersDataGridView.Columns[c].Name + "] Like '%" + (THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty).Replace("'", "''").Replace("*", "[*]").Replace("%", "[%]").Replace("[", "-QB[BQ-").Replace("]", "[]]").Replace("-QB[BQ-", "[[]") + "%'";//-QB[BQ- - для исбежания проблем с заменой в операторе .Replace("]", "[]]"), после этого
                        }
                        else
                        {
                            OverallFilter += " AND ";
                            OverallFilter += "[" + THFiltersDataGridView.Columns[c].Name + "] Like '%" + (THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty).Replace("'", "''").Replace("*", "[*]").Replace("%", "[%]").Replace("[", "-QB[BQ-").Replace("]", "[]]").Replace("-QB[BQ-", "[[]") + "%'";//-QB[BQ- - для исбежания проблем с заменой в операторе .Replace("]", "[]]"), после этого
                        }
                    }
                }
                //also about sort:https://docs.microsoft.com/ru-ru/dotnet/api/system.data.dataview.rowfilter?view=netframework-4.8
                //THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].DefaultView.Sort = String.Empty;
                //THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].DefaultView.RowFilter = String.Empty;
                //MessageBox.Show("\""+OverallFilter+ "\string.Empty);
                //MessageBox.Show(string.Format("" + THFiltersDataGridView.Columns[e.ColumnIndex].Name + " LIKE '%{0}%'", THFiltersDataGridView.Rows[0].Cells[e.ColumnIndex].Value));
                //https://10tec.com/articles/why-datagridview-slow.aspx
                //THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].DefaultView.RowFilter = string.Format("" + THFiltersDataGridView.Columns[e.ColumnIndex].Name + " LIKE '%{0}%'", THFiltersDataGridView.Rows[0].Cells[e.ColumnIndex].Value);
                THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.RowFilter = OverallFilter;
            }
            catch
            {
            }
        }

        private THSettings THSettings;
        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (THSettings == null || THSettings.IsDisposed)
                {
                    THSettings = new THSettings(this);
                }

                if (THSettings.Visible)
                {
                    THSettings.Activate();
                }
                else
                {
                    THSettings.Show();
                    //поместить на передний план
                    //THSettings.TopMost = true;
                    //THSettings.TopMost = false;
                }
            }
            catch
            {
            }
        }

        //http://qaru.site/questions/180337/show-row-number-in-row-header-of-a-datagridview
        private void THFileElementsDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;

            string rowIdx = GetSelectedRowIndexInDatatable(e.RowIndex) + 1 + string.Empty;//здесь получаю реальный индекс из Datatable
            //string rowIdx = (e.RowIndex + 1) + string.Empty;

            StringFormat centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private int GetSelectedRowIndexInDatatable(int rowIndex)
        {
            return THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Rows
                .IndexOf(
                ( (DataRowView)THFileElementsDataGridView.Rows[rowIndex].DataBoundItem ).Row
                        );
        }

        //Пример виртуального режима
        //http://www.cyberforum.ru/post9306711.html
        private void THFileElementsDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //MessageBox.Show("THFileElementsDataGridView_CellValueNeeded");
            /*
            if (e.ColumnIndex == 0)
            {
                e.Value = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks[e.RowIndex].Original;
            }
            else
            {
                e.Value = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks[e.RowIndex].Translation;
            }
            */
        }

        private void THFileElementsDataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            //if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            //{
                //newRowNeeded = true;
                /*
                if (THFileElementsDataGridView.Rows.Count < THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count)
                {
                    THFileElementsDataGridView.Rows.Add();
                }
                */

                /*debug info
                //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.forms.datagridview.scroll?view=netframework-4.8
                System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
                messageBoxCS.AppendFormat("{0} = {1}", "ScrollOrientation", e.ScrollOrientation);
                messageBoxCS.AppendLine();
                messageBoxCS.AppendFormat("{0} = {1}", "Type", e.Type);
                messageBoxCS.AppendLine();
                messageBoxCS.AppendFormat("{0} = {1}", "NewValue", e.NewValue);
                messageBoxCS.AppendLine();
                messageBoxCS.AppendFormat("{0} = {1}", "OldValue", e.OldValue);
                messageBoxCS.AppendLine();
                MessageBox.Show(messageBoxCS.ToString(), "Scroll Event");
                */


                /*
                if (THFileElementsDataGridView.Rows.Count+500 > THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count)
                {
                    THFileElementsDataGridView.RowCount = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count;
                }
                else
                {
                    THFileElementsDataGridView.RowCount = THFileElementsDataGridView.Rows.Count + 500;
                }
                */
            //}


        }

        //bool newRowNeeded;
        private void THFileElementsDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            
            //if (e.RowIndex == numberOfRows /*newRowNeeded*/)
            //{
                //newRowNeeded = false;
                //numberOfRows = numberOfRows + 1;
                //THFileElementsDataGridView.Rows.Add();
            //}
            

        }

        private void THFileElementsDataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            //MessageBox.Show("hhhhhhhhhhhh");
            //newRowNeeded = true;
        }

        private void THFileElementsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {   cellchanged = true;
                FIleDataWasChanged = true;
            }
        }

        string dbpath;
        string lastautosavepath;
        private void SaveTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dbfilename = Path.GetFileNameWithoutExtension(THSelectedDir);
            string projecttypeDBfolder = string.Empty;
            if (THSelectedSourceType.Contains("RPG Maker MV"))
            {
                projecttypeDBfolder += "RPGMakerMV";
            }
            else if (THSelectedSourceType.Contains("RPGMaker"))
            {
                projecttypeDBfolder += "RPGMakerTransPatch";
            }
            dbpath = Path.Combine(Application.StartupPath, "DB", projecttypeDBfolder);
            lastautosavepath = Path.Combine(dbpath, dbfilename + GetDBCompressionExt());

            ProgressInfo(true);

            WriteDBFileLite(THFilesElementsDataset, lastautosavepath);
            //THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data

            Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath);

            ProgressInfo(false);
        }

        bool AutosaveActivated = false;
        private void Autosave()
        {
            if (AutosaveActivated || THFilesElementsDataset == null)
            {
            }
            else
            {
                AutosaveActivated = true;

                dbpath = Path.Combine(Application.StartupPath,"DB");
                string dbfilename = Path.GetFileNameWithoutExtension(THSelectedDir)+"_autosave";
                string autosavepath = Path.Combine(dbpath, "Auto", dbfilename + ".cmx");

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => SaveLoop(THFilesElementsDataset, autosavepath) ));
                trans.Start();

                //ProgressInfo(true);

                //WriteDBFile(THFilesElementsDataset, lastautosavepath);
                ////THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data

                //Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath);

                //ProgressInfo(false);
            }
        }

        /// <summary>
        /// Background autosave
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Path"></param>
        private void SaveLoop(DataSet Data, string Path)
        {
            //asdf autosave
            while (AutosaveActivated && Data != null && Path.Length > 0)
            {
                if (TheDataSetIsNotEmpty(Data))
                {
                }
                else//если dataset пустой, нет смысла его сохранять
                {
                    AutosaveActivated = false;
                    return;
                }

                int i = 0;
                while (i<60)
                {
                    Thread.Sleep(1000);
                    if (MainIsClosing || this == null || IsDisposed || Data == null || Path.Length == 0)
                    {
                        AutosaveActivated = false;
                        return;
                    }
                    i++;
                }
                while (IsOpeningInProcess || SaveInAction)//не запускать автосохранение, пока утилита занята
                {
                    Thread.Sleep(10000);
                }
                WriteDBFileLite(Data, Path);
            }
        }

        /// <summary>
        /// True if Translation cell of any row has value
        /// </summary>
        /// <param name="DS"></param>
        /// <returns></returns>
        private bool TheDataSetIsNotEmpty(DataSet DS)
        {
            if (DS == null)
            {
                return false;
            }

            try
            {
                int DSTablesCount = DS.Tables.Count;
                for (int t = 0; t < DSTablesCount; t++)
                {
                    var table = DS.Tables[t];
                    int rowscount = table.Rows.Count;
                    for (int r = 0; r < rowscount; r++)
                    {
                        var cell = table.Rows[r][1];
                        if (cell==null && string.IsNullOrEmpty(cell as string))
                        {
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {

            }

            return false;
        }

        private void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                IsOpeningInProcess = true;
                
                string dbfilename = Path.GetFileNameWithoutExtension(THSelectedDir);
                string projecttypeDBfolder = "\\";
                if (THSelectedSourceType.Contains("RPG Maker MV"))
                {
                    projecttypeDBfolder += "RPGMakerMV\\";
                }
                else if (THSelectedSourceType.Contains("RPGMakerTransPatch"))
                {
                    projecttypeDBfolder += "RPGMakerTransPatch\\";
                }
                dbpath = Application.StartupPath + "\\DB" + projecttypeDBfolder;
                lastautosavepath = Path.Combine(dbpath,dbfilename,GetDBCompressionExt());
                if (File.Exists(lastautosavepath))
                {
                    LoadTranslationFromDB(lastautosavepath);
                }

                IsOpeningInProcess = false;
            }
        }

        bool LoadTranslationToolStripMenuItem_ClickIsBusy = false;
        private async void LoadTranslationFromDB(string sPath = "")
        {
            if (LoadTranslationToolStripMenuItem_ClickIsBusy)
            {
                return;
            }
            LoadTranslationToolStripMenuItem_ClickIsBusy = true;

            //dbpath = Application.StartupPath + "\\DB";
            //string dbfilename = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");

            ProgressInfo(true);

            //lastautosavepath = dbpath + "\\Auto\\Auto" + dbfilename + GetDBCompressionExt();

            //WriteDBFile(THFilesElementsDataset, lastautosavepath);
            //THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data

            //THFilesElementsDataset.Reset();
            //THFilesListBox.Items.Clear();

            using (DataSet DBDataSet = new DataSet())
            {

                //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                await Task.Run(() => ReadDBAndLoadDBCompare(DBDataSet, sPath));
            }

            THFileElementsDataGridView.Refresh();
            /*
            THFileElementsDataGridView.DataSource = THFilesElementsDataset;

            foreach (DataTable t in THFilesElementsDataset.Tables)
            {
                THFilesListBox.Items.Add(t.TableName);
                //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", DateTime.Now + " >>: \"" + t.TableName + "\"\r\n", true);
            }
            //THFileElementsDataGridView.Refresh();
            if (THFilesListBox.SelectedIndex > -1)
            {
                THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex];
            }            
            */


            LoadTranslationToolStripMenuItem_ClickIsBusy = false;
        }

        private void ReadDBAndLoadDBCompare(DataSet DBDataSet, string sPath)
        {
            if (sPath.Length == 0)
            {
                //THFilesElementsDataset.ReadXml(Settings.THConfigINI.ReadINI("Paths", "LastAutoSavePath")); //load new data
                DBFile.ReadDBFile(DBDataSet, Settings.THConfigINI.ReadINI("Paths", "LastAutoSavePath")); //load new data
            }
            else
            {
                DBFile.ReadDBFile(DBDataSet, sPath); //load new data
            }
            THLoadDBCompare(DBDataSet);
            ProgressInfo(false);
        }

        private void THLoadDBCompare(DataSet THTempDS)
        {
            if (!IsFullComprasionDBloadEnabled && Table.IsDataSetsElementsCountIdentical(THFilesElementsDataset, THTempDS))
            {
                CompareLiteIfIdentical(THTempDS);
                return;
            }

            //using (DataSet THTempDS = new DataSet())
            //{
            //    //LogToFile("cleaning THTempDS and refreshing dgv", true);
            //    THTempDS.Reset();//очистка временной таблицы
            //}

            //Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath); // write lastsavedpath
                       
            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //LogToFile("ocol=" + ocol);
            //оптимизация. Не искать колонку перевода, если она по стандарту первая

            int ttranscol = THTempDS.Tables[0].Columns["Translation"].Ordinal;
            if (ttranscol == 0 || ttranscol == -1)
            {
                return;
            }

            //Оптимизация. Стартовые значения номера таблицы и строки для таблицы с загруженным переводом
            int ttablestartindex = 0;
            int trowstartindex = 0;

            int tcount = THFilesElementsDataset.Tables.Count;
            string infomessage = T._("loading translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                using (var Table = THFilesElementsDataset.Tables[t])
                {
                    string tableprogressinfo = infomessage + Table.TableName + ">" + t + "/" + tcount;
                    ProgressInfo(true, tableprogressinfo);

                    int rcount = Table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var Row = Table.Rows[r];
                        var CellTranslation = Row[otranscol];
                        if (CellTranslation == null || string.IsNullOrEmpty(CellTranslation as string))
                        {
                            bool TranslationWasSet = false;

                            var DBTablesCount = THTempDS.Tables.Count;
                            //проход по всем таблицам dataset с переводом
                            for (int t1 = ttablestartindex; t1 < DBTablesCount; t1++)
                            {
                                using (var DBTable = THTempDS.Tables[t1])
                                {
                                    if (DBTable.Columns.Count > 1)
                                    {
                                        var DBTableRowsCount = THTempDS.Tables[t1].Rows.Count;
                                        //проход по всем строкам таблицы dataset с переводом
                                        for (int r1 = trowstartindex; r1 < DBTableRowsCount; r1++)
                                        {
                                            var DBRow = DBTable.Rows[r1];
                                            var DBCellTranslation = DBRow[ttranscol];
                                            if (DBCellTranslation == null || string.IsNullOrEmpty(DBCellTranslation as string))
                                            {
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (Equals(Row[0], DBRow[0]))
                                                    {
                                                        THFilesElementsDataset.Tables[t].Rows[r][otranscol] = DBCellTranslation;
                                                        TranslationWasSet = true;

                                                        trowstartindex = IsFullComprasionDBloadEnabled ? 0 : r1;//запоминание последнего индекса строки, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
                                                        break;
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }
                                        if (TranslationWasSet)//если перевод был присвоен, выйти из цикла таблицы с переводом
                                        {
                                            ttablestartindex = IsFullComprasionDBloadEnabled ? 0 : t1;//запоминание последнего индекса таблицы, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
                                            break;
                                        }
                                        else
                                        {
                                            //сбрасывать индекс на ноль
                                            trowstartindex = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CompareLiteIfIdentical(DataSet tHTempDS)
        {
            int tcount = THFilesElementsDataset.Tables.Count;
            string infomessage = T._("loading translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                var DT = THFilesElementsDataset.Tables[t];
                string tableprogressinfo = infomessage + DT.TableName + ">" + t + "/" + tcount;
                ProgressInfo(true, tableprogressinfo);

                int rcount = DT.Rows.Count;
                //проход по всем строкам таблицы рабочего dataset
                for (int r = 0; r < rcount; r++)
                {
                    ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");

                    var TranslationRow = DT.Rows[r];
                    var TranslationCell = TranslationRow[1];
                    if (TranslationCell == null || string.IsNullOrEmpty(TranslationCell as string))
                    {
                        var DBRow = tHTempDS.Tables[t].Rows[r];
                        if (Equals(TranslationRow[0], DBRow[0]))
                        {
                            THFilesElementsDataset.Tables[t].Rows[r][1] = DBRow[1];
                        }
                    }
                }
            }
        }

        private string GetDBCompressionExt()
        {
            //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
            if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked") == "True")
            {
                //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression"));
                if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "XML (none)")
                {
                    //MessageBox.Show(".xml");
                    return ".xml";
                }
                else if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Gzip (cmx)")
                {
                    //MessageBox.Show(".cmx");
                    return ".cmx";
                }
                else if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Deflate (cmz)")
                {
                    //MessageBox.Show(".cmz");
                    return ".cmz";
                }

            }
            //MessageBox.Show("Default .xml");
            return ".xml";
        }
        
        private void LoadTrasnlationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                IsOpeningInProcess = true;
                using (OpenFileDialog THFOpenBD = new OpenFileDialog())
                {
                    THFOpenBD.Filter = "DB file|*.xml;*.cmx;*.cmz|XML-file|*.xml|Gzip compressed DB (*.cmx)|*.cmx|Deflate compressed DB (*.cmz)|*.cmz";
                    string projecttypeDBfolder = string.Empty;
                    if (THSelectedSourceType.Contains("RPG Maker MV"))
                    {
                        projecttypeDBfolder += "RPGMakerMV";
                    }
                    else if (THSelectedSourceType.Contains("TransPatch"))
                    {
                        projecttypeDBfolder += "RPGMakerTransPatch";
                    }

                    THFOpenBD.InitialDirectory = Path.Combine(Application.StartupPath,"DB",projecttypeDBfolder);

                    if (THFOpenBD.ShowDialog() == DialogResult.OK)
                    {
                        if (THFOpenBD.FileName.Length == 0)
                        {
                        }
                        else
                        {
                            //string spath = THFOpenBD.FileName;
                            //THFOpenBD.OpenFile().Close();
                            //MessageBox.Show(THFOpenBD.FileName);
                            LoadTranslationFromDB(THFOpenBD.FileName);
                        }
                    }
                }
                IsOpeningInProcess = false;
            }
        }

        private void TestWriteJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\Armors.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonItemsArmorsWeapons("Armors", jsondata);
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\System.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonSystem("System", jsondata);
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\Actors.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonActors("Actors", jsondata);
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\CommonEvents.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonCommonEvents("CommonEvents", jsondata);
            //WriteJson("CommonEvents", @"C:\\000 test RPGMaker MV data\\CommonEvents.json");
        }

        //моя функция деления строки на равные части с остатком и запись их в строковый массив
        public static string[] THSplit(string str, int chunkSize)
        {
            string[] parts = new string[chunkSize];

            int ind = 0;
            int strLength = str.Length;
            //памятка о приведении типов
            //https://www.aubrett.com/article/information-technology/web-development/net-framework/csharp/csharp-division-floating-point
            //THMsg.Show("strLength=" + strLength + ",str=" + str + ",f=" + f);
            int substrLength = (int)Math.Ceiling((double)strLength / chunkSize);//округление числа символов в части в большую сторону
            //THMsg.Show("f="+f+", substrLength=" + substrLength);
            int partsLength = parts.Length;
            for (int i = 0;i< partsLength; i++)
            {
                if (i == partsLength - 1)
                {
                    parts[i] = str.Substring(ind, strLength-ind);
                }
                else
                {
                    parts[i] = str.Substring(ind, substrLength);
                    ind += substrLength;
                }
            }

            return parts;
        }

        private void TestSplitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //string[] s = THSplit("test1test2ddd", 3);
            //string infoabouts = string.Empty;
            //for (int i = 0;i< s.Length; i++)
            //{
            //    infoabouts += ", s[" + i + "]=" + s[i];
            //}

            //THMsg.Show("s.Length=" + s.Length + infoabouts);
        }

        private bool GetAnyFileWithTheNameExist(string name)
        {
            //исключение имен с недопустимыми именами для файла или папки
            //http://www.cyberforum.ru/post5599483.html
            if (name.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                //MessageBox.Show("GetAnyFileWithTheNameExist return false because invalid! name=" + name);
                return false;
            }
            //MessageBox.Show("THSelectedDir=" + THSelectedDir.Replace("\\www\\data", "\\www") + "\r\n, name=" + name + "\r\n, Count=" + Directory.EnumerateFiles(THSelectedDir, name + ".*", SearchOption.AllDirectories).Count());
            if (Directory.EnumerateFiles(THSelectedDir, name + ".*", SearchOption.AllDirectories).Count() > 0)
            {
                //MessageBox.Show("GetAnyFileWithTheNameExist Returns True! name="+ name);
                return true;
            }
            //MessageBox.Show("GetAnyFileWithTheNameExist Returns False! name=" + name);
            return false;
        }

        private bool ReadJson(string Jsonname, string sPath)
        {
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                //MessageBox.Show(THSelectedDir);
                //using (var reader = new StreamReader(THSelectedDir+"\\"+ Jsonname+".json"))
                using (StreamReader reader = new StreamReader(sPath))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        root = JToken.Load(jsonReader);

                        //ReadJson(root, Jsonname);
                    }
                }

                //ds.Tables.Add(Jsonname); // create table with json name
                //ds.Tables[Jsonname].Columns.Add("Original"); //create Original column


                //treeView1.BeginUpdate();
                // treeView1.Nodes.Clear();
                //var tNode = treeView1.Nodes[treeView1.Nodes.Add(new TreeNode(rootName))];
                //tNode.Tag = root;
                IsCommonEvents = (Jsonname == "CommonEvents");
                ProceedJToken(root, Jsonname);


                //treeView1.ExpandAll();
            }
            catch
            {
                //LogToFile(string.Empty, true);
                return false;
            }
            finally
            {
                //LogToFile(string.Empty, true);
                //MessageBox.Show("sss");
                //ds.Tables[Jsonname].Columns.Add("Translation");
                //ds.Tables[Jsonname].Columns["Original"].ReadOnly = true;
                //DGV.DataSource = ds.Tables[0];
                //treeView1.EndUpdate();
            }
            //LogToFile(string.Empty, true);
            return true;

        }

        StringBuilder textsb = new StringBuilder();
        private string curcode = string.Empty;
        //string cType;
        //private string cCode = string.Empty;
        private string cName = string.Empty;
        //private string cId = string.Empty;
        //private string OldcId = "none";
        bool IsCommonEvents = false;
        private void ProceedJToken(JToken token, string Jsonname, string propertyname = "")
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                //if (cName == "code")
                //{
                //    curcode = token.ToString();
                //    //cCode = "Code" + curcode;
                //    //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                //}
                //else if (propertyname == "id")
                //{
                //    if (cId != OldcId)
                //    {
                //        OldcId = cId;
                //        cId = "ID" + token.ToString() + ":";
                //    }
                //}
                //LogToFile("JValue: " + propertyname + "=" + token.ToString()+", token path="+token.Path);
                string tokenvalue = token + string.Empty;

                if (IsCommonEvents && (curcode == "401" || curcode == "405"))
                {
                    if (token.Type == JTokenType.String)
                    {
                        if (textsb.ToString().Length == 0)
                        {
                        }
                        else
                        {
                            textsb.Append(Environment.NewLine);
                        }
                        //LogToFile("code 401 adding valur to merge=" + tokenvalue + ", curcode=" + curcode);
                        textsb.Append(tokenvalue);
                    }
                }
                else
                {
                    if (IsCommonEvents)
                    {
                        if (string.IsNullOrWhiteSpace(textsb.ToString()))
                        {
                        }
                        else
                        {
                            string mergedstring = textsb.ToString();
                            if (/*GetAlreadyAddedInTable(Jsonname, mergedstring) || token.Path.Contains(".json'].data[") ||*/ SelectedLocalePercentFromStringIsNotValid(mergedstring))
                            {
                            }
                            else
                            {
                                //LogToFile("textsb is not empty. add. value=" + mergedstring + ", curcode=" + curcode);
                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(mergedstring);
                                //JToken t = token;
                                //while (!string.IsNullOrEmpty(t.Parent.Path))
                                //{
                                //    t = t.Parent;
                                //    extra += "\\" + t.Path;
                                //}
                                THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add("JsonPath: " + token.Path);
                            }
                            textsb.Clear();
                        }
                    }
                    if (token.Type == JTokenType.String)
                    {
                        if (tokenvalue.Length == 0/* || GetAlreadyAddedInTable(Jsonname, tokenvalue)*/ || SelectedLocalePercentFromStringIsNotValid(tokenvalue) || GetAnyFileWithTheNameExist(tokenvalue))
                        {
                        }
                        else
                        {
                            //if (IsCommonEvents && curcode == "102")
                            //{
                            //    cName = "Choice";
                            //}

                            //LogToFile("Jsonname=" + Jsonname+ ", tokenvalue=" + tokenvalue);
                            //LogToFile(string.Empty, true);
                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(tokenvalue);
                            //dsinfo.Tables[0].Rows.Add(cType+"\\"+ cId + "\\" + cCode + "\\" + cName);
                            //JToken t = token;
                            //while (!string.IsNullOrEmpty(t.Parent.Path))
                            //{
                            //    t = t.Parent;
                            //    extra += "\\"+t.Path;
                            //}

                            THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add("JsonPath: " + token.Path);
                        }
                    }
                }
                //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                //childNode.Tag = token;
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    //cType = "JObject";
                    cName = property.Name;
                    //LogToFile("JObject propery: " + property.Name + "=" + property.Value+ ", token.Path=" + token.Path);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    //childNode.Tag = property;

                    if (IsCommonEvents)//asdfg skip code 108,408,356
                    {
                        if (cName == "code")
                        {
                            curcode = property.Value + string.Empty;
                            //cCode = "Code" + curcode;
                            //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                        }
                        if (skipit)
                        {
                            if (curcode == "108" || curcode == "408" || curcode == "356")
                            {
                                if (property.Name == "parameters")//asdf
                                {
                                    skipit = false;
                                    continue;
                                }
                            }
                            else
                            {
                                skipit = false;
                            }
                        }
                        else
                        {
                            if (cName == "code")
                            {
                                if (property.Value + string.Empty == "108" || property.Value + string.Empty == "408" || property.Value + string.Empty == "356")
                                {
                                    skipit = true;
                                    continue;
                                }
                            }
                        }
                    }
                    ProceedJToken(property.Value, Jsonname, property.Name);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    //LogToFile("JArray=\r\n" + array[i] + "\r\n, token.Path=" + token.Path);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    //childNode.Tag = array[i];
                    //cType = "JArray";
                    ProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }
        bool skipit = false;

        private bool WriteJson(string Jsonname, string sPath)
        {
            ProgressInfo(true, T._("Writing: ") + Jsonname + ".json");
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                //MessageBox.Show(THSelectedDir);
                //using (var reader = new StreamReader(THSelectedDir+"\\"+ Jsonname+".json"))
                using (StreamReader reader = new StreamReader(sPath))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        root = JToken.Load(jsonReader);

                        //ReadJson(root, Jsonname);
                    }
                }

                //ds.Tables.Add(Jsonname); // create table with json name
                //ds.Tables[Jsonname].Columns.Add("Original"); //create Original column


                //treeView1.BeginUpdate();
                // treeView1.Nodes.Clear();
                //var tNode = treeView1.Nodes[treeView1.Nodes.Add(new TreeNode(rootName))];
                //tNode.Tag = root;

                startingrow = 0;//сброс начальной строки поиска в табице
                IsCommonEvents = (Jsonname == "CommonEvents");
                if (IsCommonEvents)
                {
                    //сброс значений для CommonEvents
                    curcode = string.Empty;
                    cName = string.Empty;
                    skipit = false;
                }
                WProceedJToken(root, Jsonname);

                Regex regex = new Regex(@"^\[null,(.+)\]$");//Корректировка формата записываемого json так, как в файлах RPGMaker MV
                File.WriteAllText(sPath, regex.Replace(root.ToString(Formatting.None),"[\r\nnull,\r\n$1\r\n]"));

                //treeView1.ExpandAll();
            }
            catch
            {
                //LogToFile(string.Empty, true);
                ProgressInfo(false);
                return false;
            }
            finally
            {
                //LogToFile(string.Empty, true);
                //MessageBox.Show("sss");
                //ds.Tables[Jsonname].Columns.Add("Translation");
                //ds.Tables[Jsonname].Columns["Original"].ReadOnly = true;
                //DGV.DataSource = ds.Tables[0];
                //treeView1.EndUpdate();
            }
            //LogToFile(string.Empty, true);
            ProgressInfo(false);
            return true;

        }

        int startingrow = 0;//оптимизация. начальная строка, когда идет поиск по файлу, чтобы не искало каждый раз сначала при нахождении перевода будет переприсваиваться начальная строка на последнюю
        private void WProceedJToken(JToken token, string Jsonname, string propertyname = "")
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (token.Type == JTokenType.String)
                {
                    string tokenvalue = token + string.Empty;
                    if (tokenvalue.Length == 0 || SelectedLocalePercentFromStringIsNotValid(tokenvalue))
                    {
                    }
                    else
                    {

                        //if (Jsonname == "States" && tokenvalue.Contains("自動的に付加されます"))
                        //{
                        //    //LogToFile("tokenvalue=" + tokenvalue);
                        //}
                        string parameter0value = tokenvalue;
                        if (parameter0value.Length == 0)
                        {
                        }
                        else //if code not equal old code and newline is not empty
                        {
                            //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ
                            
                            for (int i1 = startingrow; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
                            {
                                if ((THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty).Length == 0)
                                {
                                }
                                else
                                {
                                    //Where здесь формирует новый массив из входного, из элементов входного, удовлетворяющих заданному условию
                                    //https://stackoverflow.com/questions/1912128/filter-an-array-in-c-sharp
                                    string[] origA = (THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty)
                                        .Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/)
                                        .Where(emptyvalues => (emptyvalues/*.Replace("\r", string.Empty)*/).Length != 0)
                                        .ToArray();//Все строки, кроме пустых, чтобы потом исключить из проверки
                                    int origALength = origA.Length;
                                    if (origALength == 0)
                                    {
                                        origA = new string[1];
                                        origA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty;
                                        //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                    }

                                    if (origALength > 0)
                                    {
                                        string[] transA = (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty)
                                            .Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/)
                                            .Where(emptyvalues => (emptyvalues/*.Replace("\r", string.Empty)*/).Length != 0)
                                            .ToArray();//Все строки, кроме пустых
                                        if (transA.Length == 0)
                                        {
                                            transA = new string[1];
                                            transA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty;
                                            //LogToFile("(transA.Length == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString());
                                        }
                                        string transmerged = string.Empty;
                                        if (transA.Length == origALength)//если количество строк в оригинале и переводе равно
                                        {
                                            //ничего не делать
                                        }
                                        else // если перевод вдруг был переведен так, что не равен количеством строк оригиналу, тогда поделить его на равные строки
                                        {
                                            if (transA.Length > 0) // но перед этим, если перевод больше одной строки
                                            {
                                                foreach (string ts in transA)
                                                {
                                                    transmerged += ts; // объединить все строки в одну
                                                }
                                            }

                                            //Это заменил расширением Where, что выше при задании массива, пустые строки будут исключены сразу
                                            //Проверить, есть ли в массиве хоть один пустой элемент
                                            //https://stackoverflow.com/questions/44405411/how-can-i-check-wether-an-array-contains-any-item-or-is-completely-empty
                                            //if (orig.Any(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)) ) )
                                            //А это считает количество пустых элементов в массиве
                                            //https://stackoverflow.com/questions/2391743/how-many-elements-of-array-are-not-null
                                            //int ymptyelementscnt = orig.Count(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)));

                                            transA = THSplit(transmerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
                                        }

                                        //LogToFile("parameter0value=" + parameter0value);
                                        //if (Jsonname == "States" && tokenvalue.Contains("自動的に付加されます"))
                                        //{
                                        //    //LogToFile("tokenvalue=" + tokenvalue + ", tablevalue=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                        //}
                                        //Подстраховочная проверка для некоторых значений из нескольких строк, полное сравнение перед построчной
                                        if (tokenvalue == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty)
                                        {
                                            var t = token as JValue;
                                            t.Value = (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty).Replace("\r", string.Empty);//убирает \r, т.к. в json присутствует только \n
                                            startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                            break;
                                        }

                                        bool br = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
                                        for (int i2 = 0; i2 < origALength; i2++)
                                        {
                                            //LogToFile("parameter0value=" + parameter0value);
                                            //if (Jsonname == "States" && parameter0value.Contains("自動的に付加されます"))
                                            //{
                                            //    //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                            //}

                                            //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                            if (parameter0value == origA[i2]/*.Replace("\r", string.Empty)*/) //Replace здесь убирает \r из за которой строки считались неравными
                                            {
                                                //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                                var t = token as JValue;
                                                t.Value = transA[i2]/*.Replace("\r", string.Empty)*/; //Replace убирает \r

                                                startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                                //LogToFile("commoneventsdata[i].List[c].Parameters[0].String=" + commoneventsdata[i].List[c].Parameters[0].String + ",trans[i2]=" + transA[i2]);
                                                br = true;
                                                break;
                                            }
                                        }
                                        if (br) //выход из цикла прохода по всей таблице, если значение найдено для одной из строк оригинала, и переход к следующему элементу json
                                        {
                                            startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        //LogToFile("tokenvalue=" + tokenvalue);
                                        //if (Jsonname == "States" && tokenvalue.Contains("自動的に付加されます"))
                                        //{
                                        //    //LogToFile("tokenvalue=" + tokenvalue + ", tablevalue=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                        //}
                                        if (tokenvalue == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty)
                                        {
                                            var t = token as JValue;
                                            t.Value = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty;
                                            startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                //childNode.Tag = token;
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    //LogToFile("JObject propery: " + property.Name + "=" + property.Value);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    //childNode.Tag = property;
                    //cType = "JObject";
                    cName = property.Name;
                    if (IsCommonEvents)//asdfg skip code 108,408,356
                    {
                        if (cName == "code")
                        {
                            curcode = property.Value + string.Empty;
                            //cCode = "Code" + curcode;
                            //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                        }
                        if (skipit)
                        {
                            if (curcode == "108" || curcode == "408" || curcode == "356")
                            {
                                if (property.Name == "parameters")//asdf
                                {
                                    skipit = false;
                                    continue;
                                }
                            }
                            else
                            {
                                skipit = false;
                            }
                        }
                        else
                        {
                            if (cName == "code")
                            {
                                if (property.Value + string.Empty == "108" || property.Value + string.Empty == "408" || property.Value + string.Empty == "356")
                                {
                                    skipit = true;
                                    continue;
                                }
                            }
                        }
                    }
                    WProceedJToken(property.Value, Jsonname, property.Name);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    //LogToFile("JArray=\r\n" + array[i]);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    //childNode.Tag = array[i];
                    //cType = "JArray";
                    WProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        bool savemenusNOTenabled = true;
        private void THFileElementsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (FIleDataWasChanged && savemenusNOTenabled)
                {
                    writeTranslationInGameToolStripMenuItem.Enabled = true;
                    saveToolStripMenuItem.Enabled = true;
                    saveAsToolStripMenuItem.Enabled = true;
                    saveTranslationToolStripMenuItem.Enabled = true;
                    saveTranslationAsToolStripMenuItem.Enabled = true;
                }

                int tableind = THFilesList.SelectedIndex;
                int rind = THFileElementsDataGridView.CurrentCell.RowIndex;
                int cind = THFileElementsDataGridView.Columns["Original"].Index;

                if (rind > THFilesElementsDataset.Tables[tableind].Rows.Count)
                {
                }
                else
                {
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread trans = new Thread(new ParameterizedThreadStart((obj) => THAutoSetSameTranslationForSimular(tableind, rind, cind, false)));
                    trans.Start();
                }
                //if (THFilesElementsDataset.Tables[tableind].AsEnumerable().All(dr => !string.IsNullOrEmpty(dr["name"] + string.Empty)))
                //{
                //    //asdfg
                //}

                //Запуск автосохранения
                Autosave();
            }
            catch
            {
            }
        }

        bool IsTranslating = false;
        private void OnlineTranslateSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THFileElementsDataGridView.SelectedCells.Count > 0)
            {
                if (IsTranslating)
                {
                    THMsg.Show(T._("Already in process.."));
                    return;
                }
                IsTranslating = true;

                //координаты стартовой строк, колонки оригинала и номера таблицы
                int cind = THFileElementsDataGridView.Columns["Original"].Index;//-поле untrans
                int tableindex = THFilesList.SelectedIndex;
                int[] selindexes = new int[THFileElementsDataGridView.SelectedCells.Count];

                for (int i = 0; i < selindexes.Length; i++)
                {
                    //по нахождению верного индекса строки
                    //https://stackoverflow.com/questions/50999121/displaying-original-rowindex-after-filter-in-datagridview
                    //https://stackoverflow.com/questions/27125494/get-index-of-selected-row-in-filtered-datagrid
                    //DataRow r = ((DataRowView)BindingContext[THFileElementsDataGridView.DataSource].).Row;
                    //selindexes[i] = r.Table.Rows.IndexOf(r); //находит верный но только длявыбранной ячейки
                    //
                    //DataGridViewRow to DataRow: https://stackoverflow.com/questions/1822314/how-do-i-get-a-datarow-from-a-row-in-a-datagridview
                    //DataRow row = ((DataRowView)THFileElementsDataGridView.SelectedCells[i].OwningRow.DataBoundItem).Row;
                    //int index = THFilesElementsDataset.Tables[tableindex].Rows.IndexOf(row);
                    int index = GetSelectedRowIndexInDatatable(THFileElementsDataGridView.SelectedCells[i].RowIndex);
                    selindexes[i] = index;

                    //selindexes[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                }

                //THMsg.Show("selindexes[0]=" + selindexes[0] + "\r\ncind=" + cind + "\r\ntableindex=" + tableindex + "\r\nselected=" + selindexes.Length + ", lastselectedrowvalue=" + THFilesElementsDataset.Tables[tableindex].Rows[selindexes[0]][cind]);

                //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                //почемуто так не переводит, переводчик кидает ошибку при заппуске в другом потоке
                //await Task.Run(() => OnlineTranslateSelected(cind, tableindex, selindexes));  

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => THOnlineTranslate(cind, tableindex, selindexes, "s")));
                //
                //..и фикс ошибки:
                //System.TypeInitializationException: Инициализатор типа "TranslationHelper.GoogleAPI" выдал исключение. ---> System.Threading.ThreadStateException: Создание экземпляра элемента управления ActiveX '8856f961-340a-11d0-a96b-00c04fd705a2' невозможно: текущий поток не находится в однопоточном контейнере
                //https://ru.stackoverflow.com/questions/412073/c-webbrowser-threadstateexception-%D0%9E%D0%B4%D0%BD%D0%BE%D0%BF%D0%BE%D1%82%D0%BE%D1%87%D0%BD%D1%8B%D0%B9-%D0%BA%D0%BE%D0%BD%D1%82%D0%B5%D0%B9%D0%BD%D0%B5%D1%80
                trans.SetApartmentState(ApartmentState.STA);
                //но при выборе только одной строчки почему-то кидает исключение
                trans.Start();

                //OnlineTranslateSelected(cind, tableindex, selindexes);
                //ProgressInfo(false);
            }

        }

        private void OnlineTranslateTableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (IsTranslating)
            {
                THMsg.Show(T._("Already in process.."));
                return;
            }
            IsTranslating = true;

            try
            {
                if (THFilesList.SelectedItem == null)
                {
                    IsTranslating = false;
                    return;
                }
                //координаты стартовой строк, колонки оригинала и номера таблицы
                int cind = THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Original"].Ordinal;//-поле untrans
                int tableindex = THFilesList.SelectedIndex;
                int[] selindexes = new int[1];

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => THOnlineTranslate(cind, tableindex, selindexes, "t")));
                //
                //..и фикс ошибки:
                //System.TypeInitializationException: Инициализатор типа "TranslationHelper.GoogleAPI" выдал исключение. ---> System.Threading.ThreadStateException: Создание экземпляра элемента управления ActiveX '8856f961-340a-11d0-a96b-00c04fd705a2' невозможно: текущий поток не находится в однопоточном контейнере
                //https://ru.stackoverflow.com/questions/412073/c-webbrowser-threadstateexception-%D0%9E%D0%B4%D0%BD%D0%BE%D0%BF%D0%BE%D1%82%D0%BE%D1%87%D0%BD%D1%8B%D0%B9-%D0%BA%D0%BE%D0%BD%D1%82%D0%B5%D0%B9%D0%BD%D0%B5%D1%80
                trans.SetApartmentState(ApartmentState.STA);
                //но при выборе только одной строчки почему-то кидает исключение
                trans.Start();
            }
            catch
            {
                IsTranslating = false;
            }
        }

        private void OnlineTranslateAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (IsTranslating)
            {
                THMsg.Show(T._("Already in process.."));
                return;
            }
            IsTranslating = true;

            try
            {
                //int cind = THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;//-поле untrans
                //int tableindex = 0;
                //int[] selindexes = new int[1];

                //string[] input = new string[THFilesElementsDataset.Tables[0].Rows.Count];
                //for (int r = 0; r < THFilesElementsDataset.Tables[0].Rows.Count; r++)
                //{
                //    input[r] = THFilesElementsDataset.Tables[0].Rows[r][cind].ToString();
                //}

                //string[] output = GoogleAPI.TranslateMultiple(input, "jp", "en");

                //for (int r = 0; r < THFilesElementsDataset.Tables[0].Rows.Count; r++)
                //{
                //    THFilesElementsDataset.Tables[0].Rows[r][1] = output[r];
                //}


                //координаты стартовой строк, колонки оригинала и номера таблицы
                int cind = THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;//-поле untrans
                int tableindex = 0;
                int[] selindexes = new int[1];

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THOnlineTranslate(cind, tableindex, selindexes, "a")));
                //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THOnlineTranslateByBigBlocks(cind, tableindex, selindexes, "a")));
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => THOnlineTranslateByBigBlocks2(cind, tableindex, selindexes, "a")));
                //
                //..и фикс ошибки:
                //System.TypeInitializationException: Инициализатор типа "TranslationHelper.GoogleAPI" выдал исключение. ---> System.Threading.ThreadStateException: Создание экземпляра элемента управления ActiveX '8856f961-340a-11d0-a96b-00c04fd705a2' невозможно: текущий поток не находится в однопоточном контейнере
                //https://ru.stackoverflow.com/questions/412073/c-webbrowser-threadstateexception-%D0%9E%D0%B4%D0%BD%D0%BE%D0%BF%D0%BE%D1%82%D0%BE%D1%87%D0%BD%D1%8B%D0%B9-%D0%BA%D0%BE%D0%BD%D1%82%D0%B5%D0%B9%D0%BD%D0%B5%D1%80
                trans.SetApartmentState(ApartmentState.STA);
                //но при выборе только одной строчки почему-то кидает исключение
                trans.Start();
            }
            catch
            {
                //IsTranslating = false;
            }
            IsTranslating = false;
        }

        private void THOnlineTranslateByBigBlocks2(int cind, int tableindex, int[] rowindexes, string range)
        {
            try
            {
                translationInteruptToolStripMenuItem.Visible = true;

                using (DataSet THTranslationCache = new DataSet())
                {
                    TranslationCacheInit(THTranslationCache);

                    int maxchars = 1000; //большие значения ломаю ответ сервера, например отсутствует или ломается разделитель при значении 1000, потом надо будет подстроить идеальный максимум
                    int CurrentCharsCount = 0;
                    string InputOriginalLine;

                    using (DataTable InputLines = new DataTable())
                    {
                        using (DataTable InputLinesInfo = new DataTable())
                        {
                            InputLines.Columns.Add("Original");

                            InputLinesInfo.Columns.Add("Original");
                            InputLinesInfo.Columns.Add("Translation");
                            InputLinesInfo.Columns.Add("Table");
                            InputLinesInfo.Columns.Add("Row");

                            int tcount = THFilesElementsDataset.Tables.Count;
                            for (int t = 0; t < tcount; t++)
                            {
                                var Table = THFilesElementsDataset.Tables[t];
                                int rcount = Table.Rows.Count;
                                for (int r = 0; r < rcount; r++)
                                {
                                    var Row = Table.Rows[r];
                                    if (InteruptTranslation)
                                    {
                                        translationInteruptToolStripMenuItem.Visible = false;
                                        Thread.CurrentThread.Interrupt();
                                        ProgressInfo(false);
                                        return;
                                    }
                                    ProgressInfo(true, T._("translating")+": "+t+"/"+tcount+" ("+r+"/"+rcount+")");

                                    InputOriginalLine = Row[0] as string;

                                    bool TranslateIt=false;
                                    TranslateIt = (CurrentCharsCount + InputOriginalLine.Length) >= maxchars || r == rcount - 1;

                                    var Cell = Row[1];
                                    if (Cell == null || string.IsNullOrEmpty(Cell as string))
                                    {
                                        string InputOriginalLineFromCache = TranslationCacheFind(THTranslationCache, InputOriginalLine);//поиск оригинала в кеше
                                        
                                        if (InputOriginalLineFromCache.Length > 0)
                                        {
                                            THFilesElementsDataset.Tables[t].Rows[r][1] = InputOriginalLineFromCache;
                                            continue;
                                        }
                                        else
                                        {
                                            CurrentCharsCount += InputOriginalLine.Length;

                                            string[] Lines = InputOriginalLine.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
                                            string extractedvalue;
                                            string cache;

                                            if (Lines.Length > 1) //если строк больше одной
                                            {
                                                for (int s = 0; s < Lines.Length; s++) //добавить все непустые строки по отдельности, пустые добавить в Info
                                                {
                                                    string linevalue = Lines[s];

                                                    string Translation = string.Empty;
                                                    cache = TranslationCacheFind(THTranslationCache, linevalue);//поиск подстроки в кеше
                                                    if (cache.Length > 0)
                                                    {
                                                        Translation = cache;
                                                    }
                                                    else
                                                    {
                                                        if (linevalue.Length > 0)
                                                        {
                                                            extractedvalue = THExtractTextForTranslation(linevalue);//извлечение подстроки

                                                            // только если извлеченное значение отличается от оригинальной строки
                                                            cache = extractedvalue == linevalue ? string.Empty : TranslationCacheFind(THTranslationCache, extractedvalue);//поиск извлеченной подстроки в кеше
                                                            if (cache.Length > 0)
                                                            {
                                                                Translation = PasteTranslationBackIfExtracted(cache, linevalue, extractedvalue);
                                                            }
                                                            else
                                                            {
                                                                InputLines.Rows.Add(extractedvalue.Length == 0 ? linevalue : extractedvalue);
                                                            }
                                                        }
                                                    }
                                                    InputLinesInfo.Rows.Add(linevalue, Translation, t, r);
                                                }
                                            }
                                            else
                                            {
                                                cache = TranslationCacheFind(THTranslationCache, InputOriginalLine);
                                                if (cache.Length > 0)
                                                {
                                                    THFilesElementsDataset.Tables[t].Rows[r][1] = cache;
                                                }
                                                else
                                                {
                                                    //с одной строкой просто добавить её в таблицы
                                                    extractedvalue = THExtractTextForTranslation(InputOriginalLine);
                                                    InputLines.Rows.Add(extractedvalue.Length == 0 ? InputOriginalLine : extractedvalue);
                                                    InputLinesInfo.Rows.Add(InputOriginalLine, string.Empty, t, r);
                                                }

                                            }

                                        }
                                    }

                                    if (TranslateIt)
                                    {
                                        CurrentCharsCount = 0;

                                        if (InputLines.Rows.Count > 0)
                                        {
                                            TranslateLinesAndSetTranslation(InputLines, InputLinesInfo, THTranslationCache);
                                        }
                                        else if (InputLinesInfo.Rows.Count > 0)
                                        {
                                            int PreviousTableIndex = -1;
                                            int PreviousRowIndex = -1;
                                            int NewTableIndex;
                                            int NewRowIndex;
                                            int rowscount = InputLinesInfo.Rows.Count;
                                            StringBuilder ResultValue = new StringBuilder(rowscount);
                                            for (int i = 0; i < rowscount; i++)
                                            {
                                                var row = InputLinesInfo.Rows[i];
                                                NewTableIndex = int.Parse(row[2] as string);
                                                NewRowIndex = int.Parse(row[3] as string);
                                                if (string.IsNullOrEmpty(row[0] as string))
                                                {
                                                    ResultValue.Append(Environment.NewLine);
                                                }
                                                else if (!string.IsNullOrEmpty(row[1] as string))
                                                {
                                                    ResultValue.Append(row[1]);
                                                }
                                                if (NewRowIndex == PreviousRowIndex && i < rowscount-1)
                                                {
                                                    ResultValue.Append(Environment.NewLine);
                                                }
                                                else
                                                {
                                                    SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue, THTranslationCache);
                                                }
                                                PreviousTableIndex = NewTableIndex;
                                                PreviousRowIndex = NewRowIndex;
                                            }
                                        }
                                        WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);//промежуточная запись кеша

                                        InputLines.Rows.Clear();
                                        InputLinesInfo.Rows.Clear();
                                    }
                                }
                            }
                        }
                    }

                    WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            ProgressInfo(false);
        }

        private void TranslateLinesAndSetTranslation(DataTable InputLines, DataTable InputLinesInfo, DataSet THTranslationCache)
        {
            //https://www.codeproject.com/Questions/722877/DataTable-to-string-array
            string[] OriginalLines = InputLines.Rows.OfType<DataRow>().Select(row => row[0].ToString()).ToArray();

            string[] TranslatedLines = GoogleAPI.TranslateMultiple(OriginalLines);

            //int infoCount = InputLinesInfo.Rows.Count;
            //int TranslatedCount = TranslatedLines.Length-1; // -1 - отсекание последнего пустого элемента

            if (TranslatedLines == null || TranslatedLines.Length == 0)
            {
            }
            else
            {
                StringBuilder ResultValue = new StringBuilder();
                int PreviousTableIndex = -1;
                int PreviousRowIndex = -1;
                int i2 = 0;

                int TableIndex=0;
                int RowIndex=0;

                int TranslatedLinesLength = TranslatedLines.Length;
                for (int i = 0; i < TranslatedLinesLength; i++)
                {
                    string TranslatedLine = TranslatedLines[i];
                    var InfoRow = InputLinesInfo.Rows[i2];
                    //string fdsfsdf = TranslatedLines[i];
                    //string dfsgsdg1 = fdsfsdf;
                    //--------------------------------
                    //Блок считывания индеков таблицы и строки
                    //добавление переноса, если строка той же ячейки, либо запись результата, если уже новая ячейка
                    //string prelastvalue="";
                    //string lastvalue = "";
                    //try
                    //{
                    //    int inforowscount = InputLinesInfo.Rows.Count;
                    //    if (inforowscount > 0)
                    //    {
                    //        if (inforowscount > 1)
                    //        {
                    //            prelastvalue = InputLinesInfo.Rows[InputLines.Rows.Count - 2][1] + string.Empty;

                    //        }
                    //        lastvalue = InputLinesInfo.Rows[InputLinesInfo.Rows.Count - 1][1] + string.Empty;

                    //    }
                    TableIndex = int.Parse(InfoRow[2] as string);
                    RowIndex = int.Parse(InfoRow[3] as string);
                    //}
                    //catch(Exception ex)
                    //{
                    //    MessageBox.Show(ex.ToString());
                    //}
                    //string asdasd = prelastvalue;
                    //string adsasdaaa = lastvalue;

                    if (RowIndex == PreviousRowIndex)
                    {
                        ResultValue.Append(Environment.NewLine);                       
                    }
                    else
                    {
                        SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue, THTranslationCache);
                    }

                    //--------------------------------
                    //Блок записи пустой строки или кеша из информации, если они были и увеличение i2 на 1

                    bool WritedFromInfo = true;
                    while (WritedFromInfo)
                    {
                        WritedFromInfo = false;
                        if (string.IsNullOrEmpty(InfoRow[0] as string))
                        {
                            //ResultValue.Append(string.Empty); //закоментировано для оптимизации, тот же эффект добавление пустой строки
                            WritedFromInfo = true;
                            i2++;
                        }
                        else if (!string.IsNullOrEmpty(InfoRow[1] as string))
                        {
                            ResultValue.Append(InfoRow[1]);
                            WritedFromInfo = true;
                            i2++;
                        }

                        if (WritedFromInfo)//контроль, когда было записано из Info , предотвращает запись лишнего переноса
                        {
                            PreviousRowIndex = RowIndex;
                            PreviousTableIndex = TableIndex;
                            //--------------------------------
                            //Блок считывания индеков таблицы и строки
                            //добавление переноса, если строка той же ячейки, либо запись результата, если уже новая ячейка
                            InfoRow = InputLinesInfo.Rows[i2];//еще раз переприсваивание, т.к. i2 поменялось
                            TableIndex = int.Parse(InfoRow[2] as string);
                            RowIndex = int.Parse(InfoRow[3] as string);

                            if (RowIndex == PreviousRowIndex)
                            {
                                ResultValue.Append(Environment.NewLine);
                            }
                            else
                            {
                                SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue, THTranslationCache);
                            }

                            //--------------------------------
                        }
                    }

                    ResultValue.Append(PasteTranslationBackIfExtracted(TranslatedLine, InfoRow[0] as string, InputLines.Rows[i][0] as string));

                    AddToTranslationCacheIfValid(THTranslationCache, InfoRow[0] as string, TranslatedLine);

                    PreviousRowIndex = RowIndex;
                    PreviousTableIndex = TableIndex;
                    i2++;
                }
                SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue, THTranslationCache);
            }
        }

        private void AddToTranslationCacheIfValid(DataSet THTranslationCache, string Original, string Translation)
        {
            if (IsTranslationCacheEnabled)
            {
                DataTable Table = THTranslationCache.Tables[0];
                if (string.CompareOrdinal(Original, Translation) == 0 || Original.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None).Length != Translation.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None).Length || GetAlreadyAddedInTable(Table, Original))
                {
                }
                else
                {
                    Table.Rows.Add(Original, Translation);
                }
            }
        }

        private void SetTranslationResultToCellIfEmpty(int PreviousTableIndex, int PreviousRowIndex, StringBuilder ResultValue, DataSet THTranslationCache)
        {
            if (ResultValue.Length > 0 && PreviousTableIndex > -1 && PreviousRowIndex > -1)
            {
                string s; //иногда значения без перевода и равны оригиналу, но отдельным переводом выбранной ячейки получается нормально
                var Row = THFilesElementsDataset.Tables[PreviousTableIndex].Rows[PreviousRowIndex];
                var Cell = Row[0];
                if (Equals(Cell, ResultValue))
                {
                    s = GoogleAPI.Translate(Cell as string);
                }
                else
                {
                    s = ResultValue.ToString();
                }

                var TranslationCell = Row[1];
                if (TranslationCell == null || string.IsNullOrEmpty(TranslationCell as string))
                {
                    THFilesElementsDataset.Tables[PreviousTableIndex].Rows[PreviousRowIndex][1] = s;

                    AddToTranslationCacheIfValid(THTranslationCache, Cell as string, s);

                    THAutoSetSameTranslationForSimular(PreviousTableIndex, PreviousRowIndex, 0);
                }
                ResultValue.Clear();
            }
        }

        private string PasteTranslationBackIfExtracted(string Translation, string Original, string Extracted)
        {
            if (Translation.Length == 0 || Original.Length == 0 || Extracted.Length == 0 || Equals(Original, Extracted))
            {
                return Translation;
            }
            //переделано через удаление и вставку строки, чтобы точно вставлялась нужная
            //строка в нужное место и с рассчетом на будущее, когда возможно строки будут выдираться из исходной
            //, а потом вставляться обратно
            int IndexOfTheString = Original.IndexOf(Extracted);
            if (IndexOfTheString > -1)
            {
                return Original.Remove(IndexOfTheString, Extracted.Length).Insert(IndexOfTheString, Translation);
            }
            else
            {
                return Translation;
            }
        }

        //DataSet THTranslationCache = new DataSet();
        public static void TranslationCacheInit(DataSet DS)
        {
            DS.Reset();
            if (File.Exists(THTranslationCachePath))
            {
                DBFile.ReadDBFile(DS, THTranslationCachePath);
            }
            else
            {
                DS.Tables.Add("TranslationCache");
                DS.Tables["TranslationCache"].Columns.Add("Original");
                DS.Tables["TranslationCache"].Columns.Add("Translation");
            }
            //MessageBox.Show("TranslationCache Rows.Count=" + THTranslationCache.Tables["TranslationCache"].Rows.Count+ "TranslationCache Columns.Count=" + THTranslationCache.Tables["TranslationCache"].Columns.Count);
        }

        public string TranslationCacheFind(DataSet DS, string Input)
        {
            if (IsTranslationCacheEnabled)
            {
                if (Input.Length > 0)
                {
                    using (var Table = DS.Tables[0])
                    {
                        if (Table.Rows.Count > 0)
                        {
                            if (GetAlreadyAddedInTable(Table, Input))
                            {
                                var RowsCount = Table.Rows.Count;
                                for (int i = 0; i < RowsCount; i++)
                                {
                                    var row = Table.Rows[i];
                                    //MessageBox.Show("Input=" + Input+"\r\nCache="+ THTranslationCache.Tables["TranslationCache"].Rows[i][0].ToString());
                                    if (Input == row[0] as string)
                                    {
                                        return row[1] as string;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static void THTranslationCacheAdd(DataSet DS, string original, string translation)
        {
            //LogToFile("original=" + original+ ",translation=" + translation,true);
            DS.Tables[0].Rows.Add(original, translation);            
        }


        private void THOnlineTranslate(int cind, int tableindex, int[] selindexes, string method="s")
        {
            this.Invoke((Action)(() => translationInteruptToolStripMenuItem.Visible = true));
            this.Invoke((Action)(() => translationInteruptToolStripMenuItem1.Visible = true));
            //translationInteruptToolStripMenuItem.Visible = true;
            //translationInteruptToolStripMenuItem1.Visible = true;

            try
            {
                using (DataSet THTranslationCache = new DataSet())
                {
                    TranslationCacheInit(THTranslationCache);

                    //количество таблиц, строк и индекс троки для использования в переборе строк и таблиц
                    int tablescount;
                    int rowscount;
                    int rowindex;
                    tablescount = (method == "a") ? THFilesElementsDataset.Tables.Count : tablescount = tableindex + 1;
                    //if (method == "a")
                    //{
                    //    tablescount = THFilesElementsDataset.Tables.Count;//все таблицы в dataset
                    //}
                    //else
                    //{
                    //    tablescount = tableindex + 1;//одна таблица с индексом на один больше индекса стартовой
                    //}

                    //перебор таблиц dataset
                    for (int t = tableindex; t < tablescount; t++)
                    {
                        rowscount = (method == "a" || method == "t") ? THFilesElementsDataset.Tables[t].Rows.Count : selindexes.Length;
                        //if (method == "a" || method == "t")
                        //{
                        //    //все строки в выбранной таблице
                        //    rowscount = THFilesElementsDataset.Tables[t].Rows.Count;
                        //}
                        //else
                        //{
                        //    //все выделенные строки в выбранной таблице
                        //    rowscount = selindexes.Length;
                        //}

                        //перебор строк таблицы
                        for (int i = 0; i < rowscount; i++)
                        {
                            if (InteruptTranslation)
                            {
                                //translationInteruptToolStripMenuItem.Visible = false;
                                //translationInteruptToolStripMenuItem1.Visible = false;
                                this.Invoke((Action)(() => translationInteruptToolStripMenuItem.Visible = false));
                                this.Invoke((Action)(() => translationInteruptToolStripMenuItem1.Visible = false));
                                InteruptTranslation = false;
                                return;
                            }

                            string progressinfo;
                            if (method == "s")
                            {
                                progressinfo = T._("getting translation: ") + (i + 1) + "/" + rowscount;
                                //индекс = первому из заданного списка выбранных индексов
                                rowindex = selindexes[i];
                            }
                            else if (method == "t")
                            {
                                progressinfo = T._("getting translation: ") + (i + 1) + "/" + rowscount;
                                //индекс с нуля и до последней строки
                                rowindex = i;
                            }
                            else
                            {
                                progressinfo = T._("getting translation: ") + t+"/"+ tablescount+"::" + (i + 1) + "/" + rowscount;
                                //индекс с нуля и до последней строки
                                rowindex = i;
                            }

                            ProgressInfo(true, progressinfo);
                            //LogToFile("111=" + 111, true);
                            //проверка пустого значения поля для перевода
                            //if (THFileElementsDataGridView[cind + 1, rind].Value == null || string.IsNullOrEmpty(THFileElementsDataGridView[cind + 1, rind].Value.ToString()))
                            if ((THFilesElementsDataset.Tables[t].Rows[rowindex][cind + 1]+string.Empty).Length==0)
                            {
                                string InputValue = THFilesElementsDataset.Tables[t].Rows[rowindex][cind] + string.Empty;
                                //LogToFile("1 inputvalue=" + inputvalue, true);
                                //проверка наличия заданного процента romaji или other в оригинале
                                //if ( SelectedLocalePercentFromStringIsNotValid(THFileElementsDataGridView[cind, rind].Value.ToString()) || SelectedLocalePercentFromStringIsNotValid(THFileElementsDataGridView[cind, rind].Value.ToString(), "other"))

                                string ResultValue = TranslationCacheFind(THTranslationCache, InputValue);

                                if (ResultValue.Length != 0)
                                {
                                    THFilesElementsDataset.Tables[t].Rows[rowindex][cind + 1] = ResultValue;
                                    //THAutoSetValueForSameCells(t, rowindex, cind);
                                }
                                else
                                {
                                    //LogToFile("resultvalue from cache is empty. resultvalue=" + resultvalue, true);
                                    string[] inputvaluearray = InputValue.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
                                    if (inputvaluearray.Length > 1)
                                    {
                                        ResultValue = TranslateMultilineValue(inputvaluearray, THTranslationCache);
                                    }
                                    else
                                    {
                                        string ExtractedValue = THExtractTextForTranslation(InputValue);
                                        //LogToFile("extractedvalue="+ extractedvalue,true);
                                        if (ExtractedValue.Length == 0 || ExtractedValue == InputValue)
                                        {
                                            ResultValue = GoogleAPI.Translate(InputValue);

                                            //LogToFile("resultvalue=" + resultvalue, true);
                                            AddToTranslationCacheIfValid(THTranslationCache, InputValue, ResultValue);
                                        }
                                        else
                                        {
                                            string CachedExtractedValue = TranslationCacheFind(THTranslationCache, ExtractedValue);
                                            //LogToFile("cachedvalue=" + cachedvalue, true);
                                            if (CachedExtractedValue.Length == 0)
                                            {
                                                string OnlineValue = GoogleAPI.Translate(ExtractedValue);//из исходников ESPTranslator 

                                                if (Equals(ExtractedValue, OnlineValue))
                                                {
                                                }
                                                else
                                                {
                                                    //resultvalue = inputvalue.Replace(extractedvalue, onlinevalue);
                                                    ResultValue = PasteTranslationBackIfExtracted(OnlineValue, InputValue, ExtractedValue);

                                                    //LogToFile("resultvalue=" + resultvalue, true);
                                                    AddToTranslationCacheIfValid(THTranslationCache, InputValue, ResultValue);
                                                }
                                            }
                                            else
                                            {
                                                //resultvalue = inputvalue.Replace(extractedvalue, cachedvalue);
                                                ResultValue = PasteTranslationBackIfExtracted(CachedExtractedValue, InputValue, ExtractedValue);
                                            }


                                        }
                                    }                                                                                            //string onlinetranslation = DEEPL.Translate(origvalue);//из исходников ESPTranslator 

                                    //LogToFile("Result onlinetranslation=" + onlinetranslation, true);
                                    //проверка наличия результата и вторичная проверка пустого значения поля для перевода перед записью
                                    //if (!string.IsNullOrEmpty(onlinetranslation) && (THFileElementsDataGridView[cind + 1, rind].Value == null || string.IsNullOrEmpty(THFileElementsDataGridView[cind + 1, rind].Value.ToString())))
                                    if (ResultValue.Length == 0)
                                    {
                                    }
                                    else
                                    {
                                        if ((THFilesElementsDataset.Tables[t].Rows[rowindex][cind + 1] + string.Empty).Length == 0)
                                        {
                                            //LogToFile("THTranslationCache Rows count="+ THTranslationCache.Tables[0].Rows.Count);

                                            AddToTranslationCacheIfValid(THTranslationCache, InputValue, ResultValue);
                                            //THTranslationCacheAdd(inputvalue, onlinetranslation);                                    

                                            //запись перевода
                                            //THFileElementsDataGridView[cind + 1, rind].Value = onlinetranslation;
                                            THFilesElementsDataset.Tables[t].Rows[rowindex][cind + 1] = ResultValue;
                                            //THAutoSetValueForSameCells(t, rowindex, cind);
                                        }
                                    }
                                }
                                THAutoSetSameTranslationForSimular(t, rowindex, cind);
                            }
                        }
                    }
                    WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);
                }
            }
            catch
            {
                //LogToFile("Error: "+ex,true);
            }
            IsTranslating = false;
            ProgressInfo(false);
        }

        private void WriteTranslationCacheIfValid(DataSet THTranslationCache, string tHTranslationCachePath)
        {
            if (IsTranslationCacheEnabled && THTranslationCache.Tables[0].Rows.Count > 0)
            {
                DBFile.WriteDBFile(THTranslationCache, THTranslationCachePath);
                //THTranslationCache.Reset();
            }
        }

        private string TranslateMultilineValue(string[] InputLines, DataSet cacheDS)
        {
            //LogToFile("0 Started multiline array handling");
            //string ResultValue = string.Empty;
            StringBuilder ResultValue = new StringBuilder(InputLines.Length);
            string OriginalLine;
            for (int a = 0; a < InputLines.Length; a++)
            {
                OriginalLine = InputLines[a];//.Replace("\r", string.Empty);//replace было нужно когда делил строку по знаку \n и оставался \r
                //LogToFile("1 inputlinevalue="+ inputlinevalue);
                if (OriginalLine.Length == 0)
                {
                    ResultValue.Append(OriginalLine);
                    //LogToFile("1.1 inputlinevalue is empty. resultvalue="+ resultvalue);
                }
                else
                {
                    string ExtractedOriginal = THExtractTextForTranslation(OriginalLine);
                    //LogToFile("2 extractedvalue=" + extractedvalue);
                    string Result;
                    if (ExtractedOriginal.Length == 0 || Equals(ExtractedOriginal, OriginalLine))
                    {
                        Result = ReturnTranslatedOrCache(cacheDS, OriginalLine);
                        AddToTranslationCacheIfValid(cacheDS, OriginalLine, Result);
                    }
                    else
                    {
                        Result = PasteTranslationBackIfExtracted(
                            ReturnTranslatedOrCache(cacheDS, ExtractedOriginal),
                            OriginalLine,
                            ExtractedOriginal
                            );
                        AddToTranslationCacheIfValid(cacheDS, ExtractedOriginal, Result);
                    }
                    ResultValue.Append(Result);

                }
                //добавление новой строки если последняя строка не последняя в массиве строк
                if (a + 1 < InputLines.Length)
                {
                    ResultValue.Append(Environment.NewLine);
                }
                //LogToFile("5 resultvalue=" + resultvalue);
            }
            //LogToFile(string.Empty,true);
            return ResultValue.ToString();
        }

        private string ReturnTranslatedOrCache(DataSet cacheDS, string InputLine)
        {
            string valuefromcache = TranslationCacheFind(cacheDS, InputLine);

            if (valuefromcache.Length != 0)
            {
                return valuefromcache;
            }
            else
            {
                return GoogleAPI.Translate(InputLine);
            }
        }

        private void OpenInWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (THFileElementsDataGridView.Rows.Count > 0)
                {

                }
                else
                {
                    return;
                }
                //координаты стартовой строк, колонки оригинала и номера таблицы
                int cind = THFileElementsDataGridView.Columns["Original"].Index;//-поле untrans
                int tableindex = THFilesList.SelectedIndex;
                StringBuilder value = new StringBuilder();
                int selcellscnt = THFileElementsDataGridView.SelectedCells.Count;
                int[] selindexes = new int[selcellscnt];
                for (int i = 0; i < selcellscnt; i++)
                {
                    selindexes[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                }
                Array.Sort(selindexes);
                for (int i = 0; i < selcellscnt; i++)
                {
                    //MessageBox.Show(THFilesElementsDataset.Tables[tableindex].Rows[THFileElementsDataGridView.SelectedCells[i].RowIndex][cind].ToString());
                    //MessageBox.Show(THFileElementsDataGridView.CurrentCell.Value.ToString());
                    value.Append(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind] + string.Empty);
                    if (i + 1 < selcellscnt)
                    {
                        value.Append(Environment.NewLine);
                    }
                }
                //MessageBox.Show(value.ToString());
                //string result = Settings.THSettingsWebTransLinkTextBox.Text.Replace("{languagefrom}", "auto").Replace("{languageto}", "en").Replace("{text}", value.ToString().Replace("\r\n", "%0A").Replace("\"", "\\\string.Empty));
                string result = string.Format(WebTranslationLink.Replace("{languagefrom}", "{0}").Replace("{languageto}", "{1}").Replace("{text}", "{2}"), "auto", "en", HttpUtility.UrlEncode(value + string.Empty, Encoding.UTF8));
                //MessageBox.Show(result);
                Process.Start(result);

                //string input = (Regex.Replace(value.ToString(), @"\r\n|\r|\n", "DNTT")).Replace("\"", "\\\string.Empty);
                //LogToFile("input=" + input);
                //string s = GoogleAPI.Translate(input);
                ////string[] s = GoogleAPI.Translate(input).Split("\n");

                //LogToFile("Translation s=" + s);
                ////LogToFile("Translation formatted:\r\n"+s.Replace("  DNTT  ", "\r\n"));
                //LogToFile(string.Empty, true);
            }
            catch
            {
            }
        }

        private void THTargetTextBox_Leave(object sender, EventArgs e)
        {
            //int sel = dataGridView1.CurrentRow.Index; //присвоить перевенной номер выбранной строки в таблице
            if (THSourceRichTextBox.Text.Length == 0)
            {
            }
            else//если текстовое поле 2 не пустое
            {
                THFileElementsDataGridView.CurrentRow.Cells["Translation"].Value = THTargetRichTextBox.Text;// Присвоить ячейке в ds.Tables[0] значение из TextBox2                
            }
        }

        private void THFiltersDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            //var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString("F", this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        bool THIsFixingCells;
        /// <summary>
        /// Исправления формата спецсимволов в заданной ячейке перевода
        /// Для выбранных ячеек, таблицы или для всех значений задать:
        /// method:
        /// "a" - All
        /// "t" - Table
        /// "s" - Selected
        /// ..а также cind - индекс колонки, где ячейки перевода и tind - индекс таблицы, для вариантов Table и Selected
        /// Для одной выбранной ячейки, когда, например, определенная обрабатывается в коде, задать tind, cind и rind, а также true для onselectedonly
        /// </summary>
        /// <param name="method"></param>
        /// <param name="cind"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <param name="selectedonly"></param>
        private void THFixCells(string method, int cind, int tind, int rind = 0,  bool selectedonly=false)//cind - индекс столбца перевода, задан до старта потока
        {
            //возвращать, если занято, когда исправление в процессе
            if (THIsFixingCells)
            {
                return;
            }
            //установить занятость при старте
            THIsFixingCells = true;

            AutoOperations.THFixCells(THFilesElementsDataset, THFileElementsDataGridView, method, cind, tind, rind, selectedonly);

            //снять занятость по окончании
            THIsFixingCells = false;
        }

        bool THIsExtractingTextForTranslation;
        private string THExtractTextForTranslation(string input)
        {
            //возвращать, если занято, когда исправление в процессе
            if (THIsExtractingTextForTranslation)
            {
                return string.Empty;
            }
            //установить занятость при старте
            THIsExtractingTextForTranslation = true;

            string ret = AutoOperations.THExtractTextForTranslation(input);            

            //снять занятость по окончании
            THIsExtractingTextForTranslation = false;
            return ret;
        }

        private void SelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THFileElementsDataGridView.SelectedCells.Count > 0)
            {
                //эти два присвоены до начала нового потока, т.к. в другом потоке возникает исключение о попытке доступа к элементу управления, созданному в другом потоке
                //на самом деле здась даже не знаю, стоии ли оно того, чтобы кидать эту операцию на новый поток, она по идее и так должна за секунду выполниться
                //пока оставлю выполнение в другом потоке, как нибудь проверю без второго потока там, где много исправлять, по крайней мере оставлю для вариантов Таблица и все
                int cind = THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
                int tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("s", cind, tableindex)));
                //но при выборе только одной строчки почему-то кидает исключение
                //trans.Start();

                //убрал здесь выполнение во втором потоке, т.к. слишком мало править, не стоит того
                THFixCells("s", cind, tableindex);
            }
        }

        private void TableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //эти два присвоены до начала нового потока, т.к. в другом потоке возникает исключение о попытке доступа к элементу управления, созданному в другом потоке
            //на самом деле здась даже не знаю, стоии ли оно того, чтобы кидать эту операцию на новый поток, она по идее и так должна за секунду выполниться
            //пока оставлю выполнение в другом потоке, как нибудь проверю без второго потока там, где много исправлять, по крайней мере оставлю для вариантов Таблица и все
            int cind = THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
            int tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox
            
            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            Thread trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("t", cind, tableindex)));
            //но при выборе только одной строчки почему-то кидает исключение
            trans.Start();

            //THFixCells("t");
        }

        private void AllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //эти два присвоены до начала нового потока, т.к. в другом потоке возникает исключение о попытке доступа к элементу управления, созданному в другом потоке
            //на самом деле здась даже не знаю, стоии ли оно того, чтобы кидать эту операцию на новый поток, она по идее и так должна за секунду выполниться
            //пока оставлю выполнение в другом потоке, как нибудь проверю без второго потока там, где много исправлять, по крайней мере оставлю для вариантов Таблица и все
            int cind = THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
            int tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox

            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            Thread trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("a", cind, tableindex)));
            //но при выборе только одной строчки почему-то кидает исключение
            trans.Start();
            //THFixCells("a");
        }

        private void SetOriginalValueToTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.SelectedCells.Count;
            if (THFileElementsDataGridViewSelectedCellsCount > 0)
            {
                try
                {
                    for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                    {
                        //координаты ячейки
                        int rind = GetSelectedRowIndexInDatatable(THFileElementsDataGridView.SelectedCells[i].RowIndex);
                        int cind = THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Original"].Ordinal;//2-поле untrans

                        THFileElementsDataGridView[cind + 1, rind].Value = THFileElementsDataGridView[cind, rind].Value;
                    }
                }
                catch
                {
                }
            }
        }

        bool cellchanged = false;
        public void THAutoSetSameTranslationForSimular(int InputTableIndex, int InputRowIndex, int InputCellIndex, bool forcerun = true, bool forcevalue = false)
        {
            if (forcevalue || (AutotranslationForSimular && (cellchanged || forcerun))) //запуск только при изменении ячейки, чтобы не запускалось каждый раз. Переменная задается в событии изменения ячейки
            {
                AutoOperations.THAutoSetSameTranslationForSimular(THFilesElementsDataset, InputTableIndex, InputRowIndex, InputCellIndex, forcerun, forcevalue);
                
                //LogToFile(string.Empty,true);
                cellchanged = false;
            }
        }

        private void THFileElementsDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //использован код отсюда:https://stackoverflow.com/a/22912594
            //но модифицирован для ситуации когда выбрана только ячейка, а не строка полностью
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewRow clickedRow = (sender as DataGridView).Rows[e.RowIndex];
                    if (clickedRow.Cells[e.ColumnIndex].Selected || clickedRow.Selected)//вот это модифицировано
                    {
                    }
                    else
                    {
                        THFileElementsDataGridView.CurrentCell = clickedRow.Cells[e.ColumnIndex];
                    }

                    if (clickedRow.Cells[e.ColumnIndex].IsInEditMode)//не вызывать меню, когда ячейка в режиме редактирования
                    {
                    }
                    else
                    {
                        var mousePosition = THFileElementsDataGridView.PointToClient(Cursor.Position);

                        THFileElementsDataGridViewContextMenuStrip.Show(THFileElementsDataGridView, mousePosition);
                    }
                }
            }
        }

        //==============вырезать, копировать, вставить, для одной или нескольких ячеек

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CellBeginEditStarted)//если ячейка в режиме редактирования
            {
                //вылючение действий для ячеек при выходе из режима редактирования
                ControlsSwitch();
            }

            if (THFileElementsDataGridView == null)
            {
            }
            else
            {
                // Ensure that text is currently selected in the text box.    
                if (THFileElementsDataGridView.SelectedCells.Count > 0)
                {
                    //Copy to clipboard
                    CopyPaste.CopyToClipboard(THFileElementsDataGridView);

                    //Clear selected cells                
                    //проверка, выполнять очистку только если выбранные ячейки не помечены Только лдя чтения
                    if (THFileElementsDataGridView.CurrentCell.ReadOnly)
                    {
                    }
                    else
                    {
                        foreach (DataGridViewCell dgvCell in THFileElementsDataGridView.SelectedCells)
                        {
                            dgvCell.Value = string.Empty;
                        }
                    }

                }
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CellBeginEditStarted)//если ячейка в режиме редактирования
            {
                //вылючение действий для ячеек при выходе из режима редактирования
                ControlsSwitch();
            }

            if (THFileElementsDataGridView == null)
            {
            }
            else
            {
                // Ensure that text is selected in the text box.    
                if (THFileElementsDataGridView.SelectedCells.Count > 0)
                {
                    CopyPaste.CopyToClipboard(THFileElementsDataGridView);
                }
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CellBeginEditStarted)//если ячейка в режиме редактирования
            {
                //вылючение действий для ячеек при выходе из режима редактирования
                ControlsSwitch();
            }

            //LogToFile("Paste Enter");
            if (THFileElementsDataGridView == null)
            {
            }
            else
            {
                //LogToFile("DGV is not empty");
                // Determine if there is any text in the Clipboard to paste into the text box. 
                if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
                {
                    //LogToFile("GetDataPresent is true");
                    // Determine if any text is selected in the text box. 
                    if (THFileElementsDataGridView.SelectedCells.Count > 0)
                    {
                        //LogToFile("DGV sel cells > 0");
                        //Perform paste Operation
                        CopyPaste.PasteClipboardValue(THFileElementsDataGridView, this);
                    }
                }
            }
            //LogToFile("Paste End", true);
        }

        private void ClearSelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure that text is currently selected in the text box.    
            if (THFileElementsDataGridView.SelectedCells.Count > 0)
            {
                //Clear selected cells                
                //проверка, выполнять очистку только если выбранные ячейки не помечены Только лдя чтения
                //if (THFileElementsDataGridView.CurrentCell.ReadOnly)
                //{
                //}
                //else
                //{
                //    foreach (DataGridViewCell dgvCell in THFileElementsDataGridView.SelectedCells)
                //    {
                //        dgvCell.Value = string.Empty;
                //    }
                //}

                try
                {
                    int[] rindexes = new int[THFileElementsDataGridView.SelectedCells.Count];
                    int corigind = THFileElementsDataGridView.Columns["Original"].Index;//2-поле untrans
                    int ctransind = THFileElementsDataGridView.Columns["Translation"].Index;//2-поле untrans
                    for (int i = 0; i < rindexes.Length; i++)
                    {
                        int rind = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                        if ((THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value + string.Empty).Length == 0)
                        {
                        }
                        else
                        {
                            rindexes[i] = GetSelectedRowIndexInDatatable(rind);
                        }

                    }
                    foreach (int rind in rindexes)
                    {
                        THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Rows[rind][ctransind] = string.Empty;
                    }
                }
                catch
                {
                }
            }
        }
        
        //==============вырезать, копировать, вставить, для одной или нескольких ячеек

        private void SetColumnSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.Sort = string.Empty;
        }

        private void SaveTranslationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog THFSaveBDAs = new SaveFileDialog())
            {
                dbpath = Path.Combine(Application.StartupPath,"DB");
                string dbfilename = Path.GetFileNameWithoutExtension(THSelectedDir) + "_" + DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");
                
                THFSaveBDAs.Filter = "DB file|*.xml;*.cmx;*.cmz|XML-file|*.xml|Gzip compressed DB (*.cmx)|*.cmx|Deflate compressed DB (*.cmz)|*.cmz";
                string projecttypeDBfolder = string.Empty;
                if (THSelectedSourceType.Contains("RPG Maker MV"))
                {
                    projecttypeDBfolder += "RPGMakerMV";
                }
                else if (THSelectedSourceType.Contains("RPGMaker"))
                {
                    projecttypeDBfolder += "RPGMakerTransPatch";
                }
                THFSaveBDAs.InitialDirectory = Path.Combine(dbpath, projecttypeDBfolder);
                THFSaveBDAs.FileName = dbfilename + GetDBCompressionExt();

                if (THFSaveBDAs.ShowDialog() == DialogResult.OK)
                {
                    if (THFSaveBDAs.FileName.Length == 0)
                    {
                    }
                    else
                    {
                        //string spath = THFOpenBD.FileName;
                        //THFOpenBD.OpenFile().Close();
                        //MessageBox.Show(THFOpenBD.FileName);
                        //LoadTranslationFromDB();
                        
                        ProgressInfo(true);

                        //SaveNEWDB(THFilesElementsDataset, THFSaveBDAs.FileName);
                        //WriteDBFile(THFilesElementsDataset, THFSaveBDAs.FileName);
                        WriteDBFileLite(THFilesElementsDataset, THFSaveBDAs.FileName);
                        MessageBox.Show("finished");
                        ProgressInfo(false);
                    }
                }
            }
        }

        bool WriteDBFileIsBusy = false;
        string WriteDBFileLiteLastFileName = string.Empty;
        private async void WriteDBFileLite(DataSet ds, string fileName)
        {
            if (fileName.Length==0 || ds == null)
            {
                return;
            }
            while (WriteDBFileIsBusy && WriteDBFileLiteLastFileName != fileName)
            {
                await Task.Run(() => WaitThreaded(5000));
            }
            WriteDBFileIsBusy = true;
            WriteDBFileLiteLastFileName = fileName;
            using (DataSet liteds = Table.FillTempDB(ds))
            {
                await Task.Run(() => DBFile.WriteDBFile(liteds, fileName));
            }
            WriteDBFileIsBusy = false;
            WriteDBFileLiteLastFileName = string.Empty;
        }

        private void WaitThreaded(int time)
        {
            Thread.Sleep(time);
        }

        private void SaveNEWDB(DataSet DS4Save, string fileName)
        {
            //int TablesCount = DS4Save.Tables.Count;
            //for (int t = 0; t < TablesCount; t++)
            //{
            //    int RowsCount = DS4Save.Tables[t].Rows.Count;
            //    for (int r = 0; r < RowsCount; r++)
            //    {
            //        string
            //    }
            //}
        }

        private async void RunTestGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THSelectedSourceType == "RPG Maker MV")
            {
                CopyFolder.Copy(Path.Combine(THSelectedDir,"www","data"), Path.Combine(THSelectedDir,"www","data_bak"));
                try
                {
                    bool success = false;
                    for (int f = 0; f < THFilesList.Items.Count; f++)
                    {
                        //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                        //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                        bool changed = false;
                        for (int r = 0; r < THFilesElementsDataset.Tables[f].Rows.Count; r++)
                        {
                            if ((THFilesElementsDataset.Tables[f].Rows[r]["Translation"] + string.Empty).Length == 0)
                            {
                            }
                            else
                            {
                                changed = true;
                                break;
                            }
                        }
                        //THMsg.Show(THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                        if (changed)
                        {

                            //THMsg.Show("start writing");

                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            success = await Task.Run(() => WriteJson(THFilesList.Items[f] + string.Empty, Path.Combine(THSelectedDir,"www","data",THFilesList.Items[f]+".json")));
                            if (!success)
                            {
                                break;
                            }
                            //success = WriteJson(THFilesListBox.Items[f].ToString(), THWorkProjectDir + "\\www\\data\\" + THFilesListBox.Items[f].ToString() + ".json");
                        }
                    }
                    if (success)
                    {
                        using (Process Testgame = new Process())
                        {
                            try
                            {
                                DirectoryInfo di = new DirectoryInfo(THSelectedDir);
                                FileInfo[] fiArr = di.GetFiles("*.exe");
                                string largestexe = string.Empty;
                                long filesize = 0;
                                foreach (FileInfo file in fiArr)
                                {
                                    if (file.Length > filesize)
                                    {
                                        filesize = file.Length;
                                        largestexe = file.FullName;
                                    }
                                }
                                //MessageBox.Show("outdir=" + outdir);
                                //Testgame.StartInfo.FileName = Path.Combine(THSelectedDir,"game.exe");
                                Testgame.StartInfo.FileName = largestexe;
                                //RPGMakerTransPatch.StartInfo.Arguments = string.Empty;
                                //Testgame.StartInfo.UseShellExecute = true;
                                await Task.Run(() => Testgame.Start());
                                Testgame.WaitForExit();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch (Win32Exception)
                {
                }
                catch
                {
                }
                Directory.Delete(Path.Combine(THSelectedDir, "www", "data"), true);
                Directory.Move(Path.Combine(THSelectedDir, "www", "data_bak"), Path.Combine(THSelectedDir, "www", "data"));
            }
        }

        private void ToUPPERCASEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoOperations.StringCaseMorph(THFileElementsDataGridView, 2);
        }

        private void FirstCharacterToUppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoOperations.StringCaseMorph(THFileElementsDataGridView, 1);
        }

        private void TolowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoOperations.StringCaseMorph(THFileElementsDataGridView, 0);
        }

        bool InteruptTranslation = false;
        private void InteruptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InteruptTranslation = true;
        }

        bool MainIsClosing = false;
        private void THMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainIsClosing = true;
            InteruptTranslation = true;
        }

        private void SetAsDatasourceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THFileElementsDataGridView.DataSource = THFilesElementsALLDataTable;

            //смотрел тут но в данном случае пришел к тому что отображает все также только одну таблицу
            //https://social.msdn.microsoft.com/Forums/en-US/f63f612f-20be-4bad-a91c-474396941800/display-dataset-data-in-gridview-from-multiple-data-tables?forum=adodotnetdataset
            //if (THFilesElementsDataset.Relations.Contains("ALL"))
            //{

            //}
            //else
            //{
            //    DataRelation dr = new DataRelation("ALL",
            //         new DataColumn[] { THFilesElementsDataset.Tables[0].Columns["Original"], THFilesElementsDataset.Tables[0].Columns["Translation"] },
            //         new DataColumn[] { THFilesElementsDataset.Tables[1].Columns["Original"], THFilesElementsDataset.Tables[1].Columns["Translation"] },
            //         false
            //                                        );

            //    THFilesElementsDataset.Relations.Add(dr);
            //}

            //THFileElementsDataGridView.DataSource = THFilesElementsDataset.Relations["ALL"].ParentTable;
        }

        THSearch search;
        private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THFilesList.SelectedIndex==-1)
            {
            }
            else
            {
                try
                {
                    if (search == null || search.IsDisposed)
                    {
                        search = new THSearch(THFilesElementsDataset, THFilesList, THFileElementsDataGridView, THTargetRichTextBox);
                    }

                    if (search.Visible)
                    {
                        search.Activate();//помещает на передний план
                    }
                    else
                    {
                        search.Show();
                        //поместить на передний план
                        //search.TopMost = true;
                        //search.TopMost = false;
                    }
                }
                catch
                {
                }
            }
        }

        private void THMainResetTableButton_Click(object sender, EventArgs e)
        {
            if (THFiltersDataGridView.Columns.Count > 0)
            {
                for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)
                {
                    THFiltersDataGridView.Rows[0].Cells[c].Value = string.Empty;
                }
                if (THFilesList.SelectedItem == null)
                {
                }
                else
                {
                    THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.RowFilter = string.Empty;
                    THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.Sort = string.Empty;
                    THFileElementsDataGridView.Refresh();
                }
            }
        }

        private void TESTRegexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "## 0 #># Strike / Physics #<# 0 ## ## 1 #># Strike / Effect #<# 1 ## ## 2 #># Strike / Fire #<# 2 ## ## 3 #># Blow / Ice #<# 3 ## ## 4 #># Strike / Thunder #<# 4 ## ## 5 #># Slash / Physics #<# 5 ## ## 6 #># Slash / Effect #<# 6 ## ## 7 #># Slash / Fire #<# 7 ## ## 8 #># Slash / Ice #<# 8 ## ## 9 #># Slash / Thunder #<# 9 ## ## 10 #># Piercing / Physics #<# 10 ## ## 11 #># Piercing / Effect #<# 11 ## ## 12 #># Piercing / Fire #<# 12 ## ## 13 #># Piercing / Ice #<# 13 ## ## 14 #># Piercing / Thunder #<# 14 ## ## 15 #># Nail / Physical #<# 15 ## ## 16 #># Nail / Effect #<# 16 ## ## 17 #># Nail / Fire #<# 17 ## ## 18 #># Claw / Ice #<# 18 ## ## 19 #># Claw / Thunder #<# 19 ## ## 20 #># Blow / Special Move 1 #<# 20 ## ## 21 #># Blow / Special Move 2 #<# 21 ## ## 22 #># Slash / Skill 1 #<# 22 ## ## 23 #># Slash / Skill 2 ##### ## 24 #># Slash / Skill 3 #<# 24 ## ## 25 #># Piercing / Skills 1 #<# 25 ## ## 26 #># Piercing / Special Move 2 #<# 26 ## ## 27 #># Nail / Special Move #<# 27 ## ## 28 #># Arrow / Special Move #<# 28 ## ## 29 #># General purpose / Special Move 1 #<# 29 ## ## 30 #># General Purpose / Skills 2 #<# 30 ## ## 31 #># Breath #<# 31 ## ## 32 #># Pollen #<# 32 ## ## 33 #># Ultrasound #<# 33 ## ## 34 #># Fog #<# 34 ## ## 35 #># Song #<# 35 ## ## 36 #># 咆哮 #<# 36 ## ## 37 #># Foot payment #<# 37 ## ## 38 #># per body #<# 38 ## ## 39 #># Flash #<# 39 ## ## 40 #># Recovery / Single 1 #<# 40 ## ## 41 #># Recovery / Single 2 #<# 41 ## ## 42 #># Recovery / Whole 1 #<# 42 ## ## 43 #># Recovery / Whole 2 #<# 43 ## ## 44 #># Treatment / Single 1 #<# 44 ## ## 45 #># Treatment / Single 2 #<# 45 ## ## 46 #># Treatment / Whole 1 #<# 46 ## ## 47 #># Treatment / Whole 2 #<# 47 ## ## 48 #># Resuscitation 1 #<# 48 ## ## 49 #># Resuscitation 2 #<# 49 ## ## 50 #># Enhance 1 #<# 50 ## ## 51 #># Enhance 2 #<# 51 ## ## 52 #># Enhance 3 #<# 52 ## ## 53 #># Weak 1 #<# 53 ## ## 54 #># Weak 2 #<# 54 ## ## 55 #># Weak 3 #<# 55 ## ## 56 #># Spell #<# 56 ## ## 57 #># Absorption #<# 57 ## ## 58 #># Poison #<# 58 ## ## 59 #># Darkness #<# 59 ## ## 60 #># Silence #<# 60 ## ## 61 #># Sleep #<# 61 ## ## 62 #># Confused #<# 62 ## ## 63 #># Paralysis #<# 63 ## ## 64 #># Instant death #<# 64 ## ## 65 #># Flame / Single 1 #<# 65 ## ## 66 #># Flame / Single 2 #<# 66 ## ## 67 #># Flame / Whole 1 #<# 67 ## ## 68 #># Flame / Whole 2 #<# 68 ## ## 69 #># Flame / Whole 3 #<# 69 ## ## 70 #># Ice / Single 1 #<# 70 ## ## 71 #># Ice / Single 2 #<# 71 ## ## 72 #># Ice / Whole 1 #<# 72 ## ## 73 #># Ice / Whole 2 #<# 73 ## ## 74 #># Ice / Whole 3 #<# 74 ## ## 75 #># Thunder / Single 1 #<# 75 ## ## 76 #># Lightning / Single 2 #<# 76 ## ## 77 #># Thunder / Whole 1 #<# 77 ## ## 78 #># Thunder / Whole 2 #<# 78 ## ## 79 #># Thunder / Overall 3 #<# 79 ## ## 80 #># Water / Single 1 #<# 80 ## ## 81 #># Water / Single 2 #<# 81 ## ## 82 #># Water / Whole 1 #<# 82 ## ## 83 #># Water / Whole 2 #<# 83 ## ## 84 #># Water / Whole 3 #<# 84 ## ## 85 #># Sat / Single 1 #<# 85 ## ## 86 #># Sat / Single 2 #<# 86 ## ## 87 #># Sat / Whole 1 #<# 87 ## ## 88 #># Sat / Whole 2 #<# 88 ## ## 89 #># Sat / Whole 3 #<# 89 ## ## 90 #># Wind / Single 1 #<# 90 ## ## 91 #># Wind / Single 2 #<# 91 ## ## 92 #># Wind / Whole 1 #<# 92 ## ## 93 #># Wind / Whole 2 #<# 93 ## ## 94 #># Wind / Whole 3 #<# 94 ## ## 95 #># Hikari / Single 1 #<# 95 ## ## 96 #># Hikari / Single 2 #<# 96 ## ## 97 #># Light / Whole 1 #<# 97 ## ## 98 #># Light / Whole 2 #<# 98 ## ## 99 #># Light / Whole 3 #<# 99 ## ## 100 #># Darkness / Single 1 #<# 100 ## ## 101 #># Darkness / Single 2 #<# 101 ## ## 102 #># Darkness / Whole 1 #<# 102 ## ## 103 #># Darkness / Whole 2 #<# 103 ## ## 104 #># Darkness / Overall 3 #<# 104 ## ## 105 #># No Attributes / Single 1 #<# 105 ## ## 106 #># No Attributes / Single 2 #<# 106 ## ## 107 #># No attribute / Whole 1 #<# 107 ## ## 108 #># No attribute / Overall 2 #<# 108 ## ## 109 #># No attribute / Overall 3 #<# 109 ## ## 110 #># Shooting / one shot #<# 110 ## ## 111 #># Shooting / Random #<# 111 ## ## 112 #># Shooting / Whole #<# 112 ## ## 113 #># Shooting / Special Moves #<# 113 ## ## 114 #># Laser / single shot #<# 114 ## ## 115 #># Laser / Whole #<# 115 ## ## 116 #># Pillar of Light 1 #<# 116 ## ## 117 #># Light Column 2 #<# 117 ## ## 118 #># Light bullet #<# 118 ## ## 119 #># Radiation #<# 119 ## ";
            Regex myReg = new Regex(@"(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\<\# \1 \#\# )|(?<=\#\# (\d{1,5}) \#\>\# ).*?(?= \#\#\#\#\# )", RegexOptions.Compiled);

            MatchCollection matchCollection = myReg.Matches(s);

            string o = string.Empty;
            foreach(var match in matchCollection)
            {
                FileWriter.WriteData("c:\\THLogREGEXTest.log", match + Environment.NewLine);
                o += match + " AND ";
                //MessageBox.Show("match="+ match.ToString()+ ", matchCollection count="+ matchCollection.Count);
            }
            MessageBox.Show("FOUND=\r\n" + o + "\r\n, matchCollection count=" + matchCollection.Count);
        }

        private bool CellBeginEditStarted = false;
        private void THFileElementsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            CellBeginEditStarted = true;
            //отключение действий для ячеек при входе в режим редктирования
            ControlsSwitch();
        }

        private void THFileElementsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CellBeginEditStarted = false;
            //влючение действий для ячеек при выходе из режима редктирования
            ControlsSwitch(true);
        }

        private void THSourceRichTextBox_MouseEnter(object sender, EventArgs e)
        {
            if (CellBeginEditStarted)
            {
            }
            else
            {
                //отключение действий для ячеек при входе
                ControlsSwitch();
                //https://stackoverflow.com/questions/12780961/disable-copy-and-paste-in-datagridview
                THFileElementsDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            }

        }

        private void THSourceRichTextBox_MouseLeave(object sender, EventArgs e)
        {
            if (CellBeginEditStarted)
            {
            }
            else
            {
                //влючение действий для ячеек при выходе из режима редктирования
                ControlsSwitch(true);
                THFileElementsDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            }
        }

        private void THTargetRichTextBox_MouseEnter(object sender, EventArgs e)
        {
            //отключение действий для ячеек при входе в текстбокс
            ControlsSwitch();

        }

        private void THTargetRichTextBox_MouseLeave(object sender, EventArgs e)
        {
            //влючение действий для ячеек при выходе из текстбокса
            ControlsSwitch(true);
        }

        private bool ControlsSwitchIsOn = true;
        private bool ControlsSwitchActivated = false;
        private void ControlsSwitch(bool switchon=false)
        {
            if (ControlsSwitchActivated)
            {
                if (switchon && !ControlsSwitchIsOn)
                {
                    ControlsSwitchIsOn = switchon;
                    //System.Media.SystemSounds.Asterisk.Play();
                    cutToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.X;
                    copyCellValuesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
                    pasteCellValuesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
                }
                else if (ControlsSwitchIsOn)
                {
                    ControlsSwitchIsOn = switchon;
                    //System.Media.SystemSounds.Hand.Play();
                    cutToolStripMenuItem1.ShortcutKeys = Keys.None;
                    copyCellValuesToolStripMenuItem.ShortcutKeys = Keys.None;
                    pasteCellValuesToolStripMenuItem.ShortcutKeys = Keys.None;
                }
            }
        }

        private async void LoadTranslationFromCompatibleSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                IsOpeningInProcess = true;

                //об сообщении Освобождаемый объект никогда не освобождается и почему using здесь
                //https://stackoverflow.com/questions/2926869/do-you-need-to-dispose-of-objects-and-set-them-to-null
                using (OpenFileDialog THFOpen = new OpenFileDialog())
                {
                    THFOpen.InitialDirectory = Settings.THConfigINI.ReadINI("Paths", "LastPath");
                    THFOpen.Filter = "RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker game exe(*.exe)|*.exe";

                    if (THFOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (THFOpen.OpenFile() != null)
                        {
                            //THActionProgressBar.Visible = true;
                            ProgressInfo(true, T._("loading") + "..");

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            //Thread open = new Thread(new ParameterizedThreadStart((obj) => GetSourceType(THFOpen.FileName)));
                            //open.Start();

                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            await Task.Run(() => THSelectedSourceTypeTranslated = GetSourceTypeTranslated(THFOpen.FileName));

                            //THSelectedSourceTypeTranslated = GetSourceType(THFOpen.FileName);

                            //THActionProgressBar.Visible = false;
                            ProgressInfo(false, string.Empty);

                            if (THSelectedSourceTypeTranslated.Length == 0)
                            {
                                THMsg.Show(T._("Problem with loading"));
                            }
                        }
                    }
                }

                IsOpeningInProcess = false;
            }
        }

        public DirectoryInfo mvdatadirTranslated;
        public string THWorkProjectDirTranslated;

        //private bool istpptransfileTranslated;
        private DataSet THFilesElementsDatasetTranslated = new DataSet();
        public string THSelectedDirTranslated;
        public string THRPGMTransPatchverTranslated;
        public string THSelectedSourceTypeTranslated;
        private string extractedpatchpathTranslated = string.Empty;
        private string GetSourceTypeTranslated(string sPath)
        {
            //Reset temp dir for translation
            THFilesElementsDatasetTranslated.Reset();

            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            THSelectedDirTranslated = dir + string.Empty;
            //MessageBox.Show("sPath=" + sPath);
            if (sPath.ToUpper().Contains("\\RPGMKTRANSPATCH"))
            {
                return RPGMTransPatchPrepareTranslated(sPath);
                //return "RPGMakerTransPatch";
            }
            else if (sPath.ToLower().Contains(".trans"))
            {
                //istpptransfileTranslated = true;
                if (OpentppTransFile(sPath))
                {
                    for (int i = 0; i < THFilesElementsDatasetTranslated.Tables.Count; i++)
                    {
                        THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDatasetTranslated.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    }
                    return "T++ trans";
                }
            }
            else if (sPath.ToLower().Contains(".json"))
            {
                if (OpenRPGMakerMVjson(sPath))
                {
                    for (int i = 0; i < THFilesElementsDatasetTranslated.Tables.Count; i++)
                    {
                        THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDatasetTranslated.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    }
                    return "RPG Maker MV json";
                }
            }
            else if (sPath.ToLower().Contains("\\game.exe") || dir.GetFiles("*.exe").Length > 0)
            {
                if (File.Exists(Path.Combine(THSelectedDir,"www","data","system.json")))//RPGMakerMV
                {
                    try
                    {
                        //THSelectedDir += "\\www\\data";
                        //var MVJsonFIles = new List<string>();
                        mvdatadirTranslated = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(THSelectedDir,"www","data/")));
                        foreach (FileInfo file in mvdatadirTranslated.GetFiles("*.json"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            //MVJsonFIles.Add(file.FullName);

                            if (OpenRPGMakerMVjson(file.FullName))
                            {
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }

                        for (int i = 0; i < THFilesElementsDatasetTranslated.Tables.Count; i++)
                        {
                            //THFilesListBox.Items.Add(THFilesElementsDatasetTranslated.Tables[i].TableName);
                            THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDatasetTranslated.Tables[i].TableName)));
                        }

                        return "RPG Maker MV";
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                else if (dir.GetFiles("*.rgss2a").Length > 0 || dir.GetFiles("*.rvdata").Length > 0 || dir.GetFiles("*.rgssad").Length > 0 || dir.GetFiles("*.rxdata").Length > 0 || dir.GetFiles("*.lmt").Length > 0 || dir.GetFiles("*.lmu").Length > 0)
                {

                    extractedpatchpathTranslated = string.Empty;
                    bool result = TryToExtractToRPGMakerTransPatch(sPath, "Temp");
                    //MessageBox.Show("result=" + result);
                    //MessageBox.Show("extractedpatchpathTranslated=" + extractedpatchpathTranslated);
                    if (result)
                    {
                        //Cleaning of the type
                        //THRPGMTransPatchFiles.Clear();
                        //THFilesElementsDatasetTranslated.Clear();

                        //var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
                        //string patchver;
                        //MessageBox.Show("isv3=" + isv3+ ", patchdir="+ extractedpatchpathTranslated+ ", extractedpatchpathTranslated="+ extractedpatchpathTranslated);
                        if (Directory.Exists(extractedpatchpathTranslated + "\\patch")) //если есть подпапка patch, тогда это версия патча 3
                        {
                            THRPGMTransPatchverTranslated = "3";
                            extractedpatchpathTranslated += "\\patch";
                            //MessageBox.Show("extractedpatchpathTranslated=" + extractedpatchpathTranslated);
                            dir = new DirectoryInfo(Path.GetDirectoryName(extractedpatchpathTranslated + "\\")); //Два слеша здесь в конце исправляют проблему возврата информации о неверной папке
                                                                                                       //MessageBox.Show("patchdir1=" + patchdir);
                        }
                        else //иначе это версия 2
                        {
                            THRPGMTransPatchverTranslated = "2";
                        }
                        //MessageBox.Show("patchdir2=" + patchdir);

                        List<string> vRPGMTransPatchFiles = new List<string>();

                        foreach (FileInfo file in dir.GetFiles("*.txt"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            vRPGMTransPatchFiles.Add(file.FullName);
                        }

                        //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

                        //THFilesDataGridView.Nodes.Add("main");
                        //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
                        //RPGMTransPatch.OpenTransFiles(files, patchver);
                        //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
                        if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, THRPGMTransPatchver, THFilesElementsDatasetTranslated, null))
                        {
                            THSelectedDirTranslated = extractedpatchpathTranslated.Replace("\\patch", string.Empty);
                            //MessageBox.Show(THSelectedSourceType + " loaded!");
                            //ProgressInfo(false, string.Empty);                            

                            return LoadOriginalToTranslation(THFilesElementsDatasetTranslated);
                        }
                    }
                }

            }

            //MessageBox.Show("Uncompatible source or problem with opening.");
            return string.Empty;
        }

        private string LoadOriginalToTranslation(DataSet tHFilesElementsDatasetTranslated)
        {
            int THFilesElementsDatasetTablesCount = THFilesElementsDataset.Tables.Count;
            if (THFilesElementsDatasetTablesCount == THFilesElementsDatasetTranslated.Tables.Count)
            {
                for (int t=0;t < THFilesElementsDatasetTablesCount; t++)
                {
                    var table = THFilesElementsDataset.Tables[t];
                    int rowscount = table.Rows.Count;
                    if (rowscount == THFilesElementsDatasetTranslated.Tables[t].Rows.Count)
                    {
                        for (int r = 0; r < rowscount; r++)
                        {
                            var rowoftranslated = THFilesElementsDatasetTranslated.Tables[t].Rows[r];
                            if (rowoftranslated[0] == null || string.IsNullOrEmpty(rowoftranslated[0] as string))
                            {
                            }
                            {
                                var row = THFilesElementsDataset.Tables[t].Rows[r];
                                if (row[0] == rowoftranslated[0])
                                {
                                }
                                else
                                {
                                    if (row[1] == null || string.IsNullOrEmpty(row[1] as string))
                                    {
                                        THFilesElementsDataset.Tables[t].Rows[r][1] = rowoftranslated[0];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //LogToFile("Rows count is not same. Original table " + t + " rows count ="+THFilesElementsDataset.Tables[t].Rows.Count + ", translated table" + t + " rows count ="+ THFilesElementsDatasetTranslated.Tables[t].Rows.Count);
                        //THFilesElementsDatasetTranslated.Reset();
                        //return string.Empty;
                    }
                }
            }
            else
            {
                //LogToFile("Tables count is not same. Original tables count=" + THFilesElementsDataset.Tables.Count + ", translated tables count =" + THFilesElementsDatasetTranslated.Tables.Count);
                //THFilesElementsDatasetTranslated.Reset();
                //return string.Empty;
            }
            THFilesElementsDatasetTranslated.Reset();
            //LogToFile(string.Empty, true);
            return "OK";
        }

        //global brushes with ordinary/selected colors
        private readonly SolidBrush ListBoxItemForegroundBrushSelected = new SolidBrush(Color.White);
        private readonly SolidBrush ListBoxItemForegroundBrush = new SolidBrush(Color.Black);
        private readonly SolidBrush ListBoxItemBackgroundBrushSelected = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
        private readonly SolidBrush ListBoxItemBackgroundBrush1 = new SolidBrush(Color.White);
        private readonly SolidBrush ListBoxItemBackgroundBrush1Complete = new SolidBrush(Color.FromArgb(235, 255, 235));
        private readonly SolidBrush ListBoxItemBackgroundBrush2 = new SolidBrush(Color.FromArgb(235, 240, 235));
        private readonly SolidBrush ListBoxItemBackgroundBrush2Complete = new SolidBrush(Color.FromArgb(225, 255, 225));

        //custom method to draw the items, don't forget to set DrawMode of the ListBox to OwnerDrawFixed
        private void THFilesList_DrawItem(object sender, DrawItemEventArgs e)
        {
            //раскраска строк
            //https://stackoverflow.com/questions/2554609/c-sharp-changing-listbox-row-color
            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            e.DrawBackground();

            int index = e.Index;
            if (index >= 0 && index < THFilesList.Items.Count)
            {
                bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
                string text = THFilesList.Items[index] as string;
                Graphics g = e.Graphics;

                //background:
                SolidBrush backgroundBrush;
                if (selected)
                    backgroundBrush = ListBoxItemBackgroundBrushSelected;
                else if ((index % 2) == 0)
                {
                    if (Table.IsTableRowsCompleted(THFilesElementsDataset.Tables[e.Index]))
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush1Complete;
                    }
                    else
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush1;
                    }
                }
                else
                {
                    if (Table.IsTableRowsCompleted(THFilesElementsDataset.Tables[e.Index]))
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush2Complete;
                    }
                    else
                    {
                        backgroundBrush = ListBoxItemBackgroundBrush2;
                    }
                }

                g.FillRectangle(backgroundBrush, e.Bounds);

                //text:
                SolidBrush foregroundBrush = (selected) ? ListBoxItemForegroundBrushSelected : ListBoxItemForegroundBrush;
                g.DrawString(text, e.Font, foregroundBrush, THFilesList.GetItemRectangle(index).Location);
            }

            e.DrawFocusRectangle();
        }

        private void TestTimingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
            Stopwatch swatch = new System.Diagnostics.Stopwatch();
            swatch.Start();
            //if (IsTableRowsCompleted(THFilesElementsDataset.Tables[THFilesList.SelectedIndex], "Translation"))
            //{

            //}

            swatch.Stop();
            LogToFile("Time="+ swatch.Elapsed,true);
        }

        private void THFiltersDataGridView_MouseEnter(object sender, EventArgs e)
        {
            ControlsSwitch();
        }

        private void THFiltersDataGridView_MouseLeave(object sender, EventArgs e)
        {
            ControlsSwitch(true);
        }

        private void THFileElementsDataGridView_MouseEnter(object sender, EventArgs e)
        {
            //ControlsSwitch(true);
        }

        private void THFileElementsDataGridView_MouseLeave(object sender, EventArgs e)
        {
            //ControlsSwitch();
        }

        private void THSourceRichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            ControlsSwitch();
        }

        private void THFiltersDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ControlsSwitch();
        }

        private void CompleteRomajiotherLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
            {
                for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
                {
                    if ((THFilesElementsDataset.Tables[t].Rows[r][1] + string.Empty).Length == 0)
                    {
                        if (SelectedRomajiAndOtherLocalePercentFromStringIsNotValid((THFilesElementsDataset.Tables[t].Rows[r][0] + string.Empty)))
                        {
                            THFilesElementsDataset.Tables[t].Rows[r][1] = THFilesElementsDataset.Tables[t].Rows[r][0];
                        }
                    }
                }
            }
        }

        private void THFileElementsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ControlsSwitchActivated)
            {
            }
            else
            {
                ControlsSwitch(true);//не включалось копирование в ячейку, при копировании с гугла назад
            }
        }

        //int SelectedRowIndexWhenFilteredDGW = 0;
        private void THFileElementsDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //здесь добавить запоминание индекса выбранной  строки в отфильтрованном DGW
            //bool IsOneOfFiltersHasValue = false;
            //for (int s = 0; s < THFiltersDataGridView.Columns.Count; s++)
            //{
            //    if ((THFiltersDataGridView.Rows[0].Cells[s].Value + string.Empty).Length > 0)
            //    {
            //        IsOneOfFiltersHasValue = true;
            //        break;
            //    }
            //}

            //if (IsOneOfFiltersHasValue)
            //{
            //    //по нахождению верного индекса строки
            //    //https://stackoverflow.com/questions/50999121/displaying-original-rowindex-after-filter-in-datagridview
            //    //https://stackoverflow.com/questions/27125494/get-index-of-selected-row-in-filtered-datagrid
            //    var r = ((DataRowView)BindingContext[THFileElementsDataGridView.DataSource].Current).Row;
            //    SelectedRowIndexWhenFilteredDGW = r.Table.Rows.IndexOf(r); //находит верный но только для выбранной ячейки
            //}
        }

        private void showCheckboxvalueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(IsTranslationCacheEnabled.ToString());
        }

        private void saveInnewFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void forceSameTranslationForIdenticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = THFileElementsDataGridView.SelectedCells.Count;
            if (i == 1)
            {
                THAutoSetSameTranslationForSimular(THFilesList.SelectedIndex, GetSelectedRowIndexInDatatable(THFileElementsDataGridView.CurrentCell.RowIndex), 0, true, true);
            }
        }









        //Материалы
        //по оптимизации кода
        //https://cc.davelozinski.com/c-sharp/fastest-way-to-compare-strings
        //http://www.vcskicks.com/optimize_csharp_code.php
        //https://stackoverflow.com/questions/7872633/most-advisable-way-of-checking-empty-strings-in-c-sharp
        //https://social.msdn.microsoft.com/Forums/en-US/9977e45f-c8c5-4a8f-9e02-12f74c1c4579/what-is-the-difference-between-stringempty-and-quotquot-?forum=csharplanguage
        //Сортировка при виртуальном режиме DatagridView
        //http://qaru.site/questions/1486005/c-datagridview-virtual-mode-enable-sorting
        //c# - Поиск ячеек/строк по DataGridView
        //http://www.skillcoding.com/Default.aspx?id=151
        //Ошибка "Строку, связанную с положением CurrencyManager, нельзя сделать невидимой"
        //http://www.cyberforum.ru/csharp-beginners/thread757809.html
        //Виртуальный режим
        //https://stackoverflow.com/questions/31458197/how-to-sort-datagridview-data-when-virtual-mode-enable
    }
}