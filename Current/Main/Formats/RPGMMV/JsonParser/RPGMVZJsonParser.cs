﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    class RPGMVZJsonParser : JsonParserBase
    {
        public RPGMVZJsonParser(FormatBase format) : base(format)
        {
        }

        static int SkipCodesCount = -1;
        protected override void Init()
        {
            var excludedCodesCount = ExcludedCodes.Count;
            if (SkipCodesCount != excludedCodesCount)
            {
                SkipCodesCount = excludedCodesCount;
                RPGMUtils.GetSkipCodes(ExcludedCodes);
            }

            base.Init();
        }

        protected override void ParseValue(JValue jsonValue)
        {

            string tokenValue = jsonValue.Value as string;

            if (!IsValidString(jsonValue, tokenValue))
            {
                AddToStats(false);
                return;
            }

            if (Format.OpenFileMode)
            {
                AddToStats();

                bool HasCurCode = CurrentEventCode > -1;
                Format.AddRowData(tablename: JsonName, value: tokenValue, info: "JsonPath: "
                    + Environment.NewLine
                    + jsonValue.Path
                    + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + RPGMUtils.GetCodeName(CurrentEventCode) : string.Empty)
                    , isCheckInput: false);
            }
            else
            {
                //предполагается, что ячейки из таблицы были разбиты на строки и добавлены в словарь
                //if (/*IsTranslatableRow.ContainsKey(tokenvalue) && IsTranslatableRow[tokenvalue] tried to add check if row have translation but here also issue because no displaed rows will be translated &&*/
                //    !ProjectData.CurrentProject.TablesLinesDict.ContainsKey(tokenValue)
                //    || ProjectData.CurrentProject.TablesLinesDict[tokenValue].Length == 0
                //    )
                //{
                //    return;
                //}

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
            string translation = tokenValue;
            if (!Format.SetTranslation(ref translation)) return;

            int originalLinesCount;
            if (translation.GetLinesCount() == (originalLinesCount = tokenValue.GetLinesCount())
                || value.Parent == null
                || value.Parent.Parent == null
                || value.Parent.Parent.Parent == null
                || !(value.Parent.Parent.Parent is JObject currentJObject)

                )
            {
                value.Value = translation;
            }
            else
            {
                int ind = 0;
                List<string> stringToWrite = new List<string>();
                bool notWrited = true;
                bool isTheJObjetAdded = false;

                foreach (var line in translation.SplitToLines())
                {
                    if (ind < originalLinesCount)
                    {
                        stringToWrite.Add(line);
                        ind++;
                    }
                    else
                    {
                        if (notWrited)
                        {
                            value.Value = string.Join(Environment.NewLine, stringToWrite);
                            notWrited = false;
                        }

                        var newJObject = GetNewJObject(currentJObject, line);
                        currentJObject.AddAfterSelf(newJObject);
                        if (!isTheJObjetAdded && !AddedJObjects.Contains(newJObject))
                        {
                            isTheJObjetAdded = true;
                            AddedJObjects.Add(newJObject);
                        }
                        currentJObject = newJObject;//делать добавленный JObject текущим, чтобы новый добавлялся после него
                    }
                }
            }
        }

        /// <summary>
        /// Корректировка формата записываемого json так, как в файлах RPGMaker MV
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private string CorrectJsonFormatToRPGMMV()
        {
            //return JsonRoot.ToString(Formatting.Indented);
            return JsonRoot.ToString(Formatting.None);
            //return Regex.Replace(root.ToString(Formatting.None), @"^\[null,(.+)\]$", "[\r\nnull,\r\n$1\r\n]");//regex нужен только для Formatting.None
        }

        bool IsValidString(JToken token, string value)
        {
            string path;
            return Format.IsValidString(value)
                && !((path = token.Path).Contains("].name") && !value.Contains("Enemy(")/*constructions like Enemy(enemyname) using also to identify enemies*/ && Regex.IsMatch(path, @"events\[[0-9]+\]\.name")) // skip event names //disabled because using in some cases like enemy name as event name(need to make dull open save with duplicates for correct translation)
                && !(path.Contains("].image") && Regex.IsMatch(path, @"\[[0-9]+\]\.image")) // skip image names
                && !(Regex.IsMatch(token.Path, @"[Bb]g[ms]\.name")) // skip bgm s
                && !(path.Contains("soundName") || Regex.IsMatch(token.Path, @"sounds\[[0-9]+\]\.name")) // skip sounds
                && !(path.Contains("].tilesetNames")) // skip tileset names
                && !(path.Contains("parallaxName")) // skip parallax names
                ;
        }

        HashSet<JObject> AddedJObjects = new HashSet<JObject>();

        protected override void ParseJsonObject(JObject jsonObject)
        {
            if (AddedJObjects.Contains(jsonObject)) return;

            ParseJsonObjectProperties(jsonObject);

            ResetCurrentCode();
        }

        /// <summary>
        /// reset current code if it is was exist
        /// </summary>
        private void ResetCurrentCode()
        {
            if (CurrentEventCode != -1) CurrentEventCode = -1;
        }

        protected override JsonObjectPropertyState ParseJsonObjectProperty(JProperty jsonProperty)
        {
            bool IsCode = IsInteger(jsonProperty.Value.Type) && jsonProperty.Name == "code";
            if (IsCode) CurrentEventCode = (int)jsonProperty.Value;

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

                if (Format.OpenFileMode)
                {
                    bool HasCurCode = true; // message code parse
                    Format.AddRowData(tablename: JsonName, value: fullmessage, info: "JsonPath: "
                        + Environment.NewLine
                        + jsonObject.Path
                        + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + RPGMUtils.GetCodeName(CurrentEventCode) : string.Empty)
                        , isCheckInput: true);

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
        private static bool IsCodeWithStringInParameters(int currentEventCode)
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
        private static bool IsMessageCode(int currentEventCode) => currentEventCode == 401 || currentEventCode == 405;

        /// <summary>
        /// get all string lines of message from input <paramref name="messagePartsList"/>
        /// </summary>
        /// <param name="messagePartsList"></param>
        /// <returns></returns>
        private static string GetMessageLinesFrom(List<JObject> messagePartsList)
        {
            List<string> lines = new List<string>(messagePartsList.Count);
            foreach (JObject token in messagePartsList)
            {
                JProperty prop = token.Last as JProperty;
                JArray array = prop.Value as JArray;
                foreach (string value in array.Select(v => (string)v))
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
        private List<JObject> GetNextTokensWithSameCode(JToken jsonToken)
        {
            var list = new List<JObject> { jsonToken as JObject };

            var next = jsonToken.Next as JObject;
            while (next is JObject obj
                && obj.First is JProperty prop
                && prop.Name == "code"
                && (int)prop.Value == CurrentEventCode
                )
            {
                AddedJObjects.TryAdd(obj);
                list.Add(next);
                next = next.Next as JObject;
            }

            return list;
        }

        /// <summary>
        /// Translate originalMergedMessage
        /// </summary>
        /// <param name="originalMessageJTokensList"></param>
        /// <param name="originalMergedMessage"></param>
        private void TranslateMessages(List<JObject> originalMessageJTokensList, string originalMergedMessage)
        {
            if (!Format.IsValidString(originalMergedMessage)) return;

            var translation = originalMergedMessage;
            if (!Format.SetTranslation(ref translation)) return;

            var translated = translation.SplitToLines().ToArray();

            var origLength = originalMessageJTokensList.Count;
            JObject lastJObject = null;
            for (int i = 0; i < translated.Length; i++)
            {
                if (i < origLength)
                {
                    JObject jObject = originalMessageJTokensList[i];
                    JProperty jProperty = jObject.Last as JProperty;
                    JArray propertiesJArray = jProperty.Value as JArray;
                    propertiesJArray[0] = translated[i]; // set line as one value of properties array of the object

                    AddedJObjects.TryAdd(jObject);
                    lastJObject = jObject;
                }
                else
                {
                    var newJObjectWithExtraMessageLine = GetNewJObject(lastJObject, translated[i]); // get new object for extra line
                    lastJObject.AddAfterSelf(newJObjectWithExtraMessageLine); // add new object after last
                    AddedJObjects.TryAdd(newJObjectWithExtraMessageLine); // add new object to skip list

                    lastJObject = lastJObject.Next as JObject; // make new object as last object
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
        /// String for this codes must not be appeared
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private static bool IsExcludedCode(int currentEventCode) { return ExcludedCodes.ContainsKey(currentEventCode); }

        /// <summary>
        /// true if type is JTokenType.Integer
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsInteger(JTokenType type) { return type == JTokenType.Integer; }

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
            var dict = add ? AppData.RpgMVAddedCodesStat : AppData.RpgMVSkippedCodesStat;
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

        static Dictionary<int, string> ExcludedCodes { get => RPGMVLists.ExcludedCodes; set => RPGMVLists.ExcludedCodes = value; }
    }
}
