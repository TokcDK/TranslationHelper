using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class FixInstancesOfNameChanger : StringChangerBase
    {
        //The capacity used to be the row count of the whole project: an O(rows) computation and a full
        //size bucket array, paid once per instance while a new instance is created for every row.
        readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        internal override string Description => $"{nameof(FixInstancesOfNameChanger)}";

        internal override string Change(string inputString, object extraData)
        {
            var orig = extraData as string;

            //A null original (a DBNull cell) used to throw inside IsMultiline; leave such a row alone.
            if (orig == null) return inputString;

            if (orig.IsMultiline()) // skip multiline
            {
                return inputString;
            }

            var trans = inputString;

            //if (_cache.ContainsKey(orig))
            //{
            //    var cachedTrans = _cache[orig];
            //    if (orig != cachedTrans)
            //    {
            //        rowData.SelectedRow[1] = cachedTrans;
            //    }
            //}

            var extractedFromOrig = orig.ExtractMulty(onlyOne: true);
            if (string.IsNullOrWhiteSpace(extractedFromOrig[0]) || extractedFromOrig[0].Trim() == orig)
            {
                _cache.TryAdd(orig, trans);
                return inputString;
            }

            var indexes = new List<int>(1);
            var extractedFromTrans = trans.ExtractMulty(onlyOne: true, outIndexes: indexes);
            if (string.IsNullOrWhiteSpace(extractedFromTrans[0])
                || extractedFromTrans[0].Trim() == trans)
            {
                _cache.TryAdd(orig, trans);
                return inputString;
            }

            if (_cache.ContainsKey(extractedFromOrig[0]))
            {
                var cachedTranslation = _cache[extractedFromOrig[0]];
                if (cachedTranslation != extractedFromTrans[0]) // replace translation if it is not equals of same in cache
                {
                    return trans
                    .Remove(indexes[0], extractedFromTrans[0].Length)
                    .Insert(indexes[0], cachedTranslation);
                }
            }
            else
            {
                _cache.TryAdd(extractedFromOrig[0], extractedFromTrans[0]); // add translation to cache
            }

            return inputString;
        }
    }
}
