using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;

namespace TranslationHelper.Formats.IrisField
{
    class TXT : StringFileFormatBase
    {
        public TXT()
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        internal override bool Open()
        {
            return ParseFile();
        }
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
                    SaveModeAddLine(ReadLine()/*read line with voice file name*/, Environment.NewLine, true); // add line with translation                 
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

                    if (ProjectData.OpenFileMode)
                    {
                        AddRowData(RowData: mergedMessage.TrimEnd(), RowInfo: LastMSGType, CheckInput: true);
                    }
                    else if (IsValidString(mergedMessage))
                    {
                        string extraEmptyLinesForWrite = string.Empty;

                        try
                        {
                            extraEmptyLinesForWrite = mergedMessage.Replace(mergedMessage = mergedMessage.TrimEnd(), string.Empty);//только пустота на конце, пустоту надо записать в новый файл для корректности
                        }
                        catch
                        {

                        }

                        if (SetTranslation(ref mergedMessage))
                        {
                            //split lines
                            var newLine = mergedMessage.SplitMultiLineIfBeyondOfLimit(60);//37 if transform all en chars to jp variants

                            MakeRequiredEdits(ref newLine);

                            mergedMessage = newLine + extraEmptyLinesForWrite;
                        }
                    }

                    SaveModeAddLine(mergedMessage, Environment.NewLine, true); // add line with translation

                    ParseStringFileLine(); // repeat parse last line because message saved
                }
            }
        }

        private void ParseChoices()
        {
            //get choices count
            int choicesCount = GetChoicesCount();
            //add all choices
            for (int i = 0; i < choicesCount; i++)
            {
                var choice = ParseData.Reader.ReadLine(); // read choice line
                var choiceMatch = Regex.Match(choice, _choiceTextExtractionRegex);
                var choiceText = choiceMatch.Groups[1].Value;

                SaveModeAddLine(choiceText + choiceMatch.Groups[2].Value, Environment.NewLine, true);
            }
        }

        private int GetChoicesCount()
        {
            return int.Parse(ParseData.Line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// required edits to prevent ingame script errors
        /// </summary>
        /// <param name="newLine"></param>
        private void MakeRequiredEdits(ref string newLine)
        {
            newLine = PreReduceTranslation(newLine);
            newLine = ApplyRequiredCharReplacements(newLine);
            newLine = SplitMessageLinesToBlocksBy4lines(newLine);
        }

        /// <summary>
        /// splits message by blocks by 4 lines
        /// </summary>
        /// <param name="newLine"></param>
        /// <returns></returns>
        private string SplitMessageLinesToBlocksBy4lines(string newLine)
        {
            int newLinesCount = newLine.GetLinesCount();
            int linesCount = 0;
            int linesCountMax = 5;
            string retLine = string.Empty;

            //blocks #MSGVOICE after 1st must be changed to no voice #MSG to not repeat speech for each block
            LastMSGType = LastMSGType.Replace("#MSGVOICE,", "#MSG,");

            foreach (var subline in newLine.SplitToLines())
            {
                string cleanedSubline = PostTransFormLineCleaning(subline);
                cleanedSubline = ENJPCharsReplacementFirstLetter(cleanedSubline);
                //cleanedSubline = CheckFirstCharIsLatinAndTransform(cleanedSubline);
                linesCount++;
                if (linesCount == linesCountMax)
                {
                    linesCountMax += 4;
                    retLine += /*(startsWithJPQuote1 ? "」" : (startsWithJPQuote2? "』" : string.Empty))
                                                + */Environment.NewLine
                        + Environment.NewLine
                        + LastMSGType
                        + Environment.NewLine
                        /*+ (startsWithJPQuote1 ? "「" : (startsWithJPQuote2 ? "『" : string.Empty))*/;
                }
                else if (linesCount > 1 && linesCount <= newLinesCount)
                {
                    retLine += Environment.NewLine;
                }
                retLine += cleanedSubline;
            };

            return retLine;
        }

        //bool openTxt()
        //{
        //    if (ProjectData.FilePath.Length == 0)
        //    {
        //        return false;
        //    }

        //    string fileName = Path.GetFileNameWithoutExtension(ProjectData.FilePath);

        //    ProjectData.THFilesElementsDataset.Tables.Add(fileName).Columns.Add(THSettings.OriginalColumnName());
        //    ProjectData.THFilesElementsDatasetInfo.Tables.Add(fileName).Columns.Add(THSettings.OriginalColumnName());

        //    using (StreamReader sr = new StreamReader(GetOriginalWhenExists(), Encoding.GetEncoding(932)))
        //    {
        //        string line;
        //        bool readmode = false;
        //        StringBuilder sb = new StringBuilder();
        //        while (!sr.EndOfStream)
        //        {
        //            line = sr.ReadLine();

        //            if (readmode)
        //            {
        //                bool startswithsharp = line.StartsWith("#");
        //                bool startswithOther = StartsWithOther(line);
        //                if (startswithsharp || startswithOther /*string.IsNullOrEmpty(line)*/)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
        //                    {
        //                        ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(sb.ToString().TrimEnd());
        //                        ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(string.Empty);
        //                    }

        //                    readmode = false;
        //                    sb.Clear();

        //                    if (startswithsharp)
        //                    {
        //                        if (IsMessage(line))
        //                        {
        //                            readmode = true;
        //                            continue;
        //                        }
        //                        else if (IsVoicedMessage(line))
        //                        {
        //                            sr.ReadLine();
        //                            readmode = true;
        //                            continue;
        //                        }
        //                        else if (IsChoiceVariants(line))
        //                        {
        //                            int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
        //                            for (int i = 0; i < selectioncnt; i++)
        //                            {
        //                                ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1"));
        //                                ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("Choice variant " + i);
        //                            }
        //                            continue;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (sb.Length > 0)
        //                    {
        //                        sb.Append(Environment.NewLine);
        //                    }
        //                    sb.Append(line);
        //                }

        //                continue;
        //            }
        //            else
        //            {
        //                if (IsEmptyOrComment(line))//commented or empty
        //                {
        //                    continue;
        //                }
        //                else if (IsMessage(line))
        //                {
        //                    readmode = true;
        //                    continue;
        //                }
        //                else if (IsVoicedMessage(line))
        //                {
        //                    sr.ReadLine();
        //                    readmode = true;
        //                    continue;
        //                }
        //                else if (IsChoiceVariants(line))
        //                {
        //                    int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
        //                    for (int i = 0; i < selectioncnt; i++)
        //                    {
        //                        ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1"));
        //                        ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("Choice variant " + i);
        //                    }
        //                    continue;
        //                }
        //            }
        //        }
        //    }

        //    if (ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Count > 0)
        //    {
        //        ProjectData.THFilesElementsDataset.Tables[fileName].Columns.Add(THSettings.TranslationColumnName());
        //        return true;
        //    }
        //    else
        //    {
        //        ProjectData.THFilesElementsDataset.Tables.Remove(fileName);
        //        ProjectData.THFilesElementsDatasetInfo.Tables.Remove(fileName);
        //        return false;
        //    }
        //}

        //private string GetOriginalWhenExists()
        //{
        //    if (File.Exists(ProjectData.FilePath + ".orig"))
        //    {
        //        return ProjectData.FilePath + ".orig";
        //    }
        //    return ProjectData.FilePath;
        //}

        private bool IsMessageEnd()
        {
            return !string.IsNullOrWhiteSpace(ParseData.Line) &&
                (ParseData.Line.StartsWith("//") || ParseData.Line.StartsWith("[") || ParseData.Line.StartsWith("}") || ParseData.Line.StartsWith("#"));
        }

        internal override bool Save()
        {
            return ParseFile();
        }

        //private bool WriteTxt()
        //{
        //    if (ProjectData.FilePath.Length == 0)
        //    {
        //        return false;
        //    }

        //    string fileName = Path.GetFileNameWithoutExtension(ProjectData.FilePath);

        //    StringBuilder sbWrite = new StringBuilder();

        //    int TableRowIndex = 0;
        //    bool WriteIt = false;
        //    string LastMSGType = string.Empty;
        //    using (StreamReader sr = new StreamReader(GetOriginalWhenExists(), Encoding.GetEncoding(932)))
        //    {
        //        string line;
        //        bool readmode = false;
        //        StringBuilder sb = new StringBuilder();
        //        while (!sr.EndOfStream)
        //        {
        //            line = sr.ReadLine();

        //            if (readmode)
        //            {
        //                bool startswithsharp = line.StartsWith("#");
        //                bool startswithOther = StartsWithOther(line);
        //                if (startswithsharp || startswithOther /*string.IsNullOrEmpty(line)*/)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
        //                    {
        //                        string trimmedSB = sb.ToString().TrimEnd();//строка с обрезаной пустотой на конце
        //                        string extraEmptyLinesForWrite = sb.ToString().Replace(trimmedSB, string.Empty);//только пустота на конце, пустоту надо записать в новый файл для корректности

        //                        var row = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex];
        //                        if (!string.IsNullOrEmpty(row[1] + string.Empty) && (row[0] as string) == trimmedSB && !Equals(row[0], row[1]))
        //                        {
        //                            string newLine = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex][1] + string.Empty;
        //                            //bool startsWithJPQuote1 = newLine.Contains("「");
        //                            //bool startsWithJPQuote2 = newLine.Contains("『");

        //                            //split lines
        //                            newLine = newLine.SplitMultiLineIfBeyondOfLimit(60);//37 if transform all en chars to jp variants

        //                            //required edits
        //                            newLine = PreReduceTranslation(newLine);
        //                            newLine = ApplyRequiredCharReplacements(newLine);

        //                            int newLinesCount = newLine.GetLinesCount();
        //                            int linesCount = 0;
        //                            int linesCountMax = 5;
        //                            string retLine = string.Empty;
        //                            //newLine = TransformString(newLine);
        //                            LastMSGType = LastMSGType.Replace("#MSGVOICE,", "#MSG,");
        //                            foreach (var subline in newLine.SplitToLines())
        //                            {
        //                                string cleanedSubline = PostTransFormLineCleaning(subline);
        //                                cleanedSubline = ENJPCharsReplacementFirstLetter(cleanedSubline);
        //                                //cleanedSubline = CheckFirstCharIsLatinAndTransform(cleanedSubline);
        //                                linesCount++;
        //                                if (linesCount == linesCountMax)
        //                                {
        //                                    linesCountMax += 4;
        //                                    retLine += /*(startsWithJPQuote1 ? "」" : (startsWithJPQuote2? "』" : string.Empty))
        //                                        + */Environment.NewLine
        //                                        + Environment.NewLine
        //                                        + LastMSGType
        //                                        + Environment.NewLine
        //                                        /*+ (startsWithJPQuote1 ? "「" : (startsWithJPQuote2 ? "『" : string.Empty))*/;
        //                                }
        //                                else if (linesCount > 1 && linesCount <= newLinesCount)
        //                                {
        //                                    retLine += Environment.NewLine;
        //                                }
        //                                retLine += cleanedSubline;
        //                            }

        //                            sbWrite.AppendLine(retLine + extraEmptyLinesForWrite);

        //                        }
        //                        else
        //                        {
        //                            sbWrite.AppendLine(sb.ToString() + Environment.NewLine);
        //                        }
        //                        TableRowIndex++;
        //                    }
        //                    readmode = false;
        //                    sb.Clear();
        //                    WriteIt = true;
        //                    LastMSGType = string.Empty;
        //                }
        //                else
        //                {
        //                    if (sb.Length > 0)
        //                    {
        //                        sb.Append(Environment.NewLine);
        //                    }
        //                    sb.Append(line);
        //                }

        //                if (startswithsharp)
        //                {
        //                    if (IsMessage(line))
        //                    {
        //                        LastMSGType = line;
        //                        sbWrite.AppendLine(line);
        //                        readmode = true;
        //                        continue;
        //                    }
        //                    else if (IsVoicedMessage(line))
        //                    {
        //                        LastMSGType = line;
        //                        sbWrite.AppendLine(line);
        //                        sbWrite.AppendLine(sr.ReadLine());
        //                        readmode = true;
        //                        continue;
        //                    }
        //                    else if (IsChoiceVariants(line))
        //                    {
        //                        sbWrite.AppendLine(line);
        //                        int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
        //                        string[] selectionsLine;
        //                        for (int i = 0; i < selectioncnt; i++)
        //                        {
        //                            selectionsLine = Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1{{|}}$2").Split(new string[] { "{{|}}" }, StringSplitOptions.None);
        //                            var row = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex];
        //                            if ((row[0] as string) == selectionsLine[0] && !string.IsNullOrEmpty(row[1] + string.Empty))
        //                            {
        //                                sbWrite.AppendLine(TransformString(row[1] + string.Empty) + selectionsLine[1]);
        //                            }
        //                            else
        //                            {
        //                                sbWrite.AppendLine(selectionsLine[0]);
        //                            }
        //                            TableRowIndex++;
        //                        }
        //                        //sbWrite.AppendLine(Environment.NewLine);
        //                        WriteIt = true;
        //                        continue;
        //                    }
        //                }

        //                if (startswithsharp || startswithOther)
        //                {
        //                    sbWrite.AppendLine(line);
        //                }

        //                continue;
        //            }
        //            else
        //            {
        //                if (IsEmptyOrComment(line))//commented or empty
        //                {
        //                    sbWrite.AppendLine(line);
        //                    continue;
        //                }
        //                else if (IsMessage(line))
        //                {
        //                    LastMSGType = line;
        //                    sbWrite.AppendLine(line);
        //                    readmode = true;
        //                    continue;
        //                }
        //                else if (IsVoicedMessage(line))
        //                {
        //                    LastMSGType = line;
        //                    sbWrite.AppendLine(line);
        //                    sbWrite.AppendLine(sr.ReadLine());
        //                    readmode = true;
        //                    continue;
        //                }
        //                else if (IsChoiceVariants(line))
        //                {
        //                    sbWrite.AppendLine(line);
        //                    int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
        //                    string[] selectionsLine;
        //                    for (int i = 0; i < selectioncnt; i++)
        //                    {
        //                        selectionsLine = Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1{{|}}$2").Split(new string[] { "{{|}}" }, StringSplitOptions.None);
        //                        var row = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex];
        //                        if ((row[0] as string) == selectionsLine[0] && !string.IsNullOrEmpty(row[1] + string.Empty))
        //                        {
        //                            sbWrite.AppendLine(TransformString(row[1] + string.Empty) + selectionsLine[1]);
        //                        }
        //                        else
        //                        {
        //                            sbWrite.AppendLine(selectionsLine[0]);
        //                        }
        //                        TableRowIndex++;
        //                    }
        //                    //sbWrite.AppendLine(Environment.NewLine);
        //                    WriteIt = true;
        //                    continue;
        //                }
        //            }

        //            sbWrite.AppendLine(line);
        //        }
        //    }

        //    if (WriteIt && sbWrite.ToString().Length > 0 && !FunctionsFileFolder.FileInUse(ProjectData.FilePath))
        //    {
        //        if (!File.Exists(ProjectData.FilePath + ".orig"))
        //        {
        //            File.Move(ProjectData.FilePath, ProjectData.FilePath + ".orig");
        //        }

        //        File.WriteAllText(ProjectData.FilePath, sbWrite.ToString(), Encoding.GetEncoding(932));
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

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

        /// <summary>
        /// prevents script parse errors in game
        /// </summary>
        /// <param name="newLine"></param>
        /// <returns></returns>
        private static string ApplyRequiredCharReplacements(string newLine)
        {
            return newLine
                .Replace("　", "_").Replace(" ", "_")//whitespaces forbidden
                .Replace(",", "、")//message will stop with en symbol
                .Replace("[", "_")
                .Replace("]", "_")
                ;
        }

        private static string CheckFirstCharIsLatinAndTransform(string cleanedSubline)
        {
            if (string.IsNullOrEmpty(cleanedSubline))
            {
                return cleanedSubline;
            }

            var Firstletter = cleanedSubline.Substring(0, 1);
            if (Firstletter.IsDigitsOnly() || Firstletter == "!" || Firstletter == "?")
            {
                return TransformFirstChar(Firstletter) + (cleanedSubline.Length > 1 ? cleanedSubline.Substring(1) : string.Empty);
            }

            return cleanedSubline;
        }

        private static string TransformFirstChar(string firstletter)
        {
            return firstletter
                .Replace("1", "１")
                .Replace("2", "２")
                .Replace("3", "３")
                .Replace("4", "４")
                .Replace("5", "５")
                .Replace("6", "６")
                .Replace("7", "７")
                .Replace("8", "８")
                .Replace("9", "９")
                .Replace("0", "０")
                .Replace("!", "！")
                .Replace("?", "？")
                ;
        }

        private static string PreReduceTranslation(string newLine)
        {
            //newLine = WordsReplacement(newLine);
            return Regex.Replace(newLine, @"([a-zA-Z])\1{3,}", "$1-$1")
                .Replace("!!", "!")
                .Replace("!!", "!")
                .Replace("??", "?")
                .Replace("??", "?")
                .Replace("#", string.Empty)
                .Replace("「", " ")
                .Replace("『", " ")
                .Replace("」", " ")
                .Replace("』", " ")
                .Replace(" [", " ")
                .Replace("] ", " ")
                .Replace("''", string.Empty)
                .Replace("“", string.Empty)
                .Replace("”", string.Empty)
                .Replace("\"", string.Empty)
                .Replace(" a ", " ")
                .Replace("The ", string.Empty)
                .Replace(" the ", " ")
                .Replace(" '", string.Empty)
                .Replace("' ", string.Empty)
                .Replace(" ' ", string.Empty)
                .Replace(" '", string.Empty)
                .Replace("' ", string.Empty)
                .Replace("'", string.Empty)
                .Replace("’", string.Empty)
                .Replace("*", string.Empty)
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
                .Replace("  ", " ")
                ;
        }

        private static string PostTransFormLineCleaning(string s)
        {
            return s
                .TrimStart()
                .TrimStart('_', '…', '.', ',', '?', '!', '-')
                .Replace("…_", "…")
                .Replace("_…", "…")
                .Replace("、_", "、")
                .Replace("._", ".")
                //.Replace("´", string.Empty)
                //.Replace("「…", "「")
                //.Replace("…「", "「")
                //.Replace("？？", "？")
                //.Replace("！！", "！")
                //.Replace("_", "　");
                //.Replace("「``", "「")
                //.Replace("「\"", "「")
                //.Replace("「`", "「");
                ;
        }

        readonly Dictionary<string, string> ENtoJPReplacementPairsOneLetterDict = new Dictionary<string, string>
            {
                { "a", "ａ" },
                { "A", "Ａ" },
                { "b", "ｂ" },
                { "B", "Ｂ" },
                { "c", "ｃ" },
                { "C", "Ｃ" },
                { "d", "ｄ" },
                { "D", "Ｄ" },
                { "e", "ｅ" },
                { "E", "Ｅ" },
                { "f", "ｆ" },
                { "F", "Ｆ" },
                { "g", "ｇ" },
                { "G", "Ｇ" },
                { "h", "ｈ" },
                { "H", "Ｈ" },
                { "i", "ｉ" },
                { "I", "Ｉ" },
                { "j", "ｊ" },
                { "J", "Ｊ" },
                { "k", "ｋ" },
                { "K", "Ｋ" },
                { "l", "ｌ" },
                { "L", "Ｌ" },
                { "m", "ｍ" },
                { "M", "Ｍ" },
                { "n", "ｎ" },
                { "N", "Ｎ" },
                { "o", "ｏ" },
                { "O", "Ｏ" },
                { "p", "ｐ" },
                { "P", "Ｐ" },
                { "q", "ｑ" },
                { "Q", "Ｑ" },
                { "r", "ｒ" },
                { "R", "Ｒ" },
                { "s", "ｓ" },
                { "S", "Ｓ" },
                { "t", "ｔ" },
                { "T", "Ｔ" },
                { "u", "ｕ" },
                { "U", "Ｕ" },
                { "v", "ｖ" },
                { "V", "Ｖ" },
                { "w", "ｗ" },
                { "W", "Ｗ" },
                { "x", "ｘ" },
                { "X", "Ｘ" },
                { "y", "ｙ" },
                { "Y", "Ｙ" },
                { "z", "ｚ" },
                { "Z", "Ｚ" },
                { "0", "０" },
                { "1", "１" },
                { "2", "２" },
                { "3", "３" },
                { "4", "４" },
                { "5", "５" },
                { "6", "６" },
                { "7", "７" },
                { "8", "８" },
                { "9", "９" },
                { ",", "、" },
                { ".", "。" },
                { "\"", string.Empty },
                { "”", " " },
                { "'", string.Empty },
                { "’", string.Empty },
                { "{", string.Empty },
                { "}", string.Empty },
                { "[", " " },
                { "]", " " },
                { "(", "（" },
                { ")", "）" },
                { "#", string.Empty },
                { "「", " " },
                { "『", " " },
                { "」", " " },
                { "』", " " },
                { "　", " " },
                { " ", "_" }
            };

        //    readonly string[][] ENtoJPReplacementPairsOneLetter = new string[][] {
        //           new string[2]{ "a", "ａ" },
        //           new string[2]{ "A", "Ａ" },
        //           new string[2]{ "b", "ｂ" },
        //           new string[2]{ "B", "Ｂ" },
        //           new string[2]{ "c", "ｃ" },
        //           new string[2]{ "C", "Ｃ" },
        //           new string[2]{ "d", "ｄ" },
        //           new string[2]{ "D", "Ｄ" },
        //           new string[2]{ "e", "ｅ" },
        //           new string[2]{ "E", "Ｅ" },
        //           new string[2]{ "f", "ｆ" },
        //           new string[2]{ "F", "Ｆ" },
        //           new string[2]{ "g", "ｇ" },
        //           new string[2]{ "G", "Ｇ" },
        //           new string[2]{ "h", "ｈ" },
        //           new string[2]{ "H", "Ｈ" },
        //           new string[2]{ "i", "ｉ" },
        //           new string[2]{ "I", "Ｉ" },
        //           new string[2]{ "j", "ｊ" },
        //           new string[2]{ "J", "Ｊ" },
        //           new string[2]{ "k", "ｋ" },
        //           new string[2]{ "K", "Ｋ" },
        //           new string[2]{ "l", "ｌ" },
        //           new string[2]{ "L", "Ｌ" },
        //           new string[2]{ "m", "ｍ" },
        //           new string[2]{ "M", "Ｍ" },
        //           new string[2]{ "n", "ｎ" },
        //           new string[2]{ "N", "Ｎ" },
        //           new string[2]{ "o", "ｏ" },
        //           new string[2]{ "O", "Ｏ" },
        //           new string[2]{ "p", "ｐ" },
        //           new string[2]{ "P", "Ｐ" },
        //           new string[2]{ "q", "ｑ" },
        //           new string[2]{ "Q", "Ｑ" },
        //           new string[2]{ "r", "ｒ" },
        //           new string[2]{ "R", "Ｒ" },
        //           new string[2]{ "s", "ｓ" },
        //           new string[2]{ "S", "Ｓ" },
        //           new string[2]{ "t", "ｔ" },
        //           new string[2]{ "T", "Ｔ" },
        //           new string[2]{ "u", "ｕ" },
        //           new string[2]{ "U", "Ｕ" },
        //           new string[2]{ "v", "ｖ" },
        //           new string[2]{ "V", "Ｖ" },
        //           new string[2]{ "w", "ｗ" },
        //           new string[2]{ "W", "Ｗ" },
        //           new string[2]{ "x", "ｘ" },
        //           new string[2]{ "X", "Ｘ" },
        //           new string[2]{ "y", "ｙ" },
        //           new string[2]{ "Y", "Ｙ" },
        //           new string[2]{ "z", "ｚ" },
        //           new string[2]{ "Z", "Ｚ" },
        //           new string[2]{ "0", "０" },
        //           new string[2]{ "1", "１" },
        //           new string[2]{ "2", "２" },
        //           new string[2]{ "3", "３" },
        //           new string[2]{ "4", "４" },
        //           new string[2]{ "5", "５" },
        //           new string[2]{ "6", "６" },
        //           new string[2]{ "7", "７" },
        //           new string[2]{ "8", "８" },
        //           new string[2]{ "9", "９" },
        //           new string[2]{ ",", "、" },
        //           new string[2]{ ".", "。" },
        //           new string[2]{ "\"", string.Empty },
        //            new string[2]{ "”", " " },
        //           new string[2]{ "'", string.Empty },
        //           new string[2]{ "’", string.Empty },
        //           new string[2]{ "{", string.Empty },
        //           new string[2]{ "}", string.Empty },
        //           new string[2]{ "[", " " },
        //           new string[2]{ "]", " " },
        //           new string[2]{ "(", "（" },
        //           new string[2]{ ")", "）" },
        //           new string[2]{ "#", string.Empty },
        //           new string[2]{ "「", " " },
        //           new string[2]{ "『", " " },
        //           new string[2]{ "」", " " },
        //           new string[2]{ "』", " " },
        //           new string[2]{ "　", " " },
        //           new string[2]{ " ", "_" }
        //};

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
               new string[2]{ ", ", "、" },
               new string[2]{ ",", "、" },
               new string[2]{ ". ", "。" },
               new string[2]{ ".", "。" },
               new string[2]{ " ... ", "…" },
               new string[2]{ "... ", "…" },
               new string[2]{ " ...", "…" },
               new string[2]{ "...", "…" },
               new string[2]{ "...", "…" },
               new string[2]{ "……", "…" },
               new string[2]{ "……", "…" },
                new string[2]{ "……", "…" },
                new string[2]{ "……", "…" },
               new string[2]{ " … ", "…" },
               new string[2]{ "… ", "…" },
               new string[2]{ " …", "…" },
                new string[2]{ "。。", "。" },
                new string[2]{ "。。", "。" },
               new string[2]{ " \" ", " " },
               new string[2]{ "\" ", " " },
               new string[2]{ " \"", " " },
               new string[2]{ "\"", string.Empty },
                new string[2]{ " ” ", " " },
                new string[2]{ " ”", " " },
                new string[2]{ "” ", " " },
                new string[2]{ "”", " " },
               //new string[2]{ "\"", "”" },
               new string[2]{ " ~ ", " " },
               new string[2]{ "_~", string.Empty },
               //new string[2]{ "? ", "？" },
               //new string[2]{ "! ", "！" },
               new string[2]{ " '", string.Empty },
               new string[2]{ " ’", string.Empty },
               new string[2]{ "'", string.Empty },
               new string[2]{ "’", string.Empty },
               new string[2]{ "{", string.Empty },
               new string[2]{ "}", string.Empty },
               new string[2]{ " [", " " },
               new string[2]{ "] ", " " },
               new string[2]{ "[", " " },
               new string[2]{ "]", " " },
               //new string[2]{ " [", "【" },
               //new string[2]{ "] ", "】" },
               //new string[2]{ "[", "【" },
               //new string[2]{ "]", "】" },
               new string[2]{ "#", string.Empty },
               new string[2]{ "「", " " },
               new string[2]{ "『", " " },
               new string[2]{ "」", " " },
               new string[2]{ "』", " " },
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
               new string[2]{ "　", " " },
               new string[2]{ "  ", " " },
               new string[2]{ "  ", " " },
               new string[2]{ " ", "_" }
    };

        private string TransformString(string workString)
        {
            workString = WordsReplacement(workString);
            workString = ENJPCharsReplacement(workString);

            return workString;
        }

        private string ENJPCharsReplacement(string workString)
        {
            return FunctionsString.CharsReplacementByPairsFromArray(workString, ENtoJPReplacementPairs);
        }

        private string ENJPCharsReplacementFirstLetter(string workString)
        {
            if (string.IsNullOrEmpty(workString))
            {
                return workString;
            }

            if (workString.Substring(0, 1) == "_")
            {
                while (workString.Substring(0, 1) == "_")
                {
                    workString = workString.Remove(0, 1);
                    if (string.IsNullOrEmpty(workString))
                    {
                        return workString;
                    }
                }
            }

            string firstLetter = workString.Substring(0, 1);

            if (ENtoJPReplacementPairsOneLetterDict.ContainsKey(firstLetter))
            {
                firstLetter = ENtoJPReplacementPairsOneLetterDict[firstLetter];
            }

            return firstLetter + (workString.Length > 1 ? workString.Substring(1) : string.Empty);

            //return FunctionsString.CharsReplacementByPairsFromArray(workString.Substring(0, 1), ENtoJPReplacementPairsOneLetter) + (workString.Length > 1 ? workString.Substring(1) : string.Empty);
        }

        readonly string[][] ENJPWordsReplacementPairs = new string[][] {
               new string[2]{ "one hundred", "100" },
               new string[2]{ "ninety-nine", "99" },
               new string[2]{ "ninety-eight", "98" },
               new string[2]{ "ninety-seven", "97" },
               new string[2]{ "ninety-six", "96" },
               new string[2]{ "ninety-five", "95" },
               new string[2]{ "ninety-four", "94" },
               new string[2]{ "ninety-three", "93" },
               new string[2]{ "ninety-two", "92" },
               new string[2]{ "ninety-one", "91" },
               new string[2]{ "ninety", "90" },
               new string[2]{ "eighty-nine", "89" },
               new string[2]{ "eighty-eight", "88" },
               new string[2]{ "eighty-seven", "87" },
               new string[2]{ "eighty-six", "86" },
               new string[2]{ "eighty-five", "85" },
               new string[2]{ "eighty-four", "84" },
               new string[2]{ "eighty-three", "83" },
               new string[2]{ "eighty-two", "82" },
               new string[2]{ "eighty-one", "81" },
               new string[2]{ "eighty", "80" },
               new string[2]{ "seventy-nine", "79" },
               new string[2]{ "seventy-eight", "78" },
               new string[2]{ "seventy-seven", "77" },
               new string[2]{ "seventy-six", "76" },
               new string[2]{ "seventy-five", "75" },
               new string[2]{ "seventy-four", "74" },
               new string[2]{ "seventy-three", "73" },
               new string[2]{ "seventy-two", "72" },
               new string[2]{ "seventy-one", "71" },
               new string[2]{ "seventy", "70" },
               new string[2]{ "sixty-nine", "69" },
               new string[2]{ "sixty-eight", "68" },
               new string[2]{ "sixty-seven", "67" },
               new string[2]{ "sixty-six", "66" },
               new string[2]{ "sixty-five", "65" },
               new string[2]{ "sixty-four", "64" },
               new string[2]{ "sixty-three", "63" },
               new string[2]{ "sixty-two", "62" },
               new string[2]{ "sixty-one", "61" },
               new string[2]{ "sixty", "60" },
               new string[2]{ "fifty-nine", "59" },
               new string[2]{ "fifty-eight", "58" },
               new string[2]{ "fifty-seven", "57" },
               new string[2]{ "fifty-six", "56" },
               new string[2]{ "fifty-five", "55" },
               new string[2]{ "fifty-four", "54" },
               new string[2]{ "fifty-three", "53" },
               new string[2]{ "fifty-two", "52" },
               new string[2]{ "fifty-one", "51" },
               new string[2]{ "fifty", "50" },
               new string[2]{ "forty-nine", "49" },
               new string[2]{ "forty-eight", "48" },
               new string[2]{ "forty-seven", "47" },
               new string[2]{ "forty-six", "46" },
               new string[2]{ "forty-five", "45" },
               new string[2]{ "forty-four", "44" },
               new string[2]{ "forty-three", "43" },
               new string[2]{ "forty-two", "42" },
               new string[2]{ "forty-one", "41" },
               new string[2]{ "forty", "40" },
               new string[2]{ "thirty-nine", "39" },
               new string[2]{ "thirty-eight", "38" },
               new string[2]{ "thirty-seven", "37" },
               new string[2]{ "thirty-six", "36" },
               new string[2]{ "thirty-five", "35" },
               new string[2]{ "thirty-four", "34" },
               new string[2]{ "thirty-three", "33" },
               new string[2]{ "thirty-two", "32" },
               new string[2]{ "thirty-one", "31" },
               new string[2]{ "thirty", "30" },
               new string[2]{ "twenty-nine", "29" },
               new string[2]{ "twenty-eight", "28" },
               new string[2]{ "twenty-seven", "27" },
               new string[2]{ "twenty-six", "26" },
               new string[2]{ "twenty-five", "25" },
               new string[2]{ "twenty-four", "24" },
               new string[2]{ "twenty-three", "23" },
               new string[2]{ "twenty-two", "22" },
               new string[2]{ "twenty-one", "21" },
               new string[2]{ "twenty", "20" },
               new string[2]{ "nineteen", "19" },
               new string[2]{ "eighteen", "18" },
               new string[2]{ "seventeen", "17" },
               new string[2]{ "sixteen", "16" },
               new string[2]{ "fifteen", "15" },
               new string[2]{ "fourteen", "14" },
               new string[2]{ "thirteen", "13" },
               new string[2]{ "twelve", "12" },
               new string[2]{ "eleven", "11" },
               new string[2]{ "ten", "10" },
               new string[2]{ "nine", "9" },
               new string[2]{ "eight", "8" },
               new string[2]{ "seven", "7" },
               new string[2]{ "six", "6" },
               new string[2]{ "five", "5" },
               new string[2]{ "four", "4" },
               new string[2]{ "three", "3" },
               new string[2]{ "two", "2" },
               //new string[2]{ "foo2rk", "footwork" },
               new string[2]{ "one", "1" },
               //new string[2]{ "st1", "stone" },
               new string[2]{ "enhancement", "enhance" },
               new string[2]{ "ho-ho-ho-ho", "ho-ho" },
               new string[2]{ "ho-ho-ho", "ho-ho" }
            };

        private string WordsReplacement(string input)
        {
            string ret = input;
            int charsLength = ENJPWordsReplacementPairs.Length;
            for (int i = 0; i < charsLength; i++)
            {
                ret = ret.Replace(ENJPWordsReplacementPairs[i][0], ENJPWordsReplacementPairs[i][1], StringComparison.OrdinalIgnoreCase);
            }

            return ret;
        }
    }
}
