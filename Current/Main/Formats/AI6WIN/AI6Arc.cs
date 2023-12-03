using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MesArcLibCSharp;
using MesScriptDissAssLib;

namespace TranslationHelper.Formats.AI6WIN
{
    internal class AI6Arc : FormatBinaryBase
    {
        public override string Extension => ".arc";

        protected override void FileOpen()
        {
            var arcParser = new AI6WINArc(FilePath, FilePath, outToMemory: true);
            arcParser.Unpack();

            foreach (var file in arcParser.ArcFiles)
            {
                string ext = Path.GetExtension(file.FileName);
                if (!string.Equals(ext, ".mes", StringComparison.InvariantCultureIgnoreCase)) continue;

                var mesParser = new AI6WINScript(FilePath, FilePath);
                mesParser.Disassemble(file.FileBytes);

                for (int i = 0; i < mesParser.DisassembledTxt.Count; i++)
                {
                    var line = mesParser.DisassembledTxt[i];

                    if (line != "#1-STR_PRIMARY") continue;

                    var str = mesParser.DisassembledTxt[++i];
                    var strMatch = Regex.Match(str, @"\[\""(.+)\""\]");
                    if (!strMatch.Success) continue;

                    var s = strMatch.Groups[1].Value;

                    if (AddRowData(ref s) && SaveFileMode)
                    {
                        mesParser.DisassembledTxt[i] = $"[\"{CleanString(s)}\"]";
                    }
                }

                if (SaveFileMode)
                {
                    mesParser.Assemble();
                }
            }
        }

        private string CleanString(string s)
        {
            return s.Replace("'", "").Replace("\"", "");
        }
    }
}
