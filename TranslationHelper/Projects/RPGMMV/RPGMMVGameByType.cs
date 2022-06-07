using System;
using System.Collections.Generic;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Formats.RPGMMV.JsonType;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVGameByType : RPGMMVGame
    {
        protected override bool IsTypeExcluded(Type jsType)
        {
            return jsType == typeof(PLUGINS);// use by type version
        }

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
                typeof(JsonSystem),
                typeof(JsonTilesets),
                typeof(JsonTroops),
                typeof(JsonWeapons),
            };
        }

        protected override string[] MVJsonFormatsMasks()
        {
            return new[]
            {
                "Actors.json",
                "Armors.json",
                "Classes.json",
                "CommonEvents.json",
                "Enemies.json",
                "Items.json",
                "Map*.json",
                "MapInfos.json",
                "Skills.json",
                "States.json",
                "System.json",
                "Tilesets.json",
                "Troops.json",
                "Weapons.json",
            };
        }
    }
}

