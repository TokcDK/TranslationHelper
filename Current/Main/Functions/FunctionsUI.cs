﻿using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    internal class FunctionsUI
    {
        internal static void ShowNonEmptyRowsCount(System.Windows.Forms.Label tableCompleteInfoLabel)
        {
            int RowsCount = FunctionsTable.GetDatasetRowsCount(AppData.CurrentProject.FilesContent);
            if (RowsCount == 0)
            {
                tableCompleteInfoLabel.Visible = false;
            }
            else
            {
                tableCompleteInfoLabel.Visible = true;
                tableCompleteInfoLabel.Text = FunctionsTable.GetDatasetNonEmptyRowsCount(AppData.CurrentProject.FilesContent) + "/" + RowsCount;
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

        internal static void UpdateTextboxes()
        {
            try
            {
                if (AppData.Main.THFileElementsDataGridView.CurrentCell == null) return;
                var selected = AppData.Main.THFileElementsDataGridView.SelectedCells;

                BindTextBoxesOriginalTranslation();

                var tableIndex = AppSettings.THFilesListSelectedIndex = AppData.Main.THFilesList.GetSelectedIndex();
                if (tableIndex == -1) return;

                var columnIndex = AppSettings.DGVSelectedColumnIndex = AppData.Main.THFileElementsDataGridView.CurrentCell.ColumnIndex;
                if (columnIndex == -1) return;

                var rowIndex = AppSettings.DGVSelectedRowIndex = AppData.Main.THFileElementsDataGridView.CurrentCell.RowIndex;
                if (rowIndex == -1) return;

                var realrowIndex = AppSettings.DGVSelectedRowRealIndex = FunctionsTable.GetRealRowIndex(tableIndex, rowIndex);
                if (realrowIndex == -1) return;

                UpdateRowInfo(tableIndex, columnIndex, realrowIndex);

                return;

                if (AppData.Main.THFileElementsDataGridView.DataSource == null || rowIndex == -1 || tableIndex == -1)
                {
                    AppData.Main.THSourceRichTextBox.Clear();
                    AppData.Main.THTargetRichTextBox.Clear();
                    return;
                }

                //Считывание значения ячейки в текстовое поле 1, вариант 2, для DataSet, ds.Tables[0]
                //Проверка на размер индексов, для избежания ошибки при попытке сортировки " должен быть положительным числом и его размер не должен превышать размер коллекции"
                if (!AppData.Main.THSourceRichTextBox.Enabled
                    //&& THFileElementsDataGridView.CurrentCell != null
                    || AppData.Main.THFileElementsDataGridView.Rows.Count == 0
                    //|| rowIndex == -1
                    || columnIndex == -1)
                {
                    return;
                }

                AppData.Main.THTargetRichTextBox.Clear();

                if (string.IsNullOrEmpty(AppData.Main.THFileElementsDataGridView.Rows[AppData.Main.THFileElementsDataGridView.CurrentCell.RowIndex].Cells[THSettings.OriginalColumnName].Value + string.Empty))
                {
                    AppData.Main.THSourceRichTextBox.Clear();
                }
                else//проверить, не пуста ли ячейка, иначе была бы ошибка //THStrDGTranslationColumnName ошибка при попытке сортировки по столбцу
                {
                    //wrap words fix: https://stackoverflow.com/questions/1751371/how-to-use-n-in-a-textbox
                    AppData.Main.THSourceRichTextBox.Text = (AppData.Main.THFileElementsDataGridView.Rows[rowIndex].Cells[THSettings.OriginalColumnName].Value + string.Empty);
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
                if (string.IsNullOrEmpty(TranslationCellValue = AppData.Main.THFileElementsDataGridView.Rows[rowIndex].Cells[THSettings.TranslationColumnName].Value + string.Empty))
                {
                    AppData.Main.THTargetRichTextBox.Clear();
                }
                else//проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                {
                    //запоминание последнего значения ячейки перед считыванием в THTargetRichTextBox,
                    //для предотвращения записи значения обратно в ячейку, если она была изменена до изменения текстбокса
                    AppData.TargetTextBoxPreValue = TranslationCellValue;

                    //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                    AppData.Main.THTargetRichTextBox.Text = AppData.TargetTextBoxPreValue;

                    FormatTextBox();

                    //THTargetRichTextBox.Select(AppSettings.THOptionLineCharLimit+1, THTargetRichTextBox.Text.Length);
                    //THTargetRichTextBox.SelectionColor = Color.Red;

                    AppData.Main.TranslationLongestLineLenghtLabel.Text = FunctionsString.GetLongestLineLength(TranslationCellValue.ToString(CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                    AppData.Main.TargetTextBoxLinePositionLabelData.Text = string.Empty;
                }

                AppData.Main.THInfoTextBox.Text = string.Empty;

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

        internal static void BindTextBoxesOriginalTranslation()
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

        private static void UpdateRowInfo(int tableIndex, int columnIndex, int rowIndex)
        {
            string selectedCellValue;
            if ((selectedCellValue = AppData.Main.THFileElementsDataGridView.Rows[rowIndex].Cells[columnIndex].Value + string.Empty).Length == 0)
            {
                return;
            }

            AppData.Main.THInfoTextBox.Text = string.Empty;

            if (AppData.CurrentProject.FilesContentInfo != null && AppData.CurrentProject.FilesContentInfo.Tables.Count > tableIndex && AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows.Count > rowIndex)
            {
                AppData.Main.THInfoTextBox.Text += T._("rowinfo:") + Environment.NewLine + AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows[rowIndex][0];
            }

            AppData.Main.THInfoTextBox.Text += Environment.NewLine + T._("Selected bytes length") + ":" + " UTF8" + "=" + Encoding.UTF8.GetByteCount(selectedCellValue) + "/932" + "=" + Encoding.GetEncoding(932).GetByteCount(selectedCellValue);

            if (AppData.CurrentProject.Name == "RPG Maker MV")
            {
                AppData.Main.THInfoTextBox.Text += Environment.NewLine + Environment.NewLine + T._("Several strings also can be in Plugins.js in 'www\\js' folder and referred plugins in plugins folder.");
            }
            AppData.Main.THInfoTextBox.Text += Environment.NewLine + Environment.NewLine;


            AppData.Main.THInfoTextBox.Text += FunctionsRomajiKana.GetLangsOfString(selectedCellValue, "all"); //Show all detected languages count info
                                                                                                  //--------Считывание значения ячейки в текстовое поле 1
        }

        //https://stackoverflow.com/a/31150444
        private static void FormatTextBox()
        {
            if (AppData.Main.THTargetRichTextBox == null) return;

            int tl = 0;
            _ = AppData.Main.Invoke((Action)(() => tl = AppData.Main.THTargetRichTextBox.Text.Length));
            if (tl == 0) return;


            // Loop over each line
            int THTargetRichTextBoxLinesCount = 0;
            _ = AppData.Main.Invoke((Action)(() => THTargetRichTextBoxLinesCount = AppData.Main.THTargetRichTextBox.Lines.Length));
            for (int i = 0; i < THTargetRichTextBoxLinesCount; i++)
            {
                // Current line text
                string currentLine = string.Empty;
                _ = AppData.Main.Invoke((Action)(() => currentLine = AppData.Main.THTargetRichTextBox.Lines[i]));

                // Ignore the non-assembly lines
                if (currentLine.Length <= AppSettings.THOptionLineCharLimit) continue;

                // Start position
                int start = AppSettings.THOptionLineCharLimit;

                // Length
                int length = currentLine.Length - start;

                // Make the selection
                AppData.Main.THTargetRichTextBox.SelectionStart = start;
                AppData.Main.THTargetRichTextBox.SelectionLength = length;

                // Change the colour
                AppData.Main.THTargetRichTextBox.SelectionColor = Color.DarkRed;
            }
        }

        internal static volatile bool SaveInAction;
        internal static bool FileDataWasChanged;

        /// <summary>
        /// control progressbar Current\Max
        /// </summary>
        /// <param name="CurrentProgressBar">current value</param>
        /// <param name="MaxProgressBar">max value</param>
        public static void ProgressInfo(int CurrentProgressBar, int MaxProgressBar)
        {
            ProgressInfo(true, "", true, CurrentProgressBar, MaxProgressBar);
        }

        /// <summary>
        /// show status text in left-bottom field of window
        /// </summary>
        /// <param name="ShowStatus">show status</param>
        /// <param name="StatusText"></param>
        public static void ProgressInfo(string StatusText = "")
        {
            ProgressInfo(StatusText.Length > 0, StatusText);
        }

        /// <summary>
        /// show status text in left-bottom field of window
        /// </summary>
        /// <param name="ShowStatus">show status</param>
        /// <param name="StatusText"></param>
        public static void ProgressInfo(bool ShowStatus, string StatusText = "", bool SetProgressBar = false, int CurrentProgressBar = -1, int MaxProgressBar = -1)
        {
            if (SetProgressBar && AppData.Main.THActionProgressBar.Visible && AppData.Main.THInfolabel.Visible && CurrentProgressBar != -1 && MaxProgressBar != -1)
            {
                if (AppData.Main.THActionProgressBar.Style != ProgressBarStyle.Continuous)
                {
                    _ = AppData.Main.THActionProgressBar.Invoke((Action)(() => AppData.Main.THActionProgressBar.Style = ProgressBarStyle.Continuous));
                }
                if (AppData.Main.THActionProgressBar.Maximum != MaxProgressBar)
                {
                    _ = AppData.Main.THActionProgressBar.Invoke((Action)(() => AppData.Main.THActionProgressBar.Maximum = MaxProgressBar));
                }
                if (CurrentProgressBar < MaxProgressBar)
                {
                    _ = AppData.Main.THActionProgressBar.Invoke((Action)(() => AppData.Main.THActionProgressBar.Value = CurrentProgressBar));
                }
            }
            else
            {
                StatusText = StatusText?.Length == 0 ? T._("working..") : StatusText;
                try
                {
                    _ = AppData.Main.THActionProgressBar.Invoke((Action)(() => AppData.Main.THActionProgressBar.Visible = ShowStatus));
                    _ = AppData.Main.THInfolabel.Invoke((Action)(() => AppData.Main.THInfolabel.Visible = ShowStatus));
                    if (!ShowStatus)
                    {
                        _ = AppData.Main.THActionProgressBar.Invoke((Action)(() => AppData.Main.THActionProgressBar.Style = ProgressBarStyle.Marquee));
                    }
                    _ = AppData.Main.THInfolabel.Invoke((Action)(() => AppData.Main.THInfolabel.Text = StatusText));
                }
                catch
                {
                }
            }
        }

        internal static void CellChangedRegistration(int ColumnIndex = -1)
        {
            if (ColumnIndex > 0)
            {
                FileDataWasChanged = true;
            }
        }

        internal static void UpdateTranslationTextBoxValue(object sender, DataGridViewCellEventArgs e)
        {
            if (AppSettings.DGVCellInEditMode && sender is DataGridView DGV)
            {
                AppData.Main.THTargetRichTextBox.Text = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + string.Empty;
            }
        }


        //int numberOfRows=500;
        private static bool THFilesListBox_MouseClickBusy;
        internal static void ActionsOnTHFIlesListElementSelected()
        {
            if (THFilesListBox_MouseClickBusy || AppData.Main.THFilesList.GetSelectedIndex() == -1) //THFilesList.GetSelectedIndex() == -1 return - фикс исключения сразу после загрузки таблицы, когда индекс выбранной таблицы равен -1 
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

                if (AppData.Main.THFilesList.GetSelectedIndex() > -1)
                {
                    AppSettings.THFilesListSelectedIndex = AppData.Main.THFilesList.GetSelectedIndex();

                    BindToDataTableGridView(AppData.CurrentProject.FilesContent.Tables[AppSettings.THFilesListSelectedIndex]);
                }

                FunctionsUI.ShowNonEmptyRowsCount(AppData.Main.TableCompleteInfoLabel);//Show how many rows have translation

                HideAllColumnsExceptOriginalAndTranslation();

                SetFilterDGV(); //Init Filters datagridview

                SetOnTHFileElementsDataGridViewWasLoaded(); //Additional actions when elements of file was loaded in datagridview

                CheckFilterDGV(); //Apply filters if they is not empty

                FunctionsUI.UpdateTextboxes();

                FunctionsMenus.CreateFileRowMenus();

                FunctionsUI.BindTextBoxesOriginalTranslation();
            }
            catch (Exception)
            {
            }

            THFilesListBox_MouseClickBusy = false;
        }

        private static void HideAllColumnsExceptOriginalAndTranslation()
        {
            if (AppData.Main.THFileElementsDataGridView.Columns.Count == 2)
            {
                return;
            }

            foreach (DataGridViewColumn Column in AppData.Main.THFileElementsDataGridView.Columns)
            {
                if (Column.Name != THSettings.TranslationColumnName && Column.Name != THSettings.OriginalColumnName)
                {
                    Column.Visible = false;
                }
            }
        }

        public static void BindToDataTableGridView(DataTable DT)
        {
            if (AppData.Main.THFilesList != null && AppData.Main.THFilesList.GetSelectedIndex() > -1 && DT != null)//вторая попытка исправить исключение при выборе элемента списка
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
                        AppData.Main.THFileElementsDataGridView.DataSource = DT;

                        //во время прокрутки DGV чернела полоса прокрутки и в результате было получено исключение
                        //добавил это для возможного фикса
                        //https://fooobar.com/questions/1404812/datagridview-scrollbar-throwing-argumentoutofrange-exception
                        //upd. не исправляет проблему для этого dgv. возможно это dgv фильтров
                        AppData.Main.THFileElementsDataGridView.PerformLayout();
                    }
                }
                catch
                {
                }
            }
        }

        private static void SetOnTHFileElementsDataGridViewWasLoaded()
        {
            FunctionsUI.ControlsSwitchActivated = true;
            //ControlsSwitchIsOn = (CutToolStripMenuItem1.ShortcutKeys != Keys.None);

            if (AppData.Main.THFileElementsDataGridView != null && AppData.Main.THFileElementsDataGridView.Columns.Count > 1)
            {
                AppData.Main.THFileElementsDataGridView.Columns[THSettings.OriginalColumnName].HeaderText = T._(THSettings.OriginalColumnName);//THMainDGVOriginalColumnName;
                AppData.Main.THFileElementsDataGridView.Columns[THSettings.TranslationColumnName].HeaderText = T._(THSettings.TranslationColumnName);//THMainDGVTranslationColumnName;
                AppData.Main.THFileElementsDataGridView.Columns[THSettings.OriginalColumnName].ReadOnly = true;
                AppData.Main.THFiltersDataGridView.Enabled = true;
                AppData.Main.THSourceRichTextBox.Enabled = true;
                AppData.Main.THTargetRichTextBox.Enabled = true;
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

        private static void SetFilterDGV()
        {
            if (AppData.Main.THFiltersDataGridView.Columns.Count != AppData.Main.THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible))
            {
                AppData.Main.THFiltersDataGridView.Columns.Clear();
                AppData.Main.THFiltersDataGridView.Rows.Clear();
                //int visibleindex = -1;
                for (int cindx = 0; cindx < AppData.Main.THFileElementsDataGridView.Columns.Count; cindx++)
                {
                    if (AppData.Main.THFileElementsDataGridView.Columns[cindx].Visible)
                    {
                        _ = AppData.Main.THFiltersDataGridView.Columns.Add(AppData.Main.THFileElementsDataGridView.Columns[cindx].Name, AppData.Main.THFileElementsDataGridView.Columns[cindx].HeaderText);
                    }
                }
                _ = AppData.Main.THFiltersDataGridView.Rows.Add(1);
                AppData.Main.THFiltersDataGridView.CurrentRow.Selected = false;

                //во время прокрутки DGV чернела полоса прокрутки и в результате было получено исключение
                //добавил это для возможного фикса. возможно это этот dgv
                //https://fooobar.com/questions/1404812/datagridview-scrollbar-throwing-argumentoutofrange-exception
                AppData.Main.THFiltersDataGridView.PerformLayout();
                //почернения прокрутки вроде больше не видел, но ошибка с аргументом вне диапазона была снова
            }
        }

        internal static void CheckFilterDGV()
        {
            try
            {
                //private void DGVFilter()
                string OverallFilter = string.Empty;
                for (int c = 0; c < AppData.Main.THFiltersDataGridView.Columns.Count; c++)
                {
                    if ((AppData.Main.THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty).Length == 0)
                    {

                    }
                    else
                    {
                        //об экранировании спецсимволов
                        //http://skillcoding.com/Default.aspx?id=159
                        //https://webcache.googleusercontent.com/search?q=cache:irqjhHKbiFMJ:https://www.syncfusion.com/kb/4492/how-to-filter-special-characters-like-by-typing-it-in-dynamic-filter+&cd=6&hl=ru&ct=clnk&gl=ru
                        if (OverallFilter.Length == 0)
                        {
                            OverallFilter += "[" + AppData.Main.THFiltersDataGridView.Columns[c].Name + "] Like '%" + FunctionsTable.FixDataTableFilterStringValue(AppData.Main.THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty) + "%'";
                        }
                        else
                        {
                            OverallFilter += " AND ";
                            OverallFilter += "[" + AppData.Main.THFiltersDataGridView.Columns[c].Name + "] Like '%" + FunctionsTable.FixDataTableFilterStringValue(AppData.Main.THFiltersDataGridView.Rows[0].Cells[c].Value + string.Empty) + "%'";
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
                AppData.CurrentProject.FilesContent.Tables[AppData.Main.THFilesList.GetSelectedIndex()].DefaultView.RowFilter = OverallFilter;
            }
            catch
            {
            }
        }
    }
}