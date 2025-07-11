﻿using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.LiveMaker
{
    class LSBLNS : LiveMakerBase
    {
        public LSBLNS(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".lns";

        bool CaptureMessage = false;
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (CaptureMessage)
            {
                if (ParseData.Line.StartsWith("{MESOFF"))
                {
                    CaptureMessage = false;
                }
                else
                {
                    if (
                        !ParseData.Line.StartsWith("{PLAYSND")//exclude not messages
                        && !ParseData.Line.StartsWith("{WAITPLAY")//exclude not messages
                        && !ParseData.Line.StartsWith("{CHANGECG")//exclude not messages
                        && !ParseData.Line.StartsWith("{DELETECG")//exclude not messages
                        && !ParseData.Line.StartsWith("{CREATECG")//exclude not messages
                        && !ParseData.Line.StartsWith("<EVENT")//exclude not messages)
                        )
                    {
                        AddRowData(ref ParseData.Line);
                    }

                }
            }
            else if (ParseData.Line.StartsWith("{MESON"))
            {
                CaptureMessage = true;
            }

            SaveModeAddLine(newline: ParseData.Line.Length > 0 ? "\r" : "\r\n");//not empty line in lns ends with \r

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
    }
}
