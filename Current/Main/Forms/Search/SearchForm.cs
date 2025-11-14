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

            searchData = new SearchHelper();
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
