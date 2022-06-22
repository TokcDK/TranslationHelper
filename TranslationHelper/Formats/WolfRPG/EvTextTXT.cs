using System;
using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG
{
    internal class EvTextTXT : FormatTxtFileBase
    {
        readonly List<string> Buffer = new List<string>();
        protected override KeywordActionAfter ParseStringFileLine()
        {
            ReadMessage();

            SaveModeAddLine();

            return KeywordActionAfter.Continue;
        }

        private void ReadMessage()
        {
            if (ParseData.Line.Length == 0 || ParseData.Line[0] == '/') return;

            Buffer.Add(ParseData.Line);

            while (!string.IsNullOrEmpty(ReadLine()) && ParseData.Line[0] != '/')
            {
                Buffer.Add(ParseData.Line);
            }

            var value = string.Join(Environment.NewLine, Buffer);
            AddRowData(ref value);

            if (AppData.CurrentProject.SaveFileMode) ParseData.Line = value + Environment.NewLine + ParseData.Line;

            Buffer.Clear();
        }
    }
}
