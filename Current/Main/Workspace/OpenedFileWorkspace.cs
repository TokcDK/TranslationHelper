using System;
using System.Globalization;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Models;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// The controls of one opened file: its table grid, the original text and the translation text.
    /// <para>
    /// One instance is created per entry of <see cref="OpenedFilesData.OpenedFilesList"/> that is
    /// shown — per file, and per the "[ALL]" entry — and lives in that entry's tab. Because the
    /// controls belong to the entry rather than to the application, two open files can be scrolled,
    /// sorted and edited independently, and an edit can no longer land in whichever file happened to be
    /// shown last.
    /// </para>
    /// <para>
    /// It is built when its entry is selected rather than when the entry enters the list, so a project
    /// that has just been opened holds none of these and only the files the user has looked at have
    /// one. That is what keeps a project with thousands of files from being opened with thousands of
    /// grids.
    /// </para>
    /// <para>
    /// The class is a view: every handler forwards to the function that owns the behaviour and hands it
    /// this workspace, so the behaviour reads this entry's controls instead of a global one. No rule
    /// about what an edit means is written here.
    /// </para>
    /// </summary>
    internal partial class OpenedFileWorkspace : UserControl
    {
        /// <summary>
        /// The project this file belongs to. Set by <see cref="Initialize"/>.
        /// </summary>
        internal IProjectWorkspace Workspace { get; private set; }

        /// <summary>
        /// The entry this workspace presents, or null before <see cref="Initialize"/>.
        /// </summary>
        internal OpenedFileData File { get; private set; }

        internal OpenedFileWorkspace()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Bind the grid to <paramref name="file"/>'s table and remember where this workspace belongs.
        /// </summary>
        internal void Initialize(IProjectWorkspace workspace, OpenedFileData file)
        {
            Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            File = file ?? throw new ArgumentNullException(nameof(file));

            SourceRichTextBox.DetectUrls = false;

            // The grid is bound here rather than by the caller so that a workspace is usable the
            // moment it is built, and so that the binding follows the entry it was built for.
            ElementsDataGridView.DataSource = file.Table;
        }

        /// <summary>
        /// Point the grid at the entry's table again. Used for the "[ALL]" entry, whose table is
        /// rebuilt in place from the files while it is displayed.
        /// </summary>
        internal void RefreshBinding()
        {
            if (File == null) return;

            ElementsDataGridView.DataSource = File.Table;
        }

        /// <summary>
        /// Number of rows the entry's table holds, which is what the grid should be showing.
        /// </summary>
        internal int RowsCount => File?.Table?.Rows.Count ?? 0;

        #region Grid handlers

        private void ElementsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Reserved: the work used to happen in the handler the form no longer has.
        }

        private void ElementsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Reserved: editing does not change what the rest of the window offers.
        }

        private void ElementsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Reserved: see CellBeginEdit.
        }

        private void ElementsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // Reserved: validation is done by the cell type.
        }

        private void ElementsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!AppSettings.ProjectIsOpened) return;
            if (FunctionsUI.ControlsSwitchActivated) return;

            // Re-enables copying into a cell, which pasting from a browser had turned off.
            FunctionsUI.ControlsSwitch(true);

            FunctionsUI.ShowNonEmptyRowsCount(Workspace.Project, Workspace.CompletionLabel);
        }

        private void ElementsDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            FunctionsUI.THFileElementsDataGridView_CellMouseClick(sender, e);
        }

        private void ElementsDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            FunctionsTable.CellMouseDown(this, e, AppData.Main.RowMenus);
        }

        private async void ElementsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            await FunctionsUI.THFileElementsDataGridView_CellValueChangedAsync(this, e);
        }

        private void ElementsDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            FunctionsUI.PaintDigitInFrontOfRow(this, e);
        }

        private void ElementsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            FunctionsUI.UpdateTextboxes(Workspace);
        }

        private void ElementsDataGridView_Sorted(object sender, EventArgs e)
        {
            FunctionsTable.ReselectCellSelectedBeforeSorting(Workspace);
        }

        #endregion

        #region Source text handlers

        private void SourceRichTextBox_MouseEnter(object sender, EventArgs e)
        {
            if (FormMain.DGVCellInEditMode) return;

            FunctionsUI.ControlsSwitch();

            // A cell copy would otherwise fight the text the user is about to select in the box.
            ElementsDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
        }

        private void SourceRichTextBox_MouseLeave(object sender, EventArgs e)
        {
            if (FormMain.DGVCellInEditMode) return;

            ElementsDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        }

        private void SourceRichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            FunctionsUI.ControlsSwitch();
        }

        private void SourceRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!tlpTextLenPosInfo.Visible) tlpTextLenPosInfo.Visible = true;
        }

        #endregion

        #region Target text handlers

        private void TargetRichTextBox_MouseEnter(object sender, EventArgs e)
        {
            FunctionsUI.ControlsSwitch();
        }

        private void TargetRichTextBox_MouseLeave(object sender, EventArgs e)
        {
            // Reserved: copying is re-enabled when the grid is entered again.
        }

        private void TargetRichTextBox_TextChanged(object sender, EventArgs e)
        {
            TranslationLongestLineLenghtLabel.Text =
                FunctionsString.GetLongestLineLength((sender as RichTextBox).Text).ToString(CultureInfo.InvariantCulture);

            if (!tlpTextLenPosInfo.Visible) tlpTextLenPosInfo.Visible = true;
        }

        private void TargetRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            var box = sender as RichTextBox;

            TargetTextBoxLinePositionLabelData.Text = box.CurrentCharacterPosition().X.ToString(CultureInfo.InvariantCulture);
            TargetTextBoxColumnPositionLabelData.Text = box.CurrentCharacterPosition().Y.ToString(CultureInfo.InvariantCulture);
            TranslationLongestLineLenghtLabel.Text = box.CurrentSelectedTextLength().ToString(CultureInfo.InvariantCulture);

            if (!tlpTextLenPosInfo.Visible) tlpTextLenPosInfo.Visible = true;
        }

        private void TargetRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            FunctionsUI.THTargetTextBox_KeyDown(this, e);
        }

        private void TargetRichTextBox_Leave(object sender, EventArgs e)
        {
            FunctionsUI.THTargetTextBox_Leave(sender, e);
        }

        #endregion
    }
}
