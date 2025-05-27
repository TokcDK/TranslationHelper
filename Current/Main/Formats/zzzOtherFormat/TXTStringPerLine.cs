using TranslationHelper.Projects;

namespace TranslationHelper.Formats.zzzOtherFormat
{
    internal class TXTStringPerLine : FormatTxtFileBase
    {
        public TXTStringPerLine(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            AddRowData(ref ParseData.Line);

            SaveModeAddLine();

            return KeywordActionAfter.Continue;
        }

    }
}
