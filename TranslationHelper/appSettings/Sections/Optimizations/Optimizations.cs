using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    abstract class Optimizations : SettingsBase
    {
        protected Optimizations(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Section => "Optimizations";
    }
}
