using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Formats.RPGMMV.JsonType;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.ProjectMenus;
using TranslationHelper.Projects.RPGMMV.Menus;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVGameByType : RPGMMVGame
    {
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

