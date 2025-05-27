using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV
{
    class GAMEFONTCSS : RPGMMVBase
    {
        public override string Extension => ".css";

        public GAMEFONTCSS(ProjectBase parentProject) : base(parentProject)
        {
        }

        readonly Regex _fontSearchPattern = new Regex(@"src: url\(\""([^\""]+)\""\)", RegexOptions.Compiled);

        readonly string _fontInfo = T._(
                            "GameFont.\nValue in Translation field must be exist font css file name or path.\n" +
                            "If it is exists, it will be copied into %GAME%\\www\\fonts\\.\n" +
                            "If value is font file name, it will be searching in system fonts folder location." +
                            "Useful to change font when need to reduce size of text in game if need." +
                            "Examples:" +
                            "browa.ttf\n" +
                            "c:/windows/fonts/browa.ttf\n" +
                            "%LocalAppData%/Microsoft/Windows/Fonts/browa.ttf" +
                            "");

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.IsComment)
            {
                if (IsCommentSectionEnd()) ParseData.IsComment = false; //comment section end
            }
            else
            {
                Match r;
                if (IsCommentSectionStart()) //comment section start
                {
                    if (!IsCommentSectionEnd()) ParseData.IsComment = true;
                }
                else if (ParseData.TrimmedLine.StartsWith("//")) //comment
                {
                }
                else if ((r = _fontSearchPattern.Match(ParseData.Line)).Success)
                {
                    if (OpenFileMode)
                    {
                        ParseData.Ret = AddRowData(r.Groups[1].Value, _fontInfo, isCheckInput: false);

                        return KeywordActionAfter.Break;
                    }
                    else
                    {
                        string str = r.Groups[1].Value;
                        var trans = str;
                        if (SetTranslation(ref trans, isCheckInput: false) && ParseFont(str, ref trans))
                        {
                            int i;
                            ParseData.Line = ParseData.Line
                                .Remove(r.Index, r.Value.Length)
                                .Insert(r.Index,
                                r.Value.Remove(i = r.Value.IndexOf(str), str.Length).Insert(i, Environment.ExpandEnvironmentVariables(trans).Replace("\\", "/"))
                                );

                            ParseData.Ret = true;
                        }
                    }
                }
            }

            SaveModeAddLine();

            return KeywordActionAfter.ReadToEnd;
        }

        private bool IsCommentSectionEnd()
        {
            return ParseData.TrimmedLine.EndsWith("*/");
        }

        private bool IsCommentSectionStart()
        {
            return ParseData.TrimmedLine.StartsWith("/*");
        }

        /// <summary>
        /// copy font file into rpfmv fonts dir and set font name as value if exists
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        private bool ParseFont(string orig, ref string trans)
        {
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(trans.Contains("%") ? Environment.ExpandEnvironmentVariables(trans).Replace("\\", "/") : trans);
            }
            catch
            {
                return false;
            }

            var targetFontsDirPath = Path.Combine(AppData.CurrentProject.SelectedGameDir, "www", "Fonts");

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

            if (!File.Exists(fullPath) || !string.Equals(Path.GetExtension(fullPath), ".ttf", StringComparison.InvariantCultureIgnoreCase)) 
                return false;

            var fontFileName = Path.GetFileName(fullPath);

            trans = fontFileName;

            if (File.Exists(Path.Combine(targetFontsDirPath, fontFileName))) return true;

            File.Copy(fullPath, Path.Combine(targetFontsDirPath, fontFileName));

            return true;
        }

        protected override bool TrySave()
        {
            return ParseFile();
        }

    }
}
