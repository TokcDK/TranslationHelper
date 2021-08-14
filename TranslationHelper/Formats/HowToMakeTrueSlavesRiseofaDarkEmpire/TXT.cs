using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.HowToMakeTrueSlavesRiseofaDarkEmpire
{
    class TXT : FormatBase
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
            return ParseStringFile();
        }
        protected override Encoding DefaultEncoding()
        {
            return Encoding.GetEncoding(932);
        }

        bool readmode;
        readonly StringBuilder sb = new StringBuilder();
        private string LastMSGType;

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            if (readmode)
            {
                bool startswithsharp = ParseData.line.StartsWith("#");
                bool startswithOther = StartsWithOther(ParseData.line);
                if (startswithsharp || startswithOther /*string.IsNullOrEmpty(line)*/)
                {
                    var str = sb.ToString().TrimEnd();
                    if (ProjectData.OpenFileMode)
                    {
                        AddRowData(str, string.Empty, true);
                    }
                    else
                    {
                        var extraEmptyLinesForWrite = (str.Length > 0 ? sb.ToString().Replace(str, string.Empty) : sb.ToString());//только пустота на конце, пустоту надо записать в новый файл для корректности

                        if (IsValidString(str) && TablesLinesDict.ContainsKey(str))
                        {

                            //split lines
                            var newLine = TablesLinesDict[str].SplitMultiLineIfBeyondOfLimit(60);//37 if transform all en chars to jp variants

                            MakeRequiredEdits(ref newLine);

                            ParseData.ResultForWrite.AppendLine(newLine + extraEmptyLinesForWrite);

                            if (str != newLine)
                            {
                                ParseData.Ret = true;
                            }
                        }
                        else
                        {
                            ParseData.ResultForWrite.AppendLine(sb.ToString() + Environment.NewLine);
                        }
                    }

                    readmode = false;
                    sb.Clear();

                    if (startswithsharp)
                    {
                        if (IsMessage(ParseData.line))
                        {
                            ParseMessage();
                            return 0;
                        }
                        else if (IsVoicedMessage(ParseData.line))
                        {
                            ParseVoicedMessage();
                            return 0;
                        }
                        else if (IsChoiceVariants(ParseData.line))
                        {
                            ParseChoices();
                            return 0;
                        }
                    }
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(Environment.NewLine);
                    }
                    sb.Append(ParseData.line);
                }

                SaveModeAddLine();

                return 0;
            }
            else
            {
                if (IsCommentary(ParseData.line))//commented or empty
                {
                    SaveModeAddLine();
                    return 0;
                }
                else if (IsMessage(ParseData.line))
                {
                    ParseMessage();
                    return 0;
                }
                else if (IsVoicedMessage(ParseData.line))
                {
                    ParseVoicedMessage();
                    return 0;
                }
                else if (IsChoiceVariants(ParseData.line))
                {
                    ParseChoices();
                    return 0;
                }
            }

            SaveModeAddLine();

            return 0;
        }

        private void ParseMessage()
        {
            LastMSGType = ParseData.line;
            SaveModeAddLine();
            readmode = true;
        }

        private void ParseVoicedMessage()
        {
            LastMSGType = ParseData.line;
            if (ProjectData.SaveFileMode)
            {
                SaveModeAddLine();//add mark
                ParseData.ResultForWrite.AppendLine(ParseData.line = ParseData.reader.ReadLine());//add voice file name line
            }
            else
            {
                ParseData.line = ParseData.reader.ReadLine();//read next line with voice name, nex readline will skip it
            }
            readmode = true;
        }

        private void ParseChoices()
        {
            SaveModeAddLine();

            //get choices count
            int selectioncnt = int.Parse(ParseData.line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
            //add all choices
            for (int i = 0; i < selectioncnt; i++)
            {
                var str = ParseData.reader.ReadLine();
                var extracted = Regex.Replace(str, ChoiceTextExtractionRegex(), "$1");
                if (ProjectData.OpenFileMode)
                {
                    AddRowData(extracted, "Choice variant " + i, true);
                }
                else
                {
                    if (IsValidString(extracted) && TablesLinesDict.ContainsKey(extracted))
                    {
                        str = str.Replace(extracted, TablesLinesDict[extracted]);
                        ParseData.Ret = true;
                    }
                    ParseData.ResultForWrite.AppendLine(str);
                }
            }
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

        bool openTxt()
        {
            if (ProjectData.FilePath.Length == 0)
            {
                return false;
            }

            string fileName = Path.GetFileNameWithoutExtension(ProjectData.FilePath);

            ProjectData.THFilesElementsDataset.Tables.Add(fileName).Columns.Add("Original");
            ProjectData.THFilesElementsDatasetInfo.Tables.Add(fileName).Columns.Add("Original");

            using (StreamReader sr = new StreamReader(GetOriginalWhenExists(), Encoding.GetEncoding(932)))
            {
                string line;
                bool readmode = false;
                StringBuilder sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    if (readmode)
                    {
                        bool startswithsharp = line.StartsWith("#");
                        bool startswithOther = StartsWithOther(line);
                        if (startswithsharp || startswithOther /*string.IsNullOrEmpty(line)*/)
                        {
                            if (!string.IsNullOrWhiteSpace(sb.ToString()))
                            {
                                ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(sb.ToString().TrimEnd());
                                ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(string.Empty);
                            }

                            readmode = false;
                            sb.Clear();

                            if (startswithsharp)
                            {
                                if (IsMessage(line))
                                {
                                    readmode = true;
                                    continue;
                                }
                                else if (IsVoicedMessage(line))
                                {
                                    sr.ReadLine();
                                    readmode = true;
                                    continue;
                                }
                                else if (IsChoiceVariants(line))
                                {
                                    int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                                    for (int i = 0; i < selectioncnt; i++)
                                    {
                                        ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1"));
                                        ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("Choice variant " + i);
                                    }
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(Environment.NewLine);
                            }
                            sb.Append(line);
                        }

                        continue;
                    }
                    else
                    {
                        if (IsCommentary(line))//commented or empty
                        {
                            continue;
                        }
                        else if (IsMessage(line))
                        {
                            readmode = true;
                            continue;
                        }
                        else if (IsVoicedMessage(line))
                        {
                            sr.ReadLine();
                            readmode = true;
                            continue;
                        }
                        else if (IsChoiceVariants(line))
                        {
                            int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                            for (int i = 0; i < selectioncnt; i++)
                            {
                                ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1"));
                                ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("Choice variant " + i);
                            }
                            continue;
                        }
                    }
                }
            }

            if (ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Count > 0)
            {
                ProjectData.THFilesElementsDataset.Tables[fileName].Columns.Add("Translation");
                return true;
            }
            else
            {
                ProjectData.THFilesElementsDataset.Tables.Remove(fileName);
                ProjectData.THFilesElementsDatasetInfo.Tables.Remove(fileName);
                return false;
            }
        }

        private string GetOriginalWhenExists()
        {
            if (File.Exists(ProjectData.FilePath + ".orig"))
            {
                return ProjectData.FilePath + ".orig";
            }
            return ProjectData.FilePath;
        }

        private static bool StartsWithOther(string line)
        {
            return line.StartsWith("//") || line.StartsWith("[") || line.StartsWith("}");
        }

        internal override bool Save()
        {
            return ParseStringFile();
        }

        private bool WriteTxt()
        {
            if (ProjectData.FilePath.Length == 0)
            {
                return false;
            }

            string fileName = Path.GetFileNameWithoutExtension(ProjectData.FilePath);

            StringBuilder sbWrite = new StringBuilder();

            int TableRowIndex = 0;
            bool WriteIt = false;
            string LastMSGType = string.Empty;
            using (StreamReader sr = new StreamReader(GetOriginalWhenExists(), Encoding.GetEncoding(932)))
            {
                string line;
                bool readmode = false;
                StringBuilder sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    if (readmode)
                    {
                        bool startswithsharp = line.StartsWith("#");
                        bool startswithOther = StartsWithOther(line);
                        if (startswithsharp || startswithOther /*string.IsNullOrEmpty(line)*/)
                        {
                            if (!string.IsNullOrWhiteSpace(sb.ToString()))
                            {
                                string trimmedSB = sb.ToString().TrimEnd();//строка с обрезаной пустотой на конце
                                string extraEmptyLinesForWrite = sb.ToString().Replace(trimmedSB, string.Empty);//только пустота на конце, пустоту надо записать в новый файл для корректности

                                var row = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex];
                                if (!string.IsNullOrEmpty(row[1] + string.Empty) && (row[0] as string) == trimmedSB && !Equals(row[0], row[1]))
                                {
                                    string newLine = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex][1] + string.Empty;
                                    //bool startsWithJPQuote1 = newLine.Contains("「");
                                    //bool startsWithJPQuote2 = newLine.Contains("『");

                                    //split lines
                                    newLine = newLine.SplitMultiLineIfBeyondOfLimit(60);//37 if transform all en chars to jp variants

                                    //required edits
                                    newLine = PreReduceTranslation(newLine);
                                    newLine = ApplyRequiredCharReplacements(newLine);

                                    int newLinesCount = newLine.GetLinesCount();
                                    int linesCount = 0;
                                    int linesCountMax = 5;
                                    string retLine = string.Empty;
                                    //newLine = TransformString(newLine);
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
                                    }

                                    sbWrite.AppendLine(retLine + extraEmptyLinesForWrite);

                                }
                                else
                                {
                                    sbWrite.AppendLine(sb.ToString() + Environment.NewLine);
                                }
                                TableRowIndex++;
                            }
                            readmode = false;
                            sb.Clear();
                            WriteIt = true;
                            LastMSGType = string.Empty;
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(Environment.NewLine);
                            }
                            sb.Append(line);
                        }

                        if (startswithsharp)
                        {
                            if (IsMessage(line))
                            {
                                LastMSGType = line;
                                sbWrite.AppendLine(line);
                                readmode = true;
                                continue;
                            }
                            else if (IsVoicedMessage(line))
                            {
                                LastMSGType = line;
                                sbWrite.AppendLine(line);
                                sbWrite.AppendLine(sr.ReadLine());
                                readmode = true;
                                continue;
                            }
                            else if (IsChoiceVariants(line))
                            {
                                sbWrite.AppendLine(line);
                                int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                                string[] selectionsLine;
                                for (int i = 0; i < selectioncnt; i++)
                                {
                                    selectionsLine = Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1{{|}}$2").Split(new string[] { "{{|}}" }, StringSplitOptions.None);
                                    var row = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex];
                                    if ((row[0] as string) == selectionsLine[0] && !string.IsNullOrEmpty(row[1] + string.Empty))
                                    {
                                        sbWrite.AppendLine(TransformString(row[1] + string.Empty) + selectionsLine[1]);
                                    }
                                    else
                                    {
                                        sbWrite.AppendLine(selectionsLine[0]);
                                    }
                                    TableRowIndex++;
                                }
                                //sbWrite.AppendLine(Environment.NewLine);
                                WriteIt = true;
                                continue;
                            }
                        }

                        if (startswithsharp || startswithOther)
                        {
                            sbWrite.AppendLine(line);
                        }

                        continue;
                    }
                    else
                    {
                        if (IsCommentary(line))//commented or empty
                        {
                            sbWrite.AppendLine(line);
                            continue;
                        }
                        else if (IsMessage(line))
                        {
                            LastMSGType = line;
                            sbWrite.AppendLine(line);
                            readmode = true;
                            continue;
                        }
                        else if (IsVoicedMessage(line))
                        {
                            LastMSGType = line;
                            sbWrite.AppendLine(line);
                            sbWrite.AppendLine(sr.ReadLine());
                            readmode = true;
                            continue;
                        }
                        else if (IsChoiceVariants(line))
                        {
                            sbWrite.AppendLine(line);
                            int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                            string[] selectionsLine;
                            for (int i = 0; i < selectioncnt; i++)
                            {
                                selectionsLine = Regex.Replace(sr.ReadLine(), ChoiceTextExtractionRegex(), "$1{{|}}$2").Split(new string[] { "{{|}}" }, StringSplitOptions.None);
                                var row = ProjectData.THFilesElementsDataset.Tables[fileName].Rows[TableRowIndex];
                                if ((row[0] as string) == selectionsLine[0] && !string.IsNullOrEmpty(row[1] + string.Empty))
                                {
                                    sbWrite.AppendLine(TransformString(row[1] + string.Empty) + selectionsLine[1]);
                                }
                                else
                                {
                                    sbWrite.AppendLine(selectionsLine[0]);
                                }
                                TableRowIndex++;
                            }
                            //sbWrite.AppendLine(Environment.NewLine);
                            WriteIt = true;
                            continue;
                        }
                    }

                    sbWrite.AppendLine(line);
                }
            }

            if (WriteIt && sbWrite.ToString().Length > 0 && !FunctionsFileFolder.FileInUse(ProjectData.FilePath))
            {
                if (!File.Exists(ProjectData.FilePath + ".orig"))
                {
                    File.Move(ProjectData.FilePath, ProjectData.FilePath + ".orig");
                }

                File.WriteAllText(ProjectData.FilePath, sbWrite.ToString(), Encoding.GetEncoding(932));
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsCommentary(string line)
        {
            return string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//");
        }

        private static bool IsMessage(string line)
        {
            return line.StartsWith("#MSG,") || line == "#MSG";
        }

        private static bool IsVoicedMessage(string line)
        {
            return line.StartsWith("#MSGVOICE,");
        }

        private static bool IsChoiceVariants(string line)
        {
            return line.StartsWith("#SELECT,");
        }

        private static string ChoiceTextExtractionRegex()
        {
            return @"([^	]+)([	 ]+[0-9]{1,2}.*)";
        }

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
