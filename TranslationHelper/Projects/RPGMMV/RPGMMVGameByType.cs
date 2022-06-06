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
                typeof(JsonCommonEvents),
                typeof(JsonItems),
                typeof(JsonMap),
                typeof(JsonSkills),
                typeof(JsonStates),
                typeof(JsonSystem),
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
                "CommonEvents.json",
                "Items.json",
                "Map*.json",
                "Skills.json",
                "States.json",
                "System.json",
                "Troops.json",
                "Weapons.json",
            };
        }
    }
}

