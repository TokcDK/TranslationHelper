using System.Globalization;
using TranslationHelper.Data;
using TranslationHelper.Formats.KiriKiri;

namespace TranslationHelper.Projects.KiriKiri
{
    class TJSfile : ProjectBase
    {
        public TJSfile(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool OpenDetect()
        {
            return thDataWork.SPath.ToUpper(CultureInfo.GetCultureInfo("en-US")).EndsWith(".TJS");
        }

        internal override string ProjecFolderName()
        {
            return "KiriKiri";
        }

        internal override string ProjectTitle()
        {
            return "KiriKiri tjs";
        }

        internal override bool Open()
        {
            return new TJS(thDataWork).Open();
        }

        internal override bool Save()
        {
            return new TJS(thDataWork).Save();
        }
    }
}
