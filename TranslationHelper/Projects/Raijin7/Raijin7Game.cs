using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.Raijin7;
using TranslationHelper.Formats.Raijin7.eve;

namespace TranslationHelper.Projects
{
    class Raijin7Game : ProjectBase
    {
        public Raijin7Game() : base()
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

        internal override bool Check()
        {
            string dirPath = Path.GetDirectoryName(ProjectData.SelectedFilePath);
            return Path.GetExtension(ProjectData.SelectedFilePath) == ".exe"
                && Directory.Exists(Path.Combine(dirPath, "eve"))
                && Directory.Exists(Path.Combine(dirPath, "csv"))
                ;
        }

        internal override string Filters()
        {
            return GameExeFilter;
        }

        internal override string Name()
        {
            return "Raijin 7";
        }

        internal override bool Open()
        {
            return OpenSave();
        }

        internal override bool Save()
        {
            return OpenSave();
        }

        bool OpenSave()
        {
            var dirPath = Path.GetDirectoryName(ProjectData.SelectedFilePath);
            var ret = false;

            if (OpenSaveFilesBase(Path.Combine(dirPath, "eve"), new TXT(), "*.txt"))
                ret = true;
            if (OpenSaveFilesBase(Path.Combine(dirPath, "csv"), new CSV(), "*.csv"))
                ret = true;

            return ret;
        }

        internal override bool BakCreate()
        {
            return BackupRestorePaths(new[] { 
                Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "eve"),
                Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "csv")
            });
        }

        /// <summary>
        /// Will restore made buckup of project translating original files<br/>if any code exit here and buckup exists<br/>else will return false
        /// </summary>
        /// <returns></returns>
        internal override bool BakRestore()
        {
            return BackupRestorePaths(new[] {
                Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "eve"),
                Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "csv")
            }
            ,false);
        }

    }
}
