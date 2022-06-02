using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    class PluginsJSJsonParser : JSJsonParser
    {
        private bool PluginsJsNameFound;

        public PluginsJSJsonParser() { }

        protected override void Init() { }

        protected override string ExtraInfo(JValue jsonValue)
        {
            return jsonValue.Path.StartsWith("parameters.", StringComparison.InvariantCultureIgnoreCase)
                        ? Environment.NewLine + T._("Warning") + ". " + T._("Parameter: translation of some parameters can break the game.")
                        : string.Empty;
        }

        protected override bool IsValidToken(JValue value)
        {
            return base.IsValidToken(value) && !(
                   string.Equals(value.Path, "description", StringComparison.InvariantCulture)
                || string.Equals(value.Path, "Modelname", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(value.Path, "imageName", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(value.Path, "ImageFile", StringComparison.InvariantCultureIgnoreCase)
                || value.Path.ToLowerInvariant().Contains("picname")
                || value.Path.ToLowerInvariant().Contains("file")
                );
            //(!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
        }

        protected override void ParseJsonObject(JObject jsonObject)
        {
            base.ParseJsonObject(jsonObject);

            PluginsJsNameFound = false; // switch on check of parse plugin's json data
        }

        protected override JsonObjectPropertyState ParseJsonObjectProperty(JProperty jsonProperty)
        {
            if (!PluginsJsNameFound && jsonProperty.Name == "name")
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

            return base.ParseJsonObjectProperty(jsonProperty);
        }
    }
}
