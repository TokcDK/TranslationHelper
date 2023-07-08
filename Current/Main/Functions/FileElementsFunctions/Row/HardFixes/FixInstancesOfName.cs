using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixInstancesOfName : RowBase
    {
        Dictionary<string, string> _cache;
        bool needInit = true;
        protected override void ActionsPreRowsApply(TableData tableData)
        {
            if (needInit)
            {
                needInit = false;
                _cache = new Dictionary<string, string>(AppData.CurrentProject.FilesContent.GetRowsCount());
            }
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            var orig = rowData.Original;

            if (orig.IsMultiline()) // skip multiline
            {
                return false;
            }

            var trans = rowData.Translation;

            if (_cache.ContainsKey(orig))
            {
                var cachedTrans = _cache[orig];
                if (orig != cachedTrans)
                {
                    rowData.SelectedRow[1] = cachedTrans;
                }
            }

            var extractedFromOrig = orig.ExtractMulty(onlyOne: true);
            if (string.IsNullOrWhiteSpace(extractedFromOrig[0]) || extractedFromOrig[0].Trim() == orig)
            {
                _cache.TryAdd(orig, trans);
                return false;
            }

            var indexes = new List<int>(1);
            var extractedFromTrans = trans.ExtractMulty(onlyOne: true, outIndexes: indexes);
            if (string.IsNullOrWhiteSpace(extractedFromTrans[0])
                || extractedFromTrans[0].Trim() == trans)
            {
                _cache.TryAdd(orig, trans);
                return false;
            }

            if (_cache.ContainsKey(extractedFromOrig[0]))
            {
                var cachedTranslation = _cache[extractedFromOrig[0]];
                if (cachedTranslation != extractedFromTrans[0]) // replace translation if it is not equals of same in cache
                {
                    rowData.Translation = trans
                    .Remove(indexes[0], extractedFromTrans[0].Length)
                    .Insert(indexes[0], cachedTranslation);
                }
            }
            else
            {
                _cache.TryAdd(extractedFromOrig[0], extractedFromTrans[0]); // add translation to cache
            }

            return true;
        }
    }
}
