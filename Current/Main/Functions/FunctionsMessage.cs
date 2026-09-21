using System.Windows.Forms;
using MessageBox = TranslationHelper.Theming.ThemedMessageBox;

namespace TranslationHelper.Functions
{
    static class FunctionsMessage
    {
        internal static bool ShowConfirmationDialog(string QuestionMessage, string Title)
        {
            DialogResult result = MessageBox.Show(QuestionMessage, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return result == DialogResult.Yes;
        }
    }
}
