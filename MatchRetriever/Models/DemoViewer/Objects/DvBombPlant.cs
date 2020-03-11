using MatchEntities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvBombPlant
    {
        [JsonProperty]
        public int Time { get; set; }

        [JsonProperty]
        public string PlayerId { get; set; }

        [JsonProperty]
        public BombSite Site { get; set; }

        [JsonProperty]
        public Vector3 Pos { get; set; }
    }
}