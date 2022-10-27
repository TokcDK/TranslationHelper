using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TranslationHelper.Formats.RPGMMV
{
    class GAMEFONTCSS : RPGMMVBase
    {
        public GAMEFONTCSS()
        {
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.IsComment)
            {
                if (ParseData.TrimmedLine.EndsWith("*/")) ParseData.IsComment = false; //comment section end
            }
            else
            {
                Match r;
                if (ParseData.TrimmedLine.StartsWith("/*")) //comment section start
                {
                    if (!ParseData.TrimmedLine.EndsWith("*/")) ParseData.IsComment = true;
                }
                else if (ParseData.TrimmedLine.StartsWith("//")) //comment
                {
                }
                else if ((r = Regex.Match(ParseData.Line, @"src: url\(\""([^\""]+)\""\)")).Success)
                {
                    if (OpenFileMode)
                    {
                        ParseData.Ret = AddRowData(r.Result("$1"), T._("GameFont.\r\nFont must be installed in system or file placed in folder %GAME%\\www\\fonts\\ \r\n or use absolute path. \r\n Change font to smaller is more preferable than line split function\r\nexamples:\r\nWin7:c:/windows/fonts/browa.ttf\r\nWin10: %LocalAppData%/Microsoft/Windows/Fonts/browa.ttf"), isCheckInput: false);

                        return KeywordActionAfter.Break;
                    }
                    else
                    {
                        string str = r.Result("$1");
                        var trans = str;
                        if (SetTranslation(ref trans, isCheckInput: false) && ParseFont(str, ref trans))
                        {
                            int i;
                            ParseData.Line = ParseData.Line
                                .Remove(r.Index, r.Value.Length)
                                .Insert(r.Index,
                                r.Value.Remove(i = r.Value.IndexOf(str), str.Length).Insert(i, Environment.ExpandEnvironmentVariables(trans).Replace("\\", "/"))
                                );
                            //ParseData.ResultForWrite.AppendLine(str);
                            ParseData.Ret = true;
                        }
                    }
                }
            }

            SaveModeAddLine();

            return KeywordActionAfter.ReadToEnd;
        }

        /// <summary>
        /// copy font file into rpfmv fonts dir and set font name as value if exists
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        private bool ParseFont(string orig, ref string trans)
        {
            var fullPath = "";

            ParseData.Ret = false;

            try
            {
                fullPath = Path.GetFullPath(trans.Contains("%") ? Environment.ExpandEnvironmentVariables(trans).Replace("\\", "/") : trans);
            }
            catch
            {
                return ParseData.Ret;
            }

            var targetFontsDirPath = Path.Combine(Data.AppData.CurrentProject.SelectedGameDir, "www", "Fonts");

            if (!Directory.Exists(Path.GetDirectoryName(targetFontsDirPath))) return ParseData.Ret;

            var fileName = Path.GetFileName(fullPath);
            bool isFileName = string.Equals(fileName, fullPath, StringComparison.InvariantCultureIgnoreCase);

            bool isExists = File.Exists(fullPath);
            bool isBreak = (!isFileName && isExists) || (isFileName && !isExists);
            foreach (var path in new[]
            {
                "%WinDir%/fonts/",
                "%LocalAppData%/Microsoft/Windows/Fonts/"
            })
            {
                if (isBreak) break;

                var searchFontFilePath = Path.Combine(Environment.ExpandEnvironmentVariables(path), fileName);
                if (!File.Exists(searchFontFilePath)) continue;

                fullPath = searchFontFilePath;
            }

            ParseData.Ret = File.Exists(fullPath) && string.Equals(Path.GetExtension(fullPath), ".ttf", StringComparison.InvariantCultureIgnoreCase);

            if (!ParseData.Ret) return false;

            var fontFileName = Path.GetFileName(fullPath);

            trans = fontFileName;

            if (File.Exists(targetFontsDirPath)) return true;

            File.Copy(fullPath, Path.Combine(targetFontsDirPath, fontFileName));

            return ParseData.Ret;
        }

        protected override bool TrySave()
        {
            SaveFileMode = true;
            return ParseFile();
        }

    }
}
