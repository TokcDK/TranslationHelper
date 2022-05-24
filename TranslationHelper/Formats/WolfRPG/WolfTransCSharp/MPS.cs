using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.Shared;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class MPS:FormatBinaryBase
    {
        protected override void FileOpen()
        {
            var map = new MapParser();
            map.Read(FilePath);

            List<string> strings = new List<string>();
            foreach (var @event in map.Events)
            {
                if (@event == null) continue;

                foreach (var page in @event.Pages)
                {
                    foreach (var command in page.Commands)
                    {
                        foreach (var @string in CommandUtils.Strings_Of_Command(command))
                        {
                            AddRowData(@string);
                        }
                    }
                }
            }
        }
    }
}
