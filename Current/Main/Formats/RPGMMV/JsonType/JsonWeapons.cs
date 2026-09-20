using RPGMVJsonParser;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonWeapons : JsonItemTypeBase
    {
        public JsonWeapons(IFormatHost host) : base(host)
        {
        }

        protected override string ItemTypeName => "Weapon";

        protected override IItemType[] GetJsonData(string path) => Helper.LoadWeaponsArray(path);
    }
}
