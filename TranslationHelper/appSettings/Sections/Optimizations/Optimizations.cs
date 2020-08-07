using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Optimizations : SettingsBase
    {
        protected Optimizations(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Section => "Optimizations";
    }
}
