using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod
{
    internal class Params_json_c
    {
        [JsonProperty("left")]
        public long Left { get; set; }

        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("ignore_body")]
        public long[] IgnoreBody { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("unique")]
        public bool Unique { get; set; }

        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        [JsonProperty("age")]
        public long Age { get; set; }

        [JsonProperty("trait")]
        public object Trait { get; set; } // can be string or integer

        [JsonProperty("intro_message")]
        public string IntroMessage { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("stats")]
        public Stat[] Stats { get; set; }
    }

    public partial class Stat
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("val")]
        public long Val { get; set; }

        [JsonProperty("potential")]
        public long Potential { get; set; }
    }
}
