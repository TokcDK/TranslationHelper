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
        [JsonProperty("left", NullValueHandling = NullValueHandling.Ignore)]
        public long Left { get; set; }

        [JsonProperty("top", NullValueHandling = NullValueHandling.Ignore)]
        public long Top { get; set; }

        [JsonProperty("first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty("unique", NullValueHandling = NullValueHandling.Ignore)]
        public bool Unique { get; set; }

        [JsonProperty("unique_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UniqueId { get; set; }

        [JsonProperty("age", NullValueHandling = NullValueHandling.Ignore)]
        public long Age { get; set; }

        [JsonProperty("trait", NullValueHandling = NullValueHandling.Ignore)]
        public object Trait { get; set; } // can be string or integer

        [JsonProperty("intro_message", NullValueHandling = NullValueHandling.Ignore)]
        public string IntroMessage { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty("ignore_body", NullValueHandling = NullValueHandling.Ignore)]
        public long[] IgnoreBody { get; set; }

        [JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
        public Stat[] Stats { get; set; }
    }

    public partial class Stat
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("val", NullValueHandling = NullValueHandling.Ignore)]
        public long Val { get; set; }

        [JsonProperty("potential", NullValueHandling = NullValueHandling.Ignore)]
        public long Potential { get; set; }
    }
}
