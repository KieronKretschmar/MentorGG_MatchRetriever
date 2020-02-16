using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvPlayerRoundStats
    {
        [JsonProperty]
        public string PlayerId { get; set; }

        [JsonProperty]
        public int MoneyInitial { get; set; }

        [JsonProperty]
        public bool IsCT { get; set; }
    }
}