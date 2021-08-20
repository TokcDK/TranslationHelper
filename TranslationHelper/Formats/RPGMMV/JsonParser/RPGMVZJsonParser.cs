using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    class RPGMVZJsonParser : JsonParserBase
    {
        protected override void ParseValue(JValue jsonValue)
        {
            string tokenValue = jsonValue.Value as string;

            if (!IsValidString(jsonValue, tokenValue))
            {
                AddToStats(false);
                return;
            }

            if (ProjectData.OpenFileMode)
            {
                AddToStats();

                bool HasCurCode = CurrentEventCode > -1;
                Format.AddRowData(JsonName, tokenValue, "JsonPath: "
                    + Environment.NewLine
                    + jsonValue.Path
                    + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                    HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                    //+ (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                    , true, false);
            }
            else
            {
                //предполагается, что ячейки из таблицы были разбиты на строки и добавлены в словарь
                if (/*IsTranslatableRow.ContainsKey(tokenvalue) && IsTranslatableRow[tokenvalue] tried to add check if row have translation but here also issue because no displaed rows will be translated &&*/
                    !ProjectData.TablesLinesDict.ContainsKey(tokenValue)
                    || ProjectData.TablesLinesDict[tokenValue].Length == 0
                    )
                {
                    return;
                }

                WriteTranslation(jsonValue, tokenValue);
            }
        }

        /// <summary>
        /// Write translation instead of original.
        /// Will also add extra objects with lines in json if translation has more lines.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tokenValue"></param>
        private void WriteTranslation(JValue value, string tokenValue)
        {
            try
            {
                int tokenvalueLinesCount;
                if (ProjectData.TablesLinesDict[tokenValue].GetLinesCount() == (tokenvalueLinesCount = tokenValue.GetLinesCount())
                    || value.Parent == null
                    || value.Parent.Parent == null
                    || value.Parent.Parent.Parent == null
                    || !(value.Parent.Parent.Parent is JObject currentJObject)

                    )
                {
                    value.Value = ProjectData.TablesLinesDict[tokenValue];
                }
                else
                {
                    int ind = 0;
                    List<string> stringToWrite = new List<string>();
                    bool NotWrited = true;
                    bool IsTheJObjetAdded = false;

                    foreach (var line in ProjectData.TablesLinesDict[tokenValue].SplitToLines())
                    {
                        if (ind < tokenvalueLinesCount)
                        {
                            stringToWrite.Add(line);
                            ind++;
                        }
                        else
                        {
                            if (NotWrited)
                            {
                                value.Value = string.Join(Environment.NewLine, stringToWrite);
                                NotWrited = false;
                            }

                            var newJObject = GetNewJObject(currentJObject, line);
                            currentJObject.AddAfterSelf(newJObject);
                            if (!IsTheJObjetAdded && !AddedJObjects.Contains(newJObject))
                            {
                                IsTheJObjetAdded = true;
                                AddedJObjects.Add(newJObject);
                            }
                            currentJObject = newJObject;//делать добавленный JObject текущим, чтобы новый добавлялся после него
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Корректировка формата записываемого json так, как в файлах RPGMaker MV
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private string CorrectJsonFormatToRPGMMV()
        {
            return JsonRoot.ToString(Formatting.Indented);
            //return Regex.Replace(root.ToString(Formatting.None), @"^\[null,(.+)\]$", "[\r\nnull,\r\n$1\r\n]");//regex нужен только для Formatting.None
        }

        bool IsValidString(JToken token, string value)
        {
            string path;
            return Format.IsValidString(value)
                && !((path = token.Path).Contains("].name") && !value.Contains("Enemy(")/*constructions like Enemy(enemyname) using also to identify enemies*/ && Regex.IsMatch(path, @"events\[[0-9]+\]\.name")) // skip event names //disabled because using in some cases like enemy name as event name(need to make dull open save with duplicates for correct translation)
                && !(path.Contains("].image") && Regex.IsMatch(path, @"\[[0-9]+\]\.image")) // skip image names
                && !(path.Contains("gm.name") && Regex.IsMatch(token.Path, @"[Bb]gm\.name")); // skip bgm
        }

        HashSet<JObject> AddedJObjects = new HashSet<JObject>();

        protected override void ParseJsonObject(JObject jsonObject)
        {
            if (AddedJObjects.Contains(jsonObject))
            {
                return;
            }

            ParseJsonObjectProperties(jsonObject);

            ResetCurrentCode();
        }

        /// <summary>
        /// reset current code if it is was exist
        /// </summary>
        private void ResetCurrentCode()
        {
            if (CurrentEventCode != -1)
            {
                CurrentEventCode = -1;
            }
        }

        protected override JsonObjectPropertyState ParseJsonObjectProperty(JProperty jsonProperty)
        {
            bool IsCode = IsInteger(jsonProperty.Value.Type) && jsonProperty.Name == "code";
            if (IsCode)
            {
                CurrentEventCode = (int)jsonProperty.Value;
            }

            if (IsExcludedOrParsed(jsonProperty.Parent as JObject, CurrentEventCode, JsonName))
            {
                return JsonObjectPropertyState.Break; // skip all rest properties parse
            }

            return base.ParseJsonObjectProperty(jsonProperty);
        }

        private bool IsExcludedOrParsed(JObject jsonObject, int currentEventCode, string JsonName)
        {
            if (IsExcludedCode(currentEventCode)) // code always located in the objects and dont need do as code below
            {
                AddToStats(false);
                return true;
            }
            else
            if (IsMessageCode(CurrentEventCode)) // идея при записи брать сразу всё сообщение, брать перевод для него и переводить, потом пропускать объекты с частями переведенного сообщения
            {
                var messageparts = GetNextTokensWithSameCode(jsonObject);
                var fullmessage = GetMessageLinesFrom(messageparts);

                if (ProjectData.OpenFileMode)
                {
                    bool HasCurCode = true; // message code parse
                    Format.AddRowData(tablename: JsonName, RowData: fullmessage, RowInfo: "JsonPath: "
                        + Environment.NewLine
                        + jsonObject.Path
                        + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                        HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                        //+ (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                        , CheckAddHashes: true, CheckInput: true);

                    AddToStats();
                }
                else
                {
                    TranslateMessages(messageparts, fullmessage);
                }

                return true;

                //TranslateMessages(messageparts, fullmessage);
            }
            else
            if (IsCodeWithStringInParameters(currentEventCode) && jsonObject.Last is JProperty lastObjectsProperty) // in message codes need only last property's "parameters" value
            {
                Parse(lastObjectsProperty.Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The code event has strings in parameters
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private bool IsCodeWithStringInParameters(int currentEventCode)
        {
            return //IsMessageCode(currentEventCode) || 
                currentEventCode == 102
                || currentEventCode == 402
                || currentEventCode == 118
                || currentEventCode == 119;
        }

        /// <summary>
        /// Multiline message
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private static bool IsMessageCode(int currentEventCode)
        {
            return currentEventCode == 401 || currentEventCode == 405;
        }

        /// <summary>
        /// get all string lines of message from input <paramref name="messagePartsList"/>
        /// </summary>
        /// <param name="messagePartsList"></param>
        /// <returns></returns>
        private static string GetMessageLinesFrom(List<JToken> messagePartsList)
        {
            List<string> lines = new List<string>(messagePartsList.Count);
            foreach (JToken token in messagePartsList)
            {
                JProperty prop = (token as JObject).Last as JProperty;
                JArray array = prop.Value as JArray;
                foreach (string value in array)
                {
                    lines.Add(value);
                }
            }

            return string.Join("\n", lines);
        }

        /// <summary>
        /// Add all next JTokens with same message code to list
        /// </summary>
        /// <param name="jsonToken"></param>
        /// <returns></returns>
        private List<JToken> GetNextTokensWithSameCode(JToken jsonToken)
        {
            List<JToken> list = new List<JToken>
            {
                jsonToken
            };

            var next = jsonToken.Next;
            while (next is JObject obj
                && obj.First is JProperty prop
                && prop.Name == "code"
                && (int)prop.Value == CurrentEventCode
                )
            {
                AddedJObjects.AddTry(obj);
                list.Add(next);
                next = next.Next;
            }

            return list;
        }

        /// <summary>
        /// Translate originalMergedMessage
        /// </summary>
        /// <param name="originalMessageJTokensList"></param>
        /// <param name="originalMergedMessage"></param>
        private void TranslateMessages(List<JToken> originalMessageJTokensList, string originalMergedMessage)
        {
            if (!Format.IsValidString(originalMergedMessage))
            {
                return;
            }

            if (!ProjectData.TablesLinesDict.ContainsKey(originalMergedMessage))
            {
                return;
            }

            var translated = ProjectData.TablesLinesDict[originalMergedMessage].SplitToLines().ToArray();

            var origLength = originalMessageJTokensList.Count;
            JToken last = null;
            for (int i = 0; i < translated.Length; i++)
            {
                if (i < origLength)
                {
                    JObject obj = originalMessageJTokensList[i] as JObject;
                    JProperty prop = obj.Last as JProperty;
                    JArray array = prop.Value as JArray;
                    array[0] = translated[i];

                    AddedJObjects.AddTry(obj);
                    last = obj;
                }
                else
                {
                    var newJObject = GetNewJObject(last as JObject, translated[i]);
                    last.AddAfterSelf(newJObject);
                    AddedJObjects.AddTry(newJObject);

                    last = last.Next;
                }
            }

        }

        /// <summary>
        /// copy all objects from input jContainer in new JObject and replace parameters value to new
        /// </summary>
        /// <param name="jContainer">input JContainer</param>
        /// <param name="line">line to which replace parameters value</param>
        /// <returns></returns>
        private static JObject GetNewJObject(JContainer jContainer, string line)
        {
            JObject newJObject = new JObject();
            foreach (var jProperty in (jContainer as JObject).Properties())
            {
                switch (jProperty.Name)
                {
                    case "parameters":
                        if (jProperty.Value is JValue)
                        {
                            newJObject.Add(new JProperty(jProperty.Name, new JValue(line)));
                        }
                        else if (jProperty.Value is JObject)
                        {
                            newJObject.Add(new JProperty(jProperty.Name, new JObject("{" + line + "}")));
                        }
                        else if (jProperty.Value is JArray)
                        {
                            newJObject.Add(new JProperty(jProperty.Name, new JArray(new object[] { line })));
                        }
                        break;
                    default:
                        {
                            if (jProperty.Value is JValue jValue)
                            {
                                newJObject.Add(new JProperty(jProperty.Name, new JValue(jValue)));
                            }
                            else if (jProperty.Value is JObject jObject)
                            {
                                newJObject.Add(new JProperty(jProperty.Name, new JObject(jObject)));
                            }
                            else if (jProperty.Value is JArray jArray)
                            {
                                newJObject.Add(new JProperty(jProperty.Name, new JArray(jArray)));
                            }

                            break;
                        }
                }
            }

            return newJObject;
        }

        /// <summary>
        /// returns event name if found
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private string GetCodeName(int currentEventCode)
        {
            if (EventCodes.ContainsKey(currentEventCode))
            {
                var eventName = EventCodes[currentEventCode];
                if (eventName.Length > 0)
                {
                    return "\r\nEvent=\"" + eventName + "\"";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// String for this codes must not be appeared
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private static bool IsExcludedCode(int currentEventCode)
        {
            return ExcludedCodes.ContainsKey(currentEventCode); /*currentEventCode.Length == 3 && (currentEventCode == 108 || currentEventCode == 408 || currentEventCode == 356)*/;
        }

        /// <summary>
        /// true if type is JTokenType.Integer
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsInteger(JTokenType type)
        {
            return type == JTokenType.Integer;
        }

        /// <summary>
        /// Event code if exists
        /// </summary>
        private int CurrentEventCode = -1;

        /// <summary>
        /// for statistics and json open\save improvement purpose.
        /// </summary>
        /// <param name="add">for added strings else for skipped</param>
        private void AddToStats(bool add = true)
        {
            var dict = add ? ProjectData.RpgMVAddedCodesStat : ProjectData.RpgMVSkippedCodesStat;
            if (CurrentEventCode > -1)
            {
                if (!dict.ContainsKey(CurrentEventCode))
                {
                    dict.Add(CurrentEventCode, 1);
                }
                else
                {
                    dict[CurrentEventCode]++;
                }
            }
        }

        /// <summary>
        /// list of event codes
        /// </summary>
        readonly Dictionary<int, string> EventCodes = new Dictionary<int, string>(120)
        {
            { 0, "End Show Choices" },
            { 41, "Image name?" },
            { 101, "Show Text" },
            { 102, "Show Choices" },
            { 103, "Input Number" },
            { 104, "Select Item" },
            { 105, "Show Scrolling Text" },
            { 108, "Comment" },
            { 111, "Conditional Branch" },
            { 112, "Loop" },
            { 113, "Break Loop" },
            { 115, "Exit Event Processing" },
            { 117, "Common Event" },
            { 118, "Label" },
            { 119, "Jump to Label" },
            { 121, "Control Switches" },
            { 122, "Control Variables" },
            { 123, "Control Self Switch" },
            { 124, "Control Timer" },
            { 125, "Change Gold" },
            { 126, "Change Items" },
            { 127, "Change Weapons" },
            { 128, "Change Armor" },
            { 129, "Change Party Member" },
            { 132, "Change Battle BGM" },
            { 133, "Change Battle End ME" },
            { 134, "Change Save Access" },
            { 135, "Change Menu Access" },
            { 136, "Change Encounter Disable" },
            { 137, "Change Formation Access" },
            { 138, "Change Window Color" },
            { 139, "Change Defeat ME" },
            { 140, "Change Vehicle BGM" },
            { 201, "Transfer Player" },
            { 202, "Set Vehicle Location" },
            { 203, "Set Event Location" },
            { 204, "Scroll Map" },
            { 205, "Set Move Route" },
            { 206, "Getting On and Off Vehicles" },
            { 211, "Change Transparency" },
            { 212, "Show Animation" },
            { 213, "Show Balloon Icon" },
            { 214, "Temporarily Erase Event" },
            { 216, "Change Player Followers" },
            { 217, "Gather Followers" },
            { 221, "Fadeout Screen" },
            { 222, "Fadein Screen" },
            { 223, "Tint Screen" },
            { 224, "Screen Flash" },
            { 225, "Screen Shake" },
            { 230, "Wait" },
            { 231, "Show Picture" },
            { 232, "Move Picture" },
            { 233, "Rotate Picture" },
            { 234, "Tint Picture" },
            { 235, "Erase Picture" },
            { 236, "Set Weather" },
            { 241, "Play BGM" },
            { 242, "Fadeout BGM" },
            { 243, "Save BGM" },
            { 244, "Resume BGM" },
            { 245, "Play BGS" },
            { 246, "Fadeout BGS" },
            { 249, "Play ME" },
            { 250, "Play SE" },
            { 251, "Stop SE" },
            { 261, "Play Movie" },
            { 281, "Change Map Name Display" },
            { 282, "Change Tileset" },
            { 283, "Change Battle Background" },
            { 284, "Change Parallax Background" },
            { 285, "Get Location Info" },
            { 301, "Battle Processing" },
            { 302, "Shop Processing" },
            { 303, "Name Input Processing" },
            { 311, "Change HP" },
            { 312, "Change MP" },
            { 313, "Change State" },
            { 314, "Recover All" },
            { 315, "Change EXP" },
            { 316, "Change Level" },
            { 317, "Change Parameters" },
            { 318, "Change Skills" },
            { 319, "Change Equipment" },
            { 320, "Change Name" },
            { 321, "Change Class" },
            { 322, "Change Actor Graphic" },
            { 323, "Change Vehicle Graphic" },
            { 324, "Change Nickname" },
            { 325, "Change Profile" },
            { 326, "Change TP" },
            { 331, "Change Enemy HP" },
            { 332, "Change Enemy MP" },
            { 333, "Change Enemy State" },
            { 334, "Enemy Recover All" },
            { 335, "Enemy Appear" },
            { 336, "Enemy Transform" },
            { 337, "Show Battle Animation" },
            { 339, "Force Action" },
            { 340, "Abort Battle" },
            { 342, "Change Enemy TP" },
            { 351, "Open Menu Screen" },
            { 352, "Open Save Screen" },
            { 353, "Game Over" },
            { 354, "Return to Title Screen" },
            { 355, "Script" },
            { 356, "Plugin" },
            { 401, "Show Text" },
            { 402, "When [**] Choice" },
            { 403, "When Cancel" },
            { 405, "Show Text" },
            { 408, "Comment" },
            { 411, "Else" },
            { 412, "End Conditional Branch" },
            { 413, "Repeat Above" },
            { 601, "If Win" },
            { 602, "If Escape" },
            { 603, "If Lose" },
            { 604, "End Battle Result" },
            { 655, "Script" },
        };

        /// <summary>
        /// codes which must be skipped
        /// </summary>
        static readonly Dictionary<int, string> ExcludedCodes = new Dictionary<int, string>(19)
        {
            { 41, "Image name?" }, // file name
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
            //{ 108, "Comment" }, // not to skip in some games
            //{ 408, "Comment" }, // not to skip in some games
        };
    }
}
