//using KenshModTIO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.LOFI.Kenshi
{
    class MOD : FormatStringBase
    {
        internal override bool Open()
        {
            AddTables();

            //var reader = new KenshModIO();

            //foreach (var str in reader.GetStrings(ProjectData.SelectedGameDir, FilePath))
            //{
            //    AddRowData(str, CheckInput: false);
            //}

            return CheckTablesContent(FilePath);
        }

        void OpenTHeFile()
        {
            //var reader = new KenshModIO();

            //foreach (var str in reader.GetStrings(ProjectData.SelectedGameDir, FilePath))
            //{
            //    AddRowData(str, CheckInput: false);
            //}
        }
    }
}
