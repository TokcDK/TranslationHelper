using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Comment : KSSyntaxBase
    {
        public Comment(ProjectData projectData) : base(projectData)
        {
        }

        internal override string StartsWith => @"^\t*;";

        internal override string EndsWith => @"\n";
    }
}
