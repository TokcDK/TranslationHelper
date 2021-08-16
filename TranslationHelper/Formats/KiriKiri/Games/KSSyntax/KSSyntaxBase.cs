using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    abstract class KSSyntaxBase
    {
        /// <summary>
        /// determine regex of start identifier for the block
        /// </summary>
        /// <returns></returns>
        internal abstract string StartsWith { get; }
        /// <summary>
        /// determine regex of end identifier for the block
        /// </summary>
        /// <returns></returns>
        internal abstract string EndsWith { get; }
        /// <summary>
        /// determine list of blocks which included in this and will be searching inside
        /// </summary>
        /// <returns></returns>
        internal virtual List<KSSyntaxBase> Include()
        {
            return null;
        }
    }
}
