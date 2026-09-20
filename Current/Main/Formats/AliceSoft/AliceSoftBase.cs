using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.AliceSoft
{
    abstract class AliceSoftBase : FormatStringBase
    {
        protected AliceSoftBase(IFormatHost host) : base(host)
        {
        }
    }
}
