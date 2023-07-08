using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    abstract class ApplyAfterFillBufferWithOriginalsBase : RowBase
    {
        public ApplyAfterFillBufferWithOriginalsBase()
        {
        }

        protected override bool IsValidRow(RowData rowData)
        {
            return true;
        }

        protected readonly List<string> _bufferedOriginals = new List<string>();

        protected override bool Apply(RowData rowData)
        {
            _bufferedOriginals.Add(Original);//add original value

            return true;
        }

        protected abstract bool ApplyToBuffered();

        protected override void ActionsFinalize()
        {
            ApplyToBuffered();
        }
    }
}
