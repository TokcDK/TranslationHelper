using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Main.Functions
{
    class FunctionsCopyPaste
    {
        public static void CopyToClipboard(DataGridView THFileElementsDataGridView)
        {
            //Copy to clipboard
            DataObject dataObj = THFileElementsDataGridView.GetClipboardContent();
            if (dataObj == null)
            {
            }
            else
            {
                Clipboard.SetDataObject(dataObj);
            }
        }

        /// <summary>
        /// МОДИФИЦИРОВАНО
        /// Вставляет значения из буфера обмена в ячейки.
        /// Модифицированная функция учитывает количество строк в ячейке оригинала
        /// и вставляет столько же строк из буфера в ячейку перевода
        /// </summary>
        public static void PasteClipboardValue(DataGridView THFileElementsDataGridView)
        {
            //LogToFile("PasteClipboardValue Enter");
            //Show Error if no cell is selected
            if (THFileElementsDataGridView.SelectedCells.Count == 0)
            {
                MessageBox.Show(T._("Select cell first"), T._("Paste"),
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //LogToFile("PasteClipboardValue Enter 1");
            int origcolindex = AppData.CurrentProject.OriginalColumnIndex;
            //int transcolindex = THFileElementsDataGridView.Columns[THSettings.TranslationColumnName].Index;

            //Get the starting Cell
            DataGridViewCell startCell = GetStartCell(THFileElementsDataGridView);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue =
                    ClipBoardValues(Clipboard.GetText());

            int origcellmaxlines;
            int origcellcurlines = 0;
            bool OrigMaxEqualCurrent = false;
            int iRowIndex = startCell.RowIndex;
            StringBuilder cellvalue = new StringBuilder();
            DataGridViewCell cell = null;
            foreach (int rowKey in cbValue.Keys)
            {
                //LogToFile("PasteClipboardValue rowKey="+ rowKey);
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //LogToFile("PasteClipboardValue iColIndex=" + iColIndex);
                    //Check if the index is within the limit
                    if (iColIndex <= THFileElementsDataGridView.Columns.Count - 1
                    && iRowIndex <= THFileElementsDataGridView.Rows.Count - 1)
                    {
                        cell = THFileElementsDataGridView[iColIndex, iRowIndex];
                        origcellmaxlines = (THFileElementsDataGridView[origcolindex, iRowIndex].Value + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/).Length;

                        origcellcurlines++;
                        OrigMaxEqualCurrent = origcellcurlines == origcellmaxlines;

                        if (cell.ReadOnly)
                        {
                        }
                        else
                        {
                            if ((cell.Value + string.Empty).Length == 0)
                            {
                                // //LogToFile("PasteClipboardValue origcellmaxlines=" + origcellmaxlines + ",origcellcurlines=" + origcellcurlines);
                                //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                                // Закомментировал как здесь: https://code.google.com/p/seminary-software-engineering/source/browse/trunk/SystemForResultsEvaluaton/SystemForResultsEvaluaton/Core.cs?spec=svn21&r=21
                                //if ((chkPasteToSelectedCells.Checked && cell.Selected) ||
                                //   (!chkPasteToSelectedCells.Checked))
                                if (cbValue.Count > 1 && OrigMaxEqualCurrent)//модифицировано, чтобы при вставке нескольких строк значений выделенные ячейки убирался символ возврата каретки, если в буффере несколько значений
                                {
                                    //LogToFile("PasteClipboardValue cbValue.Count > 1 && OrigMaxEqualCurrent");
                                    //получались двойные значения при использовании с функцией автоподстановки для похожих
                                    //LogToFile("PasteClipboardValue value=" + Regex.Replace(cbValue[rowKey][cellKey], @"\r$", string.Empty));
                                    //cell.Value += Regex.Replace(cbValue[rowKey][cellKey], @"\r$", string.Empty);
                                    cellvalue.Append(Regex.Replace(cbValue[rowKey][cellKey], @"\r$", string.Empty));
                                }
                                else
                                {
                                    //LogToFile("PasteClipboardValue NOT cbValue.Count > 1 && OrigMaxEqualCurrent");
                                    //получались двойные значения при использовании с функцией автоподстановки для похожих
                                    //LogToFile("PasteClipboardValue value=" + cbValue[rowKey][cellKey]);
                                    //cell.Value += cbValue[rowKey][cellKey];//asdfg
                                    cellvalue.AppendLine(cbValue[rowKey][cellKey]);//asdfg
                                }
                            }
                        }
                        //LogToFile("cbValue[rowKey][cellKey]=" + cbValue[rowKey][cellKey]+ ",cell.Value="+ cell.Value);
                    }
                    //LogToFile("PasteClipboardValue next col, iColIndex=" + iColIndex);
                    iColIndex++;
                }
                //LogToFile("PasteClipboardValue check if OrigMaxEqualCurrent is true and it is " + OrigMaxEqualCurrent);
                if (OrigMaxEqualCurrent)
                {
                    if (cell != null)
                    {
                        if ((cell.Value + string.Empty).Length == 0)
                        {
                            cell.Value = cellvalue + string.Empty;
                        }
                    }
                    cellvalue.Clear();

                    //сильно тормозит процесс, отключил
                    //Main.THAutoSetSameTranslationForSimular(Main.THFilesList.GetSelectedIndex(), iRowIndex, origcolindex);

                    origcellcurlines = 0;
                    //LogToFile("PasteClipboardValue next row, iRowIndex=" + iRowIndex);
                    iRowIndex++;
                }
            }
            //LogToFile(string.Empty,true);
            //LogToFile("PasteClipboardValue Exit");
        }

        public static DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }

        public static Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionary with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }
    }
}
