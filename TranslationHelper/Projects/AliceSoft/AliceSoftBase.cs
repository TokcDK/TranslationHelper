﻿using System;
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
        protected AliceSoftBase() : base()
        {
        }

        internal override string ProjectTitlePrefix()
        {
            return "[AliceSoft]";
        }

        internal override string ProjectFolderName()
        {
            return "AliceSoft";
        }

        internal override bool BakCreate()
        {
            return BackupRestorePaths(Directory.GetFiles(ProjectData.SelectedGameDir, "*.ain"));
        }

        internal override bool BakRestore()
        {
            return BackupRestorePaths(Directory.GetFiles(ProjectData.SelectedGameDir, "*.ain"));
        }

        internal override string NewlineSymbol => "\\n";
    }
}
