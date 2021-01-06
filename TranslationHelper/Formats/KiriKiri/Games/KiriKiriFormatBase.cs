using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    abstract class KiriKiriFormatBase : FormatBase
    {
        protected KiriKiriFormatBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected Encoding encoding = Encoding.Unicode;
        /// <summary>
        /// encoding for read write file
        /// </summary>
        /// <returns></returns>
        internal virtual Encoding FileEncoding()
        {
            //using (var fs = new FileStream(thDataWork.FilePath, FileMode.Open, FileAccess.Read))
            //    return FunctionsFileFolder.GetEncoding(fs);
            return FunctionsFileFolder.GetEncoding(thDataWork.FilePath);
            //return FunctionsFileFolder.GetEncoding(thDataWork.FilePath);
            //if (Enc == null && !string.IsNullOrEmpty(thDataWork.FilePath))
            //{
            //    Enc = FunctionsFileFolder.GetEncoding(thDataWork.FilePath);
            //}
            //else
            //{
            //    Enc = Encoding.Unicode;
            //    //new UTF8Encoding(false) //UTF8 with no BOM
            //}
            //return Encoding.Unicode;
            //return Enc;
            //return new UTF8Encoding(false);
        }

        protected const string PatchDirName = "_patch";

        internal override bool Open()
        {
            return ParseStringFile();
        }

        internal override bool Save()
        {
            return ParseStringFile();
        }

        protected const string waitSymbol = "[待]";
        protected const string newlineSymbol = "[落]";
        protected override Dictionary<string, string> Patterns()
        {
            return new Dictionary<string, string>()
           {
            { "*", //*prologue1|プロローグ
                @"^\*[^\|]+\|(.+)$" },
            { "tf.gameText[", //	tf.gameText[608] = "トータルスコア : %d point";
                @"\s*tf\.gameText\[[ 0-9]{1,5}\] \= \""([^\""]+)\""\;\s*(\/\/.+)?" },
            { "drawText", //.drawText(730, 314, "出入口", 0xFFFFFF) , .drawText(15, 14, "お名前（任意）", clBtnText)
                @"\.drawText\([^,]+, [^,]+, \""([^\""]+)\""[^,]*, [^\)]+\)" },
            { "caption", //caption = "text"
                @"caption \= \""([^\""]{1,30})\""" },
            { "KAGMenu",
                @"new KAGMenuItem\([^,]+, \""([^\""]+)\""[^,]*," },
            { "KAGMenuItem",
                @"new KAGMenuItem\([^,]+, '([^']+)'[^,]*," },
            { "helpText",
                @"helpText\[[0-9]{1,5}\] \= \""([^\""]+)\""" },
            { " emb=",
                @" emb\=\""([^\""]{1,260})\""" },
            { "drawGpWindow",
                @"\[eval exp\=\""drawGpWindow\([^']*'([^']+)'[^\""]*\""\]" },
            { "drawAnswer",
                @"\[eval exp\=\""drawAnswer\('([^']+)',[^\""]*\""\]" },
            { "useSkillEffect",
                @"\[eval exp\=\""useSkillEffect\('([^']+)'" },
            { "\"name\"=>\"",
                @"\""name\""\=>\""([^\""]+)\""" },
            { "\"explanation\"=>\"",
                @"\""explanation\""\=>\""([^\""]+)\""" },
            { "text = \"",
                @"text \= \""([^\""]{1,260})\""" },
            { "text2 = \"",
                @"text2 \= \""([^\""]{1,260})\""" },
            { "text += \"",
                @"text \+\= \""([^\""]{1,260})\""" },
            { "drawMessage",
                @"drawMessage\('fore', '([^']+)', '[^']*', '[^']*'\)" },
            { "drawMessage(",
                @"drawMessage\('fore', '[^']*', '([^']+)', '[^']*'\)" },
            { "drawMessage('",
                @"drawMessage\('fore', '[^']*', '[^']*', '([^']+)'\)" },
            { "emb exp=\"",
                @"\[emb exp\=\""'([^']+)'" },
            { "eval exp=\"",
                @"\[eval exp\=\""[^\=]+\='([^']+)'[^\""]*\""" },
            { "[nowait]",
                @"\[nowait\]([^(\[endnowait\])]+)\[endnowait\]" },
            { "link storage=",
                @"\[link storage\=\""[^\""]+\"" target\=""\*[^\""]+\"" clickse\=\""[^\""]+\"" clicksebuf\=[0-9]{1,5}\]([^\[]+)\[endlink\]" },
            { "[選択肢 emb=\"",
                @"\[選択肢 emb\=\""[^\""]+\""" },
            { "「",
                @"^「.+」$" },
            { "",
                @"^.+\[SYSTEM_MENU_ON\]$" }
           };
        }
        protected bool OpenSaveKS(bool OpenKS = true)
        {
            InitData(OpenKS);

            try
            {
                if (thDataWork.OpenFileMode)
                {
                    AddTables(ParseData.tablename);
                }

                //using (var reader = new StreamReader(thDataWork.FilePath, true /*FileEncoding()*/))
                using (var reader = new StreamReader(thDataWork.FilePath, FileEncoding()))
                {
                    encoding = reader.CurrentEncoding;
                    while ((ParseData.line = reader.ReadLine()) != null)
                    {
                        if (!IsEmptyOrComment())
                        {
                            //CheckAndParseText();
                        }

                        SaveModeAddLine();
                    }
                }

                FinalTableCheckORWriteFile();
            }
            catch
            {

            }

            return ParseData.Ret;
        }

        private void InitData(bool OpenKS)
        {
            thDataWork.OpenFileMode = OpenKS;
            ParseData = new ParseFileData(thDataWork)
            {
                IsComment = false,
            };
        }

        private void FinalTableCheckORWriteFile()
        {
            if (thDataWork.OpenFileMode)
            {
                ParseData.Ret = CheckTablesContent(ParseData.tablename);
            }
            else
            {
                try
                {
                    if (ParseData.Ret)
                    {
                        File.WriteAllText(Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchDirName, ParseData.tablename), ParseData.ResultForWrite.ToString(), encoding /*FileEncoding()*/);
                    }
                }
                catch
                {
                    ParseData.Ret = false;
                }
            }
        }

        private void SaveModeAddLine()
        {
            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }
        }

        private bool IsEmptyOrComment()
        {
            if (!ParseData.IsComment && ParseData.line.Contains("/*"))
            {
                ParseData.IsComment = true;
            }
            if (ParseData.IsComment && ParseData.line.Contains("*/"))
            {
                ParseData.IsComment = false;
            }

            return ParseData.IsComment || (ParseData.TrimmedLine = ParseData.line).Length == 0 || (ParseData.TrimmedLine.Length > 0 && ParseData.TrimmedLine[0] == ';') || ParseData.TrimmedLine.StartsWith("//");
        }

        protected override void ParseStringFilePreOpenExtra()
        {
            ParseData.IsComment = false;
        }

        bool endsWithWait;
        protected override int ParseStringFileLine()
        {
            var ret = 0;

            if (!IsEmptyOrComment() && !ParsePatterns() && 
                ((endsWithWait = ParseData.TrimmedLine.EndsWith(waitSymbol)) || EndsWithValidSymbol())
               )
            {

                bool transApplied = false;
                var strarr = ParseData.line.Split(new[] { newlineSymbol }, System.StringSplitOptions.None);
                var strarrLength = strarr.Length;
                for (int i = 0; i < strarrLength; i++)
                {
                    var str = strarr[i];
                    if (endsWithWait && i == strarrLength - 1)
                    {
                        if ((str = str.Remove(str.IndexOf(waitSymbol))).Trim().Length == 0)
                        {
                            break;
                        }
                    }

                    str = CheckAndRemoveRubyText(str);

                    //clean string from vars for checking
                    var CleanedStr = CleanVars(str);

                    if (IsValidString(CleanedStr))
                    {
                        if (thDataWork.OpenFileMode)
                        {
                            AddRowData(str, string.Empty, true, false);
                        }
                        else
                        {
                            if (CheckAndSetTranslation(ref str))
                            {
                                if (!ParseData.Ret)
                                {
                                    ParseData.Ret = true;
                                }
                                if (!transApplied)
                                {
                                    transApplied = true;
                                }

                                str = FixInvalidSymbols(str);//set fixes

                                strarr[i] = str;
                            }
                        }
                    }
                    else
                    {
                        thDataWork.CurrentProject.HideVARSMatchCollectionsList?.Clear();//clear list of matches for hidevarbase
                    }
                }
                if (thDataWork.SaveFileMode && transApplied && ParseData.Ret)
                {
                    var s = string.Join(newlineSymbol, strarr);
                    ParseData.line = s + (endsWithWait && !s.EndsWith(waitSymbol) ? waitSymbol : string.Empty);
                }
            }

            SaveModeAddLine();

            return ret;
        }
        protected override string GetFilePath()
        {
            //write translated files to patch dir
            return Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchDirName, Path.GetFileName(thDataWork.FilePath));
        }

        internal string CleanVars(string str)
        {
            var keyfound = false;
            foreach (var key in thDataWork.CurrentProject.HideVarsBase.Keys)
            {
                if (str.Contains(key))
                {
                    keyfound = true;
                    break;
                }
            }
            if (!keyfound)
            {
                return str;
            }

            var mc = Regex.Matches(str, "(" + string.Join(")|(", thDataWork.CurrentProject.HideVarsBase.Values) + ")");
            if (mc.Count == 0)
            {
                return str;
            }

            for (int m = mc.Count - 1; m >= 0; m--)
            {
                try
                {
                    str = str.Remove(mc[m].Index, mc[m].Value.Length);
                }
                catch (System.ArgumentOutOfRangeException)
                {

                }
            }

            if (Regex.IsMatch(str, @"\[[a-z]{1,10}\]"))
            {
                return Regex.Replace(str
                    //.Replace("[phr]", "")
                    //.Replace("[hr]", "")
                    //.Replace("[lr]", "")
                    //.Replace("[r]", "")
                    //.Replace("[p]", "")
                    , @"\[[a-z]{1,10}\]", "");
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// remove invalid for kirikiri symbols or replace them to some valid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected override string FixInvalidSymbols(string str)
        {
            return thDataWork.CurrentProject.RestoreVARS(
                thDataWork.CurrentProject.HideVARSBase(str)
                .Replace("[r]", "{R}")
                .Replace("[p]", "{P}")
                .Replace("[lr]", "{LR}")
                .Replace("[phr]", "{PHR}")
                .Replace("[（]", "{BR01}")
                .Replace("[）]", "{BR02}")
                .Replace("[「]", "{BR11}")
                .Replace("[」]", "{BR12}")
                .Replace('[', '-')
                .Replace(']', '-')
                .Replace('(', '（')
                .Replace(')', '）')
                .Replace("'", "`")
                .Replace("*", string.Empty)
                .Replace(".", "。")
                .Replace(",", "、")
                .Replace("{R}", "[r]")
                .Replace("{P}", "[p]")
                .Replace("{LR}", "[lr]")
                .Replace("{PHR}", "[phr]")
                .Replace("{BR01}", "[（]")
                .Replace("{BR02}", "[）]")
                .Replace("{BR11}", "[「]")
                .Replace("{BR12}", "[」]")
                .Replace(@"\""", "{Q}")//preremove quotes of variables
                .Replace("\"", "`")//replace other quotes
                .Replace("{Q}", @"\""")//return quotes of variables
                                       //.Replace("0", "０")
                                       //.Replace("1", "１")
                                       //.Replace("2", "２")
                                       //.Replace("3", "３")
                                       //.Replace("4", "４")
                                       //.Replace("5", "５")
                                       //.Replace("6", "６")
                                       //.Replace("7", "７")
                                       //.Replace("8", "８")
                                       //.Replace("9", "９")
                );
        }

        private bool EndsWithValidSymbol()
        {
            foreach (var s in new[]{
                "。" ,
                "[」]" ,
                "」" ,
                "！" ,
                "[r]" ,
                "[p]" ,
                "[lr]" ,
                "[phr]",
                "[phr][resetfont]",
                "[lr][resetfont]",
                "[r][resetfont]",
                "[p][resetfont]"
            })
            {
                if (ParseData.TrimmedLine.Length > s.Length && ParseData.TrimmedLine.EndsWith(s) && (ParseData.TrimmedLine.IndexOf("//") == -1 || ParseData.TrimmedLine.IndexOf("//") > ParseData.TrimmedLine.IndexOf(s)) && !ContainsNoPatterns(ParseData.TrimmedLine))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ContainsNoPatterns(string str)
        {
            foreach (var p in Patterns())
            {
                if (str.Contains(p.Key) && Regex.IsMatch(str, p.Value))
                {
                    return true;
                }
            }
            return false;
        }

        protected readonly string RubyTextRegex = "\\[ruby text\\=\\\"([^\\\"]*)\\\"\\]";
        /// <summary>
        /// remove 'ruby text=' tags and font tags
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        protected string CheckAndRemoveRubyText(string inputString)
        {
            string ret = inputString;
            if (inputString.Contains("[ruby text="))
            {
                ret = Regex.Replace(ret, RubyTextRegex, "$1");
            }
            if (inputString.Contains("font"))
            {
                //[font size=36]
                if (Regex.IsMatch(ret, @"\[font size\=\d+\]"))
                    ret = Regex.Replace(ret, @"\[font size\=\d+\]", string.Empty);

                //[font color=0xFFCC00 bold=true]
                if (Regex.IsMatch(ret, @"\[font color\=0x[0-9A-F]{6} bold\=(true|false)\]"))
                    ret = Regex.Replace(ret, @"\[font color\=0x[0-9A-F]{6} bold\=(true|false)\]", string.Empty);

                if (Regex.IsMatch(ret, @"\[font [^\]]+\]"))
                    ret = Regex.Replace(ret, @"\[font [^\]]+\]", string.Empty);

                if (ret.Contains("[resetfont]"))
                    ret = ret.Replace("[resetfont]", string.Empty);
            }

            return ret;
        }
    }
}
