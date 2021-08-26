namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Comment : KSSyntaxBase
    {
        public Comment()
        {
        }

        internal override string StartsWith => @"^\t*;";

        internal override string EndsWith => "\n";
    }
}
