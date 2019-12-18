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

        protected int TableIndex;

        public RPGMTransPatchBase(THDataWork thData, StringBuilder sBuffer, int tableIndex) : base(thData)
        {
            buffer = sBuffer;
            TableIndex = tableIndex;
        }
    }
}
