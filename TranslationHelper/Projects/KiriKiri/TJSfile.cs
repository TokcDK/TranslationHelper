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

        internal override bool Check()
        {
            return AppData.SelectedFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS");
        }

        internal override string ProjectFolderName => "KiriKiri";

        internal override string Name => "KiriKiri tjs";

        internal override bool TryOpen()
        {
            return false;
            //var format = new TJSOld
            //{
            //    FilePath = ProjectData.SelectedFilePath
            //};
            //return format.Open();
        }

        internal override bool TrySave()
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
