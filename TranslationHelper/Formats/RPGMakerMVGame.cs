using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats
{
    internal class RPGMakerMVGame : IOpenFormats, ISaveFormats
    {
        public bool Open()
        {
            return false;
        }

        public bool Save()
        {
            return false;
        }
    }
}
