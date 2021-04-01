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
                        !ParseData.line.StartsWith("{PLAYSND")
                        && !ParseData.line.StartsWith("{WAITPLAY")
                        && !ParseData.line.StartsWith("{CHANGECG")
                        && !ParseData.line.StartsWith("{CREATECG")
                        && !ParseData.line.StartsWith("<EVENT")
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

        private string CleanedFromTags(string line)
        {
            return Regex.Replace(line, @"<STYLE ID\=\""[0-9]{1,3}\"">", "")
                .Replace("<TXSPN>", "")
                .Replace("</STYLE>", "")
                .Replace("<PG>", "")
                .Replace("<BR>", "")
                ;
        }
    }
}
