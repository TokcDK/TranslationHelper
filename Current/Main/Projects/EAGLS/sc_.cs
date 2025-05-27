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
            return AppData.SelectedProjectFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT") && Path.GetFileName(AppData.SelectedProjectFilePath).StartsWith("sc_");
        }

        internal override string FileFilter => "EAGLS SCPACK sc_txt|sc_*.txt";

        protected override bool Open()
        {
            var format = new SC_TXT
            {
                FilePath = AppData.SelectedProjectFilePath
            };

            return format.Open();
        }

        public override string Name => "EAGLS scenario (" + Path.GetFileNameWithoutExtension(AppData.SelectedProjectFilePath) + ")";

        internal override string ProjectDBFolderName => "EAGLS";

        protected override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
