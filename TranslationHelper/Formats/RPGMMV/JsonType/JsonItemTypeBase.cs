using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGMVJsonParser;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal abstract class JsonItemTypeBase : JsonTypeBase
    {
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

                var name = item.Name;
                if (AddRowData(ref name, $"{ItemTypeName} #: {item.Id}") && ProjectData.SaveFileMode)
                {
                    item.Name = name;
                }

                var description = item.Description;
                if (AddRowData(ref description, $"{ItemTypeName} #: {item.Id}\r\nName: {name}") && ProjectData.SaveFileMode)
                {
                    item.Description = description;
                }
            }

            return data;
        }
    }
}
