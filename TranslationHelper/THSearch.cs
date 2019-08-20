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
        private THMain Main = new THMain();
        public THSearch()
        {
            InitializeComponent();
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
        public ListBox THFilesListBox = new ListBox();
        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchFormFindWhatComboBox.Text) && THFilesListBox.SelectedIndex >= 0)
            {
                //MessageBox.Show("srchind1)" + srchind.ToString());
                /*
                if (dGTexts.CurrentCell.RowIndex != srchind)
                {
                    srchind = dGTexts.CurrentCell.RowIndex;//начальная строка
                    MessageBox.Show("srchind2)" + srchind.ToString());
                }*/
                string searchcolumn = "Translation";
                if (SearchMethodTranslationRadioButton.Checked)
                {
                    searchcolumn = "Original";
                }

                if (startrowsearchindex == THFilesElementsDataset.Tables[tableindex].Rows.Count)
                {
                    startrowsearchindex = 0;
                }

                for (/*подразумевает стартовое значение startrowsearchindex, присвоеное выше*/; startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count; startrowsearchindex++)
                {
                    if (IsContainsText(THFilesElementsDataset.Tables[tableindex].Rows[startrowsearchindex][searchcolumn].ToString(), SearchFormFindWhatComboBox.Text))
                    {
                        //MessageBox.Show("srchind3)" + srchind.ToString());
                        //if (tableindex==THFilesListBox.SelectedIndex)
                        //{
                        //}
                        //else
                        //{
                        //    THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[tableindex];
                        //}

                        THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, startrowsearchindex];
                        //THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = THFileElementsDataGridView.SelectedRows[0].Index;


                        if (startrowsearchindex < THFilesElementsDataset.Tables[tableindex].Rows.Count)
                        {
                            startrowsearchindex++;
                        }
                        else
                        {
                            if (SearchRangeTableRadioButton.Checked)
                            {
                                startrowsearchindex = 0;
                            }
                            else
                            {
                                if (tableindex < THFilesElementsDataset.Tables.Count)
                                {
                                    tableindex++;
                                }
                                else
                                {
                                    tableindex = 0;
                                }

                                startrowsearchindex = 0;

                            }
                        }
                        //MessageBox.Show("srchind4)" + srchind.ToString());
                        return;

                    }
                    //MessageBox.Show("srchind5)" + srchind.ToString());
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
            //if (oDsResults == null)
            //{
            //}
            //else
            //{
            //    SearchResultsDatagridview.DataSource = oDsResults.Tables[0];
            //    SearchResultsDatagridview.Visible = true;
            //}
        }

        //http://mrbool.com/dataset-advance-operations-search-sort-filter-net/24769
        //https://stackoverflow.com/questions/3608388/c-sharp-access-dataset-data-from-another-class
        //http://qaru.site/questions/236566/how-to-know-the-row-index-from-datatable-object
        public DataSet THFilesElementsDataset = new DataSet();
        public DataGridView THFileElementsDataGridView = new DataGridView();
        private void FindAllButton_Click(object sender, EventArgs e)
        {
            DataSet oDsResults = THFilesElementsDataset.Clone();

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
                }
                else
                {
                    PopulateGrid(null);
                    lblError.Visible = true;
                    lblError.Text = "Nothing Found.";
                }
            }


        }

        private DataTable SelectFromDatatables(DataSet oDsResults)
        {
            //Check for user input
            if (string.IsNullOrEmpty(SearchFormFindWhatComboBox.Text.Trim()))
            {
                //lblError.Visible = true;
                //lblError.Text = "Please fill criteria before search";
            }
            else
            {
                DataRow[] drFilterRows;
                //MessageBox.Show("tables cnt="+ oDs.Tables.Count);
                //Check if table exist
                if (THFilesElementsDataset != null && THFilesElementsDataset.Tables.Count > 0)
                {
                    for (int t=0;t< THFilesElementsDataset.Tables.Count; t++)
                    {
                        string strQuery = "[" + THFilesElementsDataset.Tables[t].Columns[1].ColumnName + "] Like '%" + SearchFormFindWhatComboBox.Text.Replace("'", "''").Replace("*", "[*]").Replace("%", "[%]").Replace("[", "-QB[BQ-").Replace("]", "[]]").Replace("-QB[BQ-", "[[]") + "%'";
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
            tableindex = THFilesListBox.SelectedIndex;
        }
    }
}
