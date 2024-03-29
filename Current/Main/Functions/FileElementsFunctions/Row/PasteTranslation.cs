﻿using System.Linq;
using System.Windows.Forms;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class PasteTranslation : RowBase
    {
        public PasteTranslation()
        {
            _buffer = Clipboard.GetText().SplitToLines().ToArray();
            _bufferLength = _buffer.Length;
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return base.IsValidRow(rowData) && (rowData.SelectedRow[1] == null || string.IsNullOrEmpty(rowData.SelectedRow[1].ToString()));
        }

        readonly string[] _buffer;
        readonly int _bufferLength;

        int _lineIndex = 0;

        protected override bool Apply(RowBaseRowData rowData)
        {
            var origValue = rowData.Original;
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

            rowData.Translation = translation;
            return true;
        }
    }
}
