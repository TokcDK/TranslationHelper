using System;
using System.Globalization;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;

namespace TranslationHelper.Projects.EAGLS
{
    class sc_ : ProjectBase
    {
        public sc_(ProjectData projectData) : base(projectData)
        {
        }

        internal override bool Check()
        {
            return projectData.SPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT") && Path.GetFileName(projectData.SPath).StartsWith("sc_");
        }

        internal override string Filters()
        {
            return "EAGLS SCPACK sc_txt|sc_*.txt";
        }

        internal override bool Open()
        {
            projectData.FilePath = projectData.SPath;
            return new SC_TXT(projectData).Open();
        }

        internal override string Name()
        {
            return "EAGLS scenario (" + Path.GetFileNameWithoutExtension(projectData.SPath) + ")";
        }

        internal override string ProjectFolderName()
        {
            return "EAGLS";
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
