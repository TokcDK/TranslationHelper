using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Script : KSSyntaxBase
    {
        public Script(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string StartsWith => @"(?<!\[)\[iscript\]|^\t*@iscript";

        internal override string EndsWith => @"\[endscript\]|^\t*@endscript";
    }
}
