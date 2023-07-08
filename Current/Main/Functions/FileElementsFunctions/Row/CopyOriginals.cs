using System.Windows.Forms;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class CopyOriginals : ApplyAfterFillBufferWithOriginalsBase
    {
        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return base.IsValidRow(rowData) && string.IsNullOrEmpty(rowData.Original);
        }

        protected override bool ApplyToBuffered()
        {
            Clipboard.SetText(string.Join("\n", _bufferedOriginals));

            return true;
        }
    }
}
