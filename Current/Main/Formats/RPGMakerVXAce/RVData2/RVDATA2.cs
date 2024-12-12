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
        readonly Regex _variableCaptureRegex = new Regex("#{[^}]+}", RegexOptions.Compiled);
        readonly Regex _mapNameCheckRegex = new Regex("[Mm]ap[0-9]{3}", RegexOptions.Compiled);

        protected override void FileOpen()
        {
            FileOpen(null);
        }

        public void FileOpen(byte[] fileBytes)
        {
            var name = Path.GetFileNameWithoutExtension(FilePath);
            _isScripts = string.Equals(name, "scripts", StringComparison.InvariantCultureIgnoreCase);

            bool isBytes = fileBytes != null && fileBytes.Length > 0;

            if (_isScripts)
            {
                _parser = new ScriptsParser(FilePath, fileBytes);

                foreach (var script in (_parser as ScriptsParser).EnumerateRMScripts())
                {
                    // parse all strings inside quotes in script content

                    if (string.IsNullOrEmpty(script.Text)) continue;

                    //if(script.Title== "Game_Battler")
                    //{
                    //}
                    if (script.Text.Contains("｜ 運（防御側）："))
                    {
                    }

                    // remove false capture commented quoted text
                    var mc = _commentaryCaptureRegex.Matches(script.Text);
                    var scriptTextNoComments = new StringBuilder(script.Text);
                    var commentsCoordinates = new Dictionary<int, Match>(mc.Count);
                    for (int i = mc.Count - 1; i >= 0; i--)
                    {
                        var m = mc[i];

                        // remember coordinates of comment an remove it
                        commentsCoordinates.Add(m.Index, m);
                        scriptTextNoComments.Remove(m.Index, m.Length);
                    }

                    string scriptText = scriptTextNoComments.ToString();

                    // capture also intext variables like #{text}
                    // remove false capture for quotes like #{Convert_Text.button_to_icon("マルチ")}
                    mc = _variableCaptureRegex.Matches(scriptText);
                    var inTextVariablesCoordinates = new Dictionary<string, Match>(mc.Count);
                    int varIndex = 1;
                    for (int i = mc.Count - 1; i >= 0; i--)
                    {
                        var m = mc[i];

                        // remember key name and then replace var with key name
                        string keyName = "#{VAR00" + $"{varIndex++}"+"}";
                        inTextVariablesCoordinates.Add(keyName, m);
                        scriptTextNoComments.Remove(m.Index, m.Length).Insert(m.Index, keyName);
                    }

                    scriptText = scriptTextNoComments.ToString();

                    // capture quoted strings itself
                    mc = _quoteCaptureRegex.Matches(scriptText);

                    var mcCount = mc.Count;
                    if (mcCount == 0) continue;

                    bool isChanged = false;

                    var scriptContentToChange = SaveFileMode ? new StringBuilder(scriptText) : null;
                    // negative order because length of content is changing
                    for (int i = mcCount - 1; i >= 0; i--)
                    {
                        var m = mc[i];
                        string s = m.Groups[1].Value;

                        if (AddRowData(ref s, $"Script: {script.Title}") && SaveFileMode)
                        {
                            // здесь вставляем обратно переменные?
                            //StringBuilder s1 = new StringBuilder(s);
                            //var mc1 = Regex.Matches(scriptText, @"#{VAR00[0-9]+}");
                            //for (int i1 = mc1.Count - 1; i1 >= 0; i1--)
                            //{
                            //    var m1 = mc1[i];

                            //    if (inTextVariablesCoordinates.TryGetValue(m1.Value, out var m2))
                            //    {
                            //        s1 = s1.Remove(m2.Index, m2.Length).Insert(m2.Index, m1.Value);
                            //    }
                            //}

                            // здесь пересчитываем длину оригинальной строки на переведенную, потом пересчитываем начальные координаты для комментария и вставляем его обратно
                            // рассчитываем как координаты для переменных, так и координаты для комментариев
                            isChanged = true;
                            scriptContentToChange
                                .Remove(m.Index, m.Length)
                                .Insert(m.Index, "\"" + s.EscapeQuotes() + "\"");
                        }
                    }

                    if (isChanged && SaveFileMode) script.Text = scriptContentToChange.ToString();
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
