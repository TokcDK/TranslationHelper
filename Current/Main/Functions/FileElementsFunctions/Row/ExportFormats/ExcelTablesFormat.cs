using MiniExcelLibs;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats
{
    class ExcelTablesFormat : ExportFormatsBase
    {
        protected override RowExportFormat Format { get; } = new RowExportFormat(
            filter: "Excel file|*.xlsx",
            markerOriginal: "",
            markerTranslation: "");

        /// <summary>
        /// Writes the whole project content as a workbook. This format does not write text, so the
        /// markers of <see cref="RowExportFormat"/> are not used.
        /// </summary>
        protected override bool WriteFile(string fileName)
        {
            MiniExcel.SaveAs(fileName, Project.FilesContent);

            return true;
        }
    }
}
