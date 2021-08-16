﻿using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.AliceSoft
{
    class Aintxt : AliceSoftBase
    {
        public Aintxt()
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        string _lastgroupname = "";
        readonly string _ainstringpattern = @"^;?[sm]\[[0-9]{1,10}\] \= \""(.+)\""$";
        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            var m = Regex.Match(ParseData.Line, _ainstringpattern);
            if (m.Success)
            {
                var orig = m.Result("$1");

                if (IsValidString(orig))
                {
                    if (ProjectData.OpenFileMode)
                    {
                        AddRowData(orig, T._("Last group") + ": " + _lastgroupname, true, false);
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
                _lastgroupname = ParseData.Line;
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
