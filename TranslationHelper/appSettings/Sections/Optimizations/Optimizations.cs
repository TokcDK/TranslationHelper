using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Optimizations : SettingsBase
    {
        protected Optimizations()
        {
        }

        internal override string Section => "Optimizations";
    }
}
