﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    abstract class KSBase : KiriKiriBase
    {
        protected KSBase()
        {
        }

        public override string Extension => ".ks";

        protected const string waitSymbol = "[待]";
        protected const string newlineSymbol = "[落]";
        protected override List<ParsePatternData> Patterns()
        {
            return new List<ParsePatternData>()
                   {
                     new ParsePatternData( @"^\*[^\|]+\|(.+)$", info: "label like *prologue1|プロローグ" ),
                     new ParsePatternData( @"\s*tf\.gameText\[[ 0-9]{1,5}\] \= \""([^\""]+)\""\;\s*(\/\/.+)?", info: "tf.gameText[: //	tf.gameText[608] = \"トータルスコア : %d point\";" ),
                     new ParsePatternData( @"\.drawText\([^,]+, [^,]+, \""([^\""]+)\""[^,]*, [^\)]+\)", info: "drawText: //.drawText(730, 314, \"出入口\", 0xFFFFFF) , .drawText(15, 14, \"お名前（任意）\", clBtnText)" ),
                     new ParsePatternData( @"caption \= \""([^\""]{1,30})\""", info: "caption: //caption = \"text\"" ),
                     new ParsePatternData( @"new KAGMenuItem\([^,]+, \""([^\""]+)\""[^,]*,", info: "KAGMenu:" ),
                     new ParsePatternData( @"new KAGMenuItem\([^,]+, '([^']+)'[^,]*,", info: "KAGMenuItem:" ),
                     new ParsePatternData( @"helpText\[[0-9]{1,5}\] \= \""([^\""]+)\""", info: "helpText:" ),
                     new ParsePatternData( @" emb\=\""([^\""]{1,260})\""", info: " emb=:" ),
                     new ParsePatternData( @"\[eval exp\=\""drawGpWindow\([^']*'([^']+)'[^\""]*\""\]", info: "drawGpWindow:" ),
                     new ParsePatternData( @"\[eval exp\=\""drawAnswer\('([^']+)',[^\""]*\""\]", info: "drawAnswer:" ),
                     new ParsePatternData( @"\[eval exp\=\""useSkillEffect\('([^']+)'", info: "useSkillEffect:" ),
                     new ParsePatternData( @"\""name\""\=>\""([^\""]+)\""", info: "\"name\"=>\":" ),
                     new ParsePatternData( @"\[[A-Za-z_]+ name \= \""([^\""\]]+)\""\]", info: "name:" ),
                     new ParsePatternData( @"\""explanation\""\=>\""([^\""]+)\""", info: "\"explanation\"=>\":" ),
                     new ParsePatternData( @"text \= \""([^\""]{1,260})\""", info: "text = \":" ),
                     new ParsePatternData( @"text2 \= \""([^\""]{1,260})\""", info: "text2 = \":" ),
                     new ParsePatternData( @"text \+\= \""([^\""]{1,260})\""", info: "text += \":" ),
                     new ParsePatternData( @"drawMessage\('fore', '([^']+)', '[^']*', '[^']*'\)", info: "drawMessage:" ),
                     new ParsePatternData( @"drawMessage\('fore', '[^']*', '([^']+)', '[^']*'\)", info: "drawMessage(:" ),
                     new ParsePatternData( @"drawMessage\('fore', '[^']*', '[^']*', '([^']+)'\)", info: "drawMessage(':" ),
                     new ParsePatternData( @"\.drawText\([^,]+, [^,]+, '([^']+)'", info: ".drawText:" ),
                     new ParsePatternData( @"\[emb exp\=\""'([^']+)'", info: "emb exp=\":" ),
                     new ParsePatternData( @"\[eval exp\=\""[^\=]+\='([^']+)'[^\""]*\""", info: "eval exp=\":" ),
                     new ParsePatternData( @"\[nowait\]([^(\[endnowait\])]+)\[endnowait\]", info: "[nowait]:" ),
                     new ParsePatternData( @"\[link [^\]]+\]([^\[]+)\[endlink\]", info: "link :" ),
                     new ParsePatternData( @"\[C_SELECT [^\]]+\]([^\[]+)\[endlink\]", info: "[C_SELECT :" ),
                     new ParsePatternData( @"\[選択肢 emb\=\""([^\""]+)\""", info: "[選択肢 emb=\":" ),
                     new ParsePatternData( @"'[^']+'\=\>'([^']+)'", info: "'=>':" ),
                     new ParsePatternData( @"\""[^\""']+\""", info: "\":" ),
                     new ParsePatternData( @"'[^']+'", info: "':" ),
                   };
        }
        //protected bool OpenSaveKS(bool OpenKS = true)
        //{
        //    InitData(OpenKS);

        //    try
        //    {
        //        if (ProjectData.OpenFileMode)
        //        {
        //            AddTables(ParseData.tablename);
        //        }

        //        //using (var reader = new StreamReader(FilePath, true /*FileEncoding()*/))
        //        using (var reader = new StreamReader(FilePath, FileEncoding()))
        //        {
        //            encoding = reader.CurrentEncoding;
        //            while ((ParseData.line = reader.ReadLine()) != null)
        //            {
        //                if (!IsEmptyOrComment())
        //                {
        //                    //CheckAndParseText();
        //                }

        //                SaveModeAddLine();
        //            }
        //        }

        //        FinalTableCheckORWriteFile();
        //    }
        //    catch
        //    {

        //    }

        //    return ParseData.Ret;
        //}

        //private void InitData(bool OpenKS)
        //{
        //    ProjectData.OpenFileMode = OpenKS;
        //    ParseData = new ParseFileData()
        //    {
        //        IsComment = false,
        //    };
        //}

        //private void FinalTableCheckORWriteFile()
        //{
        //    if (ProjectData.OpenFileMode)
        //    {
        //        ParseData.Ret = CheckTablesContent(ParseData.tablename);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            if (ParseData.Ret)
        //            {
        //                File.WriteAllText(Path.Combine(ProjectData.CurrentProject.ProjectWorkDir, PatchDirName, ParseData.tablename), ParseData.ResultForWrite.ToString(), encoding /*FileEncoding()*/);
        //            }
        //        }
        //        catch
        //        {
        //            ParseData.Ret = false;
        //        }
        //    }
        //}

        private bool IsEmptyOrComment()
        {
            if (!ParseData.IsComment && ParseData.Line.Contains("/*"))
            {
                ParseData.IsComment = true;
            }
            if (ParseData.IsComment && ParseData.Line.Contains("*/"))
            {
                ParseData.IsComment = false;
            }

            return ParseData.IsComment || ParseData.TrimmedLine.Length == 0 || ParseData.TrimmedLine[0] == ';' || ParseData.TrimmedLine.StartsWith("//");
        }

        bool isscript = false;
        protected int ParseStringFileLineNew()
        {
            if (isscript)
            {
                //skip script content and..
                if (ParseData.TrimmedLine.EndsWith("@endscript"))//check for end of script
                {
                    isscript = false;//end of script
                }
            }
            else if (ParseData.TrimmedLine.StartsWith(";"))
            {
                //comment
            }
            else if (ParseData.TrimmedLine.StartsWith("*"))
            {
                //label
            }
            else if (ParseData.TrimmedLine.StartsWith("@iscript"))
            {
                //@iscript
                //@endscript
                isscript = true;//start to skip the script content
            }
            else //parse tags
            {
                //tag patterns
                //starts:(?<!\[)\[\s*\w+
                //ends:\]
                //starts:\t*@\s*\w+
                //ends:(?<!\\)\n
                ////tag attributes
                ////match:(\w+)(\s*=\s*(&?%?(".*?"|\'.*?\'|[^\\s\]=]+)))?
                ////


                //name pattern
                //starts: '^\t*【'
                //ends: '】'

                //1. search tag start
                //2. search tag end
                //3. parse tag attributes in the tag
                var tagstartcollection = Regex.Matches(ParseData.Line, @"(?<!\[)\[\s*\w+");
                if (tagstartcollection.Count > 0)
                {
                    var tagendcollection = Regex.Matches(ParseData.Line, @"\]");

                    var endind = 0;
                    foreach (Match tagstart in tagstartcollection)
                    {
                        var start = tagstart.Index + tagstart.Length;
                        var searchstring = ParseData.Line.Substring(start, tagendcollection[endind].Index - start);

                        var attribs = Regex.Matches(searchstring, @"(\w+)(\s*=\s*(&?%?("".*? ""|\'.*?\'|[^\\s\]=]+)))?");

                        foreach (Match attrib in attribs)
                        {

                        }
                    }
                }
                else
                {
                    tagstartcollection = Regex.Matches(ParseData.Line, @"\t*@\s*\w+");
                    if (tagstartcollection.Count > 0)
                    {
                        var searchstring = ParseData.Line.Substring(tagstartcollection[0].Index + tagstartcollection[0].Length);

                        var attribs = Regex.Matches(searchstring, @"(\w+)(\s*=\s*(&?%?("".*? ""|\'.*?\'|[^\\s\]=]+)))?");

                        foreach (Match attrib in attribs)
                        {

                        }
                    }
                }
            }

            return 0;
        }

        bool endsWithWait;
        protected override KeywordActionAfter ParseStringFileLine()
        {
            var ret = KeywordActionAfter.Continue;

            if (!IsScriptBegin() && !IsEmptyOrComment() && !ParsePatterns() &&
                ((endsWithWait = ParseData.TrimmedLine.EndsWith(waitSymbol)) || EndsWithValidSymbol() || ContainsNoSpecSymbols() || ContainsCharsWhenTagsRemoved())
               )
            {

                bool transApplied = false;
                var strarr = ParseData.Line.Split(new[] { newlineSymbol }, System.StringSplitOptions.None);
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
                        if (OpenFileMode)
                        {
                            AddRowData(str, string.Empty, isCheckInput: false);
                        }
                        else
                        {
                            if (SetTranslation(ref str))
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
                        AppData.CurrentProject.HideVARSMatchCollectionsList?.Clear();//clear list of matches for hidevarbase
                    }
                }
                if (SaveFileMode && transApplied && ParseData.Ret)
                {
                    //character name correction
                    var s = string.Join(newlineSymbol, strarr);
                    var onamematch = Regex.Match(ParseData.Line, @"^【([^】]+)】.+$");
                    if (onamematch.Success)
                    {
                        var tnamematch = Regex.Match(s, @"^-([^-]+)-(.+)$");
                        if (tnamematch.Success)
                        {
                            s = "【" + tnamematch.Result("$1") + "】" + tnamematch.Result("$2");
                        }
                    }

                    ParseData.Line = s + (endsWithWait && !s.EndsWith(waitSymbol) ? waitSymbol : string.Empty);
                }
            }

            SaveModeAddLine();

            return ret;
        }

        /// <summary>
        /// true when after all tags was removed and still will be text
        /// </summary>
        /// <returns></returns>
        private bool ContainsCharsWhenTagsRemoved()
        {
            if (IsScript)
            {
                return false;
            }

            var cleaned = ParseData.Line;
            var mcstart = Regex.Matches(cleaned, @"(?<!\[)\[\s*\w+");
            if (mcstart.Count == 0)
            {
                return false;
            }
            for (int i = mcstart.Count - 1; i >= 0; i--)
            {
                //remove tag content line [lhr] or [eval name = "sss"]
                var start = mcstart[i].Index;
                var endind = cleaned.IndexOfAny(new char[] { ']' }, start);
                if (endind == -1)
                {
                    continue;
                }
                var length = endind + 1 - start;
                cleaned = cleaned.Remove(start, length);
            }

            if (cleaned.Length > 0 && IsValidString(cleaned))
            {
                return true;
            }

            return false;
        }

        bool IsScript = false;
        /// <summary>
        /// script area check
        /// </summary>
        /// <returns></returns>
        private bool IsScriptBegin()
        {
            if (IsScript)
            {
                if (ParseData.Line.Contains("endscript"))
                {
                    IsScript = false;
                }

                //return true;
                return false;//must be true but then will be skipped some strings
            }
            else
            {
                if (ParseData.Line.Contains("iscript"))
                {
                    IsScript = true;
                    //return true;
                    return false;//must be true but then will be skipped some strings
                }
            }
            return false;
        }

        private bool ContainsNoSpecSymbols()
        {
            if (IsScript)
            {
                return false;
            }

            return !(ParseData.Line.Contains("[") || ParseData.Line.Contains("]") || ParseData.Line.Contains("@") || ParseData.Line.Contains("*"));
        }

        internal string CleanVars(string str)
        {
            var keyfound = false;
            foreach (var key in AppData.CurrentProject.HideVarsBase.Keys)
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

            var mc = Regex.Matches(str, "(" + string.Join(")|(", AppData.CurrentProject.HideVarsBase.Values) + ")");
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
            return AppData.CurrentProject.RestoreVARS(
                AppData.CurrentProject.HideVARSBase(str)
                .Replace("[r]", "{R}")
                .Replace("[p]", "{P}")
                .Replace("[lr]", "{LR}")
                .Replace("[phr]", "{PHR}")
                .Replace("[（]", "{BR01}")
                .Replace("[）]", "{BR02}")
                .Replace("[「]", "{BR11}")
                .Replace("[」]", "{BR12}")
                .Replace("[SYSTEM_MENU_ON]", "{SYSTEM_MENU_ON}")
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
                .Replace("{SYSTEM_MENU_ON}", "[SYSTEM_MENU_ON]")
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
                "[p][resetfont]",
                "[SYSTEM_MENU_ON]"
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
            return Patterns().Any(p => Regex.IsMatch(str, p.Pattern));
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
