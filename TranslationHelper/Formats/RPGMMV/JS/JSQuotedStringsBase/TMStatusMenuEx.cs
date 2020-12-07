using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class TMStatusMenuEx : JSQuotedStringsBase
    {
        public TMStatusMenuEx(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string JSName => "TMStatusMenuEx.js";
    }
}
