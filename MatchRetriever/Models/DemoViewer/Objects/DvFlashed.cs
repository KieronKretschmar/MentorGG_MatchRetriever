using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvFlashed
    {
        [JsonProperty]
        public int Time { get; set; }

        [JsonProperty]
        public Vector3 VictimPos { get; set; }
        
        [JsonProperty]
        public int TimeFlashed { get; set; }
    }
}