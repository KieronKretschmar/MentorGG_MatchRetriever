using MatchEntities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvRoundStats
    {
        [JsonProperty]
        public short Round;

        [JsonProperty]
        public StartingFaction WinnerTeam;

        [JsonProperty]
        public bool BombPlanted;

        [JsonProperty]
        public WinType WinType;

        [JsonProperty]
        public int RoundTime;

        [JsonProperty]
        public int StartTime;

        [JsonProperty]
        public int EndTime;

        [JsonProperty]
        public int RealEndTime;
    }
}