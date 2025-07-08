using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TranslationHelper.Formats.RPGMMV.JsonParser;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class PLUGINS : JSBase
    {
        public PLUGINS(ProjectBase parentProject) : base(parentProject)
        {
            JsonParser = new PluginsJSJsonParser(this);
        }

        protected override bool JSTokenValid(JValue value)
        {
            return !value.Path.Contains("Modelname") && !value.Path.Contains("imageName");
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.TrimmedLine.TrimStart().StartsWith("{\"name\":"))
            {
                try
                {
                    if (ParseData.Line.EndsWith(","))
                    {
                        IsJsonNotLast = true;
                        ParseData.Line = ParseData.Line.Remove(ParseData.Line.Length - 1, 1);
                    }
                    else if (SaveFileMode)
                    {
                        IsJsonNotLast = false;
                    }
                    JsonParser.JsonName = FileName;
                    JsonParser.ParseString(ParseData.Line, this);

                    if (SaveFileMode)
                    {
                        ParseData.Line = JsonParser.JsonRoot.ToString(Formatting.None) + (IsJsonNotLast ? "," : string.Empty);
                        ParseData.Ret = true;
                    }
                }
                catch
                {
                }

                IsJsonNotLast = !IsJsonNotLast;
            }

            SaveModeAddLine(newline: "\n");

            return KeywordActionAfter.Continue;
        }        

        private bool IsJsonNotLast;

        public override string JSName => "plugins.js";
        public override string JSSubfolder => string.Empty;
    }
}
