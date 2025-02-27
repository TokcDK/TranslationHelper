using System.IO;
using System.Linq;
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
            var gameDir = Path.GetDirectoryName(Data.AppData.SelectedProjectFilePath);
            var gameRgss3aPath = Path.Combine(gameDir, "Game.rgss3a");
            var dataDirPath = Path.Combine(gameDir, "Data");

            if (OpenFileMode && File.Exists(gameRgss3aPath) && !Directory.Exists(dataDirPath))
            {
                var parser = new RGSS_Extractor.Main_Parser();

                Directory.CreateDirectory(dataDirPath);

                foreach (var entry in parser.ParseFile(gameRgss3aPath))
                {
                    var filePath = gameDir + "\\" + entry.Name;
                    Directory.GetParent(filePath).Create();
                    File.WriteAllBytes(filePath, parser.GetFiledata(entry));
                }

                parser.CloseFile();

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
