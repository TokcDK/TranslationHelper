using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Tools : SettingsBase
    {
        protected Tools(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Section => "Tools";
    }
}
