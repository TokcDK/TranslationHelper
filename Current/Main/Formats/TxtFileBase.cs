using TranslationHelper.Projects;

namespace TranslationHelper.Formats
{
    internal abstract class FormatTxtFileBase : FormatStringBase
    {
        protected FormatTxtFileBase(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".txt";
    }
}
