using System;
using System.Data;
using System.Drawing;
using System.Globalization;
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
                AppData.Main.THFileElementsDataGridView.Columns[THSettings.OriginalColumnName].HeaderText = T._(THSettings.OriginalColumnName);
                AppData.Main.THFileElementsDataGridView.Columns[THSettings.TranslationColumnName].HeaderText = T._(THSettings.TranslationColumnName);
                AppData.Main.THFileElementsDataGridView.Columns[THSettings.OriginalColumnName].ReadOnly = true;
                AppData.Main.THFiltersDataGridView.Enabled = true;
                AppData.Main.THSourceRichTextBox.Enabled = true;
                AppData.Main.THTargetRichTextBox.Enabled = true;
            }
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
            if (!AppSettings.ProjectIsOpened) return;

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

        internal static void PaintDigitInFrontOfRow(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (!AppSettings.ProjectIsOpened || AppData.Main.THFilesList.GetSelectedIndex() == -1)
                return;

            var grid = sender as DataGridView;

            int rowIdx = FunctionsTable.GetRealRowIndex(AppData.Main.THFilesList.GetSelectedIndex(), e.RowIndex);//здесь получаю реальный индекс из Datatable
            //string rowIdx = (e.RowIndex + 1) + string.Empty;

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

        internal static async Task THFileElementsDataGridView_CellValueChangedAsync(object sender, DataGridViewCellEventArgs e)
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

        internal static void THTargetTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+Del function
            //https://stackoverflow.com/questions/18543198/why-cant-i-press-ctrla-or-ctrlbackspace-in-my-textbox
            if (!e.Control || e.KeyCode != Keys.Back)
            {
                return;
            }

            e.SuppressKeyPress = true;
            int selStart = AppData.Main.THTargetRichTextBox.SelectionStart;
            while (selStart > 0 && AppData.Main.THTargetRichTextBox.Text.Substring(selStart - 1, 1) == " ")
            {
                selStart--;
            }
            int prevSpacePos = -1;
            if (selStart != 0)
            {
                prevSpacePos = AppData.Main.THTargetRichTextBox.Text.LastIndexOf(' ', selStart - 1);
            }
            AppData.Main.THTargetRichTextBox.Select(prevSpacePos + 1, AppData.Main.THTargetRichTextBox.SelectionStart - prevSpacePos - 1);
            AppData.Main.THTargetRichTextBox.SelectedText = string.Empty;
        }

        static ToolTip THToolTip;
        internal static void SetTooltips()
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
            THToolTip.SetToolTip(AppData.Main.THbtnMainResetTable, T._("Resets filters and tab sorting"));
            THToolTip.SetToolTip(AppData.Main.THFiltersDataGridView, T._("Filters for columns of main table"));
            THToolTip.SetToolTip(AppData.Main.TableCompleteInfoLabel, T._("Shows overal number of completed lines.\nClick to show first untranslated."));
            ////////////////////////////
        }

        internal static volatile bool IsOpeningInProcess;

        internal static void SetDoublebuffered(bool value)
        {
            // Double buffering can make DGV slow in remote desktop
            if (!SystemInformation.TerminalServerSession)
            {
                //THFileElementsDataGridView
                Type dgvType = AppData.Main.THFileElementsDataGridView.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(AppData.Main.THFileElementsDataGridView, value, null);

                //THFilesList
                //вроде не пашет для listbox
                Type lbxType = AppData.Main.THFilesList.GetType();
                PropertyInfo pi1 = lbxType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi1.SetValue(AppData.Main.THFilesList, value, null);
            }
        }

        internal static void SetUIStrings()
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

            AppData.SetSettings();

            FunctionsMenus.CreateMainMenus();

            FunctionsUI.SetUIStrings();

            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-winforms
            AppData.Main.THFilesList.SetDrawMode(DrawMode.OwnerDrawFixed);

            THTranslationCachePath = THSettings.THTranslationCacheFilePath;

            //THFileElementsDataGridView set doublebuffered to true
            FunctionsUI.SetDoublebuffered(true);
            if (File.Exists(THSettings.THLogPath) && new FileInfo(THSettings.THLogPath).Length > 1000000)
            {
                File.Delete(THSettings.THLogPath);
            }

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        internal static void THTargetTextBox_Leave(object sender, EventArgs e)
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

        internal static void THMain_FormClosing(object sender, FormClosingEventArgs e)
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

        internal static void THFileElementsDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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

        internal static void THMain_Load()
        {
            FunctionsUI.SetTooltips();

            //Disable links detection in edition textboxes
            AppData.Main.THSourceRichTextBox.DetectUrls = false;
            //DetectUrls = false;

            //Hide some items 
            AppData.Main.tlpTextLenPosInfo.Visible = false;
            AppData.Main.frmMainPanel.Visible = false;


            MenuItemRecent.UpdateRecentFiles();
        }

        internal static bool InteruptTranslation;
    }
}