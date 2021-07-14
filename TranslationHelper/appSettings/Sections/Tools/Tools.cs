using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Tools : SettingsBase
    {
        protected Tools(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Section => "Tools";
    }
}
