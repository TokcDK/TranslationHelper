using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.LiveMaker
{
    class LSBLNS : LiveMakerBase
    {
        public LSBLNS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        bool firstline = true;
        bool CaptureMessage = false;
        protected override int ParseStringFileLine()
        {
            if (CaptureMessage)
            {
                if (ParseData.line.StartsWith("{MESOFF"))
                {
                    CaptureMessage = false;
                }
                else
                {
                    if (
                        !ParseData.line.StartsWith("{PLAYSND")//exclude not messages
                        && !ParseData.line.StartsWith("{WAITPLAY")//exclude not messages
                        && !ParseData.line.StartsWith("{CHANGECG")//exclude not messages
                        && !ParseData.line.StartsWith("{CREATECG")//exclude not messages
                        && !ParseData.line.StartsWith("<EVENT")//exclude not messages
                        && IsValidString(CleanedFromTags(ParseData.line))
                        )
                    {
                        if (thDataWork.OpenFileMode)
                        {
                            AddRowData(ParseData.line, "", true, false);
                        }
                        else
                        {
                            AddTranslation();
                        }
                    }

                }
            }
            else if (ParseData.line.StartsWith("{MESON"))
            {
                CaptureMessage = true;
            }

            SaveModeAddLine();

            return 0;
        }

        protected override string TranslationMod(string translation)
        {
            return translation.Replace("\r\n", "\r");//using \r instead of \r\n , \r\n only after empty lines;
        }

        //check without tags
        private static string CleanedFromTags(string line)
        {
            return Regex.Replace(line, @"<STYLE ID\=\""[0-9]{1,3}\"">", "")
                .Replace("<TXSPN>", "")
                .Replace("</STYLE>", "")
                .Replace("<PG>", "")
                .Replace("<BR>", "")
                ;
        }
        protected override void SaveModeAddLine(string newline = "\r\n")
        {
            base.SaveModeAddLine(ParseData.line.Length > 0 ? "\r" : newline);//not empty line in lns ends with \r
        }
    }
}
