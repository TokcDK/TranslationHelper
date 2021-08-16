using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.DBSaveFormats
{
    class XML : IDBSave
    {
        string IDBSave.Ext => "xml";

        string IDBSave.Description => "Uncompressed xml";

        Stream IDBSave.FileStreamMod(FileStream DBInputFileStream, bool IsRead)
        {
            return DBInputFileStream;
        }
    }
}
