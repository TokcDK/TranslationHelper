using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class RECOLLECTIONMODE : JSBase
    {
        public RECOLLECTIONMODE(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseJSVarInJson("var rngd_recollection_mode_settings = {");
        }

        internal override bool Save()
        {
            return ParseJSVarInJsonWrite("var rngd_recollection_mode_settings = {");
        }

        internal override string JSName => "RecollectionMode.js";
    }
}
