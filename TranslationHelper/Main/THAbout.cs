using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THAboutForm : Form
    {
        public THAboutForm()
        {
            InitializeComponent();

            //https://stackoverflow.com/questions/3421453/why-is-text-in-textbox-highlighted-selected-when-form-is-displayed
            THAboutTextBox.TabStop = false;//убрать выделение текста после открытия

            //translation
            THAboutTextBox.Text = T._("Translation Helper by DenisK (c) 2019") + Environment.NewLine + Environment.NewLine + T._("Utility which helps in translation of some RPGMaker and other games.") + Environment.NewLine + Environment.NewLine + T._("Currently can help to translate:") + Environment.NewLine + T._("RPG Maker Trans Patch and all games with it") + Environment.NewLine + T._("RPG Maker MV games or they json files with string") + Environment.NewLine + T._("Not encrypted games on KiriKiri engine and they files") + Environment.NewLine + Environment.NewLine + T._("Credits")+":" + Environment.NewLine + T._("Aleph Fell(Habisain) for RPGMakerTrans");
        }

        private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://patreon.com/TranslationHelper");
        }
    }
}