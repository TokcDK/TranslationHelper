using System.Globalization;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri;

namespace TranslationHelper.Projects.KiriKiri
{
    class TJSfile : ProjectBase
    {
        public TJSfile(ProjectData projectData) : base(projectData)
        {
        }

        internal override bool Check()
        {
            return projectData.SPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS");
        }

        internal override string ProjectFolderName()
        {
            return "KiriKiri";
        }

        internal override string Name()
        {
            return "KiriKiri tjs";
        }

        internal override bool Open()
        {
            return new TJS(projectData).Open();
        }

        internal override bool Save()
        {
            return new TJS(projectData).Save();
        }
    }
}
