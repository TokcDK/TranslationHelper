﻿using CSRegisterHotkey;
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
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects.RPGMMV;
using TranslationHelper.Projects.RPGMTrans;
using TranslationHelper.Translators;

namespace TranslationHelper
{
    public partial class THfrmMain : Form
    {
        internal THfrmSettings Settings;

        internal string extractedpatchpath = string.Empty;

        internal string FVariant = string.Empty;
        internal THDataWork thDataWork;

        internal static string THTranslationCachePath
        {
            get => Properties.Settings.Default.THTranslationCachePath;
            set => Properties.Settings.Default.THTranslationCachePath = value;
        }

        public THfrmMain()
        {
            InitializeComponent();
            //LangF = new THLang();

            Properties.Settings.Default.ApplicationStartupPath = Application.StartupPath;
            Properties.Settings.Default.ApplicationProductName = Application.ProductName;
            Properties.Settings.Default.NewLine = Environment.NewLine;

            BindShortCuts();

            //Init Work Data
            thDataWork = new THDataWork
            {
                //need for use main form elements like ProgressBar
                Main = this
            };

            SetSettings();

            SetUIStrings();

            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            THFilesList.DrawMode = DrawMode.OwnerDrawFixed;

            THTranslationCachePath = THSettingsData.THTranslationCacheFilePath();

            //THFileElementsDataGridView set doublebuffered to true
            SetDoublebuffered(true);
            if (File.Exists(THSettingsData.THLogPath()) && new FileInfo(THSettingsData.THLogPath()).Length > 1000000)
            {
                File.Delete(THSettingsData.THLogPath());
            }

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        HotKeyRegister THFileElementsDataGridViewOriginalToTranslationHotkey;
        Keys registerKey = Keys.None;
        KeyModifiers registerModifiers = KeyModifiers.None;
        private void BindShortCuts()
        {
            THFileElementsDataGridViewOriginalToTranslationHotkey = new HotKeyRegister(this.Handle, 100, KeyModifiers.None, Keys.F8);
            THFileElementsDataGridViewOriginalToTranslationHotkey.HotKeyPressed += new EventHandler(SetOriginalValueToTranslationToolStripMenuItem_Click);
        }

        /// <summary>
        /// Handle the KeyDown of tbHotKey. In this event handler, check the pressed keys.
        /// The keys that must be pressed in combination with the key Ctrl, Shift or Alt,
        /// like Ctrl+Alt+T. The method HotKeyRegister.GetModifiers could check whether 
        /// "T" is pressed.
        /// </summary>
        private void tbHotKey_KeyDown(object sender, KeyEventArgs e)
        {
            // The key event should not be sent to the underlying control.
            e.SuppressKeyPress = true;

            // Check whether the modifier keys are pressed.
            if (e.Modifiers != Keys.None)
            {
                Keys key = Keys.None;
                KeyModifiers modifiers = HotKeyRegister.GetModifiers(e.KeyData, out key);

                // If the pressed key is valid...
                if (key != Keys.None)
                {
                    this.registerKey = key;
                    this.registerModifiers = modifiers;

                    // Display the pressed key in the textbox.
                    //tbHotKey.Text = string.Format("{0}+{1}",
                    //    this.registerModifiers, this.registerKey);

                    // Enable the button.
                    //btnRegister.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Handle the Click event of btnRegister.
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Register the hotkey.
                THFileElementsDataGridViewOriginalToTranslationHotkey = new HotKeyRegister(this.Handle, 100,
                    this.registerModifiers, this.registerKey);

                // Register the HotKeyPressed event.
                THFileElementsDataGridViewOriginalToTranslationHotkey.HotKeyPressed += new EventHandler(HotKeyPressed);

                // Update the UI.
                //btnRegister.Enabled = false;
                //tbHotKey.Enabled = false;
                //btnUnregister.Enabled = true;
            }
            catch (ArgumentException argumentException)
            {
                MessageBox.Show(argumentException.Message);
            }
            catch (ApplicationException applicationException)
            {
                MessageBox.Show(applicationException.Message);
            }
        }

        /// <summary>
        /// Show a message box if the HotKeyPressed event is raised.
        /// </summary>
        void HotKeyPressed(object sender, EventArgs e)
        {
            SetOriginalToTranslation();

            //Here is the magic!!!!!!!!'

            //DO SOMETHING COOL!!! Or Just activate this winform

            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    this.WindowState = FormWindowState.Normal;
            //}
            //this.Activate();



        }

        //Settings
        private void SetSettings()
        {
            Settings = new THfrmSettings(thDataWork);
            Settings.GetSettings();
        }

        private void SetUIStrings()
        {
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
            this.runTestGameToolStripMenuItem.Text = T._("Test");
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

            //Hide some items 
            thDataWork.Main.tlpTextLenPosInfo.Visible = false;
            thDataWork.Main.frmMainPanel.Visible = false;
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
            THToolTip.SetToolTip(THbtnMainResetTable, T._("Resets filters and tab sorting"));
            THToolTip.SetToolTip(THFiltersDataGridView, T._("Filters for columns of main table"));
            THToolTip.SetToolTip(TableCompleteInfoLabel, T._("Shows overal number of completed lines.\nClick to show first untranslated."));
            ////////////////////////////
        }

        internal bool IsOpeningInProcess;
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

                    ShowNonEmptyRowsCount();//Show how many rows have translation

                    HideAllColumnsExceptOriginalAndTranslation();

                    SetFilterDGV(); //Init Filters datagridview

                    SetOnTHFileElementsDataGridViewWasLoaded(); //Additional actions when elements of file was loaded in datagridview

                    CheckFilterDGV(); //Apply filters if they is not empty

                    UpdateTextboxes();
                }
                catch (Exception)
                {
                }

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
        }

        public void BindToDataTableGridView(DataTable DT)
        {
            if (THFilesList != null && THFilesList.SelectedIndex > -1 && DT != null)//вторая попытка исправить исключение при выборе элемента списка
            {
                try
                {
                    if (DT.TableName == "[ALL]" && thDataWork.THFilesElementsDataset.Tables.Count > 1)
                    {
                        //отображение содержимого всех таблиц в одной
                        //https://stackoverflow.com/questions/11099619/how-to-bind-dataset-to-datagridview-in-windows-application
                        //int ThisInd = THFilesList.SelectedIndex;
                        //using (DataTable dtnew = thDataWork.THFilesElementsDataset.Tables[0].Copy())
                        //{
                        //    for (var i = 1; i < thDataWork.THFilesElementsDataset.Tables.Count; i++)
                        //    {
                        //        if (i!= ThisInd)
                        //        {
                        //            dtnew.Merge(thDataWork.THFilesElementsDataset.Tables[i]);
                        //        }
                        //    }

                        //    THFileElementsDataGridView.AutoGenerateColumns = true;

                        //    DT.Clear();
                        //    DT.Merge(dtnew);
                        //    THFileElementsDataGridView.DataSource = DT;
                        //}
                    }
                    else
                    {
                        THFileElementsDataGridView.DataSource = DT;

                        //во время прокрутки DGV чернела полоса прокрутки и в результате было получено исключение
                        //добавил это для возможного фикса
                        //https://fooobar.com/questions/1404812/datagridview-scrollbar-throwing-argumentoutofrange-exception
                        //upd. не исправляет проблему для этого dgv. возможно это dgv фильтров
                        THFileElementsDataGridView.PerformLayout();
                    }
                }
                catch
                {
                }
            }
        }

        private void SetOnTHFileElementsDataGridViewWasLoaded()
        {
            ControlsSwitchActivated = true;
            ControlsSwitchIsOn = (cutToolStripMenuItem1.ShortcutKeys != Keys.None);

            if (THFileElementsDataGridView != null && THFileElementsDataGridView.Columns.Count > 1)
            {
                THFileElementsDataGridView.Columns["Original"].HeaderText = T._("Original");//THMainDGVOriginalColumnName;
                THFileElementsDataGridView.Columns["Translation"].HeaderText = T._("Translation");//THMainDGVTranslationColumnName;
                THFileElementsDataGridView.Columns["Original"].ReadOnly = true;
                THFiltersDataGridView.Enabled = true;
                THSourceRichTextBox.Enabled = true;
                THTargetRichTextBox.Enabled = true;
            }


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
        }

        private void SetFilterDGV()
        {
            if (THFiltersDataGridView.Columns.Count != THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible))
            {
                THFiltersDataGridView.Columns.Clear();
                THFiltersDataGridView.Rows.Clear();
                //int visibleindex = -1;
                for (int cindx = 0; cindx < THFileElementsDataGridView.Columns.Count; cindx++)
                {
                    if (THFileElementsDataGridView.Columns[cindx].Visible)
                    {
                        THFiltersDataGridView.Columns.Add(THFileElementsDataGridView.Columns[cindx].Name, THFileElementsDataGridView.Columns[cindx].HeaderText);
                    }
                }
                THFiltersDataGridView.Rows.Add(1);
                THFiltersDataGridView.CurrentRow.Selected = false;

                //во время прокрутки DGV чернела полоса прокрутки и в результате было получено исключение
                //добавил это для возможного фикса. возможно это этот dgv
                //https://fooobar.com/questions/1404812/datagridview-scrollbar-throwing-argumentoutofrange-exception
                THFiltersDataGridView.PerformLayout();
            }
        }

        private void THFileElementsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            this.Invoke((Action)(() => CellEnterActions(sender, e)));
        }

        private void CellEnterActions(object sender, DataGridViewCellEventArgs e)
        {
            //UpdateTextboxes(sender, e);
        }

        internal void UpdateTextboxes()
        {
            try
            {
                if (THFileElementsDataGridView.CurrentCell == null)
                {
                    return;
                }

                var tableIndex = Properties.Settings.Default.THFilesListSelectedIndex = THFilesList.SelectedIndex;
                var columnIndex = Properties.Settings.Default.DGVSelectedColumnIndex = THFileElementsDataGridView.CurrentCell.ColumnIndex;
                var rowIndex = Properties.Settings.Default.DGVSelectedRowIndex = THFileElementsDataGridView.CurrentCell.RowIndex;
                var realrowIndex = Properties.Settings.Default.DGVSelectedRowRealIndex = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, tableIndex, rowIndex);

                if (THFileElementsDataGridView.DataSource == null || rowIndex == -1 || tableIndex == -1)
                {
                    THSourceRichTextBox.Clear();
                    THTargetRichTextBox.Clear();
                    return;
                }

                //Считывание значения ячейки в текстовое поле 1, вариант 2, для DataSet, ds.Tables[0]
                //Проверка на размер индексов, для избежания ошибки при попытке сортировки " должен быть положительным числом и его размер не должен превышать размер коллекции"
                if (THSourceRichTextBox.Enabled
                    && THFileElementsDataGridView.CurrentCell != null
                    && THFileElementsDataGridView.Rows.Count > 0
                    && rowIndex > -1
                    && columnIndex > -1)
                {
                    THTargetRichTextBox.Clear();

                    if (string.IsNullOrEmpty(THFileElementsDataGridView.Rows[THFileElementsDataGridView.CurrentCell.RowIndex].Cells["Original"].Value + string.Empty))
                    {
                        THSourceRichTextBox.Clear();
                    }
                    else//проверить, не пуста ли ячейка, иначе была бы ошибка //THStrDGTranslationColumnName ошибка при попытке сортировки по столбцу
                    {
                        //wrap words fix: https://stackoverflow.com/questions/1751371/how-to-use-n-in-a-textbox
                        THSourceRichTextBox.Text = (THFileElementsDataGridView.Rows[rowIndex].Cells["Original"].Value + string.Empty);
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
                    string TranslationCellValue;
                    if (string.IsNullOrEmpty(TranslationCellValue = THFileElementsDataGridView.Rows[rowIndex].Cells["Translation"].Value + string.Empty))
                    {
                        THTargetRichTextBox.Clear();
                    }
                    else//проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                    {
                        //запоминание последнего значения ячейки перед считыванием в THTargetRichTextBox,
                        //для предотвращения записи значения обратно в ячейку, если она была изменена до изменения текстбокса
                        thDataWork.TargetTextBoxPreValue = TranslationCellValue;

                        //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                        THTargetRichTextBox.Text = thDataWork.TargetTextBoxPreValue;

                        FormatTextBox();

                        //THTargetRichTextBox.Select(Properties.Settings.Default.THOptionLineCharLimit+1, THTargetRichTextBox.Text.Length);
                        //THTargetRichTextBox.SelectionColor = Color.Red;

                        TranslationLongestLineLenghtLabel.Text = FunctionsString.GetLongestLineLength(TranslationCellValue.ToString(CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                        TargetTextBoxLinePositionLabelData.Text = string.Empty;
                    }

                    THInfoTextBox.Text = string.Empty;

                    string SelectedCellValue;
                    if ((SelectedCellValue = THFileElementsDataGridView.Rows[rowIndex].Cells[columnIndex].Value + string.Empty).Length == 0)
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
                        if (thDataWork.THFilesElementsDatasetInfo != null && thDataWork.THFilesElementsDatasetInfo.Tables.Count > tableIndex && thDataWork.THFilesElementsDatasetInfo.Tables[tableIndex].Rows.Count > rowIndex)
                        {
                            THInfoTextBox.Text += T._("rowinfo:") + Environment.NewLine + thDataWork.THFilesElementsDatasetInfo.Tables[tableIndex].Rows[rowIndex][0];
                        }

                        if (RPGMFunctions.THSelectedSourceType == "RPG Maker MV")
                        {
                            THInfoTextBox.Text += Environment.NewLine + Environment.NewLine + T._("Several strings also can be in Plugins.js in 'www\\js' folder and referred plugins in plugins folder.");
                        }
                        THInfoTextBox.Text += Environment.NewLine + Environment.NewLine;
                        THInfoTextBox.Text += FunctionsRomajiKana.GetLangsOfString(SelectedCellValue, "all"); //Show all detected languages count info
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
                string currentLine = string.Empty;
                this.Invoke((Action)(() => currentLine = THTargetRichTextBox.Lines[i]));

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

        internal bool SaveInAction;
        internal bool FileDataWasChanged;
        private async void WriteTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (thDataWork.TablesLinesDict != null && thDataWork.TablesLinesDict.Count > 0)
            {
                thDataWork.TablesLinesDict.Clear();
            }
            await Task.Run(() => new FunctionsSave(thDataWork).PrepareToWrite()).ConfigureAwait(true);
            Process.Start("explorer.exe", Properties.Settings.Default.THSelectedDir);
        }

        public void ProgressInfo(bool status, string statustext = "")
        {
            statustext = statustext?.Length == 0 ? T._("working..") : statustext;
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
                }

                THSaveFolderBrowser.SelectedPath = Properties.Settings.Default.THSelectedDir; //Установить начальный путь на тот, что был установлен при открытии.

                if (THSaveFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (RPGMFunctions.THSelectedSourceType == "RPGMakerTransPatch")
                    {
                        if (new RPGMTransOLD(thDataWork).SaveRPGMTransPatchFiles(THSaveFolderBrowser.SelectedPath, RPGMFunctions.RPGMTransPatchVersion))
                        {
                            Properties.Settings.Default.THSelectedDir = THSaveFolderBrowser.SelectedPath;
                            MessageBox.Show(T._("Save complete!"));
                        }
                    }
                }
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THfrmAbout AboutForm = new THfrmAbout();
            AboutForm.Show();
        }

        private void THTargetTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+Del function
            //https://stackoverflow.com/questions/18543198/why-cant-i-press-ctrla-or-ctrlbackspace-in-my-textbox
            if (e.Control)
            {
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

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Settings == null || Settings.IsDisposed)
                {
                    Settings = new THfrmSettings(thDataWork);
                }

                if (Settings.Visible)
                {
                    Settings.Activate();
                }
                else
                {
                    Settings.Show();
                }
            }
            catch
            {
            }
        }

        //http://qaru.site/questions/180337/show-row-number-in-row-header-of-a-datagridview
        private void THFileElementsDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PaintDigitInFrontOfRow(sender, e);
        }

        private void PaintDigitInFrontOfRow(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (!Properties.Settings.Default.ProjectIsOpened || THFilesList.SelectedIndex == -1)
                return;

            var grid = sender as DataGridView;

            int rowIdx = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, e.RowIndex);//здесь получаю реальный индекс из Datatable
            //string rowIdx = (e.RowIndex + 1) + string.Empty;

            using (StringFormat centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {

                Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString((rowIdx + 1) + string.Empty, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            }
        }

        //Пример виртуального режима
        //http://www.cyberforum.ru/post9306711.html

        private void THFileElementsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateTranslationTextBoxValue(sender, e);
            CellChangedRegistration(e.ColumnIndex);
        }

        private void CellChangedRegistration(int ColumnIndex = -1)
        {
            if (ColumnIndex > 0)
            {
                cellchanged = true;
                FileDataWasChanged = true;
            }
        }

        private void UpdateTranslationTextBoxValue(object sender, DataGridViewCellEventArgs e)
        {
            if (Properties.Settings.Default.DGVCellInEditMode && sender is DataGridView DGV)
            {
                THTargetRichTextBox.Text = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + string.Empty;
            }
        }

        string dbpath;
        string lastautosavepath;
        private async void SaveTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (thDataWork.Main.SaveInAction)
            {
                return;
            }

            lastautosavepath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(thDataWork), FunctionsDBFile.GetDBFileName(thDataWork) + FunctionsDBFile.GetDBCompressionExt(thDataWork));

            ProgressInfo(true);

            switch (RPGMFunctions.THSelectedSourceType)
            {
                case "RPGMakerTransPatch":
                case "RPG Maker game with RPGMTransPatch":
                    await Task.Run(() => new RPGMTransOLD(thDataWork).SaveRPGMTransPatchFiles(Properties.Settings.Default.THSelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);
                    break;
            }

            await Task.Run(() => WriteDBFileLite(thDataWork.THFilesElementsDataset, lastautosavepath)).ConfigureAwait(true);

            FunctionsSounds.SaveDBComplete();
            ProgressInfo(false);

            //THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data
        }

        bool AutosaveActivated;
        private void Autosave()
        {
            if (!Properties.Settings.Default.EnableDBAutosave || AutosaveActivated || thDataWork.THFilesElementsDataset == null)
            {
            }
            else
            {
                AutosaveActivated = true;

                dbpath = Path.Combine(Application.StartupPath, "DB");
                string dbfilename = Path.GetFileNameWithoutExtension(Properties.Settings.Default.THSelectedDir) + "_autosave";
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
                while (i < Properties.Settings.Default.DBAutoSaveTimeout && Properties.Settings.Default.EnableDBAutosave)
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
                    Thread.Sleep(Properties.Settings.Default.DBAutoSaveTimeout * 1000);
                }
                WriteDBFileLite(Data, Path);
            }
        }

        private async void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                IsOpeningInProcess = true;

                lastautosavepath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(thDataWork), FunctionsDBFile.GetDBFileName(thDataWork) + FunctionsDBFile.GetDBCompressionExt(thDataWork));
                if (File.Exists(lastautosavepath))
                {
                    await Task.Run(() => LoadTranslationFromDB(lastautosavepath)).ConfigureAwait(true);
                }
                else
                {
                    var result = MessageBox.Show(T._("DB not found. Try to load from all exist?"), T._("DB not found"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        await Task.Run(() => thDataWork.Main.LoadTranslationFromDB(lastautosavepath, true)).ConfigureAwait(true);
                    }
                }

                IsOpeningInProcess = false;
            }
        }

        bool LoadTranslationToolStripMenuItem_ClickIsBusy;
        internal async void LoadTranslationFromDB(string sPath = "", bool UseAllDB = false)
        {
            if (LoadTranslationToolStripMenuItem_ClickIsBusy || (!UseAllDB && sPath.Length == 0))
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


            if (UseAllDB)
            {
                ProgressInfo(true, "Get all databases");
                FunctionsDBFile.MergeAllDBtoOne(thDataWork);
                new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompareFromDictionary(thDataWork.AllDBmerged);
            }
            else
            {
                using (DataSet DBDataSet = new DataSet())
                {

                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => ReadDBAndLoadDBCompare(DBDataSet, sPath)).ConfigureAwait(true);
                }
            }


            THFileElementsDataGridView.Invoke((Action)(() => THFileElementsDataGridView.Refresh()));


            LoadTranslationToolStripMenuItem_ClickIsBusy = false;
            FunctionsSounds.LoadDBCompleted();
            THFilesList.Invoke((Action)(() => THFilesList.Refresh()));
        }

        private void ReadDBAndLoadDBCompare(DataSet DBDataSet, string sPath)
        {
            if (sPath.Length == 0)
            {
                sPath = Settings.THConfigINI.ReadINI("Paths", "LastAutoSavePath");
            }

            if (!File.Exists(sPath))
            {
                ProgressInfo(false);
                return;
            }

            ProgressInfo(true, T._("Reading DB File") + "...");

            try
            {
                //load new data
                FunctionsDBFile.ReadDBFile(DBDataSet, sPath);


                //отключение DataSource для избежания проблем от изменений DataGridView
                //bool tableSourceWasCleaned = false;
                //if (thDataWork.Main.THFileElementsDataGridView.DataSource != null)
                //{
                //    tableSourceWasCleaned = true;
                //    thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.DataSource = null));
                //    thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Update()));
                //    thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Refresh()));
                //}

                //стандартное считывание. Самое медленное
                //new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompare(DBDataSet);

                //считывание через словарь с предварительным чтением в dataset и конвертацией в словарь
                //Своего рода среднее решение, которое быстрее решения с сравнением из БД в DataSet
                //и не имеет проблем решения с чтением сразу в словарь, 
                //тут не нужно переписывать запись в xml, хотя запись таблицы в xml пишет все колонки и одинаковые значения, т.е. xml будет больше
                //чтение из xml в dataset может занимать по нескольку секунд для больших файлов
                //основную часть времени отнимал вывод информации о файлах!!
                //00.051
                new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompareFromDictionary(DBDataSet.DBDataSetToDBDictionary());

                //многопоточный вариант предыдущего, но т.к. datatable is threadunsafe то возникают разные ошибки и повреждение внутреннего индекса таблицы, хоть это и быстрее, но после добавления lock разницы не видно
                //new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompareFromDictionaryParallel(DBDataSet.DBDataSetToDBDictionary());


                //это медленнее первого варианта 
                //00.151
                //new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompareFromDictionary2(DBDataSet.DBDataSetToDBDictionary());

                //считывание через словарь Чтение xml в словарь на текущий момент имеет проблемы
                //с невозможностью чтения закодированых в hex символов(решил как костыль через try catch) и пока не может читать сжатые xml
                //нужно постепенно доработать код, исправить проблемы и перейти полностью на этот наибыстрейший вариант
                //т.к. с ним и xml бд будет меньше размером
                //new FunctionsLoadTranslationDB(thDataWork).THLoadDBCompareFromDictionary(FunctionsDBFile.ReadXMLDBToDictionary(sPath));

            }
            catch
            {

            }

            ProgressInfo(false);
        }

        private async void LoadTrasnlationAsToolStripMenuItem_Click(object sender, EventArgs e)
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

                    THFOpenBD.InitialDirectory = FunctionsDBFile.GetProjectDBFolder(thDataWork);

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
                            await Task.Run(() => LoadTranslationFromDB(THFOpenBD.FileName)).ConfigureAwait(true);
                        }
                    }
                }
                IsOpeningInProcess = false;
            }
        }

        internal bool savemenusNOTenabled = true;
        private async void THFileElementsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (!Properties.Settings.Default.ProjectIsOpened)
                return;

            try
            {
                if (FileDataWasChanged && savemenusNOTenabled)
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

                if (rind > -1 && rind < thDataWork.THFilesElementsDataset.Tables[tableind].Rows.Count && (thDataWork.THFilesElementsDataset.Tables[tableind].Rows[rind][1] + string.Empty).Length > 0)
                {
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THAutoSetSameTranslationForSimular(tableind, rind, cind, false)));
                    //trans.Start();

                    await Task.Run(() => THAutoSetSameTranslationForSimular(tableind, rind, cind, false)).ConfigureAwait(false);
                }

                //Запуск автосохранения
                Autosave();
            }
            catch
            {
            }
        }

        internal bool IsTranslating;
        private void OnlineTranslateSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount() > 0)
            {
                IsTranslating = true;

                //координаты стартовой строк, колонки оригинала и номера таблицы
                int cind = THFileElementsDataGridView.Columns["Original"].Index;//-поле untrans
                int tableindex = THFilesList.SelectedIndex;
                int[] selindexes = FunctionsTable.GetDGVRowIndexsesInDataSetTable(thDataWork);

                ///*THMsg*/MessageBox.Show("selindexes[0]=" + selindexes[0] + "\r\ncind=" + cind + "\r\ntableindex=" + tableindex + "\r\nselected=" + selindexes.Length + ", lastselectedrowvalue=" + THFilesElementsDataset.Tables[tableindex].Rows[selindexes[0]][cind]);

                //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                //почемуто так не переводит, переводчик кидает ошибку при заппуске в другом потоке
                //await Task.Run(() => OnlineTranslateSelected(cind, tableindex, selindexes));  

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                //Thread trans = new Thread(new ParameterizedThreadStart((obj) => new FunctionsOnlineTranslation(thDataWork).THOnlineTranslate(cind, tableindex, selindexes, "s")));
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => new FunctionsOnlineTranslation(thDataWork).THOnlineTranslateByBigBlocks2(cind, tableindex, selindexes, "s")));
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
                //Thread trans = new Thread(new ParameterizedThreadStart((obj) => new FunctionsOnlineTranslation(thDataWork).THOnlineTranslate(cind, tableindex, selindexes, "t")));
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => new FunctionsOnlineTranslation(thDataWork).THOnlineTranslateByBigBlocks2(cind, tableindex, selindexes, "t")));
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
            IsTranslating = true;

            try
            {
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
                int dgvSelectedRowsCount = THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount();
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
                string result = string.Format(CultureInfo.InvariantCulture, Properties.Settings.Default.WebTranslationLink.Replace("{from}", "{0}").Replace("{to}", "{1}").Replace("{text}", "{2}"), TranslatorsTools.GetSourceLanguageID(), TranslatorsTools.GetTargetLanguageID(), HttpUtility.UrlEncode(value + string.Empty, Encoding.UTF8));
                //MessageBox.Show(result);
                Process.Start(result);
            }
            catch
            {
            }
        }

        private void THTargetTextBox_Leave(object sender, EventArgs e)
        {
            //int sel = dataGridView1.CurrentRow.Index; //присвоить перевенной номер выбранной строки в таблице
            //if (THSourceRichTextBox.Text.Length == 0)
            //{
            //}
            //else//если текстовое поле 2 не пустое
            //{
            //    //не менять, если значение текстбокса не поменялось
            //    if (THTargetRichTextBox.Text != thDataWork.TargetTextBoxPreValue)
            //    {
            //        THFileElementsDataGridView.CurrentRow.Cells["Translation"].Value = THTargetRichTextBox.Text;// Присвоить ячейке в ds.Tables[0] значение из TextBox2                   
            //    }
            //}
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
        /// <br/>Для выбранных ячеек, таблицы или для всех значений задать:
        /// <br/>method:
        /// <br/>"a" - All
        /// <br/>"t" - Table
        /// <br/>"s" - Selected
        /// <br/>..а также cind - индекс колонки, где ячейки перевода и tind - индекс таблицы, для вариантов Table и Selected
        /// <br/>Для одной выбранной ячейки, когда, например, определенная обрабатывается в коде, <br/>задать tind, cind и rind, а также true для onselectedonly
        /// </summary>
        /// <param name="method"></param>
        /// <param name="cind"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <param name="selectedonly"></param>
        private void THFixCells(string method, int cind, int tind, int rind = 0, bool forceApply = false)//cind - индекс столбца перевода, задан до старта потока
        {
            //возвращать, если занято, когда исправление в процессе
            if (THIsFixingCells)
            {
                return;
            }
            //установить занятость при старте
            THIsFixingCells = true;

            FunctionsAutoOperations.THFixCells(thDataWork, method, cind, tind, rind, forceApply);

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

            string ret = FunctionsAutoOperations.THExtractTextForTranslation(thDataWork, input);

            //снять занятость по окончании
            THIsExtractingTextForTranslation = false;
            return ret;
        }

        /// <summary>
        /// Work In Progress...
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal string[] THExtractTextForTranslationSplit(string input)
        {
            //возвращать, если занято, когда исправление в процессе
            if (THIsExtractingTextForTranslation)
            {
                return null;
            }
            //установить занятость при старте
            THIsExtractingTextForTranslation = true;

            var ret = FunctionsAutoOperations.THExtractTextForTranslationSplit(thDataWork, input);

            //снять занятость по окончании
            THIsExtractingTextForTranslation = false;
            return ret;
        }

        private void CellFixesSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FixSelectedCells();
        }

        private void FixSelectedCells(bool force = false)
        {
            CellFixes(1, force);

            //if (THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount() > 0)
            //{
            //    //эти два присвоены до начала нового потока, т.к. в другом потоке возникает исключение о попытке доступа к элементу управления, созданному в другом потоке
            //    //на самом деле здась даже не знаю, стоии ли оно того, чтобы кидать эту операцию на новый поток, она по идее и так должна за секунду выполниться
            //    //пока оставлю выполнение в другом потоке, как нибудь проверю без второго потока там, где много исправлять, по крайней мере оставлю для вариантов Таблица и все
            //    int cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
            //    int tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox

            //    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            //    //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("s", cind, tableindex)));
            //    //но при выборе только одной строчки почему-то кидает исключение
            //    //trans.Start();

            //    //убрал здесь выполнение во втором потоке, т.к. слишком мало править, не стоит того
            //    THFixCells("s", cind, tableindex, -1, force);
            //}
        }

        private void CellFixesTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CellFixes(2);

            ////эти два присвоены до начала нового потока, т.к. в другом потоке возникает исключение о попытке доступа к элементу управления, созданному в другом потоке
            ////на самом деле здась даже не знаю, стоии ли оно того, чтобы кидать эту операцию на новый поток, она по идее и так должна за секунду выполниться
            ////пока оставлю выполнение в другом потоке, как нибудь проверю без второго потока там, где много исправлять, по крайней мере оставлю для вариантов Таблица и все
            //int cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
            //int tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox

            ////http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("t", cind, tableindex)));
            ////но при выборе только одной строчки почему-то кидает исключение
            //trans.Start();

            ////THFixCells("t");
        }

        private void CellFixesAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CellFixes(3);

            ////эти два присвоены до начала нового потока, т.к. в другом потоке возникает исключение о попытке доступа к элементу управления, созданному в другом потоке
            ////на самом деле здась даже не знаю, стоии ли оно того, чтобы кидать эту операцию на новый поток, она по идее и так должна за секунду выполниться
            ////пока оставлю выполнение в другом потоке, как нибудь проверю без второго потока там, где много исправлять, по крайней мере оставлю для вариантов Таблица и все
            //int cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
            //int tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox

            ////http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("a", cind, tableindex)));
            ////но при выборе только одной строчки почему-то кидает исключение
            //trans.Start();
            ////THFixCells("a");
        }

        /// <summary>
        /// run cell fixes
        /// </summary>
        /// <param name="method">Selected method:<br/>1 = "s"selected<br/>2 = "t"table<br/>3 = "a"all</param>
        internal void CellFixes(int method = 1, bool force = false)
        {
            if (method == 1 && THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount() == 0)
                return;

            var cind = thDataWork.THFilesElementsDataset.Tables[THFilesList.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans               
            var tableindex = THFilesList.SelectedIndex;//установить индекс таблицы на выбранную в listbox

            Thread trans;
            if (method == 3) // all
            {
                trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("a", cind, tableindex)));
            }
            else if (method == 2) // table
            {
                trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("t", cind, tableindex)));
            }
            else //if (method == 1) // selected
            {
                trans = new Thread(new ParameterizedThreadStart((obj) => THFixCells("s", cind, tableindex, -1, force)));
            }
            trans.Start();
        }

        private void SetOriginalValueToTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SetOriginalToTranslation(thDataWork).Selected();
        }

        private void SetOriginalToTranslation()
        {
            int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount();
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

        bool cellchanged;
        public void THAutoSetSameTranslationForSimular(int InputTableIndex, int InputRowIndex, int InputCellIndex, bool forcerun = true, bool forcevalue = false)
        {
            if (forcevalue || (Properties.Settings.Default.AutotranslationForSimular && (cellchanged || forcerun))) //запуск только при изменении ячейки, чтобы не запускалось каждый раз. Переменная задается в событии изменения ячейки
            {
                FunctionsAutoOperations.THAutoSetSameTranslationForSimular(thDataWork, InputTableIndex, InputRowIndex, InputCellIndex, forcevalue);

                //LogToFile(string.Empty,true);
                cellchanged = false;
            }
        }

        int SelectedRowRealIndex = -1;
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
            if (e.RowIndex == -1)
            {
                RememberLastCellSelection();
            }
        }

        /// <summary>
        /// remember last selected real rowindex
        /// used materials:https://stackoverflow.com/questions/4819573/selected-rows-when-sorting-datagridview-in-winform-application
        /// </summary>
        private void RememberLastCellSelection()
        {
            if (THFileElementsDataGridView.SelectedCells.Count > 0)
            {
                SelectedRowRealIndex = FunctionsTable.GetDGVSelectedRowIndexInDatatable
                    (
                    thDataWork,
                    THFilesList.SelectedIndex,
                    THFileElementsDataGridView.SelectedCells[0].RowIndex
                    );
            }
        }

        //==============вырезать, копировать, вставить, для одной или нескольких ячеек

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DGVCellInEditMode)//если ячейка в режиме редактирования
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
            if (DGVCellInEditMode)//если ячейка в режиме редактирования
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
            if (DGVCellInEditMode)//если ячейка в режиме редактирования
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
                        FunctionsCopyPaste.PasteClipboardValue(THFileElementsDataGridView);
                    }
                }
            }
            //LogToFile("Paste End", true);
        }

        bool ClearSelectedCellsIsBusy;
        private async void ClearSelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClearSelectedCellsIsBusy || Properties.Settings.Default.DGVCellInEditMode)
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

        private async void SaveTranslationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog THFSaveBDAs = new SaveFileDialog())
            {
                THFSaveBDAs.Filter = "DB file|*.xml;*.cmx;*.cmz|XML-file|*.xml|Gzip compressed DB (*.cmx)|*.cmx|Deflate compressed DB (*.cmz)|*.cmz";

                THFSaveBDAs.InitialDirectory = FunctionsDBFile.GetProjectDBFolder(thDataWork);
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

                        switch (RPGMFunctions.THSelectedSourceType)
                        {
                            case "RPGMakerTransPatch":
                            case "RPG Maker game with RPGMTransPatch":
                                await Task.Run(() => new RPGMTransOLD(thDataWork).SaveRPGMTransPatchFiles(Properties.Settings.Default.THSelectedDir, RPGMFunctions.RPGMTransPatchVersion)).ConfigureAwait(true);
                                break;
                        }

                        //SaveNEWDB(THFilesElementsDataset, THFSaveBDAs.FileName);
                        //WriteDBFile(THFilesElementsDataset, THFSaveBDAs.FileName);

                        await Task.Run(() => WriteDBFileLite(thDataWork.THFilesElementsDataset, THFSaveBDAs.FileName)).ConfigureAwait(true);
                        //Task task = new Task(() => WriteDBFileLite(thDataWork.THFilesElementsDataset, THFSaveBDAs.FileName));
                        //task.Start();
                        //task.Wait();

                        FunctionsSounds.SaveDBComplete();
                        ProgressInfo(false);
                        //MessageBox.Show("finished");
                    }
                }
            }
        }

        bool WriteDBFileIsBusy;
        string WriteDBFileLiteLastFileName = string.Empty;
        private async void WriteDBFileLite(DataSet ds, string fileName)
        {
            if (fileName.Length == 0 || ds == null)
            {
                return;
            }

            try
            {
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

                Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath);
            }
            catch
            {
            }

            WriteDBFileIsBusy = false;
            WriteDBFileLiteLastFileName = string.Empty;
        }

        private void IndicateSaveProcess(string InfoText = "")
        {
            try
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
            catch
            {
            }
        }

        //private void SaveNEWDB(DataSet DS4Save, string fileName)
        //{
        //    //int TablesCount = DS4Save.Tables.Count;
        //    //for (int t = 0; t < TablesCount; t++)
        //    //{
        //    //    int RowsCount = DS4Save.Tables[t].Rows.Count;
        //    //    for (int r = 0; r < RowsCount; r++)
        //    //    {
        //    //        string
        //    //    }
        //    //}
        //}

        private async void RunTestGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (thDataWork.TablesLinesDict != null && thDataWork.TablesLinesDict.Count > 0)
            {
                thDataWork.TablesLinesDict.Clear();
            }

            if (thDataWork.CurrentProject != null || RPGMFunctions.THSelectedSourceType == "RPG Maker MV")
            {
                bool BuckupCreated = false;
                try
                {
                    bool success = false;
                    if (thDataWork.CurrentProject != null)
                    {
                        ProgressInfo(true, "Creating buckups");
                        if (!(BuckupCreated = thDataWork.CurrentProject.BakCreate()))
                            return;

                        success = await Task.Run(() => thDataWork.CurrentProject.Save()).ConfigureAwait(true);
                    }
                    else
                    {
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
                            ///*THMsg*/MessageBox.Show(Properties.Settings.Default.THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                            if (changed)
                            {

                                ///*THMsg*/MessageBox.Show("start writing");

                                //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                                success = await Task.Run(() => new RPGMMVOLD(thDataWork).WriteJson(THFilesList.Items[f] + string.Empty, Path.Combine(Properties.Settings.Default.THSelectedDir, "www", "data", THFilesList.Items[f] + ".json"))).ConfigureAwait(true);
                                if (!success)
                                {
                                    break;
                                }
                                //success = WriteJson(THFilesListBox.Items[f].ToString(), Properties.Settings.Default.THWorkProjectDir + "\\www\\data\\" + THFilesListBox.Items[f].ToString() + ".json");
                            }
                        }
                    }

                    if (success)
                    {
                        //using (Process Testgame = new Process())
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
                                //Testgame.StartInfo.FileName = largestexe;
                                //RPGMakerTransPatch.StartInfo.Arguments = string.Empty;
                                //Testgame.StartInfo.UseShellExecute = true;

                                //http://www.cyberforum.ru/windows-forms/thread31052.html
                                // свернуть
                                WindowState = FormWindowState.Minimized;

                                Process.Start("explorer.exe", Properties.Settings.Default.THSelectedDir);

                                FunctionsProcess.RunProcess(largestexe);

                                //await Task.Run(() => Testgame.Start()).ConfigureAwait(true);
                                //Testgame.WaitForExit();

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

                if (BuckupCreated)
                {
                    thDataWork.CurrentProject.BakRestore();
                }
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

        internal bool InteruptTranslation;
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

        internal THfrmSearch search;
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
                        search = new THfrmSearch(thDataWork, THFilesList, THFileElementsDataGridView, THTargetRichTextBox);
                    }

                    if (search.Visible)
                    {
                        search.Activate();//помещает на передний план
                        search.GetSelectedText();
                    }
                    else
                    {
                        search.Show();
                        search.GetSelectedText();
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
                try
                {

                    int realRowIndex = -1;
                    string columnName = string.Empty;

                    if (THFilesList.SelectedIndex == -1)
                    {
                        return;
                    }
                    int tableindex = THFilesList.SelectedIndex;
                    var cell = THFileElementsDataGridView.CurrentCell;

                    if (tableindex > -1 && cell != null)
                    {
                        columnName = THFileElementsDataGridView.Columns[cell.ColumnIndex].Name;
                        realRowIndex = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, tableindex, cell.RowIndex);
                    }

                    for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)
                    {
                        THFiltersDataGridView.Rows[0].Cells[c].Value = string.Empty;
                    }

                    var table = thDataWork.THFilesElementsDataset.Tables[tableindex];
                    table.DefaultView.RowFilter = string.Empty;
                    table.DefaultView.Sort = string.Empty;
                    THFileElementsDataGridView.Refresh();

                    if (realRowIndex > -1 && tableindex > -1 && columnName.Length > 0)
                    {
                        FunctionsTable.ShowSelectedRow(thDataWork, tableindex, columnName, realRowIndex);
                    }
                }
                catch
                {
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

        internal static bool DGVCellInEditMode
        {
            get => Properties.Settings.Default.DGVCellInEditMode;
            set => Properties.Settings.Default.DGVCellInEditMode = value;
        }

        private void THFileElementsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DGVCellInEditMode = true;
            //отключение действий для ячеек при входе в режим редктирования
            ControlsSwitch();
        }

        private void THFileElementsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DGVCellInEditMode = false;
            //влючение действий для ячеек при выходе из режима редктирования
            ControlsSwitch(true);
        }

        private void THSourceRichTextBox_MouseEnter(object sender, EventArgs e)
        {
            if (DGVCellInEditMode)
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
            if (DGVCellInEditMode)
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
        internal bool ControlsSwitchActivated;
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
            new CompleteRomajiotherLines(thDataWork).All();

            //int THFilesElementsDatasetTablesCount = thDataWork.THFilesElementsDataset.Tables.Count;
            //for (int t = 0; t < THFilesElementsDatasetTablesCount; t++)
            //{
            //    var table = thDataWork.THFilesElementsDataset.Tables[t];
            //    int tableRowsCount = table.Rows.Count;
            //    for (int r = 0; r < tableRowsCount; r++)
            //    {
            //        var row = table.Rows[r];
            //        //if ((THFilesElementsDataset.Tables[t].Rows[r][1] + string.Empty).Length == 0)//убрал проверку пустой ячейки, чтобы насильно переприсваивать
            //        //{
            //        if ((row[1] == null || string.IsNullOrEmpty(row[1] as string) || !Equals(row[1], row[0])) && (row[0] as string).HaveMostOfRomajiOtherChars())
            //        {
            //            thDataWork.THFilesElementsDataset.Tables[t].Rows[r][1] = row[0];
            //        }
            //        //}
            //    }
            //}
        }

        private void THFileElementsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Properties.Settings.Default.ProjectIsOpened)
                return;

            if (ControlsSwitchActivated)
            {
            }
            else
            {
                ControlsSwitch(true);//не включалось копирование в ячейку, при копировании с гугла назад
            }

            ShowNonEmptyRowsCount();//Show how many rows have translation
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
            MessageBox.Show(Properties.Settings.Default.EnableTranslationCache.ToString(CultureInfo.InvariantCulture));
        }

        private void SaveInnewFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ForceSameTranslationForIdenticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount() < 1)
            {
                return;
            }

            int[] selindexes = FunctionsTable.GetDGVRowIndexsesInDataSetTable(thDataWork);

            foreach (int index in selindexes)
            {
                THAutoSetSameTranslationForSimular(THFilesList.SelectedIndex, FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, index), 0, true, true);
            }
        }

        private void SplitLinesWhichLongestOfLimitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SplitLongLines(thDataWork).Selected();
            //SplitSelectedLines();
        }

        //private void SplitSelectedLines()
        //{
        //    int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount();
        //    if (THFileElementsDataGridViewSelectedCellsCount > 0)
        //    {
        //        try
        //        {
        //            int tableIndex = THFilesList.SelectedIndex;
        //            int cind = thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns["Original"].Ordinal;// Колонка Original
        //            int cindTrans = thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns["Translation"].Ordinal;// Колонка Original
        //            int[] selectedRowIndexses = new int[THFileElementsDataGridViewSelectedCellsCount];
        //            for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
        //            {
        //                //координаты ячейки
        //                selectedRowIndexses[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, THFilesList.SelectedIndex, THFileElementsDataGridView.SelectedCells[i].RowIndex);

        //            }
        //            foreach (var rind in selectedRowIndexses)
        //            {
        //                var row = thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows[rind];
        //                string origCellValue = row[cind] as string;
        //                string transCellValue = row[cindTrans] + string.Empty;
        //                if (!string.IsNullOrWhiteSpace(transCellValue)
        //                    && transCellValue != origCellValue
        //                    && FunctionsString.GetLongestLineLength(transCellValue) > Properties.Settings.Default.THOptionLineCharLimit
        //                    /*&& !FunctionsString.IsStringContainsSpecialSymbols(transCellValue)*/
        //                    && !thDataWork.CurrentProject.LineSplitProjectSpecificSkip(origCellValue, transCellValue, tableIndex, rind))
        //                {
        //                    row[1] = transCellValue.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit);
        //                    //row[1] = transCellValue.Wrap(Properties.Settings.Default.THOptionLineCharLimit);
        //                }

        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        private void SplitLinesWhichLongerOfLimitALLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SplitLongLines(thDataWork).All();
            //int TablesCount = thDataWork.THFilesElementsDataset.Tables.Count;
            //for (int t = 0; t < TablesCount; t++)
            //{
            //    var Table = thDataWork.THFilesElementsDataset.Tables[t];
            //    if (Table.TableName.ToUpperInvariant() == "PLUGINS.JS")
            //    {
            //        continue;
            //    }

            //    int TableRowsCount = Table.Rows.Count;
            //    for (int r = 0; r < TableRowsCount; r++)
            //    {
            //        var Row = Table.Rows[r];
            //        string CellValue;
            //        if (Row[1] != null
            //            && !string.IsNullOrEmpty(CellValue = Row[1] as string)
            //            && !Equals(Row[1], Row[0])
            //            && FunctionsString.GetLongestLineLength(CellValue) > Properties.Settings.Default.THOptionLineCharLimit
            //            /* || FunctionsString.IsStringContainsSpecialSymbols(CellValue)*/
            //            && !thDataWork.CurrentProject.LineSplitProjectSpecificSkip(Row[0] as string, CellValue, t, r))
            //        {
            //            Row[1] = CellValue.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit);
            //        }
            //        else
            //        {
            //        }
            //    }
            //}
            FunctionsSounds.GlobalFunctionFinishedWork();
        }

        private void FixMessagesInTheTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FixJPMessagesTranslation(thDataWork).Selected();
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
            if (!TableCompleteInfoLabel.Visible)
            {
                TableCompleteInfoLabel.Visible = false;
            }
        }

        private void TableCompleteInfoLabel_Click(object sender, EventArgs e)
        {
            FunctionsTable.ShowFirstRowWithEmptyTranslation(thDataWork);
        }

        private async void ExtraFixesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await Task.Run(() => ProceedHardcodedFixes()).ConfigureAwait(false);
        }

        bool HardcodedFixesExecuting;
        private void ProceedHardcodedFixes()
        {
            if (HardcodedFixesExecuting)
                return;

            HardcodedFixesExecuting = true;

            new AllHardFixes(thDataWork).All();

            //int TCount = thDataWork.THFilesElementsDataset.Tables.Count;

            //Parallel.For(0, TCount, t =>
            //    {
            //        var table = thDataWork.THFilesElementsDataset.Tables[t];
            //        var RCount = table.Rows.Count;
            //        Parallel.For(0, RCount,
            //            r =>
            //            {
            //                var row = table.Rows[r];
            //                var translation = row[1] + string.Empty;
            //                if (!string.IsNullOrWhiteSpace(row[0] + string.Empty))
            //                {
            //                    //Set result value
            //                    string result = FunctionsStringFixes.ApplyHardFixes(row[0] as string, translation, thDataWork);
            //                    if (!Equals(result, translation))
            //                    {
            //                        lock (row)
            //                        {
            //                            row[1] = result;
            //                        }
            //                    }
            //                }

            //            });
            //    });

            //for (int t = 0; t < TCount; t++)
            //{
            //    int RCount = thDataWork.THFilesElementsDataset.Tables[t].Rows.Count;
            //    for (int r = 0; r < RCount; r++)
            //    {
            //        var row = thDataWork.THFilesElementsDataset.Tables[t].Rows[r];

            //        //Set result value
            //        row[1] = FunctionsStringFixes.ApplyHardFixes(row[0] + string.Empty, row[1] + string.Empty);
            //    }
            //}

            HardcodedFixesExecuting = false;

            //clear translation cache
            if (thDataWork.OnlineTranslationCache != null)
            {
                thDataWork.OnlineTranslationCache = null;
            }
        }

        private void SelectedForceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FixSelectedCells(true);
        }

        private void ReloadRulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadTranslationRegexRules();
            ReloadCellFixesRegexRules();
            FunctionsSounds.LoadDBCompleted();
        }

        internal void ReloadTranslationRegexRules()
        {
            //Reload TranslationRegexRules
            if (thDataWork.TranslationRegexRules.Count > 0)
            {
                thDataWork.TranslationRegexRules.Clear();
            }

            //если файл с правилами существует
            if (File.Exists(THSettingsData.TranslationRegexRulesFilePath()))
            {
                //читать файл с правилами
                using (var rules = new StreamReader(THSettingsData.TranslationRegexRulesFilePath()))
                {
                    //regex правило и результат из файла
                    var regexPattern = string.Empty;
                    var regexReplacement = string.Empty;
                    var ReadRule = true;
                    while (!rules.EndOfStream)
                    {
                        try
                        {
                            //читать правило и результат
                            if (ReadRule)
                            {
                                regexPattern = rules.ReadLine();
                                if (string.IsNullOrWhiteSpace(regexPattern) || regexPattern.TrimStart().StartsWith(";"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                                continue;
                            }
                            else
                            {
                                regexReplacement = rules.ReadLine();
                                if (string.IsNullOrWhiteSpace(regexPattern) || regexReplacement.TrimStart().StartsWith(";") || !FunctionsString.IsStringAContainsStringB(regexReplacement, "$"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                            }

                            if (!thDataWork.TranslationRegexRules.ContainsKey(regexPattern))
                            {
                                thDataWork.TranslationRegexRules.Add(regexPattern, regexReplacement);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        internal void ReloadCellFixesRegexRules()
        {
            //Reload TranslationRegexRules
            if (thDataWork.CellFixesRegexRules.Count > 0)
            {
                thDataWork.CellFixesRegexRules.Clear();
            }

            //если файл с правилами существует
            if (File.Exists(THSettingsData.CellFixesRegexRulesFilePath()))
            {
                //читать файл с правилами
                using (var rules = new StreamReader(THSettingsData.CellFixesRegexRulesFilePath()))
                {
                    //regex правило и результат из файла
                    var regexPattern = string.Empty;
                    var regexReplacement = string.Empty;
                    var ReadRule = true;
                    while (!rules.EndOfStream)
                    {
                        try
                        {
                            //читать правило и результат
                            if (ReadRule)
                            {
                                regexPattern = rules.ReadLine();
                                if (string.IsNullOrEmpty(regexPattern) || regexPattern.TrimStart().StartsWith(";"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                                continue;
                            }
                            else
                            {
                                regexReplacement = rules.ReadLine();
                                if (string.IsNullOrEmpty(regexPattern) || regexReplacement.TrimStart().StartsWith(";"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                            }

                            thDataWork.CellFixesRegexRules.AddTry(regexPattern, regexReplacement);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        private void THTargetRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.DGVCellInEditMode && (sender as RichTextBox).Focused && THFileElementsDataGridView.CurrentRow.Index > -1)
            {
                THFileElementsDataGridView.Rows[Properties.Settings.Default.DGVSelectedRowIndex].Cells["Translation"].Value = (sender as RichTextBox).Text;
            }

            TranslationLongestLineLenghtLabel.Text = FunctionsString.GetLongestLineLength((sender as RichTextBox).Text).ToString(CultureInfo.InvariantCulture);
            if (!tlpTextLenPosInfo.Visible)
                tlpTextLenPosInfo.Visible = true;
        }

        private void THFileElementsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateTextboxes();
        }

        private void THTargetRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            TargetTextBoxLinePositionLabelData.Text = (sender as RichTextBox).CurrentCharacterPosition().X.ToString(CultureInfo.InvariantCulture);
            TargetTextBoxColumnPositionLabelData.Text = (sender as RichTextBox).CurrentCharacterPosition().Y.ToString(CultureInfo.InvariantCulture);
            TranslationLongestLineLenghtLabel.Text = (sender as RichTextBox).CurrentSelectedTextLength().ToString(CultureInfo.InvariantCulture);
            if (!tlpTextLenPosInfo.Visible)
                tlpTextLenPosInfo.Visible = true;
        }

        private void THFileElementsDataGridView_Sorted(object sender, EventArgs e)
        {
            ReselectCellSelectedBeforeSorting();
        }

        /// <summary>
        /// reselect cell which was selected before column was sorted
        /// used materials:https://stackoverflow.com/questions/4819573/selected-rows-when-sorting-datagridview-in-winform-application
        /// </summary>
        private void ReselectCellSelectedBeforeSorting()
        {
            if (SelectedRowRealIndex > -1)
            {
                foreach (DataGridViewRow row in THFileElementsDataGridView.Rows)
                {
                    int rowindex;
                    int realrowindex;
                    //int i = Properties.Settings.Default.DGVSelectedRowIndex;
                    //int r = Properties.Settings.Default.DGVSelectedRowRealIndex;
                    if (SelectedRowRealIndex ==
                        (realrowindex = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork,
                        Properties.Settings.Default.THFilesListSelectedIndex,
                        rowindex = row.Index))
                        )
                    {
                        FunctionsTable.ShowSelectedRow(thDataWork, THFilesList.SelectedIndex, Properties.Settings.Default.DGVSelectedColumnIndex, rowindex);
                        Properties.Settings.Default.DGVSelectedRowIndex = rowindex;
                        Properties.Settings.Default.DGVSelectedRowRealIndex = realrowindex;
                        SelectedRowRealIndex = realrowindex;
                        break;
                    }
                }
            }
        }

        private void OpenProjectsDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folder;
            if (Directory.Exists(folder = Properties.Settings.Default.THProjectWorkDir))
            {
            }
            else
            {
                folder = Properties.Settings.Default.THSelectedDir;
            }
            Process.Start("explorer.exe", folder);
        }

        private void openTranslationRulesFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(THSettingsData.TranslationRegexRulesFilePath()))
            {
                Process.Start("explorer.exe", THSettingsData.TranslationRegexRulesFilePath());
            }
        }

        private void openCellFixesFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(THSettingsData.CellFixesRegexRulesFilePath()))
            {
                Process.Start("explorer.exe", THSettingsData.CellFixesRegexRulesFilePath());
            }
        }

        private void THInfolabel_Click(object sender, EventArgs e)
        {

        }

        private void THSourceRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!tlpTextLenPosInfo.Visible)
                tlpTextLenPosInfo.Visible = true;
        }

        private void testXorDecriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tests.Xorfornscript.DecryptXor();
        }

        private void testXorEncriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tests.Xorfornscript.EncryptXor();
        }

        private void THfrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (THFileElementsDataGridViewOriginalToTranslationHotkey != null)
            {
                THFileElementsDataGridViewOriginalToTranslationHotkey.Dispose();
                THFileElementsDataGridViewOriginalToTranslationHotkey = null;
            }
        }

        private void TSMIEnQuotesToJp_Click(object sender, EventArgs e)
        {
            new ENQuotesToJP(thDataWork).Selected();
        }

        private void TSMISetOriginalToTranslationAll_Click(object sender, EventArgs e)
        {
            new SetOriginalToTranslation(thDataWork).All();
        }

        private void TSMISetOriginalToTranslationTable_Click(object sender, EventArgs e)
        {
            new SetOriginalToTranslation(thDataWork).Table();
        }

        private void TSMIEnQuotesAll_Click(object sender, EventArgs e)
        {
            new ENQuotesToJP(thDataWork).All();
        }

        private void TSMIEnQuotesTable_Click(object sender, EventArgs e)
        {
            new ENQuotesToJP(thDataWork).Table();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach (var c in new char[] { '「', '>', '\0', '\n', '\\', '-', '>' })
            //{
            //    var c1 = char.IsSymbol(c);
            //    var c2 = char.IsWhiteSpace(c);
            //    var c3 = char.IsControl(c);
            //    var c4 = char.IsSurrogate(c);
            //    var c5 = char.IsHighSurrogate(c);
            //    var c6 = char.IsLowSurrogate(c);
            //    var c7 = char.IsLetterOrDigit(c);
            //    var c8 = char.IsPunctuation(c);
            //    var c9 = char.IsSeparator(c);
            //}

        }

        private void HardFixesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AllHardFixes(thDataWork).Selected();
        }

        private void fixMessagesForAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FixJPMessagesTranslation(thDataWork).All();
        }

        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FixJPMessagesTranslation(thDataWork).Table();
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