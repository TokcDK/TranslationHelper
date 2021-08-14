using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class General : SettingsBase
    {
        protected General()
        {
        }

        internal override string Section => "General";
    }
}
