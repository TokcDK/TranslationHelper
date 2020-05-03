using System;
using System.Collections.Generic;
using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats
{
    abstract class FormatBase
    {
        protected THDataWork thDataWork;

        protected FormatBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal virtual bool Detect()
        {
            return false;
        }

        internal abstract bool Open();

        internal abstract bool Save();

        protected bool ValidString(string inputString)
        {
            return !string.IsNullOrWhiteSpace(inputString) && !(Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && FunctionsRomajiKana.SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(inputString));
        }

        protected void AddRowData(string tablename, string RowData, string RowInfo, bool CheckInput = true, bool AddToDictionary = false)
        {
            if (CheckInput && !ValidString(RowData))
            {
                return;
            }

            if (AddToDictionary)
            {
                if (!thDataWork.THFilesElementsDictionary.ContainsKey(RowData))
                {
                    thDataWork.THFilesElementsDictionary.Add(RowData, string.Empty);
                    thDataWork.THFilesElementsDictionaryInfo.Add(RowData, RowInfo);
                }
            }
            else
            {
                thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Add(RowData);
                thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add(RowInfo);
            }
        }

        protected void AddTables(string tablename, string[] extraColumns = null)
        {
            if (!thDataWork.THFilesElementsDataset.Tables.Contains(tablename))
            {
                thDataWork.THFilesElementsDataset.Tables.Add(tablename);
                thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add("Original");
                thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add("Translation");

                if (extraColumns != null && extraColumns.Length > 0)
                {
                    foreach (var columnName in extraColumns)
                    {
                        thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add(columnName);
                    }
                }
            }
            if (!thDataWork.THFilesElementsDatasetInfo.Tables.Contains(tablename))
            {
                thDataWork.THFilesElementsDatasetInfo.Tables.Add(tablename);
                thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Columns.Add("Info");
            }
        }

        protected bool CheckTablesContent(string tablename, bool IsDictionary = false)
        {
            if (IsDictionary && thDataWork.THFilesElementsDictionary != null && thDataWork.THFilesElementsDictionary.Count > 0 && thDataWork.THFilesElementsDataset.Tables[tablename] != null && thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Count == 0)
            {
                return thDataWork.THFilesElementsDataset.Tables[tablename].FillWithDictionary(thDataWork.THFilesElementsDictionary);
            }
            else if (thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                if (thDataWork.THFilesElementsDataset.Tables.Contains(tablename))
                {
                    thDataWork.THFilesElementsDataset.Tables.Remove(tablename); // remove table if was no items added
                }

                if (thDataWork.THFilesElementsDatasetInfo.Tables.Contains(tablename))
                {
                    thDataWork.THFilesElementsDatasetInfo.Tables.Remove(tablename); // remove table if was no items added
                }

                return false;
            }
        }

        protected Dictionary<string, string> TableLines = new Dictionary<string, string>();
        protected void SplitTableCellValuesToDictionaryLines(string TableName)
        {
            if (!thDataWork.THFilesElementsDataset.Tables.Contains(TableName))
                return;

            if (TableLines.Count > 0)
            {
                TableLines.Clear();
            }

            foreach (DataRow Row in thDataWork.THFilesElementsDataset.Tables[TableName].Rows)
            {
                string Original = (Row[0] + string.Empty);
                int OriginalLinesCount = Original.GetLinesCount();
                if (OriginalLinesCount == 1 && TableLines.ContainsKey(Original))
                {
                    continue;
                }

                string Translation = (Row[1] + string.Empty);
                if (Translation.Length == 0)
                {
                    continue;
                }

                int TranslationLinesCount = Translation.GetLinesCount();
                if (OriginalLinesCount != TranslationLinesCount)
                {
                    Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                }

                //string[] OriginalLines = FunctionsString.SplitStringToArray(Original);
                string[] TranslationLines = FunctionsString.SplitStringToArray(Translation);

                int lineNumber = 0;
                foreach (string line in Original.SplitToLines())
                {
                    if (!TableLines.ContainsKey(line) && TranslationLines[lineNumber].Length > 0)
                    {
                        TableLines.Add(line, TranslationLines[lineNumber]);
                    }
                    lineNumber++;
                }
            }
        }
    }
}
