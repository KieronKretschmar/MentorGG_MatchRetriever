using MatchEntities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvWeaponFired
    {
        [JsonProperty]
        public int Time { get; set; }

        [JsonProperty]
        public string PlayerId { get; set; }

        [JsonProperty]
        public Vector3 PlayerPos { get; set; }

        [JsonProperty]
        public double PlayerView { get; set; }

        [JsonProperty]
        public EquipmentElement Weapon { get; set; }
    }
}