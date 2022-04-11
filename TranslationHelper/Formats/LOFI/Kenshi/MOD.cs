using KenshModTIO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.LOFI.Kenshi
{
    class MOD : FormatStringBase
    {
        internal override bool Open()
        {
            AddTables();

            GetStrings();

            return CheckTablesContent(FilePath);
        }

        void GetStrings()
        {
            foreach (var str in KenshModIO.GetStrings(ProjectData.SelectedGameDir, FilePath))
            {
                AddRowData(str, CheckInput: false);
            }
        }
    }
}
