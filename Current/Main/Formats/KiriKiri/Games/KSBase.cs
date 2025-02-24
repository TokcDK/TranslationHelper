using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    abstract class KSBase : KiriKiriBase
    {
        // Constants
        protected const string WaitSymbol = "[待]";
        protected const string NewlineSymbol = "[落]";

        // Static readonly list of patterns for efficiency (created once and reused)
        private static readonly List<ParsePatternData> PatternsList = new List<ParsePatternData>
        {
            new ParsePatternData(@"^\*[^\|]+\|(.+)$", info: "label like *prologue1|プロローグ"),
            new ParsePatternData(@"\s*tf\.gameText\[[ 0-9]{1,5}\] \= \""([^\""]+)\""\;\s*(\/\/.+)?", info: "tf.gameText[: // tf.gameText[608] = \"トータルスコア : %d point\";"),
            new ParsePatternData(@"\.drawText\([^,]+, [^,]+, \""([^\""]+)\""[^,]*, [^\)]+\)", info: "drawText: //.drawText(730, 314, \"出入口\", 0xFFFFFF) , .drawText(15, 14, \"お名前（任意）\", clBtnText)"),
            new ParsePatternData(@"caption \= \""([^\""]{1,30})\""", info: "caption: //caption = \"text\""),
            new ParsePatternData(@"new KAGMenuItem\([^,]+, \""([^\""]+)\""[^,]*,", info: "KAGMenu:"),
            new ParsePatternData(@"new KAGMenuItem\([^,]+, '([^']+)'[^,]*,", info: "KAGMenuItem:"),
            new ParsePatternData(@"helpText\[[0-9]{1,5}\] \= \""([^\""]+)\""", info: "helpText:"),
            new ParsePatternData(@" emb\=\""([^\""]{1,260})\""", info: " emb=:"),
            new ParsePatternData(@"\[eval exp\=\""drawGpWindow\([^']*'([^']+)'[^\""]*\""\]", info: "drawGpWindow:"),
            new ParsePatternData(@"\[eval exp\=\""drawAnswer\('([^']+)',[^\""]*\""\]", info: "drawAnswer:"),
            new ParsePatternData(@"\[eval exp\=\""useSkillEffect\('([^']+)'", info: "useSkillEffect:"),
            new ParsePatternData(@"\""name\""\=>\""([^\""]+)\""", info: "\"name\"=>\":"),
            new ParsePatternData(@"\[[A-Za-z_]+ name \= \""([^\""\]]+)\""\]", info: "name:"),
            new ParsePatternData(@"\""explanation\""\=>\""([^\""]+)\""", info: "\"explanation\"=>\":"),
            new ParsePatternData(@"text \= \""([^\""]{1,260})\""", info: "text = \":"),
            new ParsePatternData(@"text2 \= \""([^\""]{1,260})\""", info: "text2 = \":"),
            new ParsePatternData(@"text \+\= \""([^\""]{1,260})\""", info: "text += \":"),
            new ParsePatternData(@"drawMessage\('fore', '([^']+)', '[^']*', '[^']*'\)", info: "drawMessage:"),
            new ParsePatternData(@"drawMessage\('fore', '[^']*', '([^']+)', '[^']*'\)", info: "drawMessage(:"),
            new ParsePatternData(@"drawMessage\('fore', '[^']*', '[^']*', '([^']+)'\)", info: "drawMessage(':"),
            new ParsePatternData(@"\.drawText\([^,]+, [^,]+, '([^']+)'", info: ".drawText:"),
            new ParsePatternData(@"\[emb exp\=\""'([^']+)'", info: "emb exp=\":"),
            new ParsePatternData(@"\[eval exp\=\""[^\=]+\='([^']+)'[^\""]*\""", info: "eval exp=\":"),
            new ParsePatternData(@"\[nowait\]([^(\[endnowait\])]+)\[endnowait\]", info: "[nowait]:"),
            new ParsePatternData(@"\[link [^\]]+\]([^\[]+)\[endlink\]", info: "link :"),
            new ParsePatternData(@"\[C_SELECT [^\]]+\]([^\[]+)\[endlink\]", info: "[C_SELECT :"),
            new ParsePatternData(@"\[選択肢 emb\=\""([^\""]+)\""", info: "[選択肢 emb=\":"),
            new ParsePatternData(@"'[^']+'\=\>'([^']+)'", info: "'=>':"),
            new ParsePatternData(@"\""[^\""']+\""", info: "\":"),
            new ParsePatternData(@"[^']+'", info: "':")
        };

        // Instance fields
        private readonly ProjectHideRestoreVarsInstance _projectHideRestoreVarsInstance = new ProjectHideRestoreVarsInstance(null);
        private bool _isScript;

        protected KSBase()
        {
        }

        /// <summary>
        /// Gets the file extension associated with this parser.
        /// </summary>
        public override string Extension => ".ks";

        /// <summary>
        /// Gets the list of parse patterns for script parsing.
        /// </summary>
        protected override List<ParsePatternData> Patterns() => PatternsList;

        /// <summary>
        /// Checks if the current line is empty or a comment.
        /// Handles multi-line comments and single-line comments starting with ';' or '//'.
        /// </summary>
        private bool IsEmptyOrComment()
        {
            // Handle multi-line comment start
            if (!ParseData.IsComment && ParseData.Line.Contains("/*"))
            {
                ParseData.IsComment = true;
            }

            // Handle multi-line comment end
            if (ParseData.IsComment && ParseData.Line.Contains("*/"))
            {
                ParseData.IsComment = false;
            }

            // Check for comments or empty lines
            return ParseData.IsComment ||
                   ParseData.TrimmedLine.Length == 0 ||
                   ParseData.TrimmedLine[0] == ';' ||
                   ParseData.TrimmedLine.StartsWith("//");
        }

        /// <summary>
        /// Checks if the current line begins or ends a script section.
        /// </summary>
        private bool IsScriptBegin()
        {
            if (_isScript)
            {
                if (ParseData.Line.Contains("endscript"))
                {
                    _isScript = false;
                }
                return false; // Allows processing of strings within script sections
            }
            else if (ParseData.Line.Contains("iscript"))
            {
                _isScript = true;
                return false; // Allows processing of strings within script sections
            }
            return false;
        }

        /// <summary>
        /// Parses a line from the script file and processes it according to the current mode.
        /// Handles translation and symbol fixing without altering functionality.
        /// </summary>
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (IsScriptBegin() || IsEmptyOrComment() || ParsePatterns())
            {
                // Skip script sections, empty lines, comments, or lines matching patterns
                SaveModeAddLine();
                return KeywordActionAfter.Continue;
            }

            // Check if the line should be processed
            if (!ShouldProcessLine(out bool endsWithWait))
            {
                SaveModeAddLine();
                return KeywordActionAfter.Continue;
            }

            // Process the line
            var originalLine = ParseData.Line;
            if (endsWithWait)
            {
                originalLine = originalLine.Remove(originalLine.Length - WaitSymbol.Length);
            }

            // Split the line into parts based on the newline symbol
            var parts = originalLine.Split(new[] { NewlineSymbol }, StringSplitOptions.None);
            var translatedParts = new List<string>();
            bool transApplied = false;

            foreach (var part in parts)
            {
                ProcessPart(part, translatedParts, ref transApplied);
            }

            // Reconstruct the line if translations were applied in save mode
            if (SaveFileMode && transApplied)
            {
                var translatedLine = ReconstructTranslatedLine(translatedParts, endsWithWait, originalLine);
                ParseData.Line = translatedLine;
                ParseData.Ret = true;
            }

            SaveModeAddLine();
            return KeywordActionAfter.Continue;
        }

        /// <summary>
        /// Determines if the line should be processed based on various conditions.
        /// </summary>
        private bool ShouldProcessLine(out bool endsWithWait)
        {
            endsWithWait = ParseData.TrimmedLine.EndsWith(WaitSymbol);
            return endsWithWait ||
                   EndsWithValidSymbol() ||
                   ContainsNoSpecSymbols() ||
                   ContainsCharsWhenTagsRemoved();
        }

        /// <summary>
        /// Processes a single part of the line, handling tags, variables, and translations.
        /// </summary>
        private void ProcessPart(string part, List<string> translatedParts, ref bool transApplied)
        {
            // Remove tags and clean variables for validity check
            var cleanedPart = CleanVars(CheckAndRemoveRubyText(part));

            if (IsValidString(cleanedPart))
            {
                if (OpenFileMode)
                {
                    // In open file mode, add the part to the data structure
                    AddRowData(part, string.Empty, isCheckInput: false);
                }
                else
                {
                    // In save file mode, attempt to set the translation
                    var translatedPart = part;
                    if (SetTranslation(ref translatedPart))
                    {
                        transApplied = true;
                        // Fix invalid symbols in the translated part
                        translatedPart = FixInvalidSymbols(translatedPart);
                    }
                    translatedParts.Add(translatedPart);
                }
            }
            else
            {
                // If the part is not valid, keep it as is
                translatedParts.Add(part);
                // Clear the variable hiding instance
                _projectHideRestoreVarsInstance.Clear();
            }
        }

        /// <summary>
        /// Reconstructs the translated line, applying character name corrections and wait symbols.
        /// </summary>
        private static string ReconstructTranslatedLine(List<string> translatedParts, bool endsWithWait, string originalLine)
        {
            // Join translated parts
            var translatedLine = string.Join(NewlineSymbol, translatedParts);
            if (endsWithWait)
            {
                translatedLine += WaitSymbol;
            }

            // Apply character name correction if applicable
            var onamematch = Regex.Match(originalLine, @"^【([^】]+)】.+$");
            if (onamematch.Success)
            {
                var tnamematch = Regex.Match(translatedLine, @"^-([^-]+)-(.+)$");
                if (tnamematch.Success)
                {
                    translatedLine = "【" + tnamematch.Groups[1].Value + "】" + tnamematch.Groups[2].Value;
                }
            }
            return translatedLine;
        }

        /// <summary>
        /// Checks if there are characters left after removing all tags.
        /// </summary>
        private bool ContainsCharsWhenTagsRemoved()
        {
            if (_isScript)
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
                var start = mcstart[i].Index;
                var endind = cleaned.IndexOfAny(new char[] { ']' }, start);
                if (endind == -1)
                {
                    continue;
                }
                var length = endind + 1 - start;
                cleaned = cleaned.Remove(start, length);
            }

            return cleaned.Length > 0 && IsValidString(cleaned);
        }

        /// <summary>
        /// Checks if the line contains special symbols.
        /// </summary>
        private bool ContainsNoSpecSymbols()
        {
            if (_isScript)
            {
                return false;
            }
            return !(ParseData.Line.Contains("[") ||
                     ParseData.Line.Contains("]") ||
                     ParseData.Line.Contains("@") ||
                     ParseData.Line.Contains("*"));
        }

        /// <summary>
        /// Cleans variables from the string for validity checking.
        /// </summary>
        internal string CleanVars(string str)
        {
            str = _projectHideRestoreVarsInstance.HideVARSBase(str);
            if (Regex.IsMatch(str, @"\[[a-z]{1,10}\]"))
            {
                return Regex.Replace(str, @"\[[a-z]{1,10}\]", "");
            }
            return str;
        }

        /// <summary>
        /// Fixes invalid symbols for Kirikiri engine compatibility.
        /// </summary>
        protected override string FixInvalidSymbols(string str)
        {
            // Hide variables to prevent replacements from affecting them
            str = _projectHideRestoreVarsInstance.HideVARSBase(str);

            // Replace Kirikiri control codes with temporary placeholders
            str = str.Replace("[r]", "{R}")
                     .Replace("[p]", "{P}")
                     .Replace("[lr]", "{LR}")
                     .Replace("[phr]", "{PHR}")
                     .Replace("[（]", "{BR01}")
                     .Replace("[）]", "{BR02}")
                     .Replace("[「]", "{BR11}")
                     .Replace("[」]", "{BR12}")
                     .Replace("[SYSTEM_MENU_ON]", "{SYSTEM_MENU_ON}");

            // Replace invalid characters
            str = str.Replace('[', '-')
                     .Replace(']', '-')
                     .Replace('(', '（')
                     .Replace(')', '）')
                     .Replace("'", "`")
                     .Replace("*", string.Empty)
                     .Replace(".", "。")
                     .Replace(",", "、");

            // Restore control codes
            str = str.Replace("{R}", "[r]")
                     .Replace("{P}", "[p]")
                     .Replace("{LR}", "[lr]")
                     .Replace("{PHR}", "[phr]")
                     .Replace("{BR01}", "[（]")
                     .Replace("{BR02}", "[）]")
                     .Replace("{BR11}", "[「]")
                     .Replace("{BR12}", "[」]")
                     .Replace("{SYSTEM_MENU_ON}", "[SYSTEM_MENU_ON]");

            // Handle quotes carefully to avoid escaping issues
            str = str.Replace(@"\""", "{Q}") // Temporarily replace escaped quotes
                     .Replace("\"", "`") // Replace other quotes
                     .Replace("{Q}", @"\"""); // Restore escaped quotes

            // Restore variables
            return _projectHideRestoreVarsInstance.RestoreVARS(str);
        }

        /// <summary>
        /// Checks if the line ends with a valid symbol for processing.
        /// </summary>
        private bool EndsWithValidSymbol()
        {
            var validSymbols = new[]
            {
            "。",
            "[」]",
            "」",
            "！",
            "[r]",
            "[p]",
            "[lr]",
            "[phr]",
            "[phr][resetfont]",
            "[lr][resetfont]",
            "[r][resetfont]",
            "[p][resetfont]",
            "[SYSTEM_MENU_ON]"
        };

            foreach (var s in validSymbols)
            {
                if (ParseData.TrimmedLine.Length > s.Length &&
                    ParseData.TrimmedLine.EndsWith(s) &&
                    (ParseData.TrimmedLine.IndexOf("//") == -1 ||
                     ParseData.TrimmedLine.IndexOf("//") > ParseData.TrimmedLine.IndexOf(s)) &&
                    !ContainsNoPatterns(ParseData.TrimmedLine))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the line matches any defined patterns.
        /// </summary>
        private bool ContainsNoPatterns(string str)
        {
            return Patterns().Any(p => Regex.IsMatch(str, p.Pattern));
        }

        private readonly string RubyTextRegex = "\\[ruby text\\=\\\"([^\\\"]*)\\\"\\]";

        /// <summary>
        /// Removes ruby text tags and font tags from the input string.
        /// </summary>
        protected string CheckAndRemoveRubyText(string inputString)
        {
            string ret = inputString;
            if (inputString.Contains("[ruby text="))
            {
                ret = Regex.Replace(ret, RubyTextRegex, "$1");
            }
            if (inputString.Contains("font"))
            {
                ret = Regex.Replace(ret, @"\[font size\=\d+\]", string.Empty);
                ret = Regex.Replace(ret, @"\[font color\=0x[0-9A-F]{6} bold\=(true|false)\]", string.Empty);
                ret = Regex.Replace(ret, @"\[font [^\]]+\]", string.Empty);
                ret = ret.Replace("[resetfont]", string.Empty);
            }
            return ret;
        }

        // Note: ParseStringFileLineNew is not used in the current implementation and has been omitted.
        // If required in the future, it should be refactored similarly.
    }
}
