using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class General : SettingsBase
    {
        protected General(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Section => "General";
    }
}
