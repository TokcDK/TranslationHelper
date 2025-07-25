﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.NScriptGame.nscript.dat
{
    internal class NSCRIPT : FormatStringBase
    {
        public NSCRIPT(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".dat";

        protected override bool TryOpen()
        {
            return ParseFile();
        }

        protected override void FileOpen()
        {
            var filePath = GetOpenFilePath();
            var nscripttxt = System.Text.Encoding.GetEncoding(932).GetString(File.ReadAllBytes(filePath).XorUnxor());

            File.WriteAllText(filePath + ".OpenTest.txt", nscripttxt, System.Text.Encoding.GetEncoding(932));

            foreach (var line in nscripttxt.SplitToLines())
            {
                ParseData.Line = line;
                KeywordActionAfter parseLineResult;
                if ((parseLineResult = ParseStringFileLine()) == KeywordActionAfter.Break)
                {
                    break;
                }
                else if (parseLineResult == 0)
                {
                    continue;
                }
            }
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            //ParseData.TrimmedLine = ParseData.line;

            if (IsComment())
            {
                if (SaveFileMode)
                    ParseData.ResultForWrite.Append(ParseData.Line + '\n');
                return 0;
            }

            if (!ParsePatterns())
            {
                var array = ParseData.Line.Split(':');
                var lines = new List<string>(array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    if (IsValidString(CleanedLine(array[i].Trim())))
                    {
                        var str = array[i];

                        if (OpenFileMode)
                        {
                            AddRowData(str, "", isCheckInput: true);
                        }
                        else
                        {
                            var trans = str;
                            if (SetTranslation(ref trans) && !string.IsNullOrEmpty(trans) && str != trans)
                            {
                                int ind;
                                array[i] = array[i].Remove(ind = array[i].IndexOf(str), str.Length).Insert(ind, FixInvalidSymbols(trans));
                            }
                        }
                    }
                    if (SaveFileMode)
                    {
                        lines.Add(array[i]);
                    }
                }
                if (SaveFileMode && lines.Count > 0)
                {
                    ParseData.Line = string.Join(":", lines);
                }
            }

            if (SaveFileMode)
                ParseData.ResultForWrite.Append(ParseData.Line + '\n');
            return KeywordActionAfter.ReadToEnd;
        }

        protected override string FixInvalidSymbols(string str)
        {
            str = base.FixInvalidSymbols(str);

            str = str
                .Replace('[', '【')
                .Replace(']', '】')
                .Replace('(', '（')
                .Replace(')', '）')
                //.Replace("'", "`")
                .Replace("!", "！")
                .Replace("?", "？")
                .Replace("?", "？")
                .Replace(';', '；')
                .Replace(':', '：')
                .Replace('*', '＊')
                .Replace(".", "。")
                .Replace("'", "")
                .Replace(",", "、")
                .Replace('+', '＋')
                .Replace('+', '＋')
                .Replace("@", "")
                .Replace("/", "")
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
                ;
            return (str.StartsWith("`") ? "" : "`") + str + (str.EndsWith("`") ? "" : "`");
        }

        protected override string AddRowDataPreAddOriginalStringMod(string str)
        {
            return (str = str.TrimEnd()).EndsWith("\\") ? str.Remove(str.Length - 1, 1) : str;
        }

        protected override List<ParsePatternData> Patterns()
        {
            return new List<ParsePatternData>()
            {
                new ParsePatternData( @"csel \""([^\""]+)\""", info: "selection" ),
                new ParsePatternData( @"if (( && )?\%[^\=\>\<]+[\=\>\<]+[0-9a-zA-Z\""\']+)+ ([^\&a-zA-Z].+)", group: 3 ),
                new ParsePatternData( @"\[([^\]]+)\]「[^」]+」", info: "speaker name" ),
                new ParsePatternData( @"\[[^\]]+\]「([^」]+)」", info: "speaker's text" ),
                new ParsePatternData( @"\""([^\%\$\:\,\+/\n\\\""]+)\""", info: "quoted string" ),
                new ParsePatternData( @":([^\n\"",><;:\\]+)\\", info: "any text" ),
            };
        }

        private static string CleanedLine(string Line)
        {
            string cleaned = Line.Split(';')[0];//remove comment

            string pattern = @"(#[A-F0-9]{6})|([\%\$][a-z0-9]+)";
            if (cleaned.Contains("#") && Regex.IsMatch(cleaned, pattern))
            {
                foreach (Match match in Regex.Matches(cleaned, pattern))
                {
                    cleaned = cleaned.Replace(match.Value, string.Empty);//remove color tags
                }
            }

            return cleaned;
        }

        private bool IsComment()
        {
            return ParseData.Line.TrimStart().StartsWith(";");
        }

        protected override bool TrySave()
        {
            return ParseFile();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && SaveFileMode && ParseData.ResultForWrite.Length > 0)
                {
                    var Enc = System.Text.Encoding.GetEncoding(932);
                    var nscripttxtTranslated = ParseData.ResultForWrite.ToString();
                    if (filePath.Length == 0)
                    {
                        filePath = GetSaveFilePath();
                    }
                    File.WriteAllText(filePath + ".SaveTest.txt", nscripttxtTranslated, Enc);
                    var nscriptdatTranslated = Enc.GetBytes(nscripttxtTranslated).XorUnxor();
                    var nscriptdatTranslatedFile = new FileInfo(filePath)
                    {
                        Attributes = FileAttributes.Normal
                    };
                    File.WriteAllBytes(nscriptdatTranslatedFile.FullName, nscriptdatTranslated);

                    //ONSCRIPTER
                    {
                        //copy onscripter
                        if (!Directory.Exists(Path.Combine(ParentProject.SelectedGameDir, "onscripter")))
                            Path.Combine(THSettings.ResDirPath, "onscripter").CopyAll(Path.Combine(ParentProject.SelectedGameDir, "onscripter"));

                        //write run.bat
                        //onscripter -r "gamedir" --dll "dllpath" -f fontpath --window
                        var g = Directory.GetFiles(ParentProject.SelectedGameDir, "*.dll");
                        var ls = g.Select(fn => "--dll \"" + Path.Combine(ParentProject.SelectedGameDir, Path.GetFileName(fn)) + "\" ");
                        string dlls = string.Join("", ls);
                        var batcontent = "ONScripter "
                            + "-r \"" + ParentProject.SelectedGameDir + "\" "
                            + dlls
                            + "-f C:\\Windows\\Fonts\\msgothic.ttc "
                            + "--window"
                        ;
                        File.WriteAllText(Path.Combine(ParentProject.SelectedGameDir, "onscripter", "Run.bat"), batcontent);
                    }
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
    }
}
