using MatchEntities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvMatchStats
    {
        [JsonProperty]
        public DateTime MatchDate { get; set; }

        [JsonProperty]
        public string Map;

        [JsonProperty]
        public StartingFaction WinnerTeam { get; set; }

        [JsonProperty]
        public DvTeamMatchStats CtStarterTeamStats { get; set; }

        [JsonProperty]
        public DvTeamMatchStats TerroristStarterTeamStats { get; set; }

        [JsonProperty]
        public int RoundTimer { get; set; }

        [JsonProperty]
        public int BombTimer { get; set; }

        [JsonProperty]
        public string Source { get; set; }

        [JsonProperty]
        public double AVGRank { get; set; }
    }
}