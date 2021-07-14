using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpper : StringCaseMorphBase
    {
        public ToUpper(ProjectData projectData) : base(projectData)
        {
        }

        protected override int variant => 1;
    }
}
