using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Functions.UndoRedo
{
    public class UndoRedoCommandChangeTextBoxText : IUndoRedoCommand
    {
        private readonly string _oldText;
        private readonly string _newText;
        private readonly TextBox _textBox;

        public UndoRedoCommandChangeTextBoxText(TextBox textBox, string newText)
        {
            _textBox = textBox;
            _oldText = _textBox.Text;
            _newText = newText;
        }

        public void Execute()
        {
            _textBox.Text = _newText;
        }

        public void Undo()
        {
            _textBox.Text = _oldText;
        }
    }
}
