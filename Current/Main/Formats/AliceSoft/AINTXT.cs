﻿using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.AliceSoft
{
    class AINTXT : AliceSoftBase
    {
        public AINTXT(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".txt";

        string lastgroupname = "";
        readonly string ainstringpattern = @"^;?[sm]\[[0-9]{1,10}\] \= \""(.+)\""$";
        protected override KeywordActionAfter ParseStringFileLine()
        {
            var m = Regex.Match(ParseData.Line, ainstringpattern);
            if (m.Success)
            {
                var orig = m.Result("$1");

                if (IsValidString(orig))
                {
                    if (OpenFileMode)
                    {
                        AddRowData(orig, T._("Last group") + ": " + lastgroupname, isCheckInput: false);
                    }
                    else
                    {
                        string trans = orig;
                        if (SetTranslation(ref trans))
                        {
                            //set translation and replace in orig line
                            var ind = ParseData.Line.IndexOf(orig);
                            ParseData.Line = ParseData.Line.Remove(ind, orig.Length).Insert(ind, FixInvalidSymbols(trans));

                            if (ParseData.Line.StartsWith(";"))
                            {
                                ParseData.Line = ParseData.Line.Remove(0, 1);
                            }
                            ParseData.Ret = true;
                        }
                    }
                }
            }
            else
            {
                lastgroupname = ParseData.Line;
            }

            SaveModeAddLine();

            return 0;
        }

        protected override string FixInvalidSymbols(string str)
        {
            return str
                .Replace("\"", "'")/*fix syntax errors */
                .Replace("\u200B", "")/*fix invalid bytes sequence*/
                .Replace("é", "e")/*sometime google put it in translation*/
                .Replace(@"』\", "』");
        }
    }    
}
