using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSBase
    {
        public KS(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Ext()
        {
            return ".ks";
        }

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
