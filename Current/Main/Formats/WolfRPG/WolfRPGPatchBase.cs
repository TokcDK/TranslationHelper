using TranslationHelper.Data;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.WolfRPG
{
    abstract class WolfRPGPatchBase : RPGMWolfTransPatchBase
    {
        protected WolfRPGPatchBase(IFormatHost host) : base(host)
        {
        }
    }
}
