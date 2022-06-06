﻿using RPGMVJsonParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class EventCommandParseBase : JsonTypeBase
    {
        Dictionary<int, string> ExcludedCodes;

        public EventCommandParseBase()
        {
            ExcludedCodes = RPGMVUtils.GetSkipCodes();
        }

        protected void ParseCommandStrings(List<Command> commands, string info)
        {
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
                    if (message.Any()) { extra= ParseMessage(commands, message, info, c); c += extra; commandsCount += extra; };

                    if (ExcludedCodes.ContainsKey(command.Code)) continue;

                    int parametersCount = command.Parameters.Length;
                    for (int i = 0; i < parametersCount; i++)
                    {
                        if (command.Parameters[i] is string s)
                        {
                            if (AddRowData(ref s, info + $"\r\nCommand code: {command.Code}{RPGMVUtils.GetCodeName(command.Code)}\r\n Parameter #: {i}") && ProjectData.SaveFileMode)
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
        /// <param name="lasIndex"> last index of command</param>
        /// <returns></returns>
        private int ParseMessage(List<Command> commands, List<Command> message, string info, int lasIndex)
        {
            var s = string.Join("\r\n", message.Select(m => m.Parameters[0]));
            int extraLinesCount = 0;
            var newMessage = new List<Command>();
            if (AddRowData(ref s, info + $"\r\nCommand code: {message[0].Code}{RPGMVUtils.GetCodeName(message[0].Code)}\r\nParameter #: {0}") && ProjectData.SaveFileMode)
            {
                int lineIndex = 0;
                foreach (var line in s.SplitToLines())
                {
                    if (lineIndex < message.Count)
                    {
                        message[lineIndex].Parameters[0] = line;
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

                        commands.Insert(lasIndex++, newCommand); // insert new line as command
                        extraLinesCount++; // raise count and index
                    }
                }
            }

            message.Clear();

            return extraLinesCount;
        }
    }
}
