using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV
{
    class JSON : RPGMMVBase
    {
        public JSON() : base()
        {
        }

        internal override string Ext()
        {
            return ".json";
        }

        protected override bool UseTableNameWithoutExtension => true;

        RPGMVZJsonParser JsonParser = new RPGMVZJsonParser();
        bool JsonIsParsed;

        protected override void ParseStringFileOpen()
        {
            JsonIsParsed = JsonParser.ParseUsingProject(this);
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
