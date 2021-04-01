using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.LiveMaker
{
    abstract class LiveMakerBase : FormatBase
    {
        protected LiveMakerBase(THDataWork thDataWork) : base(thDataWork)
        {
        }
    }
}
