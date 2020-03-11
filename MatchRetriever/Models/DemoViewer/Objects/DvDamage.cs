using MatchEntities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvDamage
    {
        [JsonProperty]
        public int Time { get; set; }

        [JsonProperty]
        public string PlayerId { get; set; }

        [JsonProperty]
        public Vector3 PlayerPos { get; set; }

        [JsonProperty]
        public string VictimId { get; set; }

        [JsonProperty]
        public Vector3 VictimPos { get; set; }

        [JsonProperty]
        public int AmountArmor { get; set; }

        [JsonProperty]
        public int AmountHealth { get; set; }

        [JsonProperty]
        public EquipmentElement Weapon { get; set; }
    }
}