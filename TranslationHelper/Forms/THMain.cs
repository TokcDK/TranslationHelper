using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects.RPGMMV;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper
{
    public partial class THMain : Form
    {
        //string THLog;
        //public IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");
        internal THSettings Settings;
        //public const string THStrDGTranslationColumnName = "Translation";
        //public const string THStrDGOriginalColumnName = "Original";
        //private readonly THLang LangF;

        //public readonly static string apppath = Application.StartupPath;
        //о разнице между "" и string.Empty и использовании string.lenght==0 вместо ==string.Empty
        //https://stackoverflow.com/a/7872957
        internal string extractedpatchpath = string.Empty;

        internal string FVariant = string.Empty;
        //private BindingList<THRPGMTransPatchFile> THRPGMTransPatchFiles; //Все файлы
        //DataTable fileslistdt = new DataTable();
        //internal DataSet thDataWorkTHFilesElementsDataset;
        //internal DataSet thDataWorkTHFilesElementsDatasetInfo;
        //internal DataTable thDataWorkTHFilesElementsALLDataTable;
        internal THDataWork thDataWork;
        //DataTable THFilesElementsDatatable;
        //private BindingSource THBS = new BindingSource();

        //Language strings
        //public string THMainDGVOriginalColumnName;
        //public string THMainDGVTranslationColumnName;

        //про primary key взял отсюда: https://stackoverflow.com/questions/3567552/table-doesnt-have-a-primary-key
        //readonly DataColumn[] keyColumns = new DataColumn[1];

        //Translation cache
        //DataSet THTranslationCache;
        internal static string THTranslationCachePath
        {
            get => Properties.Settings.Default.THTranslationCachePath;
            set => Properties.Settings.Default.THTranslationCachePath = value;
        }

        public THMain()
        {
            InitializeComponent();
            //LangF = new THLang();

            //Init Work Data
            thDataWork = new THDataWork
            {
                //need for use main form elements like ProgressBar
                Main = this
            };

            SetSettings();

            SetUIStrings();

            //LangF.THReadLanguageFileToStrings();

            //thDataWork.THFilesElementsDataset = new DataSet();
            //thDataWork.THFilesElementsDatasetInfo = new DataSet();
            //thDataWork.THFilesElementsALLDataTable = new DataTable();
            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            THFilesList.DrawMode = DrawMode.OwnerDrawFixed;

            //DataSet THTranslationCache; THTranslationCache = new DataSet();
            THTranslationCachePath = Path.Combine(Application.StartupPath, "DB", "THTranslationCache.cmx");

            //THRPGMTransPatchFiles = new BindingList<THRPGMTransPatchFile>();
            //dt = new DataTable();

            //THFileElementsDataGridView set doublebuffered to true
            SetDoublebuffered(true);
            if (File.Exists(Path.Combine(Application.StartupPath, "TranslationHelper.log")))
            {
                File.Delete(Path.Combine(Application.StartupPath, "TranslationHelper.log"));
            }

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        //Settings
        //public bool IsTranslationCacheEnabled = true;
        //public string WebTranslationLink = string.Empty;
        //public bool DontLoadStringIfRomajiPercent = true;
        //public int DontLoadStringIfRomajiPercentNum = 90;
        //public bool AutotranslationForSimular = true;
        //public bool IsFullComprasionDBloadEnabled = true;
        private void SetSettings()
        {
            Settings = new THSettings();
            Properties.Settings.Default.IsTranslationCacheEnabled = Settings.EnableTranslationCacheINI;
            //IsTranslationCacheEnabled = Properties.Settings.Default.IsTranslationCacheEnabled;
            Properties.Settings.Default.WebTranslationLink = Settings.WebTransLinkINI;
            //WebTranslationLink = Properties.Settings.Default.WebTranslationLink;
            Properties.Settings.Default.DontLoadStringIfRomajiPercent = Settings.DontLoadStringIfRomajiPercentINI;
            //DontLoadStringIfRomajiPercent = Properties.Settings.Default.DontLoadStringIfRomajiPercent;
            Properties.Settings.Default.DontLoadStringIfRomajiPercentNum = Settings.DontLoadStringIfRomajiPercentNumINI;
            //DontLoadStringIfRomajiPercentNum = Properties.Settings.Default.DontLoadStringIfRomajiPercentNum;
            Properties.Settings.Default.AutotranslationForSimular = Settings.AutotranslationForIdenticalINI;
            //AutotranslationForSimular = Properties.Settings.Default.AutotranslationForSimular;
            Properties.Settings.Default.IsFullComprasionDBloadEnabled = Settings.FullComprasionDBloadINI;
            //IsFullComprasionDBloadEnabled = Properties.Settings.Default.IsFullComprasionDBloadEnabled;
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
            this.CleanSelectedCellsToolStripMenuItem1.Text = T._("Clear selected cells");
            this.toolStripMenuItem14.Text = T._("UPPERCASE");
            this.uppercaseToolStripMenuItem.Text = T._("Uppercase");
            this.lowercaseToolStripMenuItem.Text = T._("lowercase");
        }

        private void THMain_Load(object sender, EventArgs e)
        {
            SetTooltips();

            //Disable links detection in edition textboxes
            THSourceRichTextBox.DetectUrls = false;
            THTargetRichTextBox.DetectUrls = false;
        }

        ToolTip THToolTip;
        private void SetTooltips()
        {
            //http://qaru.site/questions/47162/c-how-do-i-add-a-tooltip-to-a-control
            //THMainResetTableButton
            THToolTip = new ToolTip
            {

                // Set up the delays for the ToolTip.
                AutoPopDelay = 32000,
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

        //readonly bool THdebug = true;
        StringBuilder THsbLog;// = new StringBuilder();
        public void LogToFile(string s, bool w = false)
        {
            if (THsbLog == null)
            {
                THsbLog = new StringBuilder();
            }
            if (Properties.Settings.Default.THdebug)
            {
                if (w)
                {
                    if (THsbLog.Length == 0)
                    {
                        FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", DateTime.Now + " >>" + s + Environment.NewLine, true);
                    }
                    else
                    {
                        FileWriter.WriteData(Path.Combine(Application.StartupPath, "TranslationHelper.log"), DateTime.Now + " >>" + THsbLog + Environment.NewLine, true);
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

        internal bool IsOpeningInProcess = false;
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FunctionsOpen(thDataWork).OpenProject();
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
        private void THFilesListBox_MouseClick(object sender, MouseEventArgs e)
        {
        }


        //int numberOfRows=500;
        private bool THFilesListBox_MouseClickBusy;
        internal void ActionsOnTHFIlesListElementSelected()
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

                    if (THFilesList.SelectedIndex > -1)
                    {
                        Properties.Settings.Default.THFilesListSelectedIndex = THFilesList.SelectedIndex;

                        BindToDataTableGridView(thDataWork.THFilesElementsDataset.Tables[Properties.Settings.Default.THFilesListSelectedIndex]);

                    }

                    ShowNonEmptyRowsCount();

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

                    HideAllColumnsExceptOriginalAndTranslation();

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

        private void HideAllColumnsExceptOriginalAndTranslation()
        {
            if (THFileElementsDataGridView.Columns.Count == 2)
            {
                return;
            }

            foreach (DataGridViewColumn Column in THFileElementsDataGridView.Columns)
            {
                if (Column.Name != "Original" && Column.Name != "Translation")
                {
                    Column.Visible = false;
                }
            }

            //if (RPGMFunctions.THSelectedSourceType.Contains("RPGMakerTransPatch") || RPGMFunctions.THSelectedSourceType.Contains("RPG Maker game with RPGMTransPatch")) //Additional tweaks for RPGMTransPatch table
            //{
            //    THFileElementsDataGridView.Columns["Context"].Visible = false;
            //    THFileElementsDataGridView.Columns["Status"].Visible = false;
            //    if (THFileElementsDataGridView.Columns["Advice"] != null)
            //    {
            //        THFileElementsDataGridView.Columns["Advice"].Visible = false;
            //    }
            //}
        }

        public void BindToDataTableGridView(DataTable DT)
        {
            if (THFilesList != null && THFilesList.SelectedIndex > -1)//вторая попытка исправить исключение при выборе элемента списка
            {
                THFileElementsDataGridView.DataSource = DT;

                //во время прокрутки DGV чернела полоса прокрутки и в результате было получено исключение
                //добавил это для возможного фикса
                //https://fooobar.com/questions/1404812/datagridview-scrollbar-throwing-argumentoutofrange-exception
                THFileElementsDataGridView.PerformLayout();
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
            CleanSelectedCellsToolStripMenuItem1.Enabled = true;
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
                        this.Invoke((Action)(() => THSourceRichTextBox.Text = (THFileElementsDataGridView.Rows[e.RowIndex].Cells["Original"].Value + string.Empty))); //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
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
                        var cellvalue = THFileElementsDataGridView.Rows[e.RowIndex].Cells["Translation"].Value;
                        if (cellvalue == null || (cellvalue as string).Length == 0)
                        {
                            this.Invoke((Action)(() => THTargetRichTextBox.Clear()));

                        }

                        //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                        this.Invoke((Action)(() => THTargetRichTextBox.Text = cellvalue as string));

                        FormatTextBox();

                        //THTargetRichTextBox.Select(Properties.Settings.Default.THOptionLineCharLimit+1, THTargetRichTextBox.Text.Length);
                        //THTargetRichTextBox.SelectionColor = Color.Red;

                        TranslationLongestLineLenghtLabel.Text = FunctionsString.GetLongestLineLength(cellvalue.ToString()).ToString(CultureInfo.InvariantCulture);
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
                        if (thDataWork.THFilesElementsDatasetInfo != null && thDataWork.THFilesElementsDatasetInfo.Tables.Count > THFilesList.SelectedIndex)
                        {
                            THInfoTextBox.Text += T._("rowinfo:") + Environment.NewLine + thDataWork.THFilesElementsDatasetInfo.Tables[THFilesList.SelectedIndex].Rows[e.RowIndex][0];
                        }

                        if (RPGMFunctions.THSelectedSourceType == "RPG Maker MV")
                        {
                            THInfoTextBox.Text += Environment.NewLine + Environment.NewLine + T._("Several strings also can be in Plugins.js in 'www\\js' folder and referred plugins in plugins folder.");
                        }
                        THInfoTextBox.Text += Environment.NewLine + Environment.NewLine;
                        THInfoTextBox.Text += FunctionsRomajiKana.THShowLangsOfString(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + string.Empty, "all"); //Show all detected languages count info
                    }
                }
                //--------Считывание значения ячейки в текстовое поле 1
            }
            catch
            {
            }
        }

        //https://stackoverflow.com/a/31150444
        private void FormatTextBox()
        {
            if (THTargetRichTextBox == null)
            {
                return;
            }
            int tl = 0;
            this.Invoke((Action)(() => tl = THTargetRichTextBox.Text.Length));
            if (tl == 0)
            {
                return;
            }


            // Loop over each line
            int THTargetRichTextBoxLinesCount = 0;
            this.Invoke((Action)(() => THTargetRichTextBoxLinesCount = THTargetRichTextBox.Lines.Length));
            for (int i = 0; i < THTargetRichTextBoxLinesCount; i++)
            {
                // Current line text
                string currentLine = THTargetRichTextBox.Lines[i];

                // Ignore the non-assembly lines
                if (currentLine.Length > Properties.Settings.Default.THOptionLineCharLimit)
                {
                    // Start position
                    int start = Properties.Settings.Default.THOptionLineCharLimit;

                    // Length
                    int length = currentLine.Length - start;

                    // Make the selection
                    THTargetRichTextBox.SelectionStart = start;
                    THTargetRichTextBox.SelectionLength = length;

                    // Change the colour
                    THTargetRichTextBox.SelectionColor = Color.DarkRed;
                }
                //else
                //{//пробовал сделать возвращение цвета текста на черный при редактировании текстбокса, через события TextChanged и Validated, но он не меняется, хотя и функция выполняется, только когда кликаю на ячейку таблицы
                //    //вернуть цвет на дефолт
                //    // Make the selection
                //    THTargetRichTextBox.SelectionStart = 0;
                //    THTargetRichTextBox.SelectionLength = currentLine.Length;
                //    // Change the colour
                //    THTargetRichTextBox.SelectionColor = Color.Black;
                //}

            }
        }

        private void ShowNonEmptyRowsCount()
        {
            int RowsCount = FunctionsTable.GetDatasetRowsCount(thDataWork.THFilesElementsDataset);
            if (RowsCount == 0)
            {
                TableCompleteInfoLabel.Visible = false;
            }
            else
            {
                TableCompleteInfoLabel.Visible = true;
                TableCompleteInfoLabel.Text = FunctionsTable.GetDatasetNonEmptyRowsCount(thDataWork.THFilesElementsDataset) + "/" + RowsCount;
            }
        }

        internal bool SaveInAction = false;
        internal bool FIleDataWasChanged = false;
        private async void WriteTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FunctionsSave(thDataWork).PrepareToWrite();
        }

        public void ProgressInfo(bool status, string statustext = "")
        {
            if (statustext == null /*|| THActionProgressBar == null || THInfolabel == null*/)
                return;

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
                THSaveFolderBrowser.SelectedPath = Properties.Settings.Default.THSelectedDir; //Установить начальный путь на тот, что был установлен при открытии.

                if (THSaveFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (RPGMFunctions.THSelectedSourceType == "RPGMakerTransPatch")
                    {
                        if (new RPGMTransOLD(thDataWork).SaveRPGMTransPatchFiles(THSaveFolderBrowser.SelectedPath, RPGMFunctions.RPGMTransPatchVersion))
                        {
                            Properties.Settings.Default.THSelectedDir = THSaveFolderBrowser.SelectedPath;
                            //MessageBox.Show("Сохранение завершено!");
                            THMsg.Show(T._("Save complete!"));
                        }
                    }
                }
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
#pragma warning disable CA2000 // Ликвидировать объекты перед потерей области
            THAboutForm AboutForm = new THAboutForm();
#pragma warning restore CA2000 // Ликвидировать объекты перед потерей области
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
                            OverallFilter += "[" + THFiltersDataGridView.Columns[c].Name + "] Like '%" + FunctionsTable.FixDataTableFilterStringValue(THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty) + "%'";
                        }
                        else
                        {
                            OverallFilter += " AND ";
                            OverallFilter += "[" + THFiltersDataGridView.Columns[c].Name + "] Like '%" + FunctionsTable.FixDataTableFilterStringValue(THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty) + "%'";
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
                thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.RowFilter = OverallFilter;
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
                    THSettings = new THSettings();
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

            string rowIdx = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, e.RowIndex) + 1 + string.Empty;//здесь получаю реальный индекс из Datatable
            //string rowIdx = (e.RowIndex + 1) + string.Empty;

            using (StringFormat centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {

                Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            }
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
            {
                cellchanged = true;
                FIleDataWasChanged = true;
            }
        }

        string dbpath;
        string lastautosavepath;
        private async void SaveTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lastautosavepath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName(thDataWork) + FunctionsDBFile.GetDBCompressionExt(thDataWork));

            ProgressInfo(true);

            switch (RPGMFunctions.THSelectedSourceType)
            {
                case "RPGMakerTransPatch":
                case "RPG Maker game with RPGMTransPatch":
                    await Task.Run(() => new RPGMTransOLD(thDataWork).SaveRPGMTransPatchFiles(Properties.Settings.Default.THSelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);
                    break;
            }

            WriteDBFileLite(thDataWork.THFilesElementsDataset, lastautosavepath);
            //THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data

            Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath);

            System.Media.SystemSounds.Beep.Play();
            ProgressInfo(false);
        }

        bool AutosaveActivated = false;
        private void Autosave()
        {
            if (AutosaveActivated || thDataWork.THFilesElementsDataset == null)
            {
            }
            else
            {
                AutosaveActivated = true;

                dbpath = Path.Combine(Application.StartupPath, "DB");
                string dbfilename = Path.GetFileNameWithoutExtension(Properties.Settings.Default.THSelectedDir) + "_autosave";
                string autosavepath = Path.Combine(dbpath, "Auto", dbfilename + ".cmx");

                Thread IndicateSave = new Thread(new ParameterizedThreadStart((obj) => IndicateSaveProcess(T._("Saving") + "...")));
                IndicateSave.Start();

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => SaveLoop(thDataWork.THFilesElementsDataset, autosavepath)));
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
                if (FunctionsTable.TheDataSetIsNotEmpty(Data))
                {
                }
                else//если dataset пустой, нет смысла его сохранять
                {
                    AutosaveActivated = false;
                    return;
                }

                int i = 0;
                while (i < 60)
                {
                    Thread.Sleep(1000);
                    if (Properties.Settings.Default.IsTranslationHelperWasClosed || this == null || IsDisposed || Data == null || Path.Length == 0)
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

        private void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                IsOpeningInProcess = true;

                lastautosavepath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName(thDataWork) + FunctionsDBFile.GetDBCompressionExt(thDataWork));
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
                await Task.Run(() => ReadDBAndLoadDBCompare(DBDataSet, sPath)).ConfigureAwait(true);
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
                sPath = Settings.THConfigINI.ReadINI("Paths", "LastAutoSavePath");
            }

            //стандартное считывание
            ProgressInfo(true, T._("Reading DB File")+"...");
            FunctionsDBFile.ReadDBFile(DBDataSet, sPath); //load new data
            //new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompare(DBDataSet);

            //считывание через словарь с предварительным чтением в dataset и конвертацией в словарь
            //Своего рода среднее решение, которое быстрее решения с сравнением из БД в DataSet
            //и не имеет проблем решения с чтением сразу в словарь, 
            //тут не нужно переписывать запись в xml, хотя запись таблицы в xml пишет все колонки и одинаковые значения, т.е. xml будет больше
            //чтение из xml в dataset может занимать по нескольку секунд для больших файлов
            new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompareFromDictionary(FunctionsDBFile.DataSetToDictionary(DBDataSet));

            //считывание через словарь Чтение xml в словарь на текущий момент имеет проблемы
            //с невозможностью чтения закодированых в hex символов(решил как костыль через try catch) и пока не может читать сжатые xml
            //нужно постепенно доработать код, исправить проблемы и перейти полностью на этот наибыстрейший вариант
            //т.к. с ним и xml бд будет меньше размером
            //new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompareFromDictionary(FunctionsDBFile.ReadXMLDBToDictionary(sPath));


            ProgressInfo(false);
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

                    THFOpenBD.InitialDirectory = FunctionsDBFile.GetProjectDBFolder();

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

        internal bool savemenusNOTenabled = true;
        private async void THFileElementsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
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
                    savemenusNOTenabled = false;
                }

                int tableind = THFilesList.SelectedIndex;
                int rind = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, e.RowIndex);
                int cind = THFileElementsDataGridView.Columns["Original"].Index;

                if (rind>-1 && rind < thDataWork.THFilesElementsDataset.Tables[tableind].Rows.Count && (thDataWork.THFilesElementsDataset.Tables[tableind].Rows[rind][1] + string.Empty).Length > 0)
                {
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THAutoSetSameTranslationForSimular(tableind, rind, cind, false)));
                    //trans.Start();

                    await Task.Run(() => THAutoSetSameTranslationForSimular(tableind, rind, cind, false)).ConfigureAwait(false);
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

        internal bool IsTranslating = false;
        private void OnlineTranslateSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedRowsCount = THFileElementsDataGridView.GetRowsWithSelectedCellsCount();
            if (selectedRowsCount > 0)
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
                int[] selindexes = new int[selectedRowsCount];

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
                    int index = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, THFileElementsDataGridView.SelectedCells[i].RowIndex);
                    selindexes[i] = index;

                    //selindexes[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                }

                //THMsg.Show("selindexes[0]=" + selindexes[0] + "\r\ncind=" + cind + "\r\ntableindex=" + tableindex + "\r\nselected=" + selindexes.Length + ", lastselectedrowvalue=" + THFilesElementsDataset.Tables[tableindex].Rows[selindexes[0]][cind]);

                //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                //почемуто так не переводит, переводчик кидает ошибку при заппуске в другом потоке
                //await Task.Run(() => OnlineTranslateSelected(cind, tableindex, selindexes));  

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => new FunctionsOnlineTranslation(thDataWork).THOnlineTranslate(cind, tableindex, selindexes, "s")));
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
                int cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Original"].Ordinal;//-поле untrans
                int tableindex = THFilesList.SelectedIndex;
                int[] selindexes = new int[1];

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => new FunctionsOnlineTranslation(thDataWork).THOnlineTranslate(cind, tableindex, selindexes, "t")));
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
                int cind = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;//-поле untrans
                int tableindex = 0;
                int[] selindexes = new int[1];

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THOnlineTranslate(cind, tableindex, selindexes, "a")));
                //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THOnlineTranslateByBigBlocks(cind, tableindex, selindexes, "a")));
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => new FunctionsOnlineTranslation(thDataWork).THOnlineTranslateByBigBlocks2(cind, tableindex, selindexes, "a")));
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

        private void OpenInWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int dgvSelectedRowsCount = THFileElementsDataGridView.GetRowsWithSelectedCellsCount();
                if (dgvSelectedRowsCount == 0)
                {
                    return;
                }

                //координаты стартовой строк, колонки оригинала и номера таблицы
                int cind = THFileElementsDataGridView.Columns["Original"].Index;//-поле untrans
                int tableindex = THFilesList.SelectedIndex;
                StringBuilder value = new StringBuilder();
                int[] selindexes = new int[dgvSelectedRowsCount];
                for (int i = 0; i < dgvSelectedRowsCount; i++)
                {
                    selindexes[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, tableindex, THFileElementsDataGridView.SelectedCells[i].RowIndex);
                }
                Array.Sort(selindexes);
                for (int i = 0; i < dgvSelectedRowsCount; i++)
                {
                    //MessageBox.Show(THFilesElementsDataset.Tables[tableindex].Rows[THFileElementsDataGridView.SelectedCells[i].RowIndex][cind].ToString());
                    //MessageBox.Show(THFileElementsDataGridView.CurrentCell.Value.ToString());
                    value.Append(thDataWork.THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind] + string.Empty);
                    if (i + 1 < dgvSelectedRowsCount)
                    {
                        value.Append(Environment.NewLine);
                    }
                }
                //MessageBox.Show(value.ToString());
                //string result = Settings.THSettingsWebTransLinkTextBox.Text.Replace("{languagefrom}", "auto").Replace("{languageto}", "en").Replace("{text}", value.ToString().Replace("\r\n", "%0A").Replace("\"", "\\\string.Empty));
                string result = string.Format(CultureInfo.InvariantCulture, Properties.Settings.Default.WebTranslationLink.Replace("{languagefrom}", "{0}").Replace("{languageto}", "{1}").Replace("{text}", "{2}"), "auto", "en", HttpUtility.UrlEncode(value + string.Empty, Encoding.UTF8));
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

            using (var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString("F", this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            }

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
        private void THFixCells(string method, int cind, int tind, int rind = 0)//cind - индекс столбца перевода, задан до старта потока
        {
            //возвращать, если занято, когда исправление в процессе
            if (THIsFixingCells)
            {
                return;
            }
            //установить занятость при старте
            THIsFixingCells = true;

            FunctionsAutoOperations.THFixCells(thDataWork, method, cind, tind, rind);

            //снять занятость по окончании
            THIsFixingCells = false;
        }

        bool THIsExtractingTextForTranslation;
        internal string THExtractTextForTranslation(string input)
        {
            //возвращать, если занято, когда исправление в процессе
            if (THIsExtractingTextForTranslation)
            {
                return string.Empty;
            }
            //установить занятость при старте
            THIsExtractingTextForTranslation = true;

            string ret = FunctionsAutoOperations.THExtractTextForTranslation(input);

            //снять занятость по окончании
            THIsExtractingTextForTranslation = false;
            return ret;
        }

        private void SelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THFileElementsDataGridView.GetRowsWithSelectedCellsCount() > 0)
            {
                //эти два присвоены до начала нового потока, т.к. в другом потоке возникает исключение о попытке доступа к элементу управления, созданному в другом потоке
                //на самом деле здась даже не знаю, стоии ли оно того, чтобы кидать эту операцию на новый поток, она по идее и так должна за секунду выполниться
                //пока оставлю выполнение в другом потоке, как нибудь проверю без второго потока там, где много исправлять, по крайней мере оставлю для вариантов Таблица и все
                int cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
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
            int cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
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
            int cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
            int tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox

            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            Thread trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("a", cind, tableindex)));
            //но при выборе только одной строчки почему-то кидает исключение
            trans.Start();
            //THFixCells("a");
        }

        private void SetOriginalValueToTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.GetRowsWithSelectedCellsCount();
            if (THFileElementsDataGridViewSelectedCellsCount > 0)
            {
                try
                {
                    int tableIndex = THFilesList.SelectedIndex;
                    int cind = thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns["Original"].Ordinal;// Колонка Original
                    int cindTrans = thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns["Translation"].Ordinal;// Колонка Original
                    int[] selectedRowIndexses = new int[THFileElementsDataGridViewSelectedCellsCount];
                    for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                    {
                        //координаты ячейки
                        selectedRowIndexses[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, THFileElementsDataGridView.SelectedCells[i].RowIndex);

                    }
                    foreach (var rind in selectedRowIndexses)
                    {
                        string origCellValue = thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows[rind][cind] as string;
                        string transCellValue = thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows[rind][cindTrans] + string.Empty;
                        if (transCellValue != origCellValue || transCellValue.Length == 0)
                        {
                            thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows[rind][cindTrans] = origCellValue;
                        }

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
            if (forcevalue || (Properties.Settings.Default.AutotranslationForSimular && (cellchanged || forcerun))) //запуск только при изменении ячейки, чтобы не запускалось каждый раз. Переменная задается в событии изменения ячейки
            {
                FunctionsAutoOperations.THAutoSetSameTranslationForSimular(thDataWork, InputTableIndex, InputRowIndex, InputCellIndex, forcevalue);

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
                    FunctionsCopyPaste.CopyToClipboard(THFileElementsDataGridView);

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
                    FunctionsCopyPaste.CopyToClipboard(THFileElementsDataGridView);
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
                        FunctionsCopyPaste.PasteClipboardValue(THFileElementsDataGridView, this);
                    }
                }
            }
            //LogToFile("Paste End", true);
        }

        bool ClearSelectedCellsIsBusy = false;
        private async void ClearSelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearSelectedCellsIsBusy)
                return;
            ClearSelectedCellsIsBusy = true;
            await Task.Run(() => FunctionsTable.CleanTableCells(thDataWork, Properties.Settings.Default.THFilesListSelectedIndex)).ConfigureAwait(true);
            ClearSelectedCellsIsBusy = false;
        }

        //==============вырезать, копировать, вставить, для одной или нескольких ячеек

        private void SetColumnSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.Sort = string.Empty;
        }

        private void SaveTranslationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog THFSaveBDAs = new SaveFileDialog())
            {
                THFSaveBDAs.Filter = "DB file|*.xml;*.cmx;*.cmz|XML-file|*.xml|Gzip compressed DB (*.cmx)|*.cmx|Deflate compressed DB (*.cmz)|*.cmz";

                THFSaveBDAs.InitialDirectory = FunctionsDBFile.GetProjectDBFolder();
                THFSaveBDAs.FileName = FunctionsDBFile.GetDBFileName(thDataWork, true) + FunctionsDBFile.GetDBCompressionExt(thDataWork);

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
                        WriteDBFileLite(thDataWork.THFilesElementsDataset, THFSaveBDAs.FileName);
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
            if (fileName.Length == 0 || ds == null)
            {
                return;
            }
            while (WriteDBFileIsBusy && WriteDBFileLiteLastFileName != fileName)
            {
                await Task.Run(() => FunctionsThreading.WaitThreaded(5000)).ConfigureAwait(true);
            }

            Thread IndicateSave = new Thread(new ParameterizedThreadStart((obj) => IndicateSaveProcess(T._("Saving") + "...")));
            IndicateSave.Start();

            WriteDBFileIsBusy = true;
            WriteDBFileLiteLastFileName = fileName;
            using (DataSet liteds = FunctionsTable.FillTempDB(ds))
            {
                await Task.Run(() => FunctionsDBFile.WriteDBFile(liteds, fileName)).ConfigureAwait(true);
            }
            WriteDBFileIsBusy = false;
            WriteDBFileLiteLastFileName = string.Empty;
        }

        private void IndicateSaveProcess(string InfoText = "")
        {
            bool THInfolabelEnabled = false;
            if (!Properties.Settings.Default.IsTranslationHelperWasClosed && !THInfolabel.Enabled)
            {
                THInfolabelEnabled = true;
                THInfolabel.Invoke((Action)(() => THInfolabel.Enabled = true));
            }

            if (!Properties.Settings.Default.IsTranslationHelperWasClosed)
            {
                THInfolabel.Invoke((Action)(() => THInfolabel.Text = InfoText));
            }

            FunctionsThreading.WaitThreaded(1000);

            if (THInfolabelEnabled && !Properties.Settings.Default.IsTranslationHelperWasClosed && THInfolabel.Enabled)
            {
                THInfolabel.Invoke((Action)(() => THInfolabel.Text = string.Empty));
                THInfolabel.Invoke((Action)(() => THInfolabel.Enabled = false));
            }
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
            if (RPGMFunctions.THSelectedSourceType == "RPG Maker MV")
            {
                CopyFolder.Copy(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data"), Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data_bak"));
                try
                {
                    bool success = false;
                    for (int f = 0; f < THFilesList.Items.Count; f++)
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
                            success = await Task.Run(() => new RPGMMVOLD(thDataWork).WriteJson(THFilesList.Items[f] + string.Empty, Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data", THFilesList.Items[f] + ".json"))).ConfigureAwait(true);
                            if (!success)
                            {
                                break;
                            }
                            //success = WriteJson(THFilesListBox.Items[f].ToString(), Properties.Settings.Default.THWorkProjectDir + "\\www\\data\\" + THFilesListBox.Items[f].ToString() + ".json");
                        }
                    }
                    if (success)
                    {
                        using (Process Testgame = new Process())
                        {
                            try
                            {
                                DirectoryInfo di = new DirectoryInfo(Properties.Settings.Default.THSelectedDir);
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
                                //Testgame.StartInfo.FileName = Path.Combine(Properties.Settings.Default.THSelectedDir,"game.exe");
                                Testgame.StartInfo.FileName = largestexe;
                                //RPGMakerTransPatch.StartInfo.Arguments = string.Empty;
                                //Testgame.StartInfo.UseShellExecute = true;

                                //http://www.cyberforum.ru/windows-forms/thread31052.html
                                // свернуть
                                WindowState = FormWindowState.Minimized;

                                await Task.Run(() => Testgame.Start()).ConfigureAwait(true);
                                Testgame.WaitForExit();

                                // Показать
                                WindowState = FormWindowState.Normal;
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
                Directory.Delete(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data"), true);
                Directory.Move(Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data_bak"), Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data"));
            }
        }

        private void ToUPPERCASEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionsString.StringCaseMorph(thDataWork, THFilesList.SelectedIndex, 2);
        }

        private void FirstCharacterToUppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionsString.StringCaseMorph(thDataWork, THFilesList.SelectedIndex, 1);
        }

        private void TolowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionsString.StringCaseMorph(thDataWork, THFilesList.SelectedIndex, 0);
        }

        internal bool InteruptTranslation = false;
        private void InteruptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InteruptTranslation = true;
        }


        private void THMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.IsTranslationHelperWasClosed = true;
            InteruptTranslation = true;
            //THToolTip.Dispose();
            //thDataWork.THFilesElementsDataset.Dispose();
            //thDataWork.THFilesElementsDatasetInfo.Dispose();
            //thDataWork.THFilesElementsALLDataTable.Dispose();
            //Settings.Dispose();

            //global brushes with ordinary/selected colors
            //ListBoxItemForegroundBrushSelected.Dispose();
            //ListBoxItemForegroundBrush.Dispose();
            //ListBoxItemBackgroundBrushSelected.Dispose();
            //ListBoxItemBackgroundBrush1.Dispose();
            //ListBoxItemBackgroundBrush1Complete.Dispose();
            //ListBoxItemBackgroundBrush2.Dispose();
            //ListBoxItemBackgroundBrush2Complete.Dispose();
        }

        private void SetAsDatasourceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsALLDataTable;

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
            if (THFilesList.SelectedIndex == -1)
            {
            }
            else
            {
                try
                {
                    if (search == null || search.IsDisposed)
                    {
                        search = new THSearch(thDataWork, THFilesList, THFileElementsDataGridView, THTargetRichTextBox);
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
                    thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.RowFilter = string.Empty;
                    thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].DefaultView.Sort = string.Empty;
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
            foreach (var match in matchCollection)
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
                //ControlsSwitch(true);
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
            //ControlsSwitch(true);
        }

        internal bool ControlsSwitchIsOn = true;
        internal bool ControlsSwitchActivated = false;
        internal void ControlsSwitch(bool switchon = false)
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

        private /*async*/ void LoadTranslationFromCompatibleSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    if (FunctionsTable.IsTableRowsCompleted(thDataWork.THFilesElementsDataset.Tables[e.Index]))
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
                    if (FunctionsTable.IsTableRowsCompleted(thDataWork.THFilesElementsDataset.Tables[e.Index]))
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
            LogToFile("Time=" + swatch.Elapsed, true);
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
            int THFilesElementsDatasetTablesCount = thDataWork.THFilesElementsDataset.Tables.Count;
            for (int t = 0; t < THFilesElementsDatasetTablesCount; t++)
            {
                var table = thDataWork.THFilesElementsDataset.Tables[t];
                int tableRowsCount = table.Rows.Count;
                for (int r = 0; r < tableRowsCount; r++)
                {
                    var row = table.Rows[r];
                    //if ((THFilesElementsDataset.Tables[t].Rows[r][1] + string.Empty).Length == 0)//убрал проверку пустой ячейки, чтобы насильно переприсваивать
                    //{
                    if ((row[1] == null || string.IsNullOrEmpty(row[1] as string) || !Equals(row[1], row[0])) && FunctionsRomajiKana.SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(row[0] as string))
                    {
                        thDataWork.THFilesElementsDataset.Tables[t].Rows[r][1] = row[0];
                    }
                    //}
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

        private void ShowCheckboxvalueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Settings.Default.IsTranslationCacheEnabled.ToString(CultureInfo.GetCultureInfo("en-US")));
        }

        private void SaveInnewFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ForceSameTranslationForIdenticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = THFileElementsDataGridView.GetRowsWithSelectedCellsCount();
            if (i == 1)
            {
                THAutoSetSameTranslationForSimular(THFilesList.SelectedIndex, FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, THFileElementsDataGridView.CurrentCell.RowIndex), 0, true, true);
            }
        }

        private void SplitLinesWhichLongestOfLimitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SplitSelectedLines();
        }

        private void SplitSelectedLines()
        {
            int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.GetRowsWithSelectedCellsCount();
            if (THFileElementsDataGridViewSelectedCellsCount > 0)
            {
                try
                {
                    int tableIndex = THFilesList.SelectedIndex;
                    int cind = thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns["Original"].Ordinal;// Колонка Original
                    int cindTrans = thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns["Translation"].Ordinal;// Колонка Original
                    int[] selectedRowIndexses = new int[THFileElementsDataGridViewSelectedCellsCount];
                    for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                    {
                        //координаты ячейки
                        selectedRowIndexses[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, THFileElementsDataGridView.SelectedCells[i].RowIndex);

                    }
                    foreach (var rind in selectedRowIndexses)
                    {
                        string origCellValue = thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows[rind][cind] as string;
                        string transCellValue = thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows[rind][cindTrans] + string.Empty;
                        if (!string.IsNullOrWhiteSpace(transCellValue) && transCellValue != origCellValue && FunctionsString.GetLongestLineLength(transCellValue) > Properties.Settings.Default.THOptionLineCharLimit && !FunctionsString.IsStringContainsSpecialSymbols(transCellValue))
                        {
                            thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Rows[rind][1] = FunctionsString.SplitMultiLineIfBeyondOfLimit(transCellValue, Properties.Settings.Default.THOptionLineCharLimit);
                        }

                    }
                }
                catch
                {
                }
            }
        }

        private void SplitLinesWhichLongerOfLimitALLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TablesCount = thDataWork.THFilesElementsDataset.Tables.Count;
            for (int t = 0; t < TablesCount; t++)
            {
                var Table = thDataWork.THFilesElementsDataset.Tables[t];
                int TableRowsCount = Table.Rows.Count;
                for (int r = 0; r < TableRowsCount; r++)
                {
                    var Row = Table.Rows[r];
                    string CellValue = Row[1] as string;
                    if (Row[1] == null || string.IsNullOrEmpty(CellValue) || Equals(Row[1], Row[0]) || FunctionsString.GetLongestLineLength(CellValue) <= Properties.Settings.Default.THOptionLineCharLimit || FunctionsString.IsStringContainsSpecialSymbols(CellValue))
                    {
                    }
                    else
                    {
                        thDataWork.THFilesElementsDataset.Tables[t].Rows[r][1] = FunctionsString.SplitMultiLineIfBeyondOfLimit(CellValue, Properties.Settings.Default.THOptionLineCharLimit);
                    }
                }
            }
            MessageBox.Show(T._("Finished"));
        }

        private void FixMessagesInTheTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Rows.Count; r++)
            {
                var row = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Rows[r];
                if (row[1] == null || string.IsNullOrWhiteSpace(row[1] as string))
                {
                }
                else
                {
                    var s = row[0] as string;
                    var s1 = row[1] as string;
                    if (s.StartsWith("は") && !s1.StartsWith(" "))
                    {
#pragma warning disable CA1308 // Нормализуйте строки до прописных букв
                        thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Rows[r][1] = " " + s1.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + s1.Substring(1);
#pragma warning restore CA1308 // Нормализуйте строки до прописных букв
                    }
                    else if (s.StartsWith("の") && !s1.StartsWith("'s ") && !s1.StartsWith(" "))
                    {
#pragma warning disable CA1308 // Нормализуйте строки до прописных букв
                        thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Rows[r][1] = "'s " + s1.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + s1.Substring(1);
#pragma warning restore CA1308 // Нормализуйте строки до прописных букв
                    }
                }
            }
        }

        private void LowercaseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TIndex = THFilesList.SelectedIndex;
            Thread StringCase = new Thread(new ParameterizedThreadStart((obj) => FunctionsString.StringCaseMorph(thDataWork, TIndex, 0, true)));
            StringCase.Start();
        }

        private void UppercaseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TIndex = THFilesList.SelectedIndex;
            Thread StringCase = new Thread(new ParameterizedThreadStart((obj) => FunctionsString.StringCaseMorph(thDataWork, TIndex, 1, true)));
            StringCase.Start();
        }

        private void UPPERCASEallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TIndex = THFilesList.SelectedIndex;
            Thread StringCase = new Thread(new ParameterizedThreadStart((obj) => FunctionsString.StringCaseMorph(thDataWork, TIndex, 2, true)));
            StringCase.Start();
        }

        private void THTargetRichTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void THTargetRichTextBox_Validated(object sender, EventArgs e)
        {
        }

        private void AllIfExistsFiledirWithNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread newthread = new Thread(new ParameterizedThreadStart((obj) =>
            SetOriginalToTranslationIfFileExistsInAnyFolder()
            ));
            newthread.Start();
        }

        private void SetOriginalToTranslationIfFileExistsInAnyFolder()
        {
            string[] ProjectFilesList = Directory.GetFiles(Properties.Settings.Default.THSelectedGameDir, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < ProjectFilesList.Length; i++)
            {
                ProjectFilesList[i] = Path.GetFileNameWithoutExtension(ProjectFilesList[i]);
            }
            ProjectFilesList = ProjectFilesList.Distinct().ToArray();

            int cind = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;// Колонка Original
            int cindTrans = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;// Колонка Original
            //string[] Files = Directory.GetFiles(Properties.Settings.Default.THWorkProjectDir, "*.*", SearchOption.AllDirectories);
            //string[] Dirs = Directory.GetDirectories(Properties.Settings.Default.THWorkProjectDir, "*", SearchOption.AllDirectories);
            int tablesCount = thDataWork.THFilesElementsDataset.Tables.Count;
            for (int t = 0; t < tablesCount; t++)
            {
                int rowsCount = thDataWork.THFilesElementsDataset.Tables[t].Rows.Count;
                for (int r = 0; r < rowsCount; r++)
                {
                    string origCellValue = thDataWork.THFilesElementsDataset.Tables[t].Rows[r][cind] as string;
                    string transCellValue = thDataWork.THFilesElementsDataset.Tables[t].Rows[r][cindTrans] + string.Empty;

                    if ((transCellValue.Length == 0 || origCellValue != transCellValue) && FunctionsFileFolder.GetAnyFileWithTheNameExist(ProjectFilesList, origCellValue))
                    {
                        thDataWork.THFilesElementsDataset.Tables[t].Rows[r][cindTrans] = origCellValue;
                    }
                }
            }
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void THFilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActionsOnTHFIlesListElementSelected();
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