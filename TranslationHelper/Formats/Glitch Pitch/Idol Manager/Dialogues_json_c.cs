using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.Glitch_Pitch.Idol_Manager
{
    public partial class Dialogues_json_c
    {
        [JsonProperty("latestID")]
        public long LatestId { get; set; }

        [JsonProperty("parent")]
        public string Parent { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("music")]
        public string Music { get; set; }

        [JsonProperty("random")]
        public bool Random { get; set; }

        [JsonProperty("conditions")]
        public EffectElement[] Conditions { get; set; }

        [JsonProperty("actors")]
        public Actor[] Actors { get; set; }

        [JsonProperty("setup")]
        public Setup[] Setup { get; set; }

        [JsonProperty("scene_params")]
        public SceneParam[] SceneParams { get; set; }

        [JsonProperty("script")]
        public Script[] Scripts { get; set; }
    }
    public partial class SceneParam
    {
        [JsonProperty("parameter")]
        public string Parameter { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }
    }

    public partial class Actor
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("conditions")]
        public ActorCondition[] Conditions { get; set; }
    }

    public partial class ActorCondition
    {
        [JsonProperty("parameter")]
        public string Parameter { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }
    }

    public partial class EffectElement
    {
        [JsonProperty("parameter")]
        public string Parameter { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }
    }

    public partial class Script
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("val")]
        public string Val { get; set; }

        [JsonProperty("actor")]
        public string Actor { get; set; }

        [JsonProperty("effect")]
        public EffectElement[] Effect { get; set; }

        [JsonProperty("requirements")]
        public EffectElement[] Requirements { get; set; }

        [JsonProperty("script")]
        public Script[] Scripts { get; set; }
    }

    public partial class Setup
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("posX")]
        public long PosX { get; set; }

        [JsonProperty("facing")]
        public string Facing { get; set; }

        [JsonProperty("startHidden")]
        public bool StartHidden { get; set; }

        [JsonProperty("sprite", NullValueHandling = NullValueHandling.Ignore)]
        public string Sprite { get; set; }

        [JsonProperty("facingRight", NullValueHandling = NullValueHandling.Ignore)]
        public bool FacingRight { get; set; }
    }
}
