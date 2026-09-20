using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.Glitch_Pitch.Idol_Manager.Mod
{
    internal abstract class IdolManagerModBase: FormatStringBase
    {
        protected IdolManagerModBase(IFormatHost host) : base(host)
        {
        }

        protected override Encoding GetStringFileEncoding()
        {
            return Encoding.UTF8;
        }
    }
}
