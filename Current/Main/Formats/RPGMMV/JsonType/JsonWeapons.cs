using RPGMVJsonParser;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonWeapons : JsonItemTypeBase
    {
        public JsonWeapons(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override string ItemTypeName => "Weapon";

        protected override IItemType[] GetJsonData(string path) => Helper.LoadWeaponsArray(path);
    }
}
