using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToLower : StringCaseMorphBase
    {
        public ToLower(ProjectData projectData) : base(projectData)
        {
        }

        protected override int variant => 0;
    }
}
