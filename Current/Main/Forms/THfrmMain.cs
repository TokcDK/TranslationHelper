using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;
using System;
using System.Globalization;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;

namespace TranslationHelper
{
    public partial class FormMain : Form
    {
        internal string extractedpatchpath = string.Empty;

        internal string FVariant = string.Empty;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FormMain()
        {
            InitializeComponent();

            AppData.Init(this);

            FunctionsUI.Init(this);
        }

        private void SetupLogging()
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            var rtbTarget = new RichTextBoxTarget()
            {
                Name = "ui",
                ControlName = this.rtbLog.Name,
                FormName = this.Name,
                MaxLines = 100,
                AutoScroll = true,
                Layout = "${longdate} (${level:uppercase=true}): ${message}"
            };

            var fileTarget = new FileTarget("file")
            {
                FileName = "log.txt",
                MaxArchiveDays = 10,
                Layout = "${longdate}: (${level}) ${message}"
            };

            config.AddTarget("ui", rtbTarget);
            config.AddTarget("file", fileTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, "ui");
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, "file");

            LogManager.Configuration = config;
        }

        private void THMain_Load(object sender, EventArgs e)
        {
            SetupLogging();
            FunctionsUI.THMain_Load();
            for (int i = 0; i < 10; i++)
            {
                Logger.Info(T._($"Application started {i}"));
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
            FunctionsUI.THTargetTextBox_KeyDown(sender, e);
        }

        private void THFiltersDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            FunctionsUI.CheckFilterDGV();
        }

        //http://qaru.site/questions/180337/show-row-number-in-row-header-of-a-datagridview
        private void THFileElementsDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            FunctionsUI.PaintDigitInFrontOfRow(sender, e);
        }

        //Пример виртуального режима
        //http://www.cyberforum.ru/post9306711.html

        private async void THFileElementsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            await FunctionsUI.THFileElementsDataGridView_CellValueChangedAsync(sender, e);
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
            FunctionsUI.THTargetTextBox_Leave(sender, e);
        }

        private void THFiltersDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            FunctionsTable.PaintDigitInFrontOfRow(sender, e, this.Font);
        }

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

        private void THMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            FunctionsUI.THMain_FormClosing(sender, e);
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
            FunctionsUI.THFileElementsDataGridView_CellMouseClick(sender, e);
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

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
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