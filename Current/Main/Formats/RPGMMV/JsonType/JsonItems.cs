using RPGMVJsonParser;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonItems : JsonItemTypeBase
    {
        public JsonItems(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override IItemType[] GetJsonData(string path) => Helper.LoadItemsArray(path);
    }
}
