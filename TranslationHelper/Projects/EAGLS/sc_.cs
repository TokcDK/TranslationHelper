using System;
using System.Globalization;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.EAGLS.SCPACK;

namespace TranslationHelper.Projects.EAGLS
{
    class sc_ : ProjectBase
    {
        public sc_(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            return thDataWork.SPath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".TXT") && Path.GetFileName(thDataWork.SPath).StartsWith("sc_");
        }

        internal override string Filters()
        {
            return "EAGLS SCPACK sc_txt|sc_*.txt";
        }

        internal override bool Open()
        {
            thDataWork.FilePath = thDataWork.SPath;
            return new SC_TXT(thDataWork).Open();
        }

        internal override string Name()
        {
            return "EAGLS scenario ("+Path.GetFileNameWithoutExtension(thDataWork.SPath)+")";
        }

        internal override string ProjectFolderName()
        {
            return "EAGLS";
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
