using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGMVJsonParser;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal abstract class JsonItemTypeBase : JsonTypeBase
    {
        protected JsonItemTypeBase(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected virtual string ItemTypeName { get => "Item"; }

        protected abstract IItemType[] GetJsonData(string path);

        protected override object ParseJson(string path)
        {
            var data = GetJsonData(path);

            int armorsCount = data.Length;
            for (int a = 0; a < armorsCount; a++)
            {
                var item = data[a];
                if (item == null) continue;

                var s = item.Name;
                if (!string.IsNullOrWhiteSpace(s) && AddRowData(ref s, SaveFileMode ? "" : $"{ItemTypeName} #: {item.Id}\r\nNote: \"{item.Note}\"", isCheckInput: false) && SaveFileMode) item.Name = s;

                s = item.Description;
                if (!string.IsNullOrWhiteSpace(s) && AddRowData(ref s, SaveFileMode ? "" : $"{ItemTypeName} #: {item.Id}\r\nName: {item.Name}\r\nNote: \"{item.Note}\"", isCheckInput: false) && SaveFileMode) item.Description = s;

                s = item.Note;
                if (!string.IsNullOrWhiteSpace(s) && AddRowData(ref s, SaveFileMode ? "" : $"{ItemTypeName} #: {item.Id}\r\nName: {item.Name}", isCheckInput: false) && SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
