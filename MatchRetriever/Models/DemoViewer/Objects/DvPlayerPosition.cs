using MatchEntities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvPlayerPosition
    {
        [JsonProperty]
        public int Time { get; set; }

        [JsonProperty]
        public Vector3 PlayerPos { get; set; }

        [JsonProperty]
        public double PlayerView { get; set; }

        [JsonProperty]
        public EquipmentElement Weapon { get; set; }
    }
}