using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    class JSJsonParser : JsonParserBase
    {
        private bool PluginsJsNameFound;

        public bool IsPluginsJS { get => string.Equals(JsonName, "Plugins.js", StringComparison.InvariantCultureIgnoreCase); }

        //protected int rowindex = 0;
        //private bool UseHashSet;

        public JSJsonParser()
        {
        }

        protected override void Init()
        {
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
            if (value.Type != JTokenType.String) return false;
            if (string.IsNullOrWhiteSpace(value + "")) return false;
            if (THSettings.SourceLanguageIsJapanese() && value.ToString().HaveMostOfRomajiOtherChars()) return false;

            bool b = (!IsPluginsJS ||
                !(
                   string.Equals(value.Path, "description", StringComparison.InvariantCulture)
                || string.Equals(value.Path, "Modelname", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(value.Path, "imageName", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(value.Path, "ImageFile", StringComparison.InvariantCultureIgnoreCase)
                || value.Path.ToLowerInvariant().Contains("picname")
                || value.Path.ToLowerInvariant().Contains("file")
                )
                );

            return b;
            //(!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
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
