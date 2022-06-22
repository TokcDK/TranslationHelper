using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.KiriKiri;
using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    abstract class KSParserBase : KiriKiriBase
    {
        public KSParserBase()
        {
            scriptMark = new Script();
            Tag = new TAG();
            textOnMessage = new List<string>();
        }

        internal override string Ext => ".ks";

        bool IsScript = false;
        bool TextOn = false;
        readonly Script scriptMark;
        readonly TAG Tag;
        readonly List<string> textOnMessage;
        readonly KiriKiri.Games.KSSyntax.Attribute Attr = new KiriKiri.Games.KSSyntax.Attribute();
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.Line.Contains("ゲームにもどる"))
            {

            }

            string tff = "";

            if (IsScript)
            {
                if (Regex.IsMatch(ParseData.Line, scriptMark.EndsWith))
                {
                    IsScript = false;
                }
            }

            if (ParseData.IsComment)
            {
                if (ParseData.TrimmedLine.Contains("*/")) //comment section end
                {
                    ParseData.IsComment = false;
                }
            }
            //else if (IsScript)
            //{
            //    if (Regex.IsMatch(ParseData.Line, scriptMark.EndsWith))
            //    {
            //        IsScript = false;
            //    }
            //}
            //else if (TextOn)
            //{
            //    if (ParseData.TrimmedLine == "[textoff]")
            //    {
            //        var joined = string.Join(Environment.NewLine, textOnMessage);
            //        if (IsValidString(joined))
            //        {
            //            if (ProjectData.OpenFileMode)
            //            {
            //                AddRowData(joined, "TextOn block message", CheckInput: false);
            //            }
            //            else
            //            {
            //                SetTranslation(ref joined);
            //            }
            //        }

            //        if (ProjectData.SaveFileMode)
            //        {
            //            ParseData.Line = joined + ParseData.Line;
            //        }

            //        TextOn = false;
            //    }
            //    else
            //    {
            //        textOnMessage.Add(ParseData.Line);
            //        return KeywordActionAfter.Continue;
            //    }
            //}
            else if (Regex.IsMatch(ParseData.Line, scriptMark.StartsWith))
            {
                IsScript = true;
            }
            //else if (ParseData.TrimmedLine == "[texton]")
            //{
            //    textOnMessage.Clear();
            //    TextOn = true;
            //}
            else
            {
                if (ParseData.TrimmedLine.StartsWith("/*")) //comment section start
                {
                    if (!ParseData.TrimmedLine.EndsWith("*/"))
                    {
                        ParseData.IsComment = true;
                    }
                }
                else if (string.IsNullOrWhiteSpace(ParseData.Line) || ParseData.TrimmedLine.StartsWith("//") || ParseData.TrimmedLine.StartsWith(";")) //comment
                {
                }
                else if (ParseData.TrimmedLine.StartsWith("*")) //label
                {
                    //var labelWithInfo = ParseData.Line.Split('|');
                    //if (labelWithInfo.Length == 2)
                    //{
                    //    if(AddRowData(ref labelWithInfo[1], ParseData.Line) && ProjectData.SaveFileMode)
                    //    {
                    //        ParseData.Line = string.Join("|", labelWithInfo);
                    //    }
                    //}
                }
                //else if (ParseData.Line.StartsWith("[glink")
                //    || ParseData.Line.StartsWith("[ptext")
                //    || ParseData.Line.StartsWith("[mtext")
                //    || (ParseData.Line.Contains("text=") && Regex.IsMatch(ParseData.Line, @"^\t*\[[a-zA-Z] "))
                //    )
                //{
                //    var glinkStringData = Regex.Matches(ParseData.Line, @"text\=\""([^\""\r\n\\]+(?:\\.[^\""\\]*)*)\""");//attributename="attributevalue"

                //    if (glinkStringData.Count > 0)
                //    {
                //        for (int i = glinkStringData.Count - 1; i >= 0; i--)
                //        {
                //            var value = glinkStringData[i].Result("$1");
                //            if (IsValidString(value))
                //            {
                //                if (ProjectData.OpenFileMode)
                //                {
                //                    AddRowData(value, "(glink/ptext/mtext text regex)\r\n" + ParseData.Line, CheckInput: false);
                //                }
                //                else
                //                {
                //                    var trans = value;
                //                    if (SetTranslation(ref trans))
                //                    {
                //                        ParseData.Line = ParseData.Line
                //                            .Remove(glinkStringData[i].Index, glinkStringData[i].Length)
                //                            .Insert(glinkStringData[i].Index, "text=\"" + trans + "\"");
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //else if (Regex.IsMatch(ParseData.Line, @"emb\s*\=\s*\""([^\""]*)\"""))
                //{
                //    var attributeMatches = Regex.Matches(ParseData.Line, @"emb+\s*\=\s*\""([^\""]*)\""");

                //    for (int i = attributeMatches.Count - 1; i >= 0; i--)
                //    {
                //        var attributeMatch = attributeMatches[i];

                //        //if (!attributeMatch.Value.StartsWith("emb=")) continue;

                //        var attributeValue = attributeMatch.Groups[1].Value;

                //        //if (!IsValidString(attributeValue)) continue;

                //        if (AddRowData(ref attributeValue, "(emb regex)\r\n" + ParseData.Line, CheckInput: false) && ProjectData.SaveFileMode)
                //        {
                //            int index = attributeMatch.Groups[1].Index;
                //            ParseData.Line = ParseData.Line
                //                .Remove(index, attributeMatch.Groups[1].Length)
                //                .Insert(index, attributeValue);
                //        }
                //    }
                //}
                else if (Regex.IsMatch(ParseData.Line, @"(?<!\[)\[\s*\w+\s+"))
                {
                    if (ParseData.Line.Contains("\t\t\t\tdm(tabs+'    '+objstr+'['+keyary[i]+'] = ' + obj[keyary[i]]);"))
                    {

                    }
                    if (ParseData.Line.Contains("emb exp=\"f.愛情レベル\"]になった。"))
                    {

                    }

                    // get tag begin indexes
                    var tagIndexes = new List<BracketCoordinates>();
                    for (int i = ParseData.Line.IndexOf('['); i > -1; i = ParseData.Line.IndexOf('[', i + 1)) tagIndexes.Add(new BracketCoordinates(i));
                    
                    //foreach (Match tagBegin in Regex.Matches(ParseData.Line, @"(?<!\[)\[\s*\w+\s+")) tagIndexes.Add(new BracketCoordinates(tagBegin.Index));

                    // get tag end indexes
                    //var tagEndIndexes = new List<int>();
                    //old way
                    //string searchLine = ParseData.Line;
                    //for (int i = tagBeginIndexes.Count - 1; i >= 0; i--)
                    //{
                    //    int tagBeginIndex = tagBeginIndexes[i];
                    //    int reversSearchStartIndex = searchLine.Length - 1;
                    //    var tagEndIndex = searchLine.LastIndexOf(']', reversSearchStartIndex);

                    //    if (tagEndIndex == -1) tagEndIndex = reversSearchStartIndex;
                    //    tagEndIndexes.Add(tagEndIndex);

                    //    searchLine = searchLine.Remove(tagBeginIndex);
                    //}
                    //tagEndIndexes = tagEndIndexes.OrderBy(i => i).ToList();

                    foreach(var tagIndex in tagIndexes)
                    {
                        tagIndex.CloseIndex = ParseData.Line.GetClosingBraketIndexFor(tagIndex.OpenIndex);
                    }
                    foreach (var tagIndex in tagIndexes)
                    {
                        foreach (var tagIndex1 in tagIndexes)
                        {
                            if (tagIndex1 == tagIndex) continue;

                            if (tagIndex1.OpenIndex > tagIndex.OpenIndex && tagIndex1.CloseIndex < tagIndex.CloseIndex)
                            {
                                tagIndex1.Parent.Add(tagIndex);
                            }
                        }
                    }

                    var parts = new List<LinePart>();
                    int startIndex = 0;
                    int ii = 0;
                    foreach (var tagIndex in tagIndexes)
                    {
                        if (tagIndex.Parent.Any()) continue; // skip subbrackets
                        if (tagIndex.CloseIndex==-1) continue; // alone bracked

                        if (tagIndex.OpenIndex > startIndex)
                        {
                            parts.Add(new LinePart(ParseData.Line.Substring(startIndex, tagIndex.OpenIndex - startIndex), true));
                        }

                        parts.Add(new LinePart(ParseData.Line.Substring(tagIndex.OpenIndex, tagIndex.CloseIndex + 1 - tagIndex.OpenIndex), false));
                        startIndex = tagIndex.CloseIndex + 1;
                    }
                    if (startIndex != ParseData.Line.Length)
                    {
                        parts.Add(new LinePart(ParseData.Line.Substring(startIndex), true));
                    }

                    foreach (var part in parts)
                    {
                        if (part.IsString)
                        {
                            var value = part.Value;
                            if (AddRowData(ref value, ParseData.Line) && AppData.CurrentProject.SaveFileMode)
                            {
                                part.Value = value;
                            }
                        }
                        else // if tag
                        {
                            var r = new Regex(Attr.StartsWith);
                            var mc = r.Matches(part.Value);

                            if (mc.Count == 0 && Regex.IsMatch(part.Value, @"\]([^\[]+)\[endlink\]"))
                            {
                                r = new Regex(@"\]([^\[]+)\[endlink\]"); // link
                                var match = r.Match(part.Value);

                                var mValue = match.Groups[1].Value;

                                if (AddRowData(ref mValue, "(tag regex:" + r.ToString() + ")\r\n" + ParseData.Line) && AppData.CurrentProject.SaveFileMode)
                                {
                                    int index = match.Groups[1].Index;
                                    part.Value = part.Value
                                        .Remove(index, match.Groups[1].Length)
                                        .Insert(index, mValue);
                                }
                            }
                            else
                            {
                                for (int i = mc.Count - 1; i >= 0; i--)
                                {
                                    var match = mc[i];

                                    if (!Regex.IsMatch(match.Value, @"(emb|text)\s*=\s*")) continue;

                                    var mValue = match.Groups[1].Value;

                                    if (AddRowData(ref mValue, "(tag regex:" + r.ToString() + ")\r\n" + ParseData.Line) && AppData.CurrentProject.SaveFileMode)
                                    {
                                        int index = match.Groups[1].Index;
                                        part.Value = part.Value
                                            .Remove(index, match.Groups[1].Length)
                                            .Insert(index, mValue);
                                    }
                                }
                            }
                        }
                    }

                    if (AppData.CurrentProject.SaveFileMode) ParseData.Line = string.Join("", parts.Select(p => p.Value));
                }
                else if (new Regex(Tag.StartsWith).IsMatch(ParseData.Line) && string.IsNullOrWhiteSpace(tff = new Regex(Tag.StartsWith).Replace(new Regex(@"[A-Za-z]+\s*\=\s*([^\s]*)").Replace(ParseData.Line, ""), "").TrimEnd(']')))
                {
                    // remove tag and attribute and check if result string is empty
                }
                else if (Regex.IsMatch(ParseData.Line, @"\]([^\[]+)\[endlink\]")
                    || Regex.IsMatch(ParseData.Line, @"\""[^\""]+\""\=\>\""([^\""]+)\""")
                    || Regex.IsMatch(ParseData.Line, @"\s*\=\s*\""([^\&\*\""\=\n\r\]\[\)\(\.a-zA-Z]*)\""")
                    )
                {
                    foreach (Regex r in new Regex[] {
                        new Regex(@"\]([^\[]+)\[endlink\]"), // link
                        new Regex(@"\""[^\""]+\""\=\>\""([^\""]+)\"""), // in quotes after lambda
                        new Regex(@"\s*\=\s*\""([^\&\*\""\=\n\r\]\[\)\(\.a-zA-Z]*)\"""), // any in quotes
                    })
                    {
                        var mc = r.Matches(ParseData.Line);

                        for (int i = mc.Count - 1; i >= 0; i--)
                        {
                            var match = mc[i];

                            var mValue = match.Groups[1].Value;

                            if (AddRowData(ref mValue, "(Regex:" + r.ToString() + ")\r\n" + ParseData.Line) && AppData.CurrentProject.SaveFileMode)
                            {
                                int index = match.Groups[1].Index;
                                ParseData.Line = ParseData.Line
                                    .Remove(index, match.Groups[1].Length)
                                    .Insert(index, mValue);
                            }
                        }
                    }
                }
                //else if (Regex.IsMatch(ParseData.Line, Tag.StartsWith))
                //{
                //    if (Tag.Include() != null)
                //    {
                //        foreach (var attributePattern in Tag.Include())
                //        {
                //            if (attributePattern == null)
                //            {
                //                continue;
                //            }

                //            var attributeMatches = Regex.Matches(ParseData.Line, attributePattern.StartsWith);

                //            for (int i = attributeMatches.Count - 1; i >= 0; i--)
                //            {
                //                var attributeMatch = attributeMatches[i];

                //                var attributeValue = attributeMatch.Groups[1].Value;

                //                if (!attributeValue.StartsWith("emb=")) continue;

                //                if (!IsValidString(attributeValue))
                //                {
                //                    continue;
                //                }

                //                if (ProjectData.OpenFileMode)
                //                {
                //                    AddRowData(attributeValue, "", CheckInput: false);
                //                }
                //                else
                //                {
                //                    var trans = attributeValue;
                //                    if (SetTranslation(ref trans))
                //                    {
                //                        int index = attributeMatch.Groups[1].Index;
                //                        ParseData.Line = ParseData.Line
                //                            .Remove(index, attributeMatch.Groups[1].Length)
                //                            .Insert(index, trans);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                else if (!IsScript && !string.IsNullOrWhiteSpace(ParseData.Line) && !ParseData.Line.StartsWith("@") && !ParseData.Line.StartsWith("*") /*&& ParseData.line.StartsWith("[link") || !ParseData.line.StartsWith("[")*/)
                {
                    //int brackedIndex = -1;
                    //if ((brackedIndex = ParseData.Line.IndexOf('[')) != -1)
                    //{
                    //    //int startIndex = 0;
                    //    //var parts = new List<LinePart>();

                    //    //if (brackedIndex > startIndex)
                    //    //{
                    //    //    var value = ParseData.Line.Substring(startIndex, brackedIndex - startIndex);
                    //    //    parts.Add(new LinePart(value, value.StartsWith("[") || value.EndsWith("]")));
                    //    //}

                    //    //int closingBraketIndex = ParseData.Line.IndexOf(']', startIndex);
                    //    //while (brackedIndex != -1)
                    //    //{
                    //    //    var value = ParseData.Line.Substring(startIndex, brackedIndex - startIndex);
                    //    //    parts.Add(new LinePart(value, value.StartsWith("[") || value.EndsWith("]")));

                    //    //    brackedIndex = ParseData.Line.IndexOf('[', startIndex);
                    //    //}



                    //    int startIndex = 0;
                    //    var parts = new List<LinePart>();
                    //    if (brackedIndex > startIndex)
                    //    {
                    //        var value = ParseData.Line.Substring(0, brackedIndex);
                    //        parts.Add(new LinePart(value, !value.StartsWith("[") && !value.EndsWith("]")));

                    //        startIndex = brackedIndex;
                    //    }

                    //    int brackedOpenedCount = 0;
                    //    int brackedClosedCount = 0;
                    //    int length = ParseData.Line.Length;
                    //    for (int i = startIndex; i < length; i++)
                    //    {
                    //        var c = ParseData.Line[i];

                    //        if (c == '[' && ParseData.Line[i - 1] != '[')
                    //        {
                    //            brackedOpenedCount++;
                    //        }
                    //        else if (c == ']')
                    //        {
                    //            brackedClosedCount++;
                    //        }

                    //        if (i > startIndex)
                    //        {
                    //            if (c == '[')
                    //            {
                    //                var value = ParseData.Line.Substring(startIndex, i - startIndex);
                    //                parts.Add(new LinePart(value, !value.StartsWith("[") && !value.EndsWith("]")));
                    //                startIndex = i;
                    //            }
                    //            else if (brackedOpenedCount == brackedClosedCount)
                    //            {
                    //                var value = ParseData.Line.Substring(startIndex, i + 1 - startIndex);
                    //                parts.Add(new LinePart(value, !value.StartsWith("[") && !value.EndsWith("]")));
                    //                startIndex = i + 1;
                    //            }
                    //        }
                    //        else if (i == length - 1)
                    //        {
                    //            var value = ParseData.Line.Substring(startIndex, i + 1 - startIndex);
                    //            parts.Add(new LinePart(value, !value.StartsWith("[") && !value.EndsWith("]")));
                    //            startIndex = i + 1;
                    //        }

                    //        if (i > startIndex || i == length - 1)
                    //        {
                    //            if (c == '[')
                    //            {
                    //                var value = ParseData.Line.Substring(startIndex, i - startIndex);
                    //                parts.Add(new LinePart(value, !value.StartsWith("[") && !value.EndsWith("]")));
                    //                startIndex = i;
                    //            }
                    //            else if (brackedOpenedCount == brackedClosedCount || i == length - 1)
                    //            {
                    //                var value = ParseData.Line.Substring(startIndex, i + 1 - startIndex);
                    //                parts.Add(new LinePart(value, !value.StartsWith("[") && !value.EndsWith("]")));
                    //                startIndex = i + 1;
                    //            }

                    //        }

                    //        bool closeBracketAlone = brackedClosedCount > brackedOpenedCount;
                    //        if (closeBracketAlone || (i > startIndex && (brackedOpenedCount == brackedClosedCount || ParseData.Line[startIndex] != '[')) || i == length - 1)
                    //        {
                    //            if (ParseData.Line.Contains("[jj]【サキ】「[落]……うん、大丈夫ですよ」"))
                    //            {

                    //            }
                    //            if (closeBracketAlone) brackedClosedCount = brackedOpenedCount;

                    //            var value = ParseData.Line.Substring(startIndex, i - startIndex);
                    //            parts.Add(new LinePart(value, !value.StartsWith("[") && !value.EndsWith("]")));
                    //            startIndex = i;
                    //        }
                    //    }

                    //    int index = parts.Count;
                    //    List<string> newLineParts = ProjectData.OpenFileMode ? null : new List<string>();
                    //    foreach (var part in parts)
                    //    {
                    //        if (!part.IsString) continue;

                    //        var value = part.Value;

                    //        if (AddRowData(ref value, ParseData.Line, CheckInput: false) && ProjectData.SaveFileMode) part.Value = value;
                    //    }

                    //    if (ProjectData.SaveFileMode) ParseData.Line = string.Join("", parts.Select(p => p.Value));
                    //}
                    //else
                    //{
                    //    AddRowData(ref ParseData.Line, ParseData.Line, CheckInput: true);
                    //}

                    var mc = Regex.Matches(ParseData.Line, @"(\[*([^\[\]\r\n]+(?:\\.[^\[\]]*)*)\]*)");

                    if (mc.Count > 0)
                    {
                        var parts = new List<string>();
                        for (int i = 0; i < mc.Count; i++)
                        {
                            var value = mc[i].Value;

                            if (value.StartsWith("[") || value.EndsWith("]") || value.StartsWith(".") || value.StartsWith("="))
                            {
                            }
                            else
                            {
                                AddRowData(ref value, "(Line part)\r\n" + ParseData.Line, CheckInput: true);
                            }

                            parts.Add(value);
                        }

                        if (AppData.CurrentProject.SaveFileMode) ParseData.Line = string.Join("", parts);
                    }
                }
                //else if(!IsScript)
                //{

                //    // add full line when no tag bracket found
                //    if (ParseData.Line.IndexOf('[') == -1)
                //    {
                //        AddRowData(ref ParseData.Line);

                //        Saveline();
                //        return KeywordActionAfter.Continue;
                //    }

                //    // get line parts and strings from it
                //    var morphedLine = new Regex(@"\[[a-z]+\]").Replace(ParseData.Line, string.Empty);
                //    morphedLine = new Regex(@"\[.\]").Replace(morphedLine, string.Empty);
                //    if (string.IsNullOrWhiteSpace(morphedLine)) { Saveline(); return KeywordActionAfter.Continue; }
                //    if (morphedLine.Contains("[")) { Saveline(); return KeywordActionAfter.Continue; }

                //    var regex = new Regex(@"\[[^\]]+\]");
                //    List<LinePart> lineParts = new List<LinePart>();
                //    int stringIndex = 0;
                //    var matches = regex.Matches(ParseData.Line); // any tags
                //    foreach (Match m in matches)
                //    {
                //        if (m.Index > stringIndex)
                //        {
                //            string s = ParseData.Line.Substring(stringIndex, m.Index - stringIndex);
                //            lineParts.Add(new LinePart(s, true));
                //        }

                //        if (ProjectData.SaveFileMode) lineParts.Add(new LinePart(m.Value, false)); // add only in save mode

                //        stringIndex = (m.Index + m.Length); // move position right after match
                //    }
                //    if (stringIndex != ParseData.Line.Length)
                //    {
                //        // add string in last part of line
                //        string s = ParseData.Line.Substring(stringIndex);
                //        lineParts.Add(new LinePart(s, true));
                //    }
                //    int index = lineParts.Count;
                //    List<string> newLineParts = ProjectData.OpenFileMode ? null : new List<string>();
                //    foreach (var part in lineParts)
                //    {
                //        if (!part.IsString) continue;

                //        var value = part.Value;

                //        if (AddRowData(ref value, ParseData.Line, CheckInput: false) && ProjectData.SaveFileMode) part.Value = value;
                //    }

                //    if (ProjectData.SaveFileMode) ParseData.Line = string.Join("", lineParts.Select(p => p.Value));
                //}
            }


            Saveline();

            return KeywordActionAfter.Continue;
        }
        public class LinePart
        {
            public string Value { get; set; }

            public bool IsString { get; }
            public LinePart(string value, bool isString)
            {
                Value = value;
                IsString = isString;
            }
        }

        void Saveline()
        {
            SaveModeAddLine(newline: Environment.NewLine);//using \n as new line
        }

        private static string Cleaned(string line)
        {
            return Regex.Replace(line, @"\[[^\]]+\]", "");
        }

        internal virtual Encoding FileEncoding()
        {
            return new UTF8Encoding(false);
        }
    }
}
