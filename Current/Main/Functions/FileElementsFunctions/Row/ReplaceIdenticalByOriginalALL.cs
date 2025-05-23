﻿using System.Collections.Generic;
using System.Windows.Documents;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Replace all identical translations with original text using the list
    /// </summary>
    class ReplaceIdenticalByOriginalALL : RowBase
    {
        public override string Name => "Replace identical by original";

        // List to hold the list of items to replace 
        readonly HashSet<string> _listToReplace = new HashSet<string>();
        readonly bool _isActive = false;

        public ReplaceIdenticalByOriginalALL(HashSet<string> listToReplace)
        {
            _listToReplace = listToReplace;
            _isActive = _listToReplace.Count > 0;
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            if (!_isActive) return false;

            bool haveOriginal = _listToReplace.Contains(rowData.Original);
            if ((_listToReplace.Contains(rowData.Original)
                && rowData.Original == rowData.Translation) || !haveOriginal
                ) return false;

            return true;
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            rowData.Translation = rowData.Original;

            return true;
        }
    }
}
