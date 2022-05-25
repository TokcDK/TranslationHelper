using System;
using System.Linq;
using TranslationHelper.Data;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.Shared;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class MPS : FormatBinaryBase
    {
        MapParser Data = null;
        protected override void FileOpen()
        {
            Data = new MapParser();
            Data.Read(FilePath);

            var eventsCount = Data.Events.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = Data.Events[e];
                var pagesCount = @event.Pages.Count;
                for (int p = 0; p < pagesCount; p++)
                {
                    var page = @event.Pages[p];
                    var commandsCount = page.Commands.Count;
                    for (int c = 0; c < commandsCount; c++)
                    {
                        var command = page.Commands[c];
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
                        if (ProjectData.SaveFileMode && !command.String_args.SequenceEqual(commandStrings))
                        {
                            command.String_args = commandStrings;
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
