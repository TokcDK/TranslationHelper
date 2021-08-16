using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class Xml : IDbSave
    {
        string IDbSave.Ext => "xml";

        string IDbSave.Description => "Uncompressed xml";

        Stream IDbSave.FileStreamMod(FileStream dbInputFileStream, bool isRead)
        {
            return dbInputFileStream;
        }
    }
}
