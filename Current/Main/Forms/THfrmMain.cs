using System;
using System.Data;
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
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MainMenus.File;

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

            FunctionsHotkeys.BindShortCuts();

            AppData.SetSettings();

            FunctionsMenus.CreateMainMenus();

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


            MenuItemRecent.UpdateRecentFiles();
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

                FunctionsMenus.CreateFileRowMenus();

                BindTextBoxesOriginalTranslation();
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
                THFileElementsDataGridView.Columns[THSettings.OriginalColumnName].HeaderText = T._(THSettings.OriginalColumnName);//THMainDGVOriginalColumnName;
                THFileElementsDataGridView.Columns[THSettings.TranslationColumnName].HeaderText = T._(THSettings.TranslationColumnName);//THMainDGVTranslationColumnName;
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
            //_ = this.Invoke((Action)(() => CellEnterActions(sender, e)));
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
                var selected = AppData.Main.THFileElementsDataGridView.SelectedCells;

                BindTextBoxesOriginalTranslation();

                var tableIndex = AppSettings.THFilesListSelectedIndex = THFilesList.GetSelectedIndex();
                if (tableIndex == -1) return;

                var columnIndex = AppSettings.DGVSelectedColumnIndex = THFileElementsDataGridView.CurrentCell.ColumnIndex;
                if (columnIndex == -1) return;

                var rowIndex = AppSettings.DGVSelectedRowIndex = THFileElementsDataGridView.CurrentCell.RowIndex;
                if (rowIndex == -1) return;

                var realrowIndex = AppSettings.DGVSelectedRowRealIndex = FunctionsTable.GetRealRowIndex(tableIndex, rowIndex);
                if (realrowIndex == -1) return;

                UpdateRowInfo(tableIndex, columnIndex, realrowIndex);

                return;

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

                //gem furigana
                //https://github.com/helephant/Gem
                //var furigana = new Furigana(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                //THInfoTextBox.Text += furigana.Reading + "\r\n";
                //THInfoTextBox.Text += furigana.Expression + "\r\n";
                //THInfoTextBox.Text += furigana.Hiragana + "\r\n";
                //THInfoTextBox.Text += furigana.ReadingHtml + "\r\n";
            }
            catch { }
        }

        private void BindTextBoxesOriginalTranslation()
        {
            // this way binding on each selected row changed event, it prevents errors with threads of bound dataset to dgv and seems not causing slowdown
            var selected = AppData.Main.THFileElementsDataGridView.SelectedCells;
            if (selected.Count == 0) return;
            var rowIndex = selected[0].RowIndex;
            AppData.Main.THSourceRichTextBox.DataBindings.Clear();
            AppData.Main.THSourceRichTextBox.DataBindings.Add(new Binding("Text", AppData.Main.THFileElementsDataGridView[AppData.CurrentProject.OriginalColumnIndex, rowIndex], "Value", false));
            AppData.Main.THTargetRichTextBox.DataBindings.Clear();
            AppData.Main.THTargetRichTextBox.DataBindings.Add(new Binding("Text", AppData.Main.THFileElementsDataGridView[AppData.CurrentProject.TranslationColumnIndex, rowIndex], "Value", false));
        }

        private void UpdateRowInfo(int tableIndex, int columnIndex, int rowIndex)
        {
            string selectedCellValue;
            if ((selectedCellValue = THFileElementsDataGridView.Rows[rowIndex].Cells[columnIndex].Value + string.Empty).Length == 0)
            {
                return;
            }

            THInfoTextBox.Text = string.Empty;

            if (AppData.CurrentProject.FilesContentInfo != null && AppData.CurrentProject.FilesContentInfo.Tables.Count > tableIndex && AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows.Count > rowIndex)
            {
                THInfoTextBox.Text += T._("rowinfo:") + Environment.NewLine + AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows[rowIndex][0];
            }

            THInfoTextBox.Text += Environment.NewLine + T._("Selected bytes length") + ":" + " UTF8" + "=" + Encoding.UTF8.GetByteCount(selectedCellValue) + "/932" + "=" + Encoding.GetEncoding(932).GetByteCount(selectedCellValue);

            if (AppData.CurrentProject.Name == "RPG Maker MV")
            {
                THInfoTextBox.Text += Environment.NewLine + Environment.NewLine + T._("Several strings also can be in Plugins.js in 'www\\js' folder and referred plugins in plugins folder.");
            }
            THInfoTextBox.Text += Environment.NewLine + Environment.NewLine;


            THInfoTextBox.Text += FunctionsRomajiKana.GetLangsOfString(selectedCellValue, "all"); //Show all detected languages count info
                                                                                                  //--------Считывание значения ячейки в текстовое поле 1
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
            if (!AppSettings.ProjectIsOpened) return;

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

            int rowIdx = FunctionsTable.GetRealRowIndex(THFilesList.GetSelectedIndex(), e.RowIndex);//здесь получаю реальный индекс из Datatable
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

        private async void THFileElementsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (AppData.CurrentProject == null) return;
            if (e.ColumnIndex != AppData.CurrentProject.TranslationColumnIndex) return;

            // Get the new value of the cell
            DataGridViewCell cell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];
            object newValue = cell.Value;

            // Get the old value of the cell
            object oldValue = cell.Tag;

            // Compare the new value with the old value and return if new value is same
            if (!newValue.Equals(oldValue))
            {
                cell.Tag = newValue;
            }
            else return;

            if (!AppSettings.ProjectIsOpened) return;

            await Task.Run(() => new AutoSameForSimular().Rows()).ConfigureAwait(true);
            await Task.Run(() => FunctionAutoSave.Autosave()).ConfigureAwait(true);            

            UpdateTranslationTextBoxValue(sender, e);
            CellChangedRegistration(e.ColumnIndex);
        }

        private void CellChangedRegistration(int ColumnIndex = -1)
        {
            if (ColumnIndex > 0)
            {
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

        private async void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionsDBFile.UnLockDBLoad(false);
            await FunctionsDBFile.LoadDB();
            //Invoke((Action)(() => LoadTranslationToolStripMenuItem.Enabled = true));
        }

        private void LoadTrasnlationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void THFileElementsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
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
            FunctionsTable.PaintDigitInFrontOfRow(sender, e, this.Font);
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

        private void CellFixesSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = new FixCells().Rows();
        }

        private void THFileElementsDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            FunctionsTable.CellMouseDown(THFileElementsDataGridView, THFilesList, e, RowMenus);
        }

        /// <summary>
        /// remember last selected real rowindex
        /// used materials:https://stackoverflow.com/questions/4819573/selected-rows-when-sorting-datagridview-in-winform-application
        /// </summary>
        private void RememberLastCellSelection()
        {
            FunctionsTable.RememberLastCellSelection(THFilesList, THFileElementsDataGridView);
        }

        private void SetColumnSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public void IndicateSaveProcess(string infoText = "")
        {
            FunctionsSave.IndicateSaveProcess(infoText, THInfolabel);
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
            FunctionsTable.ResetTable(THFileElementsDataGridView, THFilesList, THFiltersDataGridView);
        }

        private void TESTRegexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionTest.Test();
        }

        internal static bool DGVCellInEditMode
        {
            get => AppSettings.DGVCellInEditMode;
            //set => AppSettings.DGVCellInEditMode = value;
        }

        private void THFileElementsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //DGVCellInEditMode = true;
            //отключение действий для ячеек при входе в режим редктирования
            //ControlsSwitch();
        }

        private void THFileElementsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //DGVCellInEditMode = false;
            //влючение действий для ячеек при выходе из режима редктирования
            //ControlsSwitch(true);
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
            if (!AppSettings.ProjectIsOpened) return;
            if (ControlsSwitchActivated) return;

            ControlsSwitch(true);//не включалось копирование в ячейку, при копировании с гугла назад

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
            //if (!AppSettings.DGVCellInEditMode && (sender as RichTextBox).Focused && THFileElementsDataGridView.CurrentRow.Index > -1)
            //{
            //    THFileElementsDataGridView.Rows[AppSettings.DGVSelectedRowIndex].Cells[THSettings.TranslationColumnName].Value = (sender as RichTextBox).Text;
            //}

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
            FunctionsTable.ReselectCellSelectedBeforeSorting(THFilesList, THFileElementsDataGridView);
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