using System.Globalization;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri;

namespace TranslationHelper.Projects.KiriKiri
{
    class TJSfile : ProjectBase
    {
        public TJSfile()
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
            var format = new TJSOld
            {
                FilePath = ProjectData.FilePath
            };
            return format.Open();
        }

        internal override bool Save()
        {
            var format = new TJSOld
            {
                FilePath = ProjectData.FilePath
            };
            return format.Save();
        }
    }
}
