using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV
{
    class Json : RpgmmvBase, IUseJsonParser
    {
        public Json()
        {
            JsonParser = new RpgmvzJsonParser();
        }

        internal override string Ext()
        {
            return ".json";
        }

        internal override bool UseTableNameWithoutExtension => true;

        public JsonParserBase JsonParser { get => _jParser; set => _jParser = value; }

        JsonParserBase _jParser;

        bool _jsonIsParsed;

        protected override void ParseStringFileOpen()
        {
            _jsonIsParsed = JsonParser.ParseUsingFormat(this);
        }

        protected override bool WriteFileData(string filePath = "")
        {
            return _jsonIsParsed;
        }

        internal override bool IsValidString(string inputString)
        {
            int commentIndex = inputString.IndexOf("//");
            var tokenvalueNoComment = commentIndex > -1 ? inputString.Substring(0, commentIndex) : inputString;
            return base.IsValidString(tokenvalueNoComment);
        }
    }
}
