using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvWeaponReload
    {
        [JsonProperty]
        public int Time { get; set; }

        [JsonProperty]
        public short AmmoAfter { get; set; }
    }
}