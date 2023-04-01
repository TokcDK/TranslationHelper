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
        public override string Name => "RPG Maker VX Ace .rvdata2 from Unpacked Data (WIP!)";

        public override bool Open()
        {
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(Path.GetDirectoryName(Data.AppData.SelectedFilePath),"Data"), typeof(RVDATA2), "*.rvdata2");
        }

        public override bool Save()
        {
            return false;
        }

        internal override bool IsValid()
        {
            var parent = Path.GetDirectoryName(Data.AppData.SelectedFilePath);
            var data = Path.Combine(parent, "Data");

            return Directory.Exists(data) && File.Exists(Path.Combine(data, "System.rvdata2"));
        }
    }
}
