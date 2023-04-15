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
            return !string.IsNullOrEmpty(SelectedRow[1] + ""); // not empty original translation
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
            var originalCellValue = (string)SelectedRow[AppData.CurrentProject.OriginalColumnIndex];
            var translatedCellValue = (string)SelectedRow[AppData.CurrentProject.TranslationColumnIndex];


            var tables = AppData.CurrentProject.FilesContent.Tables;

            // parse same originals when disable option donot load duplicates
            SetDuplicatesByCoordinates(tables, originalCellValue, translatedCellValue);

            if (originalCellValue == translatedCellValue) return;

            // set for similar originals using multi extraction
            var extractDataOriginal = originalCellValue.ExtractMulty();
            var extractDataTranslation = translatedCellValue.ExtractMulty();

            var extractedDataOriginalCount = extractDataOriginal.ValueDataList.Count;
            if (extractedDataOriginalCount != extractDataTranslation.ValueDataList.Count)
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
            for (int i = 0; i < extractDataOriginal.ValueDataList.Count; i++)
            {
                var originalExtractedValueInfo = extractDataOriginal.ValueDataList[i];
                var translationExtractedValueInfo = extractDataTranslation.ValueDataList[i];

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

                    var originalToCheckExtracted = originalToCheck.ExtractMulty();
                    if (originalToCheckExtracted.ValueDataList.Count != 1) continue;


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
