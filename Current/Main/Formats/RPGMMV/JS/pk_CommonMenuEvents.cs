using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class pk_CommonMenuEvents : JSQuotedStringsBase
    {
        public pk_CommonMenuEvents(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string JSName => "pk_CommonMenuEvents.js";

        protected override string PreQuoteRegexPattern => @"eventName[0-9]+\s*\=\s*"; // get only event names
    }
}
