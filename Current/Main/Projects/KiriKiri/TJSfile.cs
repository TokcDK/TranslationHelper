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
            return AppData.SelectedProjectFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS");
        }

        internal override string ProjectDBFolderName => "KiriKiri";

        public override string Name => "KiriKiri tjs";

        protected override bool TryOpen()
        {
            return false;
            //var format = new TJSOld
            //{
            //    FilePath = ProjectData.SelectedFilePath
            //};
            //return format.Open();
        }

        protected override bool TrySaveProject()
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
