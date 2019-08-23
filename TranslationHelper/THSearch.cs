using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class c : Form
    {
        private THMain Main;
        public ListBox THFilesListBox;
        public DataSet THFilesElementsDataset;
        public DataGridView THFileElementsDataGridView;
        TextBox THTargetTextBox;
        public c(THMain MainForm, DataSet DS, ListBox listBox, DataGridView DGV, TextBox TTB)
        {
            InitializeComponent();
            Main = MainForm;
            THFilesListBox = listBox;
            THFilesElementsDataset = DS;
            THFileElementsDataGridView = DGV;
            THTargetTextBox = TTB;
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
            SearchMethodTranslationRadioButton.Checked = false;
            SearchMethodOriginalTranslationRadioButton.Checked = true;
        }

        private void SearchMethodTranslationRadioButton_Click(object sender, EventArgs e)
        {
            SearchMethodOriginalTranslationRadioButton.Checked = false;
            SearchMethodTranslationRadioButton.Checked = true;
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
            if (SearchMethodTranslationRadioButton.Checked)
            {
                return "Original";
            }
            else
            {
                return "Translation";
            }
        }

        int startrowsearchindex=0;        //Индекс стартовой ячейки для поиска
        int tableindex;
        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SearchFormFindWhatTextBox.Text.Trim()) /*|| SearchFormFindWhatTextBox.Text == lastfoundvalue*/ || THFilesElementsDataset == null)
            {
            }
            else
            {
                if (SearchFormFindWhatTextBox.Text == lastfoundvalue)
                {
                    string searchcolumn = GetSearchColumn();
                    int tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString());
                    int rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString());

                    if (tableindex == THFilesListBox.SelectedIndex)
                    {
                    }
                    else
                    {
                        THFilesListBox.SelectedIndex = tableindex;
                        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                    }
                    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];
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
                    DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);

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
                            int tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString());
                            int rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString());

                            if (tableindex == THFilesListBox.SelectedIndex)
                            {
                            }
                            else
                            {
                                THFilesListBox.SelectedIndex = tableindex;
                                THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                            }
                            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];
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

        string lastfoundvalue = "";
        private void StoryFoundValueToComboBox(string foundvalue)
        {
            lastfoundvalue = foundvalue;
            bool found = false;
            for (int i=0;i< SearchFormFindWhatComboBox.Items.Count;i++)
            {
                if (SearchFormFindWhatComboBox.Items[i].ToString() == foundvalue)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
            }
            else
            {
                SearchFormFindWhatComboBox.Items.Add(foundvalue);
            }
        }

        private bool IsContainsText(string searchobject, string searchinput)
        {
            if (THSearchMatchCaseCheckBox.Checked)
            {
                return searchobject.Contains(SearchFormFindWhatTextBox.Text);
            }
            else
            {
                return searchobject.ToLowerInvariant().Contains(SearchFormFindWhatTextBox.Text.ToLowerInvariant());
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
            if (string.IsNullOrEmpty(SearchFormFindWhatTextBox.Text.Trim()) || SearchFormFindWhatTextBox.Text == lastfoundvalue || THFilesElementsDataset == null)
            {

            }
            else
            {
                lblError.Visible = false;
                oDsResults = THFilesElementsDataset.Clone();
                DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);

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
                        lblError.Text = "Found " + drFoundRowsTable.Rows.Count + " records";
                        this.Height = 589;
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

        //Dictionary<int, int> oDsResultsCoordinates = new Dictionary<int, int>();
        DataTable oDsResultsCoordinates = new DataTable();
        private DataTable SelectFromDatatables(DataSet oDsResults)
        {
            //Check for user input
            if (THFilesElementsDataset.Tables.Count > 0)
            {
                DataRow[] drFilterRows;
                string searchcolumn = GetSearchColumn();

                //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
                if (THSearchMatchCaseCheckBox.Checked)
                {
                    THFilesElementsDataset.Tables[0].CaseSensitive = true;
                }
                else
                {
                    THFilesElementsDataset.Tables[0].CaseSensitive = false;
                }

                string strQuery = "[" + THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName + "] Like '%" + SearchFormFindWhatTextBox.Text.Replace("'", "''").Replace("*", "[*]").Replace("%", "[%]").Replace("[", "-QB[BQ-").Replace("]", "[]]").Replace("-QB[BQ-", "[[]") + "%'";

                if (SearchRangeTableRadioButton.Checked)
                {
                    //drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Select(strQuery);
                    //drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable().Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName) == strQuery);


                    //drFilterRows = GetDTRowsWithFoundValue(THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex], strQuery);
                    if (THSearchMatchCaseCheckBox.Checked)
                    {
                        //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
                        drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
                                        .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).Contains(SearchFormFindWhatTextBox.Text)).ToArray();
                    }
                    else
                    {
                        //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
                        drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
                                        .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).ToLowerInvariant().Contains(SearchFormFindWhatTextBox.Text.ToLowerInvariant())).ToArray();
                    }                  

                    if (drFilterRows.Length > 0)
                    {
                        oDsResultsCoordinates.Clear();
                        foreach (DataRow dr in drFilterRows)
                        {
                            oDsResults.Tables[0].ImportRow(dr);
                            oDsResultsCoordinates.Rows.Add(THFilesListBox.SelectedIndex, THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Rows.IndexOf(dr));
                            FileWriter.WriteData(@"c:\\ddd.log", "\r\nindex=" + THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Rows.IndexOf(dr));
                        }
                    }

                    //MessageBox.Show(oDsResults.Tables[0].Rows[0][1].ToString());
                    return oDsResults.Tables[0];
                }
                else
                {
                    oDsResultsCoordinates.Clear();
                    for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
                    {                        
                        drFilterRows = GetDTRowsWithFoundValue(THFilesElementsDataset.Tables[t], strQuery);

                        if (drFilterRows.Length > 0)
                        {
                            foreach (DataRow dr in drFilterRows)
                            {
                                oDsResults.Tables[0].ImportRow(dr);
                                oDsResultsCoordinates.Rows.Add(t, THFilesElementsDataset.Tables[t].Rows.IndexOf(dr));
                                FileWriter.WriteData(@"c:\\ddd.log", "\r\nindex=" + THFilesElementsDataset.Tables[t].Rows.IndexOf(dr));
                            }
                        }
                    }

                    //MessageBox.Show(oDsResults.Tables[0].Rows[0][1].ToString());
                    return oDsResults.Tables[0];
                }
            }
            return null;
        }

        private DataRow[] GetDTRowsWithFoundValue(DataTable DT, string strQuery)
        {
            return DT.Select(strQuery);
            //if (THSearchMatchCaseCheckBox.Checked)
            //{
            //    //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
            //    drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
            //                    .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).Contains(SearchFormFindWhatTextBox.Text)).ToArray();
            //}
            //else
            //{
            //    //https://stackoverflow.com/questions/13292771/enable-case-sensitive-when-using-datatable-select
            //    drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].AsEnumerable()
            //                    .Where(r => r.Field<string>(THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName).ToLowerInvariant().Contains(SearchFormFindWhatTextBox.Text.ToLowerInvariant())).ToArray();
            //}
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://patreon.com/TranslationHelper");
        }

        DataTable SearchFormFindWhatComboBoxCustomeSource = new DataTable();
        private void THSearch_Load(object sender, EventArgs e)
        {
            //some other info: https://stackoverflow.com/questions/20893725/how-to-hide-and-show-panels-on-a-form-and-have-it-resize-to-take-up-slack
            this.Height = 368;
            tableindex = THFilesListBox.SelectedIndex;
            oDsResultsCoordinates.Columns.Add("t");
            oDsResultsCoordinates.Columns.Add("r");
            //SearchFormFindWhatComboBox.AutoCompleteCustomSource = SearchFormFindWhatComboBoxCustomeSource.Rows.
        }

        private void SearchFormReplaceButton_Click(object sender, EventArgs e)
        {
            //Main.BindToDataTableGridView(THFilesElementsDataset.Tables[tableindex]);
            THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[0];
        }

        private void SelectTextinTextBox(string input)
        {
            //https://www.google.com/search?ei=VShgXcHAEZjfz7sP8rOEkA8&q=c%23+select+found+text+in+textbox&oq=c%23+select+found+text+in+textbox&gs_l=psy-ab.3..33i22i29i30l6.138636.149771..151070...2.2..0.462.11327.3-4j23......0....1..gws-wiz.......0i71j0j0i22i30.3S72oBLUidA&ved=0ahUKEwiBv737x5nkAhWY73MBHfIZAfIQ4dUDCAo&uact=5#kpvalbx=_7ShgXenmLdrXz7sP7pSusAg19
            //https://stackoverflow.com/questions/9682800/search-specified-string-inside-textbox
            //Thread.Sleep(100);
            //if (string.IsNullOrEmpty(THTargetTextBox.Text))
            //{

            //}
            //else
            //{
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
            int tableindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][0].ToString());
            int rowindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][1].ToString());

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
    }
}
