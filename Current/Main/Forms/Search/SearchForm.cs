using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Forms.Search
{
    public partial class SearchForm : Form
    {
        readonly SearchHelper searchData;

        public SearchForm(object[] args)
        {
            InitializeComponent();

            if(args.Length != 3)
            {
                throw new ArgumentException("Invalid number of arguments");
            }

            if(!(args[1] is DataGridView dgv))
            {
                throw new ArgumentException("Expect  the second argument to be DatagridView");
            }

            var columns = dgv.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText).ToArray();
            searchData = new SearchHelper(columns);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {

        }

        private void SearchAllButton_Click(object sender, EventArgs e)
        {

        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {

        }

        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {

        }
    }
}
