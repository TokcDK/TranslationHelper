using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.HowToMakeTrueSlavesRiseofaDarkEmpire
{
    class TXT : FormatBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            if (thDataWork.FilePath.Length == 0)
            {
                return false;
            }

            string fileName = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

            thDataWork.THFilesElementsDataset.Tables.Add(fileName).Columns.Add("Original");

            using (StreamReader sr = new StreamReader(thDataWork.FilePath, Encoding.GetEncoding(932)))
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
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(sb.ToString().TrimEnd());
                            }

                            readmode = false;
                            sb.Clear();

                            if (startswithsharp)
                            {
                                if (line.StartsWith("#MSG,") || line == "#MSG")
                                {
                                    readmode = true;
                                    continue;
                                }
                                else if (line.StartsWith("#MSGVOICE,"))
                                {
                                    sr.ReadLine();
                                    readmode = true;
                                    continue;
                                }
                                else if (line.StartsWith("#SELECT,"))
                                {
                                    int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                                    for (int i = 0; i < selectioncnt; i++)
                                    {
                                        thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Regex.Replace(sr.ReadLine(), @"([^	]+)(	+[0-9]{1,2}.*)", "$1"));
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
                        //commented or empty
                        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                        {
                            continue;
                        }
                        else if (line.StartsWith("#MSG,") || line == "#MSG")
                        {
                            readmode = true;
                            continue;
                        }
                        else if (line.StartsWith("#MSGVOICE,"))
                        {
                            sr.ReadLine();
                            readmode = true;
                            continue;
                        }
                        else if (line.StartsWith("#SELECT,"))
                        {
                            int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                            for (int i = 0; i < selectioncnt; i++)
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Regex.Replace(sr.ReadLine(), @"([^	]+)(	+[0-9]{1,2}.*)", "$1"));
                            }
                            continue;
                        }
                    }
                }
            }

            if (thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Count > 0)
            {
                thDataWork.THFilesElementsDataset.Tables[fileName].Columns.Add("Translation");
                return true;
            }
            else
            {
                thDataWork.THFilesElementsDataset.Tables.Remove(fileName);
                return false;
            }
        }

        private bool StartsWithOther(string line)
        {
            return line.StartsWith("//") || line.StartsWith("[") || line.StartsWith("}");
        }

        internal override bool Save()
        {
            if (thDataWork.FilePath.Length == 0)
            {
                return false;
            }

            string fileName = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

            StringBuilder sbWrite = new StringBuilder();

            int r = 0;
            bool WriteIt = false;
            string LastMSGType = string.Empty;
            using (StreamReader sr = new StreamReader(thDataWork.FilePath, Encoding.GetEncoding(932)))
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

                                var row = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r];
                                if ((row[0] as string) == trimmedSB && !string.IsNullOrEmpty(row[1] + string.Empty))
                                {
                                    string newLine = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r][1] + string.Empty;
                                    //bool startsWithJPQuote1 = newLine.Contains("「");
                                    //bool startsWithJPQuote2 = newLine.Contains("『");
                                    newLine = PreReduceTranslation(newLine);
                                    newLine = FunctionsString.SplitMultiLineIfBeyondOfLimit(newLine, 37);
                                    int newLinesCount = FunctionsString.GetLinesCount(newLine);
                                    int cnt = 0;
                                    int cntMax = 5;
                                    string retLine = string.Empty;
                                    newLine = TransformString(newLine);
                                    LastMSGType = LastMSGType.Replace("#MSGVOICE,", "#MSG,");
                                    foreach (var subline in newLine.SplitToLines())
                                    {
                                        string cleanedSubline = PostTransFormLineCleaning(subline);
                                        cnt++;
                                        if (cnt == cntMax)
                                        {
                                            cntMax += 4;
                                            retLine += /*(startsWithJPQuote1 ? "」" : (startsWithJPQuote2? "』" : string.Empty))
                                                + */Environment.NewLine
                                                + Environment.NewLine
                                                + LastMSGType
                                                + Environment.NewLine
                                                /*+ (startsWithJPQuote1 ? "「" : (startsWithJPQuote2 ? "『" : string.Empty))*/;
                                        }
                                        else if (cnt > 1 && cnt <= newLinesCount)
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
                                r++;
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
                            if (line.StartsWith("#MSG,") || line == "#MSG")
                            {
                                LastMSGType = line;
                                sbWrite.AppendLine(line);
                                readmode = true;
                                continue;
                            }
                            else if (line.StartsWith("#MSGVOICE,"))
                            {
                                LastMSGType = line;
                                sbWrite.AppendLine(line);
                                sbWrite.AppendLine(sr.ReadLine());
                                readmode = true;
                                continue;
                            }
                            else if (line.StartsWith("#SELECT,"))
                            {
                                sbWrite.AppendLine(line);
                                int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                                string[] selectionsLine;
                                for (int i = 0; i < selectioncnt; i++)
                                {
                                    selectionsLine = Regex.Replace(sr.ReadLine(), @"([^	]+)([	]+[0-9]{1,2}.*)", "$1{{|}}$2").Split(new string[] { "{{|}}" }, StringSplitOptions.None);
                                    var row = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r];
                                    if ((row[0] as string) == selectionsLine[0] && !string.IsNullOrEmpty(row[1] + string.Empty))
                                    {
                                        sbWrite.AppendLine(TransformString(row[1] + string.Empty) + selectionsLine[1]);
                                    }
                                    else
                                    {
                                        sbWrite.AppendLine(selectionsLine[0]);
                                    }
                                    r++;
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
                        //commented or empty
                        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                        {
                            sbWrite.AppendLine(line);
                            continue;
                        }
                        else if (line.StartsWith("#MSG,") || line == "#MSG")
                        {
                            LastMSGType = line;
                            sbWrite.AppendLine(line);
                            readmode = true;
                            continue;
                        }
                        else if (line.StartsWith("#MSGVOICE,"))
                        {
                            LastMSGType = line;
                            sbWrite.AppendLine(line);
                            sbWrite.AppendLine(sr.ReadLine());
                            readmode = true;
                            continue;
                        }
                        else if (line.StartsWith("#SELECT,"))
                        {
                            sbWrite.AppendLine(line);
                            int selectioncnt = int.Parse(line.Split(',')[1].TrimStart().Substring(0, 1), CultureInfo.InvariantCulture);
                            string[] selectionsLine;
                            for (int i = 0; i < selectioncnt; i++)
                            {
                                selectionsLine = Regex.Replace(sr.ReadLine(), @"([^	]+)([	]+[0-9]{1,2}.*)", "$1{{|}}$2").Split(new string[] { "{{|}}" }, StringSplitOptions.None);
                                var row = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r];
                                if ((row[0] as string) == selectionsLine[0] && !string.IsNullOrEmpty(row[1] + string.Empty))
                                {
                                    sbWrite.AppendLine(TransformString(row[1] + string.Empty) + selectionsLine[1]);
                                }
                                else
                                {
                                    sbWrite.AppendLine(selectionsLine[0]);
                                }
                                r++;
                            }
                            //sbWrite.AppendLine(Environment.NewLine);
                            WriteIt = true;
                            continue;
                        }
                    }

                    sbWrite.AppendLine(line);
                }
            }

            if (WriteIt && sbWrite.ToString().Length > 0)
            {
                File.WriteAllText(thDataWork.FilePath, sbWrite.ToString(), Encoding.GetEncoding(932));
                return true;
            }
            else
            {
                return false;
            }
        }

        private string PreReduceTranslation(string newLine)
        {
            return newLine
                .Replace(" a ", string.Empty)
                .Replace("The ", string.Empty)
                .Replace(" the ", string.Empty);
        }

        private string PostTransFormLineCleaning(string s)
        {
            return s
                .Trim()
                .Trim('_')
                //.Replace("´", string.Empty)
                .Replace("。。", "。")
                .Replace("。。", "。")
                .Replace("…_", "…")
                .Replace("_…", "…")
                .Replace("_…_", "…")
                .Replace("_~", "~")
                .Replace("_~_", "~")
                .Replace("「…", "「")
                .Replace("…「", "「")
                .Replace("……", "…")
                .Replace("……", "…")
                .Replace("!!", "!")
                .Replace("!!", "!")
                .Replace("??", "?")
                .Replace("”", string.Empty);
            //.Replace("_", "　");
            //.Replace("「``", "「")
            //.Replace("「\"", "「")
            //.Replace("「`", "「");
        }

        readonly string[][] ENJPcharPairs = new string[][] {
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
               new string[2]{ "　", " " },
               new string[2]{ " ...", "…" },
               new string[2]{ "...", "…" },
               new string[2]{ ", ", "、" },
               new string[2]{ ",", "、" },
               new string[2]{ ". ", "。" },
               new string[2]{ ".", "。" },
               new string[2]{ "\"", string.Empty },
               //new string[2]{ "\"", "”" },
               new string[2]{ "...", "…" },
               new string[2]{ "……", "…" },
               new string[2]{ "……", "…" },
               new string[2]{ "……", "…" },
               new string[2]{ "……", "…" },
               new string[2]{ "? ", "？" },
               new string[2]{ "! ", "！" },
               new string[2]{ " '", string.Empty },
               new string[2]{ " ’", string.Empty },
               new string[2]{ "'", string.Empty },
               new string[2]{ "’", string.Empty },
               new string[2]{ "{", string.Empty },
               new string[2]{ "}", string.Empty },
               new string[2]{ " [", "（" },
               new string[2]{ "] ", "）" },
               new string[2]{ "[", "（" },
               new string[2]{ "]", "）" },
               //new string[2]{ " [", "【" },
               //new string[2]{ "] ", "】" },
               //new string[2]{ "[", "【" },
               //new string[2]{ "]", "】" },
               new string[2]{ "#", string.Empty },
               new string[2]{ "$", string.Empty },
               new string[2]{ "@", string.Empty },
               new string[2]{ "/", "／" },
               new string[2]{ "\\", "＼" },
               new string[2]{ "(", "（" },
               new string[2]{ ")", "）" },
               new string[2]{ ":", "：" },
               new string[2]{ ";", "；" },
               new string[2]{ "*", "＊" },
               //new string[2]{ " '", "´" },
               //new string[2]{ " ’", "´" },,
               //new string[2]{ "'", "´" },
               //new string[2]{ "’", "´" },
               new string[2]{ " ", "_" },
               //new string[2]{ "#", "＃" },
               //new string[2]{ "$", "＄" },
               new string[2]{ "%", "％" },
               new string[2]{ "&", "＆" },
               new string[2]{ "「", string.Empty },
               new string[2]{ "『", string.Empty },
               new string[2]{ "」", string.Empty },
               new string[2]{ "』", string.Empty }
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
    };

        private string TransformString(string input)
        {
            string ret = input;
            ret = TransformStringWords(ret);
            int charsLength = ENJPcharPairs.Length;
            for (int i = 0; i < charsLength; i++)
            {
                ret = ret.Replace(ENJPcharPairs[i][0], ENJPcharPairs[i][1]);
            }

            return ret;
        }

        readonly string[][] ENJPWordsReplacementPairs = new string[][] {
               new string[2]{ "First", "1st" },
               new string[2]{ "Second", "2nd" },
               new string[2]{ "Third", "3rd" },
               new string[2]{ "Fourth", "4th" },
               new string[2]{ "Fifth", "5th" },
               new string[2]{ "Sixth", "6th" },
               new string[2]{ "Seventh", "7th" },
               new string[2]{ "Eighth", "8th" },
               new string[2]{ "Ninth", "9th" },
               new string[2]{ "Tenth", "10th" },
               new string[2]{ "Eleventh", "11th" },
               new string[2]{ "Twelfth", "12th" },
               new string[2]{ "Thirteenth", "13th" },
               new string[2]{ "Fourteenth", "14th" },
               new string[2]{ "Fifteenth", "15th" },
               new string[2]{ "Sixteenth", "16th" },
               new string[2]{ "Seventeenth", "17th" },
               new string[2]{ "Eighteenth", "18th" },
               new string[2]{ "Nineteenth", "19th" },
               new string[2]{ "Twentieth", "20th" },
               new string[2]{ "Twenty-first", "21st" },
               new string[2]{ "Twenty-second", "22nd" },
               new string[2]{ "Twenty-third", "23rd" },
               new string[2]{ "Twenty-fourth", "24th" },
               new string[2]{ "Twenty-fifth", "25th" },
               new string[2]{ "Thirtieth", "30th" },
               new string[2]{ "Thirty-first", "31st" },
               new string[2]{ "Thirty-second", "32nd" },
               new string[2]{ "Thirty-third", "33rd" },
               new string[2]{ "Thirty-fourth", "34th" },
               new string[2]{ "Fortieth", "40th" },
               new string[2]{ "Fiftieth", "50th" },
               new string[2]{ "Sixtieth", "60th" },
               new string[2]{ "Seventieth", "70th" },
               new string[2]{ "Eightieth", "80th" },
               new string[2]{ "Ninetieth", "90th" },
               new string[2]{ "Hundredth", "100th" },
               new string[2]{ "Thousandth", "1000th" },
               new string[2]{ "ho-ho-ho-ho", "ho-ho" },
               new string[2]{ "ho-ho-ho", "ho-ho" }
            };

        private string TransformStringWords(string input)
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
