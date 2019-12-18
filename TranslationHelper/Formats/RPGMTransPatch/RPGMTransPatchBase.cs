using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    abstract class RPGMTransPatchBase : FormatBase
    {
        protected StringBuilder buffer;

        public RPGMTransPatchBase(THDataWork thData, StringBuilder sBuffer) : base(thData)
        {
            buffer = sBuffer;
        }
    }
}
