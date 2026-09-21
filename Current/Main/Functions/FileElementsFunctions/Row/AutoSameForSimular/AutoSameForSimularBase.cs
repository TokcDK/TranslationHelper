using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    /// <summary>
    /// Gives every row whose original is the same line as the one being processed but for its numbers
    /// and symbols the same translation with those numbers and symbols replaced: "книга 1" translated
    /// as "book 1" makes "книга 2" become "book 2".
    /// <para>
    /// The operation itself only decides which rows are worth spreading from and asks for them. The
    /// scan is <see cref="SimilarTranslationSpreader"/>, the order the scans run in is
    /// <see cref="SimilarSpreadQueue"/>, the rule that recognises a similar line is
    /// <see cref="SimilarTextRules"/>, and whether any of it may run is <see cref="ProjectReadiness"/>.
    /// They are kept apart because they fail for different reasons: a wrong rule, a re-entrant scan,
    /// and a project that is not ready yet.
    /// </para>
    /// </summary>
    abstract class AutoSameForSimularBase : RowBase
    {
        /// <summary>
        /// Only a row that already has a translation is worth spreading from. The force variant also
        /// accepts a row whose translation is still its original, because it is the one that is asked
        /// to replace those.
        /// </summary>
        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return !string.IsNullOrEmpty(rowData.Translation);
        }

        protected virtual bool IsForce => false;

        protected override bool Apply(RowBaseRowData rowData)
        {
            // Nothing is started while the project is still opening, or while its translation database
            // is being read: at either point the rows a spread would compare against are only half
            // there, and the translations it would carry over are the ones a load is about to replace.
            if (!ProjectReadiness.IsReady) return false;

            SimilarSpreadQueue.Add(new SimilarSpreadRequest(
                Project, UiUpdater, rowData.SelectedTableIndex, rowData.SelectedRowIndex, IsForce));

            return true;
        }
    }
}
