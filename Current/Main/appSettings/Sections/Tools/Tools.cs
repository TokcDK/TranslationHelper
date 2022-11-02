using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Tools : SettingsBase
    {
        protected Tools()
        {
        }

        internal override string Section => "Tools";
    }
}
