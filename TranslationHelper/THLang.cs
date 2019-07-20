namespace TranslationHelper
{
    public class THLang
    {
        //Menus
        //-//File
        public string THStrfileToolStripMenuItemName { get; set; } = "Файл";

        public string THStropenToolStripMenuItemName { get; set; } = "Open";
        public string THStrsaveToolStripMenuItemName { get; set; } = "Save";
        public string THStrsaveAsToolStripMenuItemName { get; set; } = "Save As";

        //-//Edit
        public string THStreditToolStripMenuItemName { get; set; } = "Edit";

        //-//View
        public string THStrviewToolStripMenuItemName { get; set; } = "View";

        //-//Options
        public string THStroptionsToolStripMenuItemName { get; set; } = "Options";

        //-//Help
        public string THStrhelpToolStripMenuItemName { get; set; } = "Help";

        public string THStraboutToolStripMenuItemName { get; set; } = "About";

        //Elements Datagridview
        public string THStrDGTranslationColumnName { get; set; } = "Translation";

        public string THStrDGOriginalColumnName { get; set; } = "Original";

        //RPGMTransPatch strings
        public string THStrRPGMTransPatchInvalidVersionMsg { get; set; } = "Failed to identify version of patch";

        public string THStrRPGMTransPatchInvalidFormatMsg { get; set; } = "Failed to identify format of patch";

        public void THReadLanguageFileToStrings()
        {
        }
    }
}