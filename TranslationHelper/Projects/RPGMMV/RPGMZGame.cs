using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Formats.RPGMMV.JsonType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Projects.RPGMMV
{
    internal class RPGMZGame: RPGMMVGameByType
    {
        protected override bool HasWWWDir => false;

        protected override List<Type> MVJsonFormats()
        {
            return new List<Type>()
            {
                typeof(JsonActors),
                typeof(JsonArmors),
                typeof(JsonClasses),
                typeof(JsonCommonEvents),
                typeof(JsonEnemies),
                typeof(JsonItems),
                typeof(JsonMap),
                typeof(JsonMapInfos),
                typeof(JsonSkills),
                typeof(JsonStates),
                typeof(JsonSystemMZ),
                typeof(JsonTilesets),
                typeof(JsonTroops),
                typeof(JsonWeapons),
            };
        }
    }
}
