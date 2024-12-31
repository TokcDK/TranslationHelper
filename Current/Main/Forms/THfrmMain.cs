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

        private void THFileElementsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //_ = this.Invoke((Action)(() => CellEnterActions(sender, e)));
        }

        private void CellEnterActions(object sender, DataGridViewCellEventArgs e)
        {
            //UpdateTextboxes(sender, e);
        }

        private void THTargetTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+Del function
            //https://stackoverflow.com/questions/18543198/why-cant-i-press-ctrla-or-ctrlbackspace-in-my-textbox
            if (!e.Control || e.KeyCode != Keys.Back)
            {
                return;
            }

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

        private void THFiltersDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!AppSettings.ProjectIsOpened) return;

            FunctionsUI.CheckFilterDGV();
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
            //await Task.Run(() => FunctionAutoSave.Autosave()).ConfigureAwait(true);// save on each change is killing system..       

            FunctionsUI.UpdateTranslationTextBoxValue(sender, e);
            FunctionsUI.CellChangedRegistration(e.ColumnIndex);
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
            FunctionsUI.ControlsSwitch();
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
            FunctionsUI.ControlsSwitch();

        }

        private void THTargetRichTextBox_MouseLeave(object sender, EventArgs e)
        {
            //влючение действий для ячеек при выходе из текстбокса
            //ControlsSwitch(true);
        }

        private void THFiltersDataGridView_MouseEnter(object sender, EventArgs e)
        {
            FunctionsUI.ControlsSwitch();
        }

        private void THFiltersDataGridView_MouseLeave(object sender, EventArgs e)
        {
            FunctionsUI.ControlsSwitch(true);
        }

        private void THSourceRichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            FunctionsUI.ControlsSwitch();
        }

        private void THFiltersDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FunctionsUI.ControlsSwitch();
        }

        private void THFileElementsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!AppSettings.ProjectIsOpened) return;
            if (FunctionsUI.ControlsSwitchActivated) return;

            FunctionsUI.ControlsSwitch(true);//не включалось копирование в ячейку, при копировании с гугла назад

            FunctionsUI.ShowNonEmptyRowsCount(TableCompleteInfoLabel);//Show how many rows have translation
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
            FunctionsUI.UpdateTextboxes();
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