using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Tag : KsSyntaxBase
    {
        public Tag()
        {
        }

        internal override string StartsWith => @"(?<!\[)\[\s*\w+";

        internal override string EndsWith => @"\]";

        internal override List<KsSyntaxBase> Include()
        {
            return new List<KsSyntaxBase>
            {
                new Attribute()
            };
        }
    }
}
