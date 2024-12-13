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
        readonly Regex _variableCaptureRegex = new Regex("((#{[^}]+})|(#[a-zA-Z0-9]+))", RegexOptions.Compiled);
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
                    if (script.Text.Contains("ここまで（18.5文字）"))
                    {
                    }

                    // capture also intext variables like #{text}
                    // remove false capture for quotes like #{Convert_Text.button_to_icon("マルチ")}
                    var mc = _variableCaptureRegex.Matches(script.Text);
                    var scriptTextNoVarsNoComments = new StringBuilder(script.Text);
                    var inTextVariablesCoordinates = new Dictionary<string, string>(mc.Count);
                    int varIndex = 1;
                    for (int i = mc.Count - 1; i >= 0; i--)
                    {
                        var m = mc[i];

                        // remember key name and then replace var with key name
                        string keyName;
                        if (inTextVariablesCoordinates.TryGetValue(m.Value, out var foundKey))
                        {
                            keyName = foundKey;
                        }
                        else
                        {
                            keyName = $"%VAR{varIndex++}%";
                            inTextVariablesCoordinates.Add(m.Value, keyName);
                        }
                        scriptTextNoVarsNoComments.Remove(m.Index, m.Length).Insert(m.Index, keyName);
                    }
                    inTextVariablesCoordinates = inTextVariablesCoordinates
                        .ToDictionary(kv => kv.Value, kv => kv.Key);

                    // remove false capture commented quoted text
                    mc = _commentaryCaptureRegex.Matches(scriptTextNoVarsNoComments.ToString());
                    var commentsCoordinates = new Dictionary<string, string>(mc.Count);
                    int commentIndex = 1;
                    for (int i = mc.Count - 1; i >= 0; i--)
                    {
                        var m = mc[i];

                        if(m.Value=="#" || string.IsNullOrWhiteSpace(m.Value.TrimStart().TrimStart('#')))
                        {
                            continue;
                        }

                        // remember coordinates of comment an remove it
                        string keyName;
                        if (commentsCoordinates.TryGetValue(m.Value, out var foundKey))
                        {
                            keyName = foundKey;
                        }
                        else
                        {
                            keyName = $"%COMMENT{commentIndex++}%";
                            commentsCoordinates.Add(m.Value, keyName);
                        }
                        scriptTextNoVarsNoComments.Remove(m.Index, m.Length).Insert(m.Index, keyName);
                    }
                    commentsCoordinates = commentsCoordinates
                        .ToDictionary(kv => kv.Value, kv => kv.Key);

                    var scriptText = scriptTextNoVarsNoComments.ToString();

                    // capture quoted strings itself
                    mc = _quoteCaptureRegex.Matches(scriptText);

                    var mcCount = mc.Count;
                    if (mcCount == 0) continue;

                    bool isChanged = false;

                    var scriptContentToChange = SaveFileMode ? new StringBuilder(scriptText) : null;
                    // negative order because length of content is changing
                    for (int i = 0; i < mcCount; i++)
                    {
                        var m = mc[i];
                        string s = m.Groups[1].Value;
                        //int origialLength = s.Length;

                        foreach (Match match in _variableCaptureRegex.Matches(s))
                        {
                            if(inTextVariablesCoordinates.TryGetValue(match.Value, out var foundKey))
                            {
                                s = s.Replace(match.Value, foundKey);
                            }
                        }

                        if (AddRowData(ref s, $"Script: {script.Title}") && SaveFileMode)
                        {
                            //int translateLength = s.Length;
                            //int translateOriginalDifference = translateLength - origialLength; // increase result comment coordinates by this

                            // здесь вставляем обратно переменные?
                            //string stringWithRestoredVariables = s;
                            //var mc1 = Regex.Matches(scriptText, @"#{VAR00[0-9]+}");
                            //int varAfterToBeforeDifference = 0; // also increase result last message index by this
                            //for (int i1 = mc1.Count - 1; i1 >= 0; i1--)
                            //{
                            //    var m1 = mc1[i]; // variable key

                            //    if (inTextVariablesCoordinates.TryGetValue(m1.Value, out var m2))
                            //    {
                            //        //varAfterToBeforeDifference += (m2.Index - m1.Index); // m2 is original variable
                            //        stringWithRestoredVariables = stringWithRestoredVariables.Remove(m1.Index, m1.Length).Insert(m1.Index, m2.Value);
                            //    }
                            //}

                            // здесь пересчитываем длину оригинальной строки на переведенную, потом пересчитываем начальные координаты для комментария и вставляем его обратно
                            // рассчитываем как координаты для переменных, так и координаты для комментариев
                            isChanged = true;
                            scriptContentToChange
                                .Remove(m.Index, m.Length)
                                .Insert(m.Index, "\"" + s.EscapeQuotes() + "\"");

                            // нужно сначала рассчитать старый индекс комментария, который рассчитывается из отношения длина оригинала + длина всех восстановленных переменных с пересчеотм разницы
                            // потом рассчитать новый индекс комментария из рассчета старый индекс + длина перевода + длина всех восстановленных переменных с пересчетом разницы
                            // после ищем по начальному индексу если есть комментарий и вставляем его обратно
                            // также стоит учесть, что есть и комментарии вне строк с кавычками, до них и между ними, их всех нужно вставить от начала и до конца
                            //int resultCommentIndex = m.Index + translateLength + translateOriginalDifference + varAfterToBeforeDifference;
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
