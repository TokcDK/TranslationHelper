using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    abstract class ApplyAfterFillBufferithOriginalsBase : RowBase
    {
        public ApplyAfterFillBufferithOriginalsBase()
        {
        }

        protected override bool IsValidRow()
        {
            return true;
        }

        protected readonly List<string> _bufferedOriginals = new List<string>();

        protected override bool Apply()
        {
            _bufferedOriginals.Add(SelectedRow[0] as string);//add original value

            return true;
        }

        protected abstract bool ApplyToBuffered();

        protected override void ActionsFinalize()
        {
            ApplyToBuffered();
        }
    }
}
