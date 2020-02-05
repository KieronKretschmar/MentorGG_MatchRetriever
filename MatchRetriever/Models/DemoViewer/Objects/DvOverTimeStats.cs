using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvOverTimeStats
    {
        [JsonProperty]
        public byte StartT { get; set; }

        [JsonProperty]
        public byte StartCT { get; set; }

        [JsonProperty]
        public short NumRounds { get; set; }
    }
}