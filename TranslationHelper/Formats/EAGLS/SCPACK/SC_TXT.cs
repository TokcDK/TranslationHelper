using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.EAGLS.SCPACK
{
    class SC_TXT : SCPACKBase
    {
        private const string StringPattern = "&[0-9]{1,6}\"([^\"]+)\"";
        //#さくら=w0261a
        //#　花　=w0003f
        private const string StringPatternNames = @"#([^\=]+)\=w[0-9]{4}[a-z]";

        public SC_TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return OpenSC();
        }

        private bool OpenSC()
        {
            string fileName = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

            try
            {
                AddTables(fileName);
                string lastMentionedCharacter = string.Empty;
                using (StreamReader sr = new StreamReader(thDataWork.FilePath))
                {
                    string line;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        if (line.StartsWith("#"))
                        {
                            if (!line.Contains(":NameSuffix"))
                            {
                                //#さくら=w0261a
                                //#　花　=w0003f
                                var last = line.IndexOf('=');
                                var newline = (last != -1 ? line.Substring(1, last - 1) : line.Substring(1)).Trim();
                                lastMentionedCharacter = newline;
                                if (IsValid(newline))
                                {
                                    AddRowData(fileName, newline, "CharName", true);
                                }
                            }
                            else
                            {
                                lastMentionedCharacter = line.Remove(0, 1);
                            }
                        }
                        else if (line.Contains("&"))
                        {
                            MatchCollection matches = Regex.Matches(line, StringPattern);
                            foreach (Match match in matches)
                            {
                                var val = match.Result("$1");
                                if (IsValid(val))
                                {
                                    AddRowData(fileName, val, lastMentionedCharacter.Length > 0 ? T._("Last char: ") + lastMentionedCharacter : string.Empty, true);
                                }
                            }

                            matches = Regex.Matches(line, StringPatternNames);
                            foreach (Match match in matches)
                            {
                                var val = match.Result("$1");
                                if (IsValid(val) && !val.Contains(":NameSuffix"))
                                {
                                    AddRowData(fileName, val.Trim(), T._("CharName"), true);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return CheckTablesContent(fileName);
        }

        private static bool IsValid(string line)
        {
            return !FunctionsRomajiKana.LocalePercentIsNotValid(line);
        }

        internal override bool Save()
        {
            return SaveSC();
        }

        private bool SaveSC()
        {
            string fileName = Path.GetFileNameWithoutExtension(thDataWork.FilePath);
            SplitTableCellValuesAndTheirLinesToDictionary(fileName, false, false);
            StringBuilder Translated = new StringBuilder();
            bool changed = false;
            try
            {
                using (StreamReader sr = new StreamReader(thDataWork.FilePath))
                {
                    string line;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        if (line.StartsWith("#"))
                        {
                            if (!line.Contains(":NameSuffix"))
                            {
                                //#さくら=w0261a
                                //#　花　=w0003f
                                var last = line.IndexOf('=');
                                var val = (last != -1 ? line.Substring(1, last - 1) : line.Substring(1)).Trim();
                                if (IsValid(val))
                                {
                                    var valtrimmed = val.Trim();
                                    if (TablesLinesDict.ContainsKey(valtrimmed))
                                    {
                                        line = "#" + TablesLinesDict[valtrimmed].CleanForShiftJIS2004() + (last != -1 ? line.Substring(last) : string.Empty);
                                        if (!changed)
                                            changed = true;
                                    }
                                }
                            }
                        }
                        else if (line.Contains("&"))
                        {
                            MatchCollection matches = Regex.Matches(line, StringPattern);
                            foreach (Match match in matches)
                            {
                                var newMatch = match;
                                var val = newMatch.Result("$1");
                                if (IsValid(val))
                                {
                                    if (TablesLinesDict.ContainsKey(val))
                                    {
                                        line = line.Replace(match.Value, newMatch.Value.Replace(val, TablesLinesDict[val].CleanForShiftJIS2004()).Trim());
                                        if (!changed)
                                            changed = true;
                                    }
                                }
                            }

                            matches = Regex.Matches(line, StringPatternNames);
                            foreach (Match match in matches)
                            {
                                var newMatch = match;
                                var val = newMatch.Result("$1");
                                if (IsValid(val) && !val.Contains(":NameSuffix"))
                                {
                                    var valtrimmed = val.Trim();
                                    if (TablesLinesDict.ContainsKey(valtrimmed))
                                    {
                                        line = line.Replace(match.Value, newMatch.Value.Replace(val, TablesLinesDict[valtrimmed].CleanForShiftJIS2004()));
                                        if (!changed)
                                            changed = true;
                                    }
                                }
                            }

                        }
                        Translated.AppendLine(line);
                    }
                }
            }
            catch
            {
            }

            if (changed)
            {
                File.WriteAllText(thDataWork.FilePath, Translated.ToString());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
