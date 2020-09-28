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

        internal override bool Check()
        {
            return thDataWork.SPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TJS");
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
            return new TJS(thDataWork).Open();
        }

        internal override bool Save()
        {
            return new TJS(thDataWork).Save();
        }
    }
}
