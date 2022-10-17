using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod
{
    public partial class Dialogues_json_c
    {
        [JsonProperty("latestID", NullValueHandling = NullValueHandling.Ignore)]
        public long LatestId { get; set; }

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        [JsonProperty("music", NullValueHandling = NullValueHandling.Ignore)]
        public string Music { get; set; }

        [JsonProperty("random", NullValueHandling = NullValueHandling.Ignore)]
        public bool Random { get; set; }

        [JsonProperty("conditions", NullValueHandling = NullValueHandling.Ignore)]
        public EffectElement[] Conditions { get; set; }

        [JsonProperty("actors", NullValueHandling = NullValueHandling.Ignore)]
        public Actor[] Actors { get; set; }

        [JsonProperty("setup", NullValueHandling = NullValueHandling.Ignore)]
        public Setup[] Setup { get; set; }

        [JsonProperty("scene_params", NullValueHandling = NullValueHandling.Ignore)]
        public SceneParam[] SceneParams { get; set; }

        [JsonProperty("script", NullValueHandling = NullValueHandling.Ignore)]
        public Script[] Scripts { get; set; }
    }
    public partial class SceneParam
    {
        [JsonProperty("parameter", NullValueHandling = NullValueHandling.Ignore)]
        public string Parameter { get; set; }

        [JsonProperty("formula", NullValueHandling = NullValueHandling.Ignore)]
        public string Formula { get; set; }
    }

    public partial class Actor
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }

        [JsonProperty("conditions", NullValueHandling = NullValueHandling.Ignore)]
        public ActorCondition[] Conditions { get; set; }
    }

    public partial class ActorCondition
    {
        [JsonProperty("parameter", NullValueHandling = NullValueHandling.Ignore)]
        public string Parameter { get; set; }

        [JsonProperty("formula", NullValueHandling = NullValueHandling.Ignore)]
        public string Formula { get; set; }
    }

    public partial class EffectElement
    {
        [JsonProperty("parameter", NullValueHandling = NullValueHandling.Ignore)]
        public string Parameter { get; set; }

        [JsonProperty("formula", NullValueHandling = NullValueHandling.Ignore)]
        public string Formula { get; set; }

        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }
    }

    public partial class Script
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("val", NullValueHandling = NullValueHandling.Ignore)]
        public string Val { get; set; }

        [JsonProperty("actor", NullValueHandling = NullValueHandling.Ignore)]
        public string Actor { get; set; }

        [JsonProperty("effect", NullValueHandling = NullValueHandling.Ignore)]
        public EffectElement[] Effect { get; set; }

        [JsonProperty("requirements", NullValueHandling = NullValueHandling.Ignore)]
        public EffectElement[] Requirements { get; set; }

        [JsonProperty("script", NullValueHandling = NullValueHandling.Ignore)]
        public Script[] Scripts { get; set; }
    }

    public partial class Setup
    {
        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }

        [JsonProperty("posX", NullValueHandling = NullValueHandling.Ignore)]
        public long PosX { get; set; }

        [JsonProperty("facing", NullValueHandling = NullValueHandling.Ignore)]
        public string Facing { get; set; }

        [JsonProperty("startHidden", NullValueHandling = NullValueHandling.Ignore)]
        public bool StartHidden { get; set; }

        [JsonProperty("sprite", NullValueHandling = NullValueHandling.Ignore)]
        public string Sprite { get; set; }

        [JsonProperty("facingRight", NullValueHandling = NullValueHandling.Ignore)]
        public bool FacingRight { get; set; }
    }
}
