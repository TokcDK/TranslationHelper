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
        static readonly string regexQuote = @"\""";

        Parser _parser;

        protected override void FileOpen()
        {
            bool isScripts = string.Equals(Path.GetFileNameWithoutExtension(FilePath), "scripts", StringComparison.InvariantCultureIgnoreCase);

            _parser = new Parser();
            foreach (var stringData in _parser.EnumerateStrings(FilePath))
            {
                if (isScripts)
                {
                    // parse all strings inside quotes in script content

                    var scriptContentToChange = stringData.Text;
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

                        if (AddRowData(ref s, stringData.Info.ToString()) && SaveFileMode)
                        {
                            isChanged = true;
                            scriptContentToChange = scriptContentToChange
                                .Remove(m.Index, m.Length)
                                .Insert(m.Index, "\"" + s + "\"");
                        }
                    }

                    if (isChanged && SaveFileMode) stringData.Text = scriptContentToChange;
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
        protected override bool TrySave()
        {
            return base.TrySave();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            return SaveFileMode // save mode
                && ParseData.Ret // something translated
                                 //&& ParseData.NewBinaryForWrite.Count > 0 // new bynary is not empty
                && !FunctionsFileFolder.FileInUse(GetSaveFilePath())
                && DoWriteFile(filePath);
        }

        protected override bool DoWriteFile(string filePath = "")
        {
            try
            {
                _parser.Write(FilePath);
                return true;
            }
            catch { }
            return false;
        }
    }
}
