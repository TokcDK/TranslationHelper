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
            hashes = ProjectData.hashes;
            RowNumber = 0;

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
            if (!ParseStringFilePreOpen())
            {
                return false;
            }

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
            return ParseData.TableName;
        }

        /// <summary>
        /// Pre open file actions
        /// </summary>
        protected virtual bool ParseStringFilePreOpen()
        {
            ParseData = new ParseFileData();

            if (ProjectData.SaveFileMode && !ProjectData.THFilesList.Items.Contains(ParseData.TableName))
            {
                return false;
            }

            if (ProjectData.OpenFileMode)
            {
                AddTables(TableName());
            }

            if (ProjectData.SaveFileMode)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(TableName(), false, false);
            }

            ParseStringFilePreOpenExtra();

            return true;
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
            if (ProjectData.OpenFileMode)
            {
                return CheckTablesContent(ParseData.TableName);
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
            if (ProjectData.SaveFileMode)
            {
                if (LastEmptyLine)
                {
                    ParseData.ResultForWrite.Append(ParseData.Line + newline);
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

                    ParseData.ResultForWrite.Append(ParseData.Line);
                }
            }
        }

        /// <summary>
        /// add translation if exists in DB 
        /// by default will be checked translation for ParseData.line if not set
        /// translation will be set to ParseData.line. use other overload to set specific variable for translation
        /// </summary>
        /// <param name=THSettings.OriginalColumnName()>if not set then will be used ParseData.line</param>
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
        /// <param name=THSettings.TranslationColumnName()>will be set by value of translation</param>
        /// <param name=THSettings.OriginalColumnName()>if not set then will be used ParseData.line</param>
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
        /// <param name=THSettings.TranslationColumnName()></param>
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
                    var IsSet = false;
                    if (ProjectData.OpenFileMode)
                    {
                        foreach (Match m in mc)
                        {
                            var str = m.Result("$1");
                            IsSet = AddRowData(str, useInlineSearch ? pattern.Key : T._("Extracted with") + ":" + pattern.Value, CheckInput: true);
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
            public ParseFileData()
            {
                TableName = ProjectData.CurrentProject?.CurrentFormat != null && ProjectData.CurrentProject.CurrentFormat.UseTableNameWithoutExtension
                    ? Path.GetFileNameWithoutExtension(ProjectData.FilePath)
                    : Path.GetFileName(ProjectData.FilePath);

                if (ProjectData.SaveFileMode)
                {
                    ResultForWrite = new StringBuilder();
                }
            }

            /// <summary>
            /// tablename/filename
            /// </summary>
            internal string TableName;
            /// <summary>
            /// result of parsing. Must be set to true if any value was translated.
            /// </summary>
            internal bool Ret;
            /// <summary>
            /// line value
            /// </summary>
            internal string Line;

            //string trimmed = string.Empty;

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
            if (!ProjectData.THFilesElementsDataset.Tables.Contains(tablename))
            {
                ProjectData.THFilesElementsDataset.Tables.Add(tablename);
                ProjectData.THFilesElementsDataset.Tables[tablename].Columns.Add(THSettings.OriginalColumnName());
                ProjectData.THFilesElementsDataset.Tables[tablename].Columns.Add(THSettings.TranslationColumnName());

                if (extraColumns != null && extraColumns.Length > 0)
                {
                    foreach (var columnName in extraColumns)
                    {
                        ProjectData.THFilesElementsDataset.Tables[tablename].Columns.Add(columnName);
                    }
                }
            }
            if (!ProjectData.THFilesElementsDatasetInfo.Tables.Contains(tablename))
            {
                ProjectData.THFilesElementsDatasetInfo.Tables.Add(tablename);
                ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Columns.Add("Info");
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
        internal bool AddRowData(string RowData, string RowInfo = "", bool CheckInput = true)
        {
            return AddRowData(Path.GetFileName(ProjectData.FilePath), RowData, RowInfo, CheckInput);
        }
        /// <summary>
        /// Add string to table with options
        /// </summary>
        /// <param name="RowData">original string</param>
        /// <param name="RowInfo">info about the string</param>
        /// <param name="CheckAddHashes">add strings to hashes and skip same strings</param>
        /// <returns></returns>
        internal bool AddRowData(string[] RowData, string RowInfo, bool CheckInput = true)
        {
            return AddRowData(Path.GetFileName(ProjectData.FilePath), RowData, RowInfo, CheckInput);
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
        internal bool AddRowData(string tablename, string RowData, string RowInfo, bool CheckInput = true)
        {
            return AddRowData(tablename, new string[] { RowData }, RowInfo, CheckInput);
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
        internal bool AddRowData(string tablename, string[] RowData, string RowInfo, bool CheckInput = true)
        {
            var original = AddRowDataPreAddOriginalStringMod(RowData[0]);

            if (CheckInput && !IsValidString(original))
            {
                return false;
            }

            if (Properties.Settings.Default.DontLoadDuplicates && (hashes == null || hashes.Contains(RowData[0])))
            {
                return false;
            }

            ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Add(RowData);
            ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add(RowInfo);

            if (Properties.Settings.Default.DontLoadDuplicates)
            {
                // add to hashes when only unique values
                hashes.Add(original);
            }
            else
            {
                // variant with duplicates

                // check if original exists
                if (!ProjectData.OriginalsTableRowCoordinats.ContainsKey(original))
                {
                    ProjectData.OriginalsTableRowCoordinats.Add(original, new Dictionary<string, HashSet<int>>());
                }

                // check if tablename is exists
                if (!ProjectData.OriginalsTableRowCoordinats[original].ContainsKey(tablename))
                {
                    ProjectData.OriginalsTableRowCoordinats[original].Add(tablename, new HashSet<int>());
                }

                // check if current row number is exists
                if (!ProjectData.OriginalsTableRowCoordinats[original][tablename].Contains(RowNumber))
                {
                    ProjectData.OriginalsTableRowCoordinats[original][tablename].Add(RowNumber);
                }

                // raise row number
                RowNumber++;
            }

            return true;
        }

        /// <summary>
        /// current row number in parsing table
        /// </summary>
        protected int RowNumber = 0;

        /// <summary>
        /// check if translation is exists and set str return true if found.
        /// <paramref name="valueToTranslate"/> = input string, must contain original value for search.
        /// <paramref name="existsTranslation"/> = control translation value, if was loaded translation from file
        /// </summary>
        /// <param name="valueToTranslate">input=original, output=translation</param>
        /// <param name="existsTranslation">control translation value if was loaded translation from file</param>
        /// <returns>true if translation was set and not equal to input original</returns>
        internal bool SetTranslation(ref string valueToTranslate, string existsTranslation = null)
        {
            var isTranslated = false;

            if (!Properties.Settings.Default.DontLoadDuplicates
                && ProjectData.OriginalsTableRowCoordinats != null
                && ProjectData.OriginalsTableRowCoordinats.ContainsKey(valueToTranslate) // input value has original's value before it will be changed to translation
                )
            {
                var currentTableName = ParseData.TableName;
                var pretranslatedOriginal = valueToTranslate;
                if (ProjectData.OriginalsTableRowCoordinats[valueToTranslate].ContainsKey(currentTableName))
                {
                    if (ProjectData.OriginalsTableRowCoordinats[valueToTranslate][currentTableName].Contains(RowNumber))
                    {
                        valueToTranslate = ProjectData.THFilesElementsDataset.Tables[currentTableName].Rows[RowNumber][1] + "";
                        valueToTranslate = FixInvalidSymbols(valueToTranslate);

                        isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                        if (isTranslated)
                        {
                            ParseData.Ret = true;
                        }

                        //return ret;
                    }
                    else // set 1st value from avalaible values
                    {
                        ProjectData.AppLog.LogToFile("Warning! Row not found. row number=" + RowNumber + ". table name=" + TableName() + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                        foreach (var rowIndex in ProjectData.OriginalsTableRowCoordinats[valueToTranslate][currentTableName])
                        {
                            valueToTranslate = ProjectData.THFilesElementsDataset.Tables[currentTableName].Rows[rowIndex][1] + "";
                            valueToTranslate = FixInvalidSymbols(valueToTranslate);

                            isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                            if (isTranslated)
                            {
                                ParseData.Ret = true;
                            }

                            //return ret;
                        }
                    }
                }
                else // set 1st value from avalaible values
                {
                    ProjectData.AppLog.LogToFile("Warning! Table not found. row number=" + RowNumber + ". table name=" + TableName() + ".valueToTranslate:\r\n" + valueToTranslate + "\r\nexistsTranslation:\r\n" + existsTranslation);

                    foreach (var existTableName in ProjectData.OriginalsTableRowCoordinats[valueToTranslate].Values)
                    {
                        foreach (var existsRowIndex in existTableName)
                        {
                            valueToTranslate = ProjectData.THFilesElementsDataset.Tables[currentTableName].Rows[existsRowIndex][1] + "";
                            valueToTranslate = FixInvalidSymbols(valueToTranslate);

                            isTranslated = pretranslatedOriginal != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                            if (isTranslated)
                            {
                                ParseData.Ret = true;
                                break; // translated, dont need to iterate rows anymore
                            }

                            //return ret;
                        }

                        if (isTranslated)
                        {
                            ParseData.Ret = true;
                            break; // translated, dont need to iterate table names anymore
                        }
                    }
                }

                RowNumber++;
            }
            else if (ProjectData.TablesLinesDict.ContainsKey(valueToTranslate))
            {
                var control = valueToTranslate;
                valueToTranslate = ProjectData.TablesLinesDict[valueToTranslate];
                valueToTranslate = FixInvalidSymbols(valueToTranslate);

                isTranslated = control != valueToTranslate || (existsTranslation != null && existsTranslation != valueToTranslate);
                if (isTranslated)
                {
                    ParseData.Ret = true;
                }

                //return ret;
            }

            return isTranslated;
        }

        protected bool CheckTablesContent(string tablename, bool IsDictionary = false)
        {
            if (IsDictionary /*&& ProjectData.THFilesElementsDictionary != null && ProjectData.THFilesElementsDictionary.Count > 0 && ProjectData.THFilesElementsDataset.Tables[tablename] != null && ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Count == 0*/)
            {
                throw new NotImplementedException("Dictionary not implemented");
                //return ProjectData.THFilesElementsDataset.Tables[tablename].FillWithDictionary(ProjectData.THFilesElementsDictionary);
            }
            else if (ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                if (ProjectData.THFilesElementsDataset.Tables.Contains(tablename))
                {
                    ProjectData.THFilesElementsDataset.Tables.Remove(tablename); // remove table if was no items added
                }

                if (ProjectData.THFilesElementsDatasetInfo.Tables.Contains(tablename))
                {
                    ProjectData.THFilesElementsDatasetInfo.Tables.Remove(tablename); // remove table if was no items added
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
            if (!ProjectData.THFilesElementsDataset.Tables.Contains(TableName))
                return;

            if (TablesLinesDict == null)
            {
                TablesLinesDict = new Dictionary<string, string>();
            }

            if (TablesLinesDict.Count > 0)
            {
                TablesLinesDict.Clear();
            }

            foreach (DataRow Row in ProjectData.THFilesElementsDataset.Tables[TableName].Rows)
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
        /// <param name="tableName"></param>
        /// <param name="makeLinesCountEqual">if true, line count will be made equal in translation before add original else it will be made only for multiline and rigth after line by line check</param>
        /// <param name="onlyOneTable">Parse only <paramref name="tableName"/></param>
        internal void SplitTableCellValuesAndTheirLinesToDictionary(string tableName, bool makeLinesCountEqual = true, bool onlyOneTable = true)
        {
            if (!Properties.Settings.Default.DontLoadDuplicates) // skip if do not load duplicates option is disabled
            {
                return;
            }

            if (onlyOneTable)
            {
                if (!ProjectData.THFilesElementsDataset.Tables.Contains(tableName))
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


            foreach (DataTable Table in ProjectData.THFilesElementsDataset.Tables)
            {
                if (onlyOneTable && Table.TableName != tableName)
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
                    if (!LinesCountisEqual && makeLinesCountEqual)
                    {
                        if (OriginalLinesCount > Translation.Length)
                        {
                            continue;//skip lines where translation is incosistent to original
                        }

                        Translation = string.Join(Environment.NewLine, FunctionsString.SplitStringByEqualParts(Translation, OriginalLinesCount));
                    }

                    //Сначала добавить полный вариант
                    if (!TablesLinesDict.ContainsKey(Original) /*&& ((!ProjectData.CurrentProject.TablesLinesDictAddEqual && Translation != Original) || ProjectData.CurrentProject.TablesLinesDictAddEqual)*/)
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

                    if (!makeLinesCountEqual && OriginalLinesCount > TranslationLinesCount)
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
                            if (LinesCountisEqual) //когда количество строк равно, просто добавлять валидные строки в словарь
                            {
                                if (!TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                                {
                                    TablesLinesDict.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                }
                            }
                            else
                            {
                                if (lineNumber < OriginalLinesCount - 1) // пока номер строки меньше номера последней строки в оригинале
                                {
                                    if (!TablesLinesDict.ContainsKey(OriginalLines[lineNumber]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[lineNumber] != TranslationLines[lineNumber])
                                    {
                                        TablesLinesDict.Add(OriginalLines[lineNumber], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                    }
                                }
                                else // номер строки равен номеру последней строки оригинала
                                {
                                    if (lineNumber == TranslationLinesCount - 1) //если номер строки равен номеру последней строки в переводе
                                    {
                                        if (extralines.Count > 0) // если список экстра строк не пустой
                                        {
                                            extralines.Add(TranslationLines[lineNumber]); // добавить последнюю строку в переводе
                                            string result = string.Join(Environment.NewLine, extralines); // объединить экстра строки в одну


                                            if (!TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1]) //если словарь не содержит последнюю строку оригинала
                                                && result.Trim().Length > 0 // объединенные строки без пробельных символов и символов новой строки - не пустые 
                                                && OriginalLines[OriginalLinesCount - 1] != result) // оригинал не равен переводу
                                            {
                                                //добавить оригинал с переводом содержащим больше строк, чем в оригинале
                                                TablesLinesDict.Add(OriginalLines[OriginalLinesCount - 1], result/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                            }
                                        }
                                        else
                                        {
                                            // при пустом списке экстра строк добавить в словарь оригинал с переводом, если валидный
                                            if (!TablesLinesDict.ContainsKey(OriginalLines[OriginalLinesCount - 1]) && TranslationLines[lineNumber].Length > 0 && OriginalLines[OriginalLinesCount - 1] != TranslationLines[lineNumber])
                                            {
                                                TablesLinesDict.Add(OriginalLines[OriginalLinesCount - 1], TranslationLines[lineNumber]/*.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit)*/);
                                            }
                                        }
                                    }
                                    else // пока номер текущей строки меньше номера последней строки в переводе, добавлять экстра строки в один список
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
