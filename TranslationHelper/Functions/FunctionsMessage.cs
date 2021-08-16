using System.Windows.Forms;

namespace TranslationHelper.Functions
{
    static class FunctionsMessage
    {
        internal static bool ShowConfirmationDialog(string questionMessage, string title)
        {
            DialogResult result = MessageBox.Show(questionMessage, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
