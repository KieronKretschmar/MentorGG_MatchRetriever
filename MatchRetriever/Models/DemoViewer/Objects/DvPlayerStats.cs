using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvPlayerStats
    {
        [JsonProperty]
        public string SteamName { get; set; }

        [JsonProperty]
        public string AvatarIcon { get; set; }
    }
}