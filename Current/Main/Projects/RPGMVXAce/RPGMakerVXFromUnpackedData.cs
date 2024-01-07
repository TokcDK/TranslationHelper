using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGSS_Extractor;
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
            var parent = Path.GetDirectoryName(Data.AppData.SelectedProjectFilePath);
            var gameRgss3aPath = Path.Combine(parent, "Game.rgss3a");
            bool isRgss3a = File.Exists(gameRgss3aPath);
            var dataDirPath = Path.Combine(parent, "Data");

            if (isRgss3a)
            {
                var parser = new RGSS_Extractor.Main_Parser();

                Directory.CreateDirectory(dataDirPath);

                foreach (var entry in parser.parse_file(gameRgss3aPath))
                {
                    var p = Path.Combine(parent, entry.name);
                    File.WriteAllBytes(p, parser.get_filedata(entry));
                }

                parser.close_file();

                File.Move(gameRgss3aPath, gameRgss3aPath + ".orig");
            }

            return ProjectToolsOpenSave
                .OpenSaveFilesBase(this,
                dataDirPath, 
                typeof(RVDATA2), 
                "*.rvdata2");
        }

        internal override bool IsValid()
        {
            var parent = Path.GetDirectoryName(Data.AppData.SelectedProjectFilePath);
            var data = Path.Combine(parent, "Data");

            return (Directory.Exists(data) && Directory.EnumerateFiles(data, "*.rvdata2").Any()) || File.Exists(Path.Combine(parent, "Game.rgss3a"));
        }
    }
}
