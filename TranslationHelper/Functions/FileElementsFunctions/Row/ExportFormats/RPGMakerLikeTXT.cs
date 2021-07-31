using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats
{
    class RpgMakerLikeTxt : ExportFormatsBase
    {
        public RpgMakerLikeTxt() : base()
        {
        }

        protected override string Filter => "TXT file|*.txt";

        protected override string MarkerOiginal => "\r\n[ORIGINAL]\r\n";

        protected override string MarkerTranslation => "\r\n[Translation]\r\n";
    }
}
