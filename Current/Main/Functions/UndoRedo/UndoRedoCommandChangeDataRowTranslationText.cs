using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.UndoRedo
{
    public class UndoRedoCommandChangeDataRowTranslationText : IUndoRedoCommand
    {
        private readonly string _oldText;
        private readonly string _newText;
        private readonly int _index;
        private readonly DataRow _obj;

        public UndoRedoCommandChangeDataRowTranslationText(DataRow row, string newText)
        {
            _obj = row;
            _index = AppData.CurrentProject.TranslationColumnIndex;
            _oldText = _obj.Field<string>(_index);
            _newText = newText;
        }

        public void Execute()
        {
            _obj.SetField(_index, _newText);
        }

        public void Undo()
        {
            _obj.SetField(_index, _oldText);
        }
    }
}
