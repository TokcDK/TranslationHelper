using System;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Formats.WolfRPG;

namespace TranslationHelper.Formats.RPGMTrans
{
    abstract class RPGMTransPatchBase : RPGMWolfTransPatchBase
    {
        //protected StringBuilder buffer;

        public RPGMTransPatchBase(ProjectData projectData/*, StringBuilder sBuffer*/) : base(projectData)
        {
            //buffer = sBuffer;
        }
    }
}
