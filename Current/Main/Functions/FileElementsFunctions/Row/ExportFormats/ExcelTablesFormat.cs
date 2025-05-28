using MiniExcelLibs;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats
{
    class ExcelTablesFormat : ExportFormatsBase
    {
        public ExcelTablesFormat()
        {
        }

        protected override string Filter => "Excel file|*.xlsx";

        protected override string MarkerOiginal => "";

        protected override string MarkerTranslation => "";

        protected override bool WriteFile(string fileName)
        {
            MiniExcel.SaveAs(fileName, Project.FilesContent);

            return true;
        }
    }
}
