﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SplitLongLinesSpec : RowBase
    {
        public SplitLongLinesSpec()
        {
        }

        protected override bool IsOkTable(TableData table)
        {
            return !Project.LineSplitProjectSpecificSkipForTable(table.SelectedTable);
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            string origCellValue = rowData.Original as string;
            string transCellValue = rowData.Translation + string.Empty;
            if (!string.IsNullOrWhiteSpace(transCellValue)
                && transCellValue != origCellValue
                && FunctionsString.GetLongestLineLength(transCellValue) > AppSettings.THOptionLineCharLimit
                /*&& !FunctionsString.IsStringContainsSpecialSymbols(transCellValue)*/
                && !Project.LineSplitProjectSpecificSkipForLine(origCellValue, transCellValue, rowData.SelectedTableIndex, rowData.SelectedRowIndex))
            {
                rowData.Translation = SplitNew(transCellValue);


                //Translation = transCellValue.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit);
                //row[1] = transCellValue.Wrap(AppSettings.THOptionLineCharLimit);
                return true;
            }
            return false;
        }

        //rpg maker symbols
        Dictionary<string, string> _symbols = new Dictionary<string, string>
            {
                { @"(if|en)\([^\r\n]+\)$", string.Empty},
                { @"[\\]{1,4}C\[[0-9]{1,3}\]", string.Empty},
                { @"[\\]{1,4}[NV]\[[0-9]{1,3}\]", "NAME"},
                { @"\\#\{[^\}]+\}", "VAR"},
            };

        private string SplitNew(string lines)
        {
            var limit = AppSettings.THOptionLineCharLimit;
            var newlines = new List<string>();
            foreach(var line in lines.SplitToLines())
            {
                var cleaned = CleanLine(line);

                if(cleaned.Length> limit)
                {
                    var splitted = cleaned.Wrap(limit);

                    newlines.Add(ReturnSymbols(line, splitted));
                }
            }

            return lines;
        }

        private string ReturnSymbols(string line, string splitted)
        {
            int cnt = 0;
            var returnedline = line;
            foreach (var symbol in _symbols)
            {
                var replacerIsEmpty = symbol.Value.Length == 0;
                MatchCollection mc = Regex.Matches(returnedline, symbol.Key);
                if (mc.Count == 0)
                {
                    continue;
                }

                for (int i = mc.Count - 1; i >= 0; i--)
                {
                    returnedline = returnedline.Remove(mc[i].Index, mc[i].Value.Length).Insert(mc[i].Index, replacerIsEmpty ? string.Empty : "{" + symbol.Value + $"{cnt:000}" + "}");
                    cnt++;
                }
            }

            return returnedline;
        }

        private string CleanLine(string line)
        {
            int cnt = 0;
            var cleanedline = line;
            foreach (var symbol in _symbols)
            {
                var replacerIsEmpty = symbol.Value.Length == 0;
                MatchCollection mc = Regex.Matches(cleanedline, symbol.Key);
                if (mc.Count == 0)
                {
                    continue;
                }

                for (int i = mc.Count - 1; i >= 0; i--)
                {
                    cleanedline = cleanedline.Remove(mc[i].Index, mc[i].Value.Length).Insert(mc[i].Index, replacerIsEmpty ? string.Empty : "{" + symbol.Value + $"{cnt:000}" + "}");
                    cnt++;
                }
            }

            return cleanedline;
        }
    }
}
