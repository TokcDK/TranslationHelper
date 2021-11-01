using System.Windows.Forms;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class CopyOriginals : ApplyAfterFillBufferithOriginalsBase
    {
        protected override bool ApplyToBuffered()
        {
            Clipboard.SetText(string.Join("\n", _bufferedOriginals));

            return true;
        }
    }
}
