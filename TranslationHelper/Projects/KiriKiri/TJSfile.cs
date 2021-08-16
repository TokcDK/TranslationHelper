using System.Globalization;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri;

namespace TranslationHelper.Projects.KiriKiri
{
    class TjSfile : ProjectBase
    {
        public TjSfile()
        {
        }

        internal override bool Check()
        {
            return ProjectData.SelectedFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS");
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
            return new Tjs().Open();
        }

        internal override bool Save()
        {
            return new Tjs().Save();
        }
    }
}
