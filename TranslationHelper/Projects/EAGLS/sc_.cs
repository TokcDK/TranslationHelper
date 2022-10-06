using System;
using System.Globalization;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;

namespace TranslationHelper.Projects.EAGLS
{
    class sc_ : ProjectBase
    {
        [Obsolete]
        public sc_()
        {
        }

        internal override bool IsValid()
        {
            return AppData.SelectedFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT") && Path.GetFileName(AppData.SelectedFilePath).StartsWith("sc_");
        }

        internal override string FileFilter => "EAGLS SCPACK sc_txt|sc_*.txt";

        public override bool Open()
        {
            var format = new SC_TXT
            {
                FilePath = AppData.SelectedFilePath
            };

            return format.Open();
        }

        public override string Name => "EAGLS scenario (" + Path.GetFileNameWithoutExtension(AppData.SelectedFilePath) + ")";

        internal override string ProjectFolderName => "EAGLS";

        public override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
