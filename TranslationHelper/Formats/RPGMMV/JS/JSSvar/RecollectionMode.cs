using Newtonsoft.Json;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS.JSSvar
{
    class RECOLLECTIONMODE : JSSVarBase
    {
        public RECOLLECTIONMODE()
        {
        }

        internal override string JSName => "RecollectionMode.js";

        public bool IsComment { get; private set; }

        protected override string SvarIdentifier => "var rngd_recollection_mode_settings = {";
    }
}
