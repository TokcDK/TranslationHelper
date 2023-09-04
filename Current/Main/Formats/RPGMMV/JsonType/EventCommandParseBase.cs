using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        static Dictionary<int, string> SkipCodes { get => RPGMVLists.ExcludedCodes; set => RPGMVLists.ExcludedCodes = value; }

        static int SkipCodesCount = -1;
        public EventCommandParseBase()
        {
            if (AppData.CurrentProject == null) return;

            var excludedCodesCount = SkipCodes.Count;
            if (SkipCodesCount != excludedCodesCount)
            {
                SkipCodesCount = excludedCodesCount;
                RPGMUtils.GetSkipCodes(SkipCodes);
            }
        }

        readonly List<IParametersParser> ParametersParsers = new List<IParametersParser>
        {
            new SoR_GabWindow(),
        };

        readonly string[] _quotesList = new[] { "'", "`" };
        readonly string[] _commentMark = new[] { "//" };

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

                    if (SkipCodes.ContainsKey(command.Code)) continue;
                    if (command.Parameters == null) continue;
                    if (command.Parameters.Length == 0) continue;

                    // try parse by parameters parsers
                    bool isParsedByParametersParser = false;
                    var data = new ParametersData
                    {
                        ParameterIndex = -1,
                        ParentCommand = command,
                        Parser = this
                    };
                    foreach (var parser in ParametersParsers)
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
                            var isScriptCommand = command.Code == 355 
                                || command.Code == 655 
                                || command.Code == 356
                                || command.Code == 656;
                            var commentStr = "";
                            if (isScriptCommand)
                            {
                                var comment = s.IndexOf("//");
                                if (comment != -1)
                                {
                                    commentStr = s.Substring(comment);
                                    s = s.Remove(comment);
                                }
                            }

                            var commentInfo = isScriptCommand && string.IsNullOrWhiteSpace(commentStr) ? "" : $"\r\nComment:{commentStr}";

                            if (isScriptCommand && _quotesList.Any(q=>s.Contains(q)))
                            {
                                bool isChangedCommandString = false;
                                foreach (var regexQuote in _quotesList)
                                {
                                    // remove comment // area and get matches
                                    var lineNoComment = s.Split(_commentMark, System.StringSplitOptions.None)[0];
                                    var mc = Regex.Matches(lineNoComment, AppMethods.GetRegexQuotesCapturePattern(regexQuote));
                                    for (int m = mc.Count - 1; m >= 0; m--) // negative because lenght of string will be changing
                                    {
                                        var match = mc[m];

                                        var result = match.Groups[1].Value;

                                        if (!IsValidString(result)) continue;

                                        if (OpenFileMode)
                                        {
                                            AddRowData(result, isSave ? "" : info + $"\r\nCommand code: {command.Code}{RPGMUtils.GetCodeName(command.Code)}\r\n Parameter #: {i}{commentInfo}" + "\r\nOriginal line:" + s, isCheckInput: false);
                                        }
                                        else
                                        {
                                            string translation = result;
                                            if (!SetTranslation(ref translation)) continue;

                                            isChangedCommandString = true;

                                            //1111 abc "aaa"
                                            int index = match.Value.IndexOf(result); // get internal index of result
                                            s = s
                                                .Remove(index = match.Index + index/*remove only original string*/, result.Length)
                                                .Insert(index, translation); // paste translation on place of original

                                            ParseData.Ret = true;
                                        }
                                    }
                                }

                                if (isChangedCommandString)
                                {
                                    s = s + commentStr;

                                    command.Parameters[i] = s;
                                }
                            }
                            else
                            {
                                if (AddRowData(ref s, isSave ? "" : info + $"\r\nCommand code: {command.Code}{RPGMUtils.GetCodeName(command.Code)}\r\n Parameter #: {i}{commentInfo}") && SaveFileMode)
                                {
                                    if (isScriptCommand) s = s + commentStr;

                                    command.Parameters[i] = s;
                                }
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
                        if (!AddRowData(ref s, SaveFileMode ? "" : data.ParentInfo + $"\r\nCommand code: {data.ParentCommand.Code}{RPGMUtils.GetCodeName(data.ParentCommand.Code)}\r\n Parameter #: {data.ParameterIndex}") || !SaveFileMode) break;

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
            if (AddRowData(ref s, SaveFileMode ? "" : info + $"\r\nCommand code: {message[0].Code}{RPGMUtils.GetCodeName(message[0].Code)}\r\nParameter #: {0}") && SaveFileMode)
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
