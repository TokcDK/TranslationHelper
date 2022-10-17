using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod;

namespace TranslationHelper.Projects.IdolManager.Mod
{
    internal class IdolManager : ProjectBase
    {
        public override string Name => "Idol Manager Mod";
        internal override string FileFilter => $"Idol Manager Mod|*.json";

        public override bool Open() { return ParseFiles(); }

        public override bool SubpathInTableName => true;

        private bool ParseFiles()
        {
            var rootDir = Path.GetDirectoryName(Data.AppData.SelectedFilePath);
            var ret = false;

            IEnumerable<FileInfo> FileInfos()
            {
                yield return new FileInfo(Path.Combine(rootDir, "JSON", "Events", "dialogues.json"));
                yield return new FileInfo(Path.Combine(rootDir, "Singles", "marketing.json"));
            }

            var paths = FileInfos();

            var charsDataDir = new DirectoryInfo(Path.Combine(rootDir, "Textures", "IdolPortraits"));
            if (charsDataDir.Exists) paths = paths.Concat(charsDataDir.GetFiles("params.json", System.IO.SearchOption.AllDirectories));

            foreach (var path in paths) if (path.Exists && OpenSaveFilesBase(path.DirectoryName, typeof(Dialogues_json), mask: path.Name)) ret = true;

            return ret;
        }

        public override bool Save() { return ParseFiles(); }

        internal override bool IsValid()
        {
            return
                string.Equals(Path.GetFileName(Data.AppData.SelectedFilePath), "info.json", StringComparison.InvariantCultureIgnoreCase)
                ;
        }
    }
}
