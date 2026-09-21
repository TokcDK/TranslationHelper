using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats
{
    /// <summary>
    /// Collects one record per row and writes them all out once the table is done.
    /// <para>
    /// It owns the accumulation, the "where to save" question and the write; how the file is laid out
    /// is <see cref="RowExportFormat"/>'s business, and a subclass normally only supplies a format.
    /// </para>
    /// </summary>
    abstract class ExportFormatsBase : RowBase
    {
        /// <summary>
        /// Layout of the file this operation writes.
        /// </summary>
        protected abstract RowExportFormat Format { get; }

        protected override bool IsParallelRows => false;

        /// <summary>
        /// Records collected so far, written out when the last row of the table has been added.
        /// </summary>
        private readonly List<string> _records = new List<string>();

        protected override bool Apply(RowBaseRowData rowData)
        {
            _records.Add(OriginalMod(rowData.Original) + Format.MarkerTranslation + TranslationMod(rowData.Translation));

            if (!rowData.IsLastRow) return true;

            return Save();
        }

        /// <summary>
        /// Asks where to save and writes the file. Cancelling is not a failure: the rows were still
        /// collected, so it reports success the way the old code did.
        /// </summary>
        private bool Save()
        {
            using (var save = new SaveFileDialog())
            {
                save.Filter = Format.Filter;
                if (save.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(save.FileName))
                {
                    return true;
                }

                return WriteFile(save.FileName);
            }
        }

        /// <summary>
        /// Writes the collected records. Overridden by formats that do not write text.
        /// </summary>
        protected virtual bool WriteFile(string fileName)
        {
            File.WriteAllText(fileName, Format.Serialize(_records), Format.SaveEncoding);

            return true;
        }

        /// <summary>
        /// Modification of the translation before it is added. No modification by default.
        /// </summary>
        protected virtual string TranslationMod(string translation)
        {
            return translation;
        }

        /// <summary>
        /// Modification of the original before it is added. No modification by default.
        /// </summary>
        protected virtual string OriginalMod(string original)
        {
            return original;
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return true;
        }
    }
}
