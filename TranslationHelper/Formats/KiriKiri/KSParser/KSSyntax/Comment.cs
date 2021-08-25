namespace TranslationHelper.Formats.KiriKiri.KSParser.KSSyntax
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
