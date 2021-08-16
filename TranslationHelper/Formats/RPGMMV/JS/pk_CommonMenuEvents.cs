using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class PkCommonMenuEvents : JsBase
    {
        public PkCommonMenuEvents()
        {
        }

        public override string JsName => "pk_CommonMenuEvents.js";

        internal override bool Open()
        {
            return ParseJsSingleLinesWithRegex(@".*var eventName[0-9]{1,2} \= ""([^""]+)"";.*");
        }

        internal override bool Save()
        {
            return ParseJsSingleLinesWithRegex(@".*var eventName[0-9]{1,2} \= ""([^""]+)"";.*", true);
        }
    }
}
