using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Functions
{
    static class FunctionsMessage
    {
        internal static bool ShowConfirmationDialog(string QuestionMessage, string Title)
        {
            DialogResult result = MessageBox.Show(QuestionMessage, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
