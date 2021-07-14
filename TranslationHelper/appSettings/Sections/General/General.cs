using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class General : SettingsBase
    {
        protected General() : base()
        {
        }

        internal override string Section => "General";
    }
}
