using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using WolfTrans.Net.Parsers.Shared;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal abstract class CommandUserBase: WolftransCSharpBase
    {
        protected void ParseCommandStrings(List<Command> commands, string info)
        {
            var commandsCount = commands.Count;
            for (int c = 0; c < commandsCount; c++)
            {
                var command = commands[c];

                foreach (var arg in command.GetText())
                {
                    var value = arg.Value;
                    if (AddRowData(ref value, info + $"\r\nCommand id: {command.CID}\r\nCommand name: {command.GetType().Name}\r\nString index: {command.String_args.IndexOf(arg)}") && ProjectData.SaveFileMode)
                    {
                        command.SetText(arg, value);
                    }
                }
            }
        }
    }
}
