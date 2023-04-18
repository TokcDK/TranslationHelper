using System.Windows.Forms;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class CopyOriginals : ApplyAfterFillBufferWithOriginalsBase
    {
        protected override bool IsValidRow()
        {
            return base.IsValidRow() && string.IsNullOrEmpty(Original);
        }

        protected override bool ApplyToBuffered()
        {
            Clipboard.SetText(string.Join("\n", _bufferedOriginals));

            return true;
        }
    }
}
