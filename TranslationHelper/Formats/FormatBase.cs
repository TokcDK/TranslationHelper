using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
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

        internal virtual bool Check()
        {
            return false;
        }

        /// <summary>
        /// extension which can be parsed with the format, ".txt" or ".txt,.csv" for example
        /// </summary>
        /// <returns></returns>
        internal virtual string Ext()
        {
            return "";
        }

        internal virtual bool Open() { return ParseStringFile(); }

        internal virtual bool Save() { return ParseStringFile(); }

        protected bool IsValidString(string inputString)
        {
            //preclean string
            inputString = thDataWork.CurrentProject.CleanStringForCheck(inputString);

            bool jp;
            return !string.IsNullOrWhiteSpace(inputString)
                && (
                ((jp = Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp") && !inputString.HaveMostOfRomajiOtherChars())
                || !jp
                );
        }

        /// <summary>
        /// Parse Data
        /// </summary>
        protected ParseFileData ParseData;
        /// <summary>
        /// Base Parse File function
        /// </summary>
        /// <param name="IsOpen"></param>
        /// <returns></returns>
        protected bool ParseStringFile()
        {
            ParseStringFilePreOpen();

            ParseStringFileOpen();

            return ParseStringFilePostOpen();
        }

        /// <summary>
        /// Open file actions
        /// </summary>
        protected virtual void ParseStringFileOpen()
        {
            using (ParseData.reader = new StreamReader(thDataWork.FilePath, FunctionsFileFolder.GetEncoding(thDataWork.FilePath) ?? DefaultEncoding()))
            {
                ParseStringFileLines();
            }
        }

        /// <summary>
        /// Default encoding for file streamreader
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding DefaultEncoding()
        {
            return Encoding.UTF8;
        }

        /// <summary>
        /// Pre open file actions
        /// </summary>
        protected virtual void ParseStringFilePreOpen()
        {
            ParseData = new ParseFileData(thDataWork);

            if (thDataWork.OpenFileMode)
            {
                AddTables(ParseData.tablename);
            }

            if (thDataWork.SaveFileMode)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(ParseData.tablename, false, false);
            }

            ParseStringFilePreOpenExtra();
        }

        /// <summary>
        /// Pre open file extra actions
        /// </summary>
        protected virtual void ParseStringFilePreOpenExtra()
        {
        }

        protected virtual bool ParseStringFilePostOpen()
        {
            if (thDataWork.OpenFileMode)
            {
                return CheckTablesContent(ParseData.tablename);
            }
            else
            {
                return WriteFileData();
            }
        }

        protected virtual void ParseStringFileLines()
        {
            while (ReadLine() != null)
            {
                if (ParseStringFileLine() == -1)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Parse line virtual function
        /// -1=stop parse cyrcle, 0-continue cyrcle, 1 - read to end of the cyrcle
        /// </summary>
        /// <returns></returns>
        protected virtual int ParseStringFileLine()
        {
            return -1;
        }

        /// <summary>
        /// read line to ParseData.line from streamreader
        /// </summary>
        /// <returns></returns>
        protected virtual string ReadLine()
        {
            ParseData.line = ParseData.reader.ReadLine();
            ParseData.TrimmedLine = ParseData.line;

            return ParseData.line;
        }

        /// <summary>
        /// extract text from line with regex pattern
        /// </summary>
        /// <param name="pattern">Key - Part of line for find in line, Value - regex pattern</param>
        /// <returns></returns>
        protected bool ParsePattern(string pattern)
        {
            return ParsePattern(new KeyValuePair<string, string>("", pattern), false);
        }

        /// <summary>
        /// patterns for add
        /// </summary>
        protected virtual Dictionary<string, string> Patterns()
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// parse patterns default
        /// </summary>
        /// <returns></returns>
        protected bool ParsePatterns()
        {
            bool ret = false;
            foreach (var pattern in Patterns())
            {
                if (ParsePattern(pattern))
                {
                    ret = true;
                }
            }
            return ret;
        }

        /// <summary>
        /// extract text from line with regex pattern
        /// </summary>
        /// <param name="pattern">Key - Part of line for find in line, Value - regex pattern</param>
        /// <returns></returns>
        protected bool ParsePattern(KeyValuePair<string, string> pattern, bool useInlineSearch = true)
        {
            if (((useInlineSearch && ParseData.line.IndexOf(pattern.Key) != -1) || !useInlineSearch) && Regex.IsMatch(ParseData.line, pattern.Value, RegexOptions.Compiled))
            {
                var mc = Regex.Matches(ParseData.line, pattern.Value, RegexOptions.Compiled);
                if (mc.Count > 0)
                {
                    if (thDataWork.OpenFileMode)
                    {
                        foreach (Match m in mc)
                        {
                            var str = PreAddString(m.Result("$1"));
                            AddRowData(str, useInlineSearch ? pattern.Key : T._("Extracted with") + ":" + pattern.Value, true, true);
                            if (!ParseData.Ret)
                                ParseData.Ret = true;
                        }
                    }
                    else
                    {
                        for (int m = mc.Count - 1; m >= 0; m--)
                        {
                            var str = PreAddString(mc[m].Result("$1"));
                            if (IsValidString(str) && thDataWork.TablesLinesDict.ContainsKey(str))
                            {
                                ParseData.line = ParseData.line.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, mc[m].Value.Replace(str, FixInvalidSymbols(thDataWork.TablesLinesDict[str])));
                                if (!ParseData.Ret)
                                    ParseData.Ret = true;
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        protected virtual string PreAddString(string str)
        {
            return str;
        }

        /// <summary>
        /// remove invalid symbols for the project or replace them to some valid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected virtual string FixInvalidSymbols(string str)
        {
            return str;
        }

        protected virtual bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && thDataWork.SaveFileMode && ParseData.ResultForWrite.Length > 0 && !FunctionsFileFolder.FileInUse(thDataWork.FilePath))
                {
                    File.WriteAllText(filePath.Length > 0 ? filePath : GetFilePath(), ParseData.ResultForWrite.ToString(), FunctionsFileFolder.GetEncoding(thDataWork.FilePath));
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// file destination for write
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFilePath()
        {
            return thDataWork.FilePath;
        }

        protected class ParseFileData
        {
            /// <summary>
            /// Project work data
            /// </summary>
            private readonly THDataWork thDataWork;
            /// <summary>
            /// tablename/filename
            /// </summary>
            internal string tablename;
            /// <summary>
            /// result of parsing
            /// </summary>
            internal bool Ret;
            /// <summary>
            /// line value
            /// </summary>
            internal string line;
            string trimmed = string.Empty;
            /// <summary>
            /// trimmed line value
            /// </summary>
            internal string TrimmedLine { get => trimmed; set => trimmed = value != null ? value.Trim() : ""; }
            /// <summary>
            /// Usually here adding file's content for write
            /// </summary>
            internal StringBuilder ResultForWrite;
            /// <summary>
            /// Usually using to parse comment sections like /* commented text */
            /// </summary>
            internal bool IsComment;
            /// <summary>
            /// Streamreader of the processing file
            /// </summary>
            internal StreamReader reader;

            public ParseFileData(THDataWork thDataWork)
            {
                this.thDataWork = thDataWork;
                tablename = Path.GetFileName(thDataWork.FilePath);
                if (thDataWork.SaveFileMode)
                {
                    ResultForWrite = new StringBuilder();
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

        protected HashSet<string> hashes;
        protected bool AddRowData(string RowData, string RowInfo, bool CheckAddHashes = false)
        {
            return AddRowData(Path.GetFileName(thDataWork.FilePath), RowData, RowInfo, CheckAddHashes);
        }
        protected bool AddRowData(string[] RowData, string RowInfo, bool CheckAddHashes = false)
        {
            return AddRowData(Path.GetFileName(thDataWork.FilePath), RowData, RowInfo, CheckAddHashes, true, false);
        }
        protected bool AddRowData(string RowData, string RowInfo, bool CheckAddHashes, bool CheckInput)
        {
            return AddRowData(Path.GetFileName(thDataWork.FilePath), RowData, RowInfo, CheckAddHashes, CheckInput);
        }
        protected bool AddRowData(string tablename, string RowData, string RowInfo, bool CheckAddHashes = false, bool CheckInput = true, bool AddToDictionary = false)
        {
            return AddRowData(tablename, new string[] { RowData }, RowInfo, CheckAddHashes, CheckInput, AddToDictionary);
        }
        protected bool AddRowData(string tablename, string[] RowData, string RowInfo, bool CheckAddHashes = false, bool CheckInput = true, bool AddToDictionary = false)
        {
            if (CheckInput && !IsValidString(RowData[0]))
            {
                return false;
            }

            if (CheckAddHashes && hashes != null && hashes.Contains(RowData[0]))
            {
                return false;
            }

            if (AddToDictionary)
            {
                throw new NotImplementedException("Dictionary not implemented");
                //if (!thDataWork.THFilesElementsDictionary.ContainsKey(RowData[0]))
                //{
                //    thDataWork.THFilesElementsDictionary.Add(RowData[0], RowData.Length == 2 ? RowData[1] : string.Empty);
                //    thDataWork.THFilesElementsDictionaryInfo.Add(RowData[0], RowInfo);
                //}
            }
            else
            {
                thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Add(RowData);
                thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add(RowInfo);
                if (CheckAddHashes && hashes != null)
                {
                    hashes.Add(RowData[0]);
                }
            }
            return true;
        }

        protected bool CheckTablesContent(string tablename, bool IsDictionary = false)
        {
            if (IsDictionary /*&& thDataWork.THFilesElementsDictionary != null && thDataWork.THFilesElementsDictionary.Count > 0 && thDataWork.THFilesElementsDataset.Tables[tablename] != null && thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Count == 0*/)
            {
                throw new NotImplementedException("Dictionary not implemented");
                //return thDataWork.THFilesElementsDataset.Tables[tablename].FillWithDictionary(thDataWork.THFilesElementsDictionary);
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

        bool TablesLinesDictFilled;
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
                if (TablesLinesDictFilled /*|| TablesLinesDict != null && TablesLinesDict.Count > 0*/)
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
                        if (OriginalLinesCount > Translation.Length)
                        {
                            continue;//skip lines where translation is incosistent to original
                        }

                        Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                    }

                    //Сначала добавить полный вариант
                    if (!TablesLinesDict.ContainsKey(Original) && ((!thDataWork.CurrentProject.TablesLinesDictAddEqual && Translation != Original) || thDataWork.CurrentProject.TablesLinesDictAddEqual))
                    {
                        TablesLinesDict.Add(Original, Translation/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                        if (OriginalLinesCount == 1)
                        {
                            continue;//когда одна строка не тратить время на её разбор
                        }
                    }

                    if (!MakeLinesCountEqual && OriginalLinesCount > TranslationLinesCount)
                    {
                        if (OriginalLinesCount > Translation.Length)
                        {
                            continue;//skip lines where translation is incosistent to original
                        }

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
            TablesLinesDictFilled = true;
        }
    }
}
