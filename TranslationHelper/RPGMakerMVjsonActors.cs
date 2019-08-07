﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using TranslationHelper;
//
//    var actors = RPGMakerMVjsonActors.FromJson(jsonString);

namespace TranslationHelper
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class RPGMakerMVjsonActors
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("battlerName")]
        public string BattlerName { get; set; }

        [JsonProperty("characterIndex")]
        public long CharacterIndex { get; set; }

        [JsonProperty("characterName")]
        public string CharacterName { get; set; }

        [JsonProperty("classId")]
        public long ClassId { get; set; }

        [JsonProperty("equips")]
        public List<long> Equips { get; set; }

        [JsonProperty("faceIndex")]
        public long FaceIndex { get; set; }

        [JsonProperty("faceName")]
        public string FaceName { get; set; }

        [JsonProperty("traits")]
        public List<Trait> Traits { get; set; }

        [JsonProperty("initialLevel")]
        public long InitialLevel { get; set; }

        [JsonProperty("maxLevel")]
        public long MaxLevel { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("profile")]
        public string Profile { get; set; }
    }

    public partial class Trait
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("dataId")]
        public long DataId { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public enum BattlerName { Actor18, Actor38, Empty };

    public enum CharacterName { Empty, GangGreen, JakeBoss, LadyAmelia, PublicBath, Sophie };

    public enum FaceName { Actor1, Empty };

    public enum Nickname { Detective, DetectiveApprentice, Doll, Empty, GangLeader, SmokeGenerator };

    public partial class RPGMakerMVjsonActors
    {
        public static List<RPGMakerMVjsonActors> FromJson(string json) => JsonConvert.DeserializeObject<List<RPGMakerMVjsonActors>>(json, TranslationHelper.Converter.Settings);
    }

    public static class RPGMakerMVjsonActorsTo
    {
        public static string ToJson(this List<RPGMakerMVjsonActors> self) => JsonConvert.SerializeObject(self, TranslationHelper.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
