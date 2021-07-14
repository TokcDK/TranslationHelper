using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Tools : SettingsBase
    {
        protected Tools() : base()
        {
        }

        internal override string Section => "Tools";
    }
}
