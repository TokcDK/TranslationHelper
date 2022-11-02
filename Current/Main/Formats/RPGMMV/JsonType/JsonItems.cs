using RPGMVJsonParser;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonItems : JsonItemTypeBase
    {
        protected override IItemType[] GetJsonData(string path) => Helper.LoadItemsArray(path);
    }
}
