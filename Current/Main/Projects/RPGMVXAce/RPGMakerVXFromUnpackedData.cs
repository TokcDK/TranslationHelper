using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Formats.RPGMakerVX.RVData2;

namespace TranslationHelper.Projects.RPGMVXAce
{
    internal class RPGMakerVXFromUnpackedData : ProjectBase
    {
        public override string Name => "RPG Maker VX Ace -Unpacked Data-";

        internal override string ProjectDBFolderName => "RPGMakerVXAce";

        public override bool Open() { return OpenSave(); }

        public override bool Save() { return OpenSave(); }

        bool OpenSave()
        {
            return ProjectToolsOpenSave
                .OpenSaveFilesBase(this, 
                Path.Combine(Path.GetDirectoryName(Data.AppData.SelectedFilePath), "Data"), 
                typeof(RVDATA2), 
                "*.rvdata2");
        }

        internal override bool IsValid()
        {
            var parent = Path.GetDirectoryName(Data.AppData.SelectedFilePath);
            var data = Path.Combine(parent, "Data");

            return Directory.Exists(data) && File.Exists(Path.Combine(data, "System.rvdata2"));
        }
    }
}
