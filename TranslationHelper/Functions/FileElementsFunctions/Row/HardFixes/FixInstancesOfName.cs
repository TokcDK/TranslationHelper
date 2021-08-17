﻿using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixInstancesOfName : RowBase
    {
        Dictionary<string, string> _cache;
        bool needInit = true;
        protected override void ActionsPreRowsApply()
        {
            if (needInit)
            {
                needInit = false;
                _cache = new Dictionary<string, string>(ProjectData.THFilesElementsDataset.GetRowsCount());
            }
        }

        protected override bool Apply()
        {
            var orig = SelectedRow[0] as string;
            var trans = SelectedRow[1] + string.Empty;

            if (_cache.ContainsKey(orig))
            {
                if (orig != trans)
                {
                    SelectedRow[1] = trans;
                }
            }

            var extractedFromOrig = orig.ExtractMulty(true);
            if (string.IsNullOrWhiteSpace(extractedFromOrig[0]) || extractedFromOrig[0].Trim() == orig)
            {
                return false;
            }

            var indexes = new List<int>(1);
            var extractedFromTrans = trans.ExtractMulty(onlyOne: true, outIndexes: indexes);
            if (string.IsNullOrWhiteSpace(extractedFromTrans[0])
                || extractedFromTrans[0].Trim() == trans)
            {
                return false;
            }

            if (_cache.ContainsKey(extractedFromOrig[0]))
            {
                var cachedTranslation = _cache[extractedFromOrig[0]];
                if (cachedTranslation != extractedFromTrans[0]) // replace translation if it is not equals of same in cache
                {
                    SelectedRow[1] = trans
                    .Remove(indexes[0], extractedFromTrans[0].Length)
                    .Insert(indexes[0], cachedTranslation);
                }
            }
            else
            {
                _cache.AddTry(extractedFromOrig[0], extractedFromTrans[0]); // add translation to cache
            }

            return true;
        }
    }
}