using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT = WolfTrans.Net;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.CommonEvents;
using WolfTrans.Net.Parsers.Shared;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class CommonEvents:FormatBinaryBase
    {
        CommonEventsParser Data = null;

        protected override void FileOpen()
        {
            Data = new CommonEventsParser();
            Data.Read(FilePath);

            var eventsCount = Data.Events.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = Data.Events[e];
                //var patch_filename = $"dump/common/{@event.ID}_{@event.Name}.txt";

                var commandsCount = @event.Commands.Count;
                for (int c = 0; c < commandsCount; c++)
                {
                    var command = @event.Commands[c];
                    var commandStrings = command.String_args;
                    foreach (var @string in CommandUtils.Strings_Of_Command(command))
                    {
                        var value = @string;
                        if (AddRowData(ref value, $"Event ID: {@event.ID}\r\nEvent name: {@event.Name}\r\nCommand id: {command.CID}") && ProjectData.SaveFileMode)
                        {
                            for (int i = 0; i < commandStrings.Count; i++)
                            {
                                if (commandStrings[i] == @string)
                                {
                                    commandStrings[i] = value;
                                }
                            }                            
                        }
                    }
                    if(ProjectData.SaveFileMode && !command.String_args.SequenceEqual(commandStrings))
                    {
                        command.String_args = commandStrings;
                    }
                }
            }
        }

        protected override bool WriteFileData(string filePath = "")
        {
            if (!ParseData.Ret) return false;

            try { Data.Write(); } catch { return false; }
            return true;
        }
    }
}
