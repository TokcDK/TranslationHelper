using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects
{
    class ProjectRPGMakerTransPatch
    {
        protected IOpenFormats openFormat;
        protected ISaveFormats saveFormat;

        public ProjectRPGMakerTransPatch()
        {
            openFormat = new RPGMakerTransPatch();
            saveFormat = new RPGMakerTransPatch();
        }

        public string GetProjectName()
        {
            return "RPGMakerTransPatch";
        }
    }
}
