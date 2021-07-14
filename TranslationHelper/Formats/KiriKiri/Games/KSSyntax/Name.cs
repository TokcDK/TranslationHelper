using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Name : KSSyntaxBase
    {
        public Name(ProjectData projectData) : base(projectData)
        {
        }

        internal override string StartsWith => @"^\t*【";

        internal override string EndsWith => @"】";
    }
}
