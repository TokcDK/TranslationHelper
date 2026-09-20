using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.zzzOtherFormat
{
    internal class TXTStringPerLine : FormatTxtFileBase
    {
        public TXTStringPerLine(IFormatHost host) : base(host)
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
