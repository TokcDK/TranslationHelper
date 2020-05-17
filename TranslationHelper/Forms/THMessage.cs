using System;
using System.Diagnostics;
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

        private void THMessageFormInfoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://patreon.com/TranslationHelper");
        }
    }
}

