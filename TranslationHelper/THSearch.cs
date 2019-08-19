using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private void RadioButton1_Click(object sender, EventArgs e)
        {
            SearchModeRegexRadioButton.Checked = false;
            SearchModeNormalRadioButton.Checked = true;
        }

        private void RadioButton2_Click(object sender, EventArgs e)
        {
            SearchModeNormalRadioButton.Checked = false;
            SearchModeRegexRadioButton.Checked = true;
        }

        private void RadioButton4_Click(object sender, EventArgs e)
        {
            SearchMethodTranslationRadioButton.Checked = false;
            SearchMethodOriginalTranslationRadioButton.Checked = true;
        }

        private void RadioButton3_Click(object sender, EventArgs e)
        {
            SearchMethodOriginalTranslationRadioButton.Checked = false;
            SearchMethodTranslationRadioButton.Checked = true;
        }

        private void RadioButton5_Click(object sender, EventArgs e)
        {
            SearchRangeAllRadioButton.Checked = false;
            SearchRangeTableRadioButton.Checked = true;
        }

        private void RadioButton6_Click(object sender, EventArgs e)
        {
            SearchRangeTableRadioButton.Checked = false;
            SearchRangeAllRadioButton.Checked = true;
        }

        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
        }

        private void PopulateGrid(DataSet oDsResults)
        {
            if (oDsResults == null)
            {
            }
            else
            {
                SearchResultsDatagridview.DataSource = oDsResults.Tables[0];
                SearchResultsDatagridview.Visible = true;
            }
        }

        //http://mrbool.com/dataset-advance-operations-search-sort-filter-net/24769
        //https://stackoverflow.com/questions/3608388/c-sharp-access-dataset-data-from-another-class
        public DataSet oDs;
        private void FindAllButton_Click(object sender, EventArgs e)
        {
            //Check for user input
            if (SearchFormFindWhatComboBox.Text.Trim() != "")
            {
                DataSet oDsResults = oDs.Clone();
                //MessageBox.Show("tables cnt="+ oDs.Tables.Count);
                //Check if table exist
                if (oDs != null && oDs.Tables.Count > 0)
                {
                    string strQuery = "[" + oDs.Tables[2].Columns[1].ColumnName + "] Like '%" + SearchFormFindWhatComboBox.Text.Replace("'", "''").Replace("*", "[*]").Replace("%", "[%]").Replace("[", "-QB[BQ-").Replace("]", "[]]").Replace("-QB[BQ-", "[[]") + "%'";
                    DataRow[] drFilterRows = oDs.Tables[2].Select(strQuery);

                    foreach (DataRow dr in drFilterRows)
                        oDsResults.Tables[0].ImportRow(dr);
                    //MessageBox.Show(oDsResults.Tables[0].Rows[0][1].ToString());

                    if (drFilterRows.Length > 0)
                    {
                        oDsResults.AcceptChanges();
                        PopulateGrid(oDsResults);

                        lblError.Visible = true;
                        lblError.Text = "One Record Found.";
                    }
                    else
                    {
                        PopulateGrid(null);
                        lblError.Visible = true;
                        lblError.Text = "No Record Found.";
                    }
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "Please fill criteria before search";
            }

        }
    }
}
