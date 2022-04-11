using System.Text;
using TranslationHelper.Formats.WolfRPG;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    /// <summary>
    /// shared data for rpgmaker/wolfrpg trans patch txts
    /// </summary>
    abstract class PatchTXTBase : RPGMWolfTransPatchBase
    {
        protected override Encoding WriteEncoding()
        {
            return Encoding.UTF8;
        }
    }

}
