using System.Collections.Generic;
using System.Threading.Tasks;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    /// <summary>
    /// Runs the spreads one at a time on a background thread, and never twice for the same row at the
    /// same time.
    /// <para>
    /// A spread writes into the work table, and every write it makes comes back as a "cell changed"
    /// event — which is what starts the operation in the first place. Without something to recognise
    /// that, each write would start the spread over again: that is the recursion this operation used
    /// to have, and a per-instance guard could not stop it, because the event creates a new instance
    /// of the operation every time.
    /// </para>
    /// <para>
    /// The request that comes back is always for the row the spread started from, because the plan is
    /// built from the grid's selection and a spread only writes rows outside it. So a row that is
    /// already queued or already running is the re-entry and is dropped, while the set of known rows
    /// is cleared when the queue drains: editing the same row afterwards is a new run, not a
    /// duplicate.
    /// </para>
    /// <para>
    /// A run can also start for several rows at once — one per row selected in the grid — and those
    /// are real work rather than re-entries. They are queued instead of dropped, which is what keeps
    /// one scan of the whole project running at a time instead of one per selected row.
    /// </para>
    /// </summary>
    internal static class SimilarSpreadQueue
    {
        private static readonly object Locker = new object();

        private static readonly Queue<SimilarSpreadRequest> Pending = new Queue<SimilarSpreadRequest>();

        /// <summary>
        /// Keys of the spreads that are queued or running. Cleared when the queue drains.
        /// </summary>
        private static readonly HashSet<string> Known = new HashSet<string>();

        private static bool _workerRunning;

        /// <summary>
        /// Asks for the spread of one row, unless that spread is already queued or running.
        /// </summary>
        internal static void Add(SimilarSpreadRequest request)
        {
            if (request == null) return;

            lock (Locker)
            {
                if (!Known.Add(request.Key)) return;

                Pending.Enqueue(request);

                // A worker is already draining the queue, and it will pick this up.
                if (_workerRunning) return;

                _workerRunning = true;
            }

            // Deliberately not awaited: the caller is the grid's "cell changed" handler, and it must
            // not wait for a scan of every row of every file. This is also why the worker marshals
            // each write to the UI thread itself instead of relying on the caller's thread.
            _ = Task.Run(ProcessQueueAsync);
        }

        /// <summary>
        /// Carries the queued spreads out, one after another, until there is nothing left.
        /// </summary>
        private static async Task ProcessQueueAsync()
        {
            try
            {
                while (true)
                {
                    SimilarSpreadRequest request;

                    lock (Locker)
                    {
                        if (Pending.Count == 0) return;

                        request = Pending.Dequeue();
                    }

                    await Task.Run(request.Run).ConfigureAwait(false);
                }
            }
            finally
            {
                // Reached when the queue drains and when the drain itself fails, so a failure cannot
                // leave the queue looking busy for the rest of the session.
                lock (Locker)
                {
                    _workerRunning = false;

                    // The run is over, so the same row asked for again is a new run. Clearing here
                    // rather than per request is what keeps the guard up for the whole drain: the
                    // re-entrant request arrives from inside a write, which is while the loop above is
                    // still running.
                    Known.Clear();
                }
            }
        }
    }
}
