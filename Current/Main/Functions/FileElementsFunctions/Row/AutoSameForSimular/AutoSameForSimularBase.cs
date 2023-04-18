using System;
using System.Data;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class AutoSameForSimularBase : RowBase
    {
        protected override bool IsValidRow()
        {
            return !string.IsNullOrEmpty(Translation); // not empty original translation
        }

        protected virtual bool IsForce => false;
        protected override bool Apply()
        {
            Set();

            return true;
        }

        private async void Set()
        {
            await Task.Run(() => AutoSameForSimularUtils.Set(inputTableIndex: SelectedTableIndex, inputRowIndex: SelectedRowIndex, inputForceSetValue: IsForce)).ConfigureAwait(false);

        }

        class SameFormSimilarData
        {

        }

        void SameForSimilar()
        {
            var originalCellValue = Original;
            var translatedCellValue = Translation;


            var tables = AppData.CurrentProject.FilesContent.Tables;

            // parse same originals when disable option donot load duplicates
            SetDuplicatesByCoordinates(tables, originalCellValue, translatedCellValue);

            if (originalCellValue == translatedCellValue) return;

            // set for similar originals using multi extraction
            var extractDataOriginal = new ExtractRegexInfo(originalCellValue);
            var extractDataTranslation = new ExtractRegexInfo(translatedCellValue);

            var extractedDataOriginalCount = extractDataOriginal.ExtractedValuesList.Count;
            if (extractedDataOriginalCount != extractDataTranslation.ExtractedValuesList.Count)
            {
                return;
            }

            switch (extractedDataOriginalCount)
            {
                case 0:
                    break;
                case 1:
                    CheckTablesWithOneExtractedValue(tables);
                    break;
            }

            var tablesCount = tables.Count;
            for (int i = 0; i < extractDataOriginal.ExtractedValuesList.Count; i++)
            {
                var originalExtractedValueInfo = extractDataOriginal.ExtractedValuesList[i];
                var translationExtractedValueInfo = extractDataTranslation.ExtractedValuesList[i];

                foreach(DataTable tableToCheck in tables)
                {
                    foreach (DataRow rowToCheck in tableToCheck.Rows)
                    {
                        var originalToCheck = (string)rowToCheck[AppData.CurrentProject.OriginalColumnIndex];
                        var translationToCheck = (string)rowToCheck[AppData.CurrentProject.TranslationColumnIndex];

                        if (!string.IsNullOrEmpty(originalToCheck) || originalToCheck == translationToCheck) continue;

                        var originalToCheckExtracted = originalToCheck.ExtractMulty();


                    }
                }
            }
        }

        private void CheckTablesWithOneExtractedValue(DataTableCollection tables)
        {
            foreach (DataTable tableToCheck in tables)
            {
                foreach (DataRow rowToCheck in tableToCheck.Rows)
                {
                    var originalToCheck = (string)rowToCheck[AppData.CurrentProject.OriginalColumnIndex];
                    var translationToCheck = (string)rowToCheck[AppData.CurrentProject.TranslationColumnIndex];

                    if (!string.IsNullOrEmpty(originalToCheck) || originalToCheck == translationToCheck) continue;

                    var originalToCheckExtracted = new ExtractRegexInfo(originalToCheck);
                    if (originalToCheckExtracted.ExtractedValuesList.Count != 1) continue;


                }
            }
        }

        private void SetDuplicatesByCoordinates(DataTableCollection tables, string originalCellValue, string translatedCellValue)
        {
            bool weUseDuplicates = false;
            try
            {
                weUseDuplicates = !AppData.CurrentProject.DontLoadDuplicates && AppData.CurrentProject.OriginalsTableRowCoordinates != null;
            }
            catch { }
            if (!weUseDuplicates || !AppData.CurrentProject.OriginalsTableRowCoordinates.TryGetValue(originalCellValue, out var tableDatas))
            {
                return;
            }

            foreach (var tableData in tableDatas)
            {
                var table = tables[tableData.Key];
                var rowIndexes = tableData.Value;

                foreach (var rowIndex in rowIndexes)
                {
                    table.Rows[rowIndex][AppData.CurrentProject.TranslationColumnIndex] = translatedCellValue;
                }
            }
        }
    }
}
