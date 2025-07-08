using System;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Formats.WolfRPG;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMTrans
{
    abstract class RPGMTransPatchBase : RPGMWolfTransPatchBase
    {
        protected RPGMTransPatchBase(ProjectBase parentProject) : base(parentProject)
        {
        }
    }
}
