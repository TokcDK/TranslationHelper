using RPGMVJsonParser;
using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonArmors : JsonItemTypeBase
    {
        public JsonArmors(IFormatHost host) : base(host)
        {
        }

        protected override string ItemTypeName => "Armor";

        protected override IItemType[] GetJsonData(string path) => Helper.LoadArmorsArray(path);
    }
}
