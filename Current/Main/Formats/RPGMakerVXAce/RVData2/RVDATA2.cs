using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RPGMakerVXRVData2Assistant;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMakerVX.RVData2
{
    internal class RVDATA2 : FormatBinaryBase
    {
        public override string Extension => ".rvdata2";
        bool _isScripts = false;

        object _parser;
        byte[] _bytes;
        readonly Regex _quoteCaptureRegex = new Regex(AppMethods.GetRegexQuotesCapturePattern(), RegexOptions.Compiled);
        readonly Regex _mapNameCheckRegex = new Regex("[Mm]ap[0-9]{3}", RegexOptions.Compiled);

        protected override void FileOpen()
        {
            var name = Path.GetFileNameWithoutExtension(FilePath);
            _isScripts = string.Equals(name, "scripts", StringComparison.InvariantCultureIgnoreCase);

            if (_isScripts)
            {
                _parser = new ScriptsParser(FilePath);

                foreach (var script in (_parser as ScriptsParser).EnumerateRMScripts())
                {
                    // parse all strings inside quotes in script content

                    if (string.IsNullOrEmpty(script.Text)) continue;

                    var mc = _quoteCaptureRegex.Matches(script.Text);

                    var mcCount = mc.Count;
                    if (mcCount == 0) continue;

                    bool isChanged = false;

                    var scriptContentToChange = new StringBuilder(script.Text);
                    // negative order because length of content is changing
                    for (int i = mcCount - 1; i >= 0; i--)
                    {
                        var m = mc[i];
                        var s = m.Groups[1].Value;

                        if (AddRowData(ref s, $"Script: {script.Title}") && SaveFileMode)
                        {
                            isChanged = true;
                            scriptContentToChange = scriptContentToChange
                                .Remove(m.Index, m.Length)
                                .Insert(m.Index, "\"" + s + "\"");
                        }
                    }

                    if (isChanged && SaveFileMode) script.Text = scriptContentToChange.ToString();
                }
            }
            else
            {
                _parser = new Parser(FilePath);

                var isCommandsUser = string.Equals(name, "CommonEvents", StringComparison.InvariantCultureIgnoreCase)
                    || _mapNameCheckRegex.IsMatch(name);

                foreach (var stringData in (_parser as Parser).EnumerateStrings())
                {
                    string s = stringData.Text;
                    var info = stringData.Info.ToString();
                    if (isCommandsUser && IsSkipCode(info)) continue;

                    if (AddRowData(ref s, info) && SaveFileMode) stringData.Text = s;
                }
            }

        }

        protected override string FixInvalidSymbols(string str)
        {
            if (_isScripts)
            {
                // fix not escaped symbols for sccripts
                foreach (var c in new[] { '"', '\\' })
                {
                    int ind = str.LastIndexOf('"');
                    while (ind != -1)
                    {
                        var checkIndex = ind - 1;
                        if (checkIndex > -1 && str[checkIndex] != '\\')
                        {
                            str = str.Insert(ind, "\\");
                        }
                        ind = str.LastIndexOf('"', checkIndex);
                    }
                }
            }

            return base.FixInvalidSymbols(str);
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
