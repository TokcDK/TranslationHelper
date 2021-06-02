using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.AliceSoft
{
    class AINTXT : AliceSoftBase
    {
        public AINTXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        string lastgroupname = "";
        readonly string ainstringpattern = @"^;?[sm]\[[0-9]{1,10}\] \= \""(.+)\""$";
        protected override int ParseStringFileLine()
        {
            var m = Regex.Match(ParseData.line, ainstringpattern);
            if (m.Success)
            {
                var orig = m.Result("$1");

                if (IsValidString(orig))
                {
                    if (thDataWork.OpenFileMode)
                    {
                        AddRowData(orig, T._("Last group") + ": " + lastgroupname, true, false);
                    }
                    else
                    {
                        string trans = orig;
                        if (SetTranslation(ref trans))
                        {
                            //set translation and replace in orig line
                            var ind = ParseData.line.IndexOf(orig);
                            ParseData.line = ParseData.line.Remove(ind, orig.Length).Insert(ind, FixInvalidSymbols(trans));

                            if (ParseData.line.StartsWith(";"))
                            {
                                ParseData.line = ParseData.line.Remove(0, 1);
                            }
                            ParseData.Ret = true;
                        }
                    }
                }
            }
            else
            {
                lastgroupname = ParseData.line;
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
