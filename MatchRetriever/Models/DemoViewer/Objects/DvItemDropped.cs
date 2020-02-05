using MatchEntities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvItemDropped
    {
        [JsonProperty]
        public int Time { get; set; }

        [JsonProperty]
        public EquipmentElement Equipment { get; set; }

        // ItemId is just for workaround as long as ItemPickedUp.Gift and .Buy are not working properly
        //[JsonProperty]
        public long ItemId { get; set; }
    }
}