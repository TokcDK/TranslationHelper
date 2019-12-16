﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Globalization;

namespace TranslationHelper
{
    public partial class THSearch : Form
    {
        //private readonly THMain Main;
        internal ListBox THFilesListBox;
        internal DataSet THFilesElementsDataset;
        internal DataGridView THFileElementsDataGridView;
        readonly RichTextBox THTargetRichTextBox;        

        public THSearch(DataSet DS, ListBox listBox, DataGridView DGV, RichTextBox TTB)
        {
            InitializeComponent();
            //Main = MainForm;
            THFilesListBox = listBox;
            THFilesElementsDataset = DS;
            THFileElementsDataGridView = DGV;
            THTargetRichTextBox = TTB;

            //translation
            this.THSearch1st.Text = T._("Find and Replace");
            //this.label3.Text = T._("Translation  Helper support:");
            this.SearchModeGroupBox.Text = T._("Search Mode");
            this.SearchModeNormalRadioButton.Text = T._("Normal");
            this.label5.Text = T._("Range");
            this.SearchRangeTableRadioButton.Text = T._("Table");
            this.SearchRangeAllRadioButton.Text = T._("Anywhere");
            this.label2.Text = T._("Method");
            this.SearchMethodOriginalToTranslationRadioButton.Text = T._("Find in Original and Paste to Translation");
            this.SearchMethodTranslationRadioButton.Text = T._("Find and Replace in Translation");
            this.THSearchMatchCaseCheckBox.Text = T._("Match Case");
            this.FindAllButton.Text = T._("Find All");
            this.SearchFormFindNextButton.Text = T._("Find Next");
            this.SearchFormReplaceButton.Text = T._("Replace");
            this.SearchFormReplaceAllButton.Text = T._("Replace All");
            this.THSearchFindWhatLabel.Text = T._("Find what:");
            this.label1.Text = T._("Replace with:");
            this.Text = T._("Find and Replace");

            if (SearchAlwaysOnTopCheckBox.Checked)
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }

        private void SearchModeNormalRadioButton_Click(object sender, EventArgs e)
        {
            SearchModeRegexRadioButton.Checked = false;
            SearchModeNormalRadioButton.Checked = true;
        }

        private void SearchModeRegexRadioButton_Click(object sender, EventArgs e)
        {
            SearchModeNormalRadioButton.Checked = false;
            SearchModeRegexRadioButton.Checked = true;
        }

        private void SearchMethodOriginalTranslationRadioButton_Click(object sender, EventArgs e)
        {
            SearchMethodOriginalToTranslationRadioButton.Checked = false;
            SearchMethodTranslationRadioButton.Checked = true;
        }

        private void SearchMethodTranslationRadioButton_Click(object sender, EventArgs e)
        {
            SearchMethodTranslationRadioButton.Checked = false;
            SearchMethodOriginalToTranslationRadioButton.Checked = true;
        }

        private void SearchRangeTableRadioButton_Click(object sender, EventArgs e)
        {
            SearchRangeAllRadioButton.Checked = false;
            SearchRangeTableRadioButton.Checked = true;
        }

        private void SearchRangeAllRadioButton_Click(object sender, EventArgs e)
        {
            SearchRangeTableRadioButton.Checked = false;
            SearchRangeAllRadioButton.Checked = true;
        }

        private string GetSearchColumn()
        {
            return SearchMethodTranslationRadioButton.Checked ? "Translation" : "Original";
        }

        int startrowsearchindex=0;        //Индекс стартовой ячейки для поиска
        int tableindex;
        int rowindex;
        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length==0 || THFilesElementsDataset == null)
            {
            }
            else
            {
                bool inputqualwithlatest = THSearchMatchCaseCheckBox.Checked ? SearchFormFindWhatTextBox.Text == lastfoundvalue : string.Compare(SearchFormFindWhatTextBox.Text, lastfoundvalue, true) == 0;
                if (inputqualwithlatest)
                {
                    string searchcolumn = GetSearchColumn();
                    tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString());
                    rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString());

                    if (tableindex == THFilesListBox.SelectedIndex)
                    {
                    }
                    else
                    {
                        THFilesListBox.SelectedIndex = tableindex;
                        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                    }
                    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];
                    
                    //подсвечивание найденного
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                    selectstring.Start();

                    startrowsearchindex++;
                    if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                    {
                        startrowsearchindex = 0;
                    }

                }
                else
                {
                    startrowsearchindex = 0;
                    lblError.Visible = false;
                    oDsResults = THFilesElementsDataset.Clone();
                    //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
                    DataTable drFoundRowsTable = SearchNew(oDsResults);

                    if (drFoundRowsTable == null)
                    {
                    }
                    else
                    {
                        if (drFoundRowsTable.Rows.Count > 0)
                        {
                            StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                            oDsResults.AcceptChanges();
                            
                            string searchcolumn = GetSearchColumn();
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString());
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString());

                            if (tableindex == THFilesListBox.SelectedIndex)
                            {
                            }
                            else
                            {
                                THFilesListBox.SelectedIndex = tableindex;
                                THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                            }
                            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                            selectstring.Start();

                            startrowsearchindex++;
                            if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                            {
                                startrowsearchindex = 0;
                            }
                            //PopulateGrid(oDsResults);

                            //lblError.Visible = true;
                            //lblError.Text = "Found " + drFoundRowsTable.Rows.Count + " records";
                            //this.Height = 589;
                        }
                        else
                        {
                            //PopulateGrid(null);
                            lblError.Visible = true;
                            lblError.Text = "Nothing Found.";
                            this.Height = 368;
                        }
                    }
                }

            }

            //if (!string.IsNullOrEmpty(SearchFormFindWhatTextBox.Text) && THFilesListBox.SelectedIndex >= 0)
            //{

            //    string searchcolumn = "Translation";
            //    if (SearchMethodTranslationRadioButton.Checked)
            //    {
            //        searchcolumn = "Original";
            //    }

            //    if (SearchRangeTableRadioButton.Checked)
            //    {
            //        //FileWriter.WriteData("c:\\logsearch.log", "\r\n0 0 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //        if (startrowsearchindex == THFilesElementsDataset.Tables[tableindex].Rows.Count)
            //        {
            //            startrowsearchindex = 0;
            //        }

            //        for (/*подразумевает стартовое значение startrowsearchindex, присвоеное выше*/; startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count; startrowsearchindex++)
            //        {
            //            if (IsContainsText(THFilesElementsDataset.Tables[tableindex].Rows[startrowsearchindex][searchcolumn].ToString(), SearchFormFindWhatTextBox.Text))
            //            {
            //                //FileWriter.WriteData("c:\\logsearch.log", "\r\n1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                //MessageBox.Show("srchind3)" + srchind.ToString());

            //                THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, startrowsearchindex];
            //                //THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = THFileElementsDataGridView.SelectedRows[0].Index;


            //                //FileWriter.WriteData("c:\\logsearch.log", "\r\n2 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                if (startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count)
            //                {
            //                    //FileWriter.WriteData("c:\\logsearch.log", "\r\n2.1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                    startrowsearchindex++;
            //                }
            //                else
            //                {
            //                    startrowsearchindex = 0;
            //                }
            //                //MessageBox.Show("srchind4)" + srchind.ToString());
            //                //FileWriter.WriteData("c:\\logsearch.log", "\r\n111 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                return;
            //            }
            //            //FileWriter.WriteData("c:\\logsearch.log", "\r\n222 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //            //MessageBox.Show("srchind5)" + srchind.ToString());
            //        }
            //    }
            //    else
            //    {
            //        //if (tableindex == THFilesElementsDataset.Tables.Count)
            //        //{
            //        //    tableindex = 0;
            //        //}
            //        //if (startrowsearchindex == THFilesElementsDataset.Tables[tableindex].Rows.Count)
            //        //{
            //        //    if (tableindex < THFilesElementsDataset.Tables.Count-1)
            //        //    {
            //        //        tableindex++;
            //        //    }
            //        //    startrowsearchindex = 0;
            //        //}

            //        for (; tableindex < THFilesElementsDataset.Tables.Count; tableindex++)
            //        {
            //            //FileWriter.WriteData("c:\\logsearch.log", "\r\n0 0 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);


            //            for (/*подразумевает стартовое значение startrowsearchindex, присвоеное выше*/; startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count; startrowsearchindex++)
            //            {
            //                if (IsContainsText(THFilesElementsDataset.Tables[tableindex].Rows[startrowsearchindex][searchcolumn].ToString(), SearchFormFindWhatTextBox.Text))
            //                {
            //                    //FileWriter.WriteData("c:\\logsearch.log", "\r\n1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                    //MessageBox.Show("srchind3)" + srchind.ToString());
            //                    if (tableindex == THFilesListBox.SelectedIndex)
            //                    {
            //                    }
            //                    else
            //                    {
            //                        THFilesListBox.SelectedIndex = tableindex;
            //                        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
            //                    }

            //                    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, startrowsearchindex];
            //                    //THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = THFileElementsDataGridView.SelectedRows[0].Index;


            //                    //FileWriter.WriteData("c:\\logsearch.log", "\r\n2 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                    if (startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count)
            //                    {
            //                        //FileWriter.WriteData("c:\\logsearch.log", "\r\n2.1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                        startrowsearchindex++;
            //                        if (startrowsearchindex == THFilesElementsDataset.Tables[tableindex].Rows.Count)
            //                        {
            //                            startrowsearchindex = 0;
            //                            //if (tableindex == THFilesElementsDataset.Tables.Count-1)
            //                            //{
            //                            //    tableindex = 0;
            //                            //}
            //                        }
            //                    }
            //                    //MessageBox.Show("srchind4)" + srchind.ToString());
            //                    //FileWriter.WriteData("c:\\logsearch.log", "\r\n111 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                    return;
            //                }
            //                //FileWriter.WriteData("c:\\logsearch.log", "\r\n222 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
            //                //MessageBox.Show("srchind5)" + srchind.ToString());
            //            }
            //        }
            //    }
            //}
        }

        string lastfoundvalue = string.Empty;
        private void StoryFoundValueToComboBox(string foundvalue)
        {
            lastfoundvalue = foundvalue;
            for (int i=0;i< SearchFormFindWhatComboBox.Items.Count;i++)
            {
                if (SearchFormFindWhatComboBox.Items[i].ToString() == foundvalue)
                {
                    return;
                }
            }
            SearchFormFindWhatComboBox.Items.Add(foundvalue);
        }

        private bool IsContainsText(string searchobject)
        {
            if (THSearchMatchCaseCheckBox.Checked)
            {
                return searchobject.Contains(SearchFormFindWhatTextBox.Text);
            }
            else
            {
                return searchobject.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant());
            }
        }

        private void PopulateGrid(DataSet oDsResults)
        {
            if (oDsResults == null)
            {
            }
            else
            {
                SearchResultsDatagridview.DataSource = oDsResults.Tables[0];
                for (int c=0;c< SearchResultsDatagridview.ColumnCount; c++)
                {
                    if (c > 1)
                    {
                        SearchResultsDatagridview.Columns[c].Visible = false;
                    }
                }
                SearchResultsDatagridview.Columns[0].ReadOnly = true;

                SearchResultsDatagridview.Visible = true;
                SearchResultsPanel.Visible = true;
            }
        }

        //http://mrbool.com/dataset-advance-operations-search-sort-filter-net/24769
        //https://stackoverflow.com/questions/3608388/c-sharp-access-dataset-data-from-another-class
        //http://qaru.site/questions/236566/how-to-know-the-row-index-from-datatable-object
        DataSet oDsResults;
        private void FindAllButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length==0 || THFilesElementsDataset == null)
            {
            }
            else
            {
                lblError.Visible = false;
                oDsResults = THFilesElementsDataset.Clone();
                //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
                DataTable drFoundRowsTable = SearchNew(oDsResults);

                if (drFoundRowsTable == null)
                {
                }
                else
                {
                    if (drFoundRowsTable.Rows.Count > 0)
                    {
                        StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                        oDsResults.AcceptChanges();
                        PopulateGrid(oDsResults);

                        lblError.Visible = true;
                        lblError.Text = T._("Found ") + drFoundRowsTable.Rows.Count + T._(" records");
                        this.Height = 589;


                    }
                    else
                    {
                        //PopulateGrid(null);
                        lblError.Visible = true;
                        lblError.Text = T._("Nothing Found");
                        this.Height = 368;
                    }
                }
            }
        }

        //Dictionary<int, int> oDsResultsCoordinates = new Dictionary<int, int>();
        private readonly DataTable oDsResultsCoordinates = new DataTable();
        //private DataTable SelectFromDatatables(DataSet oDsResults)
        //{
        //    //Check for user input
        //    if (THFilesElementsDataset.Tables.Count > 0)
        //    {
        //        DataRow[] drFilterRows;
        //        string searchcolumn = GetSearchColumn();

        //        //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
        //        //THFilesElementsDataset.Tables[0].CaseSensitive = THSearchMatchCaseCheckBox.Checked;

        //        string strQuery = "[" + THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName + "] Like '%" + SearchFormFindWhatTextBox.Text.Replace("'", "''").Replace("*", "[*]").Replace("%", "[%]").Replace("[", "-QB[BQ-").Replace("]", "[]]").Replace("-QB[BQ-", "[[]") + "%'";

        //        if (SearchRangeTableRadioButton.Checked)
        //        {
        //            //drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Select(strQuery);
        //            //drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable().Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName) == strQuery);


        //            //drFilterRows = GetDTRowsWithFoundValue(THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex], strQuery);
        //            if (THSearchMatchCaseCheckBox.Checked)
        //            {
        //                //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
        //                drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
        //                                .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).Contains(SearchFormFindWhatTextBox.Text)).ToArray();
        //            }
        //            else
        //            {
        //                //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
        //                drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
        //                                .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).ToLowerInvariant().Contains(SearchFormFindWhatTextBox.Text.ToLowerInvariant())).ToArray();
        //            }                  

        //            if (drFilterRows.Length > 0)
        //            {
        //                oDsResultsCoordinates.Clear();
        //                foreach (DataRow dr in drFilterRows)
        //                {
        //                    oDsResults.Tables[0].ImportRow(dr);
        //                    oDsResultsCoordinates.Rows.Add(THFilesListBox.SelectedIndex, THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Rows.IndexOf(dr));
        //                    FileWriter.WriteData(@"c:\\ddd.log", "\r\nindex=" + THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Rows.IndexOf(dr));
        //                }
        //            }

        //            //MessageBox.Show(oDsResults.Tables[0].Rows[0][1].ToString());
        //            return oDsResults.Tables[0];
        //        }
        //        else
        //        {
        //            oDsResultsCoordinates.Clear();
        //            for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
        //            {                        
        //                drFilterRows = GetDTRowsWithFoundValue(THFilesElementsDataset.Tables[t], strQuery, searchcolumn);

        //                if (drFilterRows.Length > 0)
        //                {
        //                    foreach (DataRow dr in drFilterRows)
        //                    {
        //                        oDsResults.Tables[0].ImportRow(dr);
        //                        oDsResultsCoordinates.Rows.Add(t, THFilesElementsDataset.Tables[t].Rows.IndexOf(dr));
        //                        FileWriter.WriteData(@"c:\\ddd.log", "\r\nindex=" + THFilesElementsDataset.Tables[t].Rows.IndexOf(dr));
        //                    }
        //                }
        //            }

        //            //MessageBox.Show(oDsResults.Tables[0].Rows[0][1].ToString());
        //            return oDsResults.Tables[0];
        //        }
        //    }
        //    return null;
        //}


        private DataTable SearchNew(DataSet DS)
        {
            if (THFilesElementsDataset.Tables.Count > 0)
            {
                string searchcolumn = GetSearchColumn();
                string strQuery = SearchFormFindWhatTextBox.Text;
                oDsResultsCoordinates.Rows.Clear();
                int DatatablesCount = SearchRangeTableRadioButton.Checked ? THFilesListBox.SelectedIndex + 1 : THFilesElementsDataset.Tables.Count;
                int StartTableIndex = SearchRangeTableRadioButton.Checked ? THFilesListBox.SelectedIndex : 0;
                for (int t = StartTableIndex; t < DatatablesCount; t++)
                {
                    for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
                    {
                        if ((THFilesElementsDataset.Tables[t].Rows[r][searchcolumn] + string.Empty).Length == 0)
                        {

                        }
                        else
                        {
                            DataRow Row = THFilesElementsDataset.Tables[t].Rows[r];
                            string SelectedCellValue = THFilesElementsDataset.Tables[t].Rows[r][searchcolumn] + string.Empty;
                            if (SearchModeRegexRadioButton.Checked)
                            {
                                try
                                {
                                    if (THSearchMatchCaseCheckBox.Checked)
                                    {
                                        if (Regex.IsMatch(SelectedCellValue, strQuery))
                                        {
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
                                    }
                                    else
                                    {
                                        if (Regex.IsMatch(SelectedCellValue, strQuery, RegexOptions.IgnoreCase))
                                        {
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
                                    }
                                }
                                catch
                                {
                                    return null;//при ошибках регекса выходить
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (THSearchMatchCaseCheckBox.Checked)
                                    {
                                        if (SelectedCellValue.Contains(strQuery))
                                        {
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
                                    }
                                    else
                                    {
                                        if (SelectedCellValue.ToUpperInvariant().Contains(strQuery.ToUpperInvariant()))
                                        {
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
                                    }
                                }
                                catch
                                {

                                }

                            }
                        }
                    }
                }
            }
            return DS.Tables[0];
        }

        //private DataRow[] GetDTRowsWithFoundValue(DataTable DT, string strQuery, string searchcolumn)
        //{
        //    //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
        //    //DT.CaseSensitive = THSearchMatchCaseCheckBox.Checked;

        //    //return DT.Select(strQuery);
        //    if (THSearchMatchCaseCheckBox.Checked)
        //    {
        //        //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
        //        return THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
        //                        .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).Contains(strQuery)).ToArray();
        //    }
        //    else
        //    {
        //        //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
        //        return THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
        //                        .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).ToLowerInvariant().Contains(strQuery.ToLowerInvariant())).ToArray();
        //    }
        //}

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://patreon.com/TranslationHelper");
        }

        //private readonly DataTable SearchFormFindWhatComboBoxCustomeSource = new DataTable();
        private void THSearch_Load(object sender, EventArgs e)
        {
            //some other info: https://stackoverflow.com/questions/20893725/how-to-hide-and-show-panels-on-a-form-and-have-it-resize-to-take-up-slack
            this.Height = 368;
            tableindex = THFilesListBox.SelectedIndex;
            oDsResultsCoordinates.Columns.Add("t");
            oDsResultsCoordinates.Columns.Add("r");
            //SearchFormFindWhatComboBox.AutoCompleteCustomSource = SearchFormFindWhatComboBoxCustomeSource.Rows.
        }

        private void SelectTextinTextBox(string input)
        {

            _ = Invoke(new MethodInvoker(delegate ()
            {
                Thread.Sleep(200);
                if (THTargetRichTextBox.Text.Length==0)
                {
                    //MessageBox.Show("THTargetRichTextBox.Text="+ THTargetRichTextBox.Text);
                }
                else
                {
                    if (SearchModeRegexRadioButton.Checked)
                    {
                        //https://www.c-sharpcorner.com/article/search-and-highlight-text-in-rich-textbox/
                        //распознает лучше, чем код ниже, но не выделяет слово TEST
                        //string[] words = SearchFormFindWhatTextBox.Text.Split(' ');
                        bool MatchCase = THSearchMatchCaseCheckBox.Checked;
                        MatchCollection mc = Regex.Matches(THTargetRichTextBox.Text, SearchFormFindWhatTextBox.Text, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
                        if (mc.Count > 0)
                        {
                            string m = mc[0].Value;
                            foreach (Match word in mc)
                            {
                                //string word = SearchFormFindWhatTextBox.Text;
                                int startindex = 0;
                                while (startindex < THTargetRichTextBox.TextLength)
                                {
                                    //int wordstartIndex = THTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.None);
                                    int wordstartIndex;
                                    if (MatchCase)
                                    {
                                        wordstartIndex = THTargetRichTextBox.Find(word.Value, startindex, RichTextBoxFinds.MatchCase);
                                    }
                                    else
                                    {
                                        wordstartIndex = THTargetRichTextBox.Find(word.Value, startindex, RichTextBoxFinds.None);
                                    }
                                    if (wordstartIndex == -1)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        THTargetRichTextBox.SelectionStart = wordstartIndex;
                                        THTargetRichTextBox.SelectionLength = word.Length;
                                        THTargetRichTextBox.SelectionBackColor = Color.Yellow;
                                    }

                                    startindex += wordstartIndex + word.Length;
                                }
                            }
                        }
                    }
                    else
                    {
                        string word = SearchFormFindWhatTextBox.Text;
                        int startindex = 0;
                        while (startindex < THTargetRichTextBox.TextLength)
                        {
                            int wordstartIndex;
                            if (THSearchMatchCaseCheckBox.Checked)
                            {
                                wordstartIndex = THTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.MatchCase);
                            }
                            else
                            {
                                wordstartIndex = THTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.None);
                            }
                            if (wordstartIndex == -1)
                            {
                                break;
                            }
                            else
                            {
                                THTargetRichTextBox.SelectionStart = wordstartIndex;
                                THTargetRichTextBox.SelectionLength = word.Length;
                                THTargetRichTextBox.SelectionBackColor = Color.Yellow;
                            }

                            startindex += wordstartIndex + word.Length;
                        }
                    }


                    ////FileWriter.WriteData(@"c:\\ddd.log", "THTargetRichTextBox.Text=" + THTargetRichTextBox.Text+ ", input="+ input);
                    ////https://1bestcsharp.blogspot.com/2016/09/c-search-and-highlight-text-in-richtextbox.html
                    ////странное поведение с выборочным выделением
                    //string text;
                    //string selectedtext;
                    //text = THTargetRichTextBox.Text;
                    //selectedtext = SearchFormFindWhatTextBox.Text;
                    //text = THTargetRichTextBox.Text;
                    //selectedtext = SearchFormFindWhatTextBox.Text;
                    ////if (THSearchMatchCaseCheckBox.Checked)
                    ////{
                    ////    text = THTargetRichTextBox.Text;
                    ////    selectedtext = SearchFormFindWhatTextBox.Text;
                    ////}
                    ////else
                    ////{
                    ////    text = THTargetRichTextBox.Text.ToLowerInvariant();
                    ////    selectedtext = SearchFormFindWhatTextBox.Text.ToLowerInvariant();
                    ////}

                    //int start = 0;
                    //int end = THTargetRichTextBox.Text.LastIndexOf(selectedtext);

                    //THTargetRichTextBox.SelectAll();
                    //THTargetRichTextBox.SelectionBackColor = Color.White;
                    ////FileWriter.WriteData(@"c:\\ddd.log", "\r\ntext=" + text + ", selectedtext=" + selectedtext);
                    //while (start < end)
                    //{
                    //    if (THSearchMatchCaseCheckBox.Checked)
                    //    {
                    //        THTargetRichTextBox.Find(selectedtext, start, THTargetRichTextBox.TextLength, RichTextBoxFinds.MatchCase);
                    //    }
                    //    else
                    //    {
                    //        THTargetRichTextBox.Find(selectedtext, start, THTargetRichTextBox.TextLength, RichTextBoxFinds.None);
                    //    }

                    //    THTargetRichTextBox.SelectionBackColor = Color.Yellow;

                    //    start = THTargetRichTextBox.Text.IndexOf(selectedtext, start) + 1;
                    //}
                }

            }));


            ////https://www.google.com/search?ei=VShgXcHAEZjfz7sP8rOEkA8&q=c%23+select+found+text+in+textbox&oq=c%23+select+found+text+in+textbox&gs_l=psy-ab.3..33i22i29i30l6.138636.149771..151070...2.2..0.462.11327.3-4j23......0....1..gws-wiz.......0i71j0j0i22i30.3S72oBLUidA&ved=0ahUKEwiBv737x5nkAhWY73MBHfIZAfIQ4dUDCAo&uact=5#kpvalbx=_7ShgXenmLdrXz7sP7pSusAg19
            ////https://stackoverflow.com/questions/9682800/search-specified-string-inside-textbox
            //Thread.Sleep(100);
            //if (string.IsNullOrEmpty(THTargetTextBox.Text))
            //{

            //}
            //else
            //{
            ////    https://stackoverflow.com/questions/9682800/search-specified-string-inside-textbox
            //    int pos = 0;
            //    Main.Invoke((Action)(() => pos = THTargetTextBox.Text.IndexOf(input)));
            //    if (pos != -1)
            //    {
            //        Main.Invoke((Action)(() => THTargetTextBox.SelectionStart = pos));
            //        Main.Invoke((Action)(() => THTargetTextBox.SelectionLength = input.Length));
            //    }
            //}
        }

        private void SearchResultsDatagridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string searchcolumn = GetSearchColumn();
            tableindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][0].ToString());

            THFilesElementsDataset.Tables[tableindex].DefaultView.RowFilter = string.Empty;
            THFilesElementsDataset.Tables[tableindex].DefaultView.Sort = string.Empty;
            THFileElementsDataGridView.Refresh();

            rowindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][1].ToString());


            if (tableindex == THFilesListBox.SelectedIndex)
            {
            }
            else
            {
                THFilesListBox.SelectedIndex = tableindex;
                THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
            }
            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];
            
            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(THFileElementsDataGridView.CurrentCell.Value.ToString())));
            selectstring.Start();


            //int SelectedIndex;
            //for (int t=0;t< THFilesElementsDataset.Tables.Count; t++)
            //{
            //    for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
            //    {
            //        if (THFilesElementsDataset.Tables[t].Rows[r][0].ToString()== oDsResults.Tables[0].Rows[e.RowIndex][0].ToString())
            //        {
            //            if (t == THFilesListBox.SelectedIndex)
            //            {
            //            }
            //            else
            //            {
            //                THFilesListBox.SelectedIndex = t;
            //                THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[t];
            //            }
            //            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[0, r];
            //            return;
            //        }
            //    }


            //    ////http://qaru.site/questions/236566/how-to-know-the-row-index-from-datatable-object
            //    //SelectedIndex = THFilesElementsDataset.Tables[t].Rows.IndexOf(oDsResults.Tables[0].Rows[e.RowIndex]);
            //    //FileWriter.WriteData(@"c:\\Search1.log", "\r\nSelectedIndex="+ SelectedIndex+"\r\nValue="+ oDsResults.Tables[0].Rows[e.RowIndex][0].ToString());
            //    //if (SelectedIndex >= 0)
            //    //{
            //    //    if (t == THFilesListBox.SelectedIndex)
            //    //    {
            //    //    }
            //    //    else
            //    //    {
            //    //        THFilesListBox.SelectedIndex = t;
            //    //        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[t];
            //    //    }
            //    //    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[0, SelectedIndex];
            //    //    return;
            //    //}
            //}

        }

        private void SearchFormFindWhatComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            SearchFormFindWhatTextBox.Text = SearchFormFindWhatComboBox.SelectedItem.ToString();
        }

        private void SearchFormReplaceButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || SearchFormReplaceWithTextBox.Text.Length == 0 || THFilesElementsDataset == null)
            {
            }
            else
            {
                bool inputqualwithlatest = THSearchMatchCaseCheckBox.Checked ? SearchFormFindWhatTextBox.Text == lastfoundvalue : string.Compare(SearchFormFindWhatTextBox.Text, lastfoundvalue, true) == 0;
                if (inputqualwithlatest)
                {
                    string searchcolumn = GetSearchColumn();
                    
                    if (startrowsearchindex == 0)
                    {
                    }
                    else
                    {
                        if (tableindex >= 0 && tableindex < THFilesListBox.Items.Count && rowindex >= 0 && rowindex < THFileElementsDataGridView.Rows.Count)
                        {
                            string value = THFileElementsDataGridView[searchcolumn, rowindex].Value + string.Empty;
                            if (value.Length > 0)
                            {
                                if (SearchModeRegexRadioButton.Checked)
                                {
                                    if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                                    {
                                        THFileElementsDataGridView["Translation", rowindex].Value = Regex.Replace(GetFirstIfNotEmpty(THFileElementsDataGridView["Translation", rowindex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, RegexOptions.IgnoreCase);
                                    }
                                }
                                else
                                {
                                    if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                                    {
                                        THFileElementsDataGridView["Translation", rowindex].Value = ReplaceEx.Replace(GetFirstIfNotEmpty(THFileElementsDataGridView["Translation", rowindex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                    }

                    if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                    {
                        startrowsearchindex = 0;
                    }

                    tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString());
                    rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString());


                    if (tableindex == THFilesListBox.SelectedIndex)
                    {
                    }
                    else
                    {
                        THFilesListBox.SelectedIndex = tableindex;
                        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                    }
                    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];

                    //подсвечивание найденного
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                    selectstring.Start();

                    startrowsearchindex++;
                }
                else
                {
                    startrowsearchindex = 0;
                    lblError.Visible = false;
                    oDsResults = THFilesElementsDataset.Clone();
                    //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
                    DataTable drFoundRowsTable = SearchNew(oDsResults);

                    if (drFoundRowsTable == null)
                    {
                    }
                    else
                    {
                        if (drFoundRowsTable.Rows.Count > 0)
                        {
                            StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                            oDsResults.AcceptChanges();

                            string searchcolumn = GetSearchColumn();
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString());
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString());

                            if (tableindex == THFilesListBox.SelectedIndex)
                            {
                            }
                            else
                            {
                                THFilesListBox.SelectedIndex = tableindex;
                                THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                            }
                            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                            selectstring.Start();

                            startrowsearchindex++;
                            if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                            {
                                startrowsearchindex = 0;
                            }
                            //PopulateGrid(oDsResults);

                            //lblError.Visible = true;
                            //lblError.Text = "Found " + drFoundRowsTable.Rows.Count + " records";
                            //this.Height = 589;
                        }
                        else
                        {
                            //PopulateGrid(null);
                            lblError.Visible = true;
                            lblError.Text = "Nothing Found.";
                            this.Height = 368;
                        }
                    }
                }

            }
        }

        private string GetFirstIfNotEmpty(string first, string defaultvalue)
        {
            return first.Length == 0 ? defaultvalue : first;
        }

        private void SearchFormReplaceAllButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || THFilesElementsDataset == null)
            {
            }
            else
            {
                lblError.Visible = false;
                oDsResults = THFilesElementsDataset.Clone();
                //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
                DataTable drFoundRowsTable = SearchNew(oDsResults);

                if (drFoundRowsTable == null)
                {
                }
                else
                {
                    if (drFoundRowsTable.Rows.Count > 0)
                    {
                        StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                        oDsResults.AcceptChanges();
                        PopulateGrid(oDsResults);

                        lblError.Visible = true;
                        lblError.Text = T._("Found ") + drFoundRowsTable.Rows.Count + T._(" records");
                        this.Height = 589;

                        string searchcolumn = GetSearchColumn();
                        for (int r=0; r< oDsResults.Tables[0].Rows.Count; r++)
                        {
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[r][0] + string.Empty, CultureInfo.CurrentCulture);
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[r][1] + string.Empty, CultureInfo.CurrentCulture);

                            string value = THFilesElementsDataset.Tables[tableindex].Rows[rowindex][searchcolumn] + string.Empty;
                            if (value.Length > 0)
                            {
                                if (SearchModeRegexRadioButton.Checked)
                                {
                                    if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                                    {
                                        THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] = Regex.Replace(GetFirstIfNotEmpty(THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, RegexOptions.IgnoreCase);
                                    }
                                }
                                else
                                {
                                    if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                                    {
                                        THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] = ReplaceEx.Replace(GetFirstIfNotEmpty(THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //PopulateGrid(null);
                        lblError.Visible = true;
                        lblError.Text = T._("Nothing Found");
                        this.Height = 368;
                    }
                }
            }
        }

        private void THSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
            oDsResultsCoordinates.Dispose();
        }

        private void SearchAlwaysOnTopCheckBox_Click(object sender, EventArgs e)
        {
        }

        //https://www.c-sharpcorner.com/uploadfile/kirtan007/make-form-stay-always-on-top-of-every-window/
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        //private static readonly IntPtr HWND_TOP = new IntPtr(0);
        //private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private void SearchAlwaysOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SearchAlwaysOnTopCheckBox.Checked)
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
            else
            {
                SetWindowPos(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }
    }
}
