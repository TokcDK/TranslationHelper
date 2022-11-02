using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class pk_CommonMenuEvents : JSQuotedStringsBase
    {
        public pk_CommonMenuEvents()
        {
        }

        public override string JSName => "pk_CommonMenuEvents.js";

        protected override string PreQuoteRegexPattern => @"eventName[0-9]+\s*\=\s*"; // get only event names

        //protected override bool TryOpen()
        //{
        //    return ParseJSSingleLinesWithRegex(@".*var eventName[0-9]{1,2} \= ""([^""]+)"";.*");
        //}

        //protected override bool TrySave()
        //{
        //    return ParseJSSingleLinesWithRegex(@".*var eventName[0-9]{1,2} \= ""([^""]+)"";.*");
        //}
    }
}
