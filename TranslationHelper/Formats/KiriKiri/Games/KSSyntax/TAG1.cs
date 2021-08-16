using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class TAG1 : KSSyntaxBase
    {
        public TAG1()
        {
        }

        internal override string StartsWith => @"^\t*@\s*\w+";

        internal override string EndsWith => @"(?<!\\)\n";

        internal override List<KSSyntaxBase> Include()
        {
            return new List<KSSyntaxBase>
            {
                new Attribute()
            };
        }
    }
}
