using RPGMVJsonParser;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonArmors : JsonItemTypeBase
    {
        protected override string ItemTypeName => "Armor";

        protected override IItemType[] GetJsonData(string path) => Helper.LoadArmorsArray(path);
    }
}
