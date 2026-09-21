using TranslationHelper.Projects;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    /// <summary>
    /// One requested spread: the row to spread from, and the project and writer to do it with.
    /// <para>
    /// The project and the writer travel with the request because it is carried out later, on a
    /// background thread, after the caller that asked for it has returned. Everything else is read
    /// from the row at the moment the spread runs, so a request that waited in the queue works with
    /// the values the project holds when it is carried out rather than the ones it held when it was
    /// queued.
    /// </para>
    /// </summary>
    internal sealed class SimilarSpreadRequest
    {
        private readonly ProjectBase _project;
        private readonly IUiUpdater _writer;

        internal SimilarSpreadRequest(ProjectBase project, IUiUpdater writer, int tableIndex, int rowIndex, bool force)
        {
            _project = project;
            _writer = writer;

            TableIndex = tableIndex;
            RowIndex = rowIndex;
            Force = force;
        }

        internal int TableIndex { get; }

        internal int RowIndex { get; }

        internal bool Force { get; }

        /// <summary>
        /// Identifies the spread. Two requests with the same key would do the same work, which is how
        /// the queue recognises the request that a spread's own writes cause.
        /// </summary>
        internal string Key => TableIndex + "|" + RowIndex + "|" + Force;

        /// <summary>
        /// Carries the spread out. Does not throw: it runs on a background thread, where a failure
        /// would have no caller to reach.
        /// </summary>
        internal void Run()
        {
            new SimilarTranslationSpreader(_project, _writer, TableIndex, RowIndex, Force).Run();
        }
    }
}
