using System;
using System.Globalization;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri;

namespace TranslationHelper.Projects.KiriKiri
{
    class TJSfile : ProjectBase
    {
        [Obsolete]
        public TJSfile()
        {
        }

        internal override bool IsValid()
        {
            return AppData.SelectedFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS");
        }

        internal override string ProjectFolderName => "KiriKiri";

        public override string Name => "KiriKiri tjs";

        public override bool Open()
        {
            return false;
            //var format = new TJSOld
            //{
            //    FilePath = ProjectData.SelectedFilePath
            //};
            //return format.Open();
        }

        public override bool Save()
        {
            return false;
            //        var format = new TJSOld
            //        {
            //            FilePath = ProjectData.SelectedFilePath
            //};
            //        return format.Save();
        }
    }
}
