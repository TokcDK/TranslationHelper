using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats
{
    abstract class FormatStringBase : FormatBase
    {
        protected FormatStringBase()
        {
            RowNumber = 0;
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
            using (ParseData.Reader = new StreamReader(GetOpenFilePath(), GetStringFileEncoding()))
            {
                ParseFileContent();
            }
        }

        /// <summary>
        /// get encoding for string file open
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding GetStringFileEncoding()
        {
            return FunctionsFileFolder.GetEncoding(GetOpenFilePath()) ?? DefaultEncoding();
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
            ParseData = new ParseFileData(this);

            return base.FilePreOpenActions();
        }

        /// <summary>
        /// Pre open file extra actions
        /// </summary>
        protected override void PreOpenExtraActions()
        {
            firstline = true;
        }

        /// <summary>
        /// opening string file and parse lines
        /// </summary>
        protected virtual void ParseFileContent()
        {
            while (ReadLine() != null) if (ParseStringFileLine() == KeywordActionAfter.Break) break;
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
        /// <param name="newLineAfter">last line must be empty</param>
        protected virtual void SaveModeAddLine(bool newLineAfter)
        {
            SaveModeAddLine(newline: Environment.NewLine, newLineAfter: newLineAfter);
        }

        /// <summary>
        /// last newline symbol for paste after required line, not before it
        /// </summary>
        string lastNewline = Environment.NewLine;
        /// <summary>
        /// add ParseData.Line for wtite in save mode
        /// <paramref name="newline"/> will be used as newline symbol.
        /// when <paramref name="newLineAfter"/> is true after ParseData.Line also will be added <paramref name="newline"/>
        /// </summary>
        /// <param name="newline"></param>
        /// <param name="newLineAfter">last line must be empty</param>
        protected virtual void SaveModeAddLine(string newline = "\r\n", bool newLineAfter = false)
        {
            SaveModeAddLine(line: ParseData.Line, newline: newline, newLineAfter: newLineAfter);
        }

        /// <summary>
        /// add <paramref name="line"/> for wtite in save mode.
        /// <paramref name="newline"/> will be used as newline symbol.
        /// when <paramref name="newLineAfter"/> is true after <paramref name="line"/> also will be added <paramref name="newline"/>
        /// </summary>
        /// <param name="line"></param>
        /// <param name="newline"></param>
        /// <param name="newLineAfter">last line must be empty</param>
        protected virtual void SaveModeAddLine(string line, string newline = "\r\n", bool newLineAfter = false)
        {
            if (OpenFileMode) return;

            if (newLineAfter)
            {
                ParseData.ResultForWrite.Append(line + newline); // just paste newline symbol after line
                return;
            }

            if (firstline)
            {
                firstline = false; // add newline only after 1st line
            }
            else
            {
                ParseData.ResultForWrite.Append(lastNewline);
            }

            lastNewline = newline; // remember newline symbol to paste after current line

            ParseData.ResultForWrite.Append(line);
        }

        /// <summary>
        /// modification of translation before it will be added
        /// by default no modifications
        /// </summary>
        /// <param name=THSettings.TranslationColumnName></param>
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
            foreach (var pattern in Patterns()) if (ParsePattern(pattern)) ret = true;

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
            if (useInlineSearch && ParseData.Line.IndexOf(pattern.Key) == -1 || !Regex.IsMatch(ParseData.Line, pattern.Value, RegexOptions.Compiled))
            {
                return false;
            }

            var mc = Regex.Matches(ParseData.Line, pattern.Value, RegexOptions.Compiled);
            if (mc.Count <= 0) return false;

            var IsSet = false;
            if (OpenFileMode)
            {
                foreach (Match m in mc)
                {
                    var str = m.Result("$1");
                    IsSet = AddRowData(str, useInlineSearch ? pattern.Key : T._("Extracted with") + ":" + pattern.Value, isCheckInput: true);
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

        /// <summary>
        /// encoding for reading string file
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding ReadEncoding()
        {
            return GetStringFileEncoding();
        }

        /// <summary>
        /// encoding for writing string file
        /// </summary>
        /// <returns></returns>
        protected virtual Encoding WriteEncoding()
        {
            return GetStringFileEncoding();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (!ParseData.Ret) return false;
                if (!SaveFileMode) return false;
                if (ParseData.ResultForWrite.Length <= 0) return false;
                if (FunctionsFileFolder.FileInUse(GetSaveFilePath())) return false;

                File.WriteAllText(filePath.Length > 0 ? filePath : AppData.CurrentProject.IsSaveToSourceFile ? base.GetOpenFilePath() : GetSaveFilePath(), ParseData.ResultForWrite.ToString(), WriteEncoding());
                return true;
            }
            catch { }
            return false;
        }

        protected override void SetTranslationIsTranslatedAction()
        {
            ParseData.Ret = true;
        }

        internal class ParseFileData
        {
            FormatBase format;
            public ParseFileData(FormatBase format)
            {
                this.format = format;

                if (format.SaveFileMode) ResultForWrite = new StringBuilder();
            }

            /// <summary>
            /// result of parsing. Must be set to true if any value was translated.
            /// </summary>
            internal bool Ret { get => format.RET; set { if (format.RET != value) format.RET = value; } }
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
