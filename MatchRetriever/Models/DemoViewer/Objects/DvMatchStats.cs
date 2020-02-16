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
        public byte WinnerTeam { get; set; }

        [JsonProperty]
        public short Score1 { get; set; }

        [JsonProperty]
        public short Score2 { get; set; }

        [JsonProperty]
        public short NumRoundsT1 { get; set; }

        [JsonProperty]
        public short NumRoundsCT1 { get; set; }

        [JsonProperty]
        public short NumRoundsT2 { get; set; }

        [JsonProperty]
        public short NumRoundsCT2 { get; set; }

        [JsonProperty]
        public int RoundTimer { get; set; }

        [JsonProperty]
        public int BombTimer { get; set; }

        [JsonProperty]
        public string Source;

        [JsonProperty]
        public double AVGRank;
    }
}