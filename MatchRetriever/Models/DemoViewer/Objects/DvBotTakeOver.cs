using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvBotTakeOver
    {
        [JsonProperty]
        public string PlayerId { get; set; }

        [JsonProperty]
        public string BotId { get; set; }

        [JsonProperty]
        public short Round { get; set; }

        [JsonProperty]
        public int Time { get; set; }
    }
}