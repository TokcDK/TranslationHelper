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
            return !string.IsNullOrWhiteSpace(inputString) && !(Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && inputString.HaveMostOfRomajiOtherChars());
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

        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="TableName"></param>
        internal void SplitTableCellValuesToDictionaryLines(string TableName)
        {
            if (!thDataWork.THFilesElementsDataset.Tables.Contains(TableName))
                return;

            if (TableLines.Count > 0)
            {
                TableLines.Clear();
            }

            foreach (DataRow Row in thDataWork.THFilesElementsDataset.Tables[TableName].Rows)
            {
                string Original;
                string Translation;
                if (TableLines.ContainsKey(Original = Row[0] + string.Empty) || (Translation = Row[1] + string.Empty).Length == 0 || Translation == Original)
                {
                    continue;
                }

                TableLines.Add(Original, Translation);
            }
        }

        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="MakeLinesCountEqual">if true, line count will be made equal in translation before add original else it will be made only for multiline and rigth after line by line check</param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string TableName, bool MakeLinesCountEqual=true)
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
                bool LinesCountisEqual = OriginalLinesCount == TranslationLinesCount;
                if (!LinesCountisEqual && MakeLinesCountEqual)
                {
                    Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                }

                //Сначала добавить полный вариант
                if (!TableLines.ContainsKey(Original) && Translation != Original)
                {
                    TableLines.Add(Original, Translation);
                    if (OriginalLinesCount == 1)
                    {
                        continue;//когда одна строка не тратить время на её разбор
                    }
                }

                if (!MakeLinesCountEqual && OriginalLinesCount > TranslationLinesCount)
                {
                    Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                }

                //string[] OriginalLines = FunctionsString.SplitStringToArray(Original);
                string[] TranslationLines = Translation.GetAllLinesToArray();
                string[] OriginalLines = Original.GetAllLinesToArray();
                List<string> extralines = new List<string>();
                for (int lineNumber = 0; lineNumber < TranslationLinesCount; lineNumber++)
                {
                    if (LinesCountisEqual)
                    {
                        if (!TableLines.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                        {
                            TableLines.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]);
                        }
                    }
                    else
                    {
                        if (lineNumber < OriginalLinesCount - 1)
                        {
                            if (!TableLines.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                            {
                                TableLines.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]);
                            }
                        }
                        else
                        {
                            if (lineNumber == TranslationLinesCount - 1)
                            {
                                if (extralines.Count > 0)
                                {
                                    extralines.Add(TranslationLines[lineNumber]);
                                    string result = string.Join(Environment.NewLine, extralines);
                                    if (!TableLines.ContainsKey(OriginalLines[OriginalLinesCount-1]) && result.Length > 0 && OriginalLines[OriginalLinesCount - 1] != result)
                                    {
                                        TableLines.Add(OriginalLines[OriginalLinesCount - 1], result);
                                    }
                                }
                                else
                                {
                                    if (!TableLines.ContainsKey(OriginalLines[OriginalLinesCount - 1]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[OriginalLinesCount - 1] != TranslationLines[lineNumber])
                                    {
                                        TableLines.Add(OriginalLines[OriginalLinesCount - 1], TranslationLines[lineNumber]);
                                    }
                                }
                            }
                            else
                            {
                                extralines.Add(TranslationLines[lineNumber]);
                            }
                        }
                    }
                }
                //foreach (string line in Original.SplitToLines())
                //{
                //    if (!TableLines.ContainsKey(line) && TranslationLines[lineNumber].Length > 0 && TranslationLines[lineNumber] != line)
                //    {
                //        TableLines.Add(line, TranslationLines[lineNumber]);
                //    }
                //    lineNumber++;
                //}
            }
        }
    }
}
