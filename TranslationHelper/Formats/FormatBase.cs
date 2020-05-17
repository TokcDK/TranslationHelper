using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats
{
    abstract class FormatBase
    {
        protected THDataWork thDataWork;

        protected Dictionary<string, string> TablesLinesDict;

        protected FormatBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
            TablesLinesDict = thDataWork.TablesLinesDict;
            hashes = thDataWork.hashes;
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

        protected HashSet<string> hashes;
        protected void AddRowData(string RowData, string RowInfo, bool CheckAddHashes=false)
        {
            if (!string.IsNullOrEmpty(thDataWork.FilePath))
            {
                AddRowData(Path.GetFileName(thDataWork.FilePath), RowData, RowInfo, CheckAddHashes);
            }
        }
        protected void AddRowData(string tablename, string RowData, string RowInfo, bool CheckAddHashes = false, bool CheckInput = true, bool AddToDictionary = false)
        {
            if (CheckInput && !ValidString(RowData))
            {
                return;
            }

            if(CheckAddHashes && hashes != null && hashes.Contains(RowData))
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
                if (CheckAddHashes && hashes != null)
                {
                    hashes.Add(RowData);
                }
            }
        }

        protected void AddTables()
        {
            if (!string.IsNullOrEmpty(thDataWork.FilePath))
            {
                AddTables(Path.GetFileName(thDataWork.FilePath));
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

        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="TableName"></param>
        internal void SplitTableCellValuesToDictionaryLines(string TableName)
        {
            if (!thDataWork.THFilesElementsDataset.Tables.Contains(TableName))
                return;

            if (TablesLinesDict != null && TablesLinesDict.Count > 0)
            {
                TablesLinesDict.Clear();
            }

            foreach (DataRow Row in thDataWork.THFilesElementsDataset.Tables[TableName].Rows)
            {
                string Original;
                string Translation;
                if (TablesLinesDict.ContainsKey(Original = Row[0] + string.Empty) || (Translation = Row[1] + string.Empty).Length == 0 || Translation == Original)
                {
                    continue;
                }

                TablesLinesDict.Add(Original, Translation);
            }
        }

        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="MakeLinesCountEqual">if true, line count will be made equal in translation before add original else it will be made only for multiline and rigth after line by line check</param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string TableName, bool MakeLinesCountEqual = true, bool OnlyOneTable = true)
        {
            if (OnlyOneTable)
            {
                if (!thDataWork.THFilesElementsDataset.Tables.Contains(TableName))
                    return;

                if (TablesLinesDict.Count > 0)
                {
                    TablesLinesDict.Clear();
                }
            }
            else
            {
                if (TablesLinesDict != null && TablesLinesDict.Count > 0)
                {
                    return;
                }
            }


            foreach (DataTable Table in thDataWork.THFilesElementsDataset.Tables)
            {
                if (OnlyOneTable && Table.TableName != TableName)
                {
                    continue;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    string Original = (Row[0] + string.Empty);
                    int OriginalLinesCount = Original.GetLinesCount();
                    if (OriginalLinesCount == 1 && TablesLinesDict.ContainsKey(Original))
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
                    if (!TablesLinesDict.ContainsKey(Original) && Translation != Original)
                    {
                        TablesLinesDict.Add(Original, Translation/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
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
                        try
                        {
                            if (LinesCountisEqual)
                            {
                                if (!TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                                {
                                    TablesLinesDict.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                }
                            }
                            else
                            {
                                if (lineNumber < OriginalLinesCount - 1)
                                {
                                    if (!TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                                    {
                                        TablesLinesDict.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
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
                                            if (!TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1]) && result.Length > 0 && OriginalLines[OriginalLinesCount - 1] != result)
                                            {
                                                TablesLinesDict.Add(OriginalLines[OriginalLinesCount - 1], result/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                            }
                                        }
                                        else
                                        {
                                            if (!TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[OriginalLinesCount - 1] != TranslationLines[lineNumber])
                                            {
                                                TablesLinesDict.Add(OriginalLines[OriginalLinesCount - 1], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
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
                        catch
                        {

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
}
