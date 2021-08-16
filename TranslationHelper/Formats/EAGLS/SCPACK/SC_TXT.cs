using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Functions;

namespace TranslationHelper.Formats.EAGLS.SCPACK
{
    class SC_TXT : SCPACKBase
    {
        private const string StringPattern = "&[0-9]{1,6}\"([^\"\r\n]+)\"";//&17523"「はい……」"
        //#さくら=w0261a
        //#　花　=w0003f
        private const string StringPatternNames = @"#([^\=\r\n]+)(\=w[0-9]{4}[a-z])?";//#さくら=w0629a

        public SC_TXT()
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        internal override bool Open()
        {
            return ParseStringFile(); //OpenSC();
        }
        protected override Dictionary<string, string> Patterns()
        {
            return new Dictionary<string, string>()
            {
                { "#",
                    StringPatternNames },
                { "&",
                    StringPattern },
                //{ "(\"",
                //    @"[0-9]{1,6}\(\""[^\""]*\"",\""([^\""\r\n]+)\""\)" }, // 52(":NameSuffix","大地"),
                //{ "\")",
                //    @"[0-9]{1,6}\(\""([^\""\r\n]+)\""\)" } // 161("１１日目　後編選択")
            };
        }

        protected override void ParseStringFileLines()
        {
            var file = ParseData.reader.ReadToEnd();
            foreach (var pattern in Patterns())
            {
                var mc = Regex.Matches(file, pattern.Value);
                if (mc.Count == 0)
                {
                    continue;
                }

                if (ProjectData.OpenFileMode)
                {
                    foreach (Match match in mc)
                    {
                        AddRowData(match.Result("$1"), "", true);
                    }
                }
                else
                {
                    for (int i = mc.Count - 1; i >= 0; i--)
                    {
                        var value = mc[i].Result("$1");
                        if (IsValidString(value))
                        {
                            if (SetTranslation(ref value))
                            {
                                file = file.Remove(mc[i].Index, mc[i].Length).Insert(mc[i].Index, mc[i].Value.Replace(mc[i].Result("$1"), value));
                            }
                        }
                    }
                }
            }

            if (ProjectData.SaveFileMode)
            {
                ParseData.ResultForWrite.Append(file);
            }
        }

        //string lastMentionedCharacter = string.Empty;
        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {


            ParsePatterns();

            //old
            //if (ParseData.line.StartsWith("#"))
            //{
            //    if (!ParseData.line.Contains(":NameSuffix"))
            //    {
            //        ParseName();

            //        //old more manual parse
            //        //var last = ParseData.line.IndexOf('=');

            //        //var newline = (last != -1 ? ParseData.line.Substring(1, last - 1) : ParseData.line.Substring(1)).Trim();
            //        //lastMentionedCharacter = newline;
            //        //if (IsValidString(newline))
            //        //{
            //        //    if (ProjectData.OpenFileMode)
            //        //    {
            //        //        AddRowData(newline, "CharName", true, false);
            //        //    }
            //        //    else
            //        //    {
            //        //        if (TablesLinesDict.ContainsKey(newline))
            //        //        {
            //        //            ParseData.line = "#" + TablesLinesDict[newline].CleanForShiftJIS2004() + (last != -1 ? ParseData.line.Substring(last) : string.Empty);
            //        //            ParseData.Ret = true;
            //        //        }
            //        //    }
            //        //}
            //    }
            //    else
            //    {
            //        lastMentionedCharacter = ParseData.line.Remove(0, 1);
            //    }
            //}
            //else if (ParseData.line.Contains("&"))
            //{
            //    MatchCollection matches = Regex.Matches(ParseData.line, StringPattern);
            //    foreach (Match match in matches)
            //    {
            //        var val = match.Result("$1");
            //        var newMatch = match;
            //        if (IsValidString(val))
            //        {
            //            if (ProjectData.OpenFileMode)
            //            {
            //                AddRowData(val, lastMentionedCharacter.Length > 0 ? T._("Last character") + ": " + lastMentionedCharacter : string.Empty, true, false);
            //            }
            //            else
            //            {
            //                if (TablesLinesDict.ContainsKey(val))
            //                {
            //                    ParseData.line = ParseData.line.Replace(match.Value, newMatch.Value.Replace(val, TablesLinesDict[val].CleanForShiftJIS2004()).Trim());
            //                    ParseData.Ret = true;
            //                }
            //            }
            //        }
            //    }

            //    ParseName();
            //}

            SaveModeAddLine(ParseData.line.StartsWith("$") ? "\n" : "\r\n", false);//line starting with $ ends with \n

            return 0;
        }
        protected override string FixInvalidSymbols(string str)
        {
            /* For the record: most game scripts I had available don't use any text formatting, yet quite a few commands are supported:
(e) puts text on the next row
(y=・) furigana
(f=1) set font size to 1 for example, can be 0-9 for gothic and 10-19 for mincho, the mapping number<->fontsize varies between games
(c=r,g,b) text color; rgb values between 0-255
(s=r,g,b) text outline; rgb values between 0-255
(r) reset text formatting
<> escapes brackets, for example */

            str = base.FixInvalidSymbols(str);
            str = str.Replace(":NameSuffix", "％ＨＮ％");//hide hero name var
            str = str
                .Replace(",", "、")//scpack script have same symbol for scripts, changed to jp
                .Replace("\"", "”")//scpack script have same symbol for scripts, changed to jp
                .Replace("=", "＝")//scpack script have same symbol for scripts, changed to jp
                .Replace("～", "~")//unicode encode error
                .Replace("...", "…")//just for cosmetic reason?
                .Replace(".", "。")
                .Replace("'", "´")//scpack script have same symbol for scripts, changed to jp
                .Replace("!", "！")
                .Replace("?", "？")
                .Replace("%", "％")
                .Replace(":", "：")//scpack script have same symbol for scripts, changed to jp
                .Replace(";", "；")
                .Replace("&", "＆")//scpack script have same symbol for scripts, changed to jp
                .CleanForShiftJIS2004()
                ;
            //str = ENJPCharsReplacement(str);//convert en chars to jp
            str = str.Replace("％ＨＮ％", ":NameSuffix");//restore hero name var
            return str;
        }

        readonly string[][] ENtoJPReplacementPairs = new string[][] {
               new string[2]{ "a", "ａ" },
               new string[2]{ "A", "Ａ" },
               new string[2]{ "b", "ｂ" },
               new string[2]{ "B", "Ｂ" },
               new string[2]{ "c", "ｃ" },
               new string[2]{ "C", "Ｃ" },
               new string[2]{ "d", "ｄ" },
               new string[2]{ "D", "Ｄ" },
               new string[2]{ "e", "ｅ" },
               new string[2]{ "E", "Ｅ" },
               new string[2]{ "f", "ｆ" },
               new string[2]{ "F", "Ｆ" },
               new string[2]{ "g", "ｇ" },
               new string[2]{ "G", "Ｇ" },
               new string[2]{ "h", "ｈ" },
               new string[2]{ "H", "Ｈ" },
               new string[2]{ "i", "ｉ" },
               new string[2]{ "I", "Ｉ" },
               new string[2]{ "j", "ｊ" },
               new string[2]{ "J", "Ｊ" },
               new string[2]{ "k", "ｋ" },
               new string[2]{ "K", "Ｋ" },
               new string[2]{ "l", "ｌ" },
               new string[2]{ "L", "Ｌ" },
               new string[2]{ "m", "ｍ" },
               new string[2]{ "M", "Ｍ" },
               new string[2]{ "n", "ｎ" },
               new string[2]{ "N", "Ｎ" },
               new string[2]{ "o", "ｏ" },
               new string[2]{ "O", "Ｏ" },
               new string[2]{ "p", "ｐ" },
               new string[2]{ "P", "Ｐ" },
               new string[2]{ "q", "ｑ" },
               new string[2]{ "Q", "Ｑ" },
               new string[2]{ "r", "ｒ" },
               new string[2]{ "R", "Ｒ" },
               new string[2]{ "s", "ｓ" },
               new string[2]{ "S", "Ｓ" },
               new string[2]{ "t", "ｔ" },
               new string[2]{ "T", "Ｔ" },
               new string[2]{ "u", "ｕ" },
               new string[2]{ "U", "Ｕ" },
               new string[2]{ "v", "ｖ" },
               new string[2]{ "V", "Ｖ" },
               new string[2]{ "w", "ｗ" },
               new string[2]{ "W", "Ｗ" },
               new string[2]{ "x", "ｘ" },
               new string[2]{ "X", "Ｘ" },
               new string[2]{ "y", "ｙ" },
               new string[2]{ "Y", "Ｙ" },
               new string[2]{ "z", "ｚ" },
               new string[2]{ "Z", "Ｚ" },
               //new string[2]{ ", ", "、" },
               //new string[2]{ ",", "、" },
               //new string[2]{ ". ", "。" },
               //new string[2]{ ".", "。" },
               //new string[2]{ " ... ", "…" },
               //new string[2]{ "... ", "…" },
               //new string[2]{ " ...", "…" },
               //new string[2]{ "...", "…" },
               //new string[2]{ "...", "…" },
               //new string[2]{ "……", "…" },
               //new string[2]{ "……", "…" },
               // new string[2]{ "……", "…" },
               // new string[2]{ "……", "…" },
               //new string[2]{ " … ", "…" },
               //new string[2]{ "… ", "…" },
               //new string[2]{ " …", "…" },
               // new string[2]{ "。。", "。" },
               // new string[2]{ "。。", "。" },
               //new string[2]{ " \" ", " " },
               //new string[2]{ "\" ", " " },
               //new string[2]{ " \"", " " },
               //new string[2]{ "\"", string.Empty },
                //new string[2]{ " ” ", " " },
                //new string[2]{ " ”", " " },
                //new string[2]{ "” ", " " },
                //new string[2]{ "”", " " },
               //new string[2]{ "\"", "”" },
               //new string[2]{ " ~ ", " " },
               //new string[2]{ "_~", string.Empty },
               //new string[2]{ "? ", "？" },
               //new string[2]{ "! ", "！" },
               //new string[2]{ " '", string.Empty },
               //new string[2]{ " ’", string.Empty },
               //new string[2]{ "'", string.Empty },
               //new string[2]{ "’", string.Empty },
               //new string[2]{ "{", string.Empty },
               //new string[2]{ "}", string.Empty },
               //new string[2]{ " [", " " },
               //new string[2]{ "] ", " " },
               //new string[2]{ "[", " " },
               //new string[2]{ "]", " " },
               //new string[2]{ " [", "【" },
               //new string[2]{ "] ", "】" },
               //new string[2]{ "[", "【" },
               //new string[2]{ "]", "】" },
               //new string[2]{ "#", string.Empty },
               //new string[2]{ "「", " " },
               //new string[2]{ "『", " " },
               //new string[2]{ "」", " " },
               //new string[2]{ "』", " " },
               //new string[2]{ "$", string.Empty },
               //new string[2]{ "@", string.Empty },
               //new string[2]{ "/", "／" },
               //new string[2]{ "\\", "＼" },
               //new string[2]{ " (", "（" },
               //new string[2]{ ") ", "）" },
               //new string[2]{ "(", "（" },
               //new string[2]{ ")", "）" },
               //new string[2]{ ":", "：" },
               //new string[2]{ ";", "；" },
               //new string[2]{ "*", "＊" },
               //new string[2]{ " '", "´" },
               //new string[2]{ " ’", "´" },,
               //new string[2]{ "'", "´" },
               //new string[2]{ "’", "´" },
               //new string[2]{ "#", "＃" },
               //new string[2]{ "$", "＄" },
               //new string[2]{ "%", "％" },
               //new string[2]{ "&", "＆" },
               //new string[2]{ ",", "，" },
               //new string[2]{ "@", "＠" },
               //new string[2]{ "[", "［" },
               //new string[2]{ "[", "［" },
               //new string[2]{ "^", "＾" },
               //new string[2]{ "_", "＿" },
               //new string[2]{ "~", "～" },
               //new string[2]{ "¨", "￣" },
               //new string[2]{ "\"", "〃" },
               //new string[2]{ " ", "・" }
               //new string[2]{ "　", " " },
               //new string[2]{ "  ", " " },
               //new string[2]{ "  ", " " },
               //new string[2]{ " ", "_" }
    };

        private string ENJPCharsReplacement(string workString)
        {
            return FunctionsString.CharsReplacementByPairsFromArray(workString, ENtoJPReplacementPairs);
        }

        //old
        //private void ParseName()
        //{
        //    //#さくら=w0261a
        //    //#　花　=w0003f
        //    //#さくら
        //    var matches = Regex.Matches(ParseData.line, StringPatternNames);
        //    foreach (Match match in matches)
        //    {
        //        var val = match.Result("$1");
        //        var newMatch = match;
        //        if (IsValidString(val) && !val.Contains(":NameSuffix"))
        //        {
        //            var valtrimmed = val.Trim();
        //            if (ProjectData.OpenFileMode)
        //            {
        //                AddRowData(valtrimmed, T._("CharName"), true);
        //            }
        //            else
        //            {
        //                if (TablesLinesDict.ContainsKey(valtrimmed))
        //                {
        //                    ParseData.line = ParseData.line.Replace(match.Value, newMatch.Value.Replace(val, TablesLinesDict[valtrimmed].CleanForShiftJIS2004()));
        //                    ParseData.Ret = true;
        //                }
        //            }
        //        }
        //    }
        //}

        //old
        ///// <summary>
        ///// old
        ///// </summary>
        ///// <returns></returns>
        //private bool OpenSC()
        //{
        //    string fileName = Path.GetFileNameWithoutExtension(ProjectData.FilePath);

        //    try
        //    {
        //        AddTables(fileName);
        //        string lastMentionedCharacter = string.Empty;
        //        using (StreamReader sr = new StreamReader(ProjectData.FilePath))
        //        {
        //            string line;
        //            while (!sr.EndOfStream)
        //            {
        //                line = sr.ReadLine();

        //                if (line.StartsWith("#"))
        //                {
        //                    if (!line.Contains(":NameSuffix"))
        //                    {
        //                        //#さくら=w0261a
        //                        //#　花　=w0003f
        //                        var last = line.IndexOf('=');
        //                        var newline = (last != -1 ? line.Substring(1, last - 1) : line.Substring(1)).Trim();
        //                        lastMentionedCharacter = newline;
        //                        if (IsValid(newline))
        //                        {
        //                            AddRowData(fileName, newline, "CharName", true);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        lastMentionedCharacter = line.Remove(0, 1);
        //                    }
        //                }
        //                else if (line.Contains("&"))
        //                {
        //                    MatchCollection matches = Regex.Matches(line, StringPattern);
        //                    foreach (Match match in matches)
        //                    {
        //                        var val = match.Result("$1");
        //                        if (IsValid(val))
        //                        {
        //                            AddRowData(fileName, val, lastMentionedCharacter.Length > 0 ? T._("Last char: ") + lastMentionedCharacter : string.Empty, true);
        //                        }
        //                    }

        //                    matches = Regex.Matches(line, StringPatternNames);
        //                    foreach (Match match in matches)
        //                    {
        //                        var val = match.Result("$1");
        //                        if (IsValid(val) && !val.Contains(":NameSuffix"))
        //                        {
        //                            AddRowData(fileName, val.Trim(), T._("CharName"), true);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }

        //    return CheckTablesContent(fileName);
        //}

        //private static bool IsValid(string line)
        //{
        //    return !FunctionsRomajiKana.LocalePercentIsNotValid(line);
        //}

        internal override bool Save()
        {
            return ParseStringFile();// SaveSC();
        }

        //old
        //private bool SaveSC()
        //{
        //    string fileName = Path.GetFileNameWithoutExtension(ProjectData.FilePath);
        //    SplitTableCellValuesAndTheirLinesToDictionary(fileName, false, false);
        //    StringBuilder Translated = new StringBuilder();
        //    bool changed = false;
        //    try
        //    {
        //        using (StreamReader sr = new StreamReader(ProjectData.FilePath))
        //        {
        //            string line;
        //            while (!sr.EndOfStream)
        //            {
        //                line = sr.ReadLine();

        //                if (line.StartsWith("#"))
        //                {
        //                    if (!line.Contains(":NameSuffix"))
        //                    {
        //                        //#さくら=w0261a
        //                        //#　花　=w0003f
        //                        var last = line.IndexOf('=');
        //                        var val = (last != -1 ? line.Substring(1, last - 1) : line.Substring(1)).Trim();
        //                        if (IsValid(val))
        //                        {
        //                            var valtrimmed = val.Trim();
        //                            if (TablesLinesDict.ContainsKey(valtrimmed))
        //                            {
        //                                line = "#" + TablesLinesDict[valtrimmed].CleanForShiftJIS2004() + (last != -1 ? line.Substring(last) : string.Empty);
        //                                if (!changed)
        //                                    changed = true;
        //                            }
        //                        }
        //                    }
        //                }
        //                else if (line.Contains("&"))
        //                {
        //                    MatchCollection matches = Regex.Matches(line, StringPattern);
        //                    foreach (Match match in matches)
        //                    {
        //                        var newMatch = match;
        //                        var val = newMatch.Result("$1");
        //                        if (IsValid(val))
        //                        {
        //                            if (TablesLinesDict.ContainsKey(val))
        //                            {
        //                                line = line.Replace(match.Value, newMatch.Value.Replace(val, TablesLinesDict[val].CleanForShiftJIS2004()).Trim());
        //                                if (!changed)
        //                                    changed = true;
        //                            }
        //                        }
        //                    }

        //                    matches = Regex.Matches(line, StringPatternNames);
        //                    foreach (Match match in matches)
        //                    {
        //                        var newMatch = match;
        //                        var val = newMatch.Result("$1");
        //                        if (IsValid(val) && !val.Contains(":NameSuffix"))
        //                        {
        //                            var valtrimmed = val.Trim();
        //                            if (TablesLinesDict.ContainsKey(valtrimmed))
        //                            {
        //                                line = line.Replace(match.Value, newMatch.Value.Replace(val, TablesLinesDict[valtrimmed].CleanForShiftJIS2004()));
        //                                if (!changed)
        //                                    changed = true;
        //                            }
        //                        }
        //                    }

        //                }
        //                Translated.AppendLine(line);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }

        //    if (changed)
        //    {
        //        File.WriteAllText(ProjectData.FilePath, Translated.ToString());
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
