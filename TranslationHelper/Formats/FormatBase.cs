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
        protected Dictionary<string, string> TablesLinesDict;

        protected FormatBase()
        {
            TablesLinesDict = ProjectData.TablesLinesDict;
            Hashes = ProjectData.Hashes;

            if (ProjectData.CurrentProject != null)
            {
                ProjectData.CurrentProject.CurrentFormat = this;
            }
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
            return null;
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

        /// <summary>
        /// name of format
        /// </summary>
        /// <returns></returns>
        internal virtual string Name()
        {
            return null;
        }

        internal virtual bool Open() { return ParseStringFile(); }

        internal virtual bool Save() { return ParseStringFile(); }

        /// <summary>
        /// Check string if it is valid for add to work table.
        /// Usually it is not empty string. For japanese language it is also string contain most of japanese chars
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal virtual bool IsValidString(string inputString)
        {
            //preclean string
            inputString = ProjectData.CurrentProject.CleanStringForCheck(inputString);

            return !string.IsNullOrWhiteSpace(inputString) && !inputString.ForJpLangHaveMostOfRomajiOtherChars();
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
            using (ParseData.Reader = new StreamReader(ProjectData.FilePath, ParseStringFileEncoding()))
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
            return FunctionsFileFolder.GetEncoding(ProjectData.FilePath) ?? DefaultEncoding();
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
        /// Means use for table name name of file without extension
        /// </summary>
        internal virtual bool UseTableNameWithoutExtension => false;

        /// <summary>
        /// table name
        /// </summary>
        protected virtual string TableName()
        {
            if (UseTableNameWithoutExtension)
            {
                ParseData.Tablename = Path.GetFileNameWithoutExtension(ParseData.Tablename);
            }

            return ParseData.Tablename;
        }

        /// <summary>
        /// Pre open file actions
        /// </summary>
        protected virtual void ParseStringFilePreOpen()
        {
            ParseData = new ParseFileData();

            if (ProjectData.OpenFileMode)
            {
                AddTables(TableName());
            }

            if (ProjectData.SaveFileMode)
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
            _firstline = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool ParseStringFilePostOpen()
        {
            if (ProjectData.OpenFileMode)
            {
                return CheckTablesContent(ParseData.Tablename);
            }
            else
            {
                return WriteFileData();
            }
        }

        internal enum ParseStringFileLineReturnState
        {
            Break = -1,
            Continue = 0,
            ReadToEnd = 1
        }

        /// <summary>
        /// opening string file and parse lines
        /// </summary>
        protected virtual void ParseStringFileLines()
        {
            while (ReadLine() != null)
            {
                if (ParseStringFileLine() == ParseStringFileLineReturnState.Break)
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
        protected virtual ParseStringFileLineReturnState ParseStringFileLine()
        {
            return ParseStringFileLineReturnState.Break;
        }

        bool _firstline = true;

        /// <summary>
        /// add line for wtite in save mode
        /// </summary>
        /// <param name="newline"></param>
        /// <param name="lastEmptyLine">last line must be empty</param>
        protected virtual void SaveModeAddLine(bool lastEmptyLine)
        {
            SaveModeAddLine("\r\n", lastEmptyLine);
        }

        /// <summary>
        /// last newline symbol for paste after required line, not before it
        /// </summary>
        string _lastNewline = "\r\n";
        /// <summary>
        /// add line for wtite in save mode
        /// </summary>
        /// <param name="newline"></param>
        /// <param name="lastEmptyLine">last line must be empty</param>
        protected virtual void SaveModeAddLine(string newline = "\r\n", bool lastEmptyLine = false)
        {
            if (ProjectData.SaveFileMode)
            {
                if (lastEmptyLine)
                {
                    ParseData.ResultForWrite.Append(ParseData.Line + newline);
                }
                else
                {
                    if (_firstline)
                    {
                        _firstline = false;
                    }
                    else
                    {
                        ParseData.ResultForWrite.Append(_lastNewline);
                    }

                    _lastNewline = newline;//set newline symbol to paste after current line

                    ParseData.ResultForWrite.Append(ParseData.Line);
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
            original = original ?? ParseData.Line;
            if (ProjectData.TablesLinesDict.ContainsKey(original))
            {
                ParseData.Ret = true;
                ParseData.Line = TranslationMod(ProjectData.TablesLinesDict[original]);
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
            original = original ?? ParseData.Line;
            if (ProjectData.TablesLinesDict.ContainsKey(original))
            {
                ParseData.Ret = true;
                translation = TranslationMod(ProjectData.TablesLinesDict[original]);
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
            ParseData.Line = ParseData.Reader.ReadLine();
            ReadLineMod();
            //ParseData.TrimmedLine = ParseData.line;

            return ParseData.Line;
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
            if ((!useInlineSearch || ParseData.Line.IndexOf(pattern.Key) != -1) && Regex.IsMatch(ParseData.Line, pattern.Value, RegexOptions.Compiled))
            {
                var mc = Regex.Matches(ParseData.Line, pattern.Value, RegexOptions.Compiled);
                if (mc.Count > 0)
                {
                    var isSet = false;
                    if (ProjectData.OpenFileMode)
                    {
                        foreach (Match m in mc)
                        {
                            var str = m.Result("$1");
                            isSet = AddRowData(str, useInlineSearch ? pattern.Key : T._("Extracted with") + ":" + pattern.Value, true, true);
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
                                ParseData.Line = ParseData.Line.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, mc[m].Value.Replace(str, FixInvalidSymbols(trans)));
                                ParseData.Ret = true;
                                isSet = true;
                            }
                        }
                    }
                    return isSet;
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
                if (ParseData.Ret && ProjectData.SaveFileMode && ParseData.ResultForWrite.Length > 0 && !FunctionsFileFolder.FileInUse(ProjectData.FilePath))
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
            return ProjectData.FilePath;
        }

        internal class ParseFileData
        {
            /// <summary>
            /// tablename/filename
            /// </summary>
            internal string Tablename;
            /// <summary>
            /// result of parsing
            /// </summary>
            internal bool Ret;
            /// <summary>
            /// line value
            /// </summary>
            internal string Line;
            string _trimmed = string.Empty;
            /// <summary>
            /// trimmed line value
            /// </summary>
            internal string TrimmedLine { get => Line.Trim(); }
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
            internal StreamReader Reader;
            /// <summary>
            /// array of all lines of opened file. For causes when it is using
            /// </summary>
            internal string[] LinesArray;

            public ParseFileData()
            {

                Tablename = Path.GetFileName(ProjectData.FilePath);
                if (ProjectData.SaveFileMode)
                {
                    ResultForWrite = new StringBuilder();
                }
            }
        }

        protected void AddTables()
        {
            if (!string.IsNullOrEmpty(ProjectData.FilePath))
            {
                AddTables(Path.GetFileName(ProjectData.FilePath));
            }
        }
        protected void AddTables(string tablename, string[] extraColumns = null)
        {
            if (!ProjectData.ThFilesElementsDataset.Tables.Contains(tablename))
            {
                ProjectData.ThFilesElementsDataset.Tables.Add(tablename);
                ProjectData.ThFilesElementsDataset.Tables[tablename].Columns.Add("Original");
                ProjectData.ThFilesElementsDataset.Tables[tablename].Columns.Add("Translation");

                if (extraColumns != null && extraColumns.Length > 0)
                {
                    foreach (var columnName in extraColumns)
                    {
                        ProjectData.ThFilesElementsDataset.Tables[tablename].Columns.Add(columnName);
                    }
                }
            }
            if (!ProjectData.ThFilesElementsDatasetInfo.Tables.Contains(tablename))
            {
                ProjectData.ThFilesElementsDatasetInfo.Tables.Add(tablename);
                ProjectData.ThFilesElementsDatasetInfo.Tables[tablename].Columns.Add("Info");
            }
        }

        protected HashSet<string> Hashes;
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <param name="checkAddHashes">add strings to hashes and skip same strings</param>
        /// <returns></returns>
        internal bool AddRowData(string rowData, string rowInfo = "", bool checkAddHashes = false, bool checkInput = true)
        {
            return AddRowData(Path.GetFileName(ProjectData.FilePath), rowData, rowInfo, checkAddHashes, checkInput);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <param name="checkAddHashes">add strings to hashes and skip same strings</param>
        /// <returns></returns>
        internal bool AddRowData(string[] rowData, string rowInfo, bool checkAddHashes = false, bool checkInput = true)
        {
            return AddRowData(Path.GetFileName(ProjectData.FilePath), rowData, rowInfo, checkAddHashes, checkInput, false);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <param name="checkAddHashes">add strings to hashes and skip same strings</param>
        /// <param name="checkInput">cheack original string if valid</param>
        /// <param name="addToDictionary"></param>
        /// <returns></returns>
        internal bool AddRowData(string tablename, string rowData, string rowInfo, bool checkAddHashes = false, bool checkInput = true, bool addToDictionary = false)
        {
            return AddRowData(tablename, new string[] { rowData }, rowInfo, checkAddHashes, checkInput, addToDictionary);
        }

        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="tablename">file/table name</param>
        /// <param name="rowData">original string</param>
        /// <param name="rowInfo">info about the string</param>
        /// <param name="checkAddHashes">add strings to hashes and skip same strings</param>
        /// <param name="checkInput">cheack original string if valid</param>
        /// <param name="addToDictionary"></param>
        /// <returns></returns>
        internal bool AddRowData(string tablename, string[] rowData, string rowInfo, bool checkAddHashes = false, bool checkInput = true, bool addToDictionary = false)
        {
            var original = AddRowDataPreAddOriginalStringMod(rowData[0]);

            if (checkInput && !IsValidString(original))
            {
                return false;
            }

            if (Properties.Settings.Default.DontLoadDuplicates && checkAddHashes && Hashes != null && Hashes.Contains(rowData[0]))
            {
                return false;
            }

            if (addToDictionary)
            {
                throw new NotImplementedException("Dictionary not implemented");
                //if (!ProjectData.THFilesElementsDictionary.ContainsKey(original))
                //{
                //    ProjectData.THFilesElementsDictionary.Add(original, RowData.Length == 2 ? RowData[1] : string.Empty);
                //    ProjectData.THFilesElementsDictionaryInfo.Add(original, RowInfo);
                //}
            }
            else
            {
                ProjectData.ThFilesElementsDataset.Tables[tablename].Rows.Add(rowData);
                ProjectData.ThFilesElementsDatasetInfo.Tables[tablename].Rows.Add(rowInfo);

                if (Properties.Settings.Default.DontLoadDuplicates && checkAddHashes && Hashes != null)
                {
                    Hashes.Add(original);
                }
                else if (!Properties.Settings.Default.DontLoadDuplicates)
                {
                    //add coordinates
                    if (!ProjectData.OriginalsTableRowCoordinats.ContainsKey(original))
                    {
                        ProjectData.OriginalsTableRowCoordinats.Add(original, new Dictionary<string, List<int>>());
                        ProjectData.OriginalsTableRowCoordinats[original].Add(tablename, new List<int>());
                        ProjectData.OriginalsTableRowCoordinats[original][tablename].Add(ProjectData.ThFilesElementsDataset.Tables[tablename].Rows.Count);
                    }
                    else
                    {
                        if (!ProjectData.OriginalsTableRowCoordinats[original].ContainsKey(tablename))
                        {
                            ProjectData.OriginalsTableRowCoordinats[original].Add(tablename, new List<int>());
                            ProjectData.OriginalsTableRowCoordinats[original][tablename].Add(ProjectData.ThFilesElementsDataset.Tables[tablename].Rows.Count);
                        }
                        else
                        {
                            ProjectData.OriginalsTableRowCoordinats[rowData[0]][tablename].Add(ProjectData.ThFilesElementsDataset.Tables[tablename].Rows.Count);
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
                && ProjectData.OriginalsTableRowCoordinats != null
                && ProjectData.OriginalsTableRowCoordinats.ContainsKey(value))
            {
                var tname = Path.GetFileName(ProjectData.FilePath);
                if (ProjectData.OriginalsTableRowCoordinats[value].ContainsKey(tname))
                {
                    var control = value;
                    if (ProjectData.OriginalsTableRowCoordinats[value][tname].Contains(SaveRowIndex))
                    {
                        value = ProjectData.ThFilesElementsDataset.Tables[tname].Rows[SaveRowIndex][1] + "";
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
                        foreach (var rind in ProjectData.OriginalsTableRowCoordinats[value][tname])
                        {
                            value = ProjectData.ThFilesElementsDataset.Tables[tname].Rows[rind][1] + "";
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
                    foreach (var tn in ProjectData.OriginalsTableRowCoordinats[value].Values)
                    {
                        foreach (var rind in tn)
                        {
                            value = ProjectData.ThFilesElementsDataset.Tables[tname].Rows[rind][1] + "";
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
            else if (ProjectData.TablesLinesDict.ContainsKey(value))
            {
                var control = value;
                value = ProjectData.TablesLinesDict[value];
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

        protected bool CheckTablesContent(string tablename, bool isDictionary = false)
        {
            if (isDictionary /*&& ProjectData.THFilesElementsDictionary != null && ProjectData.THFilesElementsDictionary.Count > 0 && ProjectData.THFilesElementsDataset.Tables[tablename] != null && ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Count == 0*/)
            {
                throw new NotImplementedException("Dictionary not implemented");
                //return ProjectData.THFilesElementsDataset.Tables[tablename].FillWithDictionary(ProjectData.THFilesElementsDictionary);
            }
            else if (ProjectData.ThFilesElementsDataset.Tables[tablename].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                if (ProjectData.ThFilesElementsDataset.Tables.Contains(tablename))
                {
                    ProjectData.ThFilesElementsDataset.Tables.Remove(tablename); // remove table if was no items added
                }

                if (ProjectData.ThFilesElementsDatasetInfo.Tables.Contains(tablename))
                {
                    ProjectData.ThFilesElementsDatasetInfo.Tables.Remove(tablename); // remove table if was no items added
                }

                return false;
            }
        }

        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="tableName"></param>
        internal void SplitTableCellValuesToDictionaryLines(string tableName)
        {
            if (!ProjectData.ThFilesElementsDataset.Tables.Contains(tableName))
                return;

            if (TablesLinesDict == null)
            {
                TablesLinesDict = new Dictionary<string, string>();
            }

            if (TablesLinesDict.Count > 0)
            {
                TablesLinesDict.Clear();
            }

            foreach (DataRow row in ProjectData.ThFilesElementsDataset.Tables[tableName].Rows)
            {
                string original;
                string translation;
                if (TablesLinesDict.ContainsKey(original = row[0] + string.Empty) || (translation = row[1] + string.Empty).Length == 0 || translation == original)
                {
                    continue;
                }

                TablesLinesDict.Add(original, translation);
            }
        }

        bool _tablesLinesDictFilled;
        /// <summary>
        /// add all original\translation pairs of datatable rows in Dictionary<br/>
        /// also split multiline values and add all of their lines in Dictionary
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="makeLinesCountEqual">if true, line count will be made equal in translation before add original else it will be made only for multiline and rigth after line by line check</param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string tableName, bool makeLinesCountEqual = true, bool onlyOneTable = true)
        {
            if (onlyOneTable)
            {
                if (!ProjectData.ThFilesElementsDataset.Tables.Contains(tableName))
                    return;

                if (TablesLinesDict.Count > 0)
                {
                    TablesLinesDict.Clear();
                }
            }
            else
            {
                if (_tablesLinesDictFilled /*|| TablesLinesDict != null && TablesLinesDict.Count > 0*/)
                {
                    return;
                }
            }


            foreach (DataTable table in ProjectData.ThFilesElementsDataset.Tables)
            {
                if (onlyOneTable && table.TableName != tableName)
                {
                    continue;
                }

                foreach (DataRow row in table.Rows)
                {
                    string original = (row[0] + string.Empty);
                    int originalLinesCount = original.GetLinesCount();
                    if (originalLinesCount == 1 && TablesLinesDict.ContainsKey(original))
                    {
                        continue;
                    }

                    string translation = (row[1] + string.Empty);
                    if (translation.Length == 0)
                    {
                        continue;
                    }

                    int translationLinesCount = translation.GetLinesCount();
                    bool linesCountisEqual = originalLinesCount == translationLinesCount;
                    if (!linesCountisEqual && makeLinesCountEqual)
                    {
                        if (originalLinesCount > translation.Length)
                        {
                            continue;//skip lines where translation is incosistent to original
                        }

                        translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(translation, originalLinesCount));
                    }

                    //Сначала добавить полный вариант
                    if (!TablesLinesDict.ContainsKey(original) /*&& ((!ProjectData.CurrentProject.TablesLinesDictAddEqual && Translation != Original) || ProjectData.CurrentProject.TablesLinesDictAddEqual)*/)
                    {
                        TablesLinesDict.Add(original, translation/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                        if (originalLinesCount == 1)
                        {
                            continue;//когда одна строка не тратить время на её разбор
                        }
                    }
                    else if (translation != original && original == TablesLinesDict[original])
                    {
                        TablesLinesDict[original] = translation;
                        if (originalLinesCount == 1)
                        {
                            continue;//когда одна строка не тратить время на её разбор
                        }
                    }

                    if (!makeLinesCountEqual && originalLinesCount > translationLinesCount)
                    {
                        if (originalLinesCount > translation.Length)
                        {
                            continue;//skip lines where translation is incosistent to original
                        }

                        translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(translation, originalLinesCount));
                    }

                    //string[] OriginalLines = FunctionsString.SplitStringToArray(Original);
                    string[] translationLines = translation.GetAllLinesToArray();
                    string[] originalLines = original.GetAllLinesToArray();
                    List<string> extralines = new List<string>();
                    for (int lineNumber = 0; lineNumber < translationLinesCount; lineNumber++)
                    {
                        try
                        {
                            if (linesCountisEqual) //когда количество строк равно, просто добавлять валидные строки в словарь
                            {
                                if (!TablesLinesDict.ContainsKey(originalLines[lineNumber]) && translationLines[lineNumber].Length > 0 && originalLines[lineNumber] != translationLines[lineNumber])
                                {
                                    TablesLinesDict.Add(originalLines[lineNumber], translationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                }
                            }
                            else
                            {
                                if (lineNumber < originalLinesCount - 1) // пока номер строки меньше номера последней строки в оригинале
                                {
                                    if (!TablesLinesDict.ContainsKey(originalLines[lineNumber]) && translationLines[lineNumber].Length > 0 && originalLines[lineNumber] != translationLines[lineNumber])
                                    {
                                        TablesLinesDict.Add(originalLines[lineNumber], translationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                    }
                                }
                                else // номер строки равен номеру последней строки оригинала
                                {
                                    if (lineNumber == translationLinesCount - 1) //если номер строки равен номеру последней строки в переводе
                                    {
                                        if (extralines.Count > 0) // если список экстра строк не пустой
                                        {
                                            extralines.Add(translationLines[lineNumber]); // добавить последнюю строку в переводе
                                            string result = string.Join(Environment.NewLine, extralines); // объединить экстра строки в одну


                                            if (!TablesLinesDict.ContainsKey(originalLines[originalLinesCount - 1]) //если словарь не содержит последнюю строку оригинала
                                                && result.Trim().Length > 0 // объединенные строки без пробельных символов и символов новой строки - не пустые 
                                                && originalLines[originalLinesCount - 1] != result) // оригинал не равен переводу
                                            {
                                                //добавить оригинал с переводом содержащим больше строк, чем в оригинале
                                                TablesLinesDict.Add(originalLines[originalLinesCount - 1], result/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                            }
                                        }
                                        else
                                        {
                                            // при пустом списке экстра строк добавить в словарь оригинал с переводом, если валидный
                                            if (!TablesLinesDict.ContainsKey(originalLines[originalLinesCount - 1]) && translationLines[lineNumber].Length > 0 && originalLines[originalLinesCount - 1] != translationLines[lineNumber])
                                            {
                                                TablesLinesDict.Add(originalLines[originalLinesCount - 1], translationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                            }
                                        }
                                    }
                                    else // пока номер текущей строки меньше номера последней строки в переводе, добавлять экстра строки в один список
                                    {
                                        extralines.Add(translationLines[lineNumber]);
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
            _tablesLinesDictFilled = true;
        }
    }
}
