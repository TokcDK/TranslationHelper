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

            if (SelectedRowsCountRest > 0)
            {
                return true;//return while all rows originals will be added
            }

            return ApplyToBuffered();
        }

        protected abstract bool ApplyToBuffered();
    }
}
