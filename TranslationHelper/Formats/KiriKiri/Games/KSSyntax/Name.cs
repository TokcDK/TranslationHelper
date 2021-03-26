using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Name : KSSyntaxBase
    {
        public Name(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string StartsWith => @"^\t*【";

        internal override string EndsWith => @"】";
    }
}
