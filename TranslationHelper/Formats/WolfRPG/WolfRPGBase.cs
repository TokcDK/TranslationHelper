using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG
{
    abstract class WolfRPGBase : FormatBase
    {
        protected WolfRPGBase(THDataWork thDataWork) : base(thDataWork)
        {
        }
    }
}
