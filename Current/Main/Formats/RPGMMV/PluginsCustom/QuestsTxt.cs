using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranslationHelper.Formats.RPGMMV.PluginsCustom
{
    internal class QuestsTxt : FormatTxtFileBase
    {
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.Line.StartsWith("<quest"))
            {
                var questData = new List<string> { ParseData.Line };
                bool questDataClosed = false;
                while (ReadLine() != null && (questDataClosed = ParseData.Line != "</quest>"))
                {
                    questData.Add(ParseData.Line);
                }

                if (!questDataClosed) InvalidFormat();

                var questNameMatch = Regex.Match(questData[0], @"<quest ([0-9]+):([^\|]+)\|[^\|]*\|[0-9]+>");
                if (!questNameMatch.Success) InvalidFormat();
                var questNumber = questNameMatch.Groups[1].Value;
                var questNameGroup = questNameMatch.Groups[2];
                var questName = questNameGroup.Value;
                var questDescription = questData.Count == 1 ? "" : string.Join("\r\n", questData.Skip(1));

                bool isTranslated = false;
                bool isName = true;
                foreach((string sValue, string type) in new[] { (questName,$"Quest {questNumber} Name"), (questDescription, $"Quest {questNumber} Description") })
                {
                    var s = sValue;

                    if(AddRowData(ref s, type) && SaveFileMode)
                    {
                        isTranslated = true;
                        if (isName)
                        {
                            questName = questNameMatch.Value
                                .Remove(questNameGroup.Index, questNameGroup.Length)
                                .Insert(questNameGroup.Index, s);
                        }
                        else
                        {
                            questDescription = s;
                        }
                    }

                    isName = false;
                }

                if (SaveFileMode)
                {
                    if (isTranslated)
                    {
                        ParseData.Line = questName + "\r\n" + questDescription + ParseData.Line;
                    }
                    else
                    {
                        ParseData.Line = string.Join("\r\n", questData) + ParseData.Line;
                    }
                }

            }

            SaveModeAddLine();
            return KeywordActionAfter.Continue;
        }

        private void InvalidFormat()
        {
            throw new FormatException("Invalid file format!");
        }
    }
}
