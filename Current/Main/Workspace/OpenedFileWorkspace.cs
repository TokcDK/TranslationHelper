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
    /// The controls one project works in: the grid of the entry being worked on, the original text and
    /// the translation text.
    /// <para>
    /// One instance belongs to one project and lives in that project's panel. There is no tab per file
    /// and no grid per file: <see cref="Bind"/> repoints this workspace at whichever entry of
    /// <see cref="OpenedFilesData.OpenedFilesList"/> is selected — a file, or the "[ALL]" entry, which
    /// presents every file at once and is bound exactly like one.
    /// </para>
    /// <para>
    /// Because the controls belong to the project rather than to the application, two open projects can
    /// be scrolled, sorted and edited independently, and an edit can no longer land in whichever project
    /// happened to be shown last.
    /// </para>
    /// <para>
    /// The class is a view: every handler forwards to the function that owns the behaviour and hands it
    /// this workspace, so the behaviour reads this project's controls instead of a global one. No rule
    /// about what an edit means is written here.
    /// </para>
    /// </summary>
    internal partial class OpenedFileWorkspace : UserControl
    {
        /// <summary>
        /// The project this workspace belongs to. Set by <see cref="Initialize"/>.
        /// </summary>
        internal IProjectWorkspace Workspace { get; private set; }

        /// <summary>
        /// The entry this workspace is showing, or null while nothing is selected.
        /// <para>
        /// It is null rather than "the first entry" for the whole time between a project being opened
        /// and the user picking something from the files list, and the controls are left empty for that
        /// time: nothing has been asked for yet.
        /// </para>
        /// </summary>
        internal OpenedFileData File { get; private set; }

        internal OpenedFileWorkspace()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Remember where this workspace belongs. The grid stays unbound until
        /// <see cref="Bind"/> says what to show.
        /// </summary>
        internal void Initialize(IProjectWorkspace workspace)
        {
            Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));

            SourceRichTextBox.DetectUrls = false;

            Bind(null);
        }

        /// <summary>
        /// Show <paramref name="file"/>: bind its table to the grid and get the grid ready to be worked
        /// in, or empty the controls when it is null.
        /// <para>
        /// The grid is prepared here rather than once per entry, because the columns of a grid come from
        /// the table it is bound to and every entry has its own table: a grid that is repointed needs
        /// its columns hidden, named and made read-only again. The order matters — the columns exist
        /// only after the table is bound.
        /// </para>
        /// </summary>
        internal void Bind(OpenedFileData file)
        {
            File = file;

            ElementsDataGridView.DataSource = file?.Table;

            if (file == null)
            {
                // Nothing is shown, so the target box is held read-only: there is no row to write to,
                // and a read-only box keeps the theme's colour where a disabled one would paint the
                // system's light background.
                TargetRichTextBox.ReadOnly = true;
                return;
            }

            FunctionsUI.PrepareElementsGrid(this);
        }

        /// <summary>
        /// Point the grid at the entry's table again. Used where the table's contents were replaced
        /// rather than another entry selected — the "[ALL]" entry, whose table is rebuilt in place from
        /// the files while it is displayed.
        /// </summary>
        internal void RefreshBinding()
        {
            Bind(File);
        }

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
