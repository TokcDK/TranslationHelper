﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.Other
{
    internal class ExternMessageCSV : RPGMMVBase
    {
        public ExternMessageCSV(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".csv";

        protected override void ParseFileContent()
        {
            var blocks = ParseData.Reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);

            var blocksCount = blocks.Length;
            for (int i = 0; i < blocksCount; i++)
            {
                var block = blocks[i];

                if (block.Trim() == ",") continue;

                Match blockMessageMatch = Regex.Match(block, @"[^,]+,\s*\""([\s\S]+)\""\s*$");

                if (!blockMessageMatch.Success) blockMessageMatch = Regex.Match(block, @"[^,]+,\s*([\s\S]+)\s*$"); // no quotes

                if (!blockMessageMatch.Success) continue;

                var stringGroupMatch = blockMessageMatch.Groups[1];
                var messageString = stringGroupMatch.Value;

                if (!AddRowData(ref messageString, "") || !SaveFileMode) continue;

                blocks[i] = block
                    .Remove(stringGroupMatch.Index, stringGroupMatch.Length)
                    .Insert(stringGroupMatch.Index, messageString
                    .Replace("\r\n", "\n")
                    );
            }

            if (SaveFileMode) ParseData.ResultForWrite.Append(string.Join("\r\n", blocks));
        }
    }
}
