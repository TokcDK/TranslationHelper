using System;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Formats.WolfRPG;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMTrans
{
    abstract class RPGMTransPatchBase : RPGMWolfTransPatchBase
    {
        //protected StringBuilder buffer;
        protected RPGMTransPatchBase(ProjectBase parentProject) : base(parentProject)
        {
        }
    }
}
