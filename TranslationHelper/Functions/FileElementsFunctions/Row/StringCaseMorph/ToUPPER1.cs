using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUPPER : StringCaseMorphBase
    {
        public ToUPPER(ProjectData projectData) : base(projectData)
        {
        }

        protected override int variant => 2;
    }
}
