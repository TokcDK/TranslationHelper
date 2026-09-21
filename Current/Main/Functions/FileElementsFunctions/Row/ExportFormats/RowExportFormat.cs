using System.Collections.Generic;
using System.Text;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats
{
    /// <summary>
    /// How an export is laid out on disk: the save dialog filter, the markers around each record and
    /// the encoding.
    /// <para>
    /// It is the description plus the one operation that uses it — turning the collected records into
    /// the text of the file. There is no dialog and no IO here, so a format can be described and
    /// checked without touching the file system; <see cref="ExportFormatsBase"/> owns those.
    /// </para>
    /// </summary>
    internal class RowExportFormat
    {
        internal RowExportFormat(
            string filter,
            string markerOriginal,
            string markerTranslation,
            string markerFileStart = "",
            string markerStart = "",
            string markerEnd = "\r\n\r\n",
            Encoding saveEncoding = null)
        {
            Filter = filter;
            MarkerOriginal = markerOriginal;
            MarkerTranslation = markerTranslation;
            MarkerFileStart = markerFileStart;
            MarkerStart = markerStart;
            MarkerEnd = markerEnd;
            SaveEncoding = saveEncoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// File save dialog filter. Determines the file's extension.
        /// </summary>
        internal string Filter { get; }

        /// <summary>
        /// Placed before the original value.
        /// </summary>
        internal string MarkerOriginal { get; }

        /// <summary>
        /// Placed after the original and before the translation.
        /// </summary>
        internal string MarkerTranslation { get; }

        /// <summary>
        /// Placed in the first line of the file.
        /// </summary>
        internal string MarkerFileStart { get; }

        /// <summary>
        /// Placed before a new record.
        /// </summary>
        internal string MarkerStart { get; }

        /// <summary>
        /// Placed after a record.
        /// </summary>
        internal string MarkerEnd { get; }

        /// <summary>
        /// Encoding with which the file is saved.
        /// </summary>
        internal Encoding SaveEncoding { get; }

        /// <summary>
        /// Lays the collected records out as the text of the file.
        /// </summary>
        internal string Serialize(IEnumerable<string> records)
        {
            return MarkerFileStart
                + MarkerStart
                + MarkerOriginal
                + string.Join(MarkerEnd + MarkerStart + MarkerOriginal, records)
                + MarkerEnd;
        }
    }
}
