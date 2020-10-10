using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    abstract class RPGMTransPatchBase : FormatBase
    {
        //protected StringBuilder buffer;

        public RPGMTransPatchBase(THDataWork thDataWork/*, StringBuilder sBuffer*/) : base(thDataWork)
        {
            //buffer = sBuffer;
        }
    }
}
