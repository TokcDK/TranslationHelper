using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.KiriKiri
{
    class SCRIPT : StringFileFormatBase
    {
        public SCRIPT()
        {
        }

        internal override bool Open()
        {
            return KiriKiriScriptScenarioOpen();
        }

        string KiriKiriVariableSearchRegexPattern = string.Empty;
        string KiriKiriVariableSearchRegexFullPattern = string.Empty;
        string KiriKiriQuotePattern;
        private bool KiriKiriScriptScenarioOpen()
        {
            try
            {
                string fileName = Path.GetFileName(ProjectData.FilePath);

                //ks scn tjs open
                KiriKiriVariableSearchRegexPattern = @"( |'|\(|\[|,|\.)o\.";
                KiriKiriVariableSearchRegexFullPattern = @"((^o\.)|(" + KiriKiriVariableSearchRegexPattern + @"))([^\.|\s|'|\)|\]|;|:|,]+)";
                string Quote1 = "\"";
                //string Quote2 = "\'";
                KiriKiriQuotePattern = Quote1 + "(.+)" + Quote1 + ";$";
                bool TeachingFeelingCS = false;
                string tmpfile = File.ReadAllText(ProjectData.FilePath);
                if (tmpfile.Contains("[p_]") || tmpfile.Contains("[lr_]"))
                {
                    TeachingFeelingCS = true;
                }
                tmpfile = null;
                using (StreamReader file = new StreamReader(ProjectData.FilePath, TeachingFeelingCS ? Encoding.UTF8 : Encoding.GetEncoding(932)))
                {
                    string line;
                    bool iscomment = false;

                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith("/*"))
                        {
                            iscomment = true;
                        }


                        if ((iscomment || string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("//") || Regex.IsMatch(line, @"\s*//")))
                        {
                        }
                        else
                        {
                            if (TeachingFeelingCS)//Teaching Feeling cs
                            {
                                if (line.Contains("[p_]") || line.Contains("[lr_]"))
                                {
                                    int lastMergeIndex = -1;
                                    int startMergeIndex = -1;
                                    string[] temp = line.Split(']');
                                    int Cnt = temp.Length;

                                    //поиск первой и последней части для записи
                                    for (int i = 0; i < Cnt; i++)
                                    {
                                        temp[i] += "]";
                                        string substring = temp[i];
                                        if (lastMergeIndex > -1 && i >= startMergeIndex && i <= lastMergeIndex)
                                        {
                                            line += temp[i];
                                        }
                                        else if (temp[i].EndsWith("[lr_]") || temp[i].EndsWith("[p_]"))
                                        {
                                            lastMergeIndex = i;
                                            startMergeIndex = i;
                                            while (startMergeIndex > 0 && (!temp[startMergeIndex - 1].Contains("[") || temp[startMergeIndex - 1].Contains("[name")))
                                            {
                                                startMergeIndex -= 1;
                                            }
                                        }
                                    }

                                    //обнуление, для записи нового чистого значения
                                    line = string.Empty;

                                    //запись значения из найденного диапазона
                                    if (lastMergeIndex > -1)
                                    {
                                        for (int i = startMergeIndex; i <= lastMergeIndex; i++)
                                        {
                                            line += temp[i];
                                        }
                                    }

                                    //убрать идентификатор окончания строки
                                    line = line.Replace("[lr_]", string.Empty).Replace("[p_]", string.Empty);

                                    //line = Regex.Replace(line, @"^\s*(\[[a-z\/_]+\])*((\[name\])?.+)\[(lr|p)_\]\s*$", "$2");
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                                        _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("Teeching feeling cs");
                                    }
                                }
                            }
                            else
                            {
                                if (line == "@nanasi")//Life with daughter
                                {
                                    line = file.ReadLine();
                                    while (!line.EndsWith("[ll]") && !line.EndsWith("@s"))
                                    {
                                        while (line.StartsWith("@"))
                                        {
                                            line = file.ReadLine();
                                        }
                                        line += Environment.NewLine;
                                        line += file.ReadLine();
                                    }
                                    line = line.Remove(line.Length - 4);//удаление последних четырех символов "[ll]" или "\r\n@s"
                                    _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                                    _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(string.Empty);
                                }
                                else if (line.EndsWith("[k]")) // text ;Magic Swordsman Rene
                                {
                                    line = line.Replace("[k]", string.Empty);
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                                        _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("[k] = end of line");
                                    }
                                }
                                else if (line.StartsWith("*")) // text ;Magic Swordsman Rene
                                {
                                    line = line.Remove(0, 1);
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                                        _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(string.Empty);
                                    }
                                }
                                else if (line.EndsWith("[r]")) //text, first line ;Magic Swordsman Rene
                                {
                                    line = line.Replace("[r]", string.Empty).Trim();
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                                        _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("[r] = carriage return");
                                    }
                                }
                                else if (line.StartsWith("o.") || Regex.IsMatch(line, KiriKiriVariableSearchRegexPattern)) //variable, which is using even for displaing and should be translated in all files ;Magic Swordsman Rene
                                {
                                    MatchCollection matches = Regex.Matches(line, KiriKiriVariableSearchRegexFullPattern);

                                    bool startswith = line.StartsWith("o.");
                                    for (int m = 0; m < matches.Count; m++)
                                    {
                                        _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(matches[m].Value.Remove(0, startswith ? 2 : 3));
                                        _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(T._("Variable>Must be Identical in all files>Only A-Za-z0-9" + Environment.NewLine + "line: " + line));
                                        if (startswith)
                                        {
                                            startswith = false;//o. в начале встречается только в первый раз
                                        }
                                    }
                                }
                                else if (line.StartsWith("@notice text="))// ; Magic Swordsman Rene
                                {
                                    line = line.Remove(0, 13);//удаление "@notice text="
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                                        _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("@notice text=");
                                    }
                                }
                                else if (line.StartsWith("Name = '"))// ; Magic Swordsman Rene
                                {
                                    line = line.Remove(line.Length - 2, 2).Remove(0, 8);
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                                        _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("Name = '");
                                    }
                                }
                                else if (Regex.IsMatch(line, "\\\"(.*?)\\\""))// ; Magic Swordsman Rene
                                {
                                    //https://stackoverflow.com/questions/13024073/regex-c-sharp-extract-text-within-double-quotes
                                    var matches = Regex.Matches(line, "\\\"(.*?)\\\"");
                                    string subline;
                                    foreach (var m in matches)
                                    {
                                        subline = m.ToString();
                                        if (string.IsNullOrWhiteSpace(subline.Replace("\"", string.Empty)) || line.Replace("\"", string.Empty).IsDigitsOnly())
                                        {
                                        }
                                        else
                                        {
                                            _ = ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Add(subline.Remove(subline.Length - 1, 1).Remove(0, 1));
                                            _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(line);
                                        }
                                    }
                                }
                            }
                        }

                        if (line.EndsWith("*/"))
                        {
                            iscomment = false;
                        }
                    }

                    return true;
                }
            }
            catch
            {

            }

            return false;
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
