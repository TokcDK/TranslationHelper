using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TranslationHelper.Formats.Glitch_Pitch.Idol_Manager.Mod
{
    internal class Marketing_json_c
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("unlocked")]
        public bool Unlocked { get; set; }

        [JsonProperty("cost")]
        public double Cost { get; set; }

        [JsonProperty("physical")]
        public bool Physical { get; set; }

        [JsonProperty("digital_only", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DigitalOnly { get; set; }

        [JsonProperty("max_level", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxLevel { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("cost_stamina", NullValueHandling = NullValueHandling.Ignore)]
        public long? CostStamina { get; set; }

        [JsonProperty("girl_multiplier", NullValueHandling = NullValueHandling.Ignore)]
        public bool? GirlMultiplier { get; set; }

        [JsonProperty("individual_handshake", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IndividualHandshake { get; set; }

        [JsonProperty("appeal", NullValueHandling = NullValueHandling.Ignore)]
        public Appeal Appeal { get; set; }

        [JsonProperty("group_handshake", NullValueHandling = NullValueHandling.Ignore)]
        public bool? GroupHandshake { get; set; }

        [JsonProperty("minimal_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? MinimalCost { get; set; }
    }
    public partial class Appeal
    {
        [JsonProperty("hardcore", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Hardcore { get; set; }

        [JsonProperty("casual", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Casual { get; set; }

        [JsonProperty("adult", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Adult { get; set; }

        [JsonProperty("male", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Male { get; set; }

        [JsonProperty("teen", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Teen { get; set; }

        [JsonProperty("female", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Female { get; set; }
    }
}
