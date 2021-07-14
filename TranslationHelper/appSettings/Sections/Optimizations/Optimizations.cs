using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Optimizations : SettingsBase
    {
        protected Optimizations() : base()
        {
        }

        internal override string Section => "Optimizations";
    }
}
