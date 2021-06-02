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

        /// <summary>
        /// identifier to check how to identifi if selected extension must be parsed with this format.
        /// in result can be added new project which will be used Ext and this identifier to open valid standalone files.
        /// </summary>
        /// <returns></returns>
        internal virtual bool ExtIdentifier()
        {
            return false;
        }

        internal virtual bool Open() { return ParseStringFile(); }

        internal virtual bool Save() { return ParseStringFile(); }

        protected bool IsValidString(string inputString)
        {
            //preclean string
            inputString = thDataWork.CurrentProject.CleanStringForCheck(inputString);

            return !string.IsNullOrWhiteSpace(inputString) && !inputString.ForJPLangHaveMostOfRomajiOtherChars();
        }

        /// <summary>
        /// Parse Data
        /// </summary>
        internal ParseFileData ParseData;

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
            using (ParseData.reader = new StreamReader(thDataWork.FilePath, ParseStringFileEncoding()))
            {
                ParseStringFileLines();
            }
        }

        /// <summary>
        /// get encoding for string file open
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding ParseStringFileEncoding()
        {
            return FunctionsFileFolder.GetEncoding(thDataWork.FilePath) ?? DefaultEncoding();
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
        /// table name
        /// </summary>
        protected virtual string TableName()
        {
            return ParseData.tablename;
        }

        /// <summary>
        /// Pre open file actions
        /// </summary>
        protected virtual void ParseStringFilePreOpen()
        {
            ParseData = new ParseFileData(thDataWork);

            if (thDataWork.OpenFileMode)
            {
                AddTables(TableName());
            }

            if (thDataWork.SaveFileMode)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(TableName(), false, false);
            }

            ParseStringFilePreOpenExtra();
        }

        /// <summary>
        /// Pre open file extra actions
        /// </summary>
        protected virtual void ParseStringFilePreOpenExtra()
        {
            firstline = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// opening string file and parse lines
        /// </summary>
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
        /// Parse line of string file
        /// -1=stop parse cyrcle, 0-continue cyrcle, 1 - read to end of the cyrcle
        /// </summary>
        /// <returns></returns>
        protected virtual int ParseStringFileLine()
        {
            return -1;
        }

        bool firstline = true;

        /// <summary>
        /// add line for wtite in save mode
        /// </summary>
        /// <param name="newline"></param>
        /// <param name="LastEmptyLine">last line must be empty</param>
        protected virtual void SaveModeAddLine(bool LastEmptyLine)
        {
            SaveModeAddLine("\r\n", LastEmptyLine);
        }

        /// <summary>
        /// last newline symbol for paste after required line, not before it
        /// </summary>
        string lastNewline = "\r\n";
        /// <summary>
        /// add line for wtite in save mode
        /// </summary>
        /// <param name="newline"></param>
        /// <param name="LastEmptyLine">last line must be empty</param>
        protected virtual void SaveModeAddLine(string newline = "\r\n", bool LastEmptyLine = false)
        {
            if (thDataWork.SaveFileMode)
            {
                if (LastEmptyLine)
                {
                    ParseData.ResultForWrite.Append(ParseData.line + newline);
                }
                else
                {
                    if (firstline)
                    {
                        firstline = false;
                    }
                    else
                    {
                        ParseData.ResultForWrite.Append(lastNewline);
                    }

                    lastNewline = newline;//set newline symbol to paste after current line

                    ParseData.ResultForWrite.Append(ParseData.line);
                }
            }
        }

        /// <summary>
        /// add translation if exists in DB 
        /// by default will be checked translation for ParseData.line if not set
        /// translation will be set to ParseData.line. use other overload to set specific variable for translation
        /// </summary>
        /// <param name="original">if not set then will be used ParseData.line</param>
        protected virtual void AddTranslation(string original = null)
        {
            original = original ?? ParseData.line;
            if (thDataWork.TablesLinesDict.ContainsKey(original))
            {
                ParseData.Ret = true;
                ParseData.line = TranslationMod(thDataWork.TablesLinesDict[original]);
            }
        }

        /// <summary>
        /// add translation if exists in DB 
        /// by default for original will be checked translation for ParseData.line if not set
        /// 'translation' will be set with translation
        /// </summary>
        /// <param name="translation">will be set by value of translation</param>
        /// <param name="original">if not set then will be used ParseData.line</param>
        protected virtual void AddTranslation(ref string translation, string original = null)
        {
            original = original ?? ParseData.line;
            if (thDataWork.TablesLinesDict.ContainsKey(original))
            {
                ParseData.Ret = true;
                translation = TranslationMod(thDataWork.TablesLinesDict[original]);
            }
        }

        /// <summary>
        /// modification of translation before it will be added
        /// by default no modifications
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        protected virtual string TranslationMod(string translation)
        {
            return translation;
        }

        /// <summary>
        /// read line to ParseData.line from streamreader
        /// </summary>
        /// <returns></returns>
        protected virtual string ReadLine()
        {
            ParseData.line = ParseData.reader.ReadLine();
            ReadLineMod();
            //ParseData.TrimmedLine = ParseData.line;

            return ParseData.line;
        }

        /// <summary>
        /// modification of read ParseData.line
        /// </summary>
        /// <returns></returns>
        protected virtual void ReadLineMod()
        {
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
        /// patterns for add. 
        /// first string = string which line contains. 
        /// second string = regex patter from which will be get/set first element $1
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
        /// extract text from line with regex pattern.
        /// will add or save first $1 found.
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
                    var IsSet = false;
                    if (thDataWork.OpenFileMode)
                    {
                        foreach (Match m in mc)
                        {
                            var str = m.Result("$1");
                            IsSet = AddRowData(str, useInlineSearch ? pattern.Key : T._("Extracted with") + ":" + pattern.Value, true, true);
                        }
                    }
                    else
                    {
                        for (int m = mc.Count - 1; m >= 0; m--)
                        {
                            var str = mc[m].Result("$1");
                            var trans = str;
                            if (IsValidString(str) && SetTranslation(ref trans) && trans != str)
                            {
                                ParseData.line = ParseData.line.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, mc[m].Value.Replace(str, FixInvalidSymbols(trans)));
                                ParseData.Ret = true;
                                IsSet = true;
                            }
                        }
                    }
                    return IsSet;
                }
            }
            return false;
        }

        /// <summary>
        /// original string modification before add it with AddRowData.
        /// default will be returned same string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected virtual string AddRowDataPreAddOriginalStringMod(string str)
        {
            return str;
        }

        /// <summary>
        /// remove invalid symbols for the project or replace them to some valid.
        /// applied to found translation before add it.
        /// default is same string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected virtual string FixInvalidSymbols(string str)
        {
            return str
                .Replace("\u200b", string.Empty)//remove zero-length-space (can be produced by online translator)
                ;
        }

        /// <summary>
        /// encoding for reading string file
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding ReadEncoding()
        {
            return ParseStringFileEncoding();
        }

        /// <summary>
        /// encoding for writing string file
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding WriteEncoding()
        {
            return ParseStringFileEncoding();
        }

        protected virtual bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && thDataWork.SaveFileMode && ParseData.ResultForWrite.Length > 0 && !FunctionsFileFolder.FileInUse(thDataWork.FilePath))
                {
                    File.WriteAllText(filePath.Length > 0 ? filePath : GetFilePath(), ParseData.ResultForWrite.ToString(), WriteEncoding());
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
        /// usually it is same path as open but some time it can be other dir
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFilePath()
        {
            return thDataWork.FilePath;
        }

        internal class ParseFileData
        {
            /// <summary>
            /// Project work data
            /// </summary>
            protected readonly THDataWork thDataWork;
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
            internal string TrimmedLine { get => line.Trim(); }
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
            /// <summary>
            /// array of all lines of opened file. For causes when it is using
            /// </summary>
            internal string[] LinesArray;

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
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckAddHashes">add strings to hashes and skip same strings</param>
        /// <returns></returns>
        protected bool AddRowData(string RowData, string RowInfo = "", bool CheckAddHashes = false, bool CheckInput = true)
        {
            return AddRowData(Path.GetFileName(thDataWork.FilePath), RowData, RowInfo, CheckAddHashes, CheckInput);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckAddHashes">add strings to hashes and skip same strings</param>
        /// <returns></returns>
        protected bool AddRowData(string[] RowData, string RowInfo, bool CheckAddHashes = false, bool CheckInput = true)
        {
            return AddRowData(Path.GetFileName(thDataWork.FilePath), RowData, RowInfo, CheckAddHashes, CheckInput, false);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckAddHashes">add strings to hashes and skip same strings</param>
        /// <param name="CheckInput">cheack original string if valid</param>
        /// <param name="AddToDictionary"></param>
        /// <returns></returns>
        protected bool AddRowData(string tablename, string RowData, string RowInfo, bool CheckAddHashes = false, bool CheckInput = true, bool AddToDictionary = false)
        {
            return AddRowData(tablename, new string[] { RowData }, RowInfo, CheckAddHashes, CheckInput, AddToDictionary);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckAddHashes">add strings to hashes and skip same strings</param>
        /// <param name="CheckInput">cheack original string if valid</param>
        /// <param name="AddToDictionary"></param>
        /// <returns></returns>
        protected bool AddRowData(string tablename, string[] RowData, string RowInfo, bool CheckAddHashes = false, bool CheckInput = true, bool AddToDictionary = false)
        {
            var original = AddRowDataPreAddOriginalStringMod(RowData[0]);

            if (CheckInput && !IsValidString(original))
            {
                return false;
            }

            if (Properties.Settings.Default.DontLoadDuplicates && CheckAddHashes && hashes != null && hashes.Contains(RowData[0]))
            {
                return false;
            }

            if (AddToDictionary)
            {
                throw new NotImplementedException("Dictionary not implemented");
                //if (!thDataWork.THFilesElementsDictionary.ContainsKey(original))
                //{
                //    thDataWork.THFilesElementsDictionary.Add(original, RowData.Length == 2 ? RowData[1] : string.Empty);
                //    thDataWork.THFilesElementsDictionaryInfo.Add(original, RowInfo);
                //}
            }
            else
            {
                thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Add(RowData);
                thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add(RowInfo);

                if (Properties.Settings.Default.DontLoadDuplicates && CheckAddHashes && hashes != null)
                {
                    hashes.Add(original);
                }
                else if (!Properties.Settings.Default.DontLoadDuplicates)
                {
                    //add coordinates
                    if (!thDataWork.OriginalsTableRowCoordinats.ContainsKey(original))
                    {
                        thDataWork.OriginalsTableRowCoordinats.Add(original, new Dictionary<string, List<int>>());
                        thDataWork.OriginalsTableRowCoordinats[original].Add(tablename, new List<int>());
                        thDataWork.OriginalsTableRowCoordinats[original][tablename].Add(thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Count);
                    }
                    else
                    {
                        if (!thDataWork.OriginalsTableRowCoordinats[original].ContainsKey(tablename))
                        {
                            thDataWork.OriginalsTableRowCoordinats[original].Add(tablename, new List<int>());
                            thDataWork.OriginalsTableRowCoordinats[original][tablename].Add(thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Count);
                        }
                        else
                        {
                            thDataWork.OriginalsTableRowCoordinats[RowData[0]][tablename].Add(thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Count);
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// row index indicator for save purposes
        /// </summary>
        protected int SaveRowIndex = 0;

        /// <summary>
        /// check if translation is exists and set str return true if found.
        /// value = input string, must contain original value for search.
        /// controltrans = control translation value, if was loaded translation from file
        /// </summary>
        /// <param name="value">input=original, output=translation</param>
        /// <param name="controltrans">control translation value if was loaded translation from file</param>
        /// <returns>true if translation was set and not equal to input original</returns>
        internal bool SetTranslation(ref string value, string controltrans = null)
        {
            var ret = false;

            if (!Properties.Settings.Default.DontLoadDuplicates
                && thDataWork.OriginalsTableRowCoordinats != null
                && thDataWork.OriginalsTableRowCoordinats.ContainsKey(value))
            {
                var tname = Path.GetFileName(thDataWork.FilePath);
                if (thDataWork.OriginalsTableRowCoordinats[value].ContainsKey(tname))
                {
                    var control = value;
                    if (thDataWork.OriginalsTableRowCoordinats[value][tname].Contains(SaveRowIndex))
                    {
                        value = thDataWork.THFilesElementsDataset.Tables[tname].Rows[SaveRowIndex][1] + "";
                        value = FixInvalidSymbols(value);
                        SaveRowIndex++;

                        ret = control != value || (controltrans != null && controltrans != value);
                        if (ret)
                        {
                            ParseData.Ret = true;
                        }

                        //return ret;
                    }
                    else // set 1st value from avalaible values
                    {
                        foreach (var rind in thDataWork.OriginalsTableRowCoordinats[value][tname])
                        {
                            value = thDataWork.THFilesElementsDataset.Tables[tname].Rows[rind][1] + "";
                            value = FixInvalidSymbols(value);
                            SaveRowIndex++;

                            ret = control != value || (controltrans != null && controltrans != value);
                            if (ret)
                            {
                                ParseData.Ret = true;
                            }

                            //return ret;
                        }
                    }
                }
                else // set 1st value from avalaible values
                {
                    var control = value;
                    foreach (var tn in thDataWork.OriginalsTableRowCoordinats[value].Values)
                    {
                        foreach (var rind in tn)
                        {
                            value = thDataWork.THFilesElementsDataset.Tables[tname].Rows[rind][1] + "";
                            value = FixInvalidSymbols(value);
                            SaveRowIndex++;

                            ret = control != value || (controltrans != null && controltrans != value);
                            if (ret)
                            {
                                ParseData.Ret = true;
                            }

                            //return ret;
                        }
                    }
                }
            }
            else if (thDataWork.TablesLinesDict.ContainsKey(value))
            {
                var control = value;
                value = thDataWork.TablesLinesDict[value];
                value = FixInvalidSymbols(value);

                ret = control != value || (controltrans != null && controltrans != value);
                if (ret)
                {
                    ParseData.Ret = true;
                }

                //return ret;
            }

            return ret;
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
                    if (!TablesLinesDict.ContainsKey(Original) /*&& ((!thDataWork.CurrentProject.TablesLinesDictAddEqual && Translation != Original) || thDataWork.CurrentProject.TablesLinesDictAddEqual)*/)
                    {
                        TablesLinesDict.Add(Original, Translation/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                        if (OriginalLinesCount == 1)
                        {
                            continue;//когда одна строка не тратить время на её разбор
                        }
                    }
                    else if (Translation != Original && Original == TablesLinesDict[Original])
                    {
                        TablesLinesDict[Original] = Translation;
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
