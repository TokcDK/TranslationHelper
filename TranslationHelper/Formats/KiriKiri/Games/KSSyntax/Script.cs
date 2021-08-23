namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
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
