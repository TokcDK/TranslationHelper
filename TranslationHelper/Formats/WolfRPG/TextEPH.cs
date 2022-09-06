﻿using System.Collections.Generic;

namespace TranslationHelper.Formats.WolfRPG
{
    internal class TextEPH : FormatTxtFileBase
    {
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!string.IsNullOrEmpty(ParseData.Line))
            {
                var lines = new List<string>
                {
                    ParseData.Line
                };
                while (!string.IsNullOrEmpty(ReadLine()))
                {
                    lines.Add(ParseData.Line);
                }

                var s = string.Join("\r\n", lines);
                if (AddRowData(ref s, "") && SaveFileMode)
                {
                    ParseData.Line = s + "\r\n" + ParseData.Line;
                }
            }

            SaveModeAddLine();
            return KeywordActionAfter.Continue;
        }
    }
}
