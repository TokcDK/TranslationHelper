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
                typeof(JsonMap),
                typeof(JsonCommonEvents),
                typeof(JsonArmors),
                typeof(JsonItems),
                typeof(JsonWeapons),
            };
        }

        protected override string[] MVJsonFormatsMasks()
        {
            return new[] 
            { 
                "Map*.json",
                "CommonEvents.json",
                "Armors.json",
                "Items.json",
                "Weapons.json",
            };
        }
    }
}

