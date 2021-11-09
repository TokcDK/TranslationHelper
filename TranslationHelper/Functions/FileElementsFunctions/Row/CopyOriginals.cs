using System.Windows.Forms;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class CopyOriginals : ApplyAfterFillBufferWithOriginalsBase
    {
        protected override bool IsValidRow()
        {
            return base.IsValidRow() && (SelectedRow[1] == null || string.IsNullOrEmpty(SelectedRow[1].ToString()));
        }

        protected override bool ApplyToBuffered()
        {
            Clipboard.SetText(string.Join("\n", _bufferedOriginals));

            return true;
        }
    }
}
