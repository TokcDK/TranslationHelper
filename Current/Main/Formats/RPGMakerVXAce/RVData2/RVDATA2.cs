using System;
using System.IO;
using System.Text.RegularExpressions;
using RPGMakerVXRVData2Assistant;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMakerVX.RVData2
{
    internal class RVDATA2 : FormatBinaryBase
    {
        public override string Extension => ".rvdata2";
        readonly string regexQuote = @"\""";
        bool _isScripts = false;

        Parser _parser;
        ScriptsParser _scriptsParser;

        protected override void FileOpen()
        {
            _isScripts = string.Equals(Path.GetFileNameWithoutExtension(FilePath), "scripts", StringComparison.InvariantCultureIgnoreCase);

            if (_isScripts)
            {
                _scriptsParser = new ScriptsParser(FilePath);
                foreach (var script in _scriptsParser.EnumerateRMScripts())
                {
                    // parse all strings inside quotes in script content

                    var scriptContentToChange = script.Text;
                    if (scriptContentToChange.Length == 0) continue;

                    var mc = Regex.Matches(scriptContentToChange, /*@"[\""']([^\""'\r\n]+)[\""']"*/
                            @"" + regexQuote + @"([^" + regexQuote + @"\r\n\\]*(?:\\.[^" + regexQuote + @"\\]*)*)" + regexQuote //all between " or ' include \" or \' : x: "abc" or "abc\"" or 'abc' or 'abc\''
                        );

                    var mcCount = mc.Count;
                    if (mcCount == 0) continue;

                    bool isChanged = false;

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

                    if (isChanged && SaveFileMode) script.Text = scriptContentToChange;
                }
                //foreach(var script in parser.EnumerateScripts())
                //{
                //    // parse all strings inside quotes in script content

                //    var scriptContentToChange = script.Content.Text;
                //    if (scriptContentToChange.Length == 0) continue;

                //    var mc = Regex.Matches(scriptContentToChange, /*@"[\""']([^\""'\r\n]+)[\""']"*/
                //            @"" + regexQuote + @"([^" + regexQuote + @"\r\n\\]*(?:\\.[^" + regexQuote + @"\\]*)*)" + regexQuote //all between " or ' include \" or \' : x: "abc" or "abc\"" or 'abc' or 'abc\''
                //        );

                //    var mcCount = mc.Count;
                //    if (mcCount == 0) continue;

                //    bool isChanged = false;

                //    // negative order because length of content is changing
                //    for (int i = mcCount - 1; i >= 0; i--)
                //    {
                //        var m = mc[i];
                //        var s = m.Groups[1].Value;

                //        if (AddRowData(ref s, $"Script: {script.Title.Text}") && SaveFileMode)
                //        {
                //            isChanged = true;
                //            scriptContentToChange = scriptContentToChange
                //                .Remove(m.Index, m.Length)
                //                .Insert(m.Index, "\"" + s + "\"");
                //        }
                //    }

                //    if (isChanged && SaveFileMode) script.Content.Text = scriptContentToChange;
                //}
            }
            else
            {
                _parser = new Parser(FilePath);
                foreach (var stringData in _parser.EnumerateStrings())
                {
                    if (_isScripts)
                    {
                        continue;
                    }
                    else
                    {
                        string s = stringData.Text;
                        if (AddRowData(ref s, stringData.Info.ToString()) && SaveFileMode)
                        {
                            stringData.Text = s;
                        }
                    }

                }
            }

        }

        protected override bool WriteFileData(string filePath = "")
        {
            return SaveFileMode // save mode
                && ParseData.Ret // something translated
                                 //&& ParseData.NewBinaryForWrite.Count > 0 // new bynary is not empty
                && !FunctionsFileFolder.FileInUse(GetSaveFilePath())
                && DoWriteFile(filePath);
        }

        byte[] _bytes;
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
                    _bytes = _scriptsParser.DumpRMScripts();
                    if (_bytes.Length == 0) return false;

                    base.DoWriteFile(filePath);
                }
                else _parser.Write();

                return true;
            }
            catch { }
            return false;
        }
    }
}
