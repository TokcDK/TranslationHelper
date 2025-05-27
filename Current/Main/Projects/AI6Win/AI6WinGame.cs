using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TranslationHelper.Data;
using TranslationHelper.Formats.AI6WIN;

namespace TranslationHelper.Projects.AI6Win
{
    internal class AI6WinGame : ProjectBase
    {
        public override string Name => "AI6Win Game";

        protected override bool TryOpen()
        {
            return OpenSave();
        }

        protected override bool TrySaveProject()
        {
            return OpenSave();
        }
        bool OpenSave()
        {
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.GetDirectoryName(AppData.SelectedProjectFilePath), typeof(AI6Arc), "mes.arc");
        }

        internal override bool IsValid()
        {
            if (!ProjectTools.IsExe(AppData.SelectedProjectFilePath)) return false;
            if (!string.Equals(Path.GetFileName(AppData.SelectedProjectFilePath)
                , "AI6WIN.exe", StringComparison.InvariantCultureIgnoreCase)) return false;

            var dir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);

            return File.Exists(Path.Combine(dir, "mes.arc"));
        }
    }
}
