using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV
{
    class JSON : RPGMMVBase, IUseJsonParser
    {
        public JSON()
        {
            JsonParser = new RPGMVZJsonParser(this);
        }

        public override string Extension => ".json";

        internal override bool UseTableNameWithoutExtension => true;

        public JsonParserBase JsonParser { get => JParser; set => JParser = value; }

        JsonParserBase JParser;

        bool JsonIsParsed;

        protected override void FileOpen()
        {
            JsonIsParsed = JsonParser.ParseUsingFormat(this);
        }

        protected override bool WriteFileData(string filePath = "")
        {
            return JsonIsParsed;
        }

        internal override bool IsValidString(string inputString)
        {
            int commentIndex = inputString.IndexOf("//");
            var tokenvalueNoComment = commentIndex > -1 ? inputString.Substring(0, commentIndex) : inputString;
            return base.IsValidString(tokenvalueNoComment);
        }
    }
}
