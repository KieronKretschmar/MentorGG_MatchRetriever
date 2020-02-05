using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvPlayerMatchStats
    {
        [JsonProperty]
        public byte Team { get; set; }

        [JsonProperty]
        public short Kills { get; set; }

        [JsonProperty]
        public short Assists { get; set; }

        [JsonProperty]
        public short Deaths { get; set; }

        [JsonProperty]
        public short Score { get; set; }

        [JsonProperty]
        public short MVPs { get; set; }
    }
}