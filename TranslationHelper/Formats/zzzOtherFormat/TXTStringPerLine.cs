namespace TranslationHelper.Formats.zzzOtherFormat
{
    internal class TXTStringPerLine : FormatTxtFileBase
    {
        protected override KeywordActionAfter ParseStringFileLine()
        {
            AddRowData(ref ParseData.Line);

            SaveModeAddLine();

            return KeywordActionAfter.Continue;
        }

    }
}
