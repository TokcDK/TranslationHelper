using NLog;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects;
using TranslationHelper.Workspace;

namespace TranslationHelper.Functions
{
    /// <summary>
    /// What the window does with one project's files: which entry is shown, what the text boxes hold,
    /// what the grid looks like, and what an edit to a cell means.
    /// <para>
    /// Every member that touches a control takes the workspace it belongs to. That is what makes the
    /// same function work for whichever project the user is looking at: it reads the controls of the
    /// workspace it was given rather than the controls of "the" project, so two open projects cannot
    /// be confused with one another.
    /// </para>
    /// </summary>
    internal class FunctionsUI
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Show how much of <paramref name="project"/> is translated.
        /// </summary>
        internal static void ShowNonEmptyRowsCount(ProjectBase project, Label tableCompleteInfoLabel)
        {
            if (tableCompleteInfoLabel == null) return;

            int RowsCount = FunctionsTable.GetDatasetRowsCount(project?.FilesContent);
            if (RowsCount == 0)
            {
                tableCompleteInfoLabel.Visible = false;
            }
            else
            {
                tableCompleteInfoLabel.Visible = true;
                tableCompleteInfoLabel.Text = FunctionsTable.GetDatasetNonEmptyRowsCount(project.FilesContent) + "/" + RowsCount;
            }
        }

        internal static bool ControlsSwitchIsOn = true;
        internal static bool ControlsSwitchActivated;
        internal static void ControlsSwitch(bool switchon = false)
        {
            if (ControlsSwitchActivated)
            {
                if (switchon && !ControlsSwitchIsOn)
                {
                    ControlsSwitchIsOn = switchon;
                }
                else if (ControlsSwitchIsOn)
                {
                    ControlsSwitchIsOn = switchon;
                }
            }
        }

        /// <summary>
        /// Show the selected row of <paramref name="workspace"/> in its text boxes and info box.
        /// </summary>
        internal static void UpdateTextboxes(IProjectWorkspace workspace)
        {
            if (!(workspace?.ActiveFileWorkspace is OpenedFileWorkspace fileWorkspace)) return;

            try
            {
                var grid = fileWorkspace.ElementsDataGridView;
                if (grid.CurrentCell == null) return;

                BindTextBoxesOriginalTranslation(fileWorkspace);

                var listIndex = AppSettings.THFilesListSelectedIndex = workspace.FilesList?.GetSelectedIndex() ?? -1;
                if (listIndex == -1) return;

                var columnIndex = AppSettings.DGVSelectedColumnIndex = grid.CurrentCell.ColumnIndex;
                if (columnIndex == -1) return;

                var gridRowIndex = AppSettings.DGVSelectedRowIndex = grid.CurrentCell.RowIndex;
                if (gridRowIndex == -1) return;

                var realrowIndex = AppSettings.DGVSelectedRowRealIndex =
                    FunctionsTable.GetRealRowIndex(workspace, listIndex, gridRowIndex);
                if (realrowIndex == -1) return;

                // The row info describes the file the row came from, which for the [ALL] entry is not
                // the displayed table.
                if (!workspace.Project.FilesListContent.TryResolveRow(listIndex, realrowIndex, out var tableIndex, out var infoRowIndex)) return;

                UpdateRowInfo(fileWorkspace, tableIndex, columnIndex, gridRowIndex, infoRowIndex);
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to update textboxes. Error: {0}", ex);
            }
        }

        /// <summary>
        /// Point the source and target boxes of one file's workspace at the selected row of its grid.
        /// </summary>
        internal static void BindTextBoxesOriginalTranslation(OpenedFileWorkspace workspace)
        {
            if (workspace == null) return;

            var grid = workspace.ElementsDataGridView;
            var project = workspace.Workspace?.Project;
            if (project == null) return;

            // this way binding on each selected row changed event, it prevents errors with threads of bound dataset to dgv and seems not causing slowdown
            var selected = grid.SelectedCells;
            if (selected.Count == 0) return;

            var rowIndex = selected[0].RowIndex;

            workspace.SourceRichTextBox.DataBindings.Clear();
            workspace.SourceRichTextBox.DataBindings.Add(new Binding("Text", grid[project.OriginalColumnIndex, rowIndex], "Value", false));

            workspace.TargetRichTextBox.DataBindings.Clear();
            workspace.TargetRichTextBox.DataBindings.Add(new Binding("Text", grid[project.TranslationColumnIndex, rowIndex], "Value", false));
        }

        /// <summary>
        /// Show the info of the selected cell in the info box of <paramref name="workspace"/>.
        /// </summary>
        /// <param name="workspace">The file the selected cell belongs to.</param>
        /// <param name="tableIndex">
        /// Index in <see cref="ProjectBase.FilesContentInfo"/> of the file the selected row came from.
        /// </param>
        /// <param name="columnIndex">Column of the selected cell in the work table grid.</param>
        /// <param name="gridRowIndex">Row of the selected cell in the work table grid.</param>
        /// <param name="infoRowIndex">
        /// Row of the cell in the file's own table. It differs from <paramref name="gridRowIndex"/>
        /// when the grid shows the table sorted or filtered, and for the "[ALL]" entry it is always
        /// resolved from the file the row was copied from.
        /// </param>
        private static void UpdateRowInfo(OpenedFileWorkspace workspace, int tableIndex, int columnIndex, int gridRowIndex, int infoRowIndex)
        {
            var grid = workspace.ElementsDataGridView;
            var project = workspace.Workspace?.Project;
            if (project == null) return;

            string selectedCellValue;
            if ((selectedCellValue = grid.Rows[gridRowIndex].Cells[columnIndex].Value + string.Empty).Length == 0)
            {
                return;
            }

            workspace.THInfoTextBox.Text = string.Empty;

            if (project.FilesContentInfo != null
                && tableIndex >= 0
                && project.FilesContentInfo.Tables.Count > tableIndex
                && project.FilesContentInfo.Tables[tableIndex].Rows.Count > infoRowIndex)
            {
                workspace.THInfoTextBox.Text += T._("rowinfo:") + Environment.NewLine + project.FilesContentInfo.Tables[tableIndex].Rows[infoRowIndex][0];
            }

            workspace.THInfoTextBox.Text += Environment.NewLine + T._("Selected bytes length") + ":" + " UTF8" + "=" + Encoding.UTF8.GetByteCount(selectedCellValue) + "/932" + "=" + Encoding.GetEncoding(932).GetByteCount(selectedCellValue);

            if (project.Name == "RPG Maker MV")
            {
                workspace.THInfoTextBox.Text += Environment.NewLine + Environment.NewLine + T._("Several strings also can be in Plugins.js in 'www\\js' folder and referred plugins in plugins folder.");
            }
            workspace.THInfoTextBox.Text += Environment.NewLine + Environment.NewLine;

            workspace.THInfoTextBox.Text += FunctionsRomajiKana.GetLangsOfString(selectedCellValue, "all"); //Show all detected languages count info
        }

        internal static volatile bool SaveInAction;
        internal static bool FileDataWasChanged;

        internal static void CellChangedRegistration(int ColumnIndex = -1)
        {
            if (ColumnIndex > 0)
            {
                FileDataWasChanged = true;
            }
        }

        /// <summary>
        /// Copy the edited cell into the target box of the file it belongs to.
        /// </summary>
        internal static void UpdateTranslationTextBoxValue(OpenedFileWorkspace workspace, DataGridViewCellEventArgs e)
        {
            if (workspace == null) return;
            if (!AppSettings.DGVCellInEditMode) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = workspace.ElementsDataGridView;
            workspace.TargetRichTextBox.Text = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + string.Empty;
        }

        private static bool THFilesListBox_MouseClickBusy;

        /// <summary>
        /// The entry of <paramref name="workspace"/> that is shown has changed: bring the window up to
        /// date with it.
        /// <para>
        /// The grid is not rebound here. Each entry owns its own controls now, so showing another entry
        /// means showing another tab whose grid is already bound to that entry's table.
        /// </para>
        /// </summary>
        internal static void ActionsOnTHFIlesListElementSelected(IProjectWorkspace workspace)
        {
            if (workspace == null) return;
            if (THFilesListBox_MouseClickBusy) return;
            if (workspace.FilesList == null || workspace.FilesList.GetSelectedIndex() == -1) return;

            THFilesListBox_MouseClickBusy = true;

            try
            {
                ShowNonEmptyRowsCount(workspace.Project, workspace.CompletionLabel);

                UpdateTextboxes(workspace);

                FunctionsMenus.CreateFileRowMenus();

                BindTextBoxesOriginalTranslation(workspace.ActiveFileWorkspace);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to show the selected files list element content");
            }

            THFilesListBox_MouseClickBusy = false;
        }

        /// <summary>
        /// Get one file's workspace ready to be worked in: hide the columns that are not the original
        /// or the translation, name the two that are, and let the text boxes be used.
        /// </summary>
        internal static void PrepareElementsGrid(OpenedFileWorkspace workspace)
        {
            if (workspace == null) return;

            var grid = workspace.ElementsDataGridView;

            if (grid.Columns.Count > 2)
            {
                foreach (DataGridViewColumn Column in grid.Columns)
                {
                    if (Column.Name != THSettings.TranslationColumnName && Column.Name != THSettings.OriginalColumnName)
                    {
                        Column.Visible = false;
                    }
                }
            }

            ControlsSwitchActivated = true;

            if (grid.Columns.Count <= 1) return;

            grid.Columns[THSettings.OriginalColumnName].HeaderText = T._(THSettings.OriginalColumnName);
            grid.Columns[THSettings.TranslationColumnName].HeaderText = T._(THSettings.TranslationColumnName);
            grid.Columns[THSettings.OriginalColumnName].ReadOnly = true;
            workspace.SourceRichTextBox.Enabled = true;

            // The target box is held read-only until a file is selected, rather than disabled. A
            // disabled rich text box paints the system's light background whatever colour it is
            // given, which leaves a white block on a dark window; a read-only one keeps the
            // colour. Measured, and it is the same reason the source box above is read-only.
            workspace.TargetRichTextBox.ReadOnly = false;

            SetDoubleBufferedProperty(grid, true);
        }

        /// <summary>
        /// Bind <paramref name="DT"/> to the grid of the file <paramref name="workspace"/> shows.
        /// </summary>
        /// <param name="workspace">The project whose grid has to show the table.</param>
        /// <param name="DT">The table to show.</param>
        internal static void BindToDataTableGridView(IProjectWorkspace workspace, DataTable DT)
        {
            var grid = workspace?.ActiveFileWorkspace?.ElementsDataGridView;
            if (grid == null || DT == null) return;

            try
            {
                grid.DataSource = DT;

                //во время прокрутки DGV чернела полоса прокрутки и в результате было получено исключение
                //добавил это для возможного фикса
                //https://fooobar.com/questions/1404812/datagridview-scrollbar-throwing-argumentoutofrange-exception
                grid.PerformLayout();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to bind the table to the files elements grid");
            }
        }

        /// <summary>
        /// Draw the row number in front of a row of <paramref name="workspace"/>'s grid.
        /// </summary>
        internal static void PaintDigitInFrontOfRow(OpenedFileWorkspace workspace, DataGridViewRowPostPaintEventArgs e)
        {
            if (workspace == null) return;
            if (!AppSettings.ProjectIsOpened) return;

            var filesList = workspace.Workspace?.FilesList;
            if (filesList == null || filesList.GetSelectedIndex() == -1) return;

            var grid = workspace.ElementsDataGridView;

            int rowIdx = FunctionsTable.GetRealRowIndex(workspace.Workspace, filesList.GetSelectedIndex(), e.RowIndex);

            //GetRealRowIndex returns -1 for an unresolvable row; the old test then passed and painted "0".
            if (rowIdx < 0 || grid.Rows.Count <= rowIdx) return;

            using (StringFormat centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString((rowIdx + 1) + string.Empty, AppData.Main.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            }
        }

        /// <summary>
        /// A translation cell of <paramref name="workspace"/>'s grid was edited: keep the project's
        /// translations consistent with it.
        /// </summary>
        internal static async Task THFileElementsDataGridView_CellValueChangedAsync(OpenedFileWorkspace workspace, DataGridViewCellEventArgs e)
        {
            var project = workspace?.Workspace?.Project;
            if (project == null) return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.ColumnIndex != project.TranslationColumnIndex) return;

            var dgv = workspace.ElementsDataGridView;

            // Get the new value of the cell
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            var newValue = cell.Value;

            // Get the old value of the cell
            var oldValue = cell.Tag;

            // Compare the new value with the old value and return if new value is same
            if (!string.Equals(newValue, oldValue) && !string.IsNullOrEmpty(newValue + ""))
            {
                cell.Tag = newValue;
            }
            else return;

            if (!AppSettings.ProjectIsOpened) return;

            // Not while the project is still opening and not while its translation database is being
            // read: during a load every row that is written raises this event, and starting the
            // operation on each of them would spread translations read from a database that is only
            // half loaded. The operation checks the same state again, so a caller that does not ask
            // is still safe; asking here is what keeps a run's hooks and log out of a load.
            if (!ProjectReadiness.IsReady) return;

            await new AutoSameForSimular(project, workspace.Workspace).Rows().ConfigureAwait(true);

            UpdateTranslationTextBoxValue(workspace, e);
            CellChangedRegistration(e.ColumnIndex);
        }

        /// <summary>
        /// Ctrl+Del in a file's target box: delete the word before the caret.
        /// </summary>
        internal static void THTargetTextBox_KeyDown(OpenedFileWorkspace workspace, KeyEventArgs e)
        {
            if (workspace == null) return;

            //Ctrl+Del function
            //https://stackoverflow.com/questions/18543198/why-cant-i-press-ctrla-or-ctrlbackspace-in-my-textbox
            if (!e.Control || e.KeyCode != Keys.Back)
            {
                return;
            }

            var target = workspace.TargetRichTextBox;

            e.SuppressKeyPress = true;
            int selStart = target.SelectionStart;
            while (selStart > 0 && target.Text.Substring(selStart - 1, 1) == " ")
            {
                selStart--;
            }
            int prevSpacePos = -1;
            if (selStart != 0)
            {
                prevSpacePos = target.Text.LastIndexOf(' ', selStart - 1);
            }
            target.Select(prevSpacePos + 1, target.SelectionStart - prevSpacePos - 1);
            target.SelectedText = string.Empty;
        }

        /// <summary>
        /// Give the label that shows how much of a project is translated its tooltip. Called once per
        /// project, because the label belongs to the project's panel.
        /// </summary>
        internal static void SetTooltips(Label completionLabel)
        {
            if (completionLabel == null) return;

            //http://qaru.site/questions/47162/c-how-do-i-add-a-tooltip-to-a-control
            var toolTip = new ToolTip
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

            toolTip.SetToolTip(completionLabel, T._("Shows overal number of completed lines.\nClick to show first untranslated."));
        }

        internal static volatile bool IsOpeningInProcess;

        /// <summary>
        /// Sets the protected <c>DoubleBuffered</c> property of <paramref name="control"/>.
        /// </summary>
        internal static void SetDoubleBufferedProperty(Control control, bool value)
        {
            if (control == null) return;

            // Double buffering can make DGV slow in remote desktop
            if (SystemInformation.TerminalServerSession) return;

            PropertyInfo pi = control.GetType().GetProperty("DoubleBuffered",
              BindingFlags.Instance | BindingFlags.NonPublic);

            //Not every control type declares it, so a null property is expected and must be skipped.
            if (pi == null) return;

            pi.SetValue(control, value, null);
        }

        internal static string THTranslationCachePath
        {
            get => AppSettings.THTranslationCachePath;
            set => AppSettings.THTranslationCachePath = value;
        }

        internal static void Init(FormMain formMain)
        {
            AppSettings.ApplicationStartupPath = Application.StartupPath;
            AppSettings.ApplicationProductName = Application.ProductName;
            AppSettings.NewLine = Environment.NewLine;

            FunctionsHotkeys.BindShortCuts();

            AppData.InitSettings();

            FunctionsMenus.CreateMainMenus();

            THTranslationCachePath = THSettings.THTranslationCacheFilePath;

            if (File.Exists(THSettings.THLogPath) && new FileInfo(THSettings.THLogPath).Length > 1000000)
            {
                File.Delete(THSettings.THLogPath);
            }

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        internal static void THTargetTextBox_Leave(object sender, EventArgs e)
        {
            //The target box writes through the grid binding, so leaving it needs no commit step.
        }

        internal static void THMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppSettings.IsTranslationHelperWasClosed = true;
            AppSettings.InterruptTtanslation = true;
            InteruptTranslation = true;

            FunctionsSave.WriteRPGMakerMVStats();
        }

        internal static void THFileElementsDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Reserved: the index of the selected row in a filtered grid was never needed here.
        }

        internal static void THMain_Load()
        {
            //Hide the workspace until a project is opened.
            AppData.Main.frmMainPanel.Visible = false;

            MenuItemRecent.UpdateRecentFiles();
        }

        internal static bool InteruptTranslation;
    }
}
