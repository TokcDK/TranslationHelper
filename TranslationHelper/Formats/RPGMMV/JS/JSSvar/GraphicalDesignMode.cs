//just usual quoted js
#if false

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS.JSSvar
{
    class GraphicalDesignMode : JSSVarBase
    {
        public GraphicalDesignMode()
        {
        }

        internal override bool Open()
        {
            return ParseJSVarInJsonGDM("var settings = {");
        }

        protected bool ParseJSVarInJsonGDM(string SvarIdentifier)
        {
            string line;

            string tablename = Path.GetFileName(ProjectData.FilePath);

            AddTables(tablename);

            bool StartReadingSvar = false;
            bool IsComment = false;
            StringBuilder Svar = new StringBuilder();
            using (StreamReader reader = new StreamReader(ProjectData.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (StartReadingSvar)
                    {
                        if (line.TrimStart().StartsWith("{lines: ['"))
                        {
                            SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                            try
                            {
                                ReadExistStrings(tablename, line);
                            }
                            catch
                            {
                            }

                        }
                        else if (line.TrimStart().StartsWith("};"))
                        {
                            break;
                        }
                    }
                    else
                    {
                        //comments
                        if (IsComment)
                        {
                            if (line.Contains("*/"))
                            {
                                IsComment = false;
                            }
                            continue;
                        }
                        else if (line.TrimStart().StartsWith("//"))
                        {
                            continue;
                        }
                        else if (line.TrimStart().StartsWith("/*"))
                        {
                            if (!line.Contains("*/"))
                            {
                                IsComment = true;
                                continue;
                            }
                        }//endcomments
                        else if (line.TrimStart().StartsWith(SvarIdentifier))
                        {
                            StartReadingSvar = true;
                            Svar.AppendLine("{");
                        }
                    }
                }
            }

            return CheckTablesContent(tablename);
        }

        private void ReadExistStrings(string Jsonname, string line)
        {
            //                {lines: ['気になる人　\\v[48]','\\v[49]    \\v[53]\\I[155]'], switchId: 0},
            //                {lines: ['\\I[248]絆\\v[51]  \\I[7]思い出\\v[56]'], switchId: 0},
            //                {lines: ['\\I[247]運動\\v[50]  \\I[79]知識\\v[52]  \\I[87]生活\\v[55]'], switchId: 0},

            MatchCollection mc = Regex.Matches(line, @"'([^']+)'");
            foreach (Match m in mc)
            {
                var v = m.Value.Trim('\'');

                AddRowData(Jsonname, v, string.Empty, true);
            }
        }

        internal override bool Save()
        {
            return ParseJSVarInJsonWriteGDM("var settings = {");
            //return ParseJSVarInJsonWrite("var settings = {");
        }

        protected bool ParseJSVarInJsonWriteGDM(string SvarIdentifier)
        {
            StringBuilder TranslatedResult = new StringBuilder();
            string line;
            //rowindex = 0;

            string tablename = Path.GetFileName(ProjectData.FilePath);

            bool StartReadingSvar = false;
            bool IsComment = false;
            //StringBuilder Svar = new StringBuilder();
            using (StreamReader reader = new StreamReader(ProjectData.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (StartReadingSvar)
                    {
                        if (line.TrimStart().StartsWith("{lines: ['"))
                        {
                            SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                            try
                            {
                                TranslateExistStrings(ref line);
                            }
                            catch
                            {
                            }

                        }
                        else if (line.TrimStart().StartsWith("};"))
                        {
                            StartReadingSvar = false;
                        }
                    }
                    else
                    {
                        //comments
                        if (IsComment)
                        {
                            if (line.Contains("*/"))
                            {
                                IsComment = false;
                            }
                            //continue;
                        }
                        else if (line.TrimStart().StartsWith("//"))
                        {
                            //continue;
                        }
                        else if (line.TrimStart().StartsWith("/*"))
                        {
                            if (!line.Contains("*/"))
                            {
                                IsComment = true;
                                //continue;
                            }
                        }//endcomments
                        else if (line.TrimStart().StartsWith(SvarIdentifier))
                        {
                            StartReadingSvar = true;
                        }
                    }

                    TranslatedResult.AppendLine(line);
                }
            }

            try
            {
                File.WriteAllText(ProjectData.FilePath, TranslatedResult.ToString());
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void TranslateExistStrings(ref string line)
        {
            //                {lines: ['気になる人　\\v[48]','\\v[49]    \\v[53]\\I[155]'], switchId: 0},
            //                {lines: ['\\I[248]絆\\v[51]  \\I[7]思い出\\v[56]'], switchId: 0},
            //                {lines: ['\\I[247]運動\\v[50]  \\I[79]知識\\v[52]  \\I[87]生活\\v[55]'], switchId: 0},

            MatchCollection mc = Regex.Matches(line, @"'([^']+)'");

            for (int i = mc.Count; i >= 0; i--)
            {
                var v = mc[i];//.Value.Trim('\'');
                if (TablesLinesDict.ContainsKey(v))
                {
                    line = line.Remove(i = line.IndexOf(v), v.Length).Insert(i, TablesLinesDict[v]);
                }
            }
        }

        internal override string JSName => "GraphicalDesignMode.js";

        protected override string SvarIdentifier => "{lines: ['";
    }
}
#endif