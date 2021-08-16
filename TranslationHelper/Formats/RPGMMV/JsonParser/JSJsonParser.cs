using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    class JsJsonParser : JsonParserBase
    {
        protected bool IsPluginsJs;//for some specific to plugins.js operations
        private bool _pluginsJsNameFound;
        protected int Rowindex = 0;
        private bool _useHashSet;

        public JsJsonParser()
        {
        }

        protected override void Init()
        {
            IsPluginsJs = string.Equals(JsonName, "Plugins.js", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override void ParseValue(JValue jsonValue)
        {
            var tokenValue = jsonValue + "";

            if (tokenValue.StartsWith("{") && tokenValue.EndsWith("}") || tokenValue.StartsWith("[\"") && tokenValue.EndsWith("\"]"))
            {
                var root = JToken.Parse(tokenValue);

                //parse subtoken
                Parse(root);

                if (ProjectData.SaveFileMode)
                {
                    jsonValue.Value = root.ToString(Formatting.None);
                }
            }
            else
            {
                if (!IsValidToken(jsonValue))
                    return;

                if (ProjectData.OpenFileMode)
                {
                    Format.AddRowData(JsonName, tokenValue,
                        jsonValue.Path
                        + (IsPluginsJs && jsonValue.Path.StartsWith("parameters.", StringComparison.InvariantCultureIgnoreCase)
                        ? Environment.NewLine + T._("Warning") + ". " + T._("Parameter: translation of some parameters can break the game.")
                        : string.Empty)
                        , true);
                }
                else
                {

                    if(_useHashSet)
                    {
                        if (ProjectData.TablesLinesDict.ContainsKey(tokenValue)
                            && !string.IsNullOrEmpty(ProjectData.TablesLinesDict[tokenValue])
                            && ProjectData.TablesLinesDict[tokenValue] != tokenValue)
                        {
                            jsonValue.Value = ProjectData.TablesLinesDict[tokenValue];
                        }
                    }
                    else
                    {
                        var row = ProjectData.ThFilesElementsDataset.Tables[JsonName].Rows[Rowindex];
                        if (Equals(jsonValue.ToString(), row[0]) && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                        {
                            jsonValue.Value = row[1] as string;
                        }
                        Rowindex++;
                    }

                }
            }
        }
        protected bool IsValidToken(JValue value)
        {
            return value.Type == JTokenType.String
                && (!IsPluginsJs || value.Path != "Modelname")
                //&& (!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
                && !string.IsNullOrWhiteSpace(value + "")
                && !(ThSettings.SourceLanguageIsJapanese() && value.ToString().HaveMostOfRomajiOtherChars());
        }

        protected override void ParseJsonObject(JObject jsonObject)
        {
            ParseJsonObjectProperties(jsonObject);

            _pluginsJsNameFound = false; // switch on check of parse plugin's json data
        }

        protected override JsonObjectPropertyState ParseJsonObjectProperty(JProperty jsonProperty)
        {
            if (!_pluginsJsNameFound && IsPluginsJs && jsonProperty.Name == "name")
            {
                _pluginsJsNameFound = true; // switch off check of parse plugin's json data

                if (jsonProperty.Parent.Last is JProperty lastObjectsProperty)
                {
                    Parse(lastObjectsProperty.Value); // parse only parameters

                    return JsonObjectPropertyState.Break; // skip rest of properties because last was parsed
                }
                else
                {
                    return JsonObjectPropertyState.Continue;
                }
            }

            Parse(jsonProperty.Value);

            return JsonObjectPropertyState.Continue;
        }
    }
}
