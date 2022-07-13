using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    class JSJsonParser : JsonParserBase
    {
        public JSJsonParser(FormatBase format) : base(format)
        {
        }

        //protected int rowindex = 0;
        //private bool UseHashSet;

        protected override void Init() { }

        protected override void ParseValue(JValue jsonValue)
        {
            var tokenValue = (string)jsonValue.Value;
            if (tokenValue.StartsWith("{") && tokenValue.EndsWith("}") || tokenValue.StartsWith("[\"") && tokenValue.EndsWith("\"]"))
            {
                var root = JToken.Parse(tokenValue);

                //parse subtoken
                Parse(root);

                if (Format.SaveFileMode)
                {
                    jsonValue.Value = root.ToString(Formatting.None);
                }
            }
            else
            {
                if (!IsValidToken(jsonValue)) return;

                if (Format.OpenFileMode)
                {
                    Format.AddRowData(JsonName, tokenValue,
                        jsonValue.Path
                        + ExtraInfo(jsonValue)
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

        protected virtual string ExtraInfo(JValue jsonValue) { return ""; }

        protected virtual bool IsValidToken(JValue value)
        {
            if (value.Type != JTokenType.String) return false;
            if (string.IsNullOrWhiteSpace(value + "")) return false;
            if (THSettings.SourceLanguageIsJapanese && value.ToString().HaveMostOfRomajiOtherChars()) return false;

            return true;
            //(!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
        }
    }
}
