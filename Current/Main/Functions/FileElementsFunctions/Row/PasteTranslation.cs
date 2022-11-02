using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class PasteTranslation : RowBase
    {
        protected override bool IsValidRow()
        {
            return base.IsValidRow() && (SelectedRow[1] == null || string.IsNullOrEmpty(SelectedRow[1].ToString()));
        }

        string[] _buffer;
        int _bufferLength;
        protected override void Init()
        {
            _buffer = Clipboard.GetText().SplitToLines().ToArray();
            _bufferLength = _buffer.Length;
        }

        int _lineIndex = 0;
        protected override bool Apply()
        {
            var origValue = SelectedRow[0].ToString();
            var selectedRowOriginalLinesCount = origValue.GetLinesCount();
            if (_lineIndex + selectedRowOriginalLinesCount > _bufferLength)
            {
                return false;
            }

            var newLineSymbol = origValue.IndexOf("\r\n") != -1 ? "\r\n" : "\n";

            string translation = string.Join(newLineSymbol, _buffer.GetRange(_lineIndex, selectedRowOriginalLinesCount));
            _lineIndex += selectedRowOriginalLinesCount;
            //bool IsOneLne = selectedRowOriginalLinesCount == 1;
            //for (; _lineIndex < maxIndex; _lineIndex++)
            //{
            //    if (_lineIndex >= _bufferLength)
            //    {
            //        return false;
            //    }

            //    translation += _buffer[_lineIndex];
            //}

            SelectedRow[1] = translation;
            return true;
        }
    }
}
