using System.Text;
using TranslationHelper.Formats.WolfRPG;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    /// <summary>
    /// shared data for rpgmaker/wolfrpg trans patch txts
    /// </summary>
    abstract class PatchTXTBase : RPGMWolfTransPatchBase
    {
        protected PatchTXTBase(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override Encoding WriteEncoding()
        {
            return Encoding.UTF8;
        }
    }

}
