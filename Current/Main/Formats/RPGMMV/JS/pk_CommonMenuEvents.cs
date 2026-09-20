using TranslationHelper.Data;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class pk_CommonMenuEvents : JSQuotedStringsBase
    {
        public pk_CommonMenuEvents(IFormatHost host) : base(host)
        {
        }

        public override string JSName => "pk_CommonMenuEvents.js";

        protected override string PreQuoteRegexPattern => @"eventName[0-9]+\s*\=\s*"; // get only event names
    }
}
