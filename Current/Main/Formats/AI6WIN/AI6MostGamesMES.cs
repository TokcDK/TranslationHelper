using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MesScriptDissAss;

namespace TranslationHelper.Formats.AI6WIN
{
    internal class AI6MostGamesMES : FormatBinaryBase
    {
        public override string Extension => ".mes";

        protected override void FileOpen()
        {
            var parser = new AI6WINScript(FilePath, FilePath);
            parser.Disassemble();

            for (int i=0; i < parser.DisassembledTxt.Count; i++)
            {
                var line = parser.DisassembledTxt[i];

                if (line != "#1-STR_PRIMARY") continue;

                var str = parser.DisassembledTxt[++i];
                var strMatch = Regex.Match(str, @"\[\""(.+)\""\]");
                if (!strMatch.Success) continue;

                var s = strMatch.Groups[1].Value;

                if(AddRowData(ref s) && SaveFileMode)
                {
                    parser.DisassembledTxt[i] = $"[\"{CleanString(s)}\"]";
                }
            }

            if (SaveFileMode)
            {
                parser.Assemble();
            }
        }

        private string CleanString(string s)
        {
            return s.Replace("'", "").Replace("\"", "");
        }
    }
}
