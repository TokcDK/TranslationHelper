using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RPGMakerVXRVData2Assistant;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Main.Functions;
using static System.Net.Mime.MediaTypeNames;

namespace TranslationHelper.Formats.RPGMakerVX.RVData2
{
    internal class RVDATA2 : FormatBinaryBase
    {
        public override string Extension => ".rvdata2";
        bool _isScripts = false;

        object _parser;
        byte[] _bytes;
        readonly Regex _quoteCaptureRegex = new Regex(AppMethods.GetRegexQuotesCapturePattern(), RegexOptions.Compiled);
        readonly Regex _commentaryCaptureRegex = new Regex("[ \t]*#[^{][^\r\n]+", RegexOptions.Compiled);
        readonly Regex _commentaryKeyNameCaptureRegex = new Regex(@"%COMMENT[0-9]+%", RegexOptions.Compiled);
        readonly Regex _variableCaptureRegex = new Regex("((#{[^}]+})|(#[a-zA-Z0-9]+))", RegexOptions.Compiled);
        readonly Regex _variableKeyNameCaptureRegex = new Regex(@"%VAR[0-9]+%", RegexOptions.Compiled);
        readonly Regex _variableRegexCaptureRegex = new Regex(@"[A-Za-z0-9_]+\s*=\s*/(?:[^/]+)/i", RegexOptions.Compiled);
        readonly Regex _variableRegexKeyNameCaptureRegex = new Regex(@"%REGVAR[0-9]+%", RegexOptions.Compiled);
        readonly Regex _mapNameCheckRegex = new Regex("[Mm]ap[0-9]{3}", RegexOptions.Compiled);

        protected override void FileOpen()
        {
            FileOpen(null);
        }

        public void FileOpen(byte[] fileBytes)
        {
            var name = Path.GetFileNameWithoutExtension(FilePath);
            _isScripts = string.Equals(name, "scripts", StringComparison.InvariantCultureIgnoreCase);

            //bool isBytes = fileBytes != null && fileBytes.Length > 0;

            if (_isScripts)
            {
                _parser = new ScriptsParser(FilePath, fileBytes);

                foreach (var script in (_parser as ScriptsParser).EnumerateRMScripts())
                {
                    ParseScript(script);
                }
            }
            else
            {
                _parser = new Parser(FilePath);

                var isEventCommandsUser = string.Equals(name, "CommonEvents", StringComparison.InvariantCultureIgnoreCase)
                    || _mapNameCheckRegex.IsMatch(name);

                foreach (var stringData in (_parser as Parser).EnumerateStrings())
                {
                    string s = stringData.Text;
                    var info = stringData.Info.ToString();
                    if (isEventCommandsUser && IsSkipCode(info)) continue;

                    if (AddRowData(ref s, info) && SaveFileMode) stringData.Text = s;
                }
            }
        }

        private void ParseScript(ScriptsParser.Script script)
        {
            // parse all strings inside quotes in script content

            if (string.IsNullOrEmpty(script.Text)) return;
            if (script.Text.Contains("def self.バトルステータス更新"))
            {
            }

            var scriptTextNoVarsNoComments = new StringBuilder(script.Text);

            // capture also variables like #{text}
            // need for fix false capture for quotes like #{Convert_Text.button_to_icon("マルチ")}

            var variablesCoordinates = HideVariables(script.Text, _variableCaptureRegex, scriptTextNoVarsNoComments, "%VAR", "%");

            // need for fix false capture commented quoted text
            var commentsCoordinates = HideVariables(scriptTextNoVarsNoComments.ToString(), _commentaryCaptureRegex, scriptTextNoVarsNoComments, "%COMMENT", "%");
            
            // need for fix false capture quotes inside of regex value of the variables
            // like comments they are outside of required quoted strings
            var variablesRegexCoordinates = HideVariables(scriptTextNoVarsNoComments.ToString(), _variableRegexCaptureRegex, scriptTextNoVarsNoComments, "%COMMENT", "%");

            var scriptText = scriptTextNoVarsNoComments.ToString();

            // capture quoted strings itself
            var mc = _quoteCaptureRegex.Matches(scriptText);

            var mcCount = mc.Count;
            if (mcCount == 0) return;

            bool isChanged = false;

            var scriptContentToChange = SaveFileMode ? new StringBuilder(scriptText) : null;
            // negative order because length of content is changing
            for (int i = mcCount - 1; i >= 0; i--)
            {
                var m = mc[i];
                string s = m.Groups[1].Value;

                s = RestoreStrings(s, variablesCoordinates, _variableKeyNameCaptureRegex);

                if (AddRowData(ref s, $"Script: {script.Title}") && SaveFileMode)
                {
                    s = EscapeQuotes(s);

                    // здесь пересчитываем длину оригинальной строки на переведенную, потом пересчитываем начальные координаты для комментария и вставляем его обратно
                    // рассчитываем как координаты для переменных, так и координаты для комментариев
                    isChanged = true;
                    scriptContentToChange
                        .Remove(m.Index, m.Length)
                        .Insert(m.Index, "\"" + s + "\"");

                }
            }

            if (isChanged && SaveFileMode)
            {
                RestoreVarsComments(scriptContentToChange, new Dictionary<string, string>[3] 
                { 
                    commentsCoordinates,
                    variablesRegexCoordinates,
                    variablesCoordinates, 
                });

                script.Text = scriptContentToChange.ToString();
            }
        }

        private void RestoreVarsComments(StringBuilder scriptContentToChange, Dictionary<string, string>[] keyStringPairsList)
        {
            // replace all comment names back to comments
            // restore variables in case of if some was not replaced earlier

            foreach (var keyStringPairs in keyStringPairsList)
            {
                foreach (var keyStringPair in keyStringPairs)
                {
                    scriptContentToChange.Replace(keyStringPair.Key, keyStringPair.Value);
                }
            }
        }

        private string EscapeQuotes(string s)
        {
            if (s.Contains("\"") && _variableCaptureRegex.IsMatch(s))
            {
                // здесь заменить переменные на ключи передтем, как эскейпить строку, потому, что могут встречаться переменные содержащие кавычки, которые нельзя эскейпить, вроде #{Convert_Text.button_to_icon(\"決定\",false)}

                var sb = new StringBuilder(s);
                var stringHidenVars = HideVariables(s, _variableCaptureRegex, sb, "%VAR", "%");

                string s1 = sb.ToString().EscapeQuotes(); // escape quotes in string

                return RestoreStrings(s1, stringHidenVars, _variableKeyNameCaptureRegex);
            }
            else
            {
                return s.EscapeQuotes();
            }
        }

        private string RestoreStrings(string inputString, Dictionary<string, string> keyStringPairs, Regex regex)
        {
            foreach (Match match in regex.Matches(inputString))
            {
                if (keyStringPairs.TryGetValue(match.Value, out var foundVar))
                {
                    inputString = inputString.Replace(match.Value, foundVar);
                }
            }

            return inputString;
        }

        private Dictionary<string, string> HideVariables(string textWhereHide, Regex regex, StringBuilder stringBuilderWhereToHide, string keyPre = "%VAR", string keyAfter = "%")
        {
            var matches = regex.Matches(textWhereHide);
            var keyStringPairs = new Dictionary<string, string>(matches.Count);
            int keyIndex = 1;
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var m = matches[i];

                // remember key name and then replace var with key name
                string keyName;
                if (keyStringPairs.TryGetValue(m.Value, out var foundKey))
                {
                    keyName = foundKey;
                }
                else
                {
                    keyName = $"{keyPre}{keyIndex++}{keyAfter}";
                    keyStringPairs.Add(m.Value, keyName);
                }
                stringBuilderWhereToHide.Remove(m.Index, m.Length).Insert(m.Index, keyName);
            }

            keyStringPairs = keyStringPairs
                .ToDictionary(kv => kv.Value, kv => kv.Key); // make key name as key

            return keyStringPairs;
        }

        const string _codeInfoText = "Command code: ";
        private static bool IsSkipCode(string info)
        {
            var codeIndex = info.IndexOf(_codeInfoText);
            if (codeIndex <= -1) return false;

            var code = info.Substring(codeIndex + _codeInfoText.Length, 3);
            return int.TryParse(code, out var codeInt)
                && RPGMVLists.ExcludedCodes.ContainsKey(codeInt);
        }

        protected override bool WriteFileData(string filePath = "")
        {
            return SaveFileMode // save mode
                && ParseData.Ret // something translated
                                 //&& ParseData.NewBinaryForWrite.Count > 0 // new bynary is not empty
                && !FunctionsFileFolder.FileInUse(GetSaveFilePath())
                && DoWriteFile(filePath);
        }

        protected override byte[] GetFileBytes()
        {
            return _bytes;
        }

        protected override bool DoWriteFile(string filePath = "")
        {
            try
            {
                if (_isScripts)
                {
                    if (!(_parser is ScriptsParser sp)) return false;

                    _bytes = sp.DumpRMScripts();
                    if (_bytes.Length == 0) return false;

                    base.DoWriteFile(filePath);
                }
                else
                {
                    if (!(_parser is Parser p)) return false;

                    p.Write();
                }

                return true;
            }
            catch { }

            return false;
        }
    }
}
