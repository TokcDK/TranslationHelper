﻿using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod;

namespace TranslationHelper.Projects.IdolManager.Mod
{
    internal class IdolManager : ProjectBase
    {
        public override string Name => "Idol Manager Mod";
        internal override string FileFilter => $"Idol Manager Mod|*.json";

        protected override bool TryOpen() { return ParseFiles(); }

        public override bool SubpathInTableName => true;

        private bool ParseFiles()
        {
            var rootDir = Path.GetDirectoryName(Data.AppData.SelectedProjectFilePath);
            var ret = false;

            var infos = FileInfos(rootDir).Concat(ParamInfos(rootDir));

            foreach (var info in infos) if (info.i.Exists && ProjectToolsOpenSave.OpenSaveFilesBase(this, info.i.DirectoryName, info.t, mask: info.i.Name)) ret = true;

            return ret;
        }

        IEnumerable<(FileInfo i, Type t)> FileInfos(string rootDir)
        {
            yield return (new FileInfo(Path.Combine(rootDir, "JSON", "Events", "dialogues.json")), typeof(Dialogues_json));
            yield return (new FileInfo(Path.Combine(rootDir, "JSON", "Singles", "marketing.json")), typeof(Marketing_json));
        }

        IEnumerable<(FileInfo i, Type t)> ParamInfos(string rootDir)
        {
            var charsDataDir = new DirectoryInfo(Path.Combine(rootDir, "Textures", "IdolPortraits"));
            if (!charsDataDir.Exists) yield break;
            foreach (var i in charsDataDir.EnumerateFiles("params.json", System.IO.SearchOption.AllDirectories))
            {
                yield return (i, typeof(Params_json));
            }
        }

        protected override bool TrySave() { return ParseFiles(); }

        internal override bool IsValid()
        {
            return
                string.Equals(Path.GetFileName(Data.AppData.SelectedProjectFilePath), "info.json", StringComparison.InvariantCultureIgnoreCase)
                ;
        }
    }
}
