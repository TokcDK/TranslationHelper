﻿using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.Raijin7;
using TranslationHelper.Formats.Raijin7.eve;

namespace TranslationHelper.Projects
{
    class Raijin7Game : ProjectBase
    {
        public Raijin7Game()
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
            string dirPath = Path.GetDirectoryName(AppData.SelectedFilePath);
            return Path.GetExtension(AppData.SelectedFilePath) == ".exe"
                && Directory.Exists(Path.Combine(dirPath, "eve"))
                && Directory.Exists(Path.Combine(dirPath, "csv"))
                ;
        }

        internal override string FileFilter => ProjectTools.GameExeFilter;

        public override string Name => "Raijin 7";

        public override bool Open()
        {
            return OpenSave();
        }

        public override bool Save()
        {
            return OpenSave();
        }

        bool OpenSave()
        {
            var dirPath = Path.GetDirectoryName(AppData.SelectedFilePath);
            var ret = false;

            if (ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(dirPath, "eve"), typeof(TXT), "*.txt"))
                ret = true;
            if (ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(dirPath, "csv"), typeof(CSV), "*.csv"))
                ret = true;

            return ret;
        }

        public override bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(new[] { 
                Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "eve"),
                Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "csv")
            });
        }

        /// <summary>
        /// Will restore made buckup of project translating original files<br/>if any code exit here and buckup exists<br/>else will return false
        /// </summary>
        /// <returns></returns>
        public override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(new[] {
                Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "eve"),
                Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "csv")
            }
            ,false);
        }

    }
}
