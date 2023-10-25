using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Menus.MainMenus.Edit.TextCutCopyPaste
{
    internal abstract class MenuItemRowTextCopyCutBase : MainMenuEditSubItemBase
    {
        public override void OnClick(object sender, EventArgs e)
        {
            foreach (Control control in new[]
            {
                AppData.Main.THInfoTextBox,
                AppData.Main.THSourceRichTextBox,
                AppData.Main.THFileElementsDataGridView.EditingControl,
                AppData.Main.THFiltersDataGridView.EditingControl,
            })
            {
                if(control is TextBox tb 
                    && tb.Focused 
                    && ActionForTextBoxObject(tb))
                {
                    return;
                }
            }

            ActionForSelectedRows();
        }

        protected virtual bool ActionForTextBoxObject(TextBoxBase tb)
        { return false; }

        protected virtual void ActionForSelectedRows() { }
    }
}
