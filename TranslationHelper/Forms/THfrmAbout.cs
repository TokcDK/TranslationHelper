using System;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class HfrmAbout : Form
    {
        public HfrmAbout()
        {
            InitializeComponent();

            THAboutTextBox.TabStop = false;//clear text selection after open

            string games = T._("games");
            //translation
            THAboutTextBox.Text = T._("Translation Helper by DenisK (c) ????(pregit)-2019-2020")
                + Environment.NewLine
                + Environment.NewLine
                + T._("Utility which helps in translation of some RPGMaker and other games.")
                + Environment.NewLine
                + T._("It is merge of some my utilities which was made for help with games translation.")
                + Environment.NewLine
                + Environment.NewLine
                + T._("Currently can help to translate:")
                + Environment.NewLine
                + T._("RPG Maker Trans Patch and supported games by it")
                + Environment.NewLine
                + T._("RPG Maker MV") + " " + games
                + Environment.NewLine
                + T._("WOLFRPG") + " " + games
                + Environment.NewLine
                + T._("KiriKiri") + " " + games
                + Environment.NewLine
                + T._("SCPack EAGLS") + " " + games
                + Environment.NewLine
                + T._("NSPack") + " " + games
                + Environment.NewLine
                + T._("Some standalone") + " " + games
                + Environment.NewLine + Environment.NewLine
                + T._("Credits") + ":"
                + Environment.NewLine
                + "Credits to all Authors of utilties in resource folder and to Authors of all used code in the program"
                ;
        }
    }
}