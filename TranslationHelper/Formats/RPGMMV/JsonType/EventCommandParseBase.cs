using RPGMVJsonParser;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class EventCommandParseBase : JsonTypeBase
    {
        protected void ParseCommandStrings(Command[] commands, string info)
        {
            int commandsCount = commands.Length;
            for (int c = 0; c < commandsCount; c++)
            {
                var command = commands[c];
                int parametersCount = command.Parameters.Length;
                for (int i = 0; i < parametersCount; i++)
                {
                    if (command.Parameters[i] is string s)
                    {
                        if (AddRowData(ref s, info + $"\r\nCommand code: {command.Code}\r\n Parameter #: {i}") && ProjectData.SaveFileMode)
                        {
                            command.Parameters[i] = s;
                        }
                    }
                }
            }
        }
    }
}
