using TranslationHelper.Formats.KiriKiri.KSParser.KSSyntax;

namespace TranslationHelper.Formats.KiriKiri.KSParser.KSSyntax
{
    class Script : KSSyntaxBase
    {
        public Script()
        {
        }

        internal override string StartsWith => @"(?<!\[)\[iscript\]|^\t*@iscript";

        internal override string EndsWith => @"\[endscript\]|^\t*@endscript";
    }
}
