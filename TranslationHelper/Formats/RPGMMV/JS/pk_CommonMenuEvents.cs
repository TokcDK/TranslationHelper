using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class pk_CommonMenuEvents : JSBase
    {
        public pk_CommonMenuEvents()
        {
        }

        internal override string JSName => "pk_CommonMenuEvents.js";

        internal override bool Open()
        {
            return ParseJSSingleLinesWithRegex(@".*var eventName[0-9]{1,2} \= ""([^""]+)"";.*");
        }

        internal override bool Save()
        {
            return ParseJSSingleLinesWithRegex(@".*var eventName[0-9]{1,2} \= ""([^""]+)"";.*", true);
        }
    }
}
