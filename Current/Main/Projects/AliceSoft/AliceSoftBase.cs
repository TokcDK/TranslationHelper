using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.AliceSoft
{
    abstract class AliceSoftBase : ProjectBase
    {
        protected AliceSoftBase()
        {
        }

        internal override string ProjectTitlePrefix => "[AliceSoft]";

        internal override string ProjectDBFolderName => "AliceSoft";

        internal override bool BakCreate()
        {
            return ProjectToolsBackup.BackupRestorePaths(Directory.GetFiles(AppData.CurrentProject.SelectedGameDir, "*.ain"));
        }

        internal override bool BakRestore()
        {
            return ProjectToolsBackup.BackupRestorePaths(Directory.GetFiles(AppData.CurrentProject.SelectedGameDir, "*.ain"));
        }

        internal override string NewlineSymbol => "\\n";
    }
}
