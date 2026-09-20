using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.LiveMaker
{
    abstract class LiveMakerBase : FormatStringBase
    {
        protected LiveMakerBase(IFormatHost host) : base(host)
        {
        }
    }
}
