using System.Text;
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
