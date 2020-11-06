using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class NupuRnd : JSQuotedStringsBase
    {
        public NupuRnd(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string JSName => "NupuRnd.js";
    }
}
