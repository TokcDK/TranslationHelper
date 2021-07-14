using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Script : KSSyntaxBase
    {
        public Script(ProjectData projectData) : base(projectData)
        {
        }

        internal override string StartsWith => @"(?<!\[)\[iscript\]|^\t*@iscript";

        internal override string EndsWith => @"\[endscript\]|^\t*@endscript";
    }
}
