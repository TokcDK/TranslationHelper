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
        MapParser Data = null;
        protected override void FileOpen()
        {
            Data = new MapParser();
            Data.Read(FilePath);

            foreach (var @event in Data.Events)
            {
                foreach (var page in @event.Pages)
                {
                    foreach (var command in page.Commands)
                    {
                        foreach (string @string in CommandUtils.Strings_Of_Command(command))
                        {
                            var value = @string;
                            if (AddRowData(ref value, $"Event ID: {@event.ID}\r\nEvent name: {@event.Name}\r\nCommand id: {command.CID}"))
                            {
                                for(int i = 0; i < command.String_args.Count; i++)
                                {
                                    if(command.String_args[i] == @string)
                                    {
                                        command.String_args[i] = value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try { Data.Write(); } catch { return false; }
            return true;
        }
    }
}
