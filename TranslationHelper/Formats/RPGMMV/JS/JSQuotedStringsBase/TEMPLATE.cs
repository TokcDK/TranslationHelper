using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class TEMPLATE : JSQuotedStringsBase
    {
        public TEMPLATE(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string JSName => "TEMPLATE";
    }
}
