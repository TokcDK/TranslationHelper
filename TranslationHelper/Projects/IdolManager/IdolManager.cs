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
            var dialogsPath = Path.Combine(rootDir, "JSON", "Events", "dialogues.json");
            if (File.Exists(dialogsPath) && OpenSaveFilesBase(Path.GetDirectoryName(dialogsPath), typeof(Dialogues_json), mask: "dialogues.json")) ret = true;

            var charsDataDir = new DirectoryInfo(Path.Combine(rootDir, "Textures", "IdolPortraits"));
            if (charsDataDir.Exists)
            {
                foreach (var dir in charsDataDir.GetDirectories())
                    if (OpenSaveFilesBase(dir.FullName, typeof(Params_json), mask: "params.json")) ret = true;
            }

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
