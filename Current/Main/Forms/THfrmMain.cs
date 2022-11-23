using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using CSRegisterHotkey;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular;
using TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Main.Functions;

namespace TranslationHelper
{
    public partial class FormMain : Form
    {
        internal string extractedpatchpath = string.Empty;

        internal string FVariant = string.Empty;


        internal static string THTranslationCachePath
        {
            get => AppSettings.THTranslationCachePath;
            set => AppSettings.THTranslationCachePath = value;
        }

        public FormMain()
        {
            InitializeComponent();
            //LangF = new THLang();

            AppData.Init(this);

            AppSettings.ApplicationStartupPath = Application.StartupPath;
            AppSettings.ApplicationProductName = Application.ProductName;
            AppSettings.NewLine = Environment.NewLine;

            FunctionsMenus.CreateMainMenus(this.MainMenu);

            FunctionsHotkeys.BindShortCuts();

            AppData.SetSettings();

            SetUIStrings();

            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            THFilesList.SetDrawMode(DrawMode.OwnerDrawFixed);

            THTranslationCachePath = THSettings.THTranslationCacheFilePath;

            //THFileElementsDataGridView set doublebuffered to true
            SetDoublebuffered(true);
            if (File.Exists(THSettings.THLogPath) && new FileInfo(THSettings.THLogPath).Length > 1000000)
            {
                File.Delete(THSettings.THLogPath);
            }

            FunctionsOpen.UpdateRecentFiles();

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        private void SetUIStrings()
        {
            //Menu File
            //this.fileToolStripMenuItem.Text = T._("File");
            ////Menu Edit
            //this.EditToolStripMenuItem.Text = T._("Edit");
            //this.tryToTranslateOnlineToolStripMenuItem.Text = T._("Translate Online");
            //this.SelectedToolStripMenuItem1.Text = T._("Selected");
            //this.TableToolStripMenuItem1.Text = T._("Table");
            //this.allToolStripMenuItem1.Text = T._("All");
            //this.translationInteruptToolStripMenuItem.Text = T._("Interupt");
            //this.fixCellSpecialSymbolsToolStripMenuItem.Text = T._("Fix cell special symbols");
            //this.FixCellsSelectedToolStripMenuItem.Text = T._("Selected");
            //this.FixCellsTableToolStripMenuItem.Text = T._("Table");
            //this.allToolStripMenuItem.Text = T._("All");
            //this.SetOriginalValueToTranslationToolStripMenuItem.Text = T._("Translation=Original");
            //this.CompleteRomajiotherLinesToolStripMenuItem.Text = T._("Complete Romaji/Other lines");
            //this.CompleteRomajiotherLinesToolStripMenuItem1.Text = T._("Complete Romaji/Other lines");
            //this.ForceSameForSimularToolStripMenuItem.Text = T._("Force same for simular");
            //this.ForceSameForSimularToolStripMenuItem1.Text = T._("Force same for simular");
            //this.CutToolStripMenuItem1.Text = T._("Cut");
            //this.CopyCellValuesToolStripMenuItem.Text = T._("Copy");
            //this.PasteCellValuesToolStripMenuItem.Text = T._("Paste");
            //this.ClearSelectedCellsToolStripMenuItem.Text = T._("Clear selected cells");
            //this.ToUPPERCASEToolStripMenuItem.Text = T._("UPPERCASE");
            //this.FirstCharacterToUppercaseToolStripMenuItem.Text = T._("Uppercase");
            //this.ToLowercaseToolStripMenuItem.Text = T._("lowercase");
            //this.searchToolStripMenuItem.Text = T._("Search");
            ////Menu View
            //this.ViewToolStripMenuItem.Text = T._("View");
            //this.SetColumnSortingToolStripMenuItem.Text = T._("Reset column sorting");
            ////Menu Options
            //this.optionsToolStripMenuItem.Text = T._("Options");
            //this.settingsToolStripMenuItem.Text = T._("Settings");
            ////Menu Help
            //this.helpToolStripMenuItem.Text = T._("Help");
            //this.aboutToolStripMenuItem.Text = T._("About");
            ////Contex menu
            //this.OpenInWebContextToolStripMenuItem.Text = T._("Open in web");
            //this.toolStripMenuItem6.Text = T._("Translate Online");
            //this.TranslateSelectedContextToolStripMenuItem.Text = T._("Selected");
            //this.TranslateTableContextToolStripMenuItem.Text = T._("Table");
            //this.toolStripMenuItem9.Text = T._("All");
            //this.translationInteruptToolStripMenuItem1.Text = T._("Interupt");
            //this.toolStripMenuItem2.Text = T._("Fix cell special symbols");
            //this.FixSymbolsContextToolStripMenuItem.Text = T._("Selected");
            //this.FixSymbolsTableContextToolStripMenuItem.Text = T._("Table");
            //this.toolStripMenuItem5.Text = T._("All");
            //this.OriginalToTransalationContextToolStripMenuItem.Text = T._("Translation=Original");
            //this.CutToolStripMenuItem.Text = T._("Cut");
            //this.CopyCMStripMenuItem.Text = T._("Copy");
            //this.PasteToolStripMenuItem.Text = T._("Paste");
            //this.CleanSelectedCellsToolStripMenuItem1.Text = T._("Clear selected cells");
            //this.ToolStripMenuItem14.Text = T._("UPPERCASE");
            //this.UppercaseToolStripMenuItem.Text = T._("Uppercase");
            //this.LowercaseToolStripMenuItem.Text = T._("lowercase");
        }

        private void THMain_Load(object sender, EventArgs e)
        {
            SetTooltips();

            //Disable links detection in edition textboxes
            THSourceRichTextBox.DetectUrls = false;
            THTargetRichTextBox.DetectUrls = false;

            //Hide some items 
            AppData.Main.tlpTextLenPosInfo.Visible = false;
            AppData.Main.frmMainPanel.Visible = false;
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

        internal volatile bool IsOpeningInProcess;

        private void SetDoublebuffered(bool value)
        {
            // Double buffering can make DGV slow in remote desktop
            if (!SystemInformation.TerminalServerSession)
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
            if (THFilesListBox_MouseClickBusy || THFilesList.GetSelectedIndex() == -1) //THFilesList.GetSelectedIndex() == -1 return - фикс исключения сразу после загрузки таблицы, когда индекс выбранной таблицы равен -1 
            {
                return;
            }

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

                if (THFilesList.GetSelectedIndex() > -1)
                {
                    AppSettings.THFilesListSelectedIndex = THFilesList.GetSelectedIndex();

                    BindToDataTableGridView(AppData.CurrentProject.FilesContent.Tables[AppSettings.THFilesListSelectedIndex]);
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

        private void HideAllColumnsExceptOriginalAndTranslation()
        {
            if (THFileElementsDataGridView.Columns.Count == 2)
            {
                return;
            }

            foreach (DataGridViewColumn Column in THFileElementsDataGridView.Columns)
            {
                if (Column.Name != THSettings.TranslationColumnName && Column.Name != THSettings.OriginalColumnName)
                {
                    Column.Visible = false;
                }
            }
        }

        public void BindToDataTableGridView(DataTable DT)
        {
            if (THFilesList != null && THFilesList.GetSelectedIndex() > -1 && DT != null)//вторая попытка исправить исключение при выборе элемента списка
            {
                try
                {
                    if (DT.TableName == "[ALL]" && AppData.CurrentProject.FilesContent.Tables.Count > 1)
                    {
                        //отображение содержимого всех таблиц в одной
                        //https://stackoverflow.com/questions/11099619/how-to-bind-dataset-to-datagridview-in-windows-application
                        //int ThisInd = THFilesList.GetSelectedIndex();
                        //using (DataTable dtnew = ProjectData.THFilesElementsDataset.Tables[0].Copy())
                        //{
                        //    for (var i = 1; i < ProjectData.THFilesElementsDataset.Tables.Count; i++)
                        //    {
                        //        if (i!= ThisInd)
                        //        {
                        //            dtnew.Merge(ProjectData.THFilesElementsDataset.Tables[i]);
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
            //ControlsSwitchIsOn = (CutToolStripMenuItem1.ShortcutKeys != Keys.None);

            if (THFileElementsDataGridView != null && THFileElementsDataGridView.Columns.Count > 1)
            {
                THFileElementsDataGridView.Columns[THSettings.OriginalColumnName].HeaderText = T._("Original");//THMainDGVOriginalColumnName;
                THFileElementsDataGridView.Columns[THSettings.TranslationColumnName].HeaderText = T._("Translation");//THMainDGVTranslationColumnName;
                THFileElementsDataGridView.Columns[THSettings.OriginalColumnName].ReadOnly = true;
                THFiltersDataGridView.Enabled = true;
                THSourceRichTextBox.Enabled = true;
                THTargetRichTextBox.Enabled = true;
            }

            //SelectedToolStripMenuItem1.Enabled = true;
            //TableToolStripMenuItem1.Enabled = true;
            //FixCellsSelectedToolStripMenuItem.Enabled = true;
            //FixCellsTableToolStripMenuItem.Enabled = true;
            //SetOriginalValueToTranslationToolStripMenuItem.Enabled = true;
            //CompleteRomajiotherLinesToolStripMenuItem.Enabled = true;
            //CompleteRomajiotherLinesToolStripMenuItem1.Enabled = true;
            //ForceSameForSimularToolStripMenuItem.Enabled = true;
            //ForceSameForSimularToolStripMenuItem1.Enabled = true;
            //CutToolStripMenuItem1.Enabled = true;
            //CopyCellValuesToolStripMenuItem.Enabled = true;
            //PasteCellValuesToolStripMenuItem.Enabled = true;
            //ClearSelectedCellsToolStripMenuItem.Enabled = true;
            //ToUPPERCASEToolStripMenuItem.Enabled = true;
            //FirstCharacterToUppercaseToolStripMenuItem.Enabled = true;
            //ToLowercaseToolStripMenuItem.Enabled = true;
            //SetColumnSortingToolStripMenuItem.Enabled = true;
            //OpenInWebContextToolStripMenuItem.Enabled = true;
            //TranslateSelectedContextToolStripMenuItem.Enabled = true;
            //TranslateTableContextToolStripMenuItem.Enabled = true;
            //FixSymbolsContextToolStripMenuItem.Enabled = true;
            //FixSymbolsTableContextToolStripMenuItem.Enabled = true;
            //OriginalToTransalationContextToolStripMenuItem.Enabled = true;
            //CutToolStripMenuItem.Enabled = true;
            //CopyCMStripMenuItem.Enabled = true;
            //PasteToolStripMenuItem.Enabled = true;
            //CleanSelectedCellsToolStripMenuItem1.Enabled = true;
            //ToolStripMenuItem14.Enabled = true;
            //UppercaseToolStripMenuItem.Enabled = true;
            //LowercaseToolStripMenuItem.Enabled = true;
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
                        _ = THFiltersDataGridView.Columns.Add(THFileElementsDataGridView.Columns[cindx].Name, THFileElementsDataGridView.Columns[cindx].HeaderText);
                    }
                }
                _ = THFiltersDataGridView.Rows.Add(1);
                THFiltersDataGridView.CurrentRow.Selected = false;

                //во время прокрутки DGV чернела полоса прокрутки и в результате было получено исключение
                //добавил это для возможного фикса. возможно это этот dgv
                //https://fooobar.com/questions/1404812/datagridview-scrollbar-throwing-argumentoutofrange-exception
                THFiltersDataGridView.PerformLayout();
                //почернения прокрутки вроде больше не видел, но ошибка с аргументом вне диапазона была снова
            }
        }

        private void THFileElementsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _ = this.Invoke((Action)(() => CellEnterActions(sender, e)));
        }

        private void CellEnterActions(object sender, DataGridViewCellEventArgs e)
        {
            //UpdateTextboxes(sender, e);
        }

        internal void UpdateTextboxes()
        {
            try
            {
                if (THFileElementsDataGridView.CurrentCell == null) return;

                var tableIndex = AppSettings.THFilesListSelectedIndex = THFilesList.GetSelectedIndex();
                if (tableIndex == -1) return;

                var columnIndex = AppSettings.DGVSelectedColumnIndex = THFileElementsDataGridView.CurrentCell.ColumnIndex;
                if (columnIndex == -1) return;

                var rowIndex = AppSettings.DGVSelectedRowIndex = THFileElementsDataGridView.CurrentCell.RowIndex;
                if (rowIndex == -1) return;

                var realrowIndex = AppSettings.DGVSelectedRowRealIndex = FunctionsTable.GetDGVSelectedRowIndexInDatatable(tableIndex, rowIndex);
                if (realrowIndex == -1) return;

                if (THFileElementsDataGridView.DataSource == null || rowIndex == -1 || tableIndex == -1)
                {
                    THSourceRichTextBox.Clear();
                    THTargetRichTextBox.Clear();
                    return;
                }

                //Считывание значения ячейки в текстовое поле 1, вариант 2, для DataSet, ds.Tables[0]
                //Проверка на размер индексов, для избежания ошибки при попытке сортировки " должен быть положительным числом и его размер не должен превышать размер коллекции"
                if (!THSourceRichTextBox.Enabled
                    //&& THFileElementsDataGridView.CurrentCell != null
                    || THFileElementsDataGridView.Rows.Count == 0
                    //|| rowIndex == -1
                    || columnIndex == -1)
                {
                    return;
                }

                THTargetRichTextBox.Clear();

                if (string.IsNullOrEmpty(THFileElementsDataGridView.Rows[THFileElementsDataGridView.CurrentCell.RowIndex].Cells[THSettings.OriginalColumnName].Value + string.Empty))
                {
                    THSourceRichTextBox.Clear();
                }
                else//проверить, не пуста ли ячейка, иначе была бы ошибка //THStrDGTranslationColumnName ошибка при попытке сортировки по столбцу
                {
                    //wrap words fix: https://stackoverflow.com/questions/1751371/how-to-use-n-in-a-textbox
                    THSourceRichTextBox.Text = (THFileElementsDataGridView.Rows[rowIndex].Cells[THSettings.OriginalColumnName].Value + string.Empty);
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
                if (string.IsNullOrEmpty(TranslationCellValue = THFileElementsDataGridView.Rows[rowIndex].Cells[THSettings.TranslationColumnName].Value + string.Empty))
                {
                    THTargetRichTextBox.Clear();
                }
                else//проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                {
                    //запоминание последнего значения ячейки перед считыванием в THTargetRichTextBox,
                    //для предотвращения записи значения обратно в ячейку, если она была изменена до изменения текстбокса
                    AppData.TargetTextBoxPreValue = TranslationCellValue;

                    //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                    THTargetRichTextBox.Text = AppData.TargetTextBoxPreValue;

                    FormatTextBox();

                    //THTargetRichTextBox.Select(AppSettings.THOptionLineCharLimit+1, THTargetRichTextBox.Text.Length);
                    //THTargetRichTextBox.SelectionColor = Color.Red;

                    TranslationLongestLineLenghtLabel.Text = FunctionsString.GetLongestLineLength(TranslationCellValue.ToString(CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                    TargetTextBoxLinePositionLabelData.Text = string.Empty;
                }

                THInfoTextBox.Text = string.Empty;

                string SelectedCellValue;
                if ((SelectedCellValue = THFileElementsDataGridView.Rows[rowIndex].Cells[columnIndex].Value + string.Empty).Length == 0)
                {
                    return;
                }

                //gem furigana
                //https://github.com/helephant/Gem
                //var furigana = new Furigana(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                //THInfoTextBox.Text += furigana.Reading + "\r\n";
                //THInfoTextBox.Text += furigana.Expression + "\r\n";
                //THInfoTextBox.Text += furigana.Hiragana + "\r\n";
                //THInfoTextBox.Text += furigana.ReadingHtml + "\r\n";

                if (AppData.CurrentProject.FilesContentInfo != null && AppData.CurrentProject.FilesContentInfo.Tables.Count > tableIndex && AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows.Count > rowIndex)
                {
                    THInfoTextBox.Text += T._("rowinfo:") + Environment.NewLine + AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows[rowIndex][0];
                }

                THInfoTextBox.Text += Environment.NewLine + T._("Selected bytes length") + ":" + " UTF8" + "=" + Encoding.UTF8.GetByteCount(SelectedCellValue) + "/932" + "=" + Encoding.GetEncoding(932).GetByteCount(SelectedCellValue);

                if (AppData.CurrentProject.Name == "RPG Maker MV")
                {
                    THInfoTextBox.Text += Environment.NewLine + Environment.NewLine + T._("Several strings also can be in Plugins.js in 'www\\js' folder and referred plugins in plugins folder.");
                }
                THInfoTextBox.Text += Environment.NewLine + Environment.NewLine;
                THInfoTextBox.Text += FunctionsRomajiKana.GetLangsOfString(SelectedCellValue, "all"); //Show all detected languages count info
                //--------Считывание значения ячейки в текстовое поле 1
            }
            catch
            {
            }
        }

        //https://stackoverflow.com/a/31150444
        private void FormatTextBox()
        {
            if (THTargetRichTextBox == null) return;

            int tl = 0;
            _ = this.Invoke((Action)(() => tl = THTargetRichTextBox.Text.Length));
            if (tl == 0) return;


            // Loop over each line
            int THTargetRichTextBoxLinesCount = 0;
            _ = this.Invoke((Action)(() => THTargetRichTextBoxLinesCount = THTargetRichTextBox.Lines.Length));
            for (int i = 0; i < THTargetRichTextBoxLinesCount; i++)
            {
                // Current line text
                string currentLine = string.Empty;
                _ = this.Invoke((Action)(() => currentLine = THTargetRichTextBox.Lines[i]));

                // Ignore the non-assembly lines
                if (currentLine.Length <= AppSettings.THOptionLineCharLimit) continue;

                // Start position
                int start = AppSettings.THOptionLineCharLimit;

                // Length
                int length = currentLine.Length - start;

                // Make the selection
                THTargetRichTextBox.SelectionStart = start;
                THTargetRichTextBox.SelectionLength = length;

                // Change the colour
                THTargetRichTextBox.SelectionColor = Color.DarkRed;
            }
        }

        private void ShowNonEmptyRowsCount()
        {
            int RowsCount = FunctionsTable.GetDatasetRowsCount(AppData.CurrentProject.FilesContent);
            if (RowsCount == 0)
            {
                TableCompleteInfoLabel.Visible = false;
            }
            else
            {
                TableCompleteInfoLabel.Visible = true;
                TableCompleteInfoLabel.Text = FunctionsTable.GetDatasetNonEmptyRowsCount(AppData.CurrentProject.FilesContent) + "/" + RowsCount;
            }
        }

        internal volatile bool SaveInAction;
        internal bool FileDataWasChanged;

        /// <summary>
        /// control progressbar Current\Max
        /// </summary>
        /// <param name="CurrentProgressBar">current value</param>
        /// <param name="MaxProgressBar">max value</param>
        public void ProgressInfo(int CurrentProgressBar, int MaxProgressBar)
        {
            ProgressInfo(true, "", true, CurrentProgressBar, MaxProgressBar);
        }

        /// <summary>
        /// show status text in left-bottom field of window
        /// </summary>
        /// <param name="ShowStatus">show status</param>
        /// <param name="StatusText"></param>
        public void ProgressInfo(string StatusText = "")
        {
            ProgressInfo(StatusText.Length > 0, StatusText);
        }

        /// <summary>
        /// show status text in left-bottom field of window
        /// </summary>
        /// <param name="ShowStatus">show status</param>
        /// <param name="StatusText"></param>
        public void ProgressInfo(bool ShowStatus, string StatusText = "", bool SetProgressBar = false, int CurrentProgressBar = -1, int MaxProgressBar = -1)
        {
            if (SetProgressBar && THActionProgressBar.Visible && THInfolabel.Visible && CurrentProgressBar != -1 && MaxProgressBar != -1)
            {
                if (THActionProgressBar.Style != ProgressBarStyle.Continuous)
                {
                    _ = THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Style = ProgressBarStyle.Continuous));
                }
                if (THActionProgressBar.Maximum != MaxProgressBar)
                {
                    _ = THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Maximum = MaxProgressBar));
                }
                if (CurrentProgressBar < MaxProgressBar)
                {
                    _ = THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Value = CurrentProgressBar));
                }
            }
            else
            {
                StatusText = StatusText?.Length == 0 ? T._("working..") : StatusText;
                try
                {
                    _ = THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Visible = ShowStatus));
                    _ = THInfolabel.Invoke((Action)(() => THInfolabel.Visible = ShowStatus));
                    if (!ShowStatus)
                    {
                        _ = THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Style = ProgressBarStyle.Marquee));
                    }
                    _ = THInfolabel.Invoke((Action)(() => THInfolabel.Text = StatusText));
                }
                catch
                {
                }
            }
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
                AppData.CurrentProject.FilesContent.Tables[THFilesList.GetSelectedIndex()].DefaultView.RowFilter = OverallFilter;
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
            if (!AppSettings.ProjectIsOpened || THFilesList.GetSelectedIndex() == -1)
                return;

            var grid = sender as DataGridView;

            int rowIdx = FunctionsTable.GetDGVSelectedRowIndexInDatatable(THFilesList.GetSelectedIndex(), e.RowIndex);//здесь получаю реальный индекс из Datatable
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
            if (AppSettings.DGVCellInEditMode && sender is DataGridView DGV)
            {
                THTargetRichTextBox.Text = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + string.Empty;
            }
        }

        string dbpath;
        internal string lastautosavepath;

        bool AutosaveActivated;
        private void Autosave()
        {
            if (!AppSettings.EnableDBAutosave || AutosaveActivated || AppData.CurrentProject.FilesContent == null)
            {
            }
            else
            {
                AutosaveActivated = true;

                dbpath = Path.Combine(Application.StartupPath, "DB");
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

                Thread IndicateSave = new Thread(new ParameterizedThreadStart((obj) => IndicateSaveProcess(T._("Saving") + "...")));
                IndicateSave.Start();

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread trans = new Thread(new ParameterizedThreadStart((obj) => SaveLoop(AppData.CurrentProject.FilesContent, autosavepath)));
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
                while (i < AppSettings.DBAutoSaveTimeout && AppSettings.EnableDBAutosave)
                {
                    Thread.Sleep(1000);
                    if (AppSettings.IsTranslationHelperWasClosed || this == null || IsDisposed/* || Data == null || Path.Length == 0*/)
                    {
                        AutosaveActivated = false;
                        return;
                    }
                    i++;
                }
                while (IsOpeningInProcess || SaveInAction)//не запускать автосохранение, пока утилита занята
                {
                    Thread.Sleep(AppSettings.DBAutoSaveTimeout * 1000);
                }
                WriteDBFileLite(Data, Path);
            }
        }

        private void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnLockDBLoad(false);
            LoadDB();
            //Invoke((Action)(() => LoadTranslationToolStripMenuItem.Enabled = true));
        }

        internal void UnLockDBLoad(bool unlock = true)
        {
            //Invoke((Action)(() =>
            //{
            //    LoadTranslationToolStripMenuItem.Enabled = unlock;
            //    LoadTrasnlationAsToolStripMenuItem.Enabled = unlock;
            //    LoadTrasnlationAsForcedToolStripMenuItem.Enabled = unlock;
            //}));
        }

        internal async void LoadDB(bool force = true)
        {
            var lastautosavepath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName() + FunctionsDBFile.GetDBCompressionExt());
            this.lastautosavepath = lastautosavepath;
            if (File.Exists(lastautosavepath))
            {
                await Task.Run(() => LoadTranslationFromDB(lastautosavepath, false, force)).ConfigureAwait(true);
            }
            else
            {
                var result = MessageBox.Show(T._("DB not found. Try to load from all exist?"), T._("DB not found"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    await Task.Run(() => AppData.Main.LoadTranslationFromDB(lastautosavepath, true)).ConfigureAwait(true);
                }
            }
        }

        bool LoadTranslationToolStripMenuItem_ClickIsBusy;
        internal async void LoadTranslationFromDB(string sPath = "", bool UseAllDB = false, bool forced = false)
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
                FunctionsDBFile.MergeAllDBtoOne();
                new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionaryParallellTables(AppData.AllDBmerged);
            }
            else
            {
                using (DataSet DBDataSet = new DataSet())
                {

                    //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                    await Task.Run(() => ReadDBAndLoadDBCompare(DBDataSet, sPath, forced)).ConfigureAwait(true);
                }
            }


            _ = THFileElementsDataGridView.Invoke((Action)(() => THFileElementsDataGridView.Refresh()));


            LoadTranslationToolStripMenuItem_ClickIsBusy = false;
            FunctionsSounds.LoadDBCompleted();
            _ = THFilesList.Invoke((Action)(() => THFilesList.Refresh()));
        }

        private void ReadDBAndLoadDBCompare(DataSet DBDataSet, string sPath, bool forced = false)
        {
            if (sPath.Length == 0)
            {
                sPath = AppData.Settings.THConfigINI.GetKey("Paths", "LastAutoSavePath");
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
                //if (ProjectData.Main.THFileElementsDataGridView.DataSource != null)
                //{
                //    tableSourceWasCleaned = true;
                //    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                //    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                //    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                //}

                //стандартное считывание. Самое медленное
                //new FunctionsLoadTranslationDB().THLoadDBCompare(DBDataSet);

                //считывание через словарь с предварительным чтением в dataset и конвертацией в словарь
                //Своего рода среднее решение, которое быстрее решения с сравнением из БД в DataSet
                //и не имеет проблем решения с чтением сразу в словарь, 
                //тут не нужно переписывать запись в xml, хотя запись таблицы в xml пишет все колонки и одинаковые значения, т.е. xml будет больше
                //чтение из xml в dataset может занимать по нескольку секунд для больших файлов
                //основную часть времени отнимал вывод информации о файлах!!
                //00.051
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionary(DBDataSet.ToDictionary(), forced);
                if (AppData.CurrentProject.DontLoadDuplicates)
                {
                    new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionaryParallellTables(DBDataSet.ToDictionary(), forced);
                }
                else
                {
                    new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionaryParallellTables(DBDataSet.ToDictionary2(), forced);
                }

                //многопоточный вариант предыдущего, но т.к. datatable is threadunsafe то возникают разные ошибки и повреждение внутреннего индекса таблицы, хоть это и быстрее, но после добавления lock разницы не видно
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionaryParallel(DBDataSet.DBDataSetToDBDictionary());


                //это медленнее первого варианта 
                //00.151
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionary2(DBDataSet.DBDataSetToDBDictionary());

                //считывание через словарь Чтение xml в словарь на текущий момент имеет проблемы
                //с невозможностью чтения закодированых в hex символов(решил как костыль через try catch) и пока не может читать сжатые xml
                //нужно постепенно доработать код, исправить проблемы и перейти полностью на этот наибыстрейший вариант
                //т.к. с ним и xml бд будет меньше размером
                //new FunctionsLoadTranslationDB().THLoadDBCompareFromDictionary(FunctionsDBFile.ReadXMLDBToDictionary(sPath));

            }
            catch
            {

            }

            ProgressInfo(false);
        }

        private void LoadTrasnlationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Load translation from selected DB
        /// </summary>
        /// <param name="forced">means load with current lines override even if they are not empty</param>
        internal async void LoadDBAs(bool forced = false)
        {
            //Do nothing if user will try to use Open menu before previous will be finished
            if (IsOpeningInProcess) return;

            IsOpeningInProcess = true;
            using (OpenFileDialog THFOpenBD = new OpenFileDialog())
            {
                THFOpenBD.Filter = "DB file|*.xml;*.cmx;*.cmz|XML-file|*.xml|Gzip compressed DB (*.cmx)|*.cmx|Deflate compressed DB (*.cmz)|*.cmz";

                THFOpenBD.InitialDirectory = FunctionsDBFile.GetProjectDBFolder();

                if (THFOpenBD.ShowDialog() == DialogResult.OK)
                {
                    if (THFOpenBD.FileName.Length > 0)
                    {
                        await Task.Run(() => LoadTranslationFromDB(THFOpenBD.FileName, false, forced)).ConfigureAwait(true);
                    }
                }
            }
            IsOpeningInProcess = false;
        }

        internal bool SavemenusNOTenabled = true;
        private async void THFileElementsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (!AppSettings.ProjectIsOpened) return;


            try
            {
                if (FileDataWasChanged && SavemenusNOTenabled)
                {
                    SavemenusNOTenabled = false;
                }

                int tableind = THFilesList.GetSelectedIndex();
                int rind = FunctionsTable.GetDGVSelectedRowIndexInDatatable(THFilesList.GetSelectedIndex(), e.RowIndex);

                if (rind > -1 && rind < AppData.CurrentProject.FilesContent.Tables[tableind].Rows.Count && (AppData.CurrentProject.FilesContent.Tables[tableind].Rows[rind][1] + string.Empty).Length > 0)
                {
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    //Thread trans = new Thread(new ParameterizedThreadStart((obj) => THAutoSetSameTranslationForSimular(tableind, rind, cind, false)));
                    //trans.Start();

                    await Task.Run(() => SetSameTranslationForSimular(tableind, rind, false)).ConfigureAwait(false);
                }

                //Запуск автосохранения
                Autosave();
            }
            catch
            {
            }
        }

        [Obsolete]
        internal bool IsTranslating;

        private void THTargetTextBox_Leave(object sender, EventArgs e)
        {
            //int sel = dataGridView1.CurrentRow.Index; //присвоить перевенной номер выбранной строки в таблице
            //if (THSourceRichTextBox.Text.Length == 0)
            //{
            //}
            //else//если текстовое поле 2 не пустое
            //{
            //    //не менять, если значение текстбокса не поменялось
            //    if (THTargetRichTextBox.Text != ProjectData.TargetTextBoxPreValue)
            //    {
            //        THFileElementsDataGridView.CurrentRow.Cells[THSettings.TranslationColumnName].Value = THTargetRichTextBox.Text;// Присвоить ячейке в ds.Tables[0] значение из TextBox2                   
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

        //bool THIsFixingCells;
        ///// <summary>
        ///// Исправления формата спецсимволов в заданной ячейке перевода
        ///// <br/>Для выбранных ячеек, таблицы или для всех значений задать:
        ///// <br/>method:
        ///// <br/>"a" - All
        ///// <br/>"t" - Table
        ///// <br/>"s" - Selected
        ///// <br/>..а также cind - индекс колонки, где ячейки перевода и tind - индекс таблицы, для вариантов Table и Selected
        ///// <br/>Для одной выбранной ячейки, когда, например, определенная обрабатывается в коде, <br/>задать tind, cind и rind, а также true для onselectedonly
        ///// </summary>
        ///// <param name="method"></param>
        ///// <param name="cind"></param>
        ///// <param name="tind"></param>
        ///// <param name="rind"></param>
        ///// <param name="selectedonly"></param>
        //private void THFixCells(string method, int cind, int tind, int rind = 0, bool forceApply = false)//cind - индекс столбца перевода, задан до старта потока
        //{
        //    //возвращать, если занято, когда исправление в процессе
        //    if (THIsFixingCells)
        //    {
        //        return;
        //    }
        //    //установить занятость при старте
        //    THIsFixingCells = true;

        //    FunctionsAutoOperations.THFixCells(method, cind, tind, rind, forceApply);

        //    //снять занятость по окончании
        //    THIsFixingCells = false;
        //}

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

            var ret = FunctionsAutoOperations.THExtractTextForTranslationSplit(input);

            //снять занятость по окончании
            THIsExtractingTextForTranslation = false;
            return ret;
        }

        private void CellFixesSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = new FixCells().Selected();
        }

        //private void SetOriginalToTranslation()
        //{
        //    int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount();
        //    if (THFileElementsDataGridViewSelectedCellsCount > 0)
        //    {
        //        try
        //        {
        //            int tableIndex = THFilesList.GetSelectedIndex();
        //            int cind = AppData.CurrentProject.FilesContent.Tables[tableIndex].Columns[THSettings.OriginalColumnName].Ordinal;// Колонка Original
        //            int cindTrans = AppData.CurrentProject.FilesContent.Tables[tableIndex].Columns[THSettings.TranslationColumnName].Ordinal;// Колонка Original
        //            int[] selectedRowIndexses = new int[THFileElementsDataGridViewSelectedCellsCount];
        //            for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
        //            {
        //                //координаты ячейки
        //                selectedRowIndexses[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(THFilesList.GetSelectedIndex(), THFileElementsDataGridView.SelectedCells[i].RowIndex);

        //            }
        //            foreach (var rind in selectedRowIndexses)
        //            {
        //                string origCellValue = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rind][cind] as string;
        //                string transCellValue = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rind][cindTrans] + string.Empty;
        //                if (transCellValue != origCellValue || transCellValue.Length == 0)
        //                {
        //                    AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows[rind][cindTrans] = origCellValue;
        //                }

        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        bool cellchanged;
        public async void SetSameTranslationForSimular(int InputTableIndex, int InputRowIndex, bool forcerun = true, bool forcevalue = false)
        {
            if (forcevalue || (AppSettings.AutotranslationForSimular && (cellchanged || forcerun))) //запуск только при изменении ячейки, чтобы не запускалось каждый раз. Переменная задается в событии изменения ячейки
            {
                await Task.Run(() => AutoSameForSimularUtils.Set(InputTableIndex, InputRowIndex, forcevalue)).ConfigureAwait(false);

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
                    if (!clickedRow.Cells[e.ColumnIndex].Selected && !clickedRow.Selected)//вот это модифицировано
                    {
                        THFileElementsDataGridView.CurrentCell = clickedRow.Cells[e.ColumnIndex];
                    }

                    if (!clickedRow.Cells[e.ColumnIndex].IsInEditMode)//не вызывать меню, когда ячейка в режиме редактирования
                    {
                        var mousePosition = THFileElementsDataGridView.PointToClient(Cursor.Position);

                        RowMenu.Show(THFileElementsDataGridView, mousePosition);
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
                    THFilesList.GetSelectedIndex(),
                    THFileElementsDataGridView.SelectedCells[0].RowIndex
                    );
            }
        }

        private void SetColumnSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        bool WriteDBFileIsBusy;
        string WriteDBFileLiteLastFileName = string.Empty;
        internal async void WriteDBFileLite(DataSet ds, string fileName)
        {
            if (fileName.Length == 0 || ds == null) return;

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

                AppData.Settings.THConfigINI.SetKey("Paths", "LastAutoSavePath", lastautosavepath);
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
                if (!AppSettings.IsTranslationHelperWasClosed && !THInfolabel.Enabled)
                {
                    THInfolabelEnabled = true;
                    _ = THInfolabel.Invoke((Action)(() => THInfolabel.Enabled = true));
                }

                if (!AppSettings.IsTranslationHelperWasClosed)
                {
                    _ = THInfolabel.Invoke((Action)(() => THInfolabel.Text = InfoText));
                }

                FunctionsThreading.WaitThreaded(1000);

                if (THInfolabelEnabled && !AppSettings.IsTranslationHelperWasClosed && THInfolabel.Enabled)
                {
                    _ = THInfolabel.Invoke((Action)(() => THInfolabel.Text = string.Empty));
                    _ = THInfolabel.Invoke((Action)(() => THInfolabel.Enabled = false));
                }
            }
            catch
            {
            }
        }

        internal bool InteruptTranslation;

        private void THMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppSettings.IsTranslationHelperWasClosed = true;
            AppSettings.InterruptTtanslation = true;
            InteruptTranslation = true;
            //THToolTip.Dispose();
            //ProjectData.THFilesElementsDataset.Dispose();
            //ProjectData.THFilesElementsDatasetInfo.Dispose();
            //ProjectData.THFilesElementsALLDataTable.Dispose();
            //Settings.Dispose();

            //global brushes with ordinary/selected colors
            //ListBoxItemForegroundBrushSelected.Dispose();
            //ListBoxItemForegroundBrush.Dispose();
            //ListBoxItemBackgroundBrushSelected.Dispose();
            //ListBoxItemBackgroundBrush1.Dispose();
            //ListBoxItemBackgroundBrush1Complete.Dispose();
            //ListBoxItemBackgroundBrush2.Dispose();
            //ListBoxItemBackgroundBrush2Complete.Dispose();

            FunctionsSave.WriteRPGMakerMVStats();
        }

        private void SetAsDatasourceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THFileElementsDataGridView.DataSource = AppData.CurrentProject.FilesContentAll;

            //смотрел тут но в данном случае пришел к тому что отображает все также только одну таблицу
            //https://social.msdn.microsoft.com/Forums/en-US/f63f612f-20be-4bad-a91c-474396941800/display-dataset-data-in-gridview-from-multiple-data-tables?forum=adodotnetdataset
            //if (THFilesElementsDataset.Relations.Contains("ALL"))
            //{

            //}
            //else
            //{
            //    DataRelation dr = new DataRelation("ALL",
            //         new DataColumn[] { THFilesElementsDataset.Tables[0].Columns[THSettings.OriginalColumnName], THFilesElementsDataset.Tables[0].Columns[THSettings.TranslationColumnName] },
            //         new DataColumn[] { THFilesElementsDataset.Tables[1].Columns[THSettings.OriginalColumnName], THFilesElementsDataset.Tables[1].Columns[THSettings.TranslationColumnName] },
            //         false
            //                                        );

            //    THFilesElementsDataset.Relations.Add(dr);
            //}

            //THFileElementsDataGridView.DataSource = THFilesElementsDataset.Relations["ALL"].ParentTable;
        }

        internal THfrmSearch search;

        private void THMainResetTableButton_Click(object sender, EventArgs e)
        {
            if (THFiltersDataGridView.Columns.Count > 0)
            {
                try
                {

                    int realRowIndex = -1;
                    string columnName = string.Empty;

                    if (THFilesList.GetSelectedIndex() == -1)
                    {
                        return;
                    }
                    int tableindex = THFilesList.GetSelectedIndex();
                    var cell = THFileElementsDataGridView.CurrentCell;

                    if (tableindex > -1 && cell != null)
                    {
                        columnName = THFileElementsDataGridView.Columns[cell.ColumnIndex].Name;
                        realRowIndex = FunctionsTable.GetDGVSelectedRowIndexInDatatable(tableindex, cell.RowIndex);
                    }

                    for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)
                    {
                        THFiltersDataGridView.Rows[0].Cells[c].Value = string.Empty;
                    }

                    var table = AppData.CurrentProject.FilesContent.Tables[tableindex];
                    table.DefaultView.RowFilter = string.Empty;
                    table.DefaultView.Sort = string.Empty;
                    THFileElementsDataGridView.Refresh();

                    if (realRowIndex > -1 && tableindex > -1 && columnName.Length > 0)
                    {
                        FunctionsTable.ShowSelectedRow(tableindex, columnName, realRowIndex);
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
            _ = MessageBox.Show("FOUND=\r\n" + o + "\r\n, matchCollection count=" + matchCollection.Count);
        }

        internal static bool DGVCellInEditMode
        {
            get => AppSettings.DGVCellInEditMode;
            set => AppSettings.DGVCellInEditMode = value;
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
            if (DGVCellInEditMode) return;

            //отключение действий для ячеек при входе
            ControlsSwitch();
            //https://stackoverflow.com/questions/12780961/disable-copy-and-paste-in-datagridview
            THFileElementsDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;

        }

        private void THSourceRichTextBox_MouseLeave(object sender, EventArgs e)
        {
            if (DGVCellInEditMode) return;

            //влючение действий для ячеек при выходе из режима редктирования
            //ControlsSwitch(true);
            THFileElementsDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
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
                    //CutToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.X;
                    //CopyCellValuesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
                    //PasteCellValuesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
                }
                else if (ControlsSwitchIsOn)
                {
                    ControlsSwitchIsOn = switchon;
                    //System.Media.SystemSounds.Hand.Play();
                    //CutToolStripMenuItem1.ShortcutKeys = Keys.None;
                    //CopyCellValuesToolStripMenuItem.ShortcutKeys = Keys.None;
                    //PasteCellValuesToolStripMenuItem.ShortcutKeys = Keys.None;
                }
            }
        }

        private void THFiltersDataGridView_MouseEnter(object sender, EventArgs e)
        {
            ControlsSwitch();
        }

        private void THFiltersDataGridView_MouseLeave(object sender, EventArgs e)
        {
            ControlsSwitch(true);
        }

        private void THSourceRichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            ControlsSwitch();
        }

        private void THFiltersDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ControlsSwitch();
        }

        private void THFileElementsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!AppSettings.ProjectIsOpened)
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

        private void TableCompleteInfoLabel_Click(object sender, EventArgs e)
        {
            FunctionsTable.ShowFirstRowWithEmptyTranslation();
        }

        private void THTargetRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!AppSettings.DGVCellInEditMode && (sender as RichTextBox).Focused && THFileElementsDataGridView.CurrentRow.Index > -1)
            {
                THFileElementsDataGridView.Rows[AppSettings.DGVSelectedRowIndex].Cells[THSettings.TranslationColumnName].Value = (sender as RichTextBox).Text;
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
                    //int i = AppSettings.DGVSelectedRowIndex;
                    //int r = AppSettings.DGVSelectedRowRealIndex;
                    if (SelectedRowRealIndex ==
                        (realrowindex = FunctionsTable.GetDGVSelectedRowIndexInDatatable(
                        AppSettings.THFilesListSelectedIndex,
                        rowindex = row.Index))
                        )
                    {
                        FunctionsTable.ShowSelectedRow(THFilesList.GetSelectedIndex(), AppSettings.DGVSelectedColumnIndex, rowindex);
                        AppSettings.DGVSelectedRowIndex = rowindex;
                        AppSettings.DGVSelectedRowRealIndex = realrowindex;
                        SelectedRowRealIndex = realrowindex;
                        break;
                    }
                }
            }
        }

        private void THInfolabel_Click(object sender, EventArgs e)
        {

        }

        private void THSourceRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!tlpTextLenPosInfo.Visible) tlpTextLenPosInfo.Visible = true;
        }

        private void THfrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (THFileElementsDataGridViewOriginalToTranslationHotkey == null) return;

            //THFileElementsDataGridViewOriginalToTranslationHotkey.Dispose();
            //THFileElementsDataGridViewOriginalToTranslationHotkey = null;
        }

        ///// <summary>
        ///// Byte search in byte array. One of methods from here: https://stackoverflow.com/a/41414219
        ///// </summary>
        ///// <param name="src"></param>
        ///// <param name="pattern"></param>
        ///// <returns></returns>
        //internal static int GetBytesPosition(byte[] src, byte[] pattern)
        //{
        //    int index = -1;

        //    for (int i = 0; i < src.Length; i++)
        //    {
        //        if (src[i] != pattern[0])
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            bool isContinoue = true;
        //            for (int j = 1; j < pattern.Length; j++)
        //            {
        //                if (src[++i] != pattern[j])
        //                {
        //                    isContinoue = true;
        //                    break;
        //                }
        //                if (j == pattern.Length - 1)
        //                {
        //                    isContinoue = false;
        //                }
        //            }
        //            if (!isContinoue)
        //            {
        //                index = i - (pattern.Length - 1);
        //                break;
        //            }
        //        }
        //    }
        //    return index;
        //}

        private void AddToCustomDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
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