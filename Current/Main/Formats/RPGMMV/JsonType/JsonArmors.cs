using RPGMVJsonParser;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonArmors : JsonItemTypeBase
    {
        public JsonArmors(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override string ItemTypeName => "Armor";

        protected override IItemType[] GetJsonData(string path) => Helper.LoadArmorsArray(path);
    }
}
