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
                int readmodelines = 0;
                StringBuilder sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    //commented or empty
                    if (!readmode && string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                    {
                        continue;
                    }
                    else if (readmode)
                    {
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("//") /*string.IsNullOrEmpty(line)*/)
                        {
                            if(!string.IsNullOrWhiteSpace(sb.ToString()))
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(sb.ToString());
                            }

                            readmode = false;
                            readmodelines = 0;
                            sb.Clear();
                        }
                        else
                        {
                            if (readmodelines > 0)
                            {
                                sb.Append(Environment.NewLine);
                            }
                            sb.Append(line);
                            readmodelines++;
                        }

                        if (line.StartsWith("#"))
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
                        for (int i=0; i< selectioncnt; i++)
                        {
                            thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Regex.Replace(sr.ReadLine(), @"([^	]+)(	+[0-9]{1,2}.*)", "$1"));
                        }
                        continue;
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
            string LastMSGType= string.Empty;
            using (StreamReader sr = new StreamReader(thDataWork.FilePath, Encoding.GetEncoding(932)))
            {
                string line;
                bool readmode = false;
                int readmodelines = 0;
                StringBuilder sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    //commented or empty
                    if (!readmode && string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                    {
                        sbWrite.AppendLine(line);
                        continue;
                    }
                    else if (readmode)
                    {
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("//") /*string.IsNullOrEmpty(line)*/)
                        {
                            if (!string.IsNullOrWhiteSpace(sb.ToString()))
                            {
                                var row = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r];
                                if ((row[0] as string) == sb.ToString() && !string.IsNullOrEmpty(row[1] + string.Empty))
                                {
                                    string newLine = FunctionsString.SplitMultiLineIfBeyondOfLimit(thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r][1] + Environment.NewLine, 37);
                                    //int origLinesCount = FunctionsString.GetLinesCount(sb.ToString());
                                    int newLinesCount = FunctionsString.GetLinesCount(newLine);

                                    if (newLinesCount > 4)
                                    {
                                        int cnt = 0;
                                        int cntMax = 5;
                                        string retLine = string.Empty;
                                        newLine = TransformString(newLine);
                                        LastMSGType = LastMSGType.Replace("#MSGVOICE,", "#MSG,");
                                        foreach (var subline in newLine.SplitToLines())
                                        {
                                            cnt++;
                                            if (cnt == cntMax)
                                            {
                                                cntMax += 4;
                                                retLine += Environment.NewLine + LastMSGType + Environment.NewLine;
                                            }
                                            retLine += subline + Environment.NewLine;
                                        }

                                        sbWrite.AppendLine(retLine);
                                    }
                                    else
                                    {
                                        sbWrite.AppendLine(TransformString(newLine) + Environment.NewLine);
                                    }

                                }
                                else
                                {
                                    sbWrite.AppendLine(sb.ToString() + Environment.NewLine);
                                }
                                r++;
                            }
                            readmode = false;
                            readmodelines = 0;
                            sb.Clear();
                            WriteIt = true;
                            LastMSGType = string.Empty;
                        }
                        else
                        {
                            if (readmodelines > 0)
                            {
                                sb.Append(Environment.NewLine);
                            }
                            sb.Append(line);
                            readmodelines++;
                        }

                        if (line.StartsWith("#"))
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
               new string[2]{ "\"", "”" },
               new string[2]{ "...", "…" },
               new string[2]{ "……", "…" },
               new string[2]{ "……", "…" },
               new string[2]{ "? ", "？" },
               new string[2]{ "! ", "！" },
               new string[2]{ " '", "´" },
               new string[2]{ " ’", "´" },
               new string[2]{ "'", "´" },
               new string[2]{ "’", "´" },
               new string[2]{ " ", "_" }
               //new string[2]{ "#", "＃" },
               //new string[2]{ "$", "＄" },
               //new string[2]{ "%", "％" },
               //new string[2]{ "&", "＆" },
               //new string[2]{ "(", "（" },
               //new string[2]{ ")", "）" },
               //new string[2]{ "*", "＊" },
               //new string[2]{ ",", "，" },
               //new string[2]{ "/", "／" },
               //new string[2]{ ":", "：" },
               //new string[2]{ ";", "；" },
               //new string[2]{ "@", "＠" },
               //new string[2]{ "\\", "＼" },
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
            int charsLength = ENJPcharPairs.Length;
            for (int i = 0; i < charsLength; i++)
            {
                ret = ret.Replace(ENJPcharPairs[i][0], ENJPcharPairs[i][1]);
            }

            return ret;
        }
    }
}
