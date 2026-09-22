using System;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Menus.MainMenus.Edit.TextCutCopyPaste
{
    internal abstract class MenuItemRowTextCopyCutBase : MainMenuEditSubItemBase
    {
        public override void OnClick(object sender, EventArgs e)
        {
            //The controls of the project on screen. The shortcut acts on whatever holds the focus, and
            //the focused control is one of the selected project's, so the text boxes and the grid of
            //another open project are not candidates for it.
            var fileWorkspace = AppData.ActiveWorkspace?.ActiveFileWorkspace;

            foreach (Control control in new Control[]
            {
                fileWorkspace?.THInfoTextBox,
                fileWorkspace?.SourceRichTextBox,
                fileWorkspace?.ElementsDataGridView?.EditingControl,
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
