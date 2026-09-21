using System.Collections.Concurrent;
using TranslationHelper.Projects;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    /// <summary>
    /// The project's index of "which rows hold exactly this original", read so that a line the index
    /// does not know is an answer rather than an error.
    /// <para>
    /// The project collects the index while it parses its files, and it only collects it when the
    /// "don't load duplicates" setting is off. With that setting on — which is its default — a
    /// repeated line is loaded once and there is nothing to index, so the dictionary stays empty for
    /// the whole session. Even with the setting off, the index only holds the files parsed so far and
    /// only the lines the parser registered, so a line that is on screen can still be missing from it.
    /// </para>
    /// <para>
    /// Every read therefore goes through <c>TryGetValue</c>. Indexing the dictionary directly is what
    /// used to throw <see cref="System.Collections.Generic.KeyNotFoundException"/> in the middle of a
    /// spread — and, because that read was unconditional, it threw for every line as soon as the
    /// index was empty, which stopped the whole scan before it started.
    /// </para>
    /// </summary>
    internal sealed class DuplicateOriginalIndex
    {
        /// <summary>
        /// The project's own index: original text to table name to the row indexes holding it.
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>> _coordinates;

        private DuplicateOriginalIndex(
            ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentSet<int>>> coordinates)
        {
            _coordinates = coordinates;
        }

        /// <summary>
        /// The index of <paramref name="project"/>, or null when the project has none: no project, no
        /// index collected, or duplicate loading switched off.
        /// </summary>
        internal static DuplicateOriginalIndex Of(ProjectBase project)
        {
            if (project == null) return null;
            if (project.DontLoadDuplicates) return null;

            var coordinates = project.OriginalsTableRowCoordinates;

            return coordinates == null ? null : new DuplicateOriginalIndex(coordinates);
        }

        /// <summary>
        /// The rows holding <paramref name="original"/>, as table name to row indexes.
        /// <para>
        /// False means "the index does not know this line", not "no line is similar": the caller still
        /// has to scan for the lines that differ only in their numbers.
        /// </para>
        /// </summary>
        /// <param name="original">The original text to look up.</param>
        /// <param name="rows">The rows holding it, when it is known.</param>
        internal bool TryGetRows(string original, out ConcurrentDictionary<string, ConcurrentSet<int>> rows)
        {
            // A null key would be rejected by the dictionary itself, and a row without an original is
            // not something the index was ever asked about.
            if (original == null)
            {
                rows = null;
                return false;
            }

            return _coordinates.TryGetValue(original, out rows);
        }
    }
}
