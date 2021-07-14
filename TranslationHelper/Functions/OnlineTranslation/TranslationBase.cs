using TranslationHelper.Data;

namespace TranslationHelper.Functions.OnlineTranslation
{
    abstract class TranslationBase
    {
        protected ProjectData projectData;
        protected TranslationBase(ProjectData projectData)
        {
            this.projectData = projectData;
        }

        internal abstract void Get();
    }
}
