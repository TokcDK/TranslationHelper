﻿using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.EAGLS
{
    class EAGLSGame : EAGLSBase
    {
        public EAGLSGame(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "Script", "SCPACK.pak"))
                && File.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "Script", "SCPACK.idx"));
        }

        internal override string Filters()
        {
            return GameExeFilter;
        }

        internal override bool Open()
        {
            BakRestore();
            return UnpackSCPACK();
        }

        private bool UnpackSCPACK()
        {
            return PackUnpackFiles() && OpenFiles();
        }

        internal override string Name()
        {
            return ProjectTitlePrefix() + Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath));
        }

        internal override bool Save()
        {
            BakCreate();
            if (SaveFiles())
            {
                return PackFiles();
            }
            return false;
        }

        private bool PackFiles()
        {
            return PackUnpackFiles();
        }
    }
}
