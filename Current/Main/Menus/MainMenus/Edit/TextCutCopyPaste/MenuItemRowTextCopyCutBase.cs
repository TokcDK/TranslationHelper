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
            })
            {
                if (!(control is TextBoxBase tb)) continue;
                if (!tb.Focused) continue;
                if (!ActionForTextBoxObject(tb)) continue;

                return;
            }

            ActionForSelectedRows();
        }

        protected virtual bool ActionForTextBoxObject(TextBoxBase tb)
        { return false; }

        protected virtual void ActionForSelectedRows() { }

        public override int Order => base.Order + 1000;
    }
}
