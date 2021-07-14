using System;
using System.Globalization;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;

namespace TranslationHelper.Projects.EAGLS
{
    class sc_ : ProjectBase
    {
        public sc_() : base()
        {
        }

        internal override bool Check()
        {
            return ProjectData.SelectedFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT") && Path.GetFileName(ProjectData.SelectedFilePath).StartsWith("sc_");
        }

        internal override string Filters()
        {
            return "EAGLS SCPACK sc_txt|sc_*.txt";
        }

        internal override bool Open()
        {
            ProjectData.FilePath = ProjectData.SelectedFilePath;
            return new SC_TXT().Open();
        }

        internal override string Name()
        {
            return "EAGLS scenario (" + Path.GetFileNameWithoutExtension(ProjectData.SelectedFilePath) + ")";
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
