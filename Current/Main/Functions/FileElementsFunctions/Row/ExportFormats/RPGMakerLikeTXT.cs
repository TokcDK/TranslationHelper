namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats
{
    class RpgMakerLikeTxt : ExportFormatsBase
    {
        protected override RowExportFormat Format { get; } = new RowExportFormat(
            filter: "TXT file|*.txt",
            markerOriginal: "[ORIGINAL]\r\n",
            markerTranslation: "\r\n[TRANSLATION]\r\n");
    }
}
