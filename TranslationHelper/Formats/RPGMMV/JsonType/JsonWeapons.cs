using RPGMVJsonParser;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonWeapons : JsonItemTypeBase
    {
        protected override string ItemTypeName => "Weapon";

        protected override IItemType[] GetJsonData(string path) => Helper.LoadWeaponsArray(path);
    }
}
