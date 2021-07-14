using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri.Games.FGroup2;

namespace TranslationHelper.Formats.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive
{
    class KS : FGroup2Base
    {
        public KS() : base()
        {
        }

        internal override Encoding FileEncoding()
        {
            return new UTF8Encoding(false);
        }

        //old code

        //internal override bool Open()
        //{
        //    return OpenKS();
        //}

        //internal override bool Save()
        //{
        //    return SaveKS();
        //}

        //private readonly string SelectMessageRegex = @"SELECT→\[link.+\]([^\[]+)\[endlink\].*";
        //private readonly string TitleRegex = "\\[title name\\=\\\"([^\\\"]+)\\\"\\]";

        //private bool OpenKS()
        //{
        //    string line;

        //    string tablename = Path.GetFileName(ProjectData.FilePath);

        //    AddTables(tablename);

        //    using (StreamReader reader = new StreamReader(ProjectData.FilePath))
        //    {
        //        bool messageParse = false;
        //        bool statusInfoParse = false;
        //        bool Conditionalmessage = false;
        //        int MessageLineNum = 0;
        //        StringBuilder Message = new StringBuilder();
        //        StringBuilder MessageInfo = new StringBuilder();
        //        while (!reader.EndOfStream)
        //        {
        //            line = reader.ReadLine();

        //            if (statusInfoParse)
        //            {
        //                if (line == "[s]")
        //                {
        //                    statusInfoParse = false;
        //                    AddRowData(CheckAndRemoveRubyText(Message.ToString()).Trim(), "statusInfo:" + Environment.NewLine + MessageInfo.ToString());
        //                    ClearData(Message, MessageInfo, ref MessageLineNum);
        //                }
        //                else if (line.EndsWith("[r]") && line.Length > 3)
        //                {
        //                    string data = line.Remove(line.Length - 3, 3);
        //                    Message.AppendLine(data);
        //                    MessageInfo.AppendLine("[r]");
        //                }
        //                else if (line.EndsWith("[p]") && line.Length > 3)
        //                {
        //                    string data = line.Remove(line.Length - 3, 3);
        //                    Message.AppendLine(data);
        //                    MessageInfo.AppendLine("[p]");
        //                }
        //            }
        //            else if (messageParse)
        //            {
        //                //[message_clear]
        //                //[boku]
        //                //[if exp="f.airi_boku_level == 0"]
        //                //きっとそうやってお互いに依存していくことから脱していくのだろう。[lr]
        //                //妹にもいずれ好きな人ができて、自分から離れていく････････そう考えると[r]
        //                //涙が出るが、それが彼女にとっての幸せだろう。[phr]
        //                //[elsif exp="f.airi_boku_level >= 1"]
        //                //藍里も求めればいつでも応じてくれる。[lr]
        //                //快楽が身体を変化させるのか、少女が大人の階段を上るように、どこか女性[r]
        //                //らしく日々成長している感じがする。[phr]
        //                //[endif]

        //                if (!Conditionalmessage && line.StartsWith("[if "))
        //                {
        //                    Conditionalmessage = true;
        //                    MessageInfo.AppendLine(MessageLineNum + " [if ");
        //                    MessageLineNum++;
        //                }
        //                else if (line.Contains("[phr]"))
        //                {
        //                    if (Regex.IsMatch(line, SelectMessageRegex))
        //                    {
        //                        //SELECT→[link storage="event_3_c.ks" target="*藍里" clickse="se_maoudamashii_se_pc01.ogg" clicksebuf=0]藍里が先だ！[endlink][phr]
        //                        string data = Regex.Replace(line, SelectMessageRegex, "$1");
        //                        Message.Append(data);
        //                        MessageInfo.Append(MessageLineNum + " SELECTlink [phr]");
        //                    }
        //                    else
        //                    {
        //                        string data = line.Replace("[phr]", string.Empty);
        //                        string extra = GetExtraSymbol(line);
        //                        data = ExtraLineCorrection(data);
        //                        Message.Append(data);
        //                        MessageInfo.Append(MessageLineNum + " " + extra + " [phr]");
        //                    }

        //                    AddRowData(tablename, CheckAndRemoveRubyText(Message.ToString()).Trim(), "Message:" + Environment.NewLine + MessageInfo.ToString());

        //                    ClearData(Message, MessageInfo, ref MessageLineNum);
        //                    if (!Conditionalmessage)
        //                    {
        //                        messageParse = false;
        //                    }
        //                }
        //                else if (Conditionalmessage && (line.StartsWith("[elsif")))
        //                {
        //                    Conditionalmessage = false;
        //                    MessageInfo.AppendLine(MessageLineNum + " [elsif");
        //                    MessageLineNum++;
        //                }
        //                else if (Conditionalmessage && (line.StartsWith("[else")))
        //                {
        //                    Conditionalmessage = false;
        //                    MessageInfo.AppendLine(MessageLineNum + " [else");
        //                    MessageLineNum++;
        //                }
        //                else if (Regex.IsMatch(line, SelectMessageRegex))
        //                {
        //                    //SELECT→[link storage="event_3_c.ks" target="*藍里" clickse="se_maoudamashii_se_pc01.ogg" clicksebuf=0]藍里が先だ！[endlink][phr]
        //                    string Data = Regex.Replace(line, SelectMessageRegex, "$1");
        //                    Message.AppendLine(Data);
        //                    MessageInfo.AppendLine(MessageLineNum + " SELECTlink" + (line.EndsWith("[r]") ? " [r]" : line.EndsWith("[lr]") ? " [lr]" : string.Empty));
        //                    MessageLineNum++;
        //                }
        //                else if (line.EndsWith("[r]"))
        //                {
        //                    string data = line.Remove(line.Length - 3, 3);
        //                    data = ExtraLineCorrection(data);
        //                    string extra = GetExtraSymbol(line);
        //                    Message.AppendLine(data);
        //                    MessageInfo.AppendLine(MessageLineNum + " " + extra + " [r]");
        //                    MessageLineNum++;
        //                }
        //                else if (line.EndsWith("[lr]"))
        //                {
        //                    string data = line.Remove(line.Length - 4, 4);
        //                    data = ExtraLineCorrection(data);
        //                    string extra = GetExtraSymbol(line);
        //                    Message.AppendLine(data);
        //                    MessageInfo.AppendLine(MessageLineNum + " " + extra + " [lr]");
        //                    MessageLineNum++;
        //                }
        //                else if (GetExtraSymbol(line).Length > 0 || line.Contains("[font"))
        //                {
        //                    string data = ExtraLineCorrection(line);
        //                    string extra = GetExtraSymbol(line);
        //                    Message.AppendLine(data);
        //                    MessageInfo.AppendLine(MessageLineNum + " " + extra + " ");
        //                    MessageLineNum++;
        //                }
        //                else if (line == "[resetfont]")
        //                {
        //                    string data = CheckAndRemoveRubyText(Message.ToString()).Trim();
        //                    AddRowData(tablename, data, "Message:" + Environment.NewLine + MessageInfo.ToString());

        //                    ClearData(Message, MessageInfo, ref MessageLineNum);
        //                    if (!Conditionalmessage)
        //                    {
        //                        messageParse = false;
        //                    }
        //                }
        //                else if (string.IsNullOrEmpty(line) && Message.Length > 0)
        //                {
        //                    string data = CheckAndRemoveRubyText(Message.ToString()).Trim();
        //                    AddRowData(tablename, data, "Message:" + Environment.NewLine + MessageInfo.ToString());

        //                    ClearData(Message, MessageInfo, ref MessageLineNum);
        //                }
        //            }
        //            else
        //            {
        //                if (line.TrimStart().StartsWith("*event_start"))
        //                {
        //                    //*event_start_2|４日目（休み時間）★
        //                    //*event_start|それから････････

        //                    string[] RowData = line.Split('|');
        //                    AddRowData(tablename, RowData.Length > 1 ? RowData[1] : string.Empty, RowData[0]);
        //                }
        //                else if (line.TrimStart().StartsWith("[title name="))
        //                {
        //                    //[title name="バージョン情報"]
        //                    string RowData = Regex.Replace(line, TitleRegex, "$1");
        //                    AddRowData(tablename, RowData, "title name");
        //                }
        //                else if (line == "[message_clear]")
        //                {
        //                    //[message_clear]
        //                    //[boku]
        //                    //[「]････････････････････････････････････････。[」][phr]
        //                    messageParse = true;
        //                }
        //                else if (line == "[er]")
        //                {
        //                    statusInfoParse = true;
        //                }
        //            }
        //        }
        //    }

        //    return CheckTablesContent(tablename);
        //}

        //private static string GetExtraSymbol(string line)
        //{
        //    string ret = string.Empty;
        //    if (line.Contains("[（]"))
        //    {
        //        ret += "[（]";
        //    }
        //    else if (line.Contains("[「]"))
        //    {
        //        ret += "[「]";
        //    }
        //    if (line.Contains("[）]"))
        //    {
        //        ret += "[）]";
        //    }
        //    else if (line.Contains("[」]"))
        //    {
        //        ret += "[」]";
        //    }
        //    return ret;
        //}

        //private static string ExtraLineCorrection(string data)
        //{
        //    if (data.Contains("[（]"))
        //    {
        //        data = data.Replace("[（]", string.Empty);
        //    }
        //    if (data.Contains("[「]"))
        //    {
        //        data = data.Replace("[「]", string.Empty);
        //    }
        //    if (data.Contains("[）]"))
        //    {
        //        data = data.Replace("[）]", string.Empty);
        //    }
        //    if (data.Contains("[」]"))
        //    {
        //        data = data.Replace("[」]", string.Empty);
        //    }
        //    if (data.Contains("[lr]"))
        //    {
        //        data = data.Replace("[lr]", string.Empty);
        //    }
        //    if (data.Contains("[phr]"))
        //    {
        //        data = data.Replace("[phr]", string.Empty);
        //    }
        //    if (data.StartsWith("[font size="))
        //    {
        //        data = Regex.Replace(data, @"\[font size\=\d{1,3}\]", string.Empty);
        //    }
        //    return data;
        //}

        //private bool SaveKS()
        //{
        //    try
        //    {
        //        string line;

        //        string tablename = Path.GetFileName(ProjectData.FilePath);

        //        using (StreamReader reader = new StreamReader(ProjectData.FilePath))
        //        {
        //            bool messageParse = false;
        //            bool statusInfoParse = false;
        //            bool Conditionalmessage = false;
        //            int MessageLineNum = 0;
        //            //int RowIndex = 0;
        //            StringBuilder Message = new StringBuilder();
        //            StringBuilder MessageInfo = new StringBuilder();
        //            StringBuilder ResultForWrite = new StringBuilder();
        //            while (!reader.EndOfStream)
        //            {
        //                line = reader.ReadLine();

        //                if (statusInfoParse)
        //                {
        //                    if (line == "[s]")
        //                    {
        //                        statusInfoParse = false;
        //                    }
        //                    else if (line.EndsWith("[r]") && line.Length > 3)
        //                    {
        //                        string data = line.Remove(line.Length - 3, 3);
        //                        data = ExtraLineCorrection(data);
        //                        string extra = GetExtraSymbol(line);
        //                        Message.AppendLine(data);
        //                        MessageInfo.AppendLine(line);
        //                        MessageLineNum++;
        //                        continue;
        //                    }
        //                    else if (line.EndsWith("[p]") && line.Length > 3)
        //                    {
        //                        string data = line.Remove(line.Length - 3, 3);
        //                        data = ExtraLineCorrection(data);
        //                        string extra = GetExtraSymbol(line);
        //                        Message.AppendLine(data);
        //                        MessageInfo.AppendLine(line);
        //                        MessageLineNum++;
        //                        continue;
        //                    }
        //                }
        //                else if (messageParse)
        //                {
        //                    //[message_clear]
        //                    //[boku]
        //                    //[if exp="f.airi_boku_level == 0"]
        //                    //きっとそうやってお互いに依存していくことから脱していくのだろう。[lr]
        //                    //妹にもいずれ好きな人ができて、自分から離れていく････････そう考えると[r]
        //                    //涙が出るが、それが彼女にとっての幸せだろう。[phr]
        //                    //[elsif exp="f.airi_boku_level >= 1"]
        //                    //藍里も求めればいつでも応じてくれる。[lr]
        //                    //快楽が身体を変化させるのか、少女が大人の階段を上るように、どこか女性[r]
        //                    //らしく日々成長している感じがする。[phr]
        //                    //[endif]

        //                    if (!Conditionalmessage && line.StartsWith("[if "))
        //                    {
        //                        Conditionalmessage = true;
        //                        //MessageInfo.AppendLine(MessageLineNum + " [if ");
        //                        MessageLineNum++;
        //                    }
        //                    else if (line.EndsWith("[phr]"))
        //                    {
        //                        if (Regex.IsMatch(line, SelectMessageRegex))
        //                        {
        //                            //SELECT→[link storage="event_3_c.ks" target="*藍里" clickse="se_maoudamashii_se_pc01.ogg" clicksebuf=0]藍里が先だ！[endlink][phr]
        //                            string data = Regex.Replace(line, SelectMessageRegex, "$1");
        //                            Message.Append(data);
        //                            MessageInfo.Append(line);
        //                        }
        //                        else
        //                        {
        //                            string data = line.Remove(line.Length - 5, 5);
        //                            string extra = GetExtraSymbol(line);
        //                            data = ExtraLineCorrection(data);
        //                            Message.Append(data);
        //                            MessageInfo.Append(line);
        //                        }


        //                        ResultForWrite.AppendLine(GetTranslation(tablename, Message, MessageInfo/*, ref RowIndex*/));

        //                        //AddRowData(tablename, CheckAndRemoveRubyText(Message.ToString()), "Message:" + Environment.NewLine + MessageInfo.ToString());

        //                        ClearData(Message, MessageInfo, ref MessageLineNum);
        //                        if (!Conditionalmessage)
        //                        {
        //                            messageParse = false;
        //                        }
        //                        continue;
        //                    }
        //                    else if (Conditionalmessage && (line.StartsWith("[elsif")))
        //                    {
        //                        Conditionalmessage = false;
        //                        //MessageInfo.AppendLine(MessageLineNum + " [elsif");
        //                        MessageLineNum++;
        //                    }
        //                    else if (Conditionalmessage && (line.StartsWith("[else")))
        //                    {
        //                        Conditionalmessage = false;
        //                        //MessageInfo.AppendLine(MessageLineNum + " [else");
        //                        MessageLineNum++;
        //                    }
        //                    else if (Regex.IsMatch(line, SelectMessageRegex))
        //                    {
        //                        //SELECT→[link storage="event_3_c.ks" target="*藍里" clickse="se_maoudamashii_se_pc01.ogg" clicksebuf=0]藍里が先だ！[endlink][phr]
        //                        string Data = Regex.Replace(line, SelectMessageRegex, "$1");
        //                        Message.AppendLine(Data);
        //                        MessageInfo.AppendLine(line);
        //                        MessageLineNum++;
        //                        continue;
        //                    }
        //                    else if (line.EndsWith("[r]"))
        //                    {
        //                        string data = line.Remove(line.Length - 3, 3);
        //                        data = ExtraLineCorrection(data);
        //                        string extra = GetExtraSymbol(line);
        //                        Message.AppendLine(data);
        //                        MessageInfo.AppendLine(line);
        //                        MessageLineNum++;
        //                        continue;
        //                    }
        //                    else if (line.EndsWith("[lr]"))
        //                    {
        //                        string data = line.Remove(line.Length - 4, 4);
        //                        data = ExtraLineCorrection(data);
        //                        string extra = GetExtraSymbol(line);
        //                        Message.AppendLine(data);
        //                        MessageInfo.AppendLine(line);
        //                        MessageLineNum++;
        //                        continue;
        //                    }
        //                    else if (line == "[resetfont]")
        //                    {
        //                        //string data = CheckAndRemoveRubyText(Message.ToString()).Trim();
        //                        ResultForWrite.AppendLine(GetTranslation(tablename, Message, MessageInfo/*, ref RowIndex*/));
        //                        //AddRowData(tablename, data, "Message:" + Environment.NewLine + MessageInfo.ToString());
        //                        ClearData(Message, MessageInfo, ref MessageLineNum);
        //                        if (!Conditionalmessage)
        //                        {
        //                            messageParse = false;
        //                        }
        //                        continue;
        //                    }
        //                    else if (string.IsNullOrEmpty(line) && Message.Length > 0)
        //                    {
        //                        //string data = CheckAndRemoveRubyText(Message.ToString()).Trim();
        //                        ResultForWrite.AppendLine(GetTranslation(tablename, Message, MessageInfo/*, ref RowIndex*/));
        //                        //AddRowData(tablename, data, "Message:" + Environment.NewLine + MessageInfo.ToString());
        //                        ClearData(Message, MessageInfo, ref MessageLineNum);
        //                        continue;
        //                    }
        //                }
        //                else
        //                {
        //                    if (line.TrimStart().StartsWith("*event_start"))
        //                    {
        //                        //*event_start_2|４日目（休み時間）★
        //                        //*event_start|それから････････

        //                        string[] RowData = line.Split('|');
        //                        //var row = ProjectData.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
        //                        if (RowData.Length < 2 || !IsValidString(RowData[1]))
        //                        {
        //                            continue;
        //                        }

        //                        //string value;
        //                        if (IsTranslationValidFor(RowData[1]))
        //                        {
        //                            RowData[1] = ProjectData.TablesLinesDict[RowData[1]];
        //                            line = string.Join("|", RowData);
        //                        }
        //                        //RowIndex++;
        //                        //AddRowData(tablename, RowData.Length > 1 ? RowData[1] : string.Empty, RowData[0]);
        //                    }
        //                    else if (line.TrimStart().StartsWith("[title name="))
        //                    {
        //                        //[title name="バージョン情報"]
        //                        string RowData = Regex.Replace(line, TitleRegex, "$1");
        //                        //var row = ProjectData.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
        //                        if (!IsValidString(RowData))
        //                        {
        //                            continue;
        //                        }

        //                        if (IsTranslationValidFor(RowData))
        //                        {
        //                            line = line.Replace(RowData, ProjectData.TablesLinesDict[RowData]);
        //                        }
        //                        //RowIndex++;
        //                        //AddRowData(tablename, RowData, "title name");
        //                    }
        //                    else if (line == "[message_clear]")
        //                    {
        //                        //[message_clear]
        //                        //[boku]
        //                        //[「]････････････････････････････････････････。[」][phr]
        //                        messageParse = true;
        //                    }
        //                    else if (line == "[er]")
        //                    {
        //                        //[message_clear]
        //                        //[boku]
        //                        //[「]････････････････････････････････････････。[」][phr]
        //                        statusInfoParse = true;
        //                    }
        //                }

        //                ResultForWrite.AppendLine(line);
        //            }

        //            File.WriteAllText(Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchDirName, tablename), ResultForWrite.ToString(), Encoding.GetEncoding(932));
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //private bool IsTranslationValidFor(string DataString)
        //{
        //    string value;
        //    if (ProjectData.TablesLinesDict.ContainsKey(DataString) && !string.IsNullOrEmpty(value = ProjectData.TablesLinesDict[DataString]) && !Equals(DataString, value))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //private static void ClearData(StringBuilder Message, StringBuilder MessageInfo, ref int MessageLineNum)
        //{
        //    Message.Clear();
        //    MessageInfo.Clear();
        //    MessageLineNum = 0;
        //}

        //private string GetTranslation(string tablename, StringBuilder Message, StringBuilder MessageInfo/*, ref int RowIndex*/)
        //{
        //    //var row = ProjectData.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
        //    string cleanedMessage = CheckAndRemoveRubyText(Message.ToString()).Trim();
        //    bool IsTranslated = false;
        //    if (IsValidString(cleanedMessage))
        //    {

        //        if (IsTranslationValidFor(cleanedMessage))
        //        {
        //            string[] TranslatedLines = (ProjectData.TablesLinesDict[cleanedMessage]).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        //            string[] OrigLines = MessageInfo.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

        //            if (TranslatedLines.Length == OrigLines.Length)
        //            {
        //                Message.Clear();
        //                for (int l = 0; l < OrigLines.Length; l++)
        //                {
        //                    AddAllRequiredSymbols(TranslatedLines[l], OrigLines[l], Message);
        //                }
        //                IsTranslated = true;
        //            }
        //        }
        //        //RowIndex++;
        //    }

        //    return IsTranslated ? Message.ToString() : MessageInfo.ToString();
        //}

        //private void AddAllRequiredSymbols(string translation, string original, StringBuilder Message)
        //{
        //    string fontSize = string.Empty;
        //    if (original.StartsWith("[font size="))
        //    {
        //        fontSize = Regex.Replace(original, @".*(\[font size\=\d{1,3}\]).*", "$1");
        //    }
        //    if (Regex.IsMatch(original, SelectMessageRegex))
        //    {
        //        string Data = Regex.Replace(original, SelectMessageRegex, "$1");
        //        Message.AppendLine(original.Replace(Data, fontSize + AddBracketsIfNeed(translation, original)));
        //    }
        //    else if (original.EndsWith("[r]"))
        //    {
        //        Message.AppendLine(fontSize + AddBracketsIfNeed(translation, original) + "[r]");
        //    }
        //    else if (original.EndsWith("[lr]"))
        //    {
        //        Message.AppendLine(fontSize + AddBracketsIfNeed(translation, original) + "[lr]");
        //    }
        //    else if (original.EndsWith("[phr]"))
        //    {
        //        Message.Append(fontSize + AddBracketsIfNeed(translation, original) + "[phr]");
        //    }
        //}

        //private static string AddBracketsIfNeed(string translation, string original)
        //{
        //    string ret = translation;
        //    if (original.Contains("[（]"))
        //    {
        //        ret = "[（]" + ret;
        //    }
        //    else if (original.Contains("[「]"))
        //    {
        //        ret = "[「]" + ret;
        //    }
        //    if (original.Contains("[）]"))
        //    {
        //        ret += "[）]";
        //    }
        //    else if (original.Contains("[」]"))
        //    {
        //        ret += "[」]";
        //    }
        //    return ret;
        //}
    }
}
