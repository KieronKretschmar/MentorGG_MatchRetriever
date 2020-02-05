using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvFriends
    {
        [JsonProperty]
        public string SteamId { get; set; }

        [JsonProperty]
        public string FriendSteamId { get; set; }
    }
}