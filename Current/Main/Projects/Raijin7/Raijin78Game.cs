using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.Raijin7;
using TranslationHelper.Formats.Raijin7.eve;

namespace TranslationHelper.Projects
{
    class Raijin78Game : ProjectBase
    {
        public Raijin78Game()
        {
            HideVarsBase = new Dictionary<string, string>
            {
                {"all_pid", @"call_pid[0-9]{1,3}"},//call_pid1
                {"callself_pid", @"callself_pid[0-9]{1,3}"},//callself_pid1
                {"p_name", @"p_name[0-9]{1,3}"},//p_name1
                {"p_name_pval", @"p_name_pval"},//p_name_pval
                {"crp_name", @"crp_name[0-9]{1,3}"},//crp_name2
                {"s_name_pva", @"s_name_pval"},//s_name_pval
                {"buf_str", @"buf_str[0-9]{1,3}"}//buf_str1
            };
        }

        internal override bool IsValid()
        {
            string dirPath = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            return Path.GetExtension(AppData.SelectedProjectFilePath) == ".exe"
                && Directory.Exists(Path.Combine(dirPath, "eve"))
                && Directory.Exists(Path.Combine(dirPath, "csv"))
                ;
        }

        internal override string FileFilter => ProjectTools.GameExeFilter;

        public override string Name => "Raijin 7 & 8";

        protected override bool TryOpen()
        {
            return OpenSave();
        }

        protected override bool TrySave()
        {
            return OpenSave();
        }

        bool OpenSave()
        {
            var dirPath = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            var ret = false;

            if (ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(dirPath, "eve"), typeof(TXT), "*.txt"))
                ret = true;
            if (ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(dirPath, "csv"), typeof(CSV), "*.csv"))
                ret = true;
            if (ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(dirPath, "gsxy"), typeof(CSV), "*.csv"))
                ret = true;

            return ret;
        }

        public override List<string> BakPaths { get; set; } = new List<string>()
        {
                Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "eve"),
                Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "csv"),
                Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "gsxy")
        };

    }
}
