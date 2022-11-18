using RPGMVJsonParser;
using System.IO;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonTypeBase : RPGMMVBase
    {
        public override string Extension => ".json";

        protected override void FileOpen()
        {
            string filePath = FilePath;
            foreach (var path in new[] { Path.GetFullPath(FilePath + @"\..\data.bak\" + Path.GetFileName(FilePath)), FilePath + ".bak" })
            {
                if (File.Exists(path))
                {
                    filePath = path;
                    break;
                }
            }

            JsonObject = ParseJson(filePath);
        }

        protected virtual object ParseJson(string path)
        {
            return null;
        }

        object JsonObject;

        protected override bool WriteFileData(string filePath = "")
        {
            if (!RET) return false;

            try { Helper.WriteJson(FilePath, JsonObject); } catch { return false; }

            return true;
        }
    }
}
