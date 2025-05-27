using System.Text;
using TranslationHelper.Formats.TyranoBuilder.Extracted;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSParserBase//KSOther//
    {
        public KS(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".ks";

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
