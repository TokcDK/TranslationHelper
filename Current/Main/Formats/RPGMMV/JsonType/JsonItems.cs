using RPGMVJsonParser;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonItems : JsonItemTypeBase
    {
        public JsonItems(IFormatHost host) : base(host)
        {
        }

        protected override IItemType[] GetJsonData(string path) => Helper.LoadItemsArray(path);
    }
}
