﻿using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Functions;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSQuotedStringsBase : JSBase
    {
        protected JSQuotedStringsBase()
        {
        }

        /// <summary>
        /// pattern which will be placed before quoted pattern
        /// </summary>
        protected virtual string PreQuoteRegexPattern => "";

        readonly string[] _quotesList = new[] { "'", "`", @"\""" };
        readonly string[] _commentMark = new[] { "//" };
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!IsEmptyOrComment())
            {
                var quotesExtractor = new QuotedTextExtractor(ParseData.Line, removeComment: true);

                bool isChanged = false;
                foreach (var quotedString in quotesExtractor.Extract())
                {
                    if (OpenFileMode)
                    {
                        AddRowData(quotedString, ParseData.Line, isCheckInput: false);
                    }
                    else
                    {
                        string translation = quotedString;
                        if (!SetTranslation(ref translation, isCheckInput: false)) continue;

                        isChanged = true;

                        quotesExtractor.ReplaceLastExtractedString(translation);
                    }
                }

                if (isChanged) ParseData.Line = quotesExtractor.ResultString;
            }

            SaveModeAddLine(newline: System.Environment.NewLine);

            return 0;
        }

        readonly string _commentZoneStartMark = "/*";
        readonly string _commentZoneEndMark = "*/";
        private bool IsEmptyOrComment()
        {
            if (!ParseData.IsComment && ParseData.Line.Contains(_commentZoneStartMark))
            {
                ParseData.IsComment = true;
            }
            if (ParseData.IsComment && ParseData.Line.Contains(_commentZoneEndMark))
            {
                ParseData.IsComment = false;
            }

            return ParseData.IsComment || ParseData.TrimmedLine.Length == 0 || ParseData.TrimmedLine[0] == ';' || ParseData.TrimmedLine.StartsWith("//");
        }
    }
}
