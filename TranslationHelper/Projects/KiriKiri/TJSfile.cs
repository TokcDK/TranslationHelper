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
            return false;
            //var format = new TJSOld
            //{
            //    FilePath = ProjectData.SelectedFilePath
            //};
            //return format.Open();
        }

        internal override bool Save()
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
