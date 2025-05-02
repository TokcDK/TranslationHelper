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

        public override string Extension => ".ks";

        bool IsScript = false;
        //bool TextOn = false;
        readonly Script scriptMark;
        readonly TAG Tag;
        readonly List<string> textOnMessage;
        readonly KiriKiri.Games.KSSyntax.Attribute Attr = new KiriKiri.Games.KSSyntax.Attribute();
        protected override KeywordActionAfter ParseStringFileLine()
        {
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
            else if (Regex.IsMatch(ParseData.Line, scriptMark.StartsWith))
            {
                IsScript = true;
            }
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
                }
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
                            if (AddRowData(ref value, ParseData.Line) && SaveFileMode)
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

                                if (AddRowData(ref mValue, "(tag regex:" + r.ToString() + ")\r\n" + ParseData.Line) && SaveFileMode)
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

                                    if (AddRowData(ref mValue, "(tag regex:" + r.ToString() + ")\r\n" + ParseData.Line) && SaveFileMode)
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

                    if (SaveFileMode) ParseData.Line = string.Join("", parts.Select(p => p.Value));
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

                            if (AddRowData(ref mValue, "(Regex:" + r.ToString() + ")\r\n" + ParseData.Line) && SaveFileMode)
                            {
                                int index = match.Groups[1].Index;
                                ParseData.Line = ParseData.Line
                                    .Remove(index, match.Groups[1].Length)
                                    .Insert(index, mValue);
                            }
                        }
                    }
                }                
                else if (!IsScript && !string.IsNullOrWhiteSpace(ParseData.Line) && !ParseData.Line.StartsWith("@") && !ParseData.Line.StartsWith("*") /*&& ParseData.line.StartsWith("[link") || !ParseData.line.StartsWith("[")*/)
                {                   

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
                                AddRowData(ref value, "(Line part)\r\n" + ParseData.Line, isCheckInput: true);
                            }

                            parts.Add(value);
                        }

                        if (SaveFileMode) ParseData.Line = string.Join("", parts);
                    }
                }
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

        internal override Encoding FileEncoding()
        {
            return new UTF8Encoding(false);
        }
    }
}
