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
        }
    }
}