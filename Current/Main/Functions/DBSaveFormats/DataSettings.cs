using System.Collections.Generic;
using TranslationHelper.Functions;
using TranslationHelper.Settings;

namespace TranslationHelper.Functions.DBSaveFormats
{
    /// <summary>
    /// Settings of the translation database: how it is saved, what is loaded from it and when it is
    /// written automatically.
    /// <para>
    /// The database format list is read from the formats themselves, so a new
    /// <see cref="IDataBaseFileFormat"/> shows up in the settings form and in the INI file without
    /// either of them being touched.
    /// </para>
    /// </summary>
    internal static class DataSettings
    {
        internal const string SectionName = "Data";

        internal const int SectionPosition = 20;

        /// <summary>Save the database compressed instead of as plain XML.</summary>
        internal sealed class DBCompression : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DBCompression";

            internal override int Order => 10;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Compression for DB files:";

            internal override string Description =>
                "Format for DB files: standard not compressed xml and compressed xml for both other";

            internal override bool Default => true;
        }

        /// <summary>File format used to save the database.</summary>
        internal sealed class DBCompressionExt : ChoiceSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DBCompressionExt";

            internal override int Order => 20;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Database file format:";

            internal override string Description => "File format used to save the translation database.";

            internal override string Default => new XML().Description;

            internal override IReadOnlyList<string> Choices
            {
                get
                {
                    var descriptions = new List<string>();
                    foreach (var format in FunctionsInterfaces.GetDBSaveFormats())
                    {
                        descriptions.Add(format.Description);
                    }

                    return descriptions;
                }
            }
        }

        /// <summary>Load a repeated line once instead of once per occurrence.</summary>
        internal sealed class DontLoadDuplicates : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DontLoadDuplicates";

            internal override int Order => 30;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Dont load duplicates";

            internal override string Description =>
                "A line that occurs more than once is loaded into the table only once.";

            internal override bool Default => true;
        }

        /// <summary>Check every line of the database against every line of the table while loading.</summary>
        internal sealed class FullComprasionDBload : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "FullComprasionDBload";

            internal override int Order => 40;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Full recursive scan while translation DB loading (slower)";

            internal override string Description =>
                "In time of DB loading will be checked all DB lines for each line of table for translation.";

            internal override bool Default => false;
        }

        /// <summary>Save the opened database periodically instead of only when the user asks.</summary>
        internal sealed class EnableDBAutosave : BoolSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "EnableDBAutosave";

            internal override int Order => 50;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Enable autosave.";

            internal override string Description =>
                "Saves the opened translation database automatically while it is being edited.";

            internal override bool Default => true;
        }

        /// <summary>Seconds of inactivity after which the database is saved again.</summary>
        internal sealed class DBAutosaveTimeout : IntSetting
        {
            internal override string Section => SectionName;

            internal override string Key => "DBAutosaveTimeout";

            internal override int Order => 60;

            internal override int SectionOrder => SectionPosition;

            internal override string Label => "Autosave timeout (seconds):";

            internal override string Description =>
                "Seconds of inactivity before the opened translation database is saved again.";

            internal override int Default => 300;

            internal override int Min => 1;

            internal override int Max => 999;
        }
    }
}
