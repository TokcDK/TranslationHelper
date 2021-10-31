using System;
using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class PasteTranslation : RowBase
    {
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
            var selectedRowOriginalLinesCount = SelectedRow[0].ToString().GetLinesCount();
            if (_lineIndex + selectedRowOriginalLinesCount > _bufferLength)
            {
                return false;
            }

            string translation = string.Join(Environment.NewLine, _buffer.GetRange(_lineIndex, selectedRowOriginalLinesCount));
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
