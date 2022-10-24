using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using Command = RPGMVJsonParser.Command;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal interface IParametersParser
    {
        bool Parse(ParametersData parametersData);
    }

    internal class SoR_GabWindow : IParametersParser
    {
        public bool Parse(ParametersData parametersData)
        {
            if (parametersData.ParentCommand.Parameters.Length != 4) return false;
            if (!(parametersData.ParentCommand.Parameters[1] is string s)) return false;
            if (!string.Equals(s, "PushGab")) return false;
            if (!(parametersData.ParentCommand.Parameters[3] is JToken t)) return false;

            var parameterdata = new JParameterData
            {
                Token = t,
                ParameterIndex = parametersData.ParameterIndex,
                ParentCommand = parametersData.ParentCommand,
                ParentInfo = parametersData.ParentInfo
            };

            try
            {
                parametersData.Parser.ParseJToken(parameterdata);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    internal class ParametersData
    {
        public int ParameterIndex;
        public Command ParentCommand;
        public string ParentInfo;

        public EventCommandParseBase Parser;
    }

    internal class JParameterData
    {
        public JToken Token;
        public int ParameterIndex;
        public Command ParentCommand;
        public string ParentInfo;
    }

    internal class EventCommandParseBase : JsonTypeBase
    {
        static Dictionary<int, string> ExcludedCodes { get => RPGMVLists.ExcludedCodes; set => RPGMVLists.ExcludedCodes = value; }

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
                    if (message.Count > 0) { extra = ParseMessage(commands, message, info, c); c += extra; commandsCount += extra; };

                    if (ExcludedCodes.ContainsKey(command.Code)) continue;
                    if (command.Parameters == null) continue;

                    // try parse by parameters parsers
                    bool isParsedByParametersParser = false;
                    var parametersParsers = new List<IParametersParser>
                    {
                        new SoR_GabWindow(),
                    };
                    var data = new ParametersData
                    {
                        ParameterIndex = -1,
                        ParentCommand = command,
                        Parser = this
                    };
                    foreach (var parser in parametersParsers)
                    {
                        if (!parser.Parse(data)) continue;

                        isParsedByParametersParser = true;
                        break;
                    }
                    if (isParsedByParametersParser) continue;

                    // general parse
                    int parametersCount = command.Parameters.Length;
                    for (int i = 0; i < parametersCount; i++)
                    {
                        if (command.Parameters[i] is string s)
                        {
                            if (AddRowData(ref s, isSave ? "" : info + $"\r\nCommand code: {command.Code}{RPGMVUtils.GetCodeName(command.Code)}\r\n Parameter #: {i}") && SaveFileMode)
                            {
                                command.Parameters[i] = s;
                            }
                        }
                        else if (command.Parameters[i] is JToken t)
                        {
                            var parameterdata = new JParameterData
                            {
                                Token = t,
                                ParameterIndex = i,
                                ParentCommand = command,
                                ParentInfo = info
                            };

                            ParseJToken(parameterdata);
                        }
                    }
                }
            }

            if (message.Count > 0) ParseMessage(commands, message, info, commandsCount);
        }

        internal void ParseJToken(JParameterData data)
        {
            var jsonToken = data.Token;

            if (jsonToken == null) return;

            switch (jsonToken)
            {
                case JValue value:
                    {
                        if (!(value.Value is string s)) break;

                        // add string in open mode and continue
                        if (!AddRowData(ref s, SaveFileMode ? "" : data.ParentInfo + $"\r\nCommand code: {data.ParentCommand.Code}{RPGMVUtils.GetCodeName(data.ParentCommand.Code)}\r\n Parameter #: {data.ParameterIndex}") || !SaveFileMode) break;

                        // set translation in save mode
                        value.Value = s;

                        break;
                    }

                case JObject jsonObject:
                    {
                        foreach (var property in jsonObject.Properties())
                        {
                            data.Token = property.Value;
                            ParseJToken(data);
                        }
                        break;
                    }

                case JArray jsonArray:
                    {
                        int count = jsonArray.Count;
                        for (int aIndex = 0; aIndex < count; aIndex++)
                        {
                            data.Token = jsonArray[aIndex];
                            ParseJToken(data);
                        }

                        break;
                    }

                default:
                    break;
            }
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
