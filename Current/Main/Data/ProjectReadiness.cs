using System;
using System.Threading;

namespace TranslationHelper.Data
{
    /// <summary>
    /// The two facts an automatic operation has to know before it may work on the project content:
    /// whether the project has finished opening, and whether its translation database is being read.
    /// <para>
    /// Both are needed because the content is built in steps and is unusable between them. While a
    /// project opens, its tables are filled one file at a time; while the database loads, the
    /// translations are cleared and written back one row at a time. An operation started in either
    /// window sees a half-built project — which is how an automatic operation ends up spreading an
    /// empty translation, or a translation that the load is about to replace.
    /// </para>
    /// <para>
    /// The state lives here rather than on the project because it is read by callers that do not have
    /// the project in hand, and because "the database is loading right now" outlives the project
    /// instance it is loading into: a project is replaced on every open.
    /// </para>
    /// </summary>
    internal static class ProjectReadiness
    {
        /// <summary>
        /// True between the moment a project's content is complete and the moment the next project
        /// starts opening. Written from the UI thread and read from worker threads, hence volatile.
        /// </summary>
        private static volatile bool _projectIsFullyOpened;

        /// <summary>
        /// Number of translation database loads in flight.
        /// <para>
        /// Counted rather than flagged because one load runs inside another: the load that starts
        /// while a project opens goes through the same code as the one the menu starts, and the inner
        /// one reporting itself finished must not report the outer one as finished too.
        /// </para>
        /// </summary>
        private static int _databaseLoadsInFlight;

        /// <summary>
        /// True while the content of an opened project may be worked on: its files are parsed and its
        /// files list is filled.
        /// </summary>
        internal static bool IsProjectFullyOpened => _projectIsFullyOpened;

        /// <summary>
        /// True while a translation database is being read into the project content.
        /// </summary>
        internal static bool IsDatabaseLoading => Volatile.Read(ref _databaseLoadsInFlight) > 0;

        /// <summary>
        /// Whether the project content may be worked on: a project is open, it has finished opening,
        /// and nothing is being loaded into it.
        /// </summary>
        internal static bool IsReady => AppSettings.ProjectIsOpened && _projectIsFullyOpened && !IsDatabaseLoading;

        /// <summary>
        /// Reports that the project being opened now has its content complete: its files are parsed,
        /// its files list is filled and its menus exist.
        /// <para>
        /// Called before the translation database is read, because that is a separate step with its
        /// own state — the project is open at that point, and what keeps an operation out of it is
        /// <see cref="IsDatabaseLoading"/>.
        /// </para>
        /// </summary>
        internal static void MarkProjectOpened() => _projectIsFullyOpened = true;

        /// <summary>
        /// Reports that the project content is no longer usable, because a different project is about
        /// to be opened or because opening failed. Nothing automatic runs against it until
        /// <see cref="MarkProjectOpened"/> is called again.
        /// </summary>
        internal static void MarkProjectClosed() => _projectIsFullyOpened = false;

        /// <summary>
        /// Reports a translation database load as started. Disposing the result reports it finished.
        /// Loads may nest, and the state stays "loading" until the outermost one is done.
        /// </summary>
        /// <returns>A scope that has to be disposed when the load ends, whatever its outcome.</returns>
        internal static IDisposable BeginDatabaseLoad()
        {
            Interlocked.Increment(ref _databaseLoadsInFlight);

            return new DatabaseLoad();
        }

        /// <summary>
        /// One database load, undone when it ends.
        /// </summary>
        private sealed class DatabaseLoad : IDisposable
        {
            private bool _isDone;

            public void Dispose()
            {
                // A load that failed must release the state exactly like one that succeeded, and
                // releasing it twice must not count as two loads having finished.
                if (_isDone) return;

                _isDone = true;

                Interlocked.Decrement(ref _databaseLoadsInFlight);
            }
        }
    }
}
