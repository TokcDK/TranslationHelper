namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Attribute : KSSyntaxBase
    {
        public Attribute()
        {
        }

        internal override string StartsWith => @"[A-Za-z]+\s*\=\s*\""([^\""]*)\""";

        internal override string EndsWith => null;
    }
}
