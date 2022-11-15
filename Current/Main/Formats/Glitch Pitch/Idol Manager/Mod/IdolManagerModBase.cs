using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats.Glitch_Pitch.Idol_Manager.Mod
{
    internal abstract class IdolManagerModBase: FormatStringBase
    {
        protected override Encoding GetStringFileEncoding()
        {
            return Encoding.UTF8;
        }
    }
}
