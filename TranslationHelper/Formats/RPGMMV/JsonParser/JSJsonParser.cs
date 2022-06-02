using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    class JSJsonParser : JsonParserBase
    {
        protected bool IsPluginsJS;//for some specific to plugins.js operations
        private bool PluginsJsNameFound;
        //protected int rowindex = 0;
        //private bool UseHashSet;

        public JSJsonParser()
        {
        }

        protected override void Init()
        {
            IsPluginsJS = string.Equals(JsonName, "Plugins.js", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override void ParseValue(JValue jsonValue)
        {
            var tokenValue = (string)jsonValue.Value;

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
                if (!IsValidToken(jsonValue)) return;

                if (ProjectData.OpenFileMode)
                {
                    Format.AddRowData(JsonName, tokenValue,
                        jsonValue.Path
                        + (IsPluginsJS && jsonValue.Path.StartsWith("parameters.", StringComparison.InvariantCultureIgnoreCase)
                        ? Environment.NewLine + T._("Warning") + ". " + T._("Parameter: translation of some parameters can break the game.")
                        : string.Empty)
                        , CheckInput: true);
                }
                else
                {
                    string translation = tokenValue;
                    if (!Format.SetTranslation(ref translation)) return;

                    jsonValue.Value = translation;

                    //if (UseHashSet)
                    //{
                    //    if (ProjectData.CurrentProject.TablesLinesDict.ContainsKey(tokenValue)
                    //        && !string.IsNullOrEmpty(ProjectData.CurrentProject.TablesLinesDict[tokenValue])
                    //        && ProjectData.CurrentProject.TablesLinesDict[tokenValue] != tokenValue)
                    //    {
                    //        jsonValue.Value = ProjectData.CurrentProject.TablesLinesDict[tokenValue];
                    //    }
                    //}
                    //else
                    //{
                    //    var row = ProjectData.THFilesElementsDataset.Tables[JsonName].Rows[rowindex];
                    //    if (Equals(jsonValue.ToString(), row[0]) && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                    //    {
                    //        jsonValue.Value = row[1] as string;
                    //    }
                    //    rowindex++;
                    //}
                }
            }
        }
        protected bool IsValidToken(JValue value)
        {
            return value.Type == JTokenType.String
                && (!IsPluginsJS || (value.Path != "Modelname" && !value.Path.Contains("parameters.picName") && !value.Path.Contains("imageName")))
                //&& (!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
                && !string.IsNullOrWhiteSpace(value + "")
                && !(THSettings.SourceLanguageIsJapanese() && value.ToString().HaveMostOfRomajiOtherChars());
        }

        protected override void ParseJsonObject(JObject jsonObject)
        {
            ParseJsonObjectProperties(jsonObject);

            PluginsJsNameFound = false; // switch on check of parse plugin's json data
        }

        protected override JsonObjectPropertyState ParseJsonObjectProperty(JProperty jsonProperty)
        {
            if (!PluginsJsNameFound && IsPluginsJS && jsonProperty.Name == "name")
            {
                PluginsJsNameFound = true; // switch off check of parse plugin's json data

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
