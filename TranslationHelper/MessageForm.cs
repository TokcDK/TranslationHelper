using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//sources
//https://www.youtube.com/watch?v=MkFE_pM7jOc
//https://stackoverflow.com/questions/6932792/how-to-create-a-custom-messagebox
//https://stackoverflow.com/questions/4343730/how-do-i-keep-a-label-centered-in-winforms
namespace TranslationHelper
{
    public partial class THMsg : Form
    {
        public THMsg(string message)
        {
            InitializeComponent();
            THMessageFormMessageLabel.Text = message;
        }

        public static bool Show(string message)
        {
            //THMessageFormMessageLabel.Text = message;
            using (var form = new THMsg(message))
            {
                form.ShowDialog();
            }
            return true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

