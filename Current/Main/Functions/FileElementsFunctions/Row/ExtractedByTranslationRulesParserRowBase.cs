﻿using System.Collections.Generic;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExtractedParser
{
    internal abstract class ExtractedByTranslationRulesParserRowBase : RowBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extractOrigValueInfo">Value for check purposes</param>
        /// <param name="extractedTransValueInfo">value which will be changed</param>
        /// <returns>String which will be set to translation of <seealso cref="extractedTransValueInfo"/></returns>
        protected abstract string ActionWithExtractedTranslation(ExtractRegexValueInfo extractOrigValueInfo, ExtractRegexValueInfo extractedTransValueInfo);
        protected abstract string ActionWithOriginalIfNoExtracted(string original, string translation);

        protected bool IsExtracted = false;

        // RoweBase
        protected override bool Apply(RowBaseRowData rowData)
        {
            var orig = rowData.Original;
            var trans = rowData.Translation;

            if (string.IsNullOrWhiteSpace(trans)) return false;

            var etractDataOrig = new ExtractRegexInfo(orig);
            var etractDataTrans = new ExtractRegexInfo(trans);
            var etractDataTransExtractedValuesListCount = etractDataTrans.ExtractedValuesList.Count;

            bool isChanged = false;
            var newValue = trans;
            if (etractDataTransExtractedValuesListCount > 0 && etractDataOrig.ExtractedValuesList.Count == etractDataTransExtractedValuesListCount)
            {
                int i = -1;
                foreach (var extractedTransValueInfo in etractDataTrans.ExtractedValuesList)
                {
                    i++;
                    var wxtractedOrigValueInfo = etractDataOrig.ExtractedValuesList[i];
                    if (extractedTransValueInfo.Original == wxtractedOrigValueInfo.Original) continue; // do not change when equal to original

                    IsExtracted = true;
                    extractedTransValueInfo.Translation = ActionWithExtractedTranslation(wxtractedOrigValueInfo, extractedTransValueInfo);

                    if (extractedTransValueInfo.Translation != extractedTransValueInfo.Original) isChanged = true;
                }

                if (!isChanged) return false;

                var replacedStartIndexes = new List<int>();
                foreach (var (group, info) in etractDataTrans.GetByGroupIndex(isReversed: true))
                {
                    if (info.Translation == null || info.Translation == info.Original) continue;

                    var index = group.Index;
                    if (replacedStartIndexes.Contains(index)) continue; // this shorter group match was inside of other group with same start index

                    newValue = newValue.Remove(index, group.Length)
                        .Insert(index, info.Translation);

                    replacedStartIndexes.Add(index);
                }
            }
            else
            {
                IsExtracted = false;
                newValue = ActionWithOriginalIfNoExtracted(orig, trans);
            }

            if (newValue == trans) return false;

            rowData.Translation = newValue;

            return true;
        }
    }
}
