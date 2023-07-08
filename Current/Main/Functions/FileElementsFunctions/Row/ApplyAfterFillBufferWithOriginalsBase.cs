using System.Collections.Generic;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    abstract class ApplyAfterFillBufferWithOriginalsBase : RowBase
    {
        public ApplyAfterFillBufferWithOriginalsBase()
        {
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return true;
        }

        protected readonly List<string> _bufferedOriginals = new List<string>();

        protected override bool Apply(RowBaseRowData rowData)
        {
            _bufferedOriginals.Add(rowData.Original);//add original value

            return true;
        }

        protected abstract bool ApplyToBuffered();

        protected override void ActionsFinalize()
        {
            ApplyToBuffered();
        }
    }
}
