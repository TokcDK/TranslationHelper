using TranslationHelper.Formats;

namespace TranslationHelper.Projects
{
    public abstract class ProjectBase
    {
        protected IOpenFormats openFormat;
        protected ISaveFormats saveFormat;

        protected ProjectBase()
        {
            openFormat = new RPGMakerTransPatch();
            saveFormat = new RPGMakerTransPatch();
        }

        public static string GetProjectName()
        {
            return "RPGMakerTransPatch";
        }
    }
}
