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
            if (ActionForTextBoxObject(AppData.Main.THInfoTextBox))
            {
            }
            else if (ActionForTextBoxObject(AppData.Main.THSourceRichTextBox))
            {
            }
            else if (ActionForTextBoxObject(AppData.Main.THTargetRichTextBox))
            {
            }
            else
            {
                if (AppData.Main.THFileElementsDataGridView.EditingControl is TextBox tb
                    && ActionForTextBoxObject(tb))
                {
                    return;
                }

                ActionForSelectedRows();
            }
        }

        protected virtual bool ActionForTextBoxObject(TextBoxBase tb)
        { return false; }

        protected virtual void ActionForSelectedRows() { }
    }
}
