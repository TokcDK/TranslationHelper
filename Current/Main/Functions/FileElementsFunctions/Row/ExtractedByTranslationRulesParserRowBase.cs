using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExtractedParser
{
    internal abstract class ExtractedByTranslationRulesParserRowBase : RowBase
    {
        protected abstract string ActionWithExtracted(ExtractRegexValueInfo extractedValueInfo);
        protected abstract string ActionWithOriginalIfNoExtracted(string original, string translation);

        // RoweBase
        protected override bool Apply()
        {
            var orig = SelectedRow[0] as string;
            var trans = SelectedRow[1] + string.Empty;

            if (string.IsNullOrWhiteSpace(trans)) return false;

            var etractDataOrig = new ExtractRegexInfo(orig);
            var etractDataTrans = new ExtractRegexInfo(trans);
            var etractDataTransExtractedValuesListCount = etractDataTrans.ExtractedValuesList.Count;

            bool isChanged = false;
            if (etractDataOrig.ExtractedValuesList.Count == etractDataTransExtractedValuesListCount)
            {
                foreach (var extractedValueInfo in etractDataTrans.ExtractedValuesList)
                {
                    extractedValueInfo.Translation = ActionWithExtracted(extractedValueInfo);

                    if (extractedValueInfo.Translation != extractedValueInfo.Original) isChanged = true;
                }

                if (!isChanged) return false;

                var newValue = trans;
                var replacedStartIndexes = new List<int>();
                foreach (var info in etractDataTrans.GetByGroupIndex(isReversed: true))
                {
                    if (info.Value.Translation == info.Value.Original) continue;

                    var index = info.Key.Index;
                    if (replacedStartIndexes.Contains(index)) continue; // this shorter group match was inside of other group with same start index

                    newValue = newValue.Remove(index, info.Key.Length)
                        .Insert(index, info.Value.Translation);

                    replacedStartIndexes.Add(index);
                }

                if (newValue == trans) return false;

                return true;
            }

            var changedTrans = ActionWithOriginalIfNoExtracted(orig, trans);

            if (changedTrans == orig) return false;

            return true;
        }
    }
}
