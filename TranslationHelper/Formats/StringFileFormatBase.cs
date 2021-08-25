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
    abstract class StringFileFormatBase:FormatBase
    {
        protected StringFileFormatBase()
        {
            RowNumber = 0;
        }

        internal override bool Open() { return ParseFile(); }

        internal override bool Save() { return ParseFile(); }

        /// <summary>
        /// table name
        /// </summary>
        protected override string TableName()
        {
            return ParseData.TableName;
        }

        /// <summary>
        /// Parse Data
        /// </summary>
        internal ParseFileData ParseData;

        /// <summary>
        /// Open file actions
        /// </summary>
        protected override void FileOpen()
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
        /// Pre open file actions
        /// </summary>
        protected override bool FilePreOpenActions()
        {
            ParseData = new ParseFileData();            

            return base.FilePreOpenActions();
        }

        /// <summary>
        /// Pre open file extra actions
        /// </summary>
        protected override void PreOpenExtraActions()
        {
            firstline = true;
        }

        internal enum KeywordActionAfter
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
                if (ParseStringFileLine() == KeywordActionAfter.Break)
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
        protected virtual KeywordActionAfter ParseStringFileLine()
        {
            return KeywordActionAfter.Break;
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
        /// <param name="originalValue">if not set then will be used ParseData.line</param>
        protected virtual void AddTranslation(string originalValue = null)
        {
            originalValue = originalValue ?? ParseData.Line;
            if (ProjectData.CurrentProject.TablesLinesDict.ContainsKey(originalValue))
            {
                ParseData.Ret = true;
                ParseData.Line = TranslationMod(ProjectData.CurrentProject.TablesLinesDict[originalValue]);
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

        protected override bool WriteFileData(string filePath = "")
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

        protected override void SetTranslationIsTranslatedAction()
        {
            ParseData.Ret = true;
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
    }
}
