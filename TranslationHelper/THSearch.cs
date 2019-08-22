using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THSearch : Form
    {
        private THMain Main;
        public ListBox THFilesListBox;
        public DataSet THFilesElementsDataset;
        public DataGridView THFileElementsDataGridView;
        public THSearch(THMain MainForm, DataSet DS, ListBox listBox, DataGridView DGV)
        {
            InitializeComponent();
            Main = MainForm;
            THFilesListBox = listBox;
            THFilesElementsDataset = DS;
            THFileElementsDataGridView = DGV;
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

        int startrowsearchindex;        //Индекс стартовой ячейки для поиска
        int tableindex;
        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchFormFindWhatComboBox.Text) && THFilesListBox.SelectedIndex >= 0)
            {

                string searchcolumn = "Translation";
                if (SearchMethodTranslationRadioButton.Checked)
                {
                    searchcolumn = "Original";
                }

                if (SearchRangeTableRadioButton.Checked)
                {
                    //FileWriter.WriteData("c:\\logsearch.log", "\r\n0 0 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                    if (startrowsearchindex == THFilesElementsDataset.Tables[tableindex].Rows.Count)
                    {
                        startrowsearchindex = 0;
                    }

                    for (/*подразумевает стартовое значение startrowsearchindex, присвоеное выше*/; startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count; startrowsearchindex++)
                    {
                        if (IsContainsText(THFilesElementsDataset.Tables[tableindex].Rows[startrowsearchindex][searchcolumn].ToString(), SearchFormFindWhatComboBox.Text))
                        {
                            //FileWriter.WriteData("c:\\logsearch.log", "\r\n1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                            //MessageBox.Show("srchind3)" + srchind.ToString());

                            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, startrowsearchindex];
                            //THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = THFileElementsDataGridView.SelectedRows[0].Index;


                            //FileWriter.WriteData("c:\\logsearch.log", "\r\n2 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                            if (startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count)
                            {
                                //FileWriter.WriteData("c:\\logsearch.log", "\r\n2.1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                                startrowsearchindex++;
                            }
                            else
                            {
                                startrowsearchindex = 0;
                            }
                            //MessageBox.Show("srchind4)" + srchind.ToString());
                            //FileWriter.WriteData("c:\\logsearch.log", "\r\n111 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                            return;
                        }
                        //FileWriter.WriteData("c:\\logsearch.log", "\r\n222 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                        //MessageBox.Show("srchind5)" + srchind.ToString());
                    }
                }
                else
                {
                    //if (tableindex == THFilesElementsDataset.Tables.Count)
                    //{
                    //    tableindex = 0;
                    //}
                    //if (startrowsearchindex == THFilesElementsDataset.Tables[tableindex].Rows.Count)
                    //{
                    //    if (tableindex < THFilesElementsDataset.Tables.Count-1)
                    //    {
                    //        tableindex++;
                    //    }
                    //    startrowsearchindex = 0;
                    //}

                    for (; tableindex < THFilesElementsDataset.Tables.Count; tableindex++)
                    {
                        //FileWriter.WriteData("c:\\logsearch.log", "\r\n0 0 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                        

                        for (/*подразумевает стартовое значение startrowsearchindex, присвоеное выше*/; startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count; startrowsearchindex++)
                        {
                            if (IsContainsText(THFilesElementsDataset.Tables[tableindex].Rows[startrowsearchindex][searchcolumn].ToString(), SearchFormFindWhatComboBox.Text))
                            {
                                //FileWriter.WriteData("c:\\logsearch.log", "\r\n1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                                //MessageBox.Show("srchind3)" + srchind.ToString());
                                if (tableindex == THFilesListBox.SelectedIndex)
                                {
                                }
                                else
                                {
                                    THFilesListBox.SelectedIndex = tableindex;
                                    THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                                }

                                THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, startrowsearchindex];
                                //THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = THFileElementsDataGridView.SelectedRows[0].Index;


                                //FileWriter.WriteData("c:\\logsearch.log", "\r\n2 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                                if (startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count)
                                {
                                    //FileWriter.WriteData("c:\\logsearch.log", "\r\n2.1 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                                    startrowsearchindex++;
                                    if (startrowsearchindex == THFilesElementsDataset.Tables[tableindex].Rows.Count)
                                    {
                                        startrowsearchindex = 0;
                                        //if (tableindex == THFilesElementsDataset.Tables.Count-1)
                                        //{
                                        //    tableindex = 0;
                                        //}
                                    }
                                }
                                //MessageBox.Show("srchind4)" + srchind.ToString());
                                //FileWriter.WriteData("c:\\logsearch.log", "\r\n111 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                                return;
                            }
                            //FileWriter.WriteData("c:\\logsearch.log", "\r\n222 tableindex=" + tableindex + ", startrowsearchindex" + startrowsearchindex);
                            //MessageBox.Show("srchind5)" + srchind.ToString());
                        }
                    }
                }
            }
        }

        private bool IsContainsText(string searchobject, string searchinput)
        {
            if (THSearchMatchCaseCheckBox.Checked)
            {
                return searchobject.Contains(SearchFormFindWhatComboBox.Text);
            }
            else
            {
                return searchobject.ToLowerInvariant().Contains(SearchFormFindWhatComboBox.Text.ToLowerInvariant());
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

        private DataTable SelectFromDatatables(DataSet oDsResults)
        {
            //Check for user input
            if (string.IsNullOrEmpty(SearchFormFindWhatComboBox.Text.Trim()) || THFilesElementsDataset == null)
            {
                //lblError.Visible = true;
                //lblError.Text = "Please fill criteria before search";
            }
            else if (THFilesElementsDataset.Tables.Count > 0)
            {
                DataRow[] drFilterRows;
                string searchcolumn = "Translation";
                if (SearchMethodTranslationRadioButton.Checked)
                {
                    searchcolumn = "Original";
                }
                string strQuery = "[" + THFilesElementsDataset.Tables[0].Columns[searchcolumn].ColumnName + "] Like '%" + SearchFormFindWhatComboBox.Text.Replace("'", "''").Replace("*", "[*]").Replace("%", "[%]").Replace("[", "-QB[BQ-").Replace("]", "[]]").Replace("-QB[BQ-", "[[]") + "%'";

                if (SearchRangeTableRadioButton.Checked)
                {
                    drFilterRows = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Select(strQuery);

                    if (drFilterRows.Length > 0)
                    {
                        foreach (DataRow dr in drFilterRows)
                        {
                            oDsResults.Tables[0].ImportRow(dr);
                        }
                    }

                    //MessageBox.Show(oDsResults.Tables[0].Rows[0][1].ToString());
                    return oDsResults.Tables[0];
                }
                else
                {
                    for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
                    {
                        drFilterRows = THFilesElementsDataset.Tables[t].Select(strQuery);

                        if (drFilterRows.Length > 0)
                        {
                            foreach (DataRow dr in drFilterRows)
                            {
                                oDsResults.Tables[0].ImportRow(dr);
                            }
                        }
                    }

                    //MessageBox.Show(oDsResults.Tables[0].Rows[0][1].ToString());
                    return oDsResults.Tables[0];
                }
            }
            return null;
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://patreon.com/TranslationHelper");
        }

        private void THSearch_Load(object sender, EventArgs e)
        {
            //some other info: https://stackoverflow.com/questions/20893725/how-to-hide-and-show-panels-on-a-form-and-have-it-resize-to-take-up-slack
            this.Height = 368;
            tableindex = THFilesListBox.SelectedIndex;
        }

        private void SearchFormReplaceButton_Click(object sender, EventArgs e)
        {
            //Main.BindToDataTableGridView(THFilesElementsDataset.Tables[tableindex]);
            THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[0];
        }

        private void SearchResultsDatagridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int SelectedIndex;
            for (int t=0;t< THFilesElementsDataset.Tables.Count; t++)
            {
                for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
                {
                    if (THFilesElementsDataset.Tables[t].Rows[r][0].ToString()== oDsResults.Tables[0].Rows[e.RowIndex][0].ToString())
                    {
                        if (t == THFilesListBox.SelectedIndex)
                        {
                        }
                        else
                        {
                            THFilesListBox.SelectedIndex = t;
                            THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[t];
                        }
                        THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[0, r];
                        return;
                    }
                }


                ////http://qaru.site/questions/236566/how-to-know-the-row-index-from-datatable-object
                //SelectedIndex = THFilesElementsDataset.Tables[t].Rows.IndexOf(oDsResults.Tables[0].Rows[e.RowIndex]);
                //FileWriter.WriteData(@"c:\\Search1.log", "\r\nSelectedIndex="+ SelectedIndex+"\r\nValue="+ oDsResults.Tables[0].Rows[e.RowIndex][0].ToString());
                //if (SelectedIndex >= 0)
                //{
                //    if (t == THFilesListBox.SelectedIndex)
                //    {
                //    }
                //    else
                //    {
                //        THFilesListBox.SelectedIndex = t;
                //        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[t];
                //    }
                //    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[0, SelectedIndex];
                //    return;
                //}
            }
            
        }
    }
}
