using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class General : SettingsBase
    {
        protected General(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Section => "General";
    }
}
