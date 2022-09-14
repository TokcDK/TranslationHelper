using Newtonsoft.Json.Linq;
using RPGMVJsonParser;
using System.Collections.Generic;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class EventCommandParseBase : JsonTypeBase
    {
        /// <summary>
        /// codes which must be skipped
        /// </summary>
        static Dictionary<int, string> ExcludedCodes = new Dictionary<int, string>()
        {
            { 41, "Image name?" }, // file name
            { 44, "" }, // file name
            { 45, "" }, // file name
            { 118, "" }, // file name
            //{ 122, "" }, // file name
            { 123, "" }, // file name
            { 132, "" }, // file name
            { 231, "Show Picture" }, // file name
            { 232, "Move Picture" }, // file name
            { 233, "Rotate Picture" }, // file name
            { 234, "Tint Picture" }, // file name
            { 235, "Erase Picture" }, // file name
            { 236, "Set Weather" },
            { 241, "Play BGM" }, // file name
            { 242, "Fadeout BGM" }, // file name
            { 243, "Save BGM" }, // file name
            { 244, "Resume BGM" }, // file name
            { 245, "Play BGS" }, // file name
            { 246, "Fadeout BGS" }, // file name
            { 249, "Play ME" }, // file name
            { 250, "Play SE" }, // file name
            { 251, "Stop SE" }, // file name
            { 261, "Play Movie" }, // file name
            { 283, "" }, // file name
            { 320, "" }, // file name
            { 322, "" }, // file name
            { 323, "" }, // file name
            { 355, "" }, // file name
            { 356, "" }, // file name
            { 357, "Plugin command" }, // file name
            { 302, "" }, // file name
            { 405, "" }, // file name
            { 657, "Plugin command" }, // file name
            //{ 108, "Comment" }, // not to skip in some games
            //{ 408, "Comment" }, // not to skip in some games
        };

        static int SkipCodesCount = -1;
        public EventCommandParseBase()
        {
            if (AppData.CurrentProject == null) return;

            var excludedCodesCount = ExcludedCodes.Count;
            if (SkipCodesCount != excludedCodesCount)
            {
                SkipCodesCount = excludedCodesCount;
                RPGMVUtils.GetSkipCodes(ExcludedCodes);
            }
        }

        protected void ParseCommandStrings(List<Command> commands, string info)
        {
            bool isSave = SaveFileMode;
            int commandsCount = commands.Count;
            var message = new List<Command>();
            for (int c = 0; c < commandsCount; c++)
            {
                var command = commands[c];

                if (command.Code == 401)
                {
                    message.Add(command);
                    continue;
                }
                else
                {
                    int extra;
                    if (message.Any()) { extra = ParseMessage(commands, message, info, c); c += extra; commandsCount += extra; };

                    if (ExcludedCodes.ContainsKey(command.Code)) continue;
                    if (command.Parameters == null) continue;

                    int parametersCount = command.Parameters.Length;
                    for (int i = 0; i < parametersCount; i++)
                    {
                        if (command.Parameters[i] is JArray a)
                        {
                            int count = a.Count;
                            for (int i1 = 0; i1 < count; i1++)
                            {
                                if (a[i1] is JValue v) { } else continue;
                                if (v.Value is string s) { } else continue;

                                if (AddRowData(ref s, isSave ? "" : info + $"\r\nCommand code: {command.Code}{RPGMVUtils.GetCodeName(command.Code)}\r\n Parameter #: {i}") && SaveFileMode)
                                {
                                    v.Value = s;
                                }
                            }
                        }
                        else if (command.Parameters[i] is string s)
                        {
                            if (AddRowData(ref s, isSave ? "" : info + $"\r\nCommand code: {command.Code}{RPGMVUtils.GetCodeName(command.Code)}\r\n Parameter #: {i}") && SaveFileMode)
                            {
                                command.Parameters[i] = s;
                            }
                        }
                    }
                }

            }

            if (message.Any()) ParseMessage(commands, message, info, commandsCount);
        }

        /// <summary>
        /// parse message for command code 401
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="message"></param>
        /// <param name="info"></param>
        /// <param name="lastIndex"> last index of command</param>
        /// <returns></returns>
        private int ParseMessage(List<Command> commands, List<Command> message, string info, int lastIndex)
        {
            var s = string.Join("\r\n", message.Select(m => m.Parameters[0]));
            int extraLinesCount = 0;
            var newMessage = new List<Command>();
            if (AddRowData(ref s, SaveFileMode ? "" : info + $"\r\nCommand code: {message[0].Code}{RPGMVUtils.GetCodeName(message[0].Code)}\r\nParameter #: {0}") && SaveFileMode)
            {
                int lineIndex = 0;
                foreach (var line in s.SplitToLines())
                {
                    if (lineIndex < message.Count)
                    {
                        message[lineIndex++].Parameters[0] = line;
                    }
                    else
                    {
                        var newCommand = new Command
                        {
                            Code = message[0].Code,
                            Indent = message[0].Indent,
                            Parameters = new object[message[0].Parameters.Length]
                        };
                        int i = 0;
                        foreach (var o in message[0].Parameters) newCommand.Parameters[i++] = o;
                        newCommand.Parameters[0] = line;

                        commands.Insert(lastIndex++, newCommand); // insert new line as command
                        extraLinesCount++; // raise count and index
                    }
                }
            }

            message.Clear();

            return extraLinesCount;
        }
    }
}
