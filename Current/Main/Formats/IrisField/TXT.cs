using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Projects.IrisField;

namespace TranslationHelper.Formats.IrisField
{
    class TXT : FormatStringBase
    {
        public TXT()
        {
            if (SaveFileMode)
            {
                MaxLineLength = (AppData.CurrentProject as IrisFieldGameBase).MaxLineLength;
            }
        }

        readonly int MaxLineLength;

        public override string Extension => ".txt";

        protected override Encoding DefaultEncoding()
        {
            return Encoding.GetEncoding(932);
        }

        bool _messageReadMode;
        readonly StringBuilder messageContent = new StringBuilder();
        private string LastMSGType;

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!_messageReadMode) // skip when read message content
            {
                SaveModeAddLine(newline: Environment.NewLine, newLineAfter: true);
            }

            if (IsEmpty())
            {
                // do nothing
            }
            else if (IsComment())
            {
                // do nothing
            }
            else if (IsMessage())
            {
                ParseMessage();
            }
            else if (IsChoiceVariants())
            {
                ParseChoices();
            }

            return KeywordActionAfter.Continue;
        }

        private void ParseMessage()
        {
            if (!_messageReadMode)
            {
                LastMSGType = ParseData.Line;

                if (IsVoicedMessage())
                {
                    SaveModeAddLine(ReadLine(), Environment.NewLine, true); // add line with translation                 
                }

                _messageReadMode = true;
            }
            else
            {
                if (!IsMessageEnd())
                {
                    messageContent.AppendLine(ParseData.Line);
                }
                else
                {
                    var mergedMessage = messageContent.ToString();
                    messageContent.Clear();

                    _messageReadMode = false;

                    if (OpenFileMode)
                    {
                        AddRowData(rowData: mergedMessage.TrimEnd(), rowInfo: LastMSGType, isCheckInput: true);
                    }
                    else if (IsValidString(mergedMessage))
                    {
                        string extraEmptyLinesForWrite = string.Empty;

                        try
                        {
                            extraEmptyLinesForWrite = mergedMessage.Replace(mergedMessage = mergedMessage.TrimEnd(), string.Empty);
                        }
                        catch
                        {

                        }

                        if (SetTranslation(ref mergedMessage))
                        {
                            var newLine = mergedMessage.SplitMultiLineIfBeyondOfLimit(MaxLineLength);

                            MakeRequiredEdits(ref newLine);

                            mergedMessage = newLine + extraEmptyLinesForWrite;
                        }
                    }

                    SaveModeAddLine(mergedMessage, Environment.NewLine, true);

                    ParseStringFileLine();
                }
            }
        }

        private void ParseChoices()
        {
            int choicesCount = GetChoicesCount();
            for (int i = 0; i < choicesCount; i++)
            {
                var choice = ParseData.Reader.ReadLine();
                var choiceMatch = Regex.Match(choice, _choiceTextExtractionRegex);
                var choiceText = choiceMatch.Groups[1].Value;

                if (AddRowData(rowData: ref choiceText, rowInfo: "Choice: " + i) && SaveFileMode)
                {
                    choiceText = choiceText.Replace(' ', '_');
                }

                SaveModeAddLine(choiceText + choiceMatch.Groups[2].Value, Environment.NewLine, true);
            }
        }

        private int GetChoicesCount()
        {
            return int.Parse(ParseData.Line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
        }

        private void MakeRequiredEdits(ref string newLine)
        {
            newLine = PreReduceTranslation(newLine);
            newLine = ApplyRequiredCharReplacements(newLine);
            newLine = SplitMessageLinesToBlocksBy4lines(newLine);
        }

        private string SplitMessageLinesToBlocksBy4lines(string newLine)
        {
            int newLinesCount = newLine.GetLinesCount();
            int linesCount = 0;
            int linesCountMax = 5;
            string retLine = string.Empty;

            LastMSGType = LastMSGType.Replace("#MSGVOICE,", "#MSG,");

            foreach (var subline in newLine.SplitToLines())
            {
                string cleanedSubline = PostTransFormLineCleaning(subline);
                cleanedSubline = ENJPCharsReplacementFirstLetter(cleanedSubline);
                linesCount++;
                if (linesCount == linesCountMax)
                {
                    linesCountMax += 4;
                    retLine += Environment.NewLine
                        + Environment.NewLine
                        + LastMSGType
                        + Environment.NewLine;
                }
                else if (linesCount > 1 && linesCount <= newLinesCount)
                {
                    retLine += Environment.NewLine;
                }
                retLine += cleanedSubline;
            };

            return retLine;
        }

        private bool IsEmpty()
        {
            return !_messageReadMode && string.IsNullOrWhiteSpace(ParseData.Line);
        }

        private bool IsComment()
        {
            return !_messageReadMode && ParseData.TrimmedLine.StartsWith("//");
        }

        private bool IsMessage()
        {
            return _messageReadMode || ParseData.Line.StartsWith("#MSG,") || ParseData.Line == "#MSG" || IsVoicedMessage();
        }

        private bool IsVoicedMessage()
        {
            return ParseData.Line.StartsWith("#MSGVOICE");
        }

        private bool IsChoiceVariants()
        {
            return !_messageReadMode && ParseData.Line.StartsWith("#SELECT");
        }

        private static string _choiceTextExtractionRegex = @"([^\t]+)([\t]+[0-9]{1,2}.*)";

        private static string ApplyRequiredCharReplacements(string newLine)
        {
            return newLine
                .Replace('　', '_').Replace(' ', '_')
                .Replace(',', '、')
                .Replace('[', '_')
                .Replace(']', '_');
        }

        private static string PreReduceTranslation(string newLine)
        {
            return Regex.Replace(newLine, @"([a-zA-Z])\1{3,}", "$1-$1")
                .Replace("!!", "!")
                .Replace("!!", "!")
                .Replace("??", "?")
                .Replace("??", "?")
                .Replace('#', ' ')
                .Replace('「', ' ')
                .Replace('『', ' ')
                .Replace('」', ' ')
                .Replace('』', ' ')
                .Replace(" [", " ")
                .Replace("] ", " ")
                .Replace("''", string.Empty)
                .Replace('“', ' ')
                .Replace('”', ' ')
                .Replace('\"', ' ')
                .Replace(" a ", " ")
                .Replace("The ", string.Empty)
                .Replace(" the ", " ")
                .Replace(" '", string.Empty)
                .Replace("' ", string.Empty)
                .Replace(" ' ", string.Empty)
                .Replace(" '", string.Empty)
                .Replace("' ", string.Empty)
                .Replace('\'', ' ')
                .Replace('’', ' ')
                .Replace('*', ' ')
                .Replace(" , ", ",")
                .Replace(" ,", ",")
                .Replace(", ", ",")
                .Replace(" . ", ".")
                .Replace(" .", ".")
                .Replace(". ", ".")
                .Replace(" ! ", "!")
                .Replace(" !", "!")
                .Replace("! ", "!")
                .Replace(" ? ", "?")
                .Replace(" ?", "?")
                .Replace("? ", "?")
                .Replace(" ... ", "…")
                .Replace("... ", "…")
                .Replace(" ...", "…")
                .Replace("...", "…")
                .Replace("……", "…")
                .Replace("……", "…")
                .Replace("  ", " ")
                .Replace("  ", " ")
                .Replace("  ", " ");
        }

        private static string PostTransFormLineCleaning(string s)
        {
            return s
                .TrimStart()
                .TrimStart('_', '…', '.', ',', '?', '!', '-')
                .Replace("…_", "…")
                .Replace("_…", "…")
                .Replace("、_", "、")
                .Replace("._", ".");
        }
    }
}

